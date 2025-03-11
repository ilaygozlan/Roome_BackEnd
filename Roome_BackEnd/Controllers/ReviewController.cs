/*using Microsoft.AspNetCore.Mvc;
using Roome_BackEnd.BL;
using Roome_BackEnd.DAL;


namespace Roome_BackEnd.Controllers{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewController : ControllerBase
    {
        // POST add new review to DB
        [HttpPost("AddNewReview")]
        public ActionResult<int> PostAddNewReview([FromBody] Review newReview)
        {
            if (newReview == null || newReview.ApartmentId <= 0 || newReview.UserId <= 0 || string.IsNullOrWhiteSpace(newReview.ReviewText) || newReview.Rate < 1 || newReview.Rate > 5)
            {
                return BadRequest("Invalid review details.");
            }

            int result = newReview.AddReview(newReview);

            if (result <= 0)
            {
                return Conflict("Review could not be added.");
            }

            return Ok(result);
        }
        // delete review
        [HttpDelete("DeleteReview/{reviewId}")]
        public ActionResult<int> DeleteReview(int reviewId)
        {
            if (reviewId <= 0)
            {
                return BadRequest("Invalid review ID.");
            }

            Review review = new Review();
            int result = review.DeleteReview(reviewId);

            if (result == 0)
            {
                return NotFound("Review not found.");
            }

            return Ok("Review deleted successfully.");
        }


          //Get Reviews For Apartment
            [HttpGet("GetReviewsForApartment/{apartmentId}")]
            public ActionResult<List<Review>> GetReviewsForApartment(int apartmentId)
            {
                if (apartmentId <= 0)
                {
                    return BadRequest("Invalid apartment ID.");
                }

                Review review = new Review();
                List<Review> reviews = review.GetReviewsForApartment(apartmentId);

                if (reviews == null || reviews.Count == 0)
                {
                    return NotFound("No reviews found for this apartment.");
                }

                return Ok(reviews);
            }

    }
}
*/