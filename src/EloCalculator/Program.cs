namespace EloCalculator
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Data;
    using System.Data.SqlClient;
    using System.Linq;
    using System.IO;

    public class Program
    {
        public static string connectionString = ConfigurationManager.AppSettings.Get("SQLConnectionString");

        public static int defaultRating = 1000;

        public static int kCoeff = 40;

        public static int benchmarkCoeff = 400;

        public static void Main(string[] args)
        {
            //ConfigureDB();
            //NewGame("a", "b", true, DateTime.Now, true);
            //NewGame("c", "d", false, DateTime.Now, true);
            //NewGame("a", "d", null, DateTime.Now, true);
            //NewGame("b", "c", true, DateTime.Now, true);

            //AddTournament("epic tournament");
            //AddPlayerToTournament("epic tournament", "a", true);
            //AddPlayerToTournament("epic tournament", "b", true);
            //AddPlayerToTournament("epic tournament", "c", true);
            //AddPlayerToTournament("epic tournament", "d", true);

            //AddGameToTournament("epic tournament", 1, 1);
            //AddGameToTournament("epic tournament", 1, 2);
            //AddGameToTournament("epic tournament", 2, 3);
            //AddGameToTournament("epic tournament", 2, 4);

            //foreach (var elem in GetPairings("epic tournament"))
            //{
            //    Console.WriteLine(elem);
            //}
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

                using (SqlCommand addTournament = new SqlCommand($"CREATE TABLE [dbo].[{name}] ( [Id] INT NOT NULL IDENTITY(1,1) PRIMARY KEY, [Name] TEXT NOT NULL, [Active] BIT NOT NULL, [Score] FLOAT NOT NULL, [Sonneborn-Berger] FLOAT NOT NULL, [Buchholz] FLOAT NOT NULL);", connection))
                {
                    addTournament.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// Adds new games to a tournament.
        /// </summary>
        /// <param name="name">The tournament's name.</param>
        /// <param name="gameID">The games to add.</param>
        public static void AddGameToTournament(string name, int round, int gameID)
        {
            // Update game info
            AddTournamentInfoToGame(gameID, name, round);

            // Update scores
            UpdateScore(name, GetName(true, gameID), true, GetResult(gameID));
            UpdateScore(name, GetName(false, gameID), false, GetResult(gameID));

            // Update tiebreakers
            foreach (string player in GetTournamentPlayers(name))
            {
                UpdateSB(name, player);

                UpdateBuchholz(name, player);
            }
        }

        /// <summary>
        /// Adds a player to a tournament.
        /// </summary>
        /// <param name="tournamentName">The tournament's name.</param>
        /// <param name="playerName">The player's name.</param>
        public static void AddPlayerToTournament(string tournamentName, string playerName, bool isActive)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                using (SqlCommand addPlayerT = new SqlCommand($"INSERT INTO [{tournamentName}](Name, Active, Score, [Sonneborn-Berger], Buchholz) VALUES(@Name, @Active, 0, 0, 0)", connection))
                {
                    addPlayerT.Parameters.Add("@Name", SqlDbType.Text).Value = playerName;
                    addPlayerT.Parameters.Add("@Active", SqlDbType.Bit).Value = isActive;

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
        public static double GetTournamentScore(string tournamentName, string playerName)
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
                        return 0;
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
            winscore += GetOpponentScores(tournamentName, playerName, false, false);

            // White + Draw
            drawscore += GetOpponentScores(tournamentName, playerName, true, null);
            
            // Black + Draw
            drawscore += GetOpponentScores(tournamentName, playerName, false, null);

            double SB = winscore + drawscore / 2;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                using (SqlCommand updateSB = new SqlCommand($"UPDATE [{tournamentName}] SET [Sonneborn-Berger] = @Score WHERE Name LIKE @Player", connection))
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
            score += GetOpponentScores(tournamentName, playerName, false, false);

            // White + Draw
            score += GetOpponentScores(tournamentName, playerName, true, null);
            // Black + Draw
            score += GetOpponentScores(tournamentName, playerName, false, null);

            // White + Loss
            score += GetOpponentScores(tournamentName, playerName, true, false);
            // Black + Loss
            score += GetOpponentScores(tournamentName, playerName, false, true);

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
            double sum = 0;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string commandText = string.Empty;

                if (result == null)
                {
                    commandText = $"SELECT {(side ? "Black" : "White")} FROM Game WHERE {(side ? "White" : "Black")} LIKE @Player AND TournamentName LIKE @TName AND Result IS NULL";
                }
                else
                {
                    commandText = $"SELECT {(side ? "Black" : "White")} FROM Game WHERE {(side ? "White" : "Black")} LIKE @Player AND TournamentName LIKE @TName AND Result={((bool)result ? 1 : 0)}";
                }

                using (SqlCommand getOpponents = new SqlCommand(commandText, connection))
                {
                    getOpponents.Parameters.Add("@Player", SqlDbType.Text).Value = playerName;
                    getOpponents.Parameters.Add("@TName", SqlDbType.Text).Value = tournamentName;

                    var rdr = getOpponents.ExecuteReader();

                    while (rdr.Read())
                    {
                        string opp = rdr[(side ? "Black" : "White")].ToString();
                        sum += GetTournamentScore(tournamentName, opp);
                    }

                    rdr.Close();
                }
            }

            return sum;
        }
        
        /// <summary>
        /// Gets the list of all players.
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// Gets the global rankings.
        /// </summary>
        /// <returns>A ist of tuples. Format: (name, rating)</returns>
        public static List<(string, double)> GetGlobalRankings()
        {
            List<(string, double)> res = new List<(string, double)>();

            foreach (string name in GetGlobalPlayers())
            {
                res.Add((name, GetRating(name)));
            }

            return res.OrderByDescending(t => t.Item2).ToList();
        }

        /// <summary>
        /// Gets a list of players in a tournament.
        /// </summary>
        /// <param name="name">The tournament's name.</param>
        /// <returns></returns>
        public static List<string> GetTournamentPlayers(string name)
        {
            List<string> res = new List<string>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                using (SqlCommand getInfo = new SqlCommand($"SELECT Name FROM [{name}]", connection))
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

        /// <summary>
        /// Gets the rankings of a tournament.
        /// </summary>
        /// <param name="name">The tournament's name.</param>
        /// <returns>A ist of tuples. Format: (name, score, SB score, Buchholz score)</returns>
        public static List<(string, double, double, double)> GetTournamentRankings(string name)
        {
            List<(string, double, double, double)> res = new List<(string, double, double, double)>();

            foreach (string playerName in GetTournamentPlayers(name))
            {
                res.Add((playerName, GetTournamentScore(name, playerName), GetSB(name, playerName), GetBuchholz(name, playerName)));
            }

            return res.OrderByDescending(t => t.Item2).ThenByDescending(t => t.Item3).ThenByDescending(t => t.Item4).ToList();
        }

        /// <summary>
        /// Gets a player's Sonneborn-Berger score in a tournament.
        /// </summary>
        /// <param name="tournamentName">The tournament's name.</param>
        /// <param name="playerName">The player's name.</param>
        /// <returns></returns>
        public static double GetSB(string tournamentName, string playerName)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                using (SqlCommand getSB = new SqlCommand($"SELECT [Sonneborn-Berger] FROM [{tournamentName}] WHERE Name LIKE @Name", connection))
                {
                    getSB.Parameters.Add("@Name", SqlDbType.Text).Value = playerName;

                    return (double)getSB.ExecuteScalar();
                }
            }
        }

        /// <summary>
        /// Gets a player's Buchholz score in a tournament.
        /// </summary>
        /// <param name="tournamentName">The tournament's name.</param>
        /// <param name="playerName">The player's name.</param>
        /// <returns></returns>
        public static double GetBuchholz(string tournamentName, string playerName)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                using (SqlCommand getBuchholz = new SqlCommand($"SELECT Buchholz FROM [{tournamentName}] WHERE Name LIKE @Name", connection))
                {
                    getBuchholz.Parameters.Add("@Name", SqlDbType.Text).Value = playerName;

                    return (double)getBuchholz.ExecuteScalar();
                }
            }
        }

        /// <summary>
        /// Gets a list of players by their tournament ranking.
        /// </summary>
        /// <param name="tournamentName">The tournament's name.</param>
        /// <returns></returns>
        public static List<string> GetPlayersByRanking(string tournamentName)
        {
            List<string> res = new List<string>();

            foreach (var elem in GetTournamentRankings(tournamentName))
            {
                res.Add(elem.Item1);
            }

            return res;
        }

        /// <summary>
        /// Gets a list of tournament pairings for the next round.
        /// </summary>
        /// <param name="tournamentName">The tournament's name.</param>
        /// <returns>A list of (string, string?) tuples. Format: (White, Black). If Black is null that means White has a bye.</returns>
        public static List<(string, string?)> GetPairings(string tournamentName)
        {
            List<string> players = GetActivePlayersByRanking(tournamentName);

            List<(string, string?)> res = new List<(string, string?)>();
            
            for (int i = 0; i < Math.Floor(decimal.Divide(players.Count, 2)); i++)
            {
                string white = players[2 * i];
                string black = players[2 * i + 1];

                int whiteBlackID = GetLatestGameFromTournament(tournamentName, white, black);
                int blackWhiteID = GetLatestGameFromTournament(tournamentName, black, white);
                
                // did not play white-black
                if (whiteBlackID == 0)
                {
                    res.Add((white, black));
                    continue;
                }
                // did not play black-white
                if (blackWhiteID == 0)
                {
                    res.Add((black, white));
                    continue;
                }

                DateTime whiteBlack = GetDateTimeFromGame(whiteBlackID);
                DateTime blackWhite = GetDateTimeFromGame(blackWhiteID);

                // latest game was black-white; new game should be white-black
                if (whiteBlack.CompareTo(blackWhite) <= 0)
                {
                    res.Add((white, black));
                }
                // latest game was white-black; new game should be black-white
                else
                {
                    res.Add((black, white));
                }
            }

            return res;
        }

        /// <summary>
        /// Gets the latest game from a tournament between two players.
        /// </summary>
        /// <param name="tournamentName">The tournament's name.</param>
        /// <param name="white">White's name.</param>
        /// <param name="black">Black's name.</param>
        /// <returns>The game ID in the database.</returns>
        public static int GetLatestGameFromTournament(string tournamentName, string white, string black)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                using (SqlCommand getLatestGame = new SqlCommand("SELECT Id FROM Game WHERE TournamentName LIKE @TName AND White LIKE @White AND Black LIKE @Black", connection))
                {
                    getLatestGame.Parameters.Add("@TName", SqlDbType.Text).Value = tournamentName;
                    getLatestGame.Parameters.Add("@White", SqlDbType.Text).Value = white;
                    getLatestGame.Parameters.Add("@Black", SqlDbType.Text).Value = black;

                    var rdr = getLatestGame.ExecuteReader();

                    int latestGameID = 0;

                    while (rdr.Read())
                    {
                        int id = (int)rdr["Id"];

                        if (id > latestGameID)
                        {
                            latestGameID = id;
                        }
                    }

                    rdr.Close();

                    return latestGameID;
                }
            }
        }

        /// <summary>
        /// Gets the datetime of a game.
        /// </summary>
        /// <param name="id">The game's ID.</param>
        /// <returns></returns>
        public static DateTime GetDateTimeFromGame(int id)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                using (SqlCommand getDT = new SqlCommand("SELECT DateTime FROM Game WHERE Id=@ID", connection))
                {
                    getDT.Parameters.Add("@ID", SqlDbType.Int).Value = id;

                    return (DateTime)getDT.ExecuteScalar();
                }
            }
        }

        /// <summary>
        /// Adds new games from a file.
        /// </summary>
        /// <param name="path"></param>
        public static void InputFromCSV(string path)
        {
            string raw = File.ReadAllText(path);

            string[] lines = raw.Split("\n");

            foreach (string line in lines)
            {
                string[] fields = line.Split(",");

                if (fields.Length == 1)
                {
                    continue;
                }

                /*
                 * 0: White
                 * 1: Black
                 * 2: Result
                 * 3: DateTime
                 * 4: Rated
                 * 5: TournamentName
                 * 6: TournamentRound
                 */

                Dictionary<string, bool?> boolD = new Dictionary<string, bool?>
                {
                    { "0", false },
                    { "1", true },
                    { string.Empty, null },
                };

                NewGame(fields[0], fields[1], boolD[fields[2]], DateTime.Parse(fields[3]), (bool)boolD[fields[4]]);

                if (fields.Length > 5)
                {
                    // if tournament already exists
                    if (TableExists(fields[5]))
                    {
                        AddGameToTournament(fields[5], int.Parse(fields[6]), GetIDOfLastGame());
                    }
                }
            }
        }

        /// <summary>
        /// Gets the ID of the last game in the database.
        /// </summary>
        /// <returns></returns>
        public static int GetIDOfLastGame()
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                using (SqlCommand getID = new SqlCommand("SELECT TOP 1 Id FROM Game ORDER BY Id DESC", connection))
                {
                    return (int)getID.ExecuteScalar();
                }
            }
        }

        /// <summary>
        /// Gets a player's status in a tournament.
        /// </summary>
        /// <param name="tournamentName">The tournament's name.</param>
        /// <param name="playerName">The player's name.</param>
        /// <returns></returns>
        public static bool GetPlayerActivty(string tournamentName, string playerName)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                using (SqlCommand getAct = new SqlCommand($"SELECT Active FROM [{tournamentName}] WHERE Name LIKE @Name", connection))
                {
                    getAct.Parameters.Add("@Name", SqlDbType.Text).Value = playerName;

                    return (bool)getAct.ExecuteScalar();
                }
            }
        }

        /// <summary>
        /// Gets the rankings of active players in a tournament.
        /// </summary>
        /// <param name="tournamentName">The tournament's name.</param>
        /// <returns></returns>
        public static List<string> GetActivePlayersByRanking(string tournamentName)
        {
            List<string> players = GetPlayersByRanking(tournamentName);

            foreach (string player in players.ToList())
            {
                if (!GetPlayerActivty(tournamentName, player))
                {
                    players.Remove(player);
                }
            }

            return players;
        }
    }
}
