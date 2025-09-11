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
                    cmd.ExecuteScalar();
                    return true;
                }
                catch (SqlException sqlEx)
                {
                    throw new Exception("Database error occurred", sqlEx);
                }
                catch (Exception ex)
                {
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

        //--------------------------------------------------------------------------------------------------
        // This method delete roommate
        //--------------------------------------------------------------------------------------------------
        public bool DeleteRoommate(string roommateName, int apartmentId)
        {
            using (SqlConnection con = connect())
            using (SqlCommand cmd = CreateCommandWithStoredProcedureDeleteRoommate("sp_DeleteRoommate", con, roommateName, apartmentId))
            {
                try
                {
                    cmd.ExecuteNonQuery();
                    return (int)cmd.Parameters["@Result"].Value > 0;
                }
                catch (SqlException sqlEx)
                {
                    throw new Exception("Database error occurred", sqlEx);
                }
                catch (Exception ex)
                {
                    throw new Exception("Failed to execute stored procedure", ex);
                }
            }
        }

        //---------------------------------------------------------------------------------
        // Create the SqlCommand using a stored procedure to delete roommate
        //---------------------------------------------------------------------------------
        private SqlCommand CreateCommandWithStoredProcedureDeleteRoommate(string spName, SqlConnection con, string roommateName, int apartmentId)
        {
            SqlCommand cmd = new SqlCommand
            {
                Connection = con,
                CommandText = spName,
                CommandTimeout = 10,
                CommandType = CommandType.StoredProcedure
            };
            // Output Parameter
            SqlParameter resultParam = new SqlParameter("@Result", SqlDbType.Int)
            {
            Direction = ParameterDirection.Output
            };
            cmd.Parameters.Add(resultParam);
            cmd.Parameters.AddWithValue("@ApartmentID", apartmentId);
            cmd.Parameters.AddWithValue("@RoommateName", roommateName);

            return cmd;
        }
    }
}
