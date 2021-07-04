namespace EloCalculator
{
    using System.Collections.Generic;
    using System.Linq;
    using Newtonsoft.Json;
    using static System.Math;

    /// <summary>
    /// Represents a tournament.
    /// </summary>
    public class Tournament
    {
        private List<TournamentPlayer> _Players = new();

        private List<TournamentRound> _Rounds = new();

        /// <summary>
        /// Gets the tournament's unique identification number.
        /// </summary>
        [JsonProperty("ID")]
        public int ID { get; }

        /// <summary>
        /// Gets the tournament's name.
        /// </summary>
        [JsonProperty("Name")]
        public string Name { get; }

        /// <summary>
        /// Gets the tournament's type.
        /// </summary>
        [JsonProperty("Type")]
        public TournamentType Type { get; }

        /// <summary>
        /// Gets or sets list of <see cref="TournamentPlayer"/>s participating in the tournament.
        /// </summary>
        [JsonProperty("Players")]
        public List<TournamentPlayer> Players
        {
            get
            {
                this._Players = this._Players.OrderBy(i => i.ID).ToList();
                return this._Players;
            }

            set
            {
                foreach (TournamentPlayer player in value)
                {
                    if (player.Tournament != this)
                    {
                        throw new EloCalculatorException("TournamentPlayer must be local.");
                    }
                }

                this._Players = value;
            }
        }

        /// <summary>
        /// Gets or sets a list of <see cref="TournamentRound"/>s being held during the tournament.
        /// </summary>
        [JsonProperty("Rounds")]
        public List<TournamentRound> Rounds
        {
            get
            {
                this._Rounds = this._Rounds.OrderBy(i => i.ID).ToList();
                return this._Rounds;
            }

            set
            {
                foreach (TournamentRound round in value)
                {
                    if (round.Tournament != this)
                    {
                        throw new EloCalculatorException("TournamentRound must be local.");
                    }
                }

                this._Rounds = value;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Tournament"/> class.
        /// </summary>
        /// <param name="name">The tournament's name.</param>
        /// <param name="type">The tournament's type.</param>
        /// <param name="players">A list of <see cref="TournamentPlayer"/>s participating in the tournament.</param>
        /// <param name="rounds">A list of <see cref="TournamentRound"/>s being held during the tournament.</param>
        public Tournament(string name, TournamentType type)
        {
            this.ID = TournamentDatabase.Tournaments.Any() ? TournamentDatabase.Tournaments.Last().ID + 1 : 0;
            this.Name = name;
            this.Type = type;

            TournamentDatabase.Tournaments.Add(this);
        }

        /// <summary>
        /// Gets the pairings for the next round.
        /// </summary>
        /// <returns>A list of <see cref="TournamentPlayer"/> tuples. The first item represents the <see cref="Player"/> playing white, the second item represents the <see cref="Player"/> playing black.</returns>
        public List<(TournamentPlayer White, TournamentPlayer? Black)> GetPairings()
        {
            List<(TournamentPlayer white, TournamentPlayer? black)> res = new();
            List<TournamentPlayer> rankings = this.GetLeaderboardActive();
            List<TournamentPlayer> rating = this.Players.OrderByDescending(i => i.Player.Rating).ThenBy(i => i.Player.Name).ToList();

            // TODO
            switch (this.Type)
            {
                case TournamentType.Arena:
                    for (int i = 0; i < rating.Count; i += 2)
                    {
                        if (i == rating.Count - 1)
                        {
                            res.Add((rating[i], null));
                            continue;
                        }

                        TournamentPlayer higher = rating[i];
                        TournamentPlayer lower = rating[i + 1];

                        if (higher.Colours.Black > higher.Colours.White)
                        {
                            res.Add((higher, lower));
                        }
                        else
                        {
                            res.Add((lower, higher));
                        }
                    }

                    return res;

                case TournamentType.Danish:
                    List<TournamentPlayer> danishAvail = new();

                    if (this.Rounds.Count == 0)
                    {
                        danishAvail = rating;
                    }
                    else
                    {
                        danishAvail = rankings;
                    }

                    for (int i = 0; i < danishAvail.Count; i += 2)
                    {
                        if (i == danishAvail.Count - 1)
                        {
                            res.Add((danishAvail[i], null));
                            continue;
                        }

                        TournamentPlayer higher = danishAvail[i];
                        TournamentPlayer lower = danishAvail[i + 1];

                        if (higher.Colours.Black > higher.Colours.White)
                        {
                            res.Add((higher, lower));
                        }
                        else
                        {
                            res.Add((lower, higher));
                        }
                    }

                    return res;

                case TournamentType.Dutch:
                    List<TournamentPlayer> initial = new();

                    if (this.Rounds.Count == 0)
                    {
                        initial = rating;
                    }
                    else
                    {
                        initial = rankings;
                    }

                    List<TournamentPlayer> top = new();
                    List<TournamentPlayer> bottom = new();

                    for (int i = 0; i < Ceiling((double)initial.Count / 2); i++)
                    {
                        top.Add(initial[i]);
                    }

                    for (int i = (int)Ceiling((double)initial.Count / 2); i < initial.Count; i++)
                    {
                        bottom.Add(initial[i]);
                    }

                    while (top.Count != 0)
                    {
                        if (top.Count == 1)
                        {
                            if (bottom.Count == 1)
                            {
                                res.Add((top[0], bottom[0]));
                            }
                            else
                            {
                                res.Add((top[0], null));
                            }

                            return res;
                        }

                        TournamentPlayer higher = top[0];
                        TournamentPlayer lower = null;

                        Dictionary<TournamentPlayer, int> timesPlayed = new();

                        foreach (TournamentPlayer bottomPlayer in bottom)
                        {
                            timesPlayed.Add(bottomPlayer, higher.Games.Where(i => i.White == bottomPlayer.Player || i.Black == bottomPlayer.Player).ToList().Count);
                        }

                        int lowest = timesPlayed.Aggregate((l, r) => l.Value < r.Value ? l : r).Value;

                        foreach (TournamentPlayer bottomPlayer in bottom)
                        {
                            if (timesPlayed[bottomPlayer] == lowest)
                            {
                                lower = bottomPlayer;
                                break;
                            }
                        }

                        if (higher.Colours.Black > higher.Colours.White)
                        {
                            res.Add((higher, lower));
                        }
                        else
                        {
                            res.Add((lower, higher));
                        }

                        top.Remove(higher);
                        bottom.Remove(lower);
                    }

                    return res;

                case TournamentType.Monrad:
                    List<TournamentPlayer> monradAvail = new();

                    if (this.Rounds.Count == 0)
                    {
                        monradAvail = rating;
                    }
                    else
                    {
                        monradAvail = rankings;
                    }

                    while (monradAvail.Count != 0)
                    {
                        if (monradAvail.Count == 1)
                        {
                            res.Add((monradAvail[0], null));
                            return res;
                        }

                        TournamentPlayer higher = monradAvail[0];
                        TournamentPlayer lower = null;

                        Dictionary<TournamentPlayer, int> timesPlayed = new();

                        foreach (TournamentPlayer player in monradAvail)
                        {
                            if (player == higher)
                            {
                                continue;
                            }

                            timesPlayed.Add(player, higher.Games.Where(i => i.White == player.Player || i.Black == player.Player).ToList().Count);
                        }

                        int lowest = timesPlayed.Aggregate((l, r) => l.Value < r.Value ? l : r).Value;

                        foreach (TournamentPlayer player in monradAvail)
                        {
                            if (player == higher)
                            {
                                continue;
                            }

                            if (timesPlayed[player] == lowest)
                            {
                                lower = player;
                                break;
                            }
                        }

                        if (higher.Colours.Black > higher.Colours.White)
                        {
                            res.Add((higher, lower));
                        }
                        else
                        {
                            res.Add((lower, higher));
                        }

                        monradAvail.RemoveAt(0);
                        monradAvail.Remove(lower);
                    }

                    return res;

                case TournamentType.RoundRobin:
                    // Generate initial pairings (Round 1)
                    List<(int?, int?)> initialRR = new();

                    if (rankings.Count % 2 == 0)
                    {
                        for (int i = 0; i <= (rankings.Count / 2) - 1; i++)
                        {
                            initialRR.Add((i, rankings.Count - 1 - i));
                        }
                    }
                    else
                    {
                        for (int i = 0; i <= (int)Floor((double)rankings.Count / 2) - 1; i++)
                        {
                            initialRR.Add((i, rankings.Count - 2 - i));
                        }

                        initialRR.Add((rankings.Count - 1, null));
                    }

                    // Rotate initialRR
                    List<(int?, int?)> aux = new();

                    for (int i = 0; i < this.Rounds.Count; i++)
                    {
                        aux = initialRR.ToList();

                        for (int j = 0; j < initialRR.Count; j++)
                        {
                            (int? white, int? black) = aux[j];

                            if (j == 0)
                            {
                                initialRR[j] = (0, aux[j + 1].Item1);
                            }
                            else if (j == initialRR.Count - 1)
                            {
                                initialRR[j] = (aux[j].Item2, aux[j - 1].Item2);
                            }
                            else
                            {
                                initialRR[j] = (aux[j + 1].Item1, aux[j - 1].Item2);
                            }
                        }

                        foreach ((int?, int?) elem in initialRR)
                        {
                            System.Console.WriteLine(elem);
                        }
                    }

                    // Switch sides if target round number is odd.
                    if (this.Rounds.Count % 2 == 1)
                    {
                        aux = initialRR.ToList();

                        for (int i = 0; i < initialRR.Count; i++)
                        {
                            (int? white, int? black) = aux[i];

                            if (white == null)
                            {
                                initialRR[i] = (black, null);
                            }
                            else if (black == null)
                            {
                                initialRR[i] = (white, null);
                            }
                            else
                            {
                                initialRR[i] = ((int)black, white);
                            }
                        }
                    }

                    // Convert to TournamentPlayer
                    foreach ((int? white, int? black) in initialRR)
                    {
                        res.Add((white == null ? null : this.Players[(int)white], black == null ? null : this.Players[(int)black]));
                    }

                    return res;

                default:
                    throw new EloCalculatorException("Unrecognised tournament type.");
            }
        }

        /// <summary>
        /// Gets a leaderboard of all players in <see cref="Players"/> based upon <see cref="TournamentPlayer.Score"/> and various tiebreaks.
        /// </summary>
        /// <returns>A list of <see cref="TournamentPlayer"/> sorted according to the tournament's <see cref="Type"/>.</returns>
        public List<TournamentPlayer> GetLeaderboard()
        {
            return this.Type switch
            {
                TournamentType.Arena => this.Players.
                    OrderByDescending(i => i.Score).
                    ThenByDescending(i => i.PerformanceRating).
                    ThenBy(i => i.Player.Name).
                    ToList(),
                TournamentType.Danish or TournamentType.Dutch or TournamentType.Monrad => this.Players.
                    OrderByDescending(i => i.Score).
                    ThenByDescending(i => i.SonnebornBerger).
                    ThenByDescending(i => i.MedianBuchholz).
                    ThenByDescending(i => i.Culmulative).
                    ThenByDescending(i => i.Buchholz).
                    ThenByDescending(i => i.Baumbach).
                    ThenBy(i => i.Player.Name).
                    ToList(),
                TournamentType.RoundRobin => this.Players.
                   OrderByDescending(i => i.Score).
                   ThenByDescending(i => i.SonnebornBerger).
                   ThenBy(i => i.Player.Name).
                   ToList(),
                _ => throw new EloCalculatorException("Unrecognised tournament type."),
            };
        }

        /// <summary>
        /// Gets a leaderboard of all active players in <see cref="Players"/> based upon <see cref="TournamentPlayer.Score"/> and various tiebreaks.
        /// </summary>
        /// <returns>A list of active <see cref="TournamentPlayer"/> sorted according to the tournament's <see cref="Type"/>.</returns>
        public List<TournamentPlayer> GetLeaderboardActive()
        {
            return this.GetLeaderboard().Where(i => i.Active == true).ToList();
        }

        /// <summary>
        /// Adds a <see cref="TournamentPlayer"/> to <see cref="Players"/>.
        /// </summary>
        /// <param name="player">The <see cref="TournamentPlayer"/> to add.</param>
        public void AddPlayer(TournamentPlayer player)
        {
            if (this.Players.Contains(player) || player.Tournament != this)
            {
                return;
            }

            this.Players.Add(player);
        }

        /// <summary>
        /// Adds a list of <see cref="TournamentPlayer"/> to <see cref="Players"/>.
        /// </summary>
        /// <param name="players">The list of <see cref="TournamentPlayer"/>s to add.</param>
        public void AddPlayers(List<TournamentPlayer> players)
        {
            foreach (TournamentPlayer player in players)
            {
                if (this.Players.Contains(player) || player.Tournament != this)
                {
                    continue;
                }

                this.Players.Add(player);
            }
        }

        /// <summary>
        /// Adds a <see cref="TournamentRound"/> to <see cref="Rounds"/>.
        /// </summary>
        public void AddRound()
        {
            TournamentRound round = new(this);
            this.Rounds.Add(round);
        }

        /// <summary>
        /// Adds a <see cref="TournamentRound"/> to <see cref="Rounds"/>.
        /// </summary>
        /// <param name="round">The <see cref="TournamentRound"/> to add.</param>
        public void AddRound(TournamentRound round)
        {
            if (this.Rounds.Contains(round) || round.Tournament != this)
            {
                return;
            }

            this.Rounds.Add(round);
        }

        /// <summary>
        /// Adds a list of <see cref="TournamentRound"/>s to <see cref="Rounds"/>.
        /// </summary>
        /// <param name="rounds">The list of <see cref="TournamentRound"/>s to add.</param>
        public void AddRounds(List<TournamentRound> rounds)
        {
            foreach (TournamentRound round in rounds)
            {
                if (this.Rounds.Contains(round) || round.Tournament != this)
                {
                    continue;
                }

                this.Rounds.Add(round);
            }
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
