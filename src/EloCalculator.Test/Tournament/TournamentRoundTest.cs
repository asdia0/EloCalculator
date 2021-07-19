namespace EloCalculator.Test
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using EloCalculator;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Tests <see cref="TournamentRound"/>.
    /// </summary>
    [TestClass]
    public class TournamentRoundTest
    {
        /// <summary>
        /// Tests <see cref="TournamentRound.Tournament"/>.
        /// </summary>
        [TestMethod]
        public void TournamentRoundTournament()
        {
            Tournament tournament = new("Tournament 1", TournamentType.Arena);

            TournamentRound round = new(tournament);

            Assert.AreEqual(tournament, round.Tournament);

            TournamentDatabase.Tournaments.Clear();
        }

        /// <summary>
        /// Tests <see cref="TournamentRound.ID"/>.
        /// </summary>
        [TestMethod]
        public void TournamentRoundID()
        {
            Tournament tournament = new("Tournament 1", TournamentType.Arena);

            TournamentRound round = new(tournament);

            Assert.AreEqual(0, round.ID);

            TournamentDatabase.Tournaments.Clear();
        }

        /// <summary>
        /// Tests <see cref="TournamentRound.Games"/>.
        /// </summary>
        [TestMethod]
        public void TournamentRoundGames()
        {
            Tournament tournament = new("Tournament 1", TournamentType.Arena);

            TournamentRound round = new(tournament);

            CollectionAssert.AreEquivalent(new List<Game>(), round.Games.ToList());

            TournamentDatabase.Tournaments.Clear();
        }

        /// <summary>
        /// Tests <see cref="TournamentRound.GamesID"/>.
        /// </summary>
        [TestMethod]
        public void TournamentRoundGamesID()
        {
            Tournament tournament = new("Tournament 1", TournamentType.Arena);

            TournamentRound round = new(tournament);

            CollectionAssert.AreEquivalent(new List<int>(), round.GamesID);

            TournamentDatabase.Tournaments.Clear();
        }

        /// <summary>
        /// Tests <see cref="TournamentRound.PairingBye"/>.
        /// </summary>
        [TestMethod]
        public void TournamentRoundPairingBye()
        {
            Tournament tournament = new("Tournament 1", TournamentType.Arena);

            TournamentRound round = new(tournament);

            TournamentPlayer tplayer = new(tournament, new("Player 1"));

            round.PairingBye = tplayer;

            Assert.AreEqual(tplayer, round.PairingBye);

            TournamentDatabase.Tournaments.Clear();
            PlayerDatabase.Players.Clear();
        }

        /// <summary>
        /// Tests <see cref="TournamentRound.PairingByeID"/>.
        /// </summary>
        [TestMethod]
        public void TournamentRoundPairingByeID()
        {
            Tournament tournament = new("Tournament 1", TournamentType.Arena);

            TournamentRound round = new(tournament);

            TournamentPlayer tplayer = new(tournament, new("Player 1"));

            round.PairingBye = tplayer;

            Assert.AreEqual(tplayer.ID, round.PairingByeID);

            TournamentDatabase.Tournaments.Clear();
            PlayerDatabase.Players.Clear();
        }

        /// <summary>
        /// Tests <see cref="TournamentRound.RequestedByes"/>.
        /// </summary>
        [TestMethod]
        public void TournamentRoundRequestedByes()
        {
            Tournament tournament = new("Tournament 1", TournamentType.Arena);

            TournamentRound round = new(tournament);

            TournamentPlayer tplayer = new(tournament, new("Player 1"));

            round.RequestedByes.Add(tplayer);

            CollectionAssert.AreEquivalent(new List<TournamentPlayer> { tplayer }, round.RequestedByes.ToList());

            TournamentDatabase.Tournaments.Clear();
            PlayerDatabase.Players.Clear();
        }

        /// <summary>
        /// Tests <see cref="TournamentRound.RequestedByesID"/>.
        /// </summary>
        [TestMethod]
        public void TournamentRoundRequestedByesID()
        {
            Tournament tournament = new("Tournament 1", TournamentType.Arena);

            TournamentRound round = new(tournament);

            TournamentPlayer tplayer = new(tournament, new("Player 1"));

            round.RequestedByes.Add(tplayer);

            CollectionAssert.AreEquivalent(new List<int> { tplayer.ID }, round.RequestedByesID);

            TournamentDatabase.Tournaments.Clear();
            PlayerDatabase.Players.Clear();
        }

        /// <summary>
        /// Tests <see cref="TournamentRound.AddGame(Game)"/>.
        /// </summary>
        [TestMethod]
        public void TournamentRoundAddGame()
        {
            Tournament tournament = new("Tournament 1", TournamentType.Arena);

            TournamentRound round = new(tournament);

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

        /// <summary>
        /// Tests <see cref="TournamentRound.AddGames(List{Game})"/>.
        /// </summary>
        [TestMethod]
        public void TournamentRoundAddGames()
        {
            Tournament tournament = new("Tournament 1", TournamentType.Arena);

            TournamentRound round = new(tournament);

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