using System.Data.SqlClient;
using System.Data;
using Microsoft.Extensions.Configuration;
using System.IO;
using Roome_BackEnd.BL;

namespace Roome_BackEnd.DAL
{
    public class DBservicesOpenHouse
    {
        private readonly IConfigurationRoot configuration;

        public DBservicesOpenHouse()
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

            return new SqlConnection(cStr);
        }
                //--------------------------------------------------------------------------------------------------
        // This method Get Owner Id
        //--------------------------------------------------------------------------------------------------

public int GetOwnerId(int openHouseId){
    using (SqlConnection con = connect())
    using (SqlCommand cmd = CreateCommandWithStoredProcedureGetOwnerId("sp_GetOwnerId", con, openHouseId))
    {
        try
        {
            con.Open();
            object result = cmd.ExecuteScalar();
            return Convert.ToInt32(result);
            
        }
        catch (Exception ex)
        {
            throw new Exception("Error retrieving owner ID", ex);
        }
    }
}

private SqlCommand CreateCommandWithStoredProcedureGetOwnerId(string spName, SqlConnection con, int openHouseId)
{
    SqlCommand cmd = new SqlCommand
    {
        Connection = con,
        CommandText = spName,
        CommandTimeout = 10,
        CommandType = CommandType.StoredProcedure
    };
    cmd.Parameters.AddWithValue("@OpenHouseID", openHouseId);
    return cmd;
}
        //--------------------------------------------------------------------------------------------------
        // This method creates a new open house
        //--------------------------------------------------------------------------------------------------
        public int CreateAnOpenHouse(OpenHouse openHouse, int userId)
        {
            using (SqlConnection con = connect())
            using (SqlCommand cmd = CreateCommandWithStoredProcedureCreateOpenHouse("sp_AddOpenHouse", con, openHouse.ApartmentId, userId, openHouse.Date,
                openHouse.AmountOfPeoples, openHouse.StartTime, openHouse.EndTime))
            {
                try
                {
                    con.Open();

                    SqlParameter outputParam = new SqlParameter("@RowsAffected", SqlDbType.Int)
                    {
                        Direction = ParameterDirection.Output
                    };
                    cmd.Parameters.Add(outputParam);

                    cmd.ExecuteNonQuery();

                    int rowsAffected = outputParam.Value != DBNull.Value ? Convert.ToInt32(outputParam.Value) : 0;

                    if (rowsAffected == 0)
                    {
                        Console.WriteLine("Open house was not created. It might already exist.");
                        return 0;
                    }

                    Console.WriteLine($"Open House created successfully. Rows affected: {rowsAffected}");
                    return rowsAffected;
                }
                catch (SqlException ex)
                {
                    Console.WriteLine($"SQL Error: {ex.Message}");
                    throw new Exception("Database error occurred", ex);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                    throw new Exception("Failed to execute stored procedure", ex);
                }
            }
        }


