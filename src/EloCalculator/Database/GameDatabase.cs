namespace EloCalculator
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Newtonsoft.Json;

    public static class GameDatabase
    {
        public static List<Game> Games = new List<Game>();

        public static void Load(string path)
        {
            string text = File.ReadAllText(path);

            List<Game> games = JsonConvert.DeserializeObject<List<Game>>(text);

            foreach (Game game in games.ToList())
            {
                if (Games.Where(i => i.ID == game.ID).Any())
                {
                    games.Remove(game);
                }
            }

            Games.AddRange(games);
        }

        public static string Export()
        {
            return JsonConvert.SerializeObject(Games, Formatting.Indented);
        }
    }
}
