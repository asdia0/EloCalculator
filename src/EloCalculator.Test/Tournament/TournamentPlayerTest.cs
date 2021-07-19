namespace EloCalculator.Test
{
    using System;
    using System.Collections.Generic;
    using EloCalculator;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Tests <see cref="TournamentPlayer"/>.
    /// </summary>
    [TestClass]
    public class TournamentPlayerTest
    {
        /// <summary>
        /// Tests <see cref="TournamentPlayer.ID"/>.
        /// </summary>
        [TestMethod]
        public void TournamentPlayerID()
        {
            Tournament tournament = new("Tournament 1", TournamentType.Arena);

            Player player = new("Player 1");

            TournamentPlayer tplayer = new(tournament, player);

            Assert.AreEqual(0, tplayer.ID);

            TournamentDatabase.Tournaments.Clear();
            PlayerDatabase.Players.Clear();
        }

        /// <summary>
        /// Tests <see cref="TournamentPlayer.Active"/>.
        /// </summary>
        [TestMethod]
        public void TournamentPlayerActiveTrue()
        {
            Tournament tournament = new("Tournament 1", TournamentType.Arena);

            Player player = new("Player 1");

            TournamentPlayer tplayer = new(tournament, player);

            Assert.AreEqual(true, tplayer.Active);

            TournamentDatabase.Tournaments.Clear();
            PlayerDatabase.Players.Clear();
        }

        /// <summary>
        /// Tests <see cref="TournamentPlayer.Active"/>.
        /// </summary>
        [TestMethod]
        public void TournamentPlayerActiveFalse()
        {
            Tournament tournament = new("Tournament 1", TournamentType.Arena);

            Player player = new("Player 1");

            TournamentPlayer tplayer = new(tournament, player);

            tplayer.Active = false;

            Assert.AreEqual(false, tplayer.Active);

            TournamentDatabase.Tournaments.Clear();
            PlayerDatabase.Players.Clear();
        }

        /// <summary>
        /// Tests <see cref="TournamentPlayer.Player"/>.
        /// </summary>
        [TestMethod]
        public void TournamentPlayerPlayer()
        {
            Tournament tournament = new("Tournament 1", TournamentType.Arena);

            Player player = new("Player 1");

            TournamentPlayer tplayer = new(tournament, player);

            Assert.AreEqual(player, tplayer.Player);

            TournamentDatabase.Tournaments.Clear();
            PlayerDatabase.Players.Clear();
        }

        /// <summary>
        /// Tests <see cref="TournamentPlayer.PlayerID"/>.
        /// </summary>
        [TestMethod]
        public void TournamentPlayerPlayerID()
        {
            Tournament tournament = new("Tournament 1", TournamentType.Arena);

            Player player = new("Player 1");

            TournamentPlayer tplayer = new(tournament, player);

            Assert.AreEqual(player.ID, tplayer.PlayerID);

            TournamentDatabase.Tournaments.Clear();
            PlayerDatabase.Players.Clear();
        }

        /// <summary>
        /// Tests <see cref="TournamentPlayer.Games"/>.
        /// </summary>
        [TestMethod]
        public void TournamentPlayerGames()
        {
            Tournament tournament = new("Tournament 1", TournamentType.Arena);

            Player player1 = new("Player 1");

            TournamentPlayer tplayer = new(tournament, player1);

            CollectionAssert.AreEquivalent(new List<Game>() { }, tplayer.Games);

            TournamentDatabase.Tournaments.Clear();
            PlayerDatabase.Players.Clear();
        }

        /// <summary>
        /// Tests <see cref="TournamentPlayer.GamesID"/>.
        /// </summary>
        [TestMethod]
        public void TournamentPlayerGamesID()
        {
            Tournament tournament = new("Tournament 1", TournamentType.Arena);

            Player player1 = new("Player 1");

            TournamentPlayer tplayer = new(tournament, player1);

            CollectionAssert.AreEquivalent(new List<int>() { }, tplayer.GamesID);

            TournamentDatabase.Tournaments.Clear();
            PlayerDatabase.Players.Clear();
        }

        /// <summary>
        /// Tests <see cref="TournamentPlayer.Score"/>.
        /// </summary>
        [TestMethod]
        public void TournamentPlayerScore()
        {
            Tournament tournament = new("Tournament 1", TournamentType.Arena);

            Player player1 = new("Player 1");
            Player player2 = new("Player 2");

            TournamentPlayer tplayer1 = new(tournament, player1);
            _ = new TournamentPlayer(tournament, player2);

            List<Game> games = new()
            {
                new(player1, player2, Result.White, DateTime.Now, true),
                new(player1, player2, Result.Draw, DateTime.Now, true),
                new(player1, player2, Result.Black, DateTime.Now, true),
            };

            TournamentRound round = new(tournament);

            round.AddGames(games);

            Assert.AreEqual(1.5, tplayer1.Score);

            TournamentDatabase.Tournaments.Clear();
            PlayerDatabase.Players.Clear();
        }

        /// <summary>
        /// Tests <see cref="TournamentPlayer.PerformanceRating"/>.
        /// </summary>
        [TestMethod]
        public void TournamentPlayerPerformanceRating()
        {
            Tournament tournament = new("Tournament 1", TournamentType.Arena);

            Player player1 = new("Player 1");
            Player player2 = new("Player 2");

            TournamentPlayer tplayer1 = new(tournament, player1);
            _ = new TournamentPlayer(tournament, player2);

            List<Game> games = new()
            {
                new(player1, player2, Result.White, DateTime.Now, true),
                new(player1, player2, Result.Draw, DateTime.Now, true),
                new(player1, player2, Result.Black, DateTime.Now, true),
            };

            _ = new TournamentRound(tournament);

            tournament.Rounds[0].AddGames(games);

            Assert.AreEqual(1004.3240891001137, tplayer1.PerformanceRating);

            TournamentDatabase.Tournaments.Clear();
            PlayerDatabase.Players.Clear();
        }

        /// <summary>
        /// Tests <see cref="TournamentPlayer.SonnebornBerger"/>.
        /// </summary>
        [TestMethod]
        public void TournamentPlayerSonnebornBerger()
        {
            Tournament tournament = new("Tournament 1", TournamentType.Danish);

            Player player1 = new("Player 1");
            Player player2 = new("Player 2");

            TournamentPlayer tplayer1 = new(tournament, player1);
            _ = new TournamentPlayer(tournament, player2);

            List<Game> games = new()
            {
                new(player1, player2, Result.White, DateTime.Now, true),
                new(player1, player2, Result.Draw, DateTime.Now, true),
                new(player1, player2, Result.Black, DateTime.Now, true),
            };

            _ = new TournamentRound(tournament);

            tournament.Rounds[0].AddGames(games);

            Assert.AreEqual(2.25, tplayer1.SonnebornBerger);

            TournamentDatabase.Tournaments.Clear();
            PlayerDatabase.Players.Clear();
        }

        /// <summary>
        /// Tests <see cref="TournamentPlayer.Buchholz"/>.
        /// </summary>
        [TestMethod]
        public void TournamentPlayerBuchholz()
        {
            Tournament tournament = new("Tournament 1", TournamentType.Danish);

            Player player1 = new("Player 1");
            Player player2 = new("Player 2");

            TournamentPlayer tplayer1 = new(tournament, player1);
            _ = new TournamentPlayer(tournament, player2);

            List<Game> games = new()
            {
                new(player1, player2, Result.White, DateTime.Now, true),
                new(player1, player2, Result.Draw, DateTime.Now, true),
                new(player1, player2, Result.Black, DateTime.Now, true),
            };

            _ = new TournamentRound(tournament);

            tournament.Rounds[0].AddGames(games);

            Assert.AreEqual(3, tplayer1.Buchholz);

            TournamentDatabase.Tournaments.Clear();
            PlayerDatabase.Players.Clear();
        }

        /// <summary>
        /// Tests <see cref="TournamentPlayer.MedianBuchholz"/>.
        /// </summary>
        [TestMethod]
        public void TournamentPlayerMedianBuchholz()
        {
            Tournament tournament = new("Tournament 1", TournamentType.Danish);

            Player player1 = new("Player 1");
            Player player2 = new("Player 2");

            TournamentPlayer tplayer1 = new(tournament, player1);
            _ = new TournamentPlayer(tournament, player2);

            List<Game> games = new()
            {
                new(player1, player2, Result.White, DateTime.Now, true),
                new(player1, player2, Result.Draw, DateTime.Now, true),
                new(player1, player2, Result.Black, DateTime.Now, true),
            };

            _ = new TournamentRound(tournament);

            tournament.Rounds[0].AddGames(games);

            Assert.AreEqual(1.5, tplayer1.MedianBuchholz);

            TournamentDatabase.Tournaments.Clear();
            PlayerDatabase.Players.Clear();
        }

        /// <summary>
        /// Tests <see cref="TournamentPlayer.Culmulative"/>.
        /// </summary>
        [TestMethod]
        public void TournamentPlayerCulmulative()
        {
            Tournament tournament = new("Tournament 1", TournamentType.Danish);

            Player player1 = new("Player 1");
            Player player2 = new("Player 2");

            TournamentPlayer tplayer1 = new(tournament, player1);
            _ = new TournamentPlayer(tournament, player2);

            List<Game> games = new()
            {
                new(player1, player2, Result.White, DateTime.Now, true),
                new(player1, player2, Result.Draw, DateTime.Now, true),
                new(player1, player2, Result.Black, DateTime.Now, true),
            };

            _ = new TournamentRound(tournament);

            tournament.Rounds[0].AddGames(games);

            Assert.AreEqual(1.5, tplayer1.Culmulative);

            TournamentDatabase.Tournaments.Clear();
            PlayerDatabase.Players.Clear();
        }

        /// <summary>
        /// Tests <see cref="TournamentPlayer.Baumbach"/>.
        /// </summary>
        [TestMethod]
        public void TournamentPlayerBaumbach()
        {
            Tournament tournament = new("Tournament 1", TournamentType.Danish);

            Player player1 = new("Player 1");
            Player player2 = new("Player 2");

            TournamentPlayer tplayer = new(tournament, player1);
            _ = new TournamentPlayer(tournament, player2);

            List<Game> games = new()
            {
                new(player1, player2, Result.White, DateTime.Now, true),
                new(player1, player2, Result.Draw, DateTime.Now, true),
                new(player1, player2, Result.Black, DateTime.Now, true),
            };

            _ = new TournamentRound(tournament);

            tournament.Rounds[0].AddGames(games);

            Assert.AreEqual(1, tplayer.Baumbach);

            TournamentDatabase.Tournaments.Clear();
            PlayerDatabase.Players.Clear();
        }

        /// <summary>
        /// Tests <see cref="TournamentPlayer.Colours"/>.
        /// </summary>
        [TestMethod]
        public void TournamentPlayerColours()
        {
            Tournament tournament = new("Tournament 1", TournamentType.Danish);

            Player player1 = new("Player 1");
            Player player2 = new("Player 2");

            TournamentPlayer tplayer1 = new(tournament, player1);
            _ = new TournamentPlayer(tournament, player2);

            List<Game> games = new()
            {
                new(player1, player2, Result.White, DateTime.Now, true),
                new(player1, player2, Result.Draw, DateTime.Now, true),
                new(player1, player2, Result.Black, DateTime.Now, true),
            };

            _ = new TournamentRound(tournament);

            tournament.Rounds[0].AddGames(games);

            Assert.AreEqual((3, 0), tplayer1.Colours);

            TournamentDatabase.Tournaments.Clear();
            PlayerDatabase.Players.Clear();
        }

        /// <summary>
        /// Tests <see cref="TournamentPlayer.GetHeadToHeadScore(TournamentPlayer)"/>.
        /// </summary>
        [TestMethod]
        public void TournamentPlayerGetHeadToHeadWin()
        {
            Tournament tournament = new("Tournament 1", TournamentType.Danish);

            Player player1 = new("Player 1");
            Player player2 = new("Player 2");

            TournamentPlayer tplayer1 = new(tournament, player1);
            TournamentPlayer tplayer2 = new(tournament, player2);

            List<Game> games = new()
            {
                new(player1, player2, Result.White, DateTime.Now, true),
            };

            _ = new TournamentRound(tournament);

            tournament.Rounds[0].AddGames(games);

            Assert.AreEqual(1, tplayer1.GetHeadToHeadScore(tplayer2));

            TournamentDatabase.Tournaments.Clear();
            PlayerDatabase.Players.Clear();
        }

        /// <summary>
        /// Tests <see cref="TournamentPlayer.GetHeadToHeadScore(TournamentPlayer)"/>.
        /// </summary>
        [TestMethod]
        public void TournamentPlayerGetHeadToHeadLose()
        {
            Tournament tournament = new("Tournament 1", TournamentType.Danish);

            Player player1 = new("Player 1");
            Player player2 = new("Player 2");

            TournamentPlayer tplayer1 = new(tournament, player1);
            TournamentPlayer tplayer2 = new(tournament, player2);

            List<Game> games = new()
            {
                new(player1, player2, Result.Black, DateTime.Now, true),
            };

            _ = new TournamentRound(tournament);

            tournament.Rounds[0].AddGames(games);

            Assert.AreEqual(-1, tplayer1.GetHeadToHeadScore(tplayer2));

            TournamentDatabase.Tournaments.Clear();
            PlayerDatabase.Players.Clear();
        }

        /// <summary>
        /// Tests <see cref="TournamentPlayer.GetHeadToHeadScore(TournamentPlayer)"/>.
        /// </summary>
        [TestMethod]
        public void TournamentPlayerGetHeadToHeadDraw()
        {
            Tournament tournament = new("Tournament 1", TournamentType.Danish);

            Player player1 = new("Player 1");
            Player player2 = new("Player 2");

            TournamentPlayer tplayer1 = new(tournament, player1);
            TournamentPlayer tplayer2 = new(tournament, player2);

            List<Game> games = new()
            {
                new(player1, player2, Result.Draw, DateTime.Now, true),
            };

            _ = new TournamentRound(tournament);

            tournament.Rounds[0].AddGames(games);

            Assert.AreEqual(0, tplayer1.GetHeadToHeadScore(tplayer2));

            TournamentDatabase.Tournaments.Clear();
            PlayerDatabase.Players.Clear();
        }
    }
}
