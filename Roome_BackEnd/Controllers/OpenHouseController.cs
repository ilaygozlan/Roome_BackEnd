using Microsoft.AspNetCore.Mvc;
using Roome_BackEnd.BL;
using Roome_BackEnd.DAL;

namespace Roome_BackEnd.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OpenHouseController : ControllerBase
    {
        //---------------------------------------------------------------------------------
        // This method returns all open house events for a specific apartment
        //---------------------------------------------------------------------------------
        [HttpGet("GetOpenHousesByApartment/{apartmentId}/{userId}")]
        public ActionResult<List<OpenHouse>> GetOpenHousesByApartment([FromRoute] int apartmentId, int userId)
        {
            if (apartmentId <= 0)
            {
                return BadRequest("Invalid Apartment ID.");
            }

            try
            {
                List<OpenHouse> openHouses = OpenHouse.GetOpenHousesForApartment(apartmentId, userId);

                if (openHouses == null || openHouses.Count == 0)
                {
                    return NotFound("No open house events found for this apartment.");
                }

                return Ok(openHouses);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return StatusCode(500, "An error occurred while retrieving open house events.");
            }
        }


        //---------------------------------------------------------------------------------
        // This method creates a new open house event
        //---------------------------------------------------------------------------------
        [HttpPost("CreateNewOpenHouse/{userId}")]
        public ActionResult<string> CreateNewOpenHouse([FromBody] OpenHouse openHouse, [FromRoute] int userId)
        {
            if (userId <= 0)
            {
                return BadRequest("Invalid user ID.");
            }

            int result = OpenHouse.CreateAnOpenHouse(openHouse, userId);

            if (result == 0)
            {
                return Conflict("Open house already exists or failed to create.");
            }

            return Ok("New open house created successfully!");
        }


        //---------------------------------------------------------------------------------
        // This method registers a user for an open house event
        //---------------------------------------------------------------------------------
        [HttpPost("RegisterForOpenHouse")]
        public ActionResult<string> RegisterForOpenHouse([FromBody] RegisterOpenHouseRequest request)
        {
            if (request.UserID <= 0 || request.OpenHouseID <= 0)
            {
                return BadRequest("Invalid user ID or open house ID.");
            }

            bool success = OpenHouse.RegisterForOpenHouse(request.OpenHouseID, request.UserID, request.Confirmed);

            if (!success)
            {
                return Conflict("User is already registered or event does not exist.");
            }

            return Ok("User registered successfully for the open house.");
        }


        //---------------------------------------------------------------------------------
        // This method Toggle Attendance for open house
        //---------------------------------------------------------------------------------
        [HttpPut("ToggleAttendance/{openHouseId}/{userId}")]
        public ActionResult<string> ToggleAttendance([FromRoute] int openHouseId, [FromRoute] int userId)
        {
            if (openHouseId <= 0 || userId <= 0)
            {
                return BadRequest("Invalid Open House ID or User ID.");
            }

            bool result = OpenHouse.ToggleAttendance(openHouseId, userId);

            if (!result)
            {
                return Conflict("Failed to update attendance status.");
            }

            return Ok("Attendance status updated successfully.");
        }

        // DELETE: Delete open house
        [HttpDelete("DeleteOpenHouse/{openHouseId}/{userId}")]
        public ActionResult<string> DeleteOpenHouse([FromRoute] int openHouseId, [FromRoute] int userId)
        {
            if (openHouseId <= 0 || userId <= 0)
            {
                return BadRequest("Invalid Open House ID or User ID.");
            }

            bool result = OpenHouse.DeleteOpenHouse(openHouseId, userId);

            if (!result)
            {
                return NotFound("Open house does not exist or user is not authorized.");
            }

            return Ok("Open house and all its registrations deleted successfully.");
        }

        [HttpDelete("DeleteRegistration/{openHouseId}/{userId}")]
        public ActionResult<string> DeleteRegistrationForOpenHouse([FromRoute] int openHouseId, [FromRoute] int userId)
        {
            if (openHouseId <= 0 || userId <= 0)
            {
                return BadRequest("Invalid Open House ID or User ID.");
            }

            bool success = OpenHouse.DeleteRegistrationForOpenHouse(openHouseId, userId);

            if (!success)
            {
                return NotFound("Registration not found or failed to delete.");
            }

            return Ok("Registration deleted successfully.");
        }

    }


    // DTO for Open House Registration
    public class RegisterOpenHouseRequest
    {
        public int OpenHouseID { get; set; }
        public int UserID { get; set; }
        public bool Confirmed { get; set; } = false;
    }
}
