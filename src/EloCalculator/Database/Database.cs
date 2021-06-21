namespace EloCalculator
{
    using System.Data.SqlClient;

    /// <summary>
    /// Base class for methods related to databases.
    /// </summary>
    public class Database
    {
        /// <summary>
        /// Checks if a table exists in the database.
        /// </summary>
        /// <param name="name">The table's name.</param>
        /// <returns></returns>
        public static bool TableExists(string name)
        {
            using (SqlConnection connection = new SqlConnection(Settings.connectionString))
            {
                connection.Open();

                using (SqlCommand checkTable = new SqlCommand($"SELECT CASE WHEN OBJECT_ID('dbo.{name}', 'U') IS NOT NULL THEN 1 ELSE 0 END", connection))
                {
                    return (int)checkTable.ExecuteScalar() == 1;
                }
            }
        }
    }
}
