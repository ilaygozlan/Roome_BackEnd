using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.Dynamic;
using Roome_BackEnd.BL;


namespace Roome_BackEnd.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
      

        // POST add new book to DB
        [HttpPost("AddNewUser")]
        public int Post([FromBody] User newUser)
        {
            return newUser.AddUser(newUser);
        }

    }


}
