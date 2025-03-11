using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.Dynamic;
using Roome_BackEnd.BL;
using Roome_BackEnd.DAL;

namespace Roome_BackEnd.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OpenHouseController : ControllerBase
    {

     //Add friend
    [HttpPost("CreateNewOpenHouse/{userId}")]
    public ActionResult<string> CreateNewOpenHouse([FromBody] OpenHouse openHouse,[FromRoute] int userId)
    {

        if (userId <= 0 )
        {
            return BadRequest("Invalid user IDs.");
        }

        int result = OpenHouse.CreateAnOpenHouse(openHouse, userId);

        if (result < 1)
        {
            return Conflict("Didn't create new appartment.");
        }

        return Ok("New open house create!");
    }

}


}
