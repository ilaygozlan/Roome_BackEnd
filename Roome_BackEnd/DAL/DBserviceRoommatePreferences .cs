using System.Data.SqlClient;
using System.Data;
using Microsoft.Extensions.Configuration;
using System.IO;
using Roome_BackEnd.BL;
using System.Collections.Generic;

namespace Roome_BackEnd.DAL
{
    public class DBserviceRoommatePreferences
    {
        private readonly IConfigurationRoot configuration;

        public DBserviceRoommatePreferences()
        {
            configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();
        }

        private SqlConnection connect()
        {
            string? cStr = configuration.GetConnectionString("myProjDB");

            if (string.IsNullOrEmpty(cStr))
            {
                throw new Exception("Connection string 'myProjDB' not found in appsettings.json");
            }

            return new SqlConnection(cStr);
        }

        //--------------------------------------------------------------------------------------------------
        // Insert RoommatePreferences
        //--------------------------------------------------------------------------------------------------

        public int InsertRoommatePreferences(RoommatePreferences preferences)
        {
            using (SqlConnection con = connect())
            using (SqlCommand cmd = CreateCommandWithStoredProcedureInsert("sp_InsertRoommatePreferences", con, preferences))
            {
                try
                {
                    con.Open();
                    return cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    throw new Exception("Error inserting roommate preferences", ex);
                }
            }
        }

