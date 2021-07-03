namespace EloCalculator
{
    using System.Collections.Generic;
    using System.Linq;
    using Newtonsoft.Json;

    public class TournamentPlayer
    {
        [JsonIgnore]
        public Tournament Tournament { get; }

        [JsonProperty]
        public int ID { get; }

        [JsonProperty("Player")]
        public int PlayerID
        {
            get
            {
                return this.Player.ID;
            }
        }

        [JsonIgnore]
        public Player Player { get; }

        [JsonProperty("Games")]
        public List<int> GamesID
        {
            get
            {
                return this.Games.Select(i => i.ID).ToList();
            }
        }

        [JsonIgnore]
        public List<Game> Games
        {
            get
            {
                List<Game> res = new List<Game>();

                foreach (TournamentRound round in this.Tournament.Rounds)
                {
                    foreach (Game game in round.Games)
                    {
                        if (game.White == this.Player)
                        {
                            res.Add(game);
                            continue;
                        }

                        if (game.Black == this.Player)
                        {
                            res.Add(game);
                            continue;
                        }
                    }
                }

                return res;
            }
        }

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

                    if (game.White == this.Player && game.Result == Result.White)
                    {
                        res += 1;
                        continue;
                    }

                    if (game.Black == this.Player && game.Result == Result.Black)
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

        public double PerformanceRating
        {
            get
            {
                double averageOpponent = 0;

                foreach (Game game in this.Games)
                {
                    if (game.White == this.Player)
                    {
                        averageOpponent += game.Black.Rating;
                        continue;
                    }

                    if (game.Black == this.Player)
                    {
                        averageOpponent += game.White.Rating;
                        continue;
                    }
                }

                averageOpponent /= this.Games.Count;

                return averageOpponent + (800 * (double)this.Score / (double)this.Games.Count) - 400;
            }
        }

        [JsonProperty]
        public float SonnebornBerger
        {
            get
            {
                float res = 0;

                foreach (Game game in this.Games)
                {
                    if (game.White == this.Player)
                    {
                        if (game.Result == Result.Draw)
                        {
                            res += this.Tournament.Players.Where(i => i.Player == game.Black).FirstOrDefault().Score / 2;
                            continue;
                        }

                        if (game.Result == Result.White)
                        {
                            res += this.Tournament.Players.Where(i => i.Player == game.Black).FirstOrDefault().Score;
                            continue;
                        }
                    }

                    if (game.Black == this.Player)
                    {
                        if (game.Result == Result.Draw)
                        {
                            res += this.Tournament.Players.Where(i => i.Player == game.White).FirstOrDefault().Score / 2;
                            continue;
                        }

                        if (game.Result == Result.Black)
                        {
                            res += this.Tournament.Players.Where(i => i.Player == game.White).FirstOrDefault().Score;
                            continue;
                        }
                    }
                }

                return res;
            }
        }

        [JsonProperty]
        public float Buchholz
        {
            get
            {
                float res = 0;

                foreach (Game game in this.Games)
                {
                    if (game.White == this.Player)
                    {
                        if (game.Result == Result.Draw || game.Result == Result.White)
                        {
                            res += this.Tournament.Players.Where(i => i.Player == game.Black).FirstOrDefault().Score;
                            continue;
                        }
                    }

                    if (game.Black == this.Player)
                    {
                        if (game.Result == Result.Draw || game.Result == Result.Black)
                        {
                            res += this.Tournament.Players.Where(i => i.Player == game.White).FirstOrDefault().Score;
                            continue;
                        }
                    }
                }

                return res;
            }
        }

        [JsonProperty]
        public float MedianBuchholz
        {
            get
            {
                if (this.Games.Count <= 2)
                {
                    return 0;
                }

                List<float> scores = new List<float>();

                foreach (Game game in this.Games)
                {
                    if (game.White == this.Player)
                    {
                        if (game.Result == Result.Draw || game.Result == Result.White)
                        {
                            scores.Add(this.Tournament.Players.Where(i => i.Player == game.Black).FirstOrDefault().Score);
                            continue;
                        }
                    }

                    if (game.Black == this.Player)
                    {
                        if (game.Result == Result.Draw || game.Result == Result.Black)
                        {
                            scores.Add(this.Tournament.Players.Where(i => i.Player == game.Black).FirstOrDefault().Score);
                            continue;
                        }
                    }
                }

                scores.Sort();

                scores.RemoveAt(0);

                scores.Sort();

                scores.Reverse();

                scores.RemoveAt(0);

                return scores.Sum();
            }
        }

        [JsonProperty]
        public float BuchholzCut1
        {
            get
            {
                if (this.Games.Count <= 1)
                {
                    return 0;
                }

                List<float> scores = new List<float>();

                foreach (Game game in this.Games)
                {
                    if (game.White == this.Player)
                    {
                        if (game.Result == Result.Draw || game.Result == Result.White)
                        {
                            scores.Add(this.Tournament.Players.Where(i => i.Player == game.Black).FirstOrDefault().Score);
                            continue;
                        }
                    }

                    if (game.Black == this.Player)
                    {
                        if (game.Result == Result.Draw || game.Result == Result.Black)
                        {
                            scores.Add(this.Tournament.Players.Where(i => i.Player == game.Black).FirstOrDefault().Score);
                            continue;
                        }
                    }
                }

                scores.Sort();

                scores.RemoveAt(0);

                return scores.Sum();
            }
        }

        [JsonProperty]
        public float Culmulative
        {
            get
            {
                List<float> scores = new List<float>
                {
                    0,
                };

                foreach (TournamentRound round in this.Tournament.Rounds)
                {
                    foreach (Game game in round.Games)
                    {
                        if (game.White == this.Player)
                        {
                            if (game.Result == Result.Draw)
                            {
                                scores.Add(scores.Last() + 0.5F);
                            }

                            if (game.Result == Result.White)
                            {
                                scores.Add(scores.Last() + 1);
                            }
                        }

                        if (game.Black == this.Player)
                        {
                            if (game.Result == Result.Draw)
                            {
                                scores.Add(scores.Last() + 0.5F);
                            }

                            if (game.Result == Result.Black)
                            {
                                scores.Add(scores.Last() + 1);
                            }
                        }
                    }
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

        [JsonProperty]
        public int Baumbach
        {
            get
            {
                int wins = 0;

                foreach (Game game in this.Games)
                {
                    if (game.White == this.Player && game.Result == Result.White)
                    {
                        wins++;
                        continue;
                    }

                    if (game.Black == this.Player && game.Result == Result.Black)
                    {
                        wins++;
                        continue;
                    }
                }

                return wins;
            }
        }

        [JsonIgnore]
        public (int White, int Black) Colours
        {
            get
            {
                (int white, int black) = (0, 0);

                foreach (Game game in this.Games)
                {
                    if (game.White == this.Player)
                    {
                        white++;
                        continue;
                    }

                    if (game.Black == this.Player)
                    {
                        black++;
                        continue;
                    }
                }

                return (white, black);
            }
        }

        public TournamentPlayer(Tournament tournament, Player player)
        {
            this.ID = tournament.Players.Count;
            this.Tournament = tournament;
            this.Player = player;
        }

        public float GetHeadToHeadScore(TournamentPlayer player)
        {
            if (player.Tournament != this.Tournament)
            {
                throw new EloCalculatorException("Players must be in the same tournament.");
            }

            float res = 0;

            foreach (Game game in this.Games)
            {
                if (game.White == this.Player && game.Black == player.Player)
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

                if (game.Black == this.Player && game.White == player.Player)
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

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
