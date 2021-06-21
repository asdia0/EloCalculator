namespace EloCalculator
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;
    using System.IO;
    using System.Linq;

    /// <summary>
    /// Contains methods related to the Game table.
    /// </summary>
    public class GameDatabase : Database
    {
        /// <summary>
        /// The k-Coefficient used to calculate <see cref="Player.Rating"/>. Larger = bigger changes.
        /// </summary>
        public static int kCoeff = 40;

        /// <summary>
        /// The benchmark coefficient used to calculate <see cref="Player.Rating"/>. Should be at 400 for "normal" calculations.
        /// </summary>
        public static int benchmarkCoeff = 400;

        /// <summary>
        /// A dictionary of <see cref="Game"/>s from the database.
        /// </summary>
        public static Dictionary<int, Game> Games = new Dictionary<int, Game>();

        /// <summary>
        /// Gets the ID of the newest record in the Game table.
        /// </summary>
        /// <returns>The ID of the newest record.</returns>
        public static int GetIDOfLastRecord()
        {
            using (SqlConnection connection = new SqlConnection(Settings.connectionString))
            {
                connection.Open();

                using (SqlCommand getID = new SqlCommand("SELECT TOP 1 Id FROM Game ORDER BY Id DESC", connection))
                {
                    return (int)getID.ExecuteScalar();
                }
            }
        }

        /// <summary>
        /// Adds a Game table if one does not exist.
        /// </summary>
        public static void ConfigureDB()
        {
            using (SqlConnection connection = new SqlConnection(Settings.connectionString))
            {
                connection.Open();

                if (!TableExists("Game"))
                {
                    // Add
                    using (SqlCommand addGame = new SqlCommand("CREATE TABLE [dbo].[Game] ([Id] INT IDENTITY(1, 1) NOT NULL, [White] INT NOT NULL, [Black] INT NOT NULL, [Result] BIT NULL, [DateTime] DATETIME NOT NULL, [Rated] BIT NOT NULL, [TournamentName] TEXT NULL, [TournamentRound] INT NULL, PRIMARY KEY CLUSTERED([Id] ASC));", connection))
                    {
                        addGame.ExecuteNonQuery();
                    }
                }
            }
        }

        /// <summary>
        /// Calculates the <see cref="Player.Rating"/> after a <see cref="Game"/>.
        /// </summary>
        /// <param name="whiteRating">White's initial <see cref="Player.Rating"/>.</param>
        /// <param name="blackRating">Black's initial <see cref="Player.Rating"/>.</param>
        /// <param name="result">The result of the <see cref="Game"/>.</param>
        /// <returns>(White's new <see cref="Player.Rating"/>, Black's new <see cref="Player.Rating"/>)</returns>
        public static (double, double) CalculateRating(double whiteRating, double blackRating, Result result)
        {
            double scoreA = 0, scoreB = 0;

            switch (result)
            {
                case (Result.White):
                    {
                        scoreA = 1;
                        scoreB = 0;
                        break;
                    }
                case (Result.Black):
                    {
                        scoreA = 0;
                        scoreB = 1;
                        break;
                    }
                case (Result.Draw):
                    {
                        scoreA = scoreB = 0.5;
                        break;
                    }
            }

            double expectedScoreA = 1 / (1 + Math.Pow(10, (blackRating - whiteRating) / benchmarkCoeff));

            double expectedScoreB = 1 / (1 + Math.Pow(10, (whiteRating - blackRating) / benchmarkCoeff));

            return (whiteRating + kCoeff * (scoreA - expectedScoreA), blackRating + kCoeff * (scoreB - expectedScoreB));
        }

        /// <summary>
        /// Loads all records from the database to <see cref="Games"/>.
        /// </summary>
        public static void LoadDatabase()
        {
            PlayerDatabase.LoadDatabase();

            for (int id = 1; id <= GetIDOfLastRecord(); id++)
            {
                if (Games.ContainsKey(id))
                {
                    continue;
                }

                using (SqlConnection connection = new SqlConnection(Settings.connectionString))
                {
                    connection.Open();

                    using (SqlCommand getData = new SqlCommand("SELECT * FROM Game WHERE Id=@ID", connection))
                    {
                        getData.Parameters.Add("@ID", SqlDbType.Int).Value = id;

                        var rdr = getData.ExecuteReader();

                        while (rdr.Read())
                        {
                            Player white = PlayerDatabase.Players[(int)rdr["White"]];
                            Player black = PlayerDatabase.Players[(int)rdr["Black"]];
                            Result result = ((rdr["Result"] == DBNull.Value) ? Result.Draw : ((bool)rdr["Result"] ? Result.White : Result.Black));
                            DateTime dateTime = (DateTime)rdr["DateTime"];
                            bool rated = (bool)rdr["Rated"];

                            if (rdr["TournamentName"] == DBNull.Value)
                            {
                                Games.Add(id, new Game(id, white, black, result, dateTime, rated));
                            }
                            else
                            {
                                Tournament tournament = TournamentDatabase.LoadTournament(rdr["TournamentName"].ToString());
                                TournamentRound round = tournament.Rounds.Where(i => i.Number == (int)rdr["TournamentRound"]).First();

                                Games.Add(id, new Game(id, white, black, result, dateTime, rated, tournament, round));
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Adds records to the database from a tab-separated values (TSV) file.
        /// </summary>
        /// <param name="path">The path to the TSV file.</param>
        public static void AddRecordsFromTSV(string path)
        {
            PlayerDatabase.LoadDatabase();

            List<string[]> records = File.ReadAllLines(path).ToList().Select(i => i.Split("\t")).ToList();

            foreach (string[] record in records)
            {
                Player white = PlayerDatabase.Players[int.Parse(record[0])];
                Player black = PlayerDatabase.Players[int.Parse(record[1])];
                Result result = (record[2] == "NULL") ? Result.Draw : ((record[2] == "1") ? Result.White : Result.Black);
                DateTime dateTime = DateTime.Parse(record[3]);
                bool rated = (record[4] == "1") ? true : false;
                Tournament tournament = TournamentDatabase.LoadTournament(record[5]);
                TournamentRound round = tournament.Rounds.Where(i => i.Number == int.Parse(record[6])).First();

                if (record.Length == 5)
                {
                    new Game(white, black, result, dateTime, rated);
                }
                else if (record.Length == 7)
                {
                    new Game(white, black, result, dateTime, rated, tournament, round);
                }
            }
        }
    }
}
