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
        /// The value of <see cref="ByeReceiver"/>.
        /// </summary>
        private TournamentPlayer _ByeReceiver;

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
        /// Represents the <see cref="TournamentPlayer"/> that received a 1-point BYE during the <see cref="TournamentRound"/>.
        /// </summary>
        public TournamentPlayer? ByeReceiver
        {
            get
            {
                return this._ByeReceiver;
            }

            set
            {
                // if player has not played any games this round.
                if (!Games.Where(i => i.White == value.Player).ToList().Any() || !Games.Where(i => i.Black == value.Player).ToList().Any())
                {
                    // deduct bye from previous receiver
                    this._ByeReceiver.Score--;

                    // update field
                    this._ByeReceiver = value;

                    // update stats
                    this.Tournament.UpdateStats();
                }
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

            // if bye receiver played in this game.
            if (this.ByeReceiver.Player == game.White || this.ByeReceiver.Player == game.Black)
            {
                // deduct bye
                this.ByeReceiver.Score--;

                this.ByeReceiver = null;
            }

            this.Tournament.UpdateStats();
        }
    }
}
