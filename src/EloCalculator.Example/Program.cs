namespace EloCalculator.Program
{
    using System;
    using System.IO;

    using EloCalculator;

    class Program
    {
        /// <summary>
        /// An example project that uses EloCalculator.
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            Game g = new Game(new Player("Player 1"), new Player("Player 2"), Result.White, DateTime.Now, true);

            Tournament t = new Tournament("Tournament 1", TournamentType.RoundRobin);

            t.Rounds.Add(new TournamentRound(t));

            t.Rounds[0].AddGame(g);

            GameDatabase.Export("game.json");
            PlayerDatabase.Export("player.json");

            PlayerDatabase.Load("player.json");
            GameDatabase.Load("game.json");

            Console.WriteLine(GameDatabase.Games[0].White.Rating);
        }
    }
}
