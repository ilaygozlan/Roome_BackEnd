using System.Data.SqlClient;
using System.Data;
using Microsoft.Extensions.Configuration;
using System.IO;
using Roome_BackEnd.BL;

namespace Roome_BackEnd.DAL
{
    public class DBserviceUser
    {
        private readonly IConfigurationRoot configuration;

        public DBserviceUser()
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

            SqlConnection con = new SqlConnection(cStr);
            con.Open();
            return con;
        }

        public int AddNewUser(User user)
{
    SqlConnection con;
    SqlCommand cmd;

    try
    {
        con = connect();
    }
    catch (Exception ex)
    {
        throw new Exception("Failed to connect to database", ex);
    }

    cmd = CreateCommandWithStoredProcedureAddNewUser("sp_AddNewUser", con, user.Email, user.FullName, user.PhoneNumber, 
        user.Gender, user.BirthDate, user.ProfilePicture, user.OwnPet, user.Smoke);

   
    SqlParameter rowsAffectedParam = new SqlParameter("@RowsAffected", SqlDbType.Int);
    rowsAffectedParam.Direction = ParameterDirection.Output;
    cmd.Parameters.Add(rowsAffectedParam);

    try
    {
        cmd.ExecuteNonQuery();
        int numEffected = (int)cmd.Parameters["@RowsAffected"].Value;
        return numEffected;
    }
    catch (Exception ex)
    {
        throw new Exception("Failed to execute stored procedure", ex);
    }
    finally
    {
        con?.Close();
    }
}
        private SqlCommand CreateCommandWithStoredProcedureAddNewUser(
            string spName, SqlConnection con, string Email, string FullName, string PhoneNumber,
            char Gender, DateTime BirthDate, string ProfilePicture, bool OwnPet, bool Smoke)
        {
            SqlCommand cmd = new SqlCommand
            {
                Connection = con,
                CommandText = spName,
                CommandTimeout = 10,
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@Email", Email);
            cmd.Parameters.AddWithValue("@FullName", FullName);
            cmd.Parameters.AddWithValue("@PhoneNumber", PhoneNumber);
            cmd.Parameters.AddWithValue("@Gender", Gender); // תיקון: בלי רווח
            cmd.Parameters.AddWithValue("@BirthDate", BirthDate);
            cmd.Parameters.AddWithValue("@ProfilePicture", ProfilePicture);
            cmd.Parameters.AddWithValue("@OwnPet", OwnPet);
            cmd.Parameters.AddWithValue("@Smoke", Smoke);

            return cmd;
        }
    }
}
