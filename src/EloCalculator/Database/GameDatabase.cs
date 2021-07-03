namespace EloCalculator
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Newtonsoft.Json;

    /// <summary>
    /// Contains methods related to <see cref="Game"/>s.
    /// </summary>
    public static class GameDatabase
    {
        /// <summary>
        /// A list of all games loaded.
        /// </summary>
        public static List<Game> Games = new List<Game>();

        /// <summary>
        /// Load <see cref="Game"/>s from a file as a JSON object.
        /// </summary>
        /// <param name="path">The path to the file to load from.</param>
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

        /// <summary>
        /// Exports <see cref="Games"/> to a file as a JSON object.
        /// </summary>
        /// <param name="path">The path to the file to export to.</param>
        public static void Export(string path)
        {
            File.WriteAllText(path, JsonConvert.SerializeObject(Games, Formatting.Indented));
        }
    }
}
