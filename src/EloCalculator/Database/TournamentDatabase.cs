namespace EloCalculator
{
    using System.Collections.Generic;
    using System.Linq;
    using System.IO;

    using Newtonsoft.Json;

    public class TournamentDatabase
    {
        public static List<Tournament> Tournaments = new List<Tournament>();

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

        public static string Export()
        {
            return JsonConvert.SerializeObject(Tournaments, Formatting.Indented);
        }

        public static string Export(int id)
        {
            return JsonConvert.SerializeObject(Tournaments.Where(i => i.ID == id).FirstOrDefault(), Formatting.Indented);
        }
    }
}
