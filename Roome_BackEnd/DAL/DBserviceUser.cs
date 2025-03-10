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

    return new SqlConnection(cStr); // ✅ נחזיר חיבור מוכן אבל לא נפתח אותו כאן
}

    // This method add new user
    //--------------------------------------------------------------------------------------------------

   public int AddNewUser(User user)
{
    using (SqlConnection con = connect())
    using (SqlCommand cmd = CreateCommandWithStoredProcedureAddNewUser("sp_AddNewUser", con, user.Email, user.FullName, user.PhoneNumber, 
        user.Gender, user.BirthDate, user.ProfilePicture, user.OwnPet, user.Smoke))
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
        cmd.Parameters.AddWithValue("@Gender", Gender);
        cmd.Parameters.AddWithValue("@BirthDate", BirthDate);
        cmd.Parameters.AddWithValue("@ProfilePicture", ProfilePicture ?? (object)DBNull.Value);
        cmd.Parameters.AddWithValue("@OwnPet", OwnPet);
        cmd.Parameters.AddWithValue("@Smoke", Smoke);

        return cmd;
    }



    //--------------------------------------------------------------------------------------------------
    // This method get user deatils by email
    //--------------------------------------------------------------------------------------------------

        public User GetUser(string useremail)
        {
            using (SqlConnection con = connect())
            using (SqlCommand cmd = CreateCommandWithStoredProcedureGetUser("sp_GetUserByEmail", con, useremail))
            {
                try
                {
                    con.Open();
                    using (SqlDataReader dataReader = cmd.ExecuteReader())
                    {
                        if (dataReader.Read())
                        {
                            return new User
                            {
                                ID = dataReader["ID"] != DBNull.Value ? Convert.ToInt32(dataReader["ID"]) : 0,
                                Email = dataReader["Email"]?.ToString() ?? "",
                                FullName = dataReader["FullName"]?.ToString() ?? "",
                                PhoneNumber = dataReader["PhoneNumber"]?.ToString() ?? "",
                                Gender = dataReader["Sex"] != DBNull.Value ? Convert.ToChar(dataReader["Sex"]) : ' ',
                                ProfilePicture = dataReader["ProfilePicture"]?.ToString() ?? "",
                                BirthDate = dataReader["BirthDate"] != DBNull.Value ? Convert.ToDateTime(dataReader["BirthDate"]) : DateTime.MinValue,
                                Smoke = dataReader["Smoke"] != DBNull.Value ? Convert.ToBoolean(dataReader["Smoke"]) : false,
                                OwnPet = dataReader["OwnPet"] != DBNull.Value ? Convert.ToBoolean(dataReader["OwnPet"]) : false,
                                IsActive = dataReader["IsActive"] != DBNull.Value ? Convert.ToBoolean(dataReader["IsActive"]) : false
                            };
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                    throw new Exception("Failed to retrieve user", ex);
                }
            }

            return null; 
        }



      //---------------------------------------------------------------------------------
    // Create the SqlCommand using a stored procedure to get user by email 
    //---------------------------------------------------------------------------------

     private SqlCommand CreateCommandWithStoredProcedureGetUser(String spName, SqlConnection con, string useremail)
        {
            SqlCommand cmd = new SqlCommand
            {
                Connection = con,
                CommandText = spName,
                CommandTimeout = 10,
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@Email", useremail.Trim());

            return cmd;
        }

     //--------------------------------------------------------------------------------------------------
    // This method get all users
    //--------------------------------------------------------------------------------------------------
    public List<User> GetAllUser()
{
    List<User> allUsers = new List<User>();

    using (SqlConnection con = connect())
    {
        using (SqlCommand cmd = CreateCommandWithStoredProcedureGetAllUsers("GetAllUsers", con))
        {
            try
            {
                con.Open(); 

                using (SqlDataReader dataReader = cmd.ExecuteReader())
                {
                    while (dataReader.Read())
                    {
                        User user = new User
                        {
                            ID = dataReader["ID"] != DBNull.Value ? Convert.ToInt32(dataReader["ID"]) : 0,
                            Email = dataReader["Email"]?.ToString() ?? "",
                            FullName = dataReader["FullName"]?.ToString() ?? "",
                            PhoneNumber = dataReader["PhoneNumber"]?.ToString() ?? "",
                            Gender = dataReader["Sex"] != DBNull.Value ? Convert.ToChar(dataReader["Sex"]) : ' ',
                            ProfilePicture = dataReader["ProfilePicture"]?.ToString() ?? "",
                            BirthDate = dataReader["BirthDate"] != DBNull.Value ? Convert.ToDateTime(dataReader["BirthDate"]) : DateTime.MinValue,
                            Smoke = dataReader["Smoke"] != DBNull.Value ? Convert.ToBoolean(dataReader["Smoke"]) : false,
                            OwnPet = dataReader["OwnPath"] != DBNull.Value ? Convert.ToBoolean(dataReader["OwnPath"]) : false,
                            IsActive = dataReader["IsActive"] != DBNull.Value ? Convert.ToBoolean(dataReader["IsActive"]) : false
                        };

                        allUsers.Add(user);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error executing GetAllUser: {ex.Message}");
                throw new Exception("Failed to execute stored procedure", ex);
            }
        }
    }

    return allUsers;
}

      //---------------------------------------------------------------------------------
    // Create the SqlCommand using a stored procedure to  get all users
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
    using (SqlConnection con = connect()) 
    using (SqlCommand cmd = CreateCommandWithStoredProcedureDeactivateUser("DeactivateUser", con, userEmail))
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
            Console.WriteLine($"Rows affected: {numEffected}");

            return numEffected;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            throw;
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
    using (SqlConnection con = connect())
    using (SqlCommand cmd = CreateCommandWithStoredProcedureUpdateUserDetailsByEmail("UpdateUserDetailsByEmail", con, user))
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
            
            Console.WriteLine($"🔄 Rows affected: {numEffected}");
            return numEffected;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            throw new Exception("Failed to update user details", ex);
        }
    }
}

      //---------------------------------------------------------------------------------
    // Create the SqlCommand using a stored procedure to Update User Details
    //---------------------------------------------------------------------------------

    private SqlCommand CreateCommandWithStoredProcedureUpdateUserDetailsByEmail(string spName, SqlConnection con, User user)
{
    SqlCommand cmd = new SqlCommand
    {
        Connection = con,
        CommandText = spName,
        CommandTimeout = 10,
        CommandType = CommandType.StoredProcedure
    };

    cmd.Parameters.AddWithValue("@Email", user.Email.Trim());
    cmd.Parameters.AddWithValue("@FullName", (object?)user.FullName ?? DBNull.Value);
    cmd.Parameters.AddWithValue("@PhoneNumber", (object?)user.PhoneNumber ?? DBNull.Value);
    cmd.Parameters.AddWithValue("@Sex", (object?)user.Gender ?? DBNull.Value);
    cmd.Parameters.AddWithValue("@BirthDate", user.BirthDate != DateTime.MinValue ? (object)user.BirthDate : DBNull.Value);
    cmd.Parameters.AddWithValue("@ProfilePicture", (object?)user.ProfilePicture ?? DBNull.Value);
    cmd.Parameters.AddWithValue("@OwnPath", (object?)user.OwnPet ?? DBNull.Value);
    cmd.Parameters.AddWithValue("@Smoke", (object?)user.Smoke ?? DBNull.Value);
    cmd.Parameters.AddWithValue("@IsActive", (object?)user.IsActive ?? DBNull.Value);

    return cmd;
}


    }
}
