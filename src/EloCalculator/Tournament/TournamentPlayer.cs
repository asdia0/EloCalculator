namespace EloCalculator
{
    using System;
    using System.Data;
    using System.Data.SqlClient;

    public class TournamentPlayer
    {
        /// <summary>
        /// Checks if <see cref="Tournament"/> has been set.
        /// </summary>
        private bool TournamentSet = false;

        /// <summary>
        /// Checks if <see cref="Player"/> has been set.
        /// </summary>
        private bool PlayerSet = false;

        /// <summary>
        /// The value of <see cref="Tournament"/>.
        /// </summary>
        private Tournament _Tournament;

        /// <summary>
        /// The value of <see cref="Player"/>.
        /// </summary>
        private Player _Player;

        private bool _Active;

        /// <summary>
        /// Represents the <see cref="EloCalculator.Tournament"/> the <see cref="TournamentPlayer"/> took part in.
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
                    throw new Exception("Tournament has already been set.");
                }

                this.TournamentSet = true;
                this._Tournament = value;
            }
        }

        /// <summary>
        /// Represents the <see cref="TournamentPlayer"/>'s <see cref="Player"/> record.
        /// </summary>
        public Player Player
        {
            get
            {
                return this._Player;
            }

            set
            {
                if (this.PlayerSet)
                {
                    throw new Exception("Player has already been set.");
                }

                this.PlayerSet = true;
                this._Player = value;
            }
        }

        /// <summary>
        /// Represents the <see cref="TournamentPlayer"/>'s status. True means the <see cref="TournamentPlayer"/> is still participating in the <see cref="Tournament"/>.
        /// </summary>
        public bool Active
        {
            get
            {
                return this._Active;
            }

            set
            {
                using (SqlConnection connection = new SqlConnection(Settings.connectionString))
                {
                    connection.Open();

                    using (SqlCommand setActive = new SqlCommand($"UPDATE [{this.Tournament.Name}] SET Active = @Active WHERE Player = @Player", connection))
                    {
                        setActive.Parameters.Add("@Active", SqlDbType.Bit).Value = this.Active;
                        setActive.Parameters.Add("@Player", SqlDbType.Int).Value = this.Player.Id;
                    }
                }

                this._Active = value;
            }
        }
        
        /// <summary>
        /// Represents the <see cref="TournamentPlayer"/>'s conventional score. +1 for each win, +1/2 for each draw.
        /// </summary>
        public double Score
        {
            get
            {
                using (SqlConnection connection = new SqlConnection(Settings.connectionString))
                {
                    connection.Open();

                    using (SqlCommand getScore = new SqlCommand($"SELECT Score FROM [{this.Tournament.Name}] WHERE Player = @Player", connection))
                    {
                        getScore.Parameters.Add("@Player", SqlDbType.Int).Value = this.Player.Id;

                        return (double)getScore.ExecuteScalar();
                    }
                }
            }
            
            set
            {
                using (SqlConnection connection = new SqlConnection(Settings.connectionString))
                {
                    connection.Open();

                    using (SqlCommand setScore = new SqlCommand($"UPDATE [{this.Tournament.Name}] SET Score=@Score WHERE Player = @Player", connection))
                    {
                        setScore.Parameters.Add("@Score", SqlDbType.Float).Value = value;
                        setScore.Parameters.Add("@Player", SqlDbType.Int).Value = this.Player.Id;

                        setScore.ExecuteNonQuery();
                    }
                }
            }
        }

        /// <summary>
        /// Represents the <see cref="TournamentPlayer"/>'s Sonneborn-Berger (SB) score. SB = Sum of opponents's conventional score won against + (Sum of opponent's conventional score drew against / 2).
        /// </summary>
        public double SonnebornBerger
        {
            get
            {
                using (SqlConnection connection = new SqlConnection(Settings.connectionString))
                {
                    connection.Open();

                    using (SqlCommand getSB = new SqlCommand($"SELECT [Sonneborn-Berger] FROM [{this.Tournament.Name}] WHERE Player = @Player", connection))
                    {
                        getSB.Parameters.Add("@Player", SqlDbType.Int).Value = this.Player.Id;

                        return (double)getSB.ExecuteScalar();
                    }
                }
            }

            set
            {
                using (SqlConnection connection = new SqlConnection(Settings.connectionString))
                {
                    connection.Open();

                    using (SqlCommand setScore = new SqlCommand($"UPDATE [{this.Tournament.Name}] SET [Sonneborn-Berger]=@SB WHERE Player = @Player", connection))
                    {
                        setScore.Parameters.Add("@SB", SqlDbType.Float).Value = value;
                        setScore.Parameters.Add("@Player", SqlDbType.Int).Value = this.Player.Id;

                        setScore.ExecuteNonQuery();
                    }
                }
            }
        }

        /// <summary>
        /// Represents the <see cref="TournamentPlayer"/>'s Buchholz score. Buchholz = Sum of opponents's conventional score won against + Sum of opponent's conventional score drew against.
        /// </summary>
        public double Buchholz
        {
            get
            {
                using (SqlConnection connection = new SqlConnection(Settings.connectionString))
                {
                    connection.Open();

                    using (SqlCommand getScore = new SqlCommand($"SELECT Buchholz FROM [{this.Tournament.Name}] WHERE Player = @Player", connection))
                    {
                        getScore.Parameters.Add("@Player", SqlDbType.Int).Value = this.Player.Id;

                        return (double)getScore.ExecuteScalar();
                    }
                }
            }

            set
            {
                using (SqlConnection connection = new SqlConnection(Settings.connectionString))
                {
                    connection.Open();

                    using (SqlCommand setScore = new SqlCommand($"UPDATE [{this.Tournament.Name}] SET Buchholz=@BH WHERE Player = @Player", connection))
                    {
                        setScore.Parameters.Add("@BH", SqlDbType.Float).Value = value;
                        setScore.Parameters.Add("@Player", SqlDbType.Int).Value = this.Player.Id;

                        setScore.ExecuteNonQuery();
                    }
                }
            }
        }

        /// <summary>
        /// Initialises a new instance of the <see cref="TournamentPlayer"/> class and adds the record to the database.
        /// </summary>
        /// <param name="tournament">The <see cref="EloCalculator.Tournament"/> the <see cref="TournamentPlayer"/> took part in.</param>
        /// <param name="player">The <see cref="TournamentPlayer"/>'s <see cref="Player"/> record.</param>
        /// <param name="active">The <see cref="TournamentPlayer"/>'s status. True means the <see cref="TournamentPlayer"/> is still participating in the <see cref="Tournament"/>.</param>
        public TournamentPlayer(Tournament tournament, Player player, bool active)
        {
            this.Tournament = tournament;
            this.Player = player;
            this.Active = active;

            using (SqlConnection connection = new SqlConnection(Settings.connectionString))
            {
                connection.Open();

                using (SqlCommand addPlayerT = new SqlCommand($"INSERT INTO [{this.Tournament.Name}](Player, Active, Score, [Sonneborn-Berger], Buchholz) VALUES(@Player, @Active, 0, 0, 0)", connection))
                {
                    addPlayerT.Parameters.Add("@Player", SqlDbType.Int).Value = this.Player.Id;
                    addPlayerT.Parameters.Add("@Active", SqlDbType.Bit).Value = this.Active;

                    addPlayerT.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// Initialises a new instance of the <see cref="TournamentPlayer"/> class.
        /// </summary>
        /// <param name="tournament">The <see cref="EloCalculator.Tournament"/> the <see cref="TournamentPlayer"/> took part in.</param>
        /// <param name="player">The <see cref="TournamentPlayer"/>'s <see cref="Player"/> record.</param>
        /// <param name="active">The <see cref="TournamentPlayer"/>'s status. True means the <see cref="TournamentPlayer"/> is still participating in the <see cref="Tournament"/>.</param>
        /// <param name="score">The <see cref="TournamentPlayer"/>'s conventional score. +1 for each win, +1/2 for each draw.</param>
        /// <param name="sonnebornBerger">The <see cref="TournamentPlayer"/>'s Sonneborn-Berger (SB) score. SB = Sum of opponents's conventional score won against + (Sum of opponent's conventional score drew against / 2).</param>
        /// <param name="buchholz">The <see cref="TournamentPlayer"/>'s Buchholz score. Buchholz = Sum of opponents's conventional score won against + Sum of opponent's conventional score drew against.</param>
        public TournamentPlayer(Tournament tournament, Player player, bool active, double score, double sonnebornBerger, double buchholz)
        {
            this.Tournament = tournament;
            this.Player = player;
            this.Active = active;
            this.Score = score;
            this.SonnebornBerger = sonnebornBerger;
            this.Buchholz = buchholz;
        }

        /// <summary>
        /// Updates <see cref="Score"/>.
        /// </summary>
        /// <param name="side">The <see cref="Side"/> of the <see cref="TournamentPlayer"/> in the <see cref="Game"/>.</param>
        /// <param name="result">The <see cref="Game"/>'s <see cref="Game.Result"/>.</param>
        public void UpdateScore(Side side, Result result)
        {
            double increment = 0;

            // Draw
            if (result == Result.Draw)
            {
                increment = 0.5;
            }

            // Won as White
            if (side == Side.White && result == Result.White)
            {
                increment = 1;
            }

            // Won as Black
            else if (side == Side.Black && result == Result.Black)
            {
                increment = 1;
            }

            using (SqlConnection connection = new SqlConnection(Settings.connectionString))
            {
                connection.Open();

                using (SqlCommand incScore = new SqlCommand($"UPDATE [{this.Tournament.Name}] SET Score= Score + @Increment WHERE Player LIKE @Player", connection))
                {
                    incScore.Parameters.Add("@Increment", SqlDbType.Float).Value = increment;
                    incScore.Parameters.Add("@Player", SqlDbType.Int).Value = this.Player.Id;

                    incScore.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// Updates <see cref="SonnebornBerger"/>.
        /// </summary>
        public void UpdateSB()
        {
            double winscore = 0;
            double drawscore = 0;

            // White + Win
            winscore += Tournament.GetOpponentScores(this.Player, Side.White, Result.White);

            // Black + Win
            winscore += Tournament.GetOpponentScores(this.Player, Side.Black, Result.Black);

            // White + Draw
            drawscore += Tournament.GetOpponentScores(this.Player, Side.White, Result.Draw);

            // Black + Draw
            drawscore += Tournament.GetOpponentScores(this.Player, Side.Black, Result.Draw);

            this.SonnebornBerger = winscore + drawscore / 2;
        }

        /// <summary>
        /// Updates <see cref="Buchholz"/>.
        /// </summary>
        public void UpdateBuchholz()
        {
            double score = 0;

            // White + Win
            score += Tournament.GetOpponentScores(this.Player, Side.White, Result.White);
            // Black + Win
            score += Tournament.GetOpponentScores(this.Player, Side.Black, Result.Black);

            // White + Draw
            score += Tournament.GetOpponentScores(this.Player, Side.White, Result.Draw);
            // Black + Draw
            score += Tournament.GetOpponentScores(this.Player, Side.Black, Result.Draw);

            // White + Loss
            score += Tournament.GetOpponentScores(this.Player, Side.White, Result.Black);
            // Black + Loss
            score += Tournament.GetOpponentScores(this.Player, Side.Black, Result.White);

            this.Buchholz = score;
        }

        /// <summary>
        /// Awards the <see cref="TournamentPlayer"/> a 1-point BYE for a <see cref="TournamentRound"/>.
        /// </summary>
        /// <param name="round">The <see cref="TournamentRound"/> to award a BYE for.</param>
        public void AwardBye(TournamentRound round)
        {
            this.Tournament.byes[round] = this;
        }
    }
}
