namespace EloCalculator.Test
{
    using System;
    using System.Collections.Generic;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using EloCalculator;

    [TestClass]
    public class GameDatabaseTest
    {
        [TestMethod]
        public void GameDatabaseGames()
        {
            Game game = new(new("Player 1"), new("Player 2"), Result.White, DateTime.Now, true);

            List<Game> games = new()
            {
                game,
            };

            CollectionAssert.AreEquivalent(games, GameDatabase.Games);

            PlayerDatabase.Players.Clear();
            GameDatabase.Games.Clear();
        }

        [TestMethod]
        public void GameDatabaseLoadAndExport()
        {
            Game game = new(new("Player 1"), new("Player 2"), Result.White, DateTime.Now, true);

            GameDatabase.Export("game.json");

            GameDatabase.Games.Clear();

            GameDatabase.Load("game.json");

            Game actual = GameDatabase.Games[0];

            Game expected = new(PlayerDatabase.Players[0], PlayerDatabase.Players[1], Result.White, DateTime.Now, true);

            Assert.AreEqual(0, actual.ID);
            Assert.AreEqual(expected.White, actual.White);
            Assert.AreEqual(expected.WhiteID, actual.WhiteID);
            Assert.AreEqual(expected.Black, actual.Black);
            Assert.AreEqual(expected.BlackID, actual.BlackID);
            Assert.AreEqual(expected.Result, actual.Result);
            Assert.AreEqual(expected.Rated, actual.Rated);

            PlayerDatabase.Players.Clear();
            GameDatabase.Games.Clear();
        }
    }
}