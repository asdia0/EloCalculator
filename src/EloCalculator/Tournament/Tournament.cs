﻿namespace EloCalculator
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
        /// Gets a list of <see cref="TournamentPlayer"/>s participating in the tournament.
        /// </summary>
        [JsonProperty("Players")]
        public List<TournamentPlayer> Players { get; }

        /// <summary>
        /// Gets a list of <see cref="TournamentRound"/>s being held during the tournament.
        /// </summary>
        [JsonProperty("Rounds")]
        public List<TournamentRound> Rounds { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Tournament"/> class.
        /// </summary>
        /// <param name="name">The tournament's name.</param>
        /// <param name="type">The tournament's type.</param>
        /// <param name="players">A list of <see cref="TournamentPlayer"/>s participating in the tournament.</param>
        /// <param name="rounds">A list of <see cref="TournamentRound"/>s being held during the tournament.</param>
        public Tournament(string name, TournamentType type)
        {
            this.ID = TournamentDatabase.Tournaments.Count;
            this.Name = name;
            this.Type = type;
            this.Players = new List<TournamentPlayer>();
            this.Rounds = new List<TournamentRound>();

            TournamentDatabase.Tournaments.Add(this);
        }

        /// <summary>
        /// Gets the pairings for the next round.
        /// </summary>
        /// <returns>A list of <see cref="TournamentPlayer"/> tuples. The first item represents the <see cref="Player"/> playing white, the second item represents the <see cref="Player"/> playing black.</returns>
        public List<(TournamentPlayer White, TournamentPlayer? Black)> GetPairings()
        {
            List<(TournamentPlayer white, TournamentPlayer? black)> res = new List<(TournamentPlayer, TournamentPlayer?)>();
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
                    List<TournamentPlayer> danishAvail = new List<TournamentPlayer>();

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
                    List<TournamentPlayer> initial = new List<TournamentPlayer>();

                    if (this.Rounds.Count == 0)
                    {
                        initial = rating;
                    }
                    else
                    {
                        initial = rankings;
                    }

                    List<TournamentPlayer> top = new List<TournamentPlayer>();
                    List<TournamentPlayer> bottom = new List<TournamentPlayer>();

                    for (int i = 0; i < (int)Ceiling((double)initial.Count / 2); i++)
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
                            res.Add((top[0], null));
                            return res;
                        }

                        TournamentPlayer higher = top[0];

                        Dictionary<TournamentPlayer, int> timesPlayed = new();

                        foreach (TournamentPlayer bottomPlayer in bottom)
                        {
                            timesPlayed.Add(bottomPlayer, higher.Games.Where(i => i.White == bottomPlayer.Player || i.Black == bottomPlayer.Player).ToList().Count);
                        }

                        TournamentPlayer lower = timesPlayed.Aggregate((l, r) => l.Value < r.Value ? l : r).Key;


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
                    List<TournamentPlayer> monradAvail = new List<TournamentPlayer>();

                    if (this.Rounds.Count == 0)
                    {
                        monradAvail = rating;
                    }
                    else
                    {
                        monradAvail = rankings;
                    }

                    for (int i = 0; i < monradAvail.Count; i += 2)
                    {
                        if (monradAvail.Count - 1 == i)
                        {
                            res.Add((monradAvail[i], null));
                            continue;
                        }

                        TournamentPlayer higher = monradAvail[i];
                        TournamentPlayer lower = null;

                        Dictionary<TournamentPlayer, int> timesPlayed = new Dictionary<TournamentPlayer, int>();

                        foreach (TournamentPlayer player in monradAvail)
                        {
                            if (player == higher)
                            {
                                continue;
                            }

                            timesPlayed.Add(player, higher.Games.Where(i => i.White == player.Player || i.Black == player.Player).ToList().Count);
                        }

                        lower = timesPlayed.Aggregate((l, r) => l.Value < r.Value ? l : r).Key;

                        if (higher.Colours.Black > higher.Colours.White)
                        {
                            res.Add((higher, lower));
                        }
                        else
                        {
                            res.Add((lower, higher));
                        }

                        monradAvail.Remove(higher);
                        monradAvail.Remove(lower);
                    }

                    return res;

                case TournamentType.RoundRobin:
                    List<(int, int?)> list1 = new List<(int, int?)>();
                    List<(int, int?)> list2 = new List<(int, int?)>();
                    int playerCount = this.Players.Count;

                    for (int i = 0; i < Floor((double)playerCount / 2); i++)
                    {
                        list1.Add((i, playerCount - i - 1));
                    }

                    if (playerCount % 2 == 1)
                    {
                        list1.Add(((int)Floor((double)playerCount / 2), null));
                    }

                    if (this.Rounds.Count == 0)
                    {
                        foreach ((int white, int? black) in list1)
                        {
                            res.Add((this.Players[white], black == null ? null : this.Players[(int)black]));
                        }

                        return res;
                    }

                    foreach ((int white, int? black) in list1)
                    {
                        if (white == playerCount - 1)
                        {
                            list2.Add((white, black == null ? null : (black * this.Rounds.Count) % (playerCount - 1)));
                            continue;
                        }

                        if (black == playerCount - 1)
                        {
                            list2.Add(((white * this.Rounds.Count) % (playerCount - 1), black));
                            continue;
                        }

                        list2.Add(((white * this.Rounds.Count) % (playerCount - 1), black == null ? null : (black * this.Rounds.Count) % (playerCount - 1)));
                    }

                    if (playerCount % 2 == 0)
                    {
                        foreach ((int white, int? black) in list2)
                        {
                            res.Add((this.Players[white], black == null ? null : this.Players[(int)black]));
                        }
                    }
                    else
                    {
                        foreach ((int white, int? black) in list2)
                        {
                            if (black == null)
                            {
                                res.Add((this.Players[white], null));
                            }
                            else
                            {
                                res.Add((this.Players[(int)black], this.Players[white]));
                            }
                        }
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
            switch (this.Type)
            {
                case TournamentType.Arena:
                    return this.Players.
                        OrderByDescending(i => i.Score).
                        ThenByDescending(i => i.PerformanceRating).
                        ThenBy(i => i.Player.Name).
                        ToList();
                case TournamentType.Danish:
                case TournamentType.Dutch:
                case TournamentType.Monrad:
                    return this.Players.
                        OrderByDescending(i => i.Score).
                        ThenByDescending(i => i.SonnebornBerger).
                        ThenByDescending(i => i.MedianBuchholz).
                        ThenByDescending(i => i.Culmulative).
                        ThenByDescending(i => i.Buchholz).
                        ThenByDescending(i => i.Baumbach).
                        ThenBy(i => i.Player.Name).
                        ToList();
                case TournamentType.RoundRobin:
                    return this.Players.
                        OrderByDescending(i => i.Score).
                        ThenByDescending(i => i.SonnebornBerger).
                        ThenBy(i => i.Player.Name).
                        ToList();
                default:
                    throw new EloCalculatorException("Unrecognised tournament type.");
            }
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
