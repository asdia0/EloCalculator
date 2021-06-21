namespace EloCalculator
{
    using System;
    using System.Data;
    using System.Data.SqlClient;

    /// <summary>
    /// Represents a player in a <see cref="Game"/>.
    /// </summary>
    public class Player
    {
        /// <summary>
        /// Checks if <see cref="Id"/> has been set.
        /// </summary>
        private bool IdSet = false;

        /// <summary>
        /// The value of <see cref="Id"/>.
        /// </summary>
        private int _Id;

        /// <summary>
        /// Represent's the <see cref="Player"/>'s ID.
        /// </summary>
        public int Id
        {
            get
            {
                return this._Id;
            }

            set
            {
                if (this.IdSet)
                {
                    throw new Exception("Id value has already been set.");
                }

                this.IdSet = true;

                this._Id = value;
            }
        }

        /// <summary>
        /// Represent's the <see cref="Player"/>'s name.
        /// </summary>
        public string Name
        {
            get
            {
                using (SqlConnection connection = new SqlConnection(Settings.connectionString))
                {
                    connection.Open();

                    using (SqlCommand getName = new SqlCommand($"SELECT Name FROM Player WHERE Id = @ID", connection))
                    {
                        getName.Parameters.Add("@ID", SqlDbType.Int).Value = this.Id;

                        return (string)getName.ExecuteScalar();
                    }
                }
            }

            set
            {
                using (SqlConnection connection = new SqlConnection(Settings.connectionString))
                {
                    connection.Open();

                    using (SqlCommand setName = new SqlCommand("UPDATE Player SET Name = @Name WHERE Id = @ID", connection))
                    {
                        setName.Parameters.Add("@Name", SqlDbType.Text).Value = value;
                        setName.Parameters.Add("@ID", SqlDbType.Int).Value = this.Id;

                        setName.ExecuteNonQuery();
                    }
                }
            }
        }

        /// <summary>
        /// Represent's the <see cref="Player"/>'s rating.
        /// </summary>
        public double Rating
        {
            get
            {
                using (SqlConnection connection = new SqlConnection(Settings.connectionString))
                {
                    connection.Open();

                    using (SqlCommand getName = new SqlCommand($"SELECT Rating FROM Player WHERE Id = @ID", connection))
                    {
                        getName.Parameters.Add("@ID", SqlDbType.Int).Value = this.Id;

                        return (double)getName.ExecuteScalar();
                    }
                }
            }

            set
            {
                using (SqlConnection connection = new SqlConnection(Settings.connectionString))
                {
                    connection.Open();

                    using (SqlCommand setRating = new SqlCommand("UPDATE Player SET Rating = @Rating WHERE Id = @ID", connection))
                    {
                        setRating.Parameters.Add("@Rating", SqlDbType.Float).Value = value;
                        setRating.Parameters.Add("@ID", SqlDbType.Int).Value = this.Id;

                        setRating.ExecuteNonQuery();
                    }
                }
            }
        }

        /// <summary>
        /// Represent's the <see cref="Player"/>'s wins.
        /// </summary>
        public int Wins
        {
            get
            {
                using (SqlConnection connection = new SqlConnection(Settings.connectionString))
                {
                    connection.Open();

                    using (SqlCommand getName = new SqlCommand($"SELECT Wins FROM Player WHERE Id = @ID", connection))
                    {
                        getName.Parameters.Add("@ID", SqlDbType.Int).Value = this.Id;

                        return (int)getName.ExecuteScalar();
                    }
                }
            }

            set
            {
                using (SqlConnection connection = new SqlConnection(Settings.connectionString))
                {
                    connection.Open();

                    using (SqlCommand setWins = new SqlCommand("UPDATE Player SET Wins = @Wins WHERE Id = @ID", connection))
                    {
                        setWins.Parameters.Add("@Wins", SqlDbType.Int).Value = value;
                        setWins.Parameters.Add("@ID", SqlDbType.Int).Value = this.Id;

                        setWins.ExecuteNonQuery();
                    }
                }
            }
        }

        /// <summary>
        /// Represent's the <see cref="Player"/>'s losses.
        /// </summary>
        public int Losses
        {
            get
            {
                using (SqlConnection connection = new SqlConnection(Settings.connectionString))
                {
                    connection.Open();

                    using (SqlCommand getName = new SqlCommand($"SELECT Losses FROM Player WHERE Id = @ID", connection))
                    {
                        getName.Parameters.Add("@ID", SqlDbType.Int).Value = this.Id;

                        return (int)getName.ExecuteScalar();
                    }
                }
            }

            set
            {
                using (SqlConnection connection = new SqlConnection(Settings.connectionString))
                {
                    connection.Open();

                    using (SqlCommand setLosses = new SqlCommand("UPDATE Player SET Losses = @Losses WHERE Id = @ID", connection))
                    {
                        setLosses.Parameters.Add("@Losses", SqlDbType.Int).Value = value;
                        setLosses.Parameters.Add("@ID", SqlDbType.Int).Value = this.Id;

                        setLosses.ExecuteNonQuery();
                    }
                }
            }
        }

        /// <summary>
        /// Represent's the <see cref="Player"/>'s draws.
        /// </summary>
        public int Draws
        {
            get
            {
                using (SqlConnection connection = new SqlConnection(Settings.connectionString))
                {
                    connection.Open();

                    using (SqlCommand getName = new SqlCommand($"SELECT Draws FROM Player WHERE Id = @ID", connection))
                    {
                        getName.Parameters.Add("@ID", SqlDbType.Int).Value = this.Id;

                        return (int)getName.ExecuteScalar();
                    }
                }
            }

            set
            {
                using (SqlConnection connection = new SqlConnection(Settings.connectionString))
                {
                    connection.Open();

                    using (SqlCommand setDraws = new SqlCommand("UPDATE Player SET Draws = @Draws WHERE Id = @ID", connection))
                    {
                        setDraws.Parameters.Add("@Draws", SqlDbType.Int).Value = value;
                        setDraws.Parameters.Add("@ID", SqlDbType.Int).Value = this.Id;

                        setDraws.ExecuteNonQuery();
                    }
                }
            }
        }

        /// <summary>
        /// Initialises a new instance of the <see cref="Player"/> class and adds the record to the database.
        /// </summary>
        /// <param name="name">The <see cref="Player"/>'s name.</param>
        public Player(string name)
        {
            this.Name = name;
            this.Rating = PlayerDatabase.defaultRating;
            this.Wins = 0;
            this.Losses = 0;
            this.Draws = 0;

            using (SqlConnection connection = new SqlConnection(Settings.connectionString))
            {
                connection.Open();

                using (SqlCommand addPlayer = new SqlCommand("INSERT INTO Player(Name, Rating, Wins, Losses, Draws) VALUES(@Name, @Rating, 0, 0, 0);", connection))
                {
                    addPlayer.Parameters.Add("@Name", SqlDbType.Text).Value = this.Name;
                    addPlayer.Parameters.Add("@Rating", SqlDbType.Float).Value = this.Rating;

                    addPlayer.ExecuteNonQuery();
                }
            }

            this.Id = PlayerDatabase.GetIDOfLastRecord();

            PlayerDatabase.Players.Add(this.Id, this);
        }

        /// <summary>
        /// Intiialises a new instance of the <see cref="Player"/> class.
        /// </summary>
        /// <param name="id">The <see cref="Player"/>'s ID.</param>
        /// <param name="name">The <see cref="Player"/>'s name.</param>
        /// <param name="rating">The <see cref="Player"/>'s rating.</param>
        /// <param name="wins">The <see cref="Player"/>'s wins.</param>
        /// <param name="losses">The <see cref="Player"/>'s losses.</param>
        /// <param name="draws">The <see cref="Player"/>'s draws.</param>
        public Player(int id, string name, double rating, int wins, int losses, int draws)
        {
            this.Id = id;
            this.Name = name;
            this.Rating = rating;
            this.Wins = wins;
            this.Losses = losses;
            this.Draws = draws;
        }

        /// <summary>
        /// Updates <see cref="Wins"/>, <see cref="Losses"/> and <see cref="Draws"/>.
        /// </summary>
        /// <param name="side">The <see cref="Player"/>'s <see cref="Side"/> in the <see cref="Game"/>.</param>
        /// <param name="result">The <see cref="Game.Result"/>.</param>
        public void UpdateWLD(Side side, Result result)
        {
            // Draw
            if (result == Result.Draw)
            {
                this.Draws++;
                return;
            }
            // White
            if (side == Side.White)
            {
                // Won
                if (result == Result.White)
                {
                    this.Wins++;
                }
                // Lost
                else
                {
                    this.Losses++;
                }
            }
            // Black
            else
            {
                // Won
                if (result == Result.Black)
                {
                    this.Wins++;
                }
                // Lost
                else
                {
                    this.Losses++;
                }
            }
        }
    }
}
