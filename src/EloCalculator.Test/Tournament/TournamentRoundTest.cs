namespace EloCalculator.Test
{
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using EloCalculator;

    [TestClass]
    public class TournamentRoundTest
    {
        [TestMethod]
        public void TournamentRoundTournament()
        {
            Tournament tournament = new("Tournament 1", TournamentType.Arena);

            TournamentRound round = new(tournament);

            tournament.AddRound(round);

            Assert.AreEqual(tournament, round.Tournament);

            TournamentDatabase.Tournaments.Clear();
        }

        [TestMethod]
        public void TournamentRoundID()
        {
            Tournament tournament = new("Tournament 1", TournamentType.Arena);

            TournamentRound round = new(tournament);

            tournament.AddRound(round);

            Assert.AreEqual(0, round.ID);

            TournamentDatabase.Tournaments.Clear();
        }

        [TestMethod]
        public void TournamentRoundGames()
        {
            Tournament tournament = new("Tournament 1", TournamentType.Arena);

            TournamentRound round = new(tournament);

            tournament.AddRound(round);

            CollectionAssert.AreEquivalent(new List<Game>(), round.Games.ToList());

            TournamentDatabase.Tournaments.Clear();
        }

        [TestMethod]
        public void TournamentRoundGamesID()
        {
            Tournament tournament = new("Tournament 1", TournamentType.Arena);

            TournamentRound round = new(tournament);

            tournament.AddRound(round);

            CollectionAssert.AreEquivalent(new List<int>(), round.GamesID);

            TournamentDatabase.Tournaments.Clear();
        }

        [TestMethod]
        public void TournamentRoundPairingBye()
        {
            Tournament tournament = new("Tournament 1", TournamentType.Arena);

            TournamentRound round = new(tournament);

            tournament.AddRound(round);

            TournamentPlayer tplayer = new(tournament, new("Player 1"));

            round.PairingBye = tplayer;

            Assert.AreEqual(tplayer, round.PairingBye);

            TournamentDatabase.Tournaments.Clear();
            PlayerDatabase.Players.Clear();
        }

        [TestMethod]
        public void TournamentRoundPairingByeID()
        {
            Tournament tournament = new("Tournament 1", TournamentType.Arena);

            TournamentRound round = new(tournament);

            tournament.AddRound(round);

            TournamentPlayer tplayer = new(tournament, new("Player 1"));

            round.PairingBye = tplayer;

            Assert.AreEqual(tplayer.ID, round.PairingByeID);

            TournamentDatabase.Tournaments.Clear();
            PlayerDatabase.Players.Clear();
        }

        [TestMethod]
        public void TournamentRoundRequestedByes()
        {
            Tournament tournament = new("Tournament 1", TournamentType.Arena);

            TournamentRound round = new(tournament);

            tournament.AddRound(round);

            TournamentPlayer tplayer = new(tournament, new("Player 1"));

            round.RequestedByes.Add(tplayer);

            CollectionAssert.AreEquivalent(new List<TournamentPlayer> { tplayer }, round.RequestedByes.ToList());

            TournamentDatabase.Tournaments.Clear();
            PlayerDatabase.Players.Clear();
        }

        [TestMethod]
        public void TournamentRoundRequestedByesID()
        {
            Tournament tournament = new("Tournament 1", TournamentType.Arena);

            TournamentRound round = new(tournament);

            tournament.AddRound(round);

            TournamentPlayer tplayer = new(tournament, new("Player 1"));

            round.RequestedByes.Add(tplayer);

            CollectionAssert.AreEquivalent(new List<int> { tplayer.ID }, round.RequestedByesID);

            TournamentDatabase.Tournaments.Clear();
            PlayerDatabase.Players.Clear();
        }

        [TestMethod]
        public void TournamentRoundAddGame()
        {
            Tournament tournament = new("Tournament 1", TournamentType.Arena);

            TournamentRound round = new(tournament);

            tournament.AddRound(round);

            Game game = new(new("Player 1"), new("Player 2"), Result.White, DateTime.Now, true);

            round.AddGame(game);

            List<Game> expected = new()
            {
                game,
            };

            CollectionAssert.AreEquivalent(expected, round.Games.ToList());

            TournamentDatabase.Tournaments.Clear();
            PlayerDatabase.Players.Clear();
        }

        [TestMethod]
        public void TournamentRoundAddGames()
        {
            Tournament tournament = new("Tournament 1", TournamentType.Arena);

            TournamentRound round = new(tournament);

            tournament.AddRound(round);

            Game game1 = new(new("Player 1"), new("Player 2"), Result.White, DateTime.Now, true);
            Game game2 = new(new("Player 1"), new("Player 2"), Result.White, DateTime.Now, true);

            List<Game> games = new()
            {
                game1,
                game2,
            };

            round.AddGames(games);

            CollectionAssert.AreEquivalent(games, round.Games.ToList());

            TournamentDatabase.Tournaments.Clear();
            PlayerDatabase.Players.Clear();
        }
    }
}