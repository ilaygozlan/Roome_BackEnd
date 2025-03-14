using System;
using System.Data.SqlClient;
using System.Data;
using Microsoft.Extensions.Configuration;
using System.IO;
using Roome_BackEnd.BL;

namespace Roome_BackEnd.DAL
{
    public class DBserviceRoomates
    {
        private readonly IConfigurationRoot configuration;

        public DBserviceRoomates()
        {
            configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();
        }

        public SqlConnection connect()
        {
            string? cStr = configuration.GetConnectionString("myProjDB");

            if (string.IsNullOrEmpty(cStr))
            {
                throw new Exception("Connection string 'myProjDB' not found in appsettings.json");
            }

            SqlConnection con = new(cStr);
            con.Open();
            return con;
        }

        //--------------------------------------------------------------------------------------------------
        // This method adds new roommates
        //--------------------------------------------------------------------------------------------------
        public bool AddNewRoommates(string roommatesJson, int apartmentId)
        {
            using (SqlConnection con = connect())
            using (SqlCommand cmd = CreateCommandWithStoredProcedure("sp_AddMultipleRoommates", con, roommatesJson, apartmentId))
            {
                try
                {
                    cmd.ExecuteNonQuery();
                    Console.WriteLine("Roommates added successfully.");
                    return true;
                }
                catch (SqlException sqlEx)
                {
                    Console.WriteLine($"SQL Error: {sqlEx.Message}");
                    throw new Exception("Database error occurred", sqlEx);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                    throw new Exception("Failed to execute stored procedure", ex);
                }
            }
        }

        //---------------------------------------------------------------------------------
        // Create the SqlCommand using a stored procedure to add roommates
        //---------------------------------------------------------------------------------
        private SqlCommand CreateCommandWithStoredProcedure(string spName, SqlConnection con, string roommatesJson, int apartmentId)
        {
            SqlCommand cmd = new SqlCommand
            {
                Connection = con,
                CommandText = spName,
                CommandTimeout = 10,
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.Add(new SqlParameter("@ApartmentID", SqlDbType.Int) { Value = apartmentId });
            cmd.Parameters.Add(new SqlParameter("@RoommatesJSON", SqlDbType.NVarChar, -1) { Value = roommatesJson });

            return cmd;
        }
    }
}
