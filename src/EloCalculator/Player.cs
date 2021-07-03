namespace EloCalculator
{
    using System.Collections.Generic;
    using System.Linq;
    using Newtonsoft.Json;

    public class Player
    {
        [JsonProperty]
        public int ID { get; }

        [JsonProperty]
        public string Name { get; }

        [JsonProperty]
        public double Rating { get; set; }

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
                return GameDatabase.Games.Where(game => game.White == this || game.Black == this).ToList();
            }
        }

        [JsonIgnore]
        public int Wins
        {
            get
            {
                return this.Games.Where(game => (game.White == this && game.Result == Result.White) || (game.Black == this && game.Result == Result.Black)).Count();
            }
        }

        [JsonIgnore]
        public int Draws
        {
            get
            {
                return this.Games.Where(game => game.Result == Result.Draw).Count();
            }
        }

        [JsonIgnore]
        public int Losses
        {
            get
            {
                return this.Games.Where(game => (game.White == this && game.Result == Result.Black) || (game.Black == this && game.Result == Result.White)).Count();
            }
        }

        public Player(string name)
        {
            this.ID = PlayerDatabase.Players.Count;
            this.Name = name;
            this.Rating = PlayerDatabase.defaultRating;

            PlayerDatabase.Players.Add(this);
        }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