        //--------------------------------------------------------------------------------------------------
        // Create the SqlCommand using a stored procedure to add new open house
        //--------------------------------------------------------------------------------------------------
        private SqlCommand CreateCommandWithStoredProcedureCreateOpenHouse(
            string spName, SqlConnection con, int apartmentId, int userId, DateTime date,
            int amountOfPeoples, string startTime, string endTime)
        {
            SqlCommand cmd = new SqlCommand
            {
                Connection = con,
                CommandText = spName,
                CommandTimeout = 10,
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@ApartmentID", apartmentId);
            cmd.Parameters.AddWithValue("@UserID", userId);
            cmd.Parameters.AddWithValue("@Date", date);
            cmd.Parameters.AddWithValue("@AmountOfPeople", amountOfPeoples);
            cmd.Parameters.AddWithValue("@StartTime", startTime);
            cmd.Parameters.AddWithValue("@EndTime", endTime);

            return cmd;
        }

        //--------------------------------------------------------------------------------------------------
        // This method get all the open houses per apartment
        //--------------------------------------------------------------------------------------------------

        public List<OpenHouse> GetOpenHousesForApartment(int apartmentId, int userId)
        {
            List<OpenHouse> openHouses = new List<OpenHouse>();

            using (SqlConnection con = connect())
            using (SqlCommand cmd = new SqlCommand("sp_GetOpenHousesByApartment", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@ApartmentID", apartmentId);
                cmd.Parameters.AddWithValue("@UserID", userId);
                try
                {
                    con.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int openHouseId = reader.GetInt32(reader.GetOrdinal("OpenHouseID"));
                            int aptId = reader.GetInt32(reader.GetOrdinal("ApartmentID"));
                            DateTime date = reader.GetDateTime(reader.GetOrdinal("Date"));
                            int amountOfPeoples = reader.GetInt32(reader.GetOrdinal("AmountOfPeople"));
                            int TotalRegistrations = reader.GetInt32(reader.GetOrdinal("TotalRegistrations"));
                            TimeSpan start = reader.GetTimeSpan(reader.GetOrdinal("StartTime"));
                            TimeSpan end = reader.GetTimeSpan(reader.GetOrdinal("EndTime"));
                            bool isRegistered = reader.GetBoolean(reader.GetOrdinal("IsRegistered"));
                            bool userConfirmed = reader.GetBoolean(reader.GetOrdinal("UserConfirmed"));


                            OpenHouse openHouse = new OpenHouse(
                                openHouseId,
                                aptId,
                                date,
                                amountOfPeoples,
                                TotalRegistrations,
                                start.ToString(@"hh\:mm"),
                                end.ToString(@"hh\:mm"), 
                                isRegistered,
                                userConfirmed
                            );


                            openHouses.Add(openHouse);
                        }
                    }
                }
                catch (SqlException ex)
                {
                    Console.WriteLine($"SQL Error: {ex.Message}");
                    throw new Exception("Database error occurred while getting open houses", ex);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                    throw new Exception("Failed to retrieve open houses", ex);
                }
            }

            return openHouses;
        }


        //--------------------------------------------------------------------------------------------------
        // This method registers a user for an open house
        //--------------------------------------------------------------------------------------------------
        public bool RegisterForOpenHouse(int openHouseId, int userId, bool confirmed = false)
        {
            using (SqlConnection con = connect())
            using (SqlCommand cmd = CreateCommandWithStoredProcedureRegisterForOpenHouse("sp_RegisterForOpenHouse", con, openHouseId, userId, confirmed))
            {
                try
                {
                    con.Open();

                    SqlParameter outputParam = new SqlParameter("@RowsAffected", SqlDbType.Int)
                    {
                        Direction = ParameterDirection.Output
                    };
                    cmd.Parameters.Add(outputParam);

                    object result = cmd.ExecuteScalar();
                    int rowsAffected = outputParam.Value != DBNull.Value ? Convert.ToInt32(outputParam.Value) : 0;

                    if (rowsAffected <= 0)
                    {
                        Console.WriteLine($"Failed to register user {userId} for Open House {openHouseId}. Reason: {result}");
                        return false;
                    }

                    Console.WriteLine($"User {userId} registered successfully for Open House {openHouseId}. Rows affected: {rowsAffected}");
                    return true;
                }
                catch (SqlException ex)
                {
                    Console.WriteLine($"SQL Error: {ex.Message}");
                    throw new Exception("Database error occurred", ex);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                    throw new Exception("Failed to register for open house", ex);
                }
            }
        }


