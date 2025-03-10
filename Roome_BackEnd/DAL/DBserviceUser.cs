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
    //--------------------------------------------------------------------------------------------------
    // This method add new user
    //--------------------------------------------------------------------------------------------------

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
      //---------------------------------------------------------------------------------
    // Create the SqlCommand using a stored procedure to add new user
    //---------------------------------------------------------------------------------

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


    //--------------------------------------------------------------------------------------------------
    // This method get user deatils by email
    //--------------------------------------------------------------------------------------------------

    public User GetUser(string useremail)
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

        cmd = CreateCommandWithStoredProcedureGetUser("sp_GetUserByEmail", con,useremail);


        try
        {
            SqlDataReader dataReader = cmd.ExecuteReader(CommandBehavior.CloseConnection);
            dynamic user= new User();
            while (dataReader.Read())
            {
                user.Email=Convert.ToString(dataReader["Email"]);
                user.FullName=Convert.ToString(dataReader["FullName"]);
                user.PhoneNumber=Convert.ToInt32(dataReader["PhoneNumber"]);
                user.Gender=Convert.ToChar(dataReader["Sex"]);
                user.ProfilePicture=Convert.ToString(dataReader["ProfilePicture"]);
                user.BirthDate=Convert.ToDateTime(dataReader["BirthDate"]);
                user.Smoke=Convert.ToBoolean(dataReader["Smoke"]);
                user.OwnPet=Convert.ToBoolean(dataReader["OwnPath"]);
            }
            return user;
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
      //---------------------------------------------------------------------------------
    // Create the SqlCommand using a stored procedure to get user by email 
    //---------------------------------------------------------------------------------

     private SqlCommand CreateCommandWithStoredProcedureGetUser(String spName, SqlConnection con,string useremail)
    {

        SqlCommand cmd = new SqlCommand(); // create the command object

        cmd.Connection = con;              // assign the connection to the command object

        cmd.CommandText = spName;      // can be Select, Insert, Update, Delete 

        cmd.CommandTimeout = 10;           // Time to wait for the execution' The default is 30 seconds

        cmd.CommandType = System.Data.CommandType.StoredProcedure; // the type of the command, can also be text
        cmd.Parameters.AddWithValue("@Email", useremail);

        return cmd;
    }
     //--------------------------------------------------------------------------------------------------
    // This method get all users
    //--------------------------------------------------------------------------------------------------

    public List<dynamic> GetAllUser()
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

        cmd = CreateCommandWithStoredProcedureGetAllUsers("GetAllUsers", con);

        List<dynamic> allUsers = new List<dynamic>();
        try
        {
            SqlDataReader dataReader = cmd.ExecuteReader(CommandBehavior.CloseConnection);
            
            while (dataReader.Read())
            {
                dynamic user= new User();
                user.Email=Convert.ToString(dataReader["Email"]);
                user.FullName=Convert.ToString(dataReader["FullName"]);
                user.PhoneNumber=Convert.ToInt32(dataReader["PhoneNumber"]);
                user.Gender=Convert.ToChar(dataReader["Sex"]);
                user.ProfilePicture=Convert.ToString(dataReader["ProfilePicture"]);
                user.BirthDate=Convert.ToDateTime(dataReader["BirthDate"]);
                user.Smoke=Convert.ToBoolean(dataReader["Smoke"]);
                user.OwnPet=Convert.ToBoolean(dataReader["OwnPath"]);
                allUsers.Add(user);
            }
            return allUsers;
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
      //---------------------------------------------------------------------------------
    // Create the SqlCommand using a stored procedure to get user by email 
    //---------------------------------------------------------------------------------

     private SqlCommand CreateCommandWithStoredProcedureGetAllUsers(String spName, SqlConnection con)
    {

        SqlCommand cmd = new SqlCommand(); // create the command object

        cmd.Connection = con;              // assign the connection to the command object

        cmd.CommandText = spName;      // can be Select, Insert, Update, Delete 

        cmd.CommandTimeout = 10;           // Time to wait for the execution' The default is 30 seconds

        cmd.CommandType = System.Data.CommandType.StoredProcedure; // the type of the command, can also be text
       

        return cmd;
    }
    //--------------------------------------------------------------------------------------------------
    // This method change User Activity
    //--------------------------------------------------------------------------------------------------


    public int DeactivateUser(string userEmail)
    {

        SqlConnection con = null;
        SqlCommand cmd = null;
        SqlParameter returnValue = new SqlParameter();
       
    
        try
        {
            con = connect(); // create the connection

            cmd = new SqlCommand("DeactivateUser", con); // Name of the stored procedure
            cmd.CommandType = CommandType.StoredProcedure;

            // Add the email parameter
            cmd.Parameters.AddWithValue("@Email", userEmail);

            // Open connection
            con.Open();

            // Execute the command and get the number of affected rows
            int numEffected = cmd.ExecuteNonQuery();

            return numEffected; // Return the number of affected rows (should be 1 if successful)
        }
        catch (Exception ex)
        {
            // Log the error
            throw ex;
        }
        finally
        {
            if (con != null)
            {
                con.Close(); // Ensure connection is closed
            }
        }
    }

    //---------------------------------------------------------------------------------
    //  Create the SqlCommand using a stored procedure to change user activiti 
    //---------------------------------------------------------------------------------

    private SqlCommand CreateCommandWithStoredProcedureDeactivateUser(String spName, SqlConnection con,string useremail)
    {

        SqlCommand cmd = new SqlCommand(); // create the command object

        cmd.Connection = con;              // assign the connection to the command object

        cmd.CommandText = spName;      // can be Select, Insert, Update, Delete 

        cmd.CommandTimeout = 10;           // Time to wait for the execution' The default is 30 seconds

        cmd.CommandType = System.Data.CommandType.StoredProcedure; // the type of the command, can also be text


        cmd.Parameters.AddWithValue("@EMAIL", useremail);

        return cmd;
    }
   

   //--------------------------------------------------------------------------------------------------
    // This method Update User Details By Email
    //--------------------------------------------------------------------------------------------------

    public int UpdateUserDetailsByEmail(User user)
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

        cmd = CreateCommandWithStoredProcedureUpdateUserDetailsByEmail("UpdateUserDetailsByEmail", con, user.Email, user.FullName, user.PhoneNumber, 
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
      //---------------------------------------------------------------------------------
    // Create the SqlCommand using a stored procedure to Update User Details
    //---------------------------------------------------------------------------------

     private SqlCommand CreateCommandWithStoredProcedureUpdateUserDetailsByEmail(
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
                cmd.Parameters.AddWithValue("@Sex", Gender);
                cmd.Parameters.AddWithValue("@BirthDate", BirthDate);
                cmd.Parameters.AddWithValue("@ProfilePicture", ProfilePicture);
                cmd.Parameters.AddWithValue("@OwnPet", OwnPet);
                cmd.Parameters.AddWithValue("@Smoke", Smoke);

                return cmd;
        }

    }
}
