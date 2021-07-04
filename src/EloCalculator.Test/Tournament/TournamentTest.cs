namespace EloCalculator.Test
{
    using System.Collections.Generic;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using EloCalculator;

    [TestClass]
    public class TournamentTest
    {
        [TestMethod]
        public void TournamentID()
        {
            Tournament tournament = new("Tournament 1", TournamentType.RoundRobin);

            Assert.AreEqual(0, tournament.ID);

            TournamentDatabase.Tournaments.Clear();
        }

        [TestMethod]
        public void TournamentName()
        {
            Tournament tournament = new("Tournament 1", TournamentType.RoundRobin);

            Assert.AreEqual("Tournament 1", tournament.Name);

            TournamentDatabase.Tournaments.Clear();
        }

        [TestMethod]
        public void TournamentTypeT()
        {
            Tournament tournament = new("Tournament 1", TournamentType.RoundRobin);

            Assert.AreEqual(TournamentType.RoundRobin, tournament.Type);

            TournamentDatabase.Tournaments.Clear();
        }

        [TestMethod]
        public void TournamentPlayers()
        {
            Tournament tournament = new("Tournament 1", TournamentType.RoundRobin);

            List<TournamentPlayer> players = new()
            {
                new TournamentPlayer(tournament, new("Player 1")),
                new TournamentPlayer(tournament, new("Player 2")),
            };

            tournament.AddPlayers(players);

            CollectionAssert.AreEquivalent(players, tournament.Players);

            TournamentDatabase.Tournaments.Clear();
        }

        [TestMethod]
        public void TournamentRounds()
        {
            Tournament tournament = new("Tournament 1", TournamentType.RoundRobin);

            List<TournamentRound> rounds = new()
            {
                new(tournament),
            };

            tournament.Rounds.AddRange(rounds);

            CollectionAssert.AreEquivalent(rounds, tournament.Rounds);

            TournamentDatabase.Tournaments.Clear();
        }
    }
}