        //--------------------------------------------------------------------------------------------------
        // Create the SqlCommand using a stored procedure to register a user for an open house
        //--------------------------------------------------------------------------------------------------
        private SqlCommand CreateCommandWithStoredProcedureRegisterForOpenHouse(string spName, SqlConnection con, int openHouseId, int userId, bool confirmed)
        {
            SqlCommand cmd = new SqlCommand
            {
                Connection = con,
                CommandText = spName,
                CommandTimeout = 10,
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@OpenHouseID", openHouseId);
            cmd.Parameters.AddWithValue("@UserID", userId);
            cmd.Parameters.AddWithValue("@Confirmed", Convert.ToInt32(confirmed));

            return cmd;
        }
        //--------------------------------------------------------------------------------------------------
        // This method Toggle Attendance for open house
        //--------------------------------------------------------------------------------------------------
        public bool ToggleAttendance(int openHouseId, int userId)
        {
            using (SqlConnection con = connect())
            using (SqlCommand cmd = CreateCommandWithStoredProcedureToggleAttendance("sp_ToggleAttendance", con, openHouseId, userId))
            {
                try
                {
                    con.Open();

                    SqlParameter outputParam = new SqlParameter("@RowsAffected", SqlDbType.Int)
                    {
                        Direction = ParameterDirection.Output
                    };
                    cmd.Parameters.Add(outputParam);

                    cmd.ExecuteNonQuery();

                    int rowsAffected = outputParam.Value != DBNull.Value ? Convert.ToInt32(outputParam.Value) : 0;

                    if (rowsAffected <= 0)
                    {
                        Console.WriteLine($"Failed to update attendance for user {userId} in Open House {openHouseId}.");
                        return false;
                    }

                    Console.WriteLine($"Attendance status toggled for user {userId} in Open House {openHouseId}.");
                    return true;
                }
                catch (SqlException ex)
                {
                    Console.WriteLine($"SQL Error: {ex.Message}");
                    throw new Exception("Database error occurred", ex);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                    throw new Exception("Failed to update attendance status", ex);
                }
            }
        }

        //--------------------------------------------------------------------------------------------------
        // Create the SqlCommand using a stored procedure to Toggle Attendance for open house
        //--------------------------------------------------------------------------------------------------
        private SqlCommand CreateCommandWithStoredProcedureToggleAttendance(string spName, SqlConnection con, int openHouseId, int userId)
        {
            SqlCommand cmd = new SqlCommand
            {
                Connection = con,
                CommandText = spName,
                CommandTimeout = 10,
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@OpenHouseID", openHouseId);
            cmd.Parameters.AddWithValue("@UserID", userId);

            return cmd;
        }

        //--------------------------------------------------------------------------------------------------
        // This method deletes an open house and its registrations
        //--------------------------------------------------------------------------------------------------
        public bool DeleteOpenHouse(int openHouseId, int userId)
        {
            using (SqlConnection con = connect())
            using (SqlCommand cmd = CreateCommandWithStoredProcedureDeleteOpenHouse("sp_DeleteOpenHouse", con, openHouseId, userId))
            {
                try
                {
                    con.Open();

                    // כיוון שהפרוצדורה מחזירה SELECT עם RowsAffected, אנחנו משתמשים ב- ExecuteScalar()
                    object result = cmd.ExecuteScalar();
                    int rowsAffected = result != null ? Convert.ToInt32(result) : 0;

                    Console.WriteLine($"Deleted Open House {openHouseId}. Rows affected: {rowsAffected}");
                    return rowsAffected > 0;
                }
                catch (SqlException ex)
                {
                    Console.WriteLine($"SQL Error: {ex.Message}");
                    throw new Exception("Database error occurred", ex);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                    throw new Exception("Failed to delete open house", ex);
                }
            }
        }

        //--------------------------------------------------------------------------------------------------
        // Create the SqlCommand using a stored procedure to delete an open house
        //--------------------------------------------------------------------------------------------------
        private SqlCommand CreateCommandWithStoredProcedureDeleteOpenHouse(string spName, SqlConnection con, int openHouseId, int userId)
        {
            SqlCommand cmd = new SqlCommand
            {
                Connection = con,
                CommandText = spName,
                CommandTimeout = 10,
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@OpenHouseID", openHouseId);
            cmd.Parameters.AddWithValue("@UserID", userId);

            return cmd;
        }

    }
}
