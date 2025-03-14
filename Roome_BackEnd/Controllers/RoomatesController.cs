using Microsoft.AspNetCore.Mvc;
using Roome_BackEnd.BL;
using Roome_BackEnd.DAL;
using Newtonsoft.Json;
using System;

namespace Roome_BackEnd.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoomatesController : ControllerBase
    {
        private readonly DBserviceRoomates _dbService;

        [HttpPost("AddNewRoomates")]
        public IActionResult AddRoommates([FromQuery] int apartmentId, [FromBody] List<Roomate> roommates)
        {
        if (roommates == null || roommates.Count == 0)
            {
                return BadRequest("Roommates data is required.");
            }

        string roommatesJson = JsonConvert.SerializeObject(roommates); // Convert to JSON string

        bool isSuccess = Roomate.AddRoommates(roommatesJson, apartmentId);

        return isSuccess ? Ok("Roommates added successfully.") : StatusCode(500, "Failed to add roommates.");
        }

      [HttpDelete("DeleteRoommate/{appartmentId}")]
        public ActionResult<string> DeleteOpenHouse([FromRoute] int appartmentId, [FromBody] string roommateName)
        {
            if (appartmentId <= 0 || appartmentId == null)
            {
                return BadRequest("Invalid appartment ID.");
            }

            bool result = Roomate.DeleteRoommate(roommateName, appartmentId);

            if (!result)
            {
                return NotFound("Apartment does not exist or roommate.");
            }

            return Ok("Roommate deleted successfully.");
        }
    }
}