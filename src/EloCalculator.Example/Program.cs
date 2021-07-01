namespace EloCalculator.Program
{
    using System;
    using System.Linq;
    using EloCalculator;

    class Program
    {
        /// <summary>
        /// An example project that uses EloCalculator.
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            // Set up connection string.
            Settings.connectionString = "Your-Connection-String";

            // Configure main databases.
            PlayerDatabase.ConfigureDB();
            GameDatabase.ConfigureDB();

            // Add players and games.
            PlayerDatabase.AddRecordsFromTSV("player.tsv");
            GameDatabase.AddRecordsFromTSV("game.tsv");

            // Creates a game and adds it to the database.
            new Game(new Player("Player 1"), new Player("Player 2"), Result.White, DateTime.Now, true);
            
            // Exports all games into gameOutput.tsv
            GameDatabase.ExportDatabseToTSV("gameOutput.tsv");
        }
    }
}
