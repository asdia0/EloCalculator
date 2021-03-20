namespace EloCalculator
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class Game
    {
        public static int kCoeff = 40;

        public static int benchmarkCoeff = 400;

        public static List<string> validResults = new List<string>()
        {
            "1-0",
            "1/2-1/2",
            "0-1",
        };

        public Player player1;

        public Player player2;

        public string result;

        public string date;

        public Game(Player player1, Player player2, string result, string date)
        {
            if (!validResults.Contains(result))
            {
                throw new Exception("Game: Game result is invalid.");
            }

            this.player1 = player1;
            this.player2 = player2;
            this.result = result;
            this.date = date;

            UpdateGameRating(this);
            UpdatePlayerTitle(this.player1);
            UpdatePlayerTitle(this.player2);

            Program.gameList.Add(this);
        }

        public static void UpdateGameRating(Game game)
        {
            Player p1 = game.player1;
            Player p2 = game.player2;
            double scoreA;
            double scoreB;

            if (game.result == "1-0")
            {
                scoreA = 1;
                scoreB = 0;
                p1.wins++;
                p2.losses++;
            }
            else if (game.result == "0-1")
            {
                scoreA = 0;
                scoreB = 1;
                p1.losses++;
                p2.wins++;
            }
            else
            {
                scoreA = 0.5;
                scoreB = 0.5;
                p1.draws++;
                p2.draws++;
            }

            double expectedScoreA = 1 / (1 + Math.Pow(10, (p2.rating - p1.rating) / benchmarkCoeff));

            double expectedScoreB = 1 / (1 + Math.Pow(10, (p1.rating - p2.rating) / benchmarkCoeff));

            p1.rating += kCoeff * (scoreA - expectedScoreA);

            p2.rating += kCoeff * (scoreB - expectedScoreB);
        }

        public static void UpdatePlayerTitle(Player player)
        {
            List<string> titles = Program.ranks.Keys.ToList();
            List<double> ratings = Program.ranks.Values.ToList();

            for (int i = 0; i < Program.ranks.Count; i++)
            {
                string rank = titles[i];
                double rating = ratings[i];

                if (titles.IndexOf(player.title) < i && player.rating >= rating)
                {
                    player.title = rank;
                }
            }
        }
    }
}
