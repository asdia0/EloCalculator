namespace EloCalculator
{
    using System;

    public class Game
    {
        public static int kCoeff = 40;

        public static int benchmarkCoeff = 400;

        public Player player1;

        public Player player2;

        public Result result;

        public string date;

        public Game(Player player1, Player player2, Result result, string date)
        {
            this.player1 = player1;
            this.player2 = player2;
            this.result = result;
            this.date = date;

            this.UpdateGameRating();

            Program.gameList.Add(this);
        }

        public void UpdateGameRating()
        {
            Player p1 = this.player1;
            Player p2 = this.player2;
            double scoreA = 0, scoreB = 0;

            switch (this.result)
            {
                case (Result.WhiteWins):
                {
                    scoreA = 1;
                    scoreB = 0;
                    p1.wins++;
                    p2.losses++;
                    break;
                }
                case (Result.BlackWins):
                {
                    scoreA = 0;
                    scoreB = 1;
                    p1.losses++;
                    p2.wins++;
                    break;
                }
                case (Result.Draw):
                {
                    scoreA = 0.5;
                    scoreB = 0.5;
                    p1.draws++;
                    p2.draws++;
                    break;
                }
            }

            double expectedScoreA = 1 / (1 + Math.Pow(10, (p2.rating - p1.rating) / benchmarkCoeff));

            double expectedScoreB = 1 / (1 + Math.Pow(10, (p1.rating - p2.rating) / benchmarkCoeff));

            p1.rating += kCoeff * (scoreA - expectedScoreA);

            p2.rating += kCoeff * (scoreB - expectedScoreB);
        }
    }
}
