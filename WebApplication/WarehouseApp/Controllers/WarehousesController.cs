using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebApplication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WarehousesController : ControllerBase
    {
        [HttpPost]
        public IActionResult Add()
        {
            //check if product with given id exist
            // _repo.DoesProductExist(id)
            //404 
            
            
            
            return Ok();
        }
    }
}
