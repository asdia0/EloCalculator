namespace EloCalculator
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;
    using System.IO;
    using System.Linq;

    /// <summary>
    /// Contains methods related to the Player database.
    /// </summary>
    public class PlayerDatabase : Database
    {
        /// <summary>
        /// The default rating 
        /// </summary>
        public static int defaultRating = 1000;

        /// <summary>
        /// A dictionary of <see cref="Player"/>s from the database.
        /// </summary>
        public static Dictionary<int, Player> Players = new Dictionary<int, Player>();

        /// <summary>
        /// Gets the ID of the newest record in the Player table.
        /// </summary>
        /// <returns>The ID of the newest record.</returns>
        public static int GetIDOfLastRecord()
        {
            using (SqlConnection connection = new SqlConnection(Settings.connectionString))
            {
                connection.Open();

                using (SqlCommand getID = new SqlCommand("SELECT TOP 1 Id FROM Player ORDER BY Id DESC", connection))
                {
                    return (int)getID.ExecuteScalar();
                }
            }
        }

        /// <summary>
        /// Adds a Player table if one does not exist.
        /// </summary>
        public static void ConfigureDB()
        {
            using (SqlConnection connection = new SqlConnection(Settings.connectionString))
            {
                connection.Open();

                if (!TableExists("Player"))
                {
                    // Add
                    using (SqlCommand addPlayer = new SqlCommand("CREATE TABLE [dbo].[Player] ([Id] INT IDENTITY(1, 1) NOT NULL, [Name] TEXT NOT NULL, [Rating] FLOAT(53) NOT NULL, [Wins] INT NOT NULL, [Losses] INT NOT NULL, [Draws]  INT NOT NULL, PRIMARY KEY CLUSTERED([Id] ASC));", connection))
                    {
                        addPlayer.ExecuteNonQuery();
                    }
                }
            }
        }

        /// <summary>
        /// Gets a list of all players sorted by <see cref="Player.Rating"/>. Highest rating = top of list.
        /// </summary>
        /// <returns></returns>
        public static List<(Player, double)> GetGlobalRankings()
        {
            List<(Player, double)> res = new List<(Player, double)>();

            foreach (Player player in Players.Values)
            {
                res.Add((player, player.Rating));
            }

            return res.OrderByDescending(t => t.Item2).ToList();
        }

        /// <summary>
        /// Remakes all records. Useful if you accidentally screw up or change a critical value in a <see cref="Game"/>.
        /// </summary>
        /// <param name="initialRating">The initial <see cref="Player.Rating"/> of <see cref="Player"/>s.</param>
        public static void RemakeDatabase()
        {
            ResetDatabase();
            LoadDatabase();
            UpdateStats();
        }

        /// <summary>
        /// Updates <see cref="Player.Rating"/>, <see cref="Player.Wins"/>, <see cref="Player.Losses"/> and <see cref="Player.Draws"/> from all records in <see cref="GameDatabase.Games"/>.
        /// </summary>
        public static void UpdateStats()
        {
            foreach (Game game in GameDatabase.Games.Values.ToList().OrderBy(i => i.DateTime).ToList())
            {
                if (game.Rated)
                {
                    // get player ratings
                    (double whiteRating, double blackRating) = GameDatabase.CalculateRating(game.White.Rating, game.Black.Rating, game.Result);

                    // update player ratings
                    game.White.Rating = whiteRating;
                    game.Black.Rating = blackRating;
                }

                // update player win-lose-draw
                game.White.UpdateWLD(Side.White, game.Result);
                game.Black.UpdateWLD(Side.Black, game.Result);
            }
        }

        /// <summary>
        /// Resets <see cref="Player.Rating"/>, <see cref="Player.Wins"/>, <see cref="Player.Losses"/> and <see cref="Player.Draws"/> in the database.
        /// </summary>
        /// <param name="initialRating">THe initial <see cref="Player.Rating"/> of <see cref="Player"/>s.</param>
        public static void ResetDatabase()
        {
            using (SqlConnection connection = new SqlConnection(Settings.connectionString))
            {
                connection.Open();

                for (int id = 1; id <= GetIDOfLastRecord(); id++)
                {
                    using (SqlCommand reset = new SqlCommand("UPDATE Player SET Rating=@Rating, Wins=0, Losses=0, Draws=0 WHERE Id=@ID", connection))
                    {
                        reset.Parameters.Add("@Rating", SqlDbType.Float).Value = defaultRating;
                        reset.Parameters.Add("@ID", SqlDbType.Int).Value = id;

                        reset.ExecuteNonQuery();
                    }
                }
            }
        }

        /// <summary>
        /// Loads all records from the database to <see cref="Players"/>.
        /// </summary>
        public static void LoadDatabase()
        {
            for (int id = 1; id <= GetIDOfLastRecord(); id++)
            {
                if (Players.ContainsKey(id))
                {
                    continue;
                }

                using (SqlConnection connection = new SqlConnection(Settings.connectionString))
                {
                    connection.Open();

                    using (SqlCommand getData = new SqlCommand("SELECT * FROM Player WHERE Id=@ID", connection))
                    {
                        getData.Parameters.Add("@ID", SqlDbType.Int).Value = id;

                        var rdr = getData.ExecuteReader();

                        while (rdr.Read())
                        {
                            string name = rdr["Name"].ToString();
                            double rating = (double)rdr["Rating"];
                            int wins = (int)rdr["Wins"];
                            int losses = (int)rdr["Losses"];
                            int draws = (int)rdr["Draws"];

                            Players.Add(id, new Player(id, name, rating, wins, losses, draws));
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
            List<string> records = File.ReadAllLines(path).ToList();

            records = records.Where(i => i != null || i != string.Empty).ToList();

            foreach (string name in records)
            {
                Console.WriteLine((name));
                new Player(name);
            }
        }

        /// <summary>
        /// Exports records to a tab-separated values (TSV) file.
        /// </summary>
        /// <param name="path">The path to write the records to.</param>
        public static void ExportDatabseToTSV(string path)
        {
            string content = string.Empty;

            using (SqlConnection connection = new SqlConnection(Settings.connectionString))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand("SELECT Name FROM Player", connection))
                {
                    var rdr = command.ExecuteReader();

                    while (rdr.Read())
                    {
                        content += $"{rdr["Name"]}\n";
                    }
                }
            }

            File.WriteAllText(path, content);
        }
    }
}
