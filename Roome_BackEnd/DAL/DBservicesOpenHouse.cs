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

    //-------------------------------------------------------------------------------------------------
    // This method add new open house
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

            int numEffected = (outputParam.Value != DBNull.Value) ? (int)outputParam.Value : 0;

            if (numEffected == 0)
            {
                Console.WriteLine("Failed to add user. Email might already exist.");
            }
            else
            {
                Console.WriteLine($"User added successfully. Rows affected: {numEffected}");
            }

            return numEffected;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            throw new Exception("Failed to execute stored procedure", ex);
        }
    }
}

      //---------------------------------------------------------------------------------
    // Create the SqlCommand using a stored procedure to add new open house
    //---------------------------------------------------------------------------------

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
    }
}