        private SqlCommand CreateCommandWithStoredProcedureInsert(string spName, SqlConnection con, RoommatePreferences preferences)
        {
            SqlCommand cmd = CreateBaseCommand(spName, con);

            cmd.Parameters.AddWithValue("@UserId", preferences.UserId);
            cmd.Parameters.AddWithValue("@PreferredGender", preferences.PreferredGender ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@PreferredMinAge", preferences.PreferredMinAge);
            cmd.Parameters.AddWithValue("@PreferredMaxAge", preferences.PreferredMaxAge);
            cmd.Parameters.AddWithValue("@AllowSmoking", preferences.AllowSmoking);
            cmd.Parameters.AddWithValue("@AllowPets", preferences.AllowPets);
            cmd.Parameters.AddWithValue("@CleanlinessLevel", preferences.CleanlinessLevel ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@SleepSchedule", preferences.SleepSchedule ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@SocialLevel", preferences.SocialLevel ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@WorkHours", preferences.WorkHours ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@WorkFromHome", preferences.WorkFromHome);
            cmd.Parameters.AddWithValue("@HasPet", preferences.HasPet);
            cmd.Parameters.AddWithValue("@PetType", preferences.PetType ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@RelationshipStatus", preferences.RelationshipStatus ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@SocialStyle", preferences.SocialStyle ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@OpenToFriendship", preferences.OpenToFriendship);
            cmd.Parameters.AddWithValue("@Notes", preferences.Notes ?? (object)DBNull.Value);

            return cmd;
        }

        //--------------------------------------------------------------------------------------------------
        // Update RoommatePreferences
        //--------------------------------------------------------------------------------------------------

        //--------------------------------------------------------------------------------------------------
        // Update RoommatePreferences
        //--------------------------------------------------------------------------------------------------

        public int UpdateRoommatePreferences(RoommatePreferences preferences)
        {
            using (SqlConnection con = connect())
            using (SqlCommand cmd = CreateCommandWithStoredProcedureUpdate("sp_UpdateRoommatePreferences", con, preferences))
            {
                try
                {
                    con.Open();
                    return cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    throw new Exception("Error updating roommate preferences", ex);
                }
            }
        }

        private SqlCommand CreateCommandWithStoredProcedureUpdate(string spName, SqlConnection con, RoommatePreferences preferences)
        {
            SqlCommand cmd = CreateBaseCommand(spName, con);

            cmd.Parameters.AddWithValue("@UserId", preferences.UserId);

            cmd.Parameters.AddWithValue("@PreferredGender", string.IsNullOrEmpty(preferences.PreferredGender) ? (object)DBNull.Value : preferences.PreferredGender);
            cmd.Parameters.AddWithValue("@PreferredMinAge", preferences.PreferredMinAge == 0 ? 0 : preferences.PreferredMinAge);
            cmd.Parameters.AddWithValue("@PreferredMaxAge", preferences.PreferredMaxAge == 0 ? 0 : preferences.PreferredMaxAge);
            cmd.Parameters.AddWithValue("@AllowSmoking", preferences.AllowSmoking);
            cmd.Parameters.AddWithValue("@AllowPets", preferences.AllowPets);
            cmd.Parameters.AddWithValue("@CleanlinessLevel", string.IsNullOrEmpty(preferences.CleanlinessLevel) ? (object)DBNull.Value : preferences.CleanlinessLevel);
            cmd.Parameters.AddWithValue("@SleepSchedule", string.IsNullOrEmpty(preferences.SleepSchedule) ? (object)DBNull.Value : preferences.SleepSchedule);
            cmd.Parameters.AddWithValue("@SocialLevel", string.IsNullOrEmpty(preferences.SocialLevel) ? (object)DBNull.Value : preferences.SocialLevel);
            cmd.Parameters.AddWithValue("@WorkHours", string.IsNullOrEmpty(preferences.WorkHours) ? (object)DBNull.Value : preferences.WorkHours);
            cmd.Parameters.AddWithValue("@WorkFromHome", preferences.WorkFromHome);
            cmd.Parameters.AddWithValue("@HasPet", preferences.HasPet);
            cmd.Parameters.AddWithValue("@PetType", string.IsNullOrEmpty(preferences.PetType) ? (object)DBNull.Value : preferences.PetType);
            cmd.Parameters.AddWithValue("@RelationshipStatus", string.IsNullOrEmpty(preferences.RelationshipStatus) ? (object)DBNull.Value : preferences.RelationshipStatus);
            cmd.Parameters.AddWithValue("@SocialStyle", string.IsNullOrEmpty(preferences.SocialStyle) ? (object)DBNull.Value : preferences.SocialStyle);
            cmd.Parameters.AddWithValue("@OpenToFriendship", preferences.OpenToFriendship);
            cmd.Parameters.AddWithValue("@Notes", string.IsNullOrEmpty(preferences.Notes) ? (object)DBNull.Value : preferences.Notes);

            return cmd;
        }


        //--------------------------------------------------------------------------------------------------
        // Delete RoommatePreferences
        //--------------------------------------------------------------------------------------------------

        public int DeleteRoommatePreferences(int userId)
        {
            using (SqlConnection con = connect())
            using (SqlCommand cmd = CreateCommandWithStoredProcedureDelete("sp_DeleteRoommatePreferences", con, userId))
            {
                try
                {
                    con.Open();
                    return cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    throw new Exception("Error deleting roommate preferences", ex);
                }
            }
        }

        private SqlCommand CreateCommandWithStoredProcedureDelete(string spName, SqlConnection con, int userId)
        {
            SqlCommand cmd = CreateBaseCommand(spName, con);
            cmd.Parameters.AddWithValue("@UserId", userId);
            return cmd;
        }

        //--------------------------------------------------------------------------------------------------
        // Get RoommatePreferences by UserId
        //--------------------------------------------------------------------------------------------------

        public RoommatePreferences GetRoommatePreferencesByUserId(int userId)
        {
            using (SqlConnection con = connect())
            using (SqlCommand cmd = CreateCommandWithStoredProcedureGetByUserId("sp_GetRoommatePreferencesByUserId", con, userId))
            {
                try
                {
                    con.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return ReadRoommatePreferences(reader);
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Error retrieving roommate preferences", ex);
                }
            }
            return null;
        }

        private SqlCommand CreateCommandWithStoredProcedureGetByUserId(string spName, SqlConnection con, int userId)
        {
            SqlCommand cmd = CreateBaseCommand(spName, con);
            cmd.Parameters.AddWithValue("@UserId", userId);
            return cmd;
        }

        //--------------------------------------------------------------------------------------------------
        // Get All RoommatePreferences
        //--------------------------------------------------------------------------------------------------

        public List<RoommatePreferences> GetAllRoommatePreferences()
        {
            List<RoommatePreferences> preferencesList = new List<RoommatePreferences>();

            using (SqlConnection con = connect())
            using (SqlCommand cmd = CreateBaseCommand("sp_GetAllRoommatePreferences", con))
            {
                try
                {
                    con.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            preferencesList.Add(ReadRoommatePreferences(reader));
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Error retrieving all roommate preferences", ex);
                }
            }
            return preferencesList;
        }

        //--------------------------------------------------------------------------------------------------
        // Read RoommatePreferences Helper
        //--------------------------------------------------------------------------------------------------

        private RoommatePreferences ReadRoommatePreferences(SqlDataReader reader)
        {
            return new RoommatePreferences
            {
                PreferenceId = Convert.ToInt32(reader["PreferenceId"]),
                UserId = Convert.ToInt32(reader["UserId"]),
                PreferredGender = reader["PreferredGender"]?.ToString(),
                PreferredMinAge = reader["PreferredMinAge"] != DBNull.Value ? Convert.ToInt32(reader["PreferredMinAge"]) : 0,
                PreferredMaxAge = reader["PreferredMaxAge"] != DBNull.Value ? Convert.ToInt32(reader["PreferredMaxAge"]) : 0,
                AllowSmoking = Convert.ToBoolean(reader["AllowSmoking"]),
                AllowPets = Convert.ToBoolean(reader["AllowPets"]),
                CleanlinessLevel = reader["CleanlinessLevel"]?.ToString(),
                SleepSchedule = reader["SleepSchedule"]?.ToString(),
                SocialLevel = reader["SocialLevel"]?.ToString(),
                WorkHours = reader["WorkHours"]?.ToString(),
                WorkFromHome = Convert.ToBoolean(reader["WorkFromHome"]),
                HasPet = Convert.ToBoolean(reader["HasPet"]),
                PetType = reader["PetType"]?.ToString(),
                RelationshipStatus = reader["RelationshipStatus"]?.ToString(),
                SocialStyle = reader["SocialStyle"]?.ToString(),
                OpenToFriendship = Convert.ToBoolean(reader["OpenToFriendship"]),
                Notes = reader["Notes"]?.ToString()
            };
        }

        //--------------------------------------------------------------------------------------------------
        // Create Base Command
        //--------------------------------------------------------------------------------------------------

        private SqlCommand CreateBaseCommand(string spName, SqlConnection con)
        {
            SqlCommand cmd = new SqlCommand
            {
                Connection = con,
                CommandText = spName,
                CommandTimeout = 10,
                CommandType = CommandType.StoredProcedure
            };
            return cmd;
        }
    }
}
