/*using Microsoft.AspNetCore.Mvc;
using Roome_BackEnd.BL;
using Roome_BackEnd.DAL;

namespace Roome_BackEnd.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApartmentReviewController : ControllerBase
    {
        private readonly DBserviceApartment _dbService;

        public ApartmentReviewController()
        {
            _dbService = new DBserviceApartment();
        }

   
       [HttpPost("AddReview")]
        public IActionResult PostReview([FromBody] ApartmentReview review)
        {
            if (review == null || review.ApartmentID == 0 || review.UserID == 0)
                return BadRequest("Invalid review data.");

            try
            {
                int result = _dbService.PostApartmentReview(review.ApartmentID, review.UserID, review.ReviewText, review.Rate);

                if (result > 0)
                    return Ok(new { message = "Review added successfully.", reviewId = result });
                else
                    return StatusCode(500, "Failed to add review.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }


        
        [HttpGet("GetReviews/{apartmentId}")]
        public IActionResult GetReviews(int apartmentId)
        {
            if (apartmentId == 0)
                return BadRequest("Invalid apartment ID.");

            var reviews = _dbService.GetApartmentReviews(apartmentId);

            if (reviews == null || reviews.Count == 0)
                return NotFound("No reviews found for this apartment.");

            return Ok(reviews);
        }

        
        [HttpDelete("DeleteReview/{reviewId}")]
        public IActionResult DeleteReview(int reviewId)
        {
            if (reviewId == 0)
                return BadRequest("Invalid review ID.");

            bool isDeleted = _dbService.DeleteReview(reviewId);

            if (isDeleted)
                return Ok("Review deleted successfully.");
            else
                return NotFound("Review not found.");
        }
    }
}*/
