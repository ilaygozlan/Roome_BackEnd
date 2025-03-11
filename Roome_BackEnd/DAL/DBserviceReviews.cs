using System.Data.SqlClient;
using System.Data;
using Microsoft.Extensions.Configuration;
using System.IO;
using Roome_BackEnd.BL;

namespace Roome_BackEnd.DAL
{
    public class DBserviceReviews
    {
        private readonly IConfigurationRoot configuration;

        public DBserviceReviews()
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
        // This method adds a new review
        //--------------------------------------------------------------------------------------------------
        public int AddNewReview(Review review)
        {
            if (review == null || review.ApartmentId <= 0 || review.UserId <= 0 || string.IsNullOrWhiteSpace(review.ReviewText) || review.Rate < 1 || review.Rate > 5)
            {
                Console.WriteLine("Invalid review data.");
                throw new ArgumentException("Invalid review details.");
            }

            using (SqlConnection con = connect())
            using (SqlCommand cmd = CreateCommandWithStoredProcedureAddNewReview("PostApartmentReview", con, review.ApartmentId, review.UserId, review.ReviewText, review.Rate))
            {
                try
                {
                    Console.WriteLine($"Attempting to add review: ApartmentID={review.ApartmentId}, UserID={review.UserId}, Rate={review.Rate}");

                    cmd.ExecuteNonQuery();

                    int newReviewId = (cmd.Parameters["@NewReviewID"].Value != DBNull.Value) ? Convert.ToInt32(cmd.Parameters["@NewReviewID"].Value) : -1;

                    if (newReviewId <= 0)
                    {
                        Console.WriteLine("Failed to add review.");
                        return -1;
                    }

                    Console.WriteLine($"Review added successfully. New Review ID: {newReviewId}");
                    return newReviewId;
                }
                catch (SqlException sqlEx)
                {
                    Console.WriteLine($"SQL Error: {sqlEx.Message}");
                    throw new Exception("Database error occurred", sqlEx);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                    throw new Exception("Failed to execute stored procedure", ex);
                }
            }
        }

            //---------------------------------------------------------------------------------
        // Create the SqlCommand using a stored procedure to add a new review
        //---------------------------------------------------------------------------------


        private SqlCommand CreateCommandWithStoredProcedureAddNewReview(string spName, SqlConnection con, int apartmentId, int userId, string reviewText, int rate)
        {
            SqlCommand cmd = new SqlCommand
            {
                Connection = con,
                CommandText = spName,
                CommandTimeout = 10,
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.Add(new SqlParameter("@ApartmentID", SqlDbType.Int) { Value = apartmentId });
            cmd.Parameters.Add(new SqlParameter("@UserID", SqlDbType.Int) { Value = userId });
            cmd.Parameters.Add(new SqlParameter("@Review", SqlDbType.NVarChar, -1) { Value = reviewText });
            cmd.Parameters.Add(new SqlParameter("@Rate", SqlDbType.Int) { Value = rate });

            SqlParameter outputParam = new SqlParameter("@NewReviewID", SqlDbType.Int)
            {
                Direction = ParameterDirection.Output
            };
            cmd.Parameters.Add(outputParam);

            return cmd;
        }

        
        //--------------------------------------------------------------------------------------------------
        // This method deletes a review
        //--------------------------------------------------------------------------------------------------
        public int DeleteReview(int reviewId)
        {
            if (reviewId <= 0)
            {
                Console.WriteLine("Invalid review ID.");
                throw new ArgumentException("Invalid review ID.");
            }

            using (SqlConnection con = connect())
            using (SqlCommand cmd = CreateCommandWithStoredProcedureDeleteReview("deleteReview", con, reviewId))
            {
                try
                {
                    Console.WriteLine($"Attempting to delete review with ID={reviewId}");

                    cmd.ExecuteNonQuery();

                    int rowsAffected = (cmd.Parameters["@RowsAffected"].Value != DBNull.Value) ? Convert.ToInt32(cmd.Parameters["@RowsAffected"].Value) : 0;

                    if (rowsAffected == 0)
                    {
                        Console.WriteLine("Review not found.");
                        return 0; // Return 0 if the review does not exist
                    }

                    Console.WriteLine($"Review deleted successfully. Review ID: {reviewId}");
                    return 1; // Return 1 if the review was successfully deleted
                }
                catch (SqlException sqlEx)
                {
                    Console.WriteLine($"SQL Error: {sqlEx.Message}");
                    throw new Exception("Database error occurred", sqlEx);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                    throw new Exception("Failed to execute stored procedure", ex);
                }
            }
        }

        //---------------------------------------------------------------------------------
        // Create the SqlCommand using a stored procedure to delete a review
        //---------------------------------------------------------------------------------
        private SqlCommand CreateCommandWithStoredProcedureDeleteReview(string spName, SqlConnection con, int reviewId)
        {
            SqlCommand cmd = new SqlCommand
            {
                Connection = con,
                CommandText = spName,
                CommandTimeout = 10,
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.Add(new SqlParameter("@reviewId", SqlDbType.Int) { Value = reviewId });

            SqlParameter outputParam = new SqlParameter("@RowsAffected", SqlDbType.Int)
            {
                Direction = ParameterDirection.Output
            };
            cmd.Parameters.Add(outputParam);

            return cmd;
        }

        //--------------------------------------------------------------------------------------------------
// This method retrieves all reviews for a specific apartment
//--------------------------------------------------------------------------------------------------
public List<Review> GetReviewsForApartment(int apartmentId)
{
    if (apartmentId <= 0)
    {
        throw new ArgumentException("Invalid apartment ID.");
    }

    List<Review> reviews = new List<Review>();

    using (SqlConnection con = connect())
    using (SqlCommand cmd = CreateCommandWithStoredProcedureGetReviews("getReview", con, apartmentId))
    {
        try
        {
            Console.WriteLine($"Fetching reviews for Apartment ID={apartmentId}");

            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    reviews.Add(new Review
                    {
                        ReviewId = reader.GetInt32(reader.GetOrdinal("reviewId")),
                        ApartmentId = reader.GetInt32(reader.GetOrdinal("ApartmentID")),
                        Rate = reader.GetInt32(reader.GetOrdinal("rate")),
                        ReviewText = reader.GetString(reader.GetOrdinal("review")),
                        UserId = reader.GetInt32(reader.GetOrdinal("UserID"))
                    });
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
            throw new Exception("Failed to retrieve reviews", ex);
        }
    }

    return reviews;
}

//---------------------------------------------------------------------------------
// Create the SqlCommand using a stored procedure to get all reviews for an apartment
//---------------------------------------------------------------------------------
private SqlCommand CreateCommandWithStoredProcedureGetReviews(string spName, SqlConnection con, int apartmentId)
{
    SqlCommand cmd = new SqlCommand
    {
        Connection = con,
        CommandText = spName,
        CommandTimeout = 10,
        CommandType = CommandType.StoredProcedure
    };

    cmd.Parameters.Add(new SqlParameter("@ApartmentID", SqlDbType.Int) { Value = apartmentId });

    return cmd;
}

    }
}
