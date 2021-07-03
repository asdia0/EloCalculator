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

            //File.WriteAllText("game.json", GameDatabase.Export());
            //File.WriteAllText("player.json", PlayerDatabase.Export());
            //File.WriteAllText("tournament.json", TournamentDatabase.Export(0));

            foreach ((TournamentPlayer white, TournamentPlayer? black) in t.GetPairings())
            {
                if (black == null)
                {
                    Console.WriteLine((white.Player.Name, "BYE"));
                    continue;
                }

                Console.WriteLine((white.Player.Name, black.Player.Name));
            }
        }
    }
}
