using System;
using System.Data.SqlClient;
using System.Data;
using Microsoft.Extensions.Configuration;
using System.IO;
using Roome_BackEnd.BL;

namespace Roome_BackEnd.DAL
{
    public class DBserviceUploadImage
    {
        private readonly IConfigurationRoot configuration;

        public DBserviceUploadImage()
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

        //--------------------------------------------------------------------------------------------------
        // This method upload images
        //--------------------------------------------------------------------------------------------------
        public object UploadImages(string imagesLinks, int apartmentId)
        {
            using (SqlConnection con = connect())
            using (SqlCommand cmd = CreateCommandWithStoredProcedureUploadImages("UploadApartmentImages", con, imagesLinks, apartmentId))
            {
                try
                {
                    cmd.ExecuteNonQuery();
                    int rowsAffected = cmd.ExecuteNonQuery();
                    if (rowsAffected >= 0)
                    {
                        return new { message = "Images uploaded successfully!", urls = imagesLinks };
                    }
                    else return new { message = "Error" };
                }
                catch (SqlException sqlEx)
                {
                    throw new Exception("Database error occurred", sqlEx);
                }
                catch (Exception ex)
                {
                    throw new Exception("Failed to execute stored procedure", ex);
                }
            }
        }

        //---------------------------------------------------------------------------------
        // Create the SqlCommand using a stored procedure to upload images
        //---------------------------------------------------------------------------------
        private SqlCommand CreateCommandWithStoredProcedureUploadImages(string spName, SqlConnection con, string imagesLinks, int apartmentId)
        {
            SqlCommand cmd = new SqlCommand
            {
                Connection = con,
                CommandText = spName,
                CommandTimeout = 10,
                CommandType = CommandType.StoredProcedure
            };

            // Output Parameter
            SqlParameter resultParam = new SqlParameter("@RowsAffected", SqlDbType.Int)
            {
                Direction = ParameterDirection.Output
            };
            cmd.Parameters.Add(resultParam);
            cmd.Parameters.AddWithValue("@ApartmentID", apartmentId);
            cmd.Parameters.AddWithValue("@ImageUrls", imagesLinks);

            return cmd;
        }

        public List<string> GetImageUrlsForApartment(int apartmentId)
        {
            List<string> imageUrls = new();

            using (SqlConnection con = connect())
            using (SqlCommand cmd = new SqlCommand("GetApartmentImages", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@ApartmentId", apartmentId);

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string url = reader["ImageUrl"]?.ToString() ?? "";
                        if (!string.IsNullOrWhiteSpace(url))
                        {
                            imageUrls.Add(url);
                        }
                    }
                }
            }

            return imageUrls;
        }

        public bool DeleteImageByUrl(string imageUrl)
        {
            using (SqlConnection con = connect())
            using (SqlCommand cmd = new SqlCommand("DeleteApartmentImageByUrl", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@ImageUrl", imageUrl);

                try
                {
                    int rowsAffected = cmd.ExecuteNonQuery();
                    return rowsAffected > 0;
                }
                catch (SqlException sqlEx)
                {
                    throw new Exception("Database error occurred during image deletion", sqlEx);
                }
                catch (Exception ex)
                {
                    throw new Exception("Failed to delete image", ex);
                }
            }
        }

    }
}
