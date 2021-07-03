namespace EloCalculator
{
    using System.Collections.Generic;
    using System.Linq;
    using Newtonsoft.Json;
    using static System.Math;

    public class Tournament
    {
        [JsonProperty("ID")]
        public int ID { get; }

        [JsonProperty("Name")]
        public string Name { get; }

        [JsonProperty("Type")]
        public TournamentType Type { get; }

        [JsonProperty("Players")]
        public List<TournamentPlayer> Players { get; }

        [JsonProperty("Rounds")]
        public List<TournamentRound> Rounds { get; }

        public Tournament(string name, TournamentType type, List<TournamentPlayer> players = null, List<TournamentRound> rounds = null)
        {
            this.ID = TournamentDatabase.Tournaments.Count;
            this.Name = name;
            this.Type = type;
            this.Players = players ?? new List<TournamentPlayer>();
            this.Rounds = rounds ?? new List<TournamentRound>();

            TournamentDatabase.Tournaments.Add(this);
        }

        public List<(TournamentPlayer White, TournamentPlayer? Black)> GetPairings()
        {
            List<(TournamentPlayer white, TournamentPlayer? black)> res = new List<(TournamentPlayer, TournamentPlayer?)>();
            List<TournamentPlayer> rankings = this.GetRankings();

            // TODO
            switch (this.Type)
            {
                case TournamentType.Arena:
                    List<TournamentPlayer> rating = this.Players.OrderBy(i => i.Player.Rating).ToList();

                    for (int i = 0; i < rating.Count; i += 2)
                    {
                        if (i == rating.Count - 1)
                        {
                            res.Add((rating[i], null));
                            continue;
                        }

                        res.Add((rating[i], rating[i + 1]));
                    }

                    return res;

                case TournamentType.Danish:
                    List<TournamentPlayer> danishAvail = new List<TournamentPlayer>();

                    if (this.Rounds.Count == 0)
                    {
                        danishAvail = this.Players.OrderBy(i => i.Player.Rating).ToList();
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
                            res.Add((lower, higher));
                        }
                        else
                        {
                            res.Add((higher, lower));
                        }

                        danishAvail.Remove(higher);
                        danishAvail.Remove(lower);
                    }

                    return res;

                case TournamentType.Dutch:
                    List<TournamentPlayer> initial = new List<TournamentPlayer>();

                    if (this.Rounds.Count == 0)
                    {
                        initial = this.Players.OrderBy(i => i.Player.Rating).ToList();
                    }
                    else
                    {
                        initial = rankings;
                    }

                    List<TournamentPlayer> top = new List<TournamentPlayer>();
                    List<TournamentPlayer> bottom = new List<TournamentPlayer>();

                    if (initial.Count % 2 == 0)
                    {
                        for (int i = 0; i < initial.Count / 2; i++)
                        {
                            top.Add(initial[i]);
                        }

                        for (int i = initial.Count / 2; i < initial.Count; i++)
                        {
                            bottom.Add(initial[i]);
                        }
                    }
                    else
                    {
                        for (int i = 0; i < Floor((double)initial.Count / 2); i++)
                        {
                            top.Add(initial[i]);
                        }

                        for (int i = (int)Floor((double)initial.Count) + 1; i < initial.Count; i++)
                        {
                            bottom.Add(initial[i]);
                        }
                    }

                    while (top.Count != 0)
                    {
                        TournamentPlayer target = top[0];

                        Dictionary<TournamentPlayer, int> timesPlayed = new Dictionary<TournamentPlayer, int>();

                        foreach (TournamentPlayer player in bottom)
                        {
                            timesPlayed.Add(player, target.Games.Where(i => i.White == player.Player || i.Black == player.Player).ToList().Count);
                        }

                        TournamentPlayer partner = null;

                        foreach (TournamentPlayer player in bottom)
                        {
                            if (timesPlayed[player] == timesPlayed.Aggregate((l, r) => l.Value < r.Value ? l : r).Value)
                            {
                                partner = player;
                            }
                        }

                        if (target.Colours.Black > target.Colours.White)
                        {
                            res.Add((partner, target));
                        }
                        else
                        {
                            res.Add((target, partner));
                        }

                        top.Remove(target);
                        bottom.Remove(partner);
                    }

                    return res;

                case TournamentType.Monrad:
                    List<TournamentPlayer> monradAvail = new List<TournamentPlayer>();

                    if (this.Rounds.Count == 0)
                    {
                        monradAvail = this.Players.OrderBy(i => i.Player.Rating).ToList();
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
                            res.Add((lower, higher));
                        }
                        else
                        {
                            res.Add((higher, lower));
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

        public List<TournamentPlayer> GetRankings()
        {
            switch (this.Type)
            {
                case TournamentType.Arena:
                    return this.Players.
                        OrderByDescending(i => i.Score).
                        ThenByDescending(i => i.PerformanceRating).
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
                        ToList();
                case TournamentType.RoundRobin:
                    return this.Players.
                        OrderByDescending(i => i.Score).
                        ThenByDescending(i => i.SonnebornBerger).
                        ToList();
                default:
                    throw new EloCalculatorException("Unrecognised tournament type.");
            }
        }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
