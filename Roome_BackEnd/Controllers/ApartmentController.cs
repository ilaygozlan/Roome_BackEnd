using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.Dynamic;
using Roome_BackEnd.BL;
using Roome_BackEnd.DAL;


namespace Roome_BackEnd.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApartmentController : ControllerBase
    {
      

        // POST add new rentalApartment to DB
        [HttpPost("AddApartment")]
        public int Post([FromBody] RentalApartment newApartment)
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