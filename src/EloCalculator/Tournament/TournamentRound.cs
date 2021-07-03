namespace EloCalculator
{
    using System.Collections.Generic;
    using System.Linq;

    using Newtonsoft.Json;

    public class TournamentRound
    {
        [JsonIgnore]
        public Tournament Tournament { get; }

        [JsonProperty]
        public int ID { get; }

        [JsonProperty("Games")]
        public List<int> GamesID
        {
            get
            {
                return this.Games.Select(i => i.ID).ToList();
            }
        }

        [JsonIgnore]
        public HashSet<Game> Games { get; }

        [JsonIgnore]
        public TournamentPlayer? PairingBye { get; set; }

        [JsonProperty("Bye (pairings)")]
        public int? PairingByeID
        {
            get
            {
                return this.PairingBye == null ? null : this.PairingBye.ID;
            }
        }

        [JsonIgnore]
        public HashSet<TournamentPlayer> RequestedByes { get; set; }

        [JsonProperty("Bye (requested)")]
        public List<int> RequestedByesID
        {
            get
            {
                return this.RequestedByes.Select(i => i.ID).ToList();
            }
        }

        public TournamentRound(Tournament tournament, List<Game> games = null)
        {
            this.ID = tournament.Rounds.Count;
            this.Tournament = tournament;
            this.RequestedByes = new HashSet<TournamentPlayer>();

            if (games == null)
            {
                this.Games = new HashSet<Game>();
            }
            else
            {
                this.AddGames(games);
            }
        }

        public void AddGame(Game game)
        {
            this.Games.Add(game);

            if (!this.Tournament.Players.Where(i => i.Player == game.White).Any())
            {
                this.Tournament.Players.Add(new TournamentPlayer(this.Tournament, game.White));
            }

            if (!this.Tournament.Players.Where(i => i.Player == game.Black).Any())
            {
                this.Tournament.Players.Add(new TournamentPlayer(this.Tournament, game.Black));
            }
        }

        public void AddGames(List<Game> games)
        {
            foreach (Game game in games)
            {
                this.Games.Add(game);

                if (!this.Tournament.Players.Where(i => i.Player == game.White).Any())
                {
                    this.Tournament.Players.Add(new TournamentPlayer(this.Tournament, game.White));
                }

                if (!this.Tournament.Players.Where(i => i.Player == game.Black).Any())
                {
                    this.Tournament.Players.Add(new TournamentPlayer(this.Tournament, game.Black));
                }
            }
        }

        public void AwardFullBye(TournamentPlayer player)
        {
            this.PairingBye = player;
        }

        public void AwardHalfBye(TournamentPlayer player)
        {
            this.RequestedByes.Add(player);
        }

        public void AwardHalfBye(List<TournamentPlayer> players)
        {
            this.RequestedByes.Union(players);
        }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
