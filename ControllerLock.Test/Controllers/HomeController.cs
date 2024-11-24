using ControllerLock.Filter;
using ControllerLock.Test.Model;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ControllerLock.Test.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class HomeController : ControllerBase
    {
        [HttpGet]
        [ControllerLock("id,name")]
        //[ControllerLock("id,name", isDelay: true)]
        public async Task<IActionResult> Test1([FromQuery] RequestDemo value)
        {
            await Task.Delay(5000);
            return Ok(value);
        }

        [HttpPost]
        [ControllerLock("id", isMd5: true)]
        public async Task<IActionResult> Test2([FromBody] RequestDemo value)
        {
            await Task.Delay(5000);
            return Ok(value);
        }

    }
}
