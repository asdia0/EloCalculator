namespace EloCalculator
{
    using System;

    using Newtonsoft.Json;

    public class Game
    {
        [JsonProperty("ID")]
        public int ID { get; }

        [JsonIgnore]
        public Player White { get; }

        [JsonProperty("White")]
        public int WhiteID
        {
            get
            {
                return this.White.ID;
            }
        }

        [JsonIgnore]
        public Player Black { get; }

        [JsonProperty("Black")]
        public int BlackID
        {
            get
            {
                return this.White.ID;
            }
        }

        [JsonProperty("Result")]
        public Result Result { get; }

        [JsonProperty("DateTime")]
        public DateTime DateTime { get; }

        [JsonProperty("Rated")]
        public bool Rated { get; }

        public Game(Player white, Player black, Result result, DateTime dateTime, bool rated)
        {
            this.ID = GameDatabase.Games.Count;
            this.White = white;
            this.Black = black;
            this.Result = result;
            this.DateTime = dateTime;
            this.Rated = rated;

            GameDatabase.Games.Add(this);

            PlayerDatabase.UpdateRatings(this);
        }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
