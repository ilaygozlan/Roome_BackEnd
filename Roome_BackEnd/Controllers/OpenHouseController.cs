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
        // This method Get Owner Id
        //---------------------------------------------------------------------------------
[HttpGet("GetOwnerId/{openHouseId}")]
public IActionResult GetOwnerId(int openHouseId)
{
    try
    {
        int ownerId = OpenHouse.GetOwnerId(openHouseId);
        
        if(ownerId <= 0)
            return NotFound("Owner not found for this open house event.");

        return Ok(ownerId);
    }
    catch (Exception ex)
    {
        return StatusCode(500, $"Internal server error: {ex.Message}");
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
        // This method Get Open Houses By User
        //---------------------------------------------------------------------------------
        [HttpGet("getByUser/{userId}")]
            public IActionResult GetOpenHousesByUser(int userId)
            {
                try
                {
                    var openHouses = OpenHouse.GetOpenHousesForUser(userId);
                    return Ok(openHouses);
                }
                catch (Exception ex)
                {
                    return StatusCode(500, $"Internal server error: {ex.Message}");
                }
            }


        //---------------------------------------------------------------------------------
        // This method Register And Sync To Calendar
        //---------------------------------------------------------------------------------
[HttpPost("RegisterAndSyncToCalendar")]
public async Task<IActionResult> RegisterAndSyncToCalendar([FromQuery] int userId, [FromQuery] int openHouseId)
{
    Console.WriteLine($"[SERVER] Starting RegisterAndSyncToCalendar for userId={userId}, openHouseId={openHouseId}");

    try
    {
        DBservicesOpenHouse db = new DBservicesOpenHouse();
        bool success = db.RegisterForOpenHouse(openHouseId, userId, confirmed: true);
        Console.WriteLine($"[SERVER] Registration result: {success}");

        if (!success)
        {
            Console.WriteLine("[SERVER] Registration failed. Returning BadRequest.");
            return BadRequest("Failed to register for the open house.");
        }

        OpenHouse openHouse = db.GetOpenHouseById(openHouseId);
        Console.WriteLine($"[SERVER] OpenHouse loaded: ID={openHouse.OpenHouseId}, Date={openHouse.Date}, Time={openHouse.StartTime}-{openHouse.EndTime}");

        DBserviceUser userDb = new DBserviceUser();
        string token = userDb.GetToken(userId);
        Console.WriteLine($"[SERVER] Retrieved token: {(string.IsNullOrEmpty(token) ? "NULL or empty" : token.Substring(0, 10) + "...")}");

        string calendarLink = null;

        if (!string.IsNullOrEmpty(token))
        {
            try
            {
                calendarLink = await GoogleCalendarService.AddOpenHouseToCalendarAsync(openHouse, token);
                Console.WriteLine($"[SERVER] Calendar event created. Link: {calendarLink}");
            }
            catch (Exception calendarEx)
            {
                Console.WriteLine($"[SERVER] Error while syncing to calendar: {calendarEx.Message}");
            }
        }
        else
        {
            Console.WriteLine("[SERVER] No token found for user. Skipping calendar sync.");
        }

        return Ok(new
        {
            message = "Registration successful and event synced to Google Calendar.",
            calendarEventLink = calendarLink
        });
    }
    catch (Exception ex)
    {
        Console.WriteLine($"[SERVER] Internal server error: {ex}");
        return StatusCode(500, $"Internal server error: {ex.Message}");
    }
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
