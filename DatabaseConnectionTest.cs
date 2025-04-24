using MySql.Data.MySqlClient;
using Microsoft.EntityFrameworkCore;
using System;

namespace TSF_mustidisProj.Data
{
    public class DatabaseConnectionTest
    {
        public static bool TestConnection(string connectionString)
        {
            try
            {
                using (var connection = new MySqlConnection(connectionString))
                {
                    connection.Open();
                    //sConsole.WriteLine("MySQL connection successful!");
                    return true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error connecting to MySQL: {ex.Message}");
                return false;
            }
        }
    }
}