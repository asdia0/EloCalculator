﻿namespace EloCalculator
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
        private static List<Game> _Games = new();

        /// <summary>
        /// Gets or sets a list of all <see cref="Game"/>s loaded.
        /// </summary>
        public static List<Game> Games
        {
            get
            {
                _Games = _Games.OrderBy(i => i.ID).ToList();
                return _Games;
            }

            set
            {
                _Games = value;
            }
        }

        /// <summary>
        /// Clear <see cref="Games"/> and load <see cref="Game"/>s from a file as a JSON object.
        /// </summary>
        /// <param name="path">The path to the file to load from.</param>
        public static void Load(string path)
        {
            Games.Clear();

            string text = File.ReadAllText(path);

            List<Game> games = JsonConvert.DeserializeObject<List<Game>>(text);

            foreach (Game game in games)
            {
                if (!PlayerDatabase.Players.Contains(game.WhitePlayer))
                {
                    PlayerDatabase.Players.Add(game.WhitePlayer);
                }

                if (!PlayerDatabase.Players.Contains(game.BlackPlayer))
                {
                    PlayerDatabase.Players.Add(game.BlackPlayer);
                }
            }
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
