using Microsoft.AspNetCore.Mvc;
using Roome_BackEnd.BL;
using Roome_BackEnd.DAL;

namespace Roome_BackEnd.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        [HttpGet("GetAllApartmentsForAdmin")]
        public IActionResult GetAllApartmentsForAdmin()
        {
            if (ApartmentService.GetAllApartmentsForAdmin() == null)
            {
                return BadRequest("no apartments found");
            }
            return Ok(ApartmentService.GetAllApartmentsForAdmin());
        }
        // PUT toggle active status for any apartment type
        [HttpPut("ToggleActive/{apartmentId}")]
        public IActionResult ToggleApartmentActiveStatus(int apartmentId)
        {

            try
            {
                if (apartmentId <= 0)
                    return BadRequest("Invalid Apartment ID.");

                string result = ApartmentService.ToggleApartmentActiveStatus(apartmentId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }
        
        // GET all users
        [HttpGet("GetAllUsers")]
        public ActionResult<List<User>> GETAllUsers()
        {
            var users = BL.User.GetAllUser();

            if (users == null || users.Count == 0)
            {
                return NotFound("No users found.");
            }

            return Ok(users);
        }
        //Update User activity
        [HttpPut("DeactivateUser/{userEmail}")]
        public ActionResult<int> PUTDeactivateUser(string userEmail)
        {
            string decodedEmail = Uri.UnescapeDataString(userEmail);
            Console.WriteLine($"Received userEmail: '{decodedEmail}'");

            int result = BL.User.DeactivateUser(decodedEmail);

            if (result < 0)
            {
                Console.WriteLine("Unexpected error occurred.");
                return StatusCode(500, "Unexpected error occurred.");
            }

            if (result == 0)
            {
                Console.WriteLine("User not found or already deactivated.");
                return NotFound("User not found or already deactivated.");
            }

            Console.WriteLine($"User deactivated successfully. Rows affected: {result}");
            return Ok(result);
        }


    }
}
