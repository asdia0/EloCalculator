namespace EloCalculator.Test
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
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

            List<TournamentPlayer> players = new();

            CollectionAssert.AreEquivalent(players, tournament.Players);

            TournamentDatabase.Tournaments.Clear();
        }

        [TestMethod]
        public void TournamentRounds()
        {
            Tournament tournament = new("Tournament 1", TournamentType.RoundRobin);

            List<TournamentRound> rounds = new();

            CollectionAssert.AreEquivalent(rounds, tournament.Rounds);

            TournamentDatabase.Tournaments.Clear();
        }

        [TestMethod]
        public void TournamentAddPlayerLocal()
        {
            Tournament tournament = new("Tournament 1", TournamentType.RoundRobin);

            TournamentPlayer player = new(tournament, new Player("Player 1"));

            tournament.AddPlayer(player);

            List<TournamentPlayer> expected = new()
            {
                player,
            };

            CollectionAssert.AreEquivalent(expected, tournament.Players);

            TournamentDatabase.Tournaments.Clear();
            PlayerDatabase.Players.Clear();
        }

        [TestMethod]
        public void TournamentAddPlayerNonLocal()
        {
            Tournament tournament = new("Tournament 1", TournamentType.RoundRobin);
            Tournament nonLocal = new("Tournament 2", TournamentType.RoundRobin);

            TournamentPlayer player = new(nonLocal, new Player("Player 1"));

            tournament.AddPlayer(player);

            List<TournamentPlayer> expected = new();

            CollectionAssert.AreEquivalent(expected, tournament.Players);

            TournamentDatabase.Tournaments.Clear();
            PlayerDatabase.Players.Clear();
        }

        [TestMethod]
        public void TournamentAddPlayers()
        {
            Tournament tournament = new("Tournament 1", TournamentType.RoundRobin);
            Tournament nonLocal = new("Tournament 2", TournamentType.RoundRobin);

            List<TournamentPlayer> players = new()
            {
                new(tournament, new("Player 1")),
                new(tournament, new("Player 2")),
                new(nonLocal, new("Player 3")),
            };

            tournament.AddPlayers(players);

            List<TournamentPlayer> expected = players.ToList();
            expected.RemoveAt(2);

            CollectionAssert.AreEquivalent(expected, tournament.Players);

            TournamentDatabase.Tournaments.Clear();
            PlayerDatabase.Players.Clear();
        }

        [TestMethod]
        public void TournamentAddRound()
        {
            Tournament tournament = new("Tournament 1", TournamentType.RoundRobin);

            tournament.AddRound();

            Assert.AreEqual(1, tournament.Rounds.Count);

            TournamentDatabase.Tournaments.Clear();
        }

        [TestMethod]
        public void TournamentAddRoundLocal()
        {
            Tournament tournament = new("Tournament 1", TournamentType.RoundRobin);

            TournamentRound round = new(tournament);

            tournament.AddRound(round);

            List<TournamentRound> expected = new()
            {
                round,
            };

            CollectionAssert.AreEquivalent(expected, tournament.Rounds);

            TournamentDatabase.Tournaments.Clear();
        }

        [TestMethod]
        public void TournamentAddRoundNonLocal()
        {
            Tournament tournament = new("Tournament 1", TournamentType.RoundRobin);
            Tournament nonLocal = new("Tournament 2", TournamentType.RoundRobin);

            TournamentRound round = new(nonLocal);

            tournament.AddRound(round);

            List<TournamentRound> expected = new();

            CollectionAssert.AreEquivalent(expected, tournament.Rounds);

            TournamentDatabase.Tournaments.Clear();
        }

        [TestMethod]
        public void TournamentAddRounds()
        {
            Tournament tournament = new("Tournament 1", TournamentType.RoundRobin);
            Tournament nonLocal = new("Tournament 2", TournamentType.RoundRobin);

            List<TournamentRound> rounds = new()
            {
                new(tournament),
                new(tournament),
                new(nonLocal),
            };

            tournament.AddRounds(rounds);

            List<TournamentRound> expected = rounds.ToList();
            expected.RemoveAt(2);

            CollectionAssert.AreEquivalent(expected, tournament.Rounds);

            TournamentDatabase.Tournaments.Clear();
        }

        [TestMethod]
        public void TournamentGetLeaderboardArena()
        {
            Tournament tournament = new("Tournament 1", TournamentType.Danish);

            Player player1 = new("Player 1");
            Player player2 = new("Player 2");

            tournament.AddPlayers(new()
            {
                new(tournament, player1),
                new(tournament, player2),
            });

            List<TournamentPlayer> expected = new()
            {
                tournament.Players.Where(i => i.Player == player1).FirstOrDefault(),
                tournament.Players.Where(i => i.Player == player2).FirstOrDefault(),
            };

            CollectionAssert.AreEqual(expected, tournament.GetLeaderboard());

            TournamentDatabase.Tournaments.Clear();
            PlayerDatabase.Players.Clear();
        }

        [TestMethod]
        public void TournamentGetLeaderboardSwiss()
        {
            Tournament tournament = new("Tournament 1", TournamentType.Danish);

            Player player1 = new("Player 1");
            Player player2 = new("Player 2");


            tournament.AddPlayers(new()
            {
                new(tournament, player1),
                new(tournament, player2),
            });

            List<TournamentPlayer> expected = new()
            {
                tournament.Players.Where(i => i.Player == player1).FirstOrDefault(),
                tournament.Players.Where(i => i.Player == player2).FirstOrDefault(),
            };

            CollectionAssert.AreEqual(expected, tournament.GetLeaderboard());

            TournamentDatabase.Tournaments.Clear();
            PlayerDatabase.Players.Clear();
        }

        [TestMethod]
        public void TournamentGetLeaderboardRoundRobin()
        {
            Tournament tournament = new("Tournament 1", TournamentType.RoundRobin);

            Player player1 = new("Player 1");
            Player player2 = new("Player 2");

            tournament.AddPlayers(new()
            {
                new(tournament, player1),
                new(tournament, player2),
            });

            List<TournamentPlayer> expected = new()
            {
                tournament.Players.Where(i => i.Player == player1).FirstOrDefault(),
                tournament.Players.Where(i => i.Player == player2).FirstOrDefault(),
            };

            CollectionAssert.AreEqual(expected, tournament.GetLeaderboard());

            TournamentDatabase.Tournaments.Clear();
            PlayerDatabase.Players.Clear();
        }

        [TestMethod]
        public void TournamentGetLeaderboardActive()
        {
            Tournament tournament = new("Tournament 1", TournamentType.RoundRobin);

            TournamentPlayer inactive = new(tournament, new("Player 1"));
            TournamentPlayer active = new(tournament, new("Player 2"));

            inactive.Active = false;

            tournament.AddPlayers(new()
            {
                inactive,
                active,
            });

            List<TournamentPlayer> expected = new()
            {
                tournament.Players.Where(i => i == active).FirstOrDefault(),
            };

            CollectionAssert.AreEqual(expected, tournament.GetLeaderboardActive());

            TournamentDatabase.Tournaments.Clear();
            PlayerDatabase.Players.Clear();
        }

        [TestMethod]
        public void TournamentGetPairingsArenaOdd()
        {
            Tournament tournament = new("Tournament 1", TournamentType.Arena);

            Player player1 = new("Player 1");
            Player player2 = new("Player 2");
            Player player3 = new("Player 3");

            TournamentPlayer tplayer1 = new(tournament, player1);
            TournamentPlayer tplayer2 = new(tournament, player2);
            TournamentPlayer tplayer3 = new(tournament, player3);
            
            tournament.AddPlayers(new()
            {
                tplayer1,
                tplayer2,
                tplayer3,
            });

            Game game = new(player1, player2, Result.White, DateTime.Now, true);

            List<(TournamentPlayer, TournamentPlayer?)> expected = new()
            {
                (tplayer3, tplayer1),
                (tplayer2, null),
            };

            CollectionAssert.AreEqual(expected, tournament.GetPairings());

            TournamentDatabase.Tournaments.Clear();
            PlayerDatabase.Players.Clear();
            GameDatabase.Games.Clear();
        }

        [TestMethod]
        public void TournamentGetPairingsArenaEven()
        {
            Tournament tournament = new("Tournament 1", TournamentType.Arena);

            Player player1 = new("Player 1");
            Player player2 = new("Player 2");
            Player player3 = new("Player 3");
            Player player4 = new("Player 4");

            TournamentPlayer tplayer1 = new(tournament, player1);
            TournamentPlayer tplayer2 = new(tournament, player2);
            TournamentPlayer tplayer3 = new(tournament, player3);
            TournamentPlayer tplayer4 = new(tournament, player4);

            tournament.AddPlayers(new()
            {
                tplayer1,
                tplayer2,
                tplayer3,
                tplayer4,
            });

            Game game = new(player1, player2, Result.White, DateTime.Now, true);

            List<(TournamentPlayer, TournamentPlayer?)> expected = new()
            {
                (tplayer3, tplayer1),
                (tplayer2, tplayer4),
            };

            CollectionAssert.AreEqual(expected, tournament.GetPairings());

            TournamentDatabase.Tournaments.Clear();
            PlayerDatabase.Players.Clear();
            GameDatabase.Games.Clear();
        }

        [TestMethod]
        public void TournamentGetPairingsDanishRound1Odd()
        {
            Tournament tournament = new("Tournament 1", TournamentType.Danish);

            Player player1 = new("Player 1");
            Player player2 = new("Player 2");
            Player player3 = new("Player 3");

            TournamentPlayer tplayer1 = new(tournament, player1);
            TournamentPlayer tplayer2 = new(tournament, player2);
            TournamentPlayer tplayer3 = new(tournament, player3);

            tournament.AddPlayers(new()
            {
                tplayer1,
                tplayer2,
                tplayer3,
            });

            Game game = new(player1, player2, Result.White, DateTime.Now, true);

            List<(TournamentPlayer, TournamentPlayer?)> expected = new()
            {
                (tplayer3, tplayer1),
                (tplayer2, null),
            };

            CollectionAssert.AreEqual(expected, tournament.GetPairings());

            TournamentDatabase.Tournaments.Clear();
            PlayerDatabase.Players.Clear();
            GameDatabase.Games.Clear();
        }

        [TestMethod]
        public void TournamentGetPairingsDanishRound1Even()
        {
            Tournament tournament = new("Tournament 1", TournamentType.Danish);

            Player player1 = new("Player 1");
            Player player2 = new("Player 2");
            Player player3 = new("Player 3");
            Player player4 = new("Player 4");

            TournamentPlayer tplayer1 = new(tournament, player1);
            TournamentPlayer tplayer2 = new(tournament, player2);
            TournamentPlayer tplayer3 = new(tournament, player3);
            TournamentPlayer tplayer4 = new(tournament, player4);

            tournament.AddPlayers(new()
            {
                tplayer1,
                tplayer2,
                tplayer3,
                tplayer4,
            });

            Game game = new(player1, player2, Result.White, DateTime.Now, true);

            List<(TournamentPlayer, TournamentPlayer?)> expected = new()
            {
                (tplayer3, tplayer1),
                (tplayer2, tplayer4),
            };

            CollectionAssert.AreEqual(expected, tournament.GetPairings());

            TournamentDatabase.Tournaments.Clear();
            PlayerDatabase.Players.Clear();
            GameDatabase.Games.Clear();
        }

        [TestMethod]
        public void TournamentGetPairingsDanishRoundNOdd()
        {
            Tournament tournament = new("Tournament 1", TournamentType.Danish);

            Player player1 = new("Player 1");
            Player player2 = new("Player 2");
            Player player3 = new("Player 3");

            TournamentPlayer tplayer1 = new(tournament, player1);
            TournamentPlayer tplayer2 = new(tournament, player2);
            TournamentPlayer tplayer3 = new(tournament, player3);

            tournament.AddPlayers(new()
            {
                tplayer1,
                tplayer2,
                tplayer3,
            });

            tournament.AddRound();

            tournament.Rounds[0].AddGame(new(player1, player2, Result.Black, DateTime.Now, true));

            List <(TournamentPlayer, TournamentPlayer?)> expected = new()
            {
                (tplayer2, tplayer1),
                (tplayer3, null),
            };

            CollectionAssert.AreEqual(expected, tournament.GetPairings());

            TournamentDatabase.Tournaments.Clear();
            PlayerDatabase.Players.Clear();
            GameDatabase.Games.Clear();
        }

        [TestMethod]
        public void TournamentGetPairingsDanishRoundNEven()
        {
            Tournament tournament = new("Tournament 1", TournamentType.Danish);

            Player player1 = new("Player 1");
            Player player2 = new("Player 2");
            Player player3 = new("Player 3");
            Player player4 = new("Player 4");

            TournamentPlayer tplayer1 = new(tournament, player1);
            TournamentPlayer tplayer2 = new(tournament, player2);
            TournamentPlayer tplayer3 = new(tournament, player3);
            TournamentPlayer tplayer4 = new(tournament, player4);

            tournament.AddPlayers(new()
            {
                tplayer1,
                tplayer2,
                tplayer3,
                tplayer4,
            });

            tournament.AddRound();

            tournament.Rounds[0].AddGames(new()
            {
                new(player1, player2, Result.White, DateTime.Now, true),
                new(player3, player4, Result.Black, DateTime.Now, true),
            });

            List<(TournamentPlayer, TournamentPlayer?)> expected = new()
            {
                (tplayer4, tplayer1),
                (tplayer2, tplayer3),
            };

            CollectionAssert.AreEqual(expected, tournament.GetPairings());

            TournamentDatabase.Tournaments.Clear();
            PlayerDatabase.Players.Clear();
            GameDatabase.Games.Clear();
        }

        [TestMethod]
        public void TournamentGetPairingsDutchRound1Odd()
        {
            Tournament tournament = new("Tournament 1", TournamentType.Dutch);

            Player player1 = new("Player 1");
            Player player2 = new("Player 2");
            Player player3 = new("Player 3");

            TournamentPlayer tplayer1 = new(tournament, player1);
            TournamentPlayer tplayer2 = new(tournament, player2);
            TournamentPlayer tplayer3 = new(tournament, player3);

            tournament.AddPlayers(new()
            {
                tplayer1,
                tplayer2,
                tplayer3,
            });

            Game game = new(player1, player2, Result.White, DateTime.Now, true);

            List<(TournamentPlayer, TournamentPlayer?)> expected = new()
            {
                (tplayer2, tplayer1),
                (tplayer3, null),
            };

            CollectionAssert.AreEqual(expected, tournament.GetPairings());

            TournamentDatabase.Tournaments.Clear();
            PlayerDatabase.Players.Clear();
            GameDatabase.Games.Clear();
        }

        [TestMethod]
        public void TournamentGetPairingsDutchRound1Even()
        {
            Tournament tournament = new("Tournament 1", TournamentType.Dutch);

            Player player1 = new("Player 1");
            Player player2 = new("Player 2");
            Player player3 = new("Player 3");
            Player player4 = new("Player 4");

            TournamentPlayer tplayer1 = new(tournament, player1);
            TournamentPlayer tplayer2 = new(tournament, player2);
            TournamentPlayer tplayer3 = new(tournament, player3);
            TournamentPlayer tplayer4 = new(tournament, player4);

            tournament.AddPlayers(new()
            {
                tplayer1,
                tplayer2,
                tplayer3,
                tplayer4,
            });

            Game game = new(player1, player2, Result.White, DateTime.Now, true);

            List<(TournamentPlayer, TournamentPlayer?)> expected = new()
            {
                (tplayer4, tplayer1),
                (tplayer3, tplayer2),
            };

            CollectionAssert.AreEqual(expected, tournament.GetPairings());

            TournamentDatabase.Tournaments.Clear();
            PlayerDatabase.Players.Clear();
            GameDatabase.Games.Clear();
        }

        [TestMethod]
        public void TournamentGetPairingsDutchRoundNOdd()
        {
            Tournament tournament = new("Tournament 1", TournamentType.Dutch);

            Player player1 = new("Player 1");
            Player player2 = new("Player 2");
            Player player3 = new("Player 3");

            TournamentPlayer tplayer1 = new(tournament, player1);
            TournamentPlayer tplayer2 = new(tournament, player2);
            TournamentPlayer tplayer3 = new(tournament, player3);

            tournament.AddPlayers(new()
            {
                tplayer1,
                tplayer2,
                tplayer3,
            });

            tournament.AddRound();

            tournament.Rounds[0].AddGame(new(player1, player2, Result.Black, DateTime.Now, true));

            List<(TournamentPlayer, TournamentPlayer?)> expected = new()
            {
                (tplayer2, tplayer3),
                (tplayer1, null),
            };

            CollectionAssert.AreEqual(expected, tournament.GetPairings());

            TournamentDatabase.Tournaments.Clear();
            PlayerDatabase.Players.Clear();
            GameDatabase.Games.Clear();
        }

        [TestMethod]
        public void TournamentGetPairingsDutchRoundNEven()
        {
            Tournament tournament = new("Tournament 1", TournamentType.Dutch);

            Player player1 = new("Player 1");
            Player player2 = new("Player 2");
            Player player3 = new("Player 3");
            Player player4 = new("Player 4");

            TournamentPlayer tplayer1 = new(tournament, player1);
            TournamentPlayer tplayer2 = new(tournament, player2);
            TournamentPlayer tplayer3 = new(tournament, player3);
            TournamentPlayer tplayer4 = new(tournament, player4);

            tournament.AddPlayers(new()
            {
                tplayer1,
                tplayer2,
                tplayer3,
                tplayer4,
            });

            tournament.AddRound();

            tournament.Rounds[0].AddGames(new()
            {
                new(player1, player2, Result.White, DateTime.Now, true),
                new(player3, player4, Result.Black, DateTime.Now, true),
            });

            List<(TournamentPlayer, TournamentPlayer?)> expected = new()
            {
                (tplayer3, tplayer1),
                (tplayer4, tplayer2),
            };

            CollectionAssert.AreEqual(expected, tournament.GetPairings());

            TournamentDatabase.Tournaments.Clear();
            PlayerDatabase.Players.Clear();
            GameDatabase.Games.Clear();
        }

        [TestMethod]
        public void TournamentGetPairingsMonradRound1Odd()
        {
            Tournament tournament = new("Tournament 1", TournamentType.Monrad);

            Player player1 = new("Player 1");
            Player player2 = new("Player 2");
            Player player3 = new("Player 3");

            TournamentPlayer tplayer1 = new(tournament, player1);
            TournamentPlayer tplayer2 = new(tournament, player2);
            TournamentPlayer tplayer3 = new(tournament, player3);

            tournament.AddPlayers(new()
            {
                tplayer1,
                tplayer2,
                tplayer3,
            });

            Game game = new(player1, player2, Result.White, DateTime.Now, true);

            List<(TournamentPlayer, TournamentPlayer?)> expected = new()
            {
                (tplayer3, tplayer1),
                (tplayer2, null),
            };

            CollectionAssert.AreEqual(expected, tournament.GetPairings());

            TournamentDatabase.Tournaments.Clear();
            PlayerDatabase.Players.Clear();
            GameDatabase.Games.Clear();
        }

        [TestMethod]
        public void TournamentGetPairingsMonradRound1Even()
        {
            Tournament tournament = new("Tournament 1", TournamentType.Monrad);

            Player player1 = new("Player 1");
            Player player2 = new("Player 2");
            Player player3 = new("Player 3");
            Player player4 = new("Player 4");

            TournamentPlayer tplayer1 = new(tournament, player1);
            TournamentPlayer tplayer2 = new(tournament, player2);
            TournamentPlayer tplayer3 = new(tournament, player3);
            TournamentPlayer tplayer4 = new(tournament, player4);

            tournament.AddPlayers(new()
            {
                tplayer1,
                tplayer2,
                tplayer3,
                tplayer4,
            });

            Game game = new(player1, player2, Result.White, DateTime.Now, true);

            List<(TournamentPlayer, TournamentPlayer?)> expected = new()
            {
                (tplayer3, tplayer1),
                (tplayer2, tplayer4),
            };

            CollectionAssert.AreEqual(expected, tournament.GetPairings());

            TournamentDatabase.Tournaments.Clear();
            PlayerDatabase.Players.Clear();
            GameDatabase.Games.Clear();
        }

        [TestMethod]
        public void TournamentGetPairingsMonradRoundNOdd()
        {
            Tournament tournament = new("Tournament 1", TournamentType.Monrad);

            Player player1 = new("Player 1");
            Player player2 = new("Player 2");
            Player player3 = new("Player 3");

            TournamentPlayer tplayer1 = new(tournament, player1);
            TournamentPlayer tplayer2 = new(tournament, player2);
            TournamentPlayer tplayer3 = new(tournament, player3);

            tournament.AddPlayers(new()
            {
                tplayer1,
                tplayer2,
                tplayer3,
            });

            tournament.AddRound();

            tournament.Rounds[0].AddGame(new(player1, player2, Result.Black, DateTime.Now, true));

            List<(TournamentPlayer, TournamentPlayer?)> expected = new()
            {
                (tplayer2, tplayer3),
                (tplayer1, null),
            };

            CollectionAssert.AreEqual(expected, tournament.GetPairings());

            TournamentDatabase.Tournaments.Clear();
            PlayerDatabase.Players.Clear();
            GameDatabase.Games.Clear();
        }

        [TestMethod]
        public void TournamentGetPairingsMonradRoundNEven()
        {
            Tournament tournament = new("Tournament 1", TournamentType.Monrad);

            Player player1 = new("Player 1");
            Player player2 = new("Player 2");
            Player player3 = new("Player 3");
            Player player4 = new("Player 4");

            TournamentPlayer tplayer1 = new(tournament, player1);
            TournamentPlayer tplayer2 = new(tournament, player2);
            TournamentPlayer tplayer3 = new(tournament, player3);
            TournamentPlayer tplayer4 = new(tournament, player4);

            tournament.AddPlayers(new()
            {
                tplayer1,
                tplayer2,
                tplayer3,
                tplayer4,
            });

            tournament.AddRound();

            tournament.Rounds[0].AddGames(new()
            {
                new(player1, player2, Result.White, DateTime.Now, true),
                new(player3, player4, Result.Black, DateTime.Now, true),
            });

            List<(TournamentPlayer, TournamentPlayer?)> expected = new()
            {
                (tplayer4, tplayer1),
                (tplayer2, tplayer3),
            };

            CollectionAssert.AreEqual(expected, tournament.GetPairings());

            TournamentDatabase.Tournaments.Clear();
            PlayerDatabase.Players.Clear();
            GameDatabase.Games.Clear();
        }

        [TestMethod]
        public void TournamentGetPairingsRoundRobinRoundNOdd()
        {
            Tournament tournament = new("Tournament 1", TournamentType.RoundRobin);

            Player player1 = new("Player 1");
            Player player2 = new("Player 2");
            Player player3 = new("Player 3");

            TournamentPlayer tplayer1 = new(tournament, player1);
            TournamentPlayer tplayer2 = new(tournament, player2);
            TournamentPlayer tplayer3 = new(tournament, player3);

            tournament.AddPlayers(new()
            {
                tplayer1,
                tplayer2,
                tplayer3,
            });

            tournament.AddRound();
            tournament.AddRound();

            List<(TournamentPlayer, TournamentPlayer?)> expected = new()
            {
                (tplayer1, null),
                (tplayer2, tplayer3),
            };

            CollectionAssert.AreEqual(expected, tournament.GetPairings());

            TournamentDatabase.Tournaments.Clear();
            PlayerDatabase.Players.Clear();
            GameDatabase.Games.Clear();
        }

        [TestMethod]
        public void TournamentGetPairingsRoundRobinRoundNEven()
        {
            Tournament tournament = new("Tournament 1", TournamentType.RoundRobin);

            Player player1 = new("Player 1");
            Player player2 = new("Player 2");
            Player player3 = new("Player 3");
            Player player4 = new("Player 4");

            TournamentPlayer tplayer1 = new(tournament, player1);
            TournamentPlayer tplayer2 = new(tournament, player2);
            TournamentPlayer tplayer3 = new(tournament, player3);
            TournamentPlayer tplayer4 = new(tournament, player4);

            tournament.AddPlayers(new()
            {
                tplayer1,
                tplayer2,
                tplayer3,
                tplayer4,
            });

            tournament.AddRound();
            tournament.AddRound();

            List<(TournamentPlayer, TournamentPlayer?)> expected = new()
            {
                (tplayer1, tplayer3),
                (tplayer4, tplayer2),
            };

            CollectionAssert.AreEqual(expected, tournament.GetPairings());

            TournamentDatabase.Tournaments.Clear();
            PlayerDatabase.Players.Clear();
            GameDatabase.Games.Clear();
        }
    }
}
