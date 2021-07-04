namespace EloCalculator.Test
{
    using System;
    using System.Collections.Generic;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using EloCalculator;

    [TestClass]
    public class TournamentPlayerTest
    {
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

        [TestMethod]
        public void TournamentPlayerScore()
        {
            Tournament tournament = new("Tournament 1", TournamentType.Arena);

            Player player1 = new("Player 1");
            Player player2 = new("Player 2");

            TournamentPlayer tplayer1 = new(tournament, player1);
            TournamentPlayer tplayer2 = new(tournament, player2);

            List<Game> games = new()
            {
                new(player1, player2, Result.White, DateTime.Now, true),
                new(player1, player2, Result.Draw, DateTime.Now, true),
                new(player1, player2, Result.Black, DateTime.Now, true),
            };

            tournament.AddRound();

            tournament.Rounds[0].AddGames(games);

            Assert.AreEqual(1.5, tplayer1.Score);

            TournamentDatabase.Tournaments.Clear();
            PlayerDatabase.Players.Clear();
        }

        [TestMethod]
        public void TournamentPlayerPerformanceRating()
        {
            Tournament tournament = new("Tournament 1", TournamentType.Arena);

            Player player1 = new("Player 1");
            Player player2 = new("Player 2");

            TournamentPlayer tplayer1 = new(tournament, player1);
            TournamentPlayer tplayer2 = new(tournament, player2);

            List<Game> games = new()
            {
                new(player1, player2, Result.White, DateTime.Now, true),
                new(player1, player2, Result.Draw, DateTime.Now, true),
                new(player1, player2, Result.Black, DateTime.Now, true),
            };

            tournament.AddRound();

            tournament.Rounds[0].AddGames(games);

            Assert.AreEqual(1004.3240891001137, tplayer1.PerformanceRating);

            TournamentDatabase.Tournaments.Clear();
            PlayerDatabase.Players.Clear();
        }

        [TestMethod]
        public void TournamentPlayerSonnebornBerger()
        {
            Tournament tournament = new("Tournament 1", TournamentType.Danish);

            Player player1 = new("Player 1");
            Player player2 = new("Player 2");

            TournamentPlayer tplayer1 = new(tournament, player1);
            TournamentPlayer tplayer2 = new(tournament, player2);

            List<Game> games = new()
            {
                new(player1, player2, Result.White, DateTime.Now, true),
                new(player1, player2, Result.Draw, DateTime.Now, true),
                new(player1, player2, Result.Black, DateTime.Now, true),
            };

            tournament.AddRound();

            tournament.Rounds[0].AddGames(games);

            Assert.AreEqual(2.25, tplayer1.SonnebornBerger);

            TournamentDatabase.Tournaments.Clear();
            PlayerDatabase.Players.Clear();
        }

        [TestMethod]
        public void TournamentPlayerBuchholz()
        {
            Tournament tournament = new("Tournament 1", TournamentType.Danish);

            Player player1 = new("Player 1");
            Player player2 = new("Player 2");

            TournamentPlayer tplayer1 = new(tournament, player1);
            TournamentPlayer tplayer2 = new(tournament, player2);

            List<Game> games = new()
            {
                new(player1, player2, Result.White, DateTime.Now, true),
                new(player1, player2, Result.Draw, DateTime.Now, true),
                new(player1, player2, Result.Black, DateTime.Now, true),
            };

            tournament.AddRound();

            tournament.Rounds[0].AddGames(games);

            Assert.AreEqual(3, tplayer1.Buchholz);

            TournamentDatabase.Tournaments.Clear();
            PlayerDatabase.Players.Clear();
        }

        [TestMethod]
        public void TournamentPlayerMedianBuchholz()
        {
            Tournament tournament = new("Tournament 1", TournamentType.Danish);

            Player player1 = new("Player 1");
            Player player2 = new("Player 2");

            TournamentPlayer tplayer1 = new(tournament, player1);
            TournamentPlayer tplayer2 = new(tournament, player2);

            List<Game> games = new()
            {
                new(player1, player2, Result.White, DateTime.Now, true),
                new(player1, player2, Result.Draw, DateTime.Now, true),
                new(player1, player2, Result.Black, DateTime.Now, true),
            };

            tournament.AddRound();

            tournament.Rounds[0].AddGames(games);

            Assert.AreEqual(1.5, tplayer1.MedianBuchholz);

            TournamentDatabase.Tournaments.Clear();
            PlayerDatabase.Players.Clear();
        }

        [TestMethod]
        public void TournamentPlayerCulmulative()
        {
            Tournament tournament = new("Tournament 1", TournamentType.Danish);

            Player player1 = new("Player 1");
            Player player2 = new("Player 2");

            TournamentPlayer tplayer1 = new(tournament, player1);
            TournamentPlayer tplayer2 = new(tournament, player2);

            List<Game> games = new()
            {
                new(player1, player2, Result.White, DateTime.Now, true),
                new(player1, player2, Result.Draw, DateTime.Now, true),
                new(player1, player2, Result.Black, DateTime.Now, true),
            };

            tournament.AddRound();

            tournament.Rounds[0].AddGames(games);

            Assert.AreEqual(1.5, tplayer1.Culmulative);

            TournamentDatabase.Tournaments.Clear();
            PlayerDatabase.Players.Clear();
        }

        [TestMethod]
        public void TournamentPlayerBaumbach()
        {
            Tournament tournament = new("Tournament 1", TournamentType.Danish);

            Player player1 = new("Player 1");
            Player player2 = new("Player 2");

            TournamentPlayer tplayer1 = new(tournament, player1);
            TournamentPlayer tplayer2 = new(tournament, player2);

            List<Game> games = new()
            {
                new(player1, player2, Result.White, DateTime.Now, true),
                new(player1, player2, Result.Draw, DateTime.Now, true),
                new(player1, player2, Result.Black, DateTime.Now, true),
            };

            tournament.AddRound();

            tournament.Rounds[0].AddGames(games);

            Assert.AreEqual(1, tplayer1.Baumbach);

            TournamentDatabase.Tournaments.Clear();
            PlayerDatabase.Players.Clear();
        }

        [TestMethod]
        public void TournamentPlayerColours()
        {
            Tournament tournament = new("Tournament 1", TournamentType.Danish);

            Player player1 = new("Player 1");
            Player player2 = new("Player 2");

            TournamentPlayer tplayer1 = new(tournament, player1);
            TournamentPlayer tplayer2 = new(tournament, player2);

            List<Game> games = new()
            {
                new(player1, player2, Result.White, DateTime.Now, true),
                new(player1, player2, Result.Draw, DateTime.Now, true),
                new(player1, player2, Result.Black, DateTime.Now, true),
            };

            tournament.AddRound();

            tournament.Rounds[0].AddGames(games);

            Assert.AreEqual((3, 0), tplayer1.Colours);

            TournamentDatabase.Tournaments.Clear();
            PlayerDatabase.Players.Clear();
        }

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

            tournament.AddRound();

            tournament.Rounds[0].AddGames(games);

            Assert.AreEqual(1, tplayer1.GetHeadToHeadScore(tplayer2));

            TournamentDatabase.Tournaments.Clear();
            PlayerDatabase.Players.Clear();
        }

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

            tournament.AddRound();

            tournament.Rounds[0].AddGames(games);

            Assert.AreEqual(-1, tplayer1.GetHeadToHeadScore(tplayer2));

            TournamentDatabase.Tournaments.Clear();
            PlayerDatabase.Players.Clear();
        }

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

            tournament.AddRound();

            tournament.Rounds[0].AddGames(games);

            Assert.AreEqual(0, tplayer1.GetHeadToHeadScore(tplayer2));

            TournamentDatabase.Tournaments.Clear();
            PlayerDatabase.Players.Clear();
        }
    }
}
