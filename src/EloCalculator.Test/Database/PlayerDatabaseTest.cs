namespace EloCalculator.Test
{
    using System;
    using System.Collections.Generic;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using EloCalculator;

    [TestClass]
    public class PlayerDatabaseTest
    {
        [TestMethod]
        public void PlayerDatabasePlayers()
        {
            Player player = new("Player 1");

            List<Player> players = new()
            {
                player,
            };

            CollectionAssert.AreEquivalent(players, PlayerDatabase.Players);

            PlayerDatabase.Players.Clear();
        }

        [TestMethod]
        public void PlayerDatabaseUpdateRatingsTrue()
        {
            Player player1 = new("Player 1");
            Player player2 = new("Player 2");

            _ = new Game(player1, player2, Result.White, DateTime.Now, true);

            Assert.AreEqual(1020, player1.Rating);
            Assert.AreEqual(980, player2.Rating);

            PlayerDatabase.Players.Clear();
            GameDatabase.Games.Clear();
        }

        [TestMethod]
        public void PlayerDatabaseUpdateRatingsFalse()
        {
            Player player1 = new("Player 1");
            Player player2 = new("Player 2");

            _ = new Game(player1, player2, Result.White, DateTime.Now, false);

            Assert.AreEqual(1000, player1.Rating);
            Assert.AreEqual(1000, player2.Rating);

            PlayerDatabase.Players.Clear();
            GameDatabase.Games.Clear();
        }

        [TestMethod]
        public void PlayerDatabaseLoadAndExport()
        {
            Player expected = new("Player 1");

            PlayerDatabase.Export("player.json");

            PlayerDatabase.Players.Clear();

            PlayerDatabase.Load("player.json");

            Player actual = PlayerDatabase.Players[0];

            Assert.AreEqual(expected.ID, actual.ID);
            Assert.AreEqual(expected.Name, actual.Name);
            CollectionAssert.AreEquivalent(expected.Games, actual.Games);
            CollectionAssert.AreEquivalent(expected.GamesID, actual.GamesID);
            Assert.AreEqual(expected.Wins, actual.Wins);
            Assert.AreEqual(expected.Draws, actual.Draws);
            Assert.AreEqual(expected.Losses, actual.Losses);

            PlayerDatabase.Players.Clear();
        }
    }
}