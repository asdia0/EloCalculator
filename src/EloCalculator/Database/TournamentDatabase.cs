namespace EloCalculator
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Newtonsoft.Json;

    /// <summary>
    /// Contains methods related to <see cref="Tournament"/>s.
    /// </summary>
    public class TournamentDatabase
    {
        /// <summary>
        /// A list of all tournaments loaded.
        /// </summary>
        public static List<Tournament> Tournaments = new List<Tournament>();

        /// <summary>
        /// Loads a <see cref="Tournament"/>s from a file as a JSON object.
        /// </summary>
        /// <param name="path">The path to the file to load from.</param>
        public static void LoadTournament(string path)
        {
            string text = File.ReadAllText(path);

            List<Tournament> tournaments = JsonConvert.DeserializeObject<List<Tournament>>(text);

            foreach (Tournament tournament in tournaments.ToList())
            {
                if (Tournaments.Where(i => i.ID == tournament.ID).Any())
                {
                    tournaments.Remove(tournament);
                }
            }

            Tournaments.AddRange(tournaments);
        }

        /// <summary>
        /// Exports <see cref="Tournaments"/> to a file as a JSON object.
        /// </summary>
        /// <param name="path">The path to the file to export to.</param>
        public static void Export(string path)
        {
            File.WriteAllText(path, JsonConvert.SerializeObject(Tournaments, Formatting.Indented));
        }

        /// <summary>
        /// Exports a <see cref="Tournament"/> to a file as a JSON object.
        /// </summary>
        /// <param name="path">The path to the file to export to.</param>
        /// <param name="id">The unique identification number of the tournament to export.</param>
        public static void Export(string path, int id)
        {
            File.WriteAllText(path, JsonConvert.SerializeObject(Tournaments.Where(i => i.ID == id).FirstOrDefault(), Formatting.Indented));
        }
    }
}
