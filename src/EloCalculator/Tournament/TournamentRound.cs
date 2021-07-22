namespace EloCalculator
{
    using System.Collections.Generic;
    using System.Linq;
    using Newtonsoft.Json;

    /// <summary>
    /// Represents a round in a <see cref="EloCalculator.Tournament"/>.
    /// </summary>
    public class TournamentRound
    {
        /// <summary>
        /// Gets the <see cref="EloCalculator.Tournament"/> the round is a part of.
        /// </summary>
        [JsonIgnore]
        public Tournament Tournament { get; }

        /// <summary>
        /// Gets the unique identification number of the round.
        /// </summary>
        [JsonProperty]
        public int ID { get; }

        /// <summary>
        /// Gets a list of <see cref="Game"/>s played during the round.
        /// </summary>
        [JsonIgnore]
        public HashSet<Game> Games { get; }

        /// <summary>
        /// Gets a list of the unique identifcation numbers of the <see cref="Game"/>s played during the round.
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
        /// Gets or sets the <see cref="TournamentPlayer"/> that received a bye due to pairings this round.
        /// </summary>
        [JsonIgnore]
        public TournamentPlayer? PairingBye { get; set; }

        /// <summary>
        /// Gets the unique identification number of the <see cref="TournamentPlayer"/> that received a bye due to pairings this round.
        /// </summary>
        [JsonProperty("Bye (pairings)")]
        public int? PairingByeID
        {
            get
            {
                return this.PairingBye?.ID;
            }
        }

        /// <summary>
        /// Gets or sets a list of <see cref="TournamentPlayer"/>s that requested a bye this round.
        /// </summary>
        [JsonIgnore]
        public HashSet<TournamentPlayer> RequestedByes { get; set; }

        /// <summary>
        /// Gets the unique identification numbers of the list of <see cref="TournamentPlayer"/>s that requested a bye this round.
        /// </summary>
        [JsonProperty("Bye (requested)")]
        public List<int> RequestedByesID
        {
            get
            {
                return this.RequestedByes.Select(i => i.ID).ToList();
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TournamentRound"/> class.
        /// </summary>
        /// <param name="tournament">The <see cref="EloCalculator.Tournament"/> that round is a part of.</param>
        /// <param name="games">A list of <see cref="Game"/>s played during this round.</param>
        public TournamentRound(Tournament tournament)
        {
            this.ID = tournament.Rounds.Any() ? tournament.Rounds.Last().ID + 1 : 0;
            this.Tournament = tournament;
            this.RequestedByes = new();
            this.Games = new();
            this.Tournament.Rounds.Add(this);
        }

        /// <summary>
        /// Adds a game to <see cref="Games"/>.
        /// </summary>
        /// <param name="game">The game to add.</param>
        public void AddGame(Game game)
        {
            this.Games.Add(game);

            if (!this.Tournament.Players.Where(i => i.Player == game.WhitePlayer).Any())
            {
                TournamentPlayer player = new(this.Tournament, game.WhitePlayer);
            }

            if (!this.Tournament.Players.Where(i => i.Player == game.BlackPlayer).Any())
            {
                TournamentPlayer player = new(this.Tournament, game.BlackPlayer);
            }
        }

        /// <summary>
        /// Adds multiple games to <see cref="Games"/>.
        /// </summary>
        /// <param name="games">The games to add.</param>
        public void AddGames(List<Game> games)
        {
            foreach (Game game in games)
            {
                this.Games.Add(game);

                if (!this.Tournament.Players.Where(i => i.Player == game.WhitePlayer).Any())
                {
                    TournamentPlayer player = new(this.Tournament, game.WhitePlayer);
                    this.Tournament.Players.Add(player);
                }

                if (!this.Tournament.Players.Where(i => i.Player == game.BlackPlayer).Any())
                {
                    TournamentPlayer player = new(this.Tournament, game.BlackPlayer);
                    this.Tournament.Players.Add(player);
                }
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
