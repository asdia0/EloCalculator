namespace EloCalculator
{
    using System.Collections.Generic;
    using System.Linq;
    using Newtonsoft.Json;

    /// <summary>
    /// Represents a player participating in a <see cref="EloCalculator.Tournament"/>.
    /// </summary>
    public class TournamentPlayer
    {
        /// <summary>
        /// Gets the <see cref="EloCalculator.Tournament"/> the player is participating in.
        /// </summary>
        [JsonIgnore]
        public Tournament Tournament { get; }

        /// <summary>
        /// Gets the player's unique identification number.
        /// </summary>
        [JsonProperty]
        public int ID { get; }

        /// <summary>
        /// Gets or sets a value indicating whether the player should be paired in the next <see cref="TournamentRound"/>.
        /// </summary>
        public bool Active { get; set; }

        /// <summary>
        /// Gets the player's <see cref="EloCalculator.Player"/>.
        /// </summary>
        [JsonIgnore]
        public Player Player { get; }

        /// <summary>
        /// Gets the unique identifaction number of the player's <see cref="EloCalculator.Player"/>.
        /// </summary>
        [JsonProperty("Player")]
        public int PlayerID
        {
            get
            {
                return this.Player.ID;
            }
        }

        /// <summary>
        /// Gets a list of <see cref="Game"/>s played by the player during the <see cref="Tournament"/>.
        /// </summary>
        [JsonIgnore]
        public List<Game> Games
        {
            get
            {
                List<Game> res = new();

                foreach (TournamentRound round in this.Tournament.Rounds)
                {
                    foreach (Game game in round.Games)
                    {
                        if (game.WhitePlayer == this.Player)
                        {
                            res.Add(game);
                            continue;
                        }

                        if (game.BlackPlayer == this.Player)
                        {
                            res.Add(game);
                            continue;
                        }
                    }
                }

                return res;
            }
        }

        /// <summary>
        /// Gets a list of the unique identifciation numbers of the <see cref="Game"/>s played by the player during the <see cref="Tournament"/>.
        /// </summary>
        [JsonProperty("Games")]
        public List<int> GamesID
        {
            get
            {
                return this.Games.Select(i => i.ID).ToList();
            }
        }

        /// <summary>
        /// Gets the player's conventional score.
        /// </summary>
        [JsonProperty]
        public float Score
        {
            get
            {
                float res = 0;

                foreach (Game game in this.Games)
                {
                    if (game.Result == Result.Draw)
                    {
                        res += 0.5F;
                        continue;
                    }

                    if (game.WhitePlayer == this.Player && game.Result == Result.White)
                    {
                        res += 1;
                        continue;
                    }

                    if (game.BlackPlayer == this.Player && game.Result == Result.Black)
                    {
                        res += 1;
                        continue;
                    }
                }

                foreach (TournamentRound round in this.Tournament.Rounds)
                {
                    if (round.PairingBye == this)
                    {
                        res += 1;
                        continue;
                    }

                    if (round.RequestedByes.Contains(this))
                    {
                        res += 0.5F;
                        continue;
                    }
                }

                return res;
            }
        }

        /// <summary>
        /// Gets the player's <see href="https://en.wikipedia.org/wiki/Performance_rating_(chess)#Linear_performance_rating">linear performance rating</see>.
        /// </summary>
        public double PerformanceRating
        {
            get
            {
                double averageOpponent = 0;

                foreach (Game game in this.Games)
                {
                    if (game.WhitePlayer == this.Player)
                    {
                        averageOpponent += game.BlackPlayer.Rating;
                        continue;
                    }

                    if (game.BlackPlayer == this.Player)
                    {
                        averageOpponent += game.WhitePlayer.Rating;
                        continue;
                    }
                }

                averageOpponent /= this.Games.Count;

                return averageOpponent + (800 * (double)this.Score / (double)this.Games.Count) - 400;
            }
        }

        /// <summary>
        /// Gets the player's <see href="https://en.wikipedia.org/wiki/Sonneborn%E2%80%93Berger_score#Neustadtl_Sonneborn%E2%80%93Berger_score">Sonneborn-Berger score</see>.
        /// </summary>
        [JsonProperty]
        public float SonnebornBerger
        {
            get
            {
                float res = 0;

                foreach (Game game in this.Games)
                {
                    if (game.WhitePlayer == this.Player)
                    {
                        if (game.Result == Result.Draw)
                        {
                            res += this.Tournament.Players.Where(i => i.Player == game.BlackPlayer).FirstOrDefault().Score / 2;
                            continue;
                        }

                        if (game.Result == Result.White)
                        {
                            res += this.Tournament.Players.Where(i => i.Player == game.BlackPlayer).FirstOrDefault().Score;
                            continue;
                        }
                    }

                    if (game.BlackPlayer == this.Player)
                    {
                        if (game.Result == Result.Draw)
                        {
                            res += this.Tournament.Players.Where(i => i.Player == game.WhitePlayer).FirstOrDefault().Score / 2;
                            continue;
                        }

                        if (game.Result == Result.Black)
                        {
                            res += this.Tournament.Players.Where(i => i.Player == game.WhitePlayer).FirstOrDefault().Score;
                            continue;
                        }
                    }
                }

                return res;
            }
        }

        /// <summary>
        /// Gets the player's <see href=https://en.wikipedia.org/wiki/Buchholz_system"">Buchholz score</see>.
        /// </summary>
        [JsonProperty]
        public float Buchholz
        {
            get
            {
                float res = 0;

                foreach (Game game in this.Games)
                {
                    if (game.WhitePlayer == this.Player)
                    {
                        if (game.Result == Result.Draw || game.Result == Result.White)
                        {
                            res += this.Tournament.Players.Where(i => i.Player == game.BlackPlayer).FirstOrDefault().Score;
                            continue;
                        }
                    }

                    if (game.BlackPlayer == this.Player)
                    {
                        if (game.Result == Result.Draw || game.Result == Result.Black)
                        {
                            res += this.Tournament.Players.Where(i => i.Player == game.WhitePlayer).FirstOrDefault().Score;
                            continue;
                        }
                    }
                }

                return res;
            }
        }

        /// <summary>
        /// Gets the player's <see href=https://en.wikipedia.org/wiki/Buchholz_system"">Median Buchholz score</see>.
        /// </summary>
        [JsonProperty]
        public float MedianBuchholz
        {
            get
            {
                List<float> scores = new();

                foreach (Game game in this.Games)
                {
                    if (game.WhitePlayer == this.Player)
                    {
                        scores.Add(this.Tournament.Players.Where(i => i.Player == game.BlackPlayer).FirstOrDefault().Score);
                        continue;
                    }

                    if (game.BlackPlayer == this.Player)
                    {
                        scores.Add(this.Tournament.Players.Where(i => i.Player == game.BlackPlayer).FirstOrDefault().Score);
                        continue;
                    }
                }

                if (scores.Count <= 2)
                {
                    return 0;
                }

                scores.Sort();

                scores.RemoveAt(0);

                scores.Sort();

                scores.Reverse();

                scores.RemoveAt(0);

                return scores.Sum();
            }
        }

        /// <summary>
        /// Gets the player's <see href="https://en.wikipedia.org/wiki/Tie-breaking_in_Swiss-system_tournaments#Cumulative">culmulative score</see>.
        /// </summary>
        [JsonProperty]
        public float Culmulative
        {
            get
            {
                List<float> scores = new()
                {
                    0,
                };

                foreach (TournamentRound round in this.Tournament.Rounds)
                {
                    float roundScore = 0;

                    foreach (Game game in round.Games)
                    {
                        if (game.WhitePlayer == this.Player)
                        {
                            if (game.Result == Result.Draw)
                            {
                                roundScore += 0.5F;
                            }

                            if (game.Result == Result.White)
                            {
                                roundScore += 1;
                            }
                        }

                        if (game.BlackPlayer == this.Player)
                        {
                            if (game.Result == Result.Draw)
                            {
                                roundScore += 0.5F;
                            }

                            if (game.Result == Result.Black)
                            {
                                roundScore += 1;
                            }
                        }
                    }

                    scores.Add(scores.Last() + roundScore);
                }

                float byes = 0;

                foreach (TournamentRound round in this.Tournament.Rounds)
                {
                    if (round.PairingBye == this)
                    {
                        byes += 1;
                        continue;
                    }

                    if (round.RequestedByes.Contains(this))
                    {
                        byes += 0.5F;
                        continue;
                    }
                }

                return scores.Sum() - byes;
            }
        }

        /// <summary>
        /// Gets the player's <see href="https://en.wikipedia.org/wiki/Tie-breaking_in_Swiss-system_tournaments#Most_wins_(Baumbach)">Baumbach score</see>.
        /// </summary>
        [JsonProperty]
        public int Baumbach
        {
            get
            {
                int wins = 0;

                foreach (Game game in this.Games)
                {
                    if (game.WhitePlayer == this.Player && game.Result == Result.White)
                    {
                        wins++;
                        continue;
                    }

                    if (game.BlackPlayer == this.Player && game.Result == Result.Black)
                    {
                        wins++;
                        continue;
                    }
                }

                return wins;
            }
        }

        /// <summary>
        /// Gets the number of times the player has played each colour during the <see cref="Tournament"/>.
        /// </summary>
        [JsonIgnore]
        public (int White, int Black) Colours
        {
            get
            {
                return (this.Games.Where(i => i.WhitePlayer == this.Player).Count(), this.Games.Where(i => i.BlackPlayer == this.Player).Count());
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TournamentPlayer"/> class.
        /// </summary>
        /// <param name="tournament">The <see cref="EloCalculator.Tournament"/> the player is participating in.</param>
        /// <param name="player">The player's <see cref="EloCalculator.Player"/>.</param>
        public TournamentPlayer(Tournament tournament, Player player)
        {
            this.ID = tournament.Players.Any() ? tournament.Players.Last().ID + 1 : 0;
            this.Tournament = tournament;
            this.Player = player;
            this.Active = true;
            this.Tournament.Players.Add(this);
        }

        /// <summary>
        /// Gets the player's net score against another player.
        /// </summary>
        /// <param name="player">The player to compare with.</param>
        /// <returns>The player's net score.</returns>
        public float GetHeadToHeadScore(TournamentPlayer player)
        {
            if (player.Tournament != this.Tournament)
            {
                throw new EloCalculatorException("Players must be in the same tournament.");
            }

            float res = 0;

            foreach (Game game in this.Games)
            {
                if (game.WhitePlayer == this.Player && game.BlackPlayer == player.Player)
                {
                    if (game.Result == Result.White)
                    {
                        res += 1;
                        continue;
                    }

                    if (game.Result == Result.Black)
                    {
                        res -= 1;
                        continue;
                    }
                }

                if (game.BlackPlayer == this.Player && game.WhitePlayer == player.Player)
                {
                    if (game.Result == Result.White)
                    {
                        res -= 1;
                        continue;
                    }

                    if (game.Result == Result.Black)
                    {
                        res += 1;
                        continue;
                    }
                }
            }

            return res;
        }

        /// <summary>
        /// Gets a JSON string representing the game.
        /// </summary>
        /// <returns>A JSON string that represents the game.</returns>
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
