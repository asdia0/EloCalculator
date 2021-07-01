namespace EloCalculator
{
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;
    using System.Linq;

    /// <summary>
    /// Contains methods related to the Tournament tables.
    /// </summary>
    public class TournamentDatabase : Database
    {
        /// <summary>
        /// A list of <see cref="Tournament"/>s.
        /// </summary>
        public static List<Tournament> Tournaments = new List<Tournament>();

        /// <summary>
        /// Loads and returns a tournament table as a <see cref="Tournament"/> object.
        /// </summary>
        /// <param name="name">The name of the tournament table.</param>
        /// <returns>A <see cref="Tournament"/>.</returns>
        public static Tournament LoadTournament(string name)
        {
            if (Tournaments.Where(i => i.Name == name).Any())
            {
                return Tournaments.Where(i => i.Name == name).First();
            }

            PlayerDatabase.LoadDatabase();

            Tournament tournament = new Tournament(name);

            using (SqlConnection connection = new SqlConnection(Settings.connectionString))
            {
                connection.Open();

                using (SqlCommand getPlayerData = new SqlCommand($"SELECT * FROM [{name}]", connection))
                {
                    var rdr = getPlayerData.ExecuteReader();

                    while (rdr.Read())
                    {
                        Player player = PlayerDatabase.Players[(int)rdr["Player"]];
                        bool active = (bool)rdr["Active"];
                        double score = (double)rdr["Score"];
                        double SB = (double)rdr["Sonneborn-Berger"];
                        double BH = (double)rdr["Buchholz"];

                        tournament.Players.Add(new TournamentPlayer(tournament, player, active, score, SB, BH));
                    }
                }

                using (SqlCommand getRoundData = new SqlCommand("SELECT * FROM Game WHERE TournamentName LIKE @TName", connection))
                {
                    getRoundData.Parameters.Add("@TName", SqlDbType.Text).Value = name;

                    var rdr = getRoundData.ExecuteReader();

                    while (rdr.Read())
                    {
                        int id = (int)rdr["Id"];
                        int number = (int)rdr["TournamentRound"];

                        if (tournament.Rounds.Where(i => i.Number == number).Any())
                        {
                            tournament.Rounds.Where(i => i.Number == number).First().AddGame(GameDatabase.Games[id]);
                        }
                        else
                        {
                            tournament.Rounds.Add(new TournamentRound(tournament, number));
                            tournament.Rounds.Where(i => i.Number == number).First().AddGame(GameDatabase.Games[id]);
                        }
                    }

                    rdr.Close();
                }
            }

            return tournament;
        }
    }
}
