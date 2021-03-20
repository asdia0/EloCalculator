namespace EloCalculator
{
    using System;
    using System.Linq;

    public class Player
    {
        public double rating;

        public string title;

        public string name;

        public int wins;

        public int draws;

        public int losses;

        public Player(string name)
        {
            foreach (Player p in Program.playerList)
            {
                if (name == p.name)
                {
                    throw new Exception("Player: Players cannot have the same name");
                }
            }

            this.name = name;
            this.wins = 0;
            this.draws = 0;
            this.losses = 0;

            if (Program.setPlayerRatings.Keys.ToList().Contains(name))
            {
                this.rating = Program.setPlayerRatings[name];
                Game.UpdatePlayerTitle(this);
            }
            else
            {
                this.rating = Program.startingRating;
            }
            Program.playerList.Add(this);
        }
    }
}
