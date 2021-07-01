namespace EloCalculator
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class TournamentRound
    {
        /// <summary>
        /// Checks if <see cref="Tournament"/> has been set.
        /// </summary>
        private bool TournamentSet;

        /// <summary>
        /// Checks if <see cref="Number"/> has been set.
        /// </summary>
        private bool NumberSet;

        /// <summary>
        /// The value of <see cref="Tournament"/>.
        /// </summary>
        private Tournament _Tournament;

        /// <summary>
        /// The value of <see cref="Number"/>.
        /// </summary>
        private int _Number;

        /// <summary>
        /// Represents the <see cref="Tournament"/> the <see cref="TournamentRound"/> is part of.
        /// </summary>
        public Tournament Tournament
        {
            get
            {
                return this._Tournament;
            }

            set
            {
                if (this.TournamentSet)
                {
                    throw new Exception("Name has already been set.");
                }

                this.TournamentSet = true;
                this._Tournament = value;
            }
        }

        /// <summary>
        /// Represents the round number of the <see cref="TournamentRound"/>.
        /// </summary>
        public int Number
        {
            get
            {
                return this._Number;
            }

            set
            {
                if (this.NumberSet)
                {
                    throw new Exception("Name has already been set.");
                }

                this.NumberSet = true;
                this._Number = value;
            }
        }

        /// <summary>
        /// A list of <see cref="Game"/>s that took place during the <see cref="TournamentRound"/>.
        /// </summary>
        public List<Game> Games = new List<Game>();

        /// <summary>
        /// Initialises a new instance of the <see cref="TournamentRound"/> class.
        /// </summary>
        /// <param name="tournament">The <see cref="Tournament"/> the <see cref="TournamentRound"/> is part of.</param>
        /// <param name="number">The round number of the <see cref="TournamentRound"/>.</param>
        public TournamentRound(Tournament tournament, int number)
        {
            this.Tournament = tournament;
            this.Number = number;
            
            if (!this.Tournament.Rounds.Where(i => i.Number == number).Any())
            {
                this.Tournament.Rounds.Add(this);
            }
        }

        /// <summary>
        /// Adds a <see cref="Game"/> to <see cref="Games"/> and updates the <see cref="Tournament"/>'s stats.
        /// </summary>
        /// <param name="game">The <see cref="Game"/> to add</param>
        public void AddGame(Game game)
        {
            game.Tournament = this.Tournament;
            game.TournamentRound = this;

            this.Games.Add(game);

            if (!this.Tournament.Players.Where(i => i.Player == game.White).Any())
            {
                this.Tournament.Players.Add(new TournamentPlayer(this.Tournament, game.White, true));
            }

            if (!this.Tournament.Players.Where(i => i.Player == game.Black).Any())
            {
                this.Tournament.Players.Add(new TournamentPlayer(this.Tournament, game.Black, true));
            }

            this.Tournament.UpdateStats();
        }
    }
}
