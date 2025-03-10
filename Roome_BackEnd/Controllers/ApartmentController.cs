using Microsoft.AspNetCore.Mvc;
using Roome_BackEnd.BL;
using Roome_BackEnd.DAL;

namespace Roome_BackEnd.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApartmentController : ControllerBase
    {
        // GET RentalApartments
        [HttpGet("GetRentalApartments")]
        public IActionResult GetAllRentalApartments()
        {
            RentalApartment rentalApartment = new RentalApartment();
            return Ok(rentalApartment.GetApartment());
        }

        // GET SharedApartments
        [HttpGet("GetSharedApartments")]
        public IActionResult GetSharedApartments()
        {
            SharedApartment sharedApartment = new SharedApartment();
            return Ok(sharedApartment.GetSharedApartments());
        }

        // GET SubletApartments
        [HttpGet("GetSubletApartments")]
        public IActionResult GetSubletApartments()
        {
            SubletApartment subletApartment = new SubletApartment();
            return Ok(subletApartment.GetSubletApartments());
        }

        // POST add new rental apartment to DB
        [HttpPost("AddRentalApartment")]
        public int Post([FromBody] RentalApartment newApartment)
        {
            return newApartment.AddApartment();
        }

        // POST add new shared apartment to DB
        [HttpPost("AddSharedApartment")]
        public int Post([FromBody] SharedApartment newApartment)
        {
            return newApartment.AddApartment();
        }

        // POST add new sublet apartment to DB
        [HttpPost("AddSubletApartment")]
        public int Post([FromBody] SubletApartment newApartment)
        {
            return newApartment.AddApartment();
        }

        // PUT deactivate apartment (soft delete)
        [HttpPut("DeactivateApartment/{apartmentId}")]
        public IActionResult DeactivateApartment(int apartmentId)
        {
            DBserviceApartment dbService = new DBserviceApartment();

            RentalApartment? apartment = dbService.GetRentalApartmentById(apartmentId);

            if (apartment == null)
            {
                return NotFound($"Apartment with ID {apartmentId} not found.");
            }

            bool isUpdated = dbService.SoftDeleteRentalApartment(apartmentId);

            if (isUpdated)
                return Ok($"Apartment with ID {apartmentId} has been deactivated.");
            else
                return StatusCode(500, "Failed to deactivate the apartment.");
        }
    }
}
