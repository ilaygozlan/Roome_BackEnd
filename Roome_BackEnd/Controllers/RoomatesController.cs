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

        /// <summary>
        /// Adds new roommates to an apartment.
        /// </summary>
        /// <param name="apartmentId">The ID of the apartment</param>
        /// <param name="roommatesJson">A JSON string containing roommate details</param>
        /// <returns>ActionResult indicating success or failure</returns>
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

    }
}