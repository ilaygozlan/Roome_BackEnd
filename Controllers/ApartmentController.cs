using Microsoft.AspNetCore.Mvc;
using Roome_BackEnd.BL;
using Roome_BackEnd.DAL;

namespace Roome_BackEnd.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApartmentController : ControllerBase
    {
        [HttpGet("GetAllApartments")]
        public IActionResult GetAllApartments()
        {
            var allApartments = ApartmentService.GetAllApartments();
            if (allApartments == null || allApartments.Count == 0)
            {
                return NotFound("No apartments found");
            }

            return Ok(allApartments);
        }

        [HttpGet("GetAllActiveApartments/{userId}")]
        public IActionResult GetAllActiveApartments(int userId)
        {
            List<Dictionary<string, object>> apartments = ApartmentService.GetAllActiveApartments(userId);

            if (apartments == null || apartments.Count == 0)
            {
                return NotFound(new { message = "No active apartments found." });
            }

            return Ok(apartments);
        }

        [HttpGet("GetRentalApartments")]
        public IActionResult GetAllRentalApartments()
        {
            RentalApartment rentalApartment = new();
            return Ok(rentalApartment.GetApartment());
        }

        [HttpGet("GetSharedApartments")]
        public IActionResult GetSharedApartments()
        {
            SharedApartment sharedApartment = new();
            return Ok(sharedApartment.GetSharedApartments());
        }

        [HttpGet("GetSubletApartments")]
        public IActionResult GetSubletApartments()
        {
            SubletApartment subletApartment = new();
            return Ok(subletApartment.GetSubletApartments());
        }

        // ✅ מתוקן: GetApartmentById כולל תוויות
        [HttpGet("GetApartmentById/{apartmentId}")]
        public IActionResult GetApartmentById(int apartmentId)
        {
            AbstractApartment? apartment = ApartmentService.GetApartmentById(apartmentId);

            if (apartment == null)
            {
                return NotFound($"Apartment with ID {apartmentId} not found.");
            }

            return Ok(apartment);
        }

        [HttpPost("AddRentalApartment")]
        public int Post([FromBody] RentalApartment newApartment)
        {
            return newApartment.AddApartment();
        }

        [HttpPost("AddSharedApartment")]
        public int Post([FromBody] SharedApartment newApartment)
        {
            return newApartment.AddApartment();
        }

        [HttpPost("AddSubletApartment")]
        public int Post([FromBody] SubletApartment newApartment)
        {
            return newApartment.AddApartment();
        }

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

        [HttpPut("EditSharedApartment")]
        public IActionResult EditApartment(SharedApartment apartment)
        {
            bool result = ApartmentService.editApartment(apartment);
            return Ok(result);
        }

        [HttpPut("EditRentalApartment")]
        public IActionResult EditApartment(RentalApartment apartment)
        {
            bool result = ApartmentService.editApartment(apartment);
            return Ok(result);
        }

        [HttpPut("EditSubletApartment")]
        public IActionResult EditSublet(SubletApartment apartment)
        {
            bool result = ApartmentService.EditApartment(apartment);
            return Ok(result);
        }
    }
}
