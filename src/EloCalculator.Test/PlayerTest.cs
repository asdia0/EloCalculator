namespace EloCalculator.Test
{
    using System;
    using System.Collections.Generic;
    using EloCalculator;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Tests <see cref="Player"/>.
    /// </summary>
    [TestClass]
    public class PlayerTest
    {
        /// <summary>
        /// Tests <see cref="Player.ID"/>.
        /// </summary>
        [TestMethod]
        public void PlayerID()
        {
            Player player = new("Player 1");

            Assert.AreEqual(0, player.ID);

            PlayerDatabase.Players.Clear();
        }

        /// <summary>
        /// Tests <see cref="Player.Name"/>.
        /// </summary>
        [TestMethod]
        public void PlayerName()
        {
            Player player = new("Player 1");

            Assert.AreEqual("Player 1", player.Name);

            PlayerDatabase.Players.Clear();
        }

        /// <summary>
        /// Tests <see cref="Player.Rating"/>.
        /// </summary>
        [TestMethod]
        public void PlayerRating()
        {
            Player player1 = new("Player 1");
            Player player2 = new("Player 2");

            _ = new Game(player1, player2, Result.White, DateTime.Now, true);

            Assert.AreEqual(1020, player1.Rating);

            GameDatabase.Games.Clear();
            PlayerDatabase.Players.Clear();
        }

        /// <summary>
        /// Tests <see cref="Player.Games"/>.
        /// </summary>
        [TestMethod]
        public void PlayerGames()
        {
            Player player1 = new("Player 1");
            Player player2 = new("Player 2");

            Game game = new(player1, player2, Result.White, DateTime.Now, true);

            List<Game> games = new()
            {
                game,
            };

            CollectionAssert.AreEquivalent(games, player1.Games);

            GameDatabase.Games.Clear();
            PlayerDatabase.Players.Clear();
        }

        /// <summary>
        /// Tests <see cref="Player.GamesID"/>.
        /// </summary>
        [TestMethod]
        public void PlayerGamesID()
        {
            Player player1 = new("Player 1");
            Player player2 = new("Player 2");

            _ = new Game(player1, player2, Result.White, DateTime.Now, true);

            List<int> games = new()
            {
                0,
            };

            CollectionAssert.AreEquivalent(games, player1.GamesID);

            GameDatabase.Games.Clear();
            PlayerDatabase.Players.Clear();
        }

        /// <summary>
        /// Tests <see cref="Player.Wins"/>.
        /// </summary>
        [TestMethod]
        public void PlayerWins()
        {
            Player player1 = new("Player 1");
            Player player2 = new("Player 2");

            _ = new Game(player1, player2, Result.White, DateTime.Now, true);

            Assert.AreEqual(1, player1.Wins);

            GameDatabase.Games.Clear();
            PlayerDatabase.Players.Clear();
        }

        /// <summary>
        /// Tests <see cref="Player.Draws"/>.
        /// </summary>
        [TestMethod]
        public void PlayerDraws()
        {
            Player player1 = new("Player 1");
            Player player2 = new("Player 2");

            _ = new Game(player1, player2, Result.Draw, DateTime.Now, true);

            Assert.AreEqual(1, player1.Draws);

            GameDatabase.Games.Clear();
            PlayerDatabase.Players.Clear();
        }

        /// <summary>
        /// Tests <see cref="Player.Losses"/>.
        /// </summary>
        [TestMethod]
        public void PlayerLosses()
        {
            Player player1 = new("Player 1");
            Player player2 = new("Player 2");

            _ = new Game(player1, player2, Result.Black, DateTime.Now, true);

            Assert.AreEqual(1, player1.Losses);

            GameDatabase.Games.Clear();
            PlayerDatabase.Players.Clear();
        }
    }
}