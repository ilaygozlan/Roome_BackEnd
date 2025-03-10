using System.Data.SqlClient;
using System.Data;
using Microsoft.Extensions.Configuration;
using System.IO;
using Roome_BackEnd.BL;

namespace Roome_BackEnd.DAL
{
    public class DBserviceApartment
    {
        private readonly IConfigurationRoot configuration;

        public DBserviceApartment()
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

        public RentalApartment? GetRentalApartmentById(int apartmentId)
        {
            using (SqlConnection con = connect())
            {
                using (SqlCommand cmd = new("SELECT * FROM AbstractApartment WHERE ID = @ApartmentID", con))
                {
                    cmd.Parameters.AddWithValue("@ApartmentID", apartmentId);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new RentalApartment(
                                (int)reader["ID"],
                                (int)reader["Price"],
                                (int)reader["AmountOfRooms"],
                                reader["Location"].ToString() ?? "",
                                (bool)reader["AllowPet"],
                                (bool)reader["AllowSmoking"],
                                (bool)reader["GardenBalcony"],
                                (int)reader["ParkingSpace"],
                                (DateTime)reader["EntryDate"],
                                (DateTime)reader["ExitDate"],
                                (bool)reader["IsActive"],
                                (int)reader["PropertyTypeID"],
                                (int)reader["UserID"],
                                (int)reader["Floor"],
                                reader["Description"].ToString() ?? "",
                                0,
                                false
                            );
                        }
                    }
                }
            }
            return null;
        }

        public bool SoftDeleteRentalApartment(int apartmentId)
        {
            using (SqlConnection con = connect())
            {
                using (SqlCommand cmd = new("DeleteRentalApartment", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@ApartmentID", apartmentId);

                    try
                    {
                        int rowsAffected = cmd.ExecuteNonQuery();
                        return rowsAffected > 0;
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("Failed to execute stored procedure", ex);
                    }
                }
            }
        }
       

 //get all shared apartments
       public List<SharedApartment> GetAllSharedApartments()
    {
        List<SharedApartment> sharedApartments = new();

        using (SqlConnection con = connect())
        {
            using (SqlCommand cmd = CreateCommandWithStoredProcedure("GetAllSharedApartments", con))
            {
                using (SqlDataReader dataReader = cmd.ExecuteReader())
                {
                    while (dataReader.Read())
                    {
                        int apartmentID = dataReader["ApartmentID"] != DBNull.Value ? Convert.ToInt32(dataReader["ApartmentID"]) : 0;
                        int price = dataReader["Price"] != DBNull.Value ? Convert.ToInt32(dataReader["Price"]) : 0;
                        int amountOfRooms = dataReader["AmountOfRooms"] != DBNull.Value ? Convert.ToInt32(dataReader["AmountOfRooms"]) : 0;
                        string location = dataReader["Location"]?.ToString() ?? "";
                        bool allowPet = dataReader["AllowPet"] != DBNull.Value && Convert.ToBoolean(dataReader["AllowPet"]);
                        bool allowSmoking = dataReader["AllowSmoking"] != DBNull.Value && Convert.ToBoolean(dataReader["AllowSmoking"]);
                        bool gardenBalcony = dataReader["GardenBalcony"] != DBNull.Value && Convert.ToBoolean(dataReader["GardenBalcony"]);
                        int parkingSpace = dataReader["ParkingSpace"] != DBNull.Value ? Convert.ToInt32(dataReader["ParkingSpace"]) : 0;
                        DateTime entryDate = dataReader["EntryDate"] != DBNull.Value ? Convert.ToDateTime(dataReader["EntryDate"]) : DateTime.MinValue;
                        DateTime exitDate = dataReader["ExitDate"] != DBNull.Value ? Convert.ToDateTime(dataReader["ExitDate"]) : DateTime.MinValue;
                        bool isActive = dataReader["IsActive"] != DBNull.Value && Convert.ToBoolean(dataReader["IsActive"]);
                        int propertyTypeID = dataReader["PropertyTypeID"] != DBNull.Value ? Convert.ToInt32(dataReader["PropertyTypeID"]) : 0;
                        int userID = dataReader["UserID"] != DBNull.Value ? Convert.ToInt32(dataReader["UserID"]) : 0;
                        int floor = dataReader["Floor"] != DBNull.Value ? Convert.ToInt32(dataReader["Floor"]) : 0;
                        string description = dataReader["Description"]?.ToString() ?? "";
                        int numberOfRoommates = dataReader["NumberOfRoommates"] != DBNull.Value ? Convert.ToInt32(dataReader["NumberOfRoommates"]) : 0;

                    
                        SharedApartment sharedApartment = new SharedApartment(
                            apartmentID, price, amountOfRooms, location, allowPet, allowSmoking, gardenBalcony, parkingSpace, 
                            entryDate, exitDate, isActive, propertyTypeID, userID, floor, description, numberOfRoommates
                        );

                        sharedApartments.Add(sharedApartment);
                    }
                }
            }
        }

        return sharedApartments;
    }

        private SqlCommand CreateCommandWithStoredProcedure(string spName, SqlConnection con)
        {
            SqlCommand cmd = new(spName, con);
            cmd.CommandTimeout = 30;
            cmd.CommandType = CommandType.StoredProcedure;
            return cmd;
        }

        //get Rental Apartments

        public List<RentalApartment> GetAllRentalApartments()
        {
            List<RentalApartment> rentalApartments = new();

            try
            {
                using (SqlConnection con = connect())
                {
                    using (SqlCommand cmd = CreateCommandWithStoredProceduregetAllRentalApartments("GetAllRentalApartments", con))
                    {
                        try
                        {
                            using (SqlDataReader dataReader = cmd.ExecuteReader())
                            {
                                while (dataReader.Read())
                                {
#pragma warning disable CS8604 // Possible null reference argument.
                                    RentalApartment rentalApartment = new(
                                    Convert.ToInt32(dataReader["id"]),
                                    Convert.ToInt32(dataReader["price"]),
                                    Convert.ToInt32(dataReader["amountOfRooms"]),
                                    Convert.ToString(dataReader["location"]),
                                    Convert.ToBoolean(dataReader["allowPet"]),
                                    Convert.ToBoolean(dataReader["allowSmoking"]),
                                    Convert.ToBoolean(dataReader["gardenBalcony"]),
                                    Convert.ToInt32(dataReader["parkingSpace"]),
                                    Convert.ToDateTime(dataReader["entryDate"]),
                                    Convert.ToDateTime(dataReader["exitDate"]),
                                    Convert.ToBoolean(dataReader["isActive"]),
                                    Convert.ToInt32(dataReader["propertyTypeID"]),
                                    Convert.ToInt32(dataReader["userID"]),
                                    Convert.ToInt32(dataReader["floor"]),
                                    Convert.ToString(dataReader["description"]),
                                    Convert.ToInt32(dataReader["contractLength"]),
                                    Convert.ToBoolean(dataReader["extensionPossible"])
                                );
#pragma warning restore CS8604 

                                    rentalApartments.Add(rentalApartment);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            throw new Exception("Error fetching rental apartments", ex);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to connect to database", ex);
            }

            return rentalApartments;
        }



        private SqlCommand CreateCommandWithStoredProceduregetAllRentalApartments(String spName, SqlConnection con)
        {

            SqlCommand cmd = new(); // create the command object

            cmd.Connection = con;              // assign the connection to the command object

            cmd.CommandText = spName;      // can be Select, Insert, Update, Delete 

            cmd.CommandTimeout = 30;           // Time to wait for the execution' The default is 30 seconds

            cmd.CommandType = System.Data.CommandType.StoredProcedure; // the type of the command, can also be text


            return cmd;
        }
        // Add New Shared Apartment 
        public int AddNewSharedApartment(SharedApartment apartment)
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

            cmd = CreateCommandWithStoredProcedureAddNewRentalApartment(
                "AddNewSharedApartment", con,
                apartment.UserID,
                apartment.Price,
                apartment.AmountOfRooms,
                apartment.Location,
                apartment.AllowPet,
                apartment.AllowSmoking,
                apartment.GardenBalcony,
                apartment.ParkingSpace,
                apartment.EntryDate,
                apartment.ExitDate,
                apartment.IsActive,
                apartment.PropertyTypeID,
                apartment.Floor,
                apartment.Description
            );

            try
            {
                // return new apartment id
                object result = cmd.ExecuteScalar();
                int newApartmentId = (result != null && result != DBNull.Value) ? Convert.ToInt32(result) : 0;
                return newApartmentId;
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

        // creatr function for Stored Procedure
        private SqlCommand CreateCommandWithStoredProcedureAddNewRentalApartment(
            string spName, SqlConnection con,
            int userID, int price, int amountOfRooms, string location,
            bool allowPet, bool allowSmoking, bool gardenBalcony, int parkingSpace,
            DateTime entryDate, DateTime exitDate, bool isActive, int propertyTypeID,
            int floor, string description
            )
        {
            SqlCommand cmd = new()
            {
                Connection = con,
                CommandText = spName,
                CommandTimeout = 10,
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@UserID", userID);
            cmd.Parameters.AddWithValue("@Price", price);
            cmd.Parameters.AddWithValue("@AmountOfRooms", amountOfRooms);
            cmd.Parameters.AddWithValue("@Location", location);
            cmd.Parameters.AddWithValue("@AllowPet", allowPet);
            cmd.Parameters.AddWithValue("@AllowSmoking", allowSmoking);
            cmd.Parameters.AddWithValue("@GardenBalcony", gardenBalcony);
            cmd.Parameters.AddWithValue("@ParkingSpace", parkingSpace);
            cmd.Parameters.AddWithValue("@EntryDate", entryDate);
            cmd.Parameters.AddWithValue("@ExitDate", exitDate);
            cmd.Parameters.AddWithValue("@IsActive", isActive);
            cmd.Parameters.AddWithValue("@PropertyTypeID", propertyTypeID);
            cmd.Parameters.AddWithValue("@Floor", floor);
            cmd.Parameters.AddWithValue("@Description", description);

            return cmd;
        }



        // Add New Rental Apartment
        public int AddNewRentalApartment(RentalApartment apartment)
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

            cmd = CreateCommandWithStoredProcedureAddNewRentalApartment(
                "AddNewRentalApartment", con,
                apartment.UserID,
                apartment.Price,
                apartment.AmountOfRooms,
                apartment.Location,
                apartment.AllowPet,
                apartment.AllowSmoking,
                apartment.GardenBalcony,
                apartment.ParkingSpace,
                apartment.EntryDate,
                apartment.ExitDate,
                apartment.IsActive,
                apartment.PropertyTypeID,
                apartment.Floor,
                apartment.Description,
                apartment.ContractLength,
                apartment.ExtensionPossible
            );

            try
            {
                // return new apartment id
                object result = cmd.ExecuteScalar();
                int newApartmentId = (result != null && result != DBNull.Value) ? Convert.ToInt32(result) : 0;
                return newApartmentId;
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

        // creatr function for Stored Procedure
        private SqlCommand CreateCommandWithStoredProcedureAddNewRentalApartment(
            string spName, SqlConnection con,
            int userID, int price, int amountOfRooms, string location,
            bool allowPet, bool allowSmoking, bool gardenBalcony, int parkingSpace,
            DateTime entryDate, DateTime exitDate, bool isActive, int propertyTypeID,
            int floor, string description, int contractLength, bool extensionPossible)
        {
            SqlCommand cmd = new()
            {
                Connection = con,
                CommandText = spName,
                CommandTimeout = 10,
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@UserID", userID);
            cmd.Parameters.AddWithValue("@Price", price);
            cmd.Parameters.AddWithValue("@AmountOfRooms", amountOfRooms);
            cmd.Parameters.AddWithValue("@Location", location);
            cmd.Parameters.AddWithValue("@AllowPet", allowPet);
            cmd.Parameters.AddWithValue("@AllowSmoking", allowSmoking);
            cmd.Parameters.AddWithValue("@GardenBalcony", gardenBalcony);
            cmd.Parameters.AddWithValue("@ParkingSpace", parkingSpace);
            cmd.Parameters.AddWithValue("@EntryDate", entryDate);
            cmd.Parameters.AddWithValue("@ExitDate", exitDate);
            cmd.Parameters.AddWithValue("@IsActive", isActive);
            cmd.Parameters.AddWithValue("@PropertyTypeID", propertyTypeID);
            cmd.Parameters.AddWithValue("@Floor", floor);
            cmd.Parameters.AddWithValue("@Description", description);
            cmd.Parameters.AddWithValue("@ContractLength", contractLength);
            cmd.Parameters.AddWithValue("@ExtensionPossible", extensionPossible);

            return cmd;
        }
    }
}
