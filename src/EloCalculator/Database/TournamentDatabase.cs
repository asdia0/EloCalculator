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
        private static List<Tournament> _Tournaments = new();

        /// <summary>
        /// Gets or sets a list of <see cref="Tournament"/>s loaded.
        /// </summary>
        public static List<Tournament> Tournaments
        {
            get
            {
                _Tournaments = _Tournaments.OrderBy(i => i.ID).ToList();
                return _Tournaments;
            }

            set
            {
                _Tournaments = value;
            }
        }

        /// <summary>
        /// Loads <see cref="Tournament"/>s from a file as a JSON object.
        /// </summary>
        /// <param name="path">The path to the file to load from.</param>
        public static void Load(string path)
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
    }
}
