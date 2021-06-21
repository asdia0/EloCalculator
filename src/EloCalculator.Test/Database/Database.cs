namespace EloCalculator.Test
{
    using System.Data.SqlClient;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class Database
    {
        [TestMethod]
        public void TableExists_False()
        {
            string name = "amogus";

            using (SqlConnection connection = new SqlConnection(Settings.connectionString))
            {
                connection.Open();

                using (SqlCommand removeTable = new SqlCommand($"DROP TABLE IF EXISTS [{name}];", connection))
                {
                    removeTable.ExecuteNonQuery();
                }
            }

            Assert.AreEqual(false, EloCalculator.Database.TableExists(name));
        }

        [TestMethod]
        public void TableExists_True()
        {
            string name = "amogus";

            using (SqlConnection connection = new SqlConnection(Settings.connectionString))
            {
                connection.Open();

                using (SqlCommand addTable = new SqlCommand($"DROP TABLE IF EXISTS [{name}]; CREATE TABLE [{name}] (Sussy bit);", connection))
                {
                    addTable.ExecuteNonQuery();
                }

                Assert.AreEqual(true, EloCalculator.Database.TableExists(name));

                using (SqlCommand removeTable = new SqlCommand($"DROP TABLE IF EXISTS [{name}];", connection))
                {
                    removeTable.ExecuteNonQuery();
                }
            }
        }
    }
}
