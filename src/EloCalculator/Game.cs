﻿namespace EloCalculator
{
    using System;
    using System.Data;
    using System.Data.SqlClient;
    using System.Linq;

    /// <summary>
    /// Represents a game.
    /// </summary>
    public class Game
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
        /// Checks if <see cref="Tournament"/> has been set.
        /// </summary>
        private bool TournamentSet = false;

        private Tournament? _Tournament;

        private bool RoundSet = false;

        private TournamentRound? _Round;

        private bool _Rated;

        private Player _White;

        private Player _Black;

        private Result _Result;

        /// <summary>
        /// Represents the <see cref="Game"/>'s ID.
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
        /// Represents the <see cref="Player"/> behind the white pieces in the <see cref="Game"/>.
        /// </summary>
        public Player White
        {
            get
            {
                return this._White;
            }

            set
            {
                // Update DB
                using (SqlConnection connection = new SqlConnection(Settings.connectionString))
                {
                    connection.Open();

                    using (SqlCommand setWhite = new SqlCommand("UPDATE Game SET White = @White WHERE Id=@ID", connection))
                    {
                        setWhite.Parameters.Add("@White", SqlDbType.Int).Value = value.Id;
                        setWhite.Parameters.Add("@ID", SqlDbType.Int).Value = this.Id;

                        setWhite.ExecuteNonQuery();
                    }
                }

                if (this.Rated)
                {
                    PlayerDatabase.RemakeDatabase();
                }

                this.Tournament.UpdateStats();

                this._White = value;
            }
        }

        /// <summary>
        /// Represents the <see cref="Player"/> behind the black pieces in the <see cref="Game"/>.
        /// </summary>
        public Player Black
        {
            get
            {
                return this._Black;
            }

            set
            {
                // Update DB
                using (SqlConnection connection = new SqlConnection(Settings.connectionString))
                {
                    connection.Open();

                    using (SqlCommand setBlack = new SqlCommand("UPDATE Game SET Black = @Black WHERE Id=@ID", connection))
                    {
                        setBlack.Parameters.Add("@Black", SqlDbType.Int).Value = value.Id;
                        setBlack.Parameters.Add("@ID", SqlDbType.Int).Value = this.Id;

                        setBlack.ExecuteNonQuery();
                    }
                }

                if (this.Rated)
                {
                    PlayerDatabase.RemakeDatabase();
                }

                this.Tournament.UpdateStats();

                this._Black = value;
            }
        }

        /// <summary>
        /// Represents the <see cref="Game"/>'s result.
        /// </summary>
        public Result Result
        {
            get
            {
                return this._Result;
            }

            set
            {
                // Update DB
                using (SqlConnection connection = new SqlConnection(Settings.connectionString))
                {
                    connection.Open();

                    using (SqlCommand setResult = new SqlCommand("UPDATE Game SET Result = @Result WHERE Id=@ID", connection))
                    {
                        setResult.Parameters.Add("@Result", SqlDbType.Bit).Value = value;
                        setResult.Parameters.Add("@ID", SqlDbType.Int).Value = this.Id;

                        setResult.ExecuteNonQuery();
                    }
                }

                if (this.Rated)
                {
                    PlayerDatabase.RemakeDatabase();
                }

                this.Tournament.UpdateStats();

                this._Result = value;
            }
        }

        /// <summary>
        /// Represents when the <see cref="Game"/> occurred..
        /// </summary>
        public DateTime DateTime
        {
            get
            {
                using (SqlConnection connection = new SqlConnection(Settings.connectionString))
                {
                    connection.Open();

                    using (SqlCommand getDateTime = new SqlCommand($"SELECT DateTime FROM Game WHERE Id = @ID", connection))
                    {
                        getDateTime.Parameters.Add("@ID", SqlDbType.Int).Value = this.Id;

                        return (DateTime)getDateTime.ExecuteScalar();
                    }
                }
            }

            set
            {
                // Update DB
                using (SqlConnection connection = new SqlConnection(Settings.connectionString))
                {
                    connection.Open();

                    using (SqlCommand setDateTime = new SqlCommand("UPDATE Game SET DateTime = @DateTime WHERE Id=@ID", connection))
                    {
                        setDateTime.Parameters.Add("@DateTime", SqlDbType.DateTime).Value = value;
                        setDateTime.Parameters.Add("@ID", SqlDbType.Int).Value = this.Id;

                        setDateTime.ExecuteNonQuery();
                    }
                }

                if (this.Rated)
                {
                    PlayerDatabase.RemakeDatabase();
                }

                this.Tournament.UpdateStats();
            }
        }

        /// <summary>
        /// Represents whether the <see cref="Game"/> affects <see cref="Player.Rating"/>s.
        /// </summary>
        public bool Rated
        {
            get
            {
                return this._Rated;
            }

            set
            {
                // Update DB
                using (SqlConnection connection = new SqlConnection(Settings.connectionString))
                {
                    connection.Open();

                    using (SqlCommand setRated = new SqlCommand("UPDATE Game SET Rated = @Rated WHERE Id=@ID", connection))
                    {
                        setRated.Parameters.Add("@Rated", SqlDbType.Bit).Value = value;
                        setRated.Parameters.Add("@ID", SqlDbType.Int).Value = this.Id;

                        setRated.ExecuteNonQuery();
                    }
                }

                if (value)
                {
                    PlayerDatabase.RemakeDatabase();
                }

                this.Tournament.UpdateStats();

                this._Rated = value;
            }
        }

        /// <summary>
        /// Represents the <see cref="EloCalculator.Tournament"/> the <see cref="Game"/> is part of.
        /// </summary>
        public Tournament? Tournament
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

                // Update DB
                using (SqlConnection connection = new SqlConnection(Settings.connectionString))
                {
                    connection.Open();

                    using (SqlCommand setTournament = new SqlCommand("UPDATE Game SET TournamentName = @Name WHERE Id=@ID", connection))
                    {
                        setTournament.Parameters.Add("@Name", SqlDbType.Text).Value = value.Name;
                        setTournament.Parameters.Add("@ID", SqlDbType.Int).Value = this.Id;

                        setTournament.ExecuteNonQuery();
                    }
                }

                this._Tournament = value;
            }
        }

        /// <summary>
        /// Represents the <see cref="EloCalculator.TournamentRound"/> the <see cref="Game"/> was played in.
        /// </summary>
        public TournamentRound? TournamentRound
        {
            get
            {
                return this._Round;
            }

            set
            {
                if (this.RoundSet)
                {
                    throw new Exception("Round has already been set.");
                }
                // Update DB
                using (SqlConnection connection = new SqlConnection(Settings.connectionString))
                {
                    connection.Open();

                    using (SqlCommand setTournamentRound = new SqlCommand("UPDATE Game SET TournamentRound = @Round WHERE Id=@ID", connection))
                    {
                        setTournamentRound.Parameters.Add("@Round", SqlDbType.Int).Value = value.Number;
                        setTournamentRound.Parameters.Add("@ID", SqlDbType.Int).Value = this.Id;

                        setTournamentRound.ExecuteNonQuery();
                    }
                }

                this._Round = value;
            }
        }

        /// <summary>
        /// Initialises a new instance of the <see cref="Game"/> class and adds the record to the database.
        /// </summary>
        /// <param name="white">Represents the <see cref="Player"/> behind the white pieces.</param>
        /// <param name="black">Represents the <see cref="Player"/> behind the black pieces.</param>
        /// <param name="result">Represents the <see cref="Game"/>'s result.</param>
        /// <param name="dateTime">Represents when the <see cref="Game"/> occurred.</param>
        /// <param name="rated">Represents whether the <see cref="Game"/> affects <see cref="Player.Rating"/>s.</param>
        public Game(Player white, Player black, Result result, DateTime dateTime, bool rated)
        {
            this.Rated = rated;
            this.White = white;
            this.Black = black;
            this.Result = result;
            this.DateTime = dateTime;

            // add game
            using (SqlConnection connection = new SqlConnection(Settings.connectionString))
            {
                connection.Open();

                using (SqlCommand addGame = new SqlCommand($"INSERT INTO Game(White, Black, Result, DateTime, Rated) VALUES(@White, @Black, @Result, @DateTime, @Rated);", connection))
                {
                    addGame.Parameters.Add("@White", SqlDbType.Text).Value = this.White.Name;
                    addGame.Parameters.Add("@Black", SqlDbType.Text).Value = this.Black.Name;
                    if (result == Result.Draw)
                    {
                        addGame.Parameters.Add("@Result", SqlDbType.Bit).Value = DBNull.Value;
                    }
                    else if (result == Result.White)
                    {
                        addGame.Parameters.Add("@Result", SqlDbType.Bit).Value = true;
                    }
                    else
                    {
                        addGame.Parameters.Add("@Result", SqlDbType.Bit).Value = false;
                    }
                    addGame.Parameters.Add("@DateTime", SqlDbType.DateTime).Value = this.DateTime.ToString("yyyy-MM-dd HH:mm:ss");
                    addGame.Parameters.Add("@Rated", SqlDbType.Bit).Value = this.Rated;

                    addGame.ExecuteNonQuery();
                }
            }

            if (this.Rated)
            {
                // get player ratings
                (double whiteRating, double blackRating) = GameDatabase.CalculateRating(this.White.Rating, this.Black.Rating, this.Result);

                // update player ratings
                this.White.Rating = whiteRating;
                this.Black.Rating = blackRating;
            }

            // update player win-lose-draw
            this.White.UpdateWLD(Side.White, this.Result);
            this.Black.UpdateWLD(Side.Black, this.Result);

            this.Id = GameDatabase.GetIDOfLastRecord();

            GameDatabase.Games.Add(this.Id, this);
        }

        /// <summary>
        /// Initialises a new instance of the <see cref="Game"/> class and adds the record to the database.
        /// </summary>
        /// <param name="white">Represents the <see cref="Player"/> behind the white pieces.</param>
        /// <param name="black">Represents the <see cref="Player"/> behind the black pieces.</param>
        /// <param name="result">Represents the <see cref="Game"/>'s result.</param>
        /// <param name="dateTime">Represents when the <see cref="Game"/> occurred.</param>
        /// <param name="rated">Represents whether the <see cref="Game"/> affects <see cref="Player.Rating"/>s.</param>
        /// <param name="tournament">Represents the <see cref="EloCalculator.Tournament"/> the <see cref="Game"/> was played in.</param>
        /// <param name="tournamentRound">Represents the <see cref="EloCalculator.TournamentRound"/> the <see cref="Game"/> was played in.</param>
        public Game(Player white, Player black, Result result, DateTime dateTime, bool rated, Tournament tournament, TournamentRound tournamentRound)
        {
            this.Tournament = tournament;
            this.TournamentRound = tournamentRound;
            this.Rated = rated;
            this.White = white;
            this.Black = black;
            this.Result = result;
            this.DateTime = dateTime;

            // add game
            using (SqlConnection connection = new SqlConnection(Settings.connectionString))
            {
                connection.Open();

                using (SqlCommand addGame = new SqlCommand($"INSERT INTO Game(White, Black, Result, DateTime, Rated, TournamentName, TournamentRound) VALUES(@White, @Black, @Result, @DateTime, @Rated, @TName, @TRound);", connection))
                {
                    addGame.Parameters.Add("@White", SqlDbType.Int).Value = white.Id;
                    addGame.Parameters.Add("@Black", SqlDbType.Int).Value = black.Id;
                    if (result == Result.Draw)
                    {
                        addGame.Parameters.Add("@Result", SqlDbType.Bit).Value = DBNull.Value;
                    }
                    else if (result == Result.White)
                    {
                        addGame.Parameters.Add("@Result", SqlDbType.Bit).Value = true;
                    }
                    else
                    {
                        addGame.Parameters.Add("@Result", SqlDbType.Bit).Value = false;
                    }
                    addGame.Parameters.Add("@DateTime", SqlDbType.DateTime).Value = dateTime.ToString("yyyy-MM-dd HH:mm:ss");
                    addGame.Parameters.Add("@Rated", SqlDbType.Bit).Value = rated;
                    addGame.Parameters.Add("@TName", SqlDbType.Text).Value = tournament.Name;
                    addGame.Parameters.Add("@TRound", SqlDbType.Int).Value = tournamentRound.Number;

                    addGame.ExecuteNonQuery();
                }
            }

            if (this.Rated)
            {
                // get player ratings
                (double whiteRating, double blackRating) = GameDatabase.CalculateRating(this.White.Rating, this.Black.Rating, this.Result);

                // update player ratings
                this.White.Rating = whiteRating;
                this.Black.Rating = blackRating;
            }

            // update player win-lose-draw
            this.White.UpdateWLD(Side.White, this.Result);
            this.Black.UpdateWLD(Side.Black, this.Result);

            this.Id = GameDatabase.GetIDOfLastRecord();

            GameDatabase.Games.Add(this.Id, this);
        }

        /// <summary>
        /// Initialises a new instance of the <see cref="Game"/> class.
        /// </summary>
        /// <param name="white">Represents the <see cref="Player"/> behind the white pieces.</param>
        /// <param name="black">Represents the <see cref="Player"/> behind the black pieces.</param>
        /// <param name="result">Represents the <see cref="Game"/>'s result.</param>
        /// <param name="dateTime">Represents when the <see cref="Game"/> occurred.</param>
        /// <param name="rated">Represents whether the <see cref="Game"/> affects <see cref="Player.Rating"/>s.</param>
        public Game(int id, Player white, Player black, Result result, DateTime dateTime, bool rated)
        {
            this.Id = id;
            this.Rated = rated;
            this.White = white;
            this.Black = black;
            this.Result = result;
            this.DateTime = dateTime;
        }

        /// <summary>
        /// Initialises a new instance of the <see cref="Game"/> class.
        /// </summary>
        /// <param name="white">Represents the <see cref="Player"/> behind the white pieces.</param>
        /// <param name="black">Represents the <see cref="Player"/> behind the black pieces.</param>
        /// <param name="result">Represents the <see cref="Game"/>'s result.</param>
        /// <param name="dateTime">Represents when the <see cref="Game"/> occurred.</param>
        /// <param name="rated">Represents whether the <see cref="Game"/> affects <see cref="Player.Rating"/>s.</param>
        /// <param name="tournament">Represents the <see cref="EloCalculator.Tournament"/> the <see cref="Game"/> was played in.</param>
        /// <param name="tournamentRound">Represents the <see cref="EloCalculator.TournamentRound"/> the <see cref="Game"/> was played in.</param>
        public Game(int id, Player white, Player black, Result result, DateTime dateTime, bool rated, Tournament tournament, TournamentRound tournamentRound)
        {
            this.Id = id;
            this.Tournament = tournament;
            this.TournamentRound = tournamentRound;
            this.Rated = rated;
            this.White = white;
            this.Black = black;
            this.Result = result;
            this.DateTime = dateTime;
        }
    }
}
