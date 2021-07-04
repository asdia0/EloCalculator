namespace EloCalculator.Program
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.IO;

    using EloCalculator;

    class Program
    {
        /// <summary>
        /// An example project that uses EloCalculator.
        /// </summary>
        static void Main()
        {
            // Create players.
            Player player1 = new("Player 1");
            Player player2 = new("Player 2");
            Player player3 = new("Player 3");
            Player player4 = new("Player 4");

            // Create games.
            // Game 1 and 3 are rated; they will affect the players' ratings.
            // Game 2 is unrated; it will not affect the player's ratings.
            Game game1 = new(player1, player2, Result.White, DateTime.Now, true);
            Game game2 = new(player1, player3, Result.Draw, DateTime.Now, false);
            Game game3 = new(player1, player4, Result.Black, DateTime.Now, true);

            // Print current standings.
            //
            // Prints:
            // (Player 4, 1021.1500225556907)
            // (Player 3, 1000)
            // (Player 1, 998.8499774443093)
            // (Player 2, 980)
            foreach (Player player in PlayerDatabase.GetLeaderboard())
            {
                Console.WriteLine((player.Name, player.Rating));
            }

            // Create a Swiss tournament (Danish variation).
            Tournament danishTournament = new("Tournament 1", TournamentType.Danish);

            // Create a round in the tournament.
            TournamentRound firstRound = new(danishTournament);

            // Add the round to the tournament.
            danishTournament.AddRound(firstRound);

            // Add games that were played during that round.
            firstRound.AddGames(new()
            {
                game1,
                game3,
            });

            // Print current tournament standings.
            //
            // Prints:
            // (Player 4, 1, 1)
            // (Player 1, 1, 0)
            // (Player 2, 0, 0)
            foreach (TournamentPlayer danishPlayer in danishTournament.GetLeaderboard())
            {
                Console.WriteLine((danishPlayer.Player.Name, danishPlayer.Score, danishPlayer.Buchholz));
            }
        }
    }
}
