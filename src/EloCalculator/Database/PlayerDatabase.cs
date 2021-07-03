namespace EloCalculator
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Newtonsoft.Json;

    /// <summary>
    /// Contains methods related to <see cref="Player"/>s.
    /// </summary>
    public static class PlayerDatabase
    {
        /// <summary>
        /// <see cref="Player"/>s' initial rating.
        /// </summary>
        public static double DefaultRating = 1000;

        /// <summary>
        /// The largest change that can occur to a <see cref="Player"/>'s rating.
        /// </summary>
        public static int KCoefficient = 40;

        /// <summary>
        /// A list of all players loaded.
        /// </summary>
        public static List<Player> Players = new List<Player>();

        /// <summary>
        /// Load <see cref="Player"/>s from a file as a JSON object.
        /// </summary>
        /// <param name="path">The path to the file to load from.</param>
        public static void Load(string path)
        {
            string text = File.ReadAllText(path);

            List<Player> players = JsonConvert.DeserializeObject<List<Player>>(text);

            foreach (Player player in players.ToList())
            {
                if (Players.Where(i => i.ID == player.ID).Any())
                {
                    players.Remove(player);
                }
            }

            Players.AddRange(players);
        }

        /// <summary>
        /// Exports <see cref="Players"/> to a file as a JSON object.
        /// </summary>
        /// <param name="path">The path to the file to export to.</param>
        public static void Export(string path)
        {
            File.WriteAllText(path, JsonConvert.SerializeObject(Players, Formatting.Indented));
        }

        /// <summary>
        /// Updates the ratings of the <paramref name="game"/>'s players.
        /// </summary>
        /// <param name="game">The game to get the players to update from.</param>
        public static void UpdateRatings(Game game)
        {
            Player white = game.White;
            Player black = game.Black;

            double whiteRating = white.Rating;
            double blackRating = black.Rating;

            Result result = game.Result;

            double scoreA = 0, scoreB = 0;

            switch (result)
            {
                case Result.White:
                    {
                        scoreA = 1;
                        scoreB = 0;
                        break;
                    }

                case Result.Black:
                    {
                        scoreA = 0;
                        scoreB = 1;
                        break;
                    }

                case Result.Draw:
                    {
                        scoreA = scoreB = 0.5;
                        break;
                    }
            }

            double expectedScoreA = 1 / (1 + Math.Pow(10, (blackRating - whiteRating) / 400));

            double expectedScoreB = 1 / (1 + Math.Pow(10, (whiteRating - blackRating) / 400));

            white.Rating = whiteRating + (KCoefficient * (scoreA - expectedScoreA));
            black.Rating = blackRating + (KCoefficient * (scoreB - expectedScoreB));
        }

        /// <summary>
        /// Gets a leaderboard of all players in <see cref="Players"/> based upon <see cref="Player.Rating"/>.
        /// </summary>
        /// <returns>A list of <see cref="Player"/> sorted from highest rating to lowest.</returns>
        public static List<Player> GetLeaderboard()
        {
            return Players.OrderByDescending(i => i.Rating).ToList();
        }
    }
}
