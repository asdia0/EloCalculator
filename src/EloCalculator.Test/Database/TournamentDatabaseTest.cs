namespace EloCalculator.Test
{
    using System.Collections.Generic;
    using EloCalculator;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Tests <see cref="TournamentDatabase"/>.
    /// </summary>
    [TestClass]
    public class TournamentDatabaseTest
    {
        /// <summary>
        /// Tests <see cref="TournamentDatabase.Tournaments"/>.
        /// </summary>
        [TestMethod]
        public void TournamentDatabaseTournaments()
        {
            Tournament tournament = new("Tournament 1", TournamentType.Arena);

            List<Tournament> tournaments = new()
            {
                tournament,
            };

            CollectionAssert.AreEquivalent(tournaments, TournamentDatabase.Tournaments);

            TournamentDatabase.Tournaments.Clear();
        }

        /// <summary>
        /// Tests <see cref="TournamentDatabase.Load(string)"/> and <see cref="TournamentDatabase.Export(string)"/>.
        /// </summary>
        [TestMethod]
        public void GameDatabaseLoadAndExport()
        {
            _ = new Tournament("Tournament 1", TournamentType.Arena);

            TournamentDatabase.Export("tournaments.json");

            TournamentDatabase.Tournaments.Clear();

            TournamentDatabase.Load("tournaments.json");

            Tournament actual = TournamentDatabase.Tournaments[0];

            Tournament expected = new("Tournament 1", TournamentType.Arena);

            Assert.AreEqual(0, actual.ID);
            Assert.AreEqual(expected.Name, actual.Name);
            Assert.AreEqual(expected.Type, actual.Type);
            CollectionAssert.AreEquivalent(expected.Players, actual.Players);
            CollectionAssert.AreEquivalent(expected.Rounds, actual.Rounds);

            TournamentDatabase.Tournaments.Clear();
        }
    }
}