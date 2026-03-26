using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;

namespace WPF_StudentManagement_Project.Services
{
    class DatabaseHelper
    {
        /// <summary>
        /// REQUIREMENTS: SQL Server Express LocalDB 2019+
        /// </summary>
        private static string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=|DataDirectory|\Services\QLHS.mdf;Integrated Security=True;Connect Timeout=30;";
        /// <summary>
        /// Executes a SQL query and returns the result set as a DataTable (Read operation).
        /// </summary>
        /// <example>
        /// 1. We define a query with parameters (@Role and @Status)
        /// Note the comma after @Role - this triggers the .Replace(",", "") logic!
        /// string sql = "SELECT UserName, Email FROM Users WHERE Role = @Role , AND Status = @Status";
        /// 2. We provide the values for those parameters in an object array
        /// object[] vals = { "Admin", "Active" };
        /// 3. We call the function
        /// DataTable result = ExecuteQuery(sql, vals);
        /// 4. Now 'result' contains only the Active Admins
        ///  foreach (DataRow row in result.Rows)
        ///  {
        ///    Console.WriteLine(row["UserName"]);
        //   }
        /// </example>
        /// <param name="query">The SQL string containing @parameters.</param>
        /// <param name="parameters">An array of values matching the order of @parameters in the query.</param>
        public static DataTable ExecuteQuery(string query, object[]? parameters = null)
        {
            DataTable data = new DataTable();
            // The 'using' block handles closing the connection automatically.
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand(query, connection);

                if (parameters != null)
                {
                    // Maps the 'parameters' array to @tags in the query string by splitting words
                    string[] parts = query.Split(' ');
                    int i = 0;
                    foreach (string word in parts)
                    {
                        if (word.StartsWith("@"))
                        {
                            // Clean punctuation from parameter names (e.g., "@id," -> "@id")
                            string cleanParam = word.Replace(",", "").Replace(")", "");
                            command.Parameters.AddWithValue(cleanParam, parameters[i++]);
                        }
                    }
                }
                // Bridge that fetches data from the database and populates the DataTable
                SqlDataAdapter adapter = new SqlDataAdapter(command);
                adapter.Fill(data);
            }
            return data;
        }
        /// <summary>
        /// Executes a SQL query and returns the number of rows affected (INSERT, DELETE, UPDATE operation).
        /// </summary>
        /// <example>
        // string sql = "UPDATE Products SET Price = @NewPrice WHERE ProductID = @ID";
        // object[] vals = { 19.99, 101 };
        // int rowsAffected = ExecuteNonQuery(sql, vals);
        // if (rowsAffected > 0) 
        // {
        //    Console.WriteLine("Success! Price updated.");
        // }
        // else 
        // {
        //    Console.WriteLine("No product found with that ID.");
        // }
        /// </example>
        /// <param name="query">The SQL string containing @parameters.</param>
        /// <param name="parameters">An array of values matching the order of @parameters in the query.</param>
        public static int ExecuteNonQuery(string query, object[]? parameters = null)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand(query, connection);

                if (parameters != null)
                {
                    // Parses the query string to map @parameter names to the values provided in the array
                    string[] parts = query.Split(' ');
                    int i = 0;
                    foreach (string word in parts)
                    {
                        if (word.StartsWith("@"))
                        {
                            // Strips punctuation to ensure the parameter name matches the SQL definition
                            string cleanParam = word.Replace(",", "").Replace(")", "");
                            command.Parameters.AddWithValue(cleanParam, parameters[i++]);
                        }
                    }
                }
                // Executes INSERT, UPDATE, or DELETE and returns the number of rows affected
                return command.ExecuteNonQuery();
            }
        }
    }
}
