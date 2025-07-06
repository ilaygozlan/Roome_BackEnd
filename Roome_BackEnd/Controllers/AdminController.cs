using Microsoft.AspNetCore.Mvc;
using Roome_BackEnd.BL;
using Roome_BackEnd.DAL;

namespace Roome_BackEnd.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
         [HttpGet("GetAllApartmentsForAdmin")]
        public IActionResult GetAllApartmentsForAdmin()
        {
            if(ApartmentService.GetAllApartmentsForAdmin()==null){
                return BadRequest("no apartments found");
            }
            return Ok(ApartmentService.GetAllApartmentsForAdmin());
        }

     }
}
