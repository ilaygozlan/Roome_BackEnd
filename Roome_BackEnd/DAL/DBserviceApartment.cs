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

            SqlConnection con = new SqlConnection(cStr);
            con.Open();
            return con;
        }

        public RentalApartment? GetRentalApartmentById(int apartmentId)
        {
            using (SqlConnection con = connect())
            {
                using (SqlCommand cmd = new SqlCommand("SELECT * FROM AbstractApartment WHERE ID = @ApartmentID", con))
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
                using (SqlCommand cmd = new SqlCommand("DeleteRentalApartment", con))
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
                apartment.Description, 
                apartment.NumberOfRommates 
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
            int floor, string description, int numberOfRommates)
        {
            SqlCommand cmd = new SqlCommand
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
            cmd.Parameters.AddWithValue("@NumberOfRommates", numberOfRommates);

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
            SqlCommand cmd = new SqlCommand
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
