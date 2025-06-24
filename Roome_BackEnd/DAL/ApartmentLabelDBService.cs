using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Roome_BackEnd.BL;

namespace Roome_BackEnd.DAL
{
    public class DBserviceApartmentLabel
    {
        private readonly IConfigurationRoot configuration;

        public DBserviceApartmentLabel()
        {
            configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();
        }

        // Connect to database
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

        // Add a new label to an apartment
        public void AddLabel(int apartmentId, string label)
        {
            using (SqlConnection con = connect())
            using (SqlCommand cmd = new SqlCommand("AddApartmentLabel", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@ApartmentId", apartmentId);
                cmd.Parameters.AddWithValue("@Label", label);
                cmd.ExecuteNonQuery();
            }
        }

        // Update a label by ID
        public void UpdateLabel(int id, string newLabel)
        {
            using (SqlConnection con = connect())
            using (SqlCommand cmd = new SqlCommand("UpdateApartmentLabel", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Id", id);
                cmd.Parameters.AddWithValue("@Label", newLabel);
                cmd.ExecuteNonQuery();
            }
        }

        // Delete a label by ID
        public void DeleteLabel(int id)
        {
            using (SqlConnection con = connect())
            using (SqlCommand cmd = new SqlCommand("DeleteApartmentLabel", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Id", id);
                cmd.ExecuteNonQuery();
            }
        }

        // Get all labels for a specific apartment
        public List<ApartmentLabel> GetLabels(int apartmentId)
        {
            List<ApartmentLabel> labels = new();

            using (SqlConnection con = connect())
            using (SqlCommand cmd = new SqlCommand("GetApartmentLabels", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@ApartmentId", apartmentId);

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        labels.Add(new ApartmentLabel
                        {
                            Id = (int)reader["Id"],
                            ApartmentId = (int)reader["ApartmentId"],
                            Label = reader["Label"].ToString() ?? string.Empty,
                            CreatedAt = (DateTime)reader["CreatedAt"]
                        });
                    }
                }
            }

            return labels;
        }
    }
}
