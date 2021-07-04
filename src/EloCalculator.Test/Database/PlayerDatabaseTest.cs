namespace EloCalculator.Test
{
    using System;
    using System.Collections.Generic;
    using EloCalculator;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Tests <see cref="PlayerDatabase"/>.
    /// </summary>
    [TestClass]
    public class PlayerDatabaseTest
    {
        /// <summary>
        /// Tests <see cref="PlayerDatabase.Players"/>.
        /// </summary>
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

        /// <summary>
        /// Tests <see cref="PlayerDatabase.UpdateRatings(Game)"/>.
        /// </summary>
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

        /// <summary>
        /// Test <see cref="PlayerDatabase.UpdateRatings(Game)"/>.
        /// </summary>
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

        /// <summary>
        /// Tests <see cref="PlayerDatabase.Load(string)"/> and <see cref="PlayerDatabase.Export(string)"/>.
        /// </summary>
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