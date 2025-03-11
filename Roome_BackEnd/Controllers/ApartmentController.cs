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
            if(ApartmentService.GetAllApartments()==null){
                return BadRequest("no apartments found");
            }
            return Ok(ApartmentService.GetAllApartments());
        }

        
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
        [HttpGet("GetApartmentById/{apartmentId}")]
        // Get Apartment By Id
public IActionResult GetApartmentById (int apartmentId)
{
    AbstractApartment? apartment= ApartmentService.GetApartmentById(apartmentId);

    if (apartment == null)
    {
        return NotFound($"Apartment with ID {apartmentId} not found.");
    }

    return Ok(apartment);
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
        //edit shared apartment 
        [HttpPut("EditSharedApartment")]
        public IActionResult EditApartment(SharedApartment apartment){
        bool result = ApartmentService.editApartment(apartment);
        return Ok(result);
        }
        //edit rental apartment 
        [HttpPut("EditRentalApartment")]
        public IActionResult EditApartment(RentalApartment apartment){
        bool result = ApartmentService.editApartment(apartment);
        return Ok(result);
        }
        //edit sublet apartment 
        [HttpPut("EditSubletApartment")]
        public IActionResult EditSublet(SubletApartment apartment){
        bool result = ApartmentService.EditApartment(apartment);
        return Ok(result);
        }

    }
}
