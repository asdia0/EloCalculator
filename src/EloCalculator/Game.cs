namespace EloCalculator
{
    using System;
    using Newtonsoft.Json;

    /// <summary>
    /// Represents a game.
    /// </summary>
    public class Game
    {
        /// <summary>
        /// Gets the game's unique identification number.
        /// </summary>
        [JsonProperty("ID")]
        public int ID { get; }

        /// <summary>
        /// Gets the <see cref="Player"/> that played as white.
        /// </summary>
        [JsonIgnore]
        public Player White { get; }

        /// <summary>
        /// Gets <see cref="White"/>'s unique identification number.
        /// </summary>
        [JsonProperty("White")]
        public int WhiteID
        {
            get
            {
                return this.White.ID;
            }
        }

        /// <summary>
        /// Gets the <see cref="Player"/> that played as black.
        /// </summary>
        [JsonIgnore]
        public Player Black { get; }

        /// <summary>
        /// Gets <see cref="Black"/>'s unique identification number.
        /// </summary>
        [JsonProperty("Black")]
        public int BlackID
        {
            get
            {
                return this.White.ID;
            }
        }

        /// <summary>
        /// Gets the game's <see cref="Result"/>.
        /// </summary>
        [JsonProperty("Result")]
        public Result Result { get; }

        /// <summary>
        /// Gets the time at which the game was played.
        /// </summary>
        [JsonProperty("DateTime")]
        public DateTime DateTime { get; }

        /// <summary>
        /// Gets a value indicating whether the game affects <see cref="White"/> and <see cref="Black"/>'s ratings.
        /// </summary>
        [JsonProperty("Rated")]
        public bool Rated { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Game"/> class.
        /// </summary>
        /// <param name="white">The <see cref="Player"/> that played as white.</param>
        /// <param name="black">The <see cref="Player"/> that played as black.</param>
        /// <param name="result">The game's <see cref="Result"/>.</param>
        /// <param name="dateTime">The time at which the game was played.</param>
        /// <param name="rated">A value indicating whether the game affects <paramref name="white"/> and <paramref name="black"/>'s ratings.</param>
        public Game(Player white, Player black, Result result, DateTime dateTime, bool rated)
        {
            if (white == black)
            {
                throw new EloCalculatorException("Players must not be the same.");
            }

            this.ID = GameDatabase.Games.Count;
            this.White = white;
            this.Black = black;
            this.Result = result;
            this.DateTime = dateTime;
            this.Rated = rated;

            GameDatabase.Games.Add(this);

            PlayerDatabase.UpdateRatings(this);
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
