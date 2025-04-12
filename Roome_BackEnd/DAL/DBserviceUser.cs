using System.Data.SqlClient;
using System.Data;
using Microsoft.Extensions.Configuration;
using System.IO;
using Roome_BackEnd.BL;
using System.Dynamic;
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

    return new SqlConnection(cStr);
}
    //--------------------------------------------------------------------------------------------------
    // This method add new user
    //--------------------------------------------------------------------------------------------------

public (int userId, bool isNew) AddNewUser(User user)
{
    using (SqlConnection con = connect())
    using (SqlCommand cmd = CreateCommandWithStoredProcedureAddNewUser("sp_AddNewUser", con, user.Email, user.FullName, user.PhoneNumber,
        user.Gender, user.BirthDate, user.ProfilePicture, user.OwnPet, user.Smoke))
    {
        try
        {
           
            SqlParameter userIdParam = new SqlParameter("@UserId", SqlDbType.Int)
            {
                Direction = ParameterDirection.Output
            };
            cmd.Parameters.Add(userIdParam);

            SqlParameter isNewParam = new SqlParameter("@IsNew", SqlDbType.Bit)
            {
                Direction = ParameterDirection.Output
            };
            cmd.Parameters.Add(isNewParam);

            con.Open();
            cmd.ExecuteNonQuery();

            int userId = (userIdParam.Value != DBNull.Value) ? (int)userIdParam.Value : -1;
            bool isNew = (isNewParam.Value != DBNull.Value) && (bool)isNewParam.Value;

            return (userId, isNew);
        }
        catch (Exception ex)
        {
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


   //--------------------------------------------------------------------------------------------------
    // This method add friend 
    //--------------------------------------------------------------------------------------------------

public string AddFriend(int userId1, int userId2)
{
    using (SqlConnection con = connect())
    using (SqlCommand cmd = CreateCommandWithStoredProcedureAddFriend("AddFriend", con, userId1, userId2))
    {
        try
        {
            con.Open();
            object result = cmd.ExecuteScalar();
            return result != null ? result.ToString() : "Error adding friend";
        }
        catch (Exception ex)
        {
            Console.WriteLine($" Error: {ex.Message}");
            throw new Exception("Failed to add friend", ex);
        }
    }
}
      //---------------------------------------------------------------------------------
    // Create the SqlCommand using a stored procedure to add friend 
    //---------------------------------------------------------------------------------

private SqlCommand CreateCommandWithStoredProcedureAddFriend(string spName, SqlConnection con, int userId1, int userId2)
{
    SqlCommand cmd = new SqlCommand
    {
        Connection = con,
        CommandText = spName,
        CommandTimeout = 10,
        CommandType = CommandType.StoredProcedure
    };

    cmd.Parameters.AddWithValue("@UserID1", userId1);
    cmd.Parameters.AddWithValue("@UserID2", userId2);

    return cmd;
}


   //--------------------------------------------------------------------------------------------------
    // This method Get User Friends
    //--------------------------------------------------------------------------------------------------

public List<User> GetUserFriends(int userId)
{
    List<User> friends = new List<User>();

    using (SqlConnection con = connect())
    using (SqlCommand cmd = CreateCommandWithStoredProcedureGetUserFriends("GetUserFriends", con, userId))
    {
        try
        {
            con.Open();
            using (SqlDataReader dataReader = cmd.ExecuteReader())
            {
                while (dataReader.Read())
                {
                    User friend = new User
                    {
                        ID = Convert.ToInt32(dataReader["FriendID"]),
                        Email = dataReader["Email"].ToString() ?? "",
                        FullName = dataReader["FullName"].ToString() ?? "",
                        PhoneNumber = dataReader["PhoneNumber"].ToString() ?? "",
                        Gender = dataReader["Sex"] != DBNull.Value ? Convert.ToChar(dataReader["Sex"]) : ' ',
                        ProfilePicture = dataReader["ProfilePicture"]?.ToString() ?? "",
                        BirthDate = dataReader["BirthDate"] != DBNull.Value ? Convert.ToDateTime(dataReader["BirthDate"]) : DateTime.MinValue,
                        Smoke = dataReader["Smoke"] != DBNull.Value ? Convert.ToBoolean(dataReader["Smoke"]) : false,
                        OwnPet = dataReader["OwnPath"] != DBNull.Value ? Convert.ToBoolean(dataReader["OwnPath"]) : false,
                        IsActive = dataReader["IsActive"] != DBNull.Value ? Convert.ToBoolean(dataReader["IsActive"]) : false
                    };

                    friends.Add(friend);
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            throw new Exception("Failed to retrieve user friends", ex);
        }
    }

    return friends;
}
 //---------------------------------------------------------------------------------
    // Create the SqlCommand using a stored procedure to Get User Friends
    //---------------------------------------------------------------------------------

private SqlCommand CreateCommandWithStoredProcedureGetUserFriends(string spName, SqlConnection con, int userId)
{
    SqlCommand cmd = new SqlCommand
    {
        Connection = con,
        CommandText = spName,
        CommandTimeout = 10,
        CommandType = CommandType.StoredProcedure
    };

    cmd.Parameters.AddWithValue("@UserID", userId);

    return cmd;
}

   //--------------------------------------------------------------------------------------------------
    // This method Get Remove Friends
    //--------------------------------------------------------------------------------------------------

    public string RemoveFriend(int userId1, int userId2)
    {
        using (SqlConnection con = connect())
        using (SqlCommand cmd = CreateCommandWithStoredProcedureRemoveFriend("RemoveFriend", con, userId1, userId2))
        {
            try
            {
                con.Open();
                object result = cmd.ExecuteScalar();
                return result != null ? result.ToString() : "Operation failed.";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                throw new Exception("Failed to remove friend", ex);
            }
        }
    }
 //---------------------------------------------------------------------------------
    // Create the SqlCommand using a stored procedure to RemoveFriend
    //---------------------------------------------------------------------------------

    private SqlCommand CreateCommandWithStoredProcedureRemoveFriend(string spName, SqlConnection con, int userId1, int userId2)
    {
        SqlCommand cmd = new SqlCommand
        {
            Connection = con,
            CommandText = spName,
            CommandTimeout = 10,
            CommandType = CommandType.StoredProcedure
        };

        cmd.Parameters.AddWithValue("@UserID1", userId1);
        cmd.Parameters.AddWithValue("@UserID2", userId2);

        return cmd;
    }

       //--------------------------------------------------------------------------------------------------
   // This method allows a user to like an apartment
   //--------------------------------------------------------------------------------------------------

   public string UserLikeApartment(int userId, int apartmentId)
   {
       using (SqlConnection con = connect())
       using (SqlCommand cmd = CreateCommandWithStoredProcedureUserLikeApartment("sp_UserLikeApartment", con, userId, apartmentId))
       {
           try
           {
               con.Open();
               cmd.ExecuteNonQuery();
               return "Like added successfully";
           }
           catch (SqlException ex)
           {
               Console.WriteLine($"Error: {ex.Message}");
               throw new Exception("Failed to like the apartment", ex);
           }
       }
   }

   //---------------------------------------------------------------------------------
   // Create the SqlCommand using a stored procedure to like an apartment
   //---------------------------------------------------------------------------------

   private SqlCommand CreateCommandWithStoredProcedureUserLikeApartment(string spName, SqlConnection con, int userId, int apartmentId)
   {
       SqlCommand cmd = new SqlCommand
       {
           Connection = con,
           CommandText = spName,
           CommandTimeout = 10,
           CommandType = CommandType.StoredProcedure
       };

       cmd.Parameters.AddWithValue("@UserID", userId);
       cmd.Parameters.AddWithValue("@ApartmentID", apartmentId);

       return cmd;
   }
   //--------------------------------------------------------------------------------------------------
   // This method allows a user to remove a like from an apartment
   //--------------------------------------------------------------------------------------------------

   public string RemoveUserLikeApartment(int userId, int apartmentId)
   {
       using (SqlConnection con = connect())
       using (SqlCommand cmd = CreateCommandWithStoredProcedureRemoveUserLikeApartment("sp_RemoveUserLikeApartment", con, userId, apartmentId))
       {
           try
           {
               con.Open();
               cmd.ExecuteNonQuery();
               return "Like removed successfully";
           }
           catch (SqlException ex)
           {
               Console.WriteLine($"Error: {ex.Message}");
               throw new Exception("Failed to remove like from the apartment", ex);
           }
       }
   }

   //---------------------------------------------------------------------------------
   // Create the SqlCommand using a stored procedure to remove a like from an apartment
   //---------------------------------------------------------------------------------

   private SqlCommand CreateCommandWithStoredProcedureRemoveUserLikeApartment(string spName, SqlConnection con, int userId, int apartmentId)
   {
       SqlCommand cmd = new SqlCommand
       {
           Connection = con,
           CommandText = spName,
           CommandTimeout = 10,
           CommandType = CommandType.StoredProcedure
       };

       cmd.Parameters.AddWithValue("@UserID", userId);
       cmd.Parameters.AddWithValue("@ApartmentID", apartmentId);

       return cmd;
   }

        //--------------------------------------------------------------------------------------------------
        // This method retrieves all apartments that a user has liked dynamically
        //--------------------------------------------------------------------------------------------------
        public List<dynamic> GetUserLikedApartments(int userId)
        {
            if (userId <= 0)
            {
                throw new ArgumentException("Invalid user ID.");
            }

            List<dynamic> likedApartments = new List<dynamic>();

            using (SqlConnection con = connect())
            using (SqlCommand cmd = CreateCommandWithStoredProcedureGetUserLikedApartments("sp_GetUserLikedApartments", con, userId))
            {
                try
                {
                    Console.WriteLine($"Fetching liked apartments for User ID={userId}");

                    con.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            dynamic apartment = new ExpandoObject();
                            var apartmentDict = (IDictionary<string, object>)apartment;

                            // Common properties from AbstractApartment
                            apartmentDict["ApartmentID"] = reader["ApartmentID"];
                            apartmentDict["Price"] = reader["Price"];
                            apartmentDict["AmountOfRooms"] = reader["AmountOfRooms"];
                            apartmentDict["Location"] = reader["Location"];
                            apartmentDict["AllowPet"] = reader["AllowPet"];
                            apartmentDict["AllowSmoking"] = reader["AllowSmoking"];
                            apartmentDict["GardenBalcony"] = reader["GardenBalcony"];
                            apartmentDict["ParkingSpace"] = reader["ParkingSpace"];
                            apartmentDict["EntryDate"] = reader["EntryDate"];
                            apartmentDict["ExitDate"] = reader["ExitDate"];
                            apartmentDict["IsActive"] = reader["IsActive"];
                            apartmentDict["OwnerUserID"] = reader["OwnerUserID"];
                            apartmentDict["Floor"] = reader["Floor"];
                            apartmentDict["Description"] = reader["Description"];
                            apartmentDict["Images"] = reader["Images"] != DBNull.Value ? reader["Images"].ToString().Split(", ").ToList() : new List<string>();
                            apartmentDict["Roommates"] = reader["Roommates"] != DBNull.Value ? reader["Roommates"].ToString() : null;

                            // Determine apartment type based on joined tables
                            if (reader["Shared_NumberOfRoommates"] != DBNull.Value) 
                            {
                                apartmentDict["ApartmentType"] = "SharedApartment";
                                apartmentDict["NumberOfRoommates"] = reader["Shared_NumberOfRoommates"];
                            }
                            else if (reader["Rental_ContractLength"] != DBNull.Value) 
                            {
                                apartmentDict["ApartmentType"] = "RentalApartment";
                                apartmentDict["ContractLength"] = reader["Rental_ContractLength"];
                                apartmentDict["ExtensionPossible"] = reader["Rental_ExtensionPossible"];
                            }
                            else if (reader["Sublet_CanCancelWithoutPenalty"] != DBNull.Value) 
                            {
                                apartmentDict["ApartmentType"] = "SubletApartment";
                                apartmentDict["CanCancelWithoutPenalty"] = reader["Sublet_CanCancelWithoutPenalty"];
                                apartmentDict["IsWholeProperty"] = reader["Sublet_IsWholeProperty"];
                            }

                            likedApartments.Add(apartment);
                        }
                    }
                }
                catch (SqlException sqlEx)
                {
                    Console.WriteLine($"SQL Error: {sqlEx.Message}");
                    throw new Exception("Database error occurred", sqlEx);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                    throw new Exception("Failed to retrieve liked apartments", ex);
                }
            }

            return likedApartments;
        }

        //---------------------------------------------------------------------------------
        // Create the SqlCommand using a stored procedure to get all liked apartments for a user
        //---------------------------------------------------------------------------------
        private SqlCommand CreateCommandWithStoredProcedureGetUserLikedApartments(string spName, SqlConnection con, int userId)
        {
            SqlCommand cmd = new SqlCommand
            {
                Connection = con,
                CommandText = spName,
                CommandTimeout = 10,
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.Add(new SqlParameter("@UserID", SqlDbType.Int) { Value = userId });

            return cmd;
        }

        //--------------------------------------------------------------------------------------------------
// This method retrieves all apartments owned by a specific user dynamically
//--------------------------------------------------------------------------------------------------
        public List<dynamic> GetUserOwnedApartments(int userId)
        {
            if (userId <= 0)
            {
                throw new ArgumentException("Invalid user ID.");
            }

            List<dynamic> ownedApartments = new List<dynamic>();

            using (SqlConnection con = connect())
            using (SqlCommand cmd = CreateCommandWithStoredProcedureGetUserOwnedApartments("sp_GetUserOwnedApartments", con, userId))
            {
                try
                {
                    con.Open(); // ✅ Ensure connection is open before reading data

                    Console.WriteLine($"Fetching owned apartments for User ID={userId}");

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            dynamic apartment = new ExpandoObject();
                            var apartmentDict = (IDictionary<string, object>)apartment;

                            // Common properties from AbstractApartment
                            apartmentDict["ApartmentID"] = reader["ApartmentID"];
                            apartmentDict["Price"] = reader["Price"];
                            apartmentDict["AmountOfRooms"] = reader["AmountOfRooms"];
                            apartmentDict["Location"] = reader["Location"];
                            apartmentDict["AllowPet"] = reader["AllowPet"];
                            apartmentDict["AllowSmoking"] = reader["AllowSmoking"];
                            apartmentDict["GardenBalcony"] = reader["GardenBalcony"];
                            apartmentDict["ParkingSpace"] = reader["ParkingSpace"];
                            apartmentDict["EntryDate"] = reader["EntryDate"];
                            apartmentDict["ExitDate"] = reader["ExitDate"];
                            apartmentDict["IsActive"] = reader["IsActive"];
                            apartmentDict["UserID"] = reader["UserID"];
                            apartmentDict["Floor"] = reader["Floor"];
                            apartmentDict["Description"] = reader["Description"];
                            apartmentDict["Images"] = reader["Images"] != DBNull.Value ? reader["Images"].ToString().Split(", ").ToList() : new List<string>();
                            apartmentDict["Roommates"] = reader["Roommates"] != DBNull.Value ? reader["Roommates"].ToString() : null;

                            // Determine apartment type based on joined tables
                            if (reader["Shared_NumberOfRoommates"] != DBNull.Value)
                            {
                                apartmentDict["ApartmentType"] = "SharedApartment";
                                apartmentDict["NumberOfRoommates"] = reader["Shared_NumberOfRoommates"];
                            }
                            else if (reader["Rental_ContractLength"] != DBNull.Value)
                            {
                                apartmentDict["ApartmentType"] = "RentalApartment";
                                apartmentDict["ContractLength"] = reader["Rental_ContractLength"];
                                apartmentDict["ExtensionPossible"] = reader["Rental_ExtensionPossible"];
                            }
                            else if (reader["Sublet_CanCancelWithoutPenalty"] != DBNull.Value)
                            {
                                apartmentDict["ApartmentType"] = "SubletApartment";
                                apartmentDict["CanCancelWithoutPenalty"] = reader["Sublet_CanCancelWithoutPenalty"];
                                apartmentDict["IsWholeProperty"] = reader["Sublet_IsWholeProperty"];
                            }

                            ownedApartments.Add(apartment);
                        }
                    }
                }
                catch (SqlException sqlEx)
                {
                    Console.WriteLine($"SQL Error: {sqlEx.Message}");
                    throw new Exception("Database error occurred", sqlEx);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                    throw new Exception("Failed to retrieve owned apartments", ex);
                }
            }

            return ownedApartments;
        }

                //---------------------------------------------------------------------------------
        // Create the SqlCommand using a stored procedure to get all owned apartments for a user
        //---------------------------------------------------------------------------------
        private SqlCommand CreateCommandWithStoredProcedureGetUserOwnedApartments(string spName, SqlConnection con, int userId)
        {
            SqlCommand cmd = new SqlCommand
            {
                Connection = con,
                CommandText = spName,
                CommandTimeout = 10,
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.Add(new SqlParameter("@UserID", SqlDbType.Int) { Value = userId });

            return cmd;
        }

        //---------------------------------------------------------------------------------
        // Create the SqlCommand using a stored procedure to get user info
        //---------------------------------------------------------------------------------

public User GetUserById(int userId)
{
    string query = "GetUserInfo";
    using (SqlConnection con = connect())
    using (SqlCommand cmd = new SqlCommand(query, con))
    {
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("@UserId", userId);

        try
        {
            con.Open();
            SqlDataReader reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                return new User
                {
                    ID = Convert.ToInt32(reader["ID"]),
                    Email = reader["Email"].ToString(),
                    FullName = reader["FullName"].ToString(),
                    PhoneNumber = reader["PhoneNumber"].ToString(),
                    Gender = Convert.ToChar(reader["Sex"]),
                    BirthDate = Convert.ToDateTime(reader["BirthDate"]),
                    ProfilePicture = reader["ProfilePicture"].ToString(),
                    OwnPet = Convert.ToBoolean(reader["OwnPath"]),
                    Smoke = Convert.ToBoolean(reader["Smoke"]),
                    IsActive = Convert.ToBoolean(reader["IsActive"])
                };
            }
            return null;
        }
        catch (Exception ex)
        {
            throw new Exception("Error fetching user info by ID", ex);
        }
    }
}

        //---------------------------------------------------------------------------------
        // Create the SqlCommand using a stored procedure to check if user exists
        //---------------------------------------------------------------------------------

public int CheckIfUserExists(string email)
{
    using (SqlConnection con = connect())
    using (SqlCommand cmd = new SqlCommand("checkIfUserExist", con))
    {
        cmd.CommandType = CommandType.StoredProcedure;

        // קלט: האימייל של המשתמש
        cmd.Parameters.AddWithValue("@Email", email);

        // פלט: userId או -1
        SqlParameter userIdParam = new SqlParameter("@UserId", SqlDbType.Int)
        {
            Direction = ParameterDirection.Output
        };
        cmd.Parameters.Add(userIdParam);

        try
        {
            con.Open();
            cmd.ExecuteNonQuery();

            int userId = (userIdParam.Value != DBNull.Value) ? (int)userIdParam.Value : -1;
            return userId;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error checking if user exists: " + ex.Message);
            throw;
        }
    }
}

    }
}


 
