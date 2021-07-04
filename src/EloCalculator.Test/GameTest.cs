namespace EloCalculator.Test
{
    using System;
    using EloCalculator;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Tests <see cref="Game"/>.
    /// </summary>
    [TestClass]
    public class GameTest
    {
        /// <summary>
        /// Tests <see cref="Game.ID"/>.
        /// </summary>
        [TestMethod]
        public void GameID()
        {
            Player white = new("Player 1");
            Player black = new("Player 2");
            Result result = Result.White;
            DateTime dateTime = DateTime.Now;
            bool rated = true;

            Game game = new(white, black, result, dateTime, rated);

            Assert.AreEqual(0, game.ID);

            GameDatabase.Games.Clear();
            PlayerDatabase.Players.Clear();
        }

        /// <summary>
        /// Tests <see cref="Game.White"/>.
        /// </summary>
        [TestMethod]
        public void GameWhite()
        {
            Player white = new("Player 1");
            Player black = new("Player 2");
            Result result = Result.White;
            DateTime dateTime = DateTime.Now;
            bool rated = true;

            Game game = new(white, black, result, dateTime, rated);

            Assert.AreEqual(white, game.White);

            GameDatabase.Games.Clear();
            PlayerDatabase.Players.Clear();
        }

        /// <summary>
        /// Tests <see cref="Game.WhiteID"/>.
        /// </summary>
        [TestMethod]
        public void GameWhiteID()
        {
            Player white = new("Player 1");
            Player black = new("Player 2");
            Result result = Result.White;
            DateTime dateTime = DateTime.Now;
            bool rated = true;

            Game game = new(white, black, result, dateTime, rated);

            Assert.AreEqual(white.ID, game.WhiteID);

            GameDatabase.Games.Clear();
            PlayerDatabase.Players.Clear();
        }

        /// <summary>
        /// Tests <see cref="Game.Black"/>.
        /// </summary>
        [TestMethod]
        public void GameBlack()
        {
            Player white = new("Player 1");
            Player black = new("Player 2");
            Result result = Result.White;
            DateTime dateTime = DateTime.Now;
            bool rated = true;

            Game game = new(white, black, result, dateTime, rated);

            Assert.AreEqual(black, game.Black);

            GameDatabase.Games.Clear();
            PlayerDatabase.Players.Clear();
        }

        /// <summary>
        /// Tests <see cref="Game.BlackID"/>.
        /// </summary>
        [TestMethod]
        public void GameBlackID()
        {
            Player white = new("Player 1");
            Player black = new("Player 2");
            Result result = Result.White;
            DateTime dateTime = DateTime.Now;
            bool rated = true;

            Game game = new(white, black, result, dateTime, rated);

            Assert.AreEqual(black.ID, game.BlackID);

            GameDatabase.Games.Clear();
            PlayerDatabase.Players.Clear();
        }

        /// <summary>
        /// Tests <see cref="Game.Result"/>.
        /// </summary>
        [TestMethod]
        public void GameResult()
        {
            Player white = new("Player 1");
            Player black = new("Player 2");
            Result result = Result.White;
            DateTime dateTime = DateTime.Now;
            bool rated = true;

            Game game = new(white, black, result, dateTime, rated);

            Assert.AreEqual(result, game.Result);

            GameDatabase.Games.Clear();
            PlayerDatabase.Players.Clear();
        }

        /// <summary>
        /// Tests <see cref="Game.DateTime"/>.
        /// </summary>
        [TestMethod]
        public void GameDateTime()
        {
            Player white = new("Player 1");
            Player black = new("Player 2");
            Result result = Result.White;
            DateTime dateTime = DateTime.Now;
            bool rated = true;

            Game game = new(white, black, result, dateTime, rated);

            Assert.AreEqual(dateTime, game.DateTime);

            GameDatabase.Games.Clear();
            PlayerDatabase.Players.Clear();
        }

        /// <summary>
        /// Tests <see cref="Game.Rated"/>.
        /// </summary>
        [TestMethod]
        public void GameRated()
        {
            Player white = new("Player 1");
            Player black = new("Player 2");
            Result result = Result.White;
            DateTime dateTime = DateTime.Now;
            bool rated = true;

            Game game = new(white, black, result, dateTime, rated);

            Assert.AreEqual(rated, game.Rated);

            GameDatabase.Games.Clear();
            PlayerDatabase.Players.Clear();
        }
    }
}
