namespace EloCalculator
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.IO;

    using Newtonsoft.Json;

    public static class PlayerDatabase
    {
        public static double defaultRating = 1000;

        public static int kCoeff = 40;

        public static int benchmarkCoeff = 400;

        public static List<Player> Players = new List<Player>();

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

        public static string Export()
        {
            return JsonConvert.SerializeObject(Players, Formatting.Indented);
        }

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

            double expectedScoreA = 1 / (1 + Math.Pow(10, (blackRating - whiteRating) / benchmarkCoeff));

            double expectedScoreB = 1 / (1 + Math.Pow(10, (whiteRating - blackRating) / benchmarkCoeff));

            white.Rating = whiteRating + kCoeff * (scoreA - expectedScoreA);
            black.Rating = blackRating + kCoeff * (scoreB - expectedScoreB);
        }

        public static List<Player> GetRankings()
        {
            return Players.OrderByDescending(i => i.Rating).ToList();
        }
    }
}
