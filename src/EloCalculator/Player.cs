namespace EloCalculator
{
    using System.Collections.Generic;
    using System.Linq;
    using Newtonsoft.Json;

    /// <summary>
    /// Represents a player.
    /// </summary>
    public class Player
    {
        /// <summary>
        /// Gets the player's unique identification number.
        /// </summary>
        [JsonProperty]
        public int ID { get; }

        /// <summary>
        /// Gets the player's name.
        /// </summary>
        [JsonProperty]
        public string Name { get; }

        /// <summary>
        /// Gets or sets the player's Elo rating.
        /// </summary>
        [JsonProperty]
        public double Rating { get; set; }

        /// <summary>
        /// Gets a list of <see cref="Game"/>s the player has played.
        /// </summary>
        [JsonIgnore]
        public List<Game> Games
        {
            get
            {
                return GameDatabase.Games.Where(game => game.White == this || game.Black == this).ToList();
            }
        }

        /// <summary>
        /// Gets a list of the unique identification number of the <see cref="Game"/>s that the player has played.
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
        /// Gets the number of <see cref="Game"/>s the player has won.
        /// </summary>
        [JsonIgnore]
        public int Wins
        {
            get
            {
                return this.Games.Where(game => (game.White == this && game.Result == Result.White) || (game.Black == this && game.Result == Result.Black)).Count();
            }
        }

        /// <summary>
        /// Gets the number of <see cref="Game"/>s the player has drew.
        /// </summary>
        [JsonIgnore]
        public int Draws
        {
            get
            {
                return this.Games.Where(game => game.Result == Result.Draw).Count();
            }
        }

        /// <summary>
        /// Gets the number of <see cref="Game"/>s the player has lost.
        /// </summary>
        [JsonIgnore]
        public int Losses
        {
            get
            {
                return this.Games.Where(game => (game.White == this && game.Result == Result.Black) || (game.Black == this && game.Result == Result.White)).Count();
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Player"/> class.
        /// </summary>
        /// <param name="name">The player's name.</param>
        public Player(string name)
        {
            this.ID = PlayerDatabase.Players.Count;
            this.Name = name;
            this.Rating = PlayerDatabase.DefaultRating;

            PlayerDatabase.Players.Add(this);
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
