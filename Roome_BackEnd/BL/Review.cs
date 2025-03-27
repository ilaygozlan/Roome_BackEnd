using System;
using System.Collections.Generic;
using Roome_BackEnd.DAL;

namespace Roome_BackEnd.BL
{
    public class Review
    {
        private int reviewId = 0;
        private int apartmentId;
        private int rate;
        private string reviewText; 
        private int userId;

        public int ReviewId { get; set; }
        public int ApartmentId { get; set; }
        public int Rate { get; set; }
        public string ReviewText { get; set; }
        public int UserId { get; set; }

       
        public Review() { }

       
        public Review(int reviewId, int apartmentId, int rate, string reviewText, int userId)
        {
            ReviewId = reviewId;
            ApartmentId = apartmentId;
            Rate = rate;
            ReviewText = reviewText;
            UserId = userId;
        }

        
        public int AddReview(Review newReview)
        {
            if (newReview == null || newReview.ApartmentId <= 0 || newReview.UserId <= 0 || 
                string.IsNullOrWhiteSpace(newReview.ReviewText) || newReview.Rate < 1 || newReview.Rate > 5)
            {
                throw new ArgumentException("Invalid review details.");
            }

            DBserviceReviews dbServiceReviews = new DBserviceReviews();
            return dbServiceReviews.AddNewReview(newReview);
        }

       public int DeleteReview(int reviewId)
        {
            if (reviewId <= 0)
            {
                throw new ArgumentException("Invalid review ID.");
            }

            DBserviceReviews dbServiceReviews = new DBserviceReviews();
            return dbServiceReviews.DeleteReview(reviewId);
        }

        public List<Review> GetReviewsForApartment(int apartmentId)
        {
            if (apartmentId <= 0)
            {
                throw new ArgumentException("Invalid apartment ID.");
            }

            DBserviceReviews dbServiceReviews = new DBserviceReviews();
            return dbServiceReviews.GetReviewsForApartment(apartmentId);
        }


    }
}
