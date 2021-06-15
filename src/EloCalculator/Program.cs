namespace EloCalculator
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Data;
    using System.Data.SqlClient;
    using System.Linq;

    public class Program
    {
        public static string connectionString = ConfigurationManager.AppSettings.Get("SQLConnectionString");

        public static int defaultRating = 1000;

        public static int kCoeff = 40;

        public static int benchmarkCoeff = 400;

        public static void Main(string[] args)
        {
            foreach ((string, double) elem in GetGlobalRankings())
            {
                Console.WriteLine(elem);
            }
        }

        /// <summary>
        /// Adds a game and updates the players' info.
        /// </summary>
        /// <param name="white">White player's name.</param>
        /// <param name="black">Black player's name.</param>
        /// <param name="result">Result of the game. True = white won, False = black won, Null = draw.</param>
        /// <param name="dateTime">When the game was played.</param>
        /// <param name="isRated">Whether the game affects the player's ratings.</param>
        public static void NewGame(string white, string black, bool? result, DateTime dateTime, bool isRated)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                // add game
                AddGame(white, black, result, dateTime, isRated);

                if (isRated)
                {
                    // get player ratings
                    (double whiteRating, double blackRating) = CalculateRating(GetRating(white), GetRating(black), result);

                    // update player ratings
                    UpdatePlayerRatings(white, whiteRating);
                    UpdatePlayerRatings(black, blackRating);
                }

                // update player win-lose-draw
                UpdatePlayerWLD(white, true, result);
                UpdatePlayerWLD(black, false, result);
            }
        }

        /// <summary>
        /// Calculates the ratings of the players after a game.
        /// </summary>
        /// <param name="whiteRating">White's initial rating.</param>
        /// <param name="blackRating">Black's initial rating.</param>
        /// <param name="result">Result of the game. True = white won, False = black won, Null = draw.</param>
        /// <returns>A tuplet of doubles. The first item is white's new rating, the second item is black's new rating.</returns>
        public static (double, double) CalculateRating(double whiteRating, double blackRating, bool? result)
        {
            double scoreA, scoreB = 0;

            switch (result)
            {
                case (true):
                    {
                        scoreA = 1;
                        scoreB = 0;
                        break;
                    }
                case (false):
                    {
                        scoreA = 0;
                        scoreB = 1;
                        break;
                    }
                case (null):
                    {
                        scoreA = scoreB = 0.5;
                        break;
                    }
            }

            double expectedScoreA = 1 / (1 + Math.Pow(10, (blackRating - whiteRating) / benchmarkCoeff));

            double expectedScoreB = 1 / (1 + Math.Pow(10, (whiteRating - blackRating) / benchmarkCoeff));

            return (whiteRating + kCoeff * (scoreA - expectedScoreA), blackRating + kCoeff * (scoreB - expectedScoreB));
        }

        /// <summary>
        /// Gets the ID of the player from the database.
        /// </summary>
        /// <param name="name">The player's name.</param>
        /// <returns></returns>
        public static int GetID(string name)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                using (SqlCommand getID = new SqlCommand($"SELECT Id FROM Player WHERE Name LIKE @Name", connection))
                {
                    getID.Parameters.Add("@Name", SqlDbType.Text).Value = name;
                    var res =  getID.ExecuteScalar();

                    if (res == null)
                    {
                        return 0;
                    }
                    else
                    {
                        return (int)res;
                    }
                }
            }
        }

        /// <summary>
        /// Gets the rating of the player from the database.
        /// </summary>
        /// <param name="name">The player's name.</param>
        /// <returns></returns>
        public static double GetRating(string name)
        {
            int PlayerID = GetID(name);
            
            while (PlayerID == 0)
            {
                // will probably cause some problems later on, but who cares?
                AddPlayer(name);
                PlayerID = GetID(name);
            }

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                using (SqlCommand getRating = new SqlCommand($"SELECT Rating FROM Player WHERE Id = @PlayerID", connection))
                {
                    getRating.Parameters.Add("@PlayerID", SqlDbType.Int).Value = PlayerID;
                    return (double)getRating.ExecuteScalar();
                }
            }
        }

        /// <summary>
        /// Adds a player to the database.
        /// </summary>
        /// <param name="name">The player's name.</param>
        public static void AddPlayer(string name)
        {
            if (GetID(name) == 0)
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    using (SqlCommand addPlayer = new SqlCommand("INSERT INTO Player(Name, Rating, Wins, Losses, Draws) VALUES(@Name, @Rating, @Wins, @Losses, @Draws);", connection))
                    {
                        addPlayer.Parameters.Add("@Name", SqlDbType.Text).Value = name;
                        addPlayer.Parameters.Add("@Rating", SqlDbType.Float).Value = defaultRating;
                        addPlayer.Parameters.Add("@Wins", SqlDbType.Int).Value = 0;
                        addPlayer.Parameters.Add("@Losses", SqlDbType.Int).Value = 0;
                        addPlayer.Parameters.Add("@Draws", SqlDbType.Int).Value = 0;

                        addPlayer.ExecuteNonQuery();
                    }
                }
            }
        }

        /// <summary>
        /// Adds a game to the database.
        /// </summary>
        /// <param name="white">White player's name</param>
        /// <param name="black">Black player's name</param>
        /// <param name="result">Result of the game. True = white won, False = black won, Null = draw.</param>
        /// <param name="dateTime">When the game was played.</param>
        /// <param name="isRated">Whether the game affects the player's ratings.</param>
        public static void AddGame(string white, string black, bool? result, DateTime dateTime, bool isRated)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                using (SqlCommand addGame = new SqlCommand($"INSERT INTO Game(White, Black, Result, DateTime, Rated) VALUES(@White, @Black, @Result, @DateTime, @Rated);", connection))
                {
                    addGame.Parameters.Add("@White", SqlDbType.Text).Value = white;
                    addGame.Parameters.Add("@Black", SqlDbType.Text).Value = black;
                    if (result == null)
                    {
                        addGame.Parameters.Add("@Result", SqlDbType.Bit).Value = DBNull.Value;
                    }
                    else
                    {
                        addGame.Parameters.Add("@Result", SqlDbType.Bit).Value = result;
                    }
                    addGame.Parameters.Add("@DateTime", SqlDbType.DateTime).Value = dateTime.ToString("yyyy-MM-dd HH:mm:ss");
                    addGame.Parameters.Add("@Rated", SqlDbType.Bit).Value = isRated;

                    addGame.ExecuteNonQuery();
                }
            }
        }
        
        /// <summary>
        /// Updates the player's ratings.
        /// </summary>
        /// <param name="name">The player's name.</param>
        /// <param name="rating">The player's new rating.</param>
        public static void UpdatePlayerRatings(string name, double rating)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                using (SqlCommand updateRating = new SqlCommand($"UPDATE Player SET Rating = {rating} WHERE Name LIKE @Name", connection))
                {
                    updateRating.Parameters.Add("@Name", SqlDbType.Text).Value = name;
                    updateRating.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// Updates the player's win-lose-draw stats.
        /// </summary>
        /// <param name="name">The player's name.</param>
        /// <param name="side">The player's colour in the game. True = white, False = black.</param>
        /// <param name="result">Result of the game. True = white won, False = black won, Null = draw.</param>
        public static void UpdatePlayerWLD(string name, bool side, bool? result)
        {
            // Draw
            if (result == null)
            {
                IncrementDraws(name);
                return;
            }

            // White
            if (side)
            {
                // Won
                if (result == true)
                {
                    IncrementWins(name);
                }
                // Lost
                else
                {
                    IncrementLosses(name);
                }
            }
            // Black
            else
            {
                // Won
                if (result == false)
                {
                    IncrementWins(name);
                }
                // Lost
                else
                {
                    IncrementLosses(name);
                }
            }
        }

        /// <summary>
        /// Adds 1 to Player.Draws.
        /// </summary>
        /// <param name="name">The player's name.</param>
        public static void IncrementDraws(string name)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                using (SqlCommand incDraw = new SqlCommand($"UPDATE Player SET Draws = Draws + 1 WHERE Name LIKE @Name", connection))
                {
                    incDraw.Parameters.Add("@Name", SqlDbType.Text).Value = name;
                    incDraw.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// Adds 1 to Player.Wins.
        /// </summary>
        /// <param name="name">The player's name.</param>
        public static void IncrementWins(string name)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                using (SqlCommand incWin = new SqlCommand($"UPDATE Player SET Wins = Wins + 1 WHERE Name LIKE @Name", connection))
                {
                    incWin.Parameters.Add("@Name", SqlDbType.Text).Value = name;
                    incWin.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// Adds 1 to Player.Losses.
        /// </summary>
        /// <param name="name">The player's name.</param>
        public static void IncrementLosses(string name)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                using (SqlCommand incLoss = new SqlCommand($"UPDATE Player SET Losses = Losses + 1 WHERE Name LIKE @Name", connection))
                {
                    incLoss.Parameters.Add("@Name", SqlDbType.Text).Value = name;
                    incLoss.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// Checks if the tables Game and Player exists and adds them if not.
        /// </summary>
        public static void ConfigureDB()
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                // Game
                if (!TableExists("Game"))
                {
                    // Add
                    using (SqlCommand addGame = new SqlCommand("CREATE TABLE [dbo].[Game] ([Id] INT IDENTITY(1, 1) NOT NULL, [White] TEXT NOT NULL, [Black] TEXT NOT NULL, [Result] BIT NULL, [DateTime] DATETIME NOT NULL, [Rated] BIT NOT NULL, [TournamentName] TEXT NULL, [TournamentRound] INT NULL, PRIMARY KEY CLUSTERED([Id] ASC));", connection))
                    {
                        addGame.ExecuteNonQuery();
                    }
                }

                // Player
                if (!TableExists("Player"))
                {
                    // Add
                    using (SqlCommand addPlayer = new SqlCommand("CREATE TABLE [dbo].[Player] ([Id] INT IDENTITY(1, 1) NOT NULL, [Name] TEXT NOT NULL, [Rating] FLOAT(53) NOT NULL, [Wins] INT NOT NULL, [Losses] INT NOT NULL, [Draws]  INT NOT NULL, PRIMARY KEY CLUSTERED([Id] ASC));", connection))
                    {
                        addPlayer.ExecuteNonQuery();
                    }
                }
            }
        }

        /// <summary>
        /// Checks if a table exists in the database.
        /// </summary>
        /// <param name="name">The table's name.</param>
        public static bool TableExists(string name)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                using (SqlCommand checkTable = new SqlCommand($"SELECT CASE WHEN OBJECT_ID('dbo.{name}', 'U') IS NOT NULL THEN 1 ELSE 0 END", connection))
                {
                    return (int)checkTable.ExecuteScalar() == 1;
                }
            }
        }

        /// <summary>
        /// Adds a new tournament table to the database
        /// </summary>
        /// <param name="name">The tournament's name</param>
        public static void AddTournament(string name)
        {
            if (TableExists(name))
            {
                return;
            }

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                using (SqlCommand addTournament = new SqlCommand($"CREATE TABLE [dbo].[{name}] ( [Id] INT NOT NULL IDENTITY(1,1) PRIMARY KEY, [Name] TEXT NOT NULL, [Rating] FLOAT NOT NULL, [Score] FLOAT NOT NULL, [Sonneborn-Berger] FLOAT NOT NULL, [Buchholz] FLOAT NOT NULL);", connection))
                {
                    addTournament.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// Adds a new round to the tournament. THIS SHOULD ONLY BE CALLED ONCE PER ROUND.
        /// </summary>
        /// <param name="name">The tournament's name.</param>
        /// <param name="games">The games played that round.</param>
        public static void AddTournamentRound(string name, int round, List<int> games)
        {
            foreach (int gameID in games)
            {
                // Update game info
                AddTournamentInfoToGame(gameID, name, round);

                // Update scores
                UpdateScore(name, GetName(true, gameID), true, GetResult(gameID));
                UpdateScore(name, GetName(false, gameID), false, GetResult(gameID));

                // Update tiebreakers
                UpdateSB(name, GetName(true, gameID));
                UpdateSB(name, GetName(false, gameID));

                UpdateBuchholz(name, GetName(true, gameID));
                UpdateBuchholz(name, GetName(false, gameID));
            }
        }

        public static void AddPlayerToTournament(string tournamentName, string playerName)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                using (SqlCommand addPlayerT = new SqlCommand($"INSERT INTO [{tournamentName}](Name, Rating, Score, [Sonneborn-Berger], Buchholz) VALUES(@Name, @Rating, 0, 0, 0)", connection))
                {
                    addPlayerT.Parameters.Add("@Name", SqlDbType.Text).Value = playerName;
                    addPlayerT.Parameters.Add("@Rating", SqlDbType.Float).Value = GetRating(playerName);

                    addPlayerT.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// Gets a player's name from a game.
        /// </summary>
        /// <param name="side">The player's colour in the game. True = white, False = black.</param>
        /// <param name="id">The game's ID.</param>
        /// <returns></returns>
        public static string GetName(bool side, int id)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                using (SqlCommand getName = new SqlCommand($"SELECT {(side ? "White" : "Black")} FROM Game WHERE Id={id}", connection))
                {
                    return getName.ExecuteScalar().ToString();
                }
            }
        }

        /// <summary>
        /// Gets the result of a game.
        /// </summary>
        /// <param name="id">The game's ID.</param>
        /// <returns>True = White won, False = Black won, Null = draw.</returns>
        public static bool? GetResult(int id)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                using (SqlCommand getResult = new SqlCommand("SELECT Result FROM Game WHERE Id=@ID", connection))
                {
                    getResult.Parameters.Add("@ID", SqlDbType.Int).Value = id;

                    var res = getResult.ExecuteScalar();

                    if (res == DBNull.Value)
                    {
                        return null;
                    }
                    return (bool)res;
                }
            }
        }

        /// <summary>
        /// Adds data to the Tournament Name and Tournament Round fields.
        /// </summary>
        /// <param name="id">The game's ID.</param>
        /// <param name="name">The tournament's name.</param>
        /// <param name="round">The tournament's round.</param>
        public static void AddTournamentInfoToGame(int id, string name, int round)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                using (SqlCommand addInfo = new SqlCommand("UPDATE Game SET TournamentName=@Name, TournamentRound=@Round WHERE Id=@ID", connection))
                {
                    addInfo.Parameters.Add("@Name", SqlDbType.Text).Value = name;
                    addInfo.Parameters.Add("@Round", SqlDbType.Int).Value = round;
                    addInfo.Parameters.Add("@ID", SqlDbType.Int).Value = id;

                    addInfo.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// Updates a player's score in a tournament.
        /// </summary>
        /// <param name="tournamentName">The tournament's name.</param>
        /// <param name="id">The player's ID.</param>
        /// <param name="side">The player's colour in the game. True = white, False = black.</param>
        /// <param name="result">Result of the game. True = white won, False = black won, Null = draw.</param>
        public static void UpdateScore(string tournamentName, string playerName, bool side, bool? result)
        {
            double increment = 0;

            // Draw
            if (result == null)
            {
                increment = 0.5;
            }

            // Won as White
            if (side && result == true)
            {
                increment = 1;
            }

            // Won as Black
            else if (!side && result == false)
            {
                increment = 1;
            }

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                using (SqlCommand incScore = new SqlCommand($"UPDATE [{tournamentName}] SET Score= Score + @Increment WHERE Name LIKE @Name", connection))
                {
                    incScore.Parameters.Add("@Increment", SqlDbType.Float).Value = increment;
                    incScore.Parameters.Add("@Name", SqlDbType.Text).Value = playerName;

                    incScore.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// Gets the score of a player in a tournament.
        /// </summary>
        /// <param name="tournamentName">The tournament's name.</param>
        /// <param name="playerName">The player's name.</param>
        public static double? GetTournamentScore(string tournamentName, string playerName)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                using (SqlCommand getTScore = new SqlCommand($"SELECT Score FROM [{tournamentName}] WHERE Name LIKE @Name", connection))
                {
                    getTScore.Parameters.Add("@Name", SqlDbType.Text).Value = playerName;

                    var res = getTScore.ExecuteScalar();

                    if (res == DBNull.Value)
                    {
                        return null;
                    }

                    return (double)res;
                }
            }
        }

        /// <summary>
        /// Updates a player's Sonneborn-Berger score.
        /// </summary>
        /// <param name="tournamentName">The tournament's name.</param>
        /// <param name="playerName">The player's name.</param>
        public static void UpdateSB(string tournamentName, string playerName)
        {
            double winscore = 0;
            double drawscore = 0;

            // White + Win
            winscore += GetOpponentScores(tournamentName, playerName, true, true);
            // Black + Win
            winscore += GetOpponentScores(tournamentName, playerName, false, true);

            // White + Draw
            drawscore += GetOpponentScores(tournamentName, playerName, true, null);
            // Black + Draw
            drawscore += GetOpponentScores(tournamentName, playerName, false, null);

            double SB = winscore + (double)(decimal.Divide((decimal)drawscore, 2));

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                using (SqlCommand updateSB = new SqlCommand($"UPDATE [{tournamentName}] SET [Sonneborn-Berger]=@Score WHERE Name LIKE @Player", connection))
                {
                    updateSB.Parameters.Add("@Score", SqlDbType.Float).Value = SB;
                    updateSB.Parameters.Add("@Player", SqlDbType.Text).Value = playerName;

                    updateSB.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// Updates a player's Buchholz System score.
        /// </summary>
        /// <param name="tournamentName">The tournament's name.</param>
        /// <param name="playerName">The player's name.</param>
        public static void UpdateBuchholz(string tournamentName, string playerName)
        {
            double score = 0;

            // White + Win
            score += GetOpponentScores(tournamentName, playerName, true, true);
            // Black + Win
            score += GetOpponentScores(tournamentName, playerName, false, true);

            // White + Draw
            score += GetOpponentScores(tournamentName, playerName, true, null);
            // Black + Draw
            score += GetOpponentScores(tournamentName, playerName, false, null);

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                using (SqlCommand updateSB = new SqlCommand($"UPDATE [{tournamentName}] SET Buchholz=@Score WHERE Name LIKE @Player", connection))
                {
                    updateSB.Parameters.Add("@Score", SqlDbType.Float).Value = score;
                    updateSB.Parameters.Add("@Player", SqlDbType.Text).Value = playerName;

                    updateSB.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// Gets the sum of opponent scores. Used in SB and Buchholz calculations.
        /// </summary>
        /// <param name="tournamentName">The tournament's name.</param>
        /// <param name="playerName">The player's name.</param>
        /// <param name="side">The player's colour in the game. True = White, False = Black.</param>
        /// <param name="result">The result of the game. True = White won, False = Black won, Null = draw.</param>
        /// <returns></returns>
        public static double GetOpponentScores(string tournamentName, string playerName, bool side, bool? result)
        {
            double score = 0;
            Dictionary<string, double> oppcount = new Dictionary<string, double>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                using (SqlCommand getOpp = new SqlCommand($"SELECT {(side ? "Black" : "White")} FROM Game WHERE {(side ? "White" : "Black")} LIKE @Player AND Result=@Result", connection))
                {
                    getOpp.Parameters.Add("@Player", SqlDbType.Text).Value = playerName;
                    if (result == null)
                    {
                        getOpp.Parameters.Add("@Result", SqlDbType.Bit).Value = DBNull.Value;
                    }
                    else
                    {
                        getOpp.Parameters.Add("@Result", SqlDbType.Bit).Value = result;
                    }
                    SqlDataReader rdr = getOpp.ExecuteReader();
                    while (rdr.Read())
                    {
                        string opp = rdr[(side ? "Black" : "White")].ToString();
                        if (!oppcount.ContainsKey(opp))
                        {
                            oppcount.Add(opp, 1);
                        }
                        else
                        {
                            oppcount[opp]++;
                        }
                    }
                    rdr.Close();
                }

                foreach (string opponent in oppcount.Keys.ToList())
                {
                    using (SqlCommand getOppScore = new SqlCommand($"SELECT Score FROM [{tournamentName}] WHERE Name LIKE @OppName", connection))
                    {
                        getOppScore.Parameters.Add("@OppName", SqlDbType.Text).Value = opponent;

                        score += oppcount[opponent] * (double)getOppScore.ExecuteScalar();
                    }
                }
            }

            return score;
        }
        
        public static List<string> GetGlobalPlayers()
        {
            List<string> res = new List<string>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                using (SqlCommand getInfo = new SqlCommand("SELECT Name FROM Player", connection))
                {
                    var rdr = getInfo.ExecuteReader();

                    while (rdr.Read())
                    {
                        res.Add(rdr["Name"].ToString());
                    }
                }
            }

            return res;
        }

        public static List<(string, double)> GetGlobalRankings()
        {
            List<(string, double)> res = new List<(string, double)>();

            foreach (string name in GetGlobalPlayers())
            {
                res.Add((name, GetRating(name)));
            }

            return res.OrderByDescending(t => t.Item2).ToList();
        }
    }
}
