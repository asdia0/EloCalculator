namespace EloCalculator
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;
    using System.Linq;

    /// <summary>
    /// Represents a tournament.
    /// </summary>
    public class Tournament
    {
        /// <summary>
        /// Checks if <see cref="Name"/> has been set.
        /// </summary>
        private bool NameSet = false;

        /// <summary>
        /// The value of <see cref="Name"/>.
        /// </summary>
        private string _Name;

        /// <summary>
        /// Represents the <see cref="Tournament"/>'s name.
        /// </summary>
        public string Name
        {
            get
            {
                return this._Name;
            }

            set
            {
                if (this.NameSet)
                {
                    throw new Exception("Name has already been set.");
                }

                this.NameSet = true;
                this._Name = value;
            }
        }

        /// <summary>
        /// A list of <see cref="TournamentRound"/>s held during the <see cref="Tournament"/>.
        /// </summary>
        public List<TournamentRound> Rounds = new List<TournamentRound>();

        /// <summary>
        /// A list of <see cref="TournamentPlayer"/>s participating in the <see cref="Tournament"/>.
        /// </summary>
        public List<TournamentPlayer> Players = new List<TournamentPlayer>();

        /// <summary>
        /// Initialises a new instance of the <see cref="Tournament"/> class.
        /// </summary>
        /// <param name="name"></param>
        public Tournament(string name)
        {
            this.Name = name;

            if (!Database.TableExists(this.Name))
            {
                using (SqlConnection connection = new SqlConnection(Settings.connectionString))
                {
                    connection.Open();

                    using (SqlCommand addTournament = new SqlCommand($"CREATE TABLE [dbo].[{name}] ( [Player] INT NOT NULL, [Active] BIT NOT NULL, [Score] FLOAT NOT NULL, [Sonneborn-Berger] FLOAT NOT NULL, [Buchholz] FLOAT NOT NULL);", connection))
                    {
                        addTournament.ExecuteNonQuery();
                    }
                }
            }

            if (!TournamentDatabase.Tournaments.Contains(this))
            {
                TournamentDatabase.Tournaments.Add(this);
            }
        }

        /// <summary>
        /// Updates <see cref="TournamentPlayer"/>s' <see cref="TournamentPlayer.Score"/>, <see cref="TournamentPlayer.SonnebornBerger"/> and <see cref="TournamentPlayer.Buchholz"/>.
        /// </summary>
        public void UpdateStats()
        {
            // Update scores
            foreach (TournamentRound round in this.Rounds)
            {
                foreach (Game game in round.Games)
                {
                    foreach (TournamentPlayer player in this.Players)
                    {
                        if (player.Player.Name == game.White.Name)
                        {
                            player.UpdateScore(Side.White, game.Result);
                        }

                        if (player.Player.Name == game.Black.Name)
                        {
                            player.UpdateScore(Side.Black, game.Result);
                        }
                    }
                }
            }

            // Update tiebreakers
            foreach (TournamentPlayer player in this.Players)
            {
                player.UpdateSB();

                player.UpdateBuchholz();
            }
        }

        /// <summary>
        /// Gets a list of pairings for the next <see cref="TournamentRound"/>. A `null` value represents a BYE.
        /// </summary>
        /// <returns>(White, Black)</returns>
        public List<(TournamentPlayer, TournamentPlayer?)> GetPairings()
        {
            string tournamentName = this.Name;

            List<TournamentPlayer> players = GetActivePlayersByRanking();

            List<(TournamentPlayer, TournamentPlayer?)> res = new List<(TournamentPlayer, TournamentPlayer?)>();

            for (int i = 0; i < Math.Floor(decimal.Divide(players.Count, 2)); i++)
            {
                TournamentPlayer white = players[2 * i];
                TournamentPlayer black = players[2 * i + 1];

                int whiteBlackID = GetLastPlayed(white, black);
                int blackWhiteID = GetLastPlayed(black, white);

                // did not play white-black
                if (whiteBlackID == 0)
                {
                    res.Add((white, black));
                    continue;
                }
                // did not play black-white
                if (blackWhiteID == 0)
                {
                    res.Add((black, white));
                    continue;
                }

                DateTime whiteBlack = GameDatabase.Games[whiteBlackID].DateTime;
                DateTime blackWhite = GameDatabase.Games[blackWhiteID].DateTime;

                // latest game was black-white; new game should be white-black
                if (whiteBlack.CompareTo(blackWhite) <= 0)
                {
                    res.Add((white, black));
                }
                // latest game was white-black; new game should be black-white
                else
                {
                    res.Add((black, white));
                }
            }

            // BYE if odd
            if (players.Count % 2 == 1)
            {
                res.Add((players.Last(), null));
            }

            return res;
        }

        /// <summary>
        /// Gets the <see cref="Game.Id"/> of the last <see cref="Game"/> played between two <see cref="TournamentPlayer"/>s in the <see cref="Tournament"/>.
        /// </summary>
        /// <param name="white">The white <see cref="TournamentPlayer"/>.</param>
        /// <param name="black">The black <see cref="TournamentPlayer"/>.</param>
        /// <returns></returns>
        public int GetLastPlayed(TournamentPlayer white, TournamentPlayer black)
        {
            string tournamentName = this.Name;

            using (SqlConnection connection = new SqlConnection(Settings.connectionString))
            {
                connection.Open();

                using (SqlCommand getLatestGame = new SqlCommand("SELECT Id FROM Game WHERE TournamentName LIKE @TName AND White LIKE @White AND Black LIKE @Black", connection))
                {
                    getLatestGame.Parameters.Add("@TName", SqlDbType.Text).Value = tournamentName;
                    getLatestGame.Parameters.Add("@White", SqlDbType.Text).Value = white.Player.Name;
                    getLatestGame.Parameters.Add("@Black", SqlDbType.Text).Value = black.Player.Name;

                    var rdr = getLatestGame.ExecuteReader();

                    int latestGameID = 0;

                    while (rdr.Read())
                    {
                        int id = (int)rdr["Id"];

                        if (id > latestGameID)
                        {
                            latestGameID = id;
                        }
                    }

                    rdr.Close();

                    return latestGameID;
                }
            }
        }

        /// <summary>
        /// Gets a list of active <see cref="TournamentPlayer"/>s by ranking. Highest ranking = top of list.
        /// </summary>
        /// <returns></returns>
        public List<TournamentPlayer> GetActivePlayersByRanking()
        {
            List<TournamentPlayer> players = GetPlayersByRanking();

            foreach (TournamentPlayer player in players.ToList())
            {
                if (player.Active == false)
                {
                    players.Remove(player);
                }
            }

            return players;
        }

        /// <summary>
        /// Gets a list of <see cref="TournamentPlayer"/>s by ranking. Highest ranking = top of list.
        /// </summary>
        /// <returns></returns>
        public List<TournamentPlayer> GetPlayersByRanking()
        {
            List<TournamentPlayer> res = new List<TournamentPlayer>();

            foreach (var elem in GetTournamentRankings())
            {
                res.Add(elem.Item1);
            }

            return res;
        }
        
        /// <summary>
        /// Gets the current rankings. Highest ranking = top of list.
        /// </summary>
        /// <returns>(Player, Score, Sonneborn-Berger, Buchholz)</returns>
        public List<(TournamentPlayer, double, double, double)> GetTournamentRankings()
        {
            List<(TournamentPlayer, double, double, double)> res = new List<(TournamentPlayer, double, double, double)>();

            foreach (TournamentPlayer player in this.Players)
            {
                res.Add((player, player.Score, player.SonnebornBerger, player.Buchholz));
            }

            return res.OrderByDescending(t => t.Item2).ThenByDescending(t => t.Item3).ThenByDescending(t => t.Item4).ToList();
        }

        /// <summary>
        /// Gets the sum of the <see cref="TournamentPlayer.Score"/> of the opponents the <see cref="Player"/> has played against with certain criteria.
        /// </summary>
        /// <param name="player">The <see cref="Player"/> to search for.</param>
        /// <param name="side">The <see cref="Side"/> of the player to search for.</param>
        /// <param name="result">The <see cref="Result"/> of the game to search for.</param>
        /// <returns></returns>
        public double GetOpponentScores(Player player, Side side, Result result)
        {
            string tournamentName = this.Name;

            double sum = 0;
            bool sideB = true;

            if (side == Side.Black)
            {
                sideB = false;
            }

            using (SqlConnection connection = new SqlConnection(Settings.connectionString))
            {
                connection.Open();

                string commandText = string.Empty;

                if (result == Result.Draw)
                {
                    commandText = $"SELECT {(sideB ? "Black" : "White")} FROM Game WHERE {(sideB ? "White" : "Black")} LIKE @Player AND TournamentName LIKE @TName AND Result IS NULL";
                }
                else
                {
                    commandText = $"SELECT {(sideB ? "Black" : "White")} FROM Game WHERE {(sideB ? "White" : "Black")} LIKE @Player AND TournamentName LIKE @TName AND Result={(result == Result.White ? 1 : 0)}";
                }

                using (SqlCommand getOpponents = new SqlCommand(commandText, connection))
                {
                    getOpponents.Parameters.Add("@Player", SqlDbType.Int).Value = player.Id;
                    getOpponents.Parameters.Add("@TName", SqlDbType.Text).Value = tournamentName;

                    var rdr = getOpponents.ExecuteReader();

                    while (rdr.Read())
                    {
                        int oppname = (int)rdr[(sideB ? "Black" : "White")];

                        // exclude BYEs
                        if (oppname != 0)
                        {
                            sum += this.Players.Where(i => i.Player.Id == oppname).First().Score;
                        }
                    }

                    rdr.Close();
                }
            }

            return sum;
        }
    }
}
