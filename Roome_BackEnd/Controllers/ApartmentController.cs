using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.Dynamic;
using Roome_BackEnd.BL;
using Roome_BackEnd.DAL;
using Microsoft.AspNetCore.StaticFiles.Infrastructure;


namespace Roome_BackEnd.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApartmentController : ControllerBase
    {
      
        //GET RentalApartments
        [HttpGet("GetRentalApartments")]
        public IActionResult GetAllRentalApartments(){
           RentalApartment rentalApartment = new RentalApartment(); 
        return Ok(rentalApartment.GetApartment()); 
        }
       //GET SharedApartments
        [HttpGet("GetSharedApartments")]
        public IActionResult GetSharedApartments(){
           SharedApartment sharedApartment = new SharedApartment(); 
        return Ok(sharedApartment.GetSharedApartments()); 
        }

        // POST add new rentalApartment to DB
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

        // put apartment to not active 
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
    }}