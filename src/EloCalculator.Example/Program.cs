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
            Player jambon = new("Jambon");
            Player droplt = new("Droplt");
            Player jacob = new("Jacob");

            // Create games.
            // Game 1 is rated; it will affect the players' ratings.
            // Game 2 is unrated; it will not affect the player's ratings.
            Game game1 = new(jambon, droplt, Result.White, DateTime.Now, true);
            Game game2 = new(jambon, jacob, Result.Draw, DateTime.Now, false);

            // Print current standings.
            //
            // Prints:
            // (Jambon, 1020)
            // (Jacob, 1000)
            // (Droplt, 980)
            Console.WriteLine("Global leaderboard:");
            foreach (Player player in PlayerDatabase.GetLeaderboard())
            {
                Console.WriteLine((player.Name, player.Rating));
            }

            // Create a Swiss tournament (Danish variation).
            Tournament spookBoomer = new("SpookBoomer", TournamentType.Danish);

            // Create a round in the tournament.
            TournamentRound firstRound = new(spookBoomer);

            // Add the round to the tournament.
            spookBoomer.AddRound(firstRound);

            // Add games that were played during that round.
            firstRound.AddGames(new()
            {
                game1,
                game2,
            });

            // Print current tournament standings.
            //
            // Prints:
            // (Jambon, 1.5)
            // (Jacob, 0.5)
            // (Droplt, 0)
            Console.WriteLine($"{spookBoomer.Name} leaderboard:");
            foreach (TournamentPlayer boomer in spookBoomer.GetLeaderboard())
            {
                Console.WriteLine((boomer.Player.Name, boomer.Score));
            }
        }
    }
}
