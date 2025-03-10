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
      

        // POST add new user to DB
        [HttpPost("AddNewUser")]
        public int PostAddNewUser([FromBody] User newUser)
        {
            return newUser.AddUser(newUser);
        }

        // GET user by email
        [HttpGet("GetUserByEmail")]
        public User GETUserByEmail([FromBody] User tempUser)
        {
            return tempUser.GetUser(tempUser.Email);
        }

         // GET all users
        [HttpGet("GetAllUsers")]
        public List<dynamic> GETAllUsers()
        {
            return BL.User.GetAllUser();
        }

         // Change user activity
        [HttpPut("DeactivateUser")]
        public int PUTDeactivateUser([FromBody] User tempUser)
        {
            return tempUser.DeactivateUser(tempUser);
        }

        //Update User Details 
        [HttpPut("UpdateUserDetails")]
        public int PUTUserDetails([FromBody] User user)
        {
            return user.UpdateUserDetailsByEmail(user);
        }
    }


}
