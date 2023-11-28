using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace Project1.Controllers.LoggingController
{
    [ApiController]
    public class LoggingController : ControllerBase
    {
        public LoggingController()
        {
        }

        [HttpPost]
        [Route("api/[controller]/error")]
        public IActionResult Error([FromBody] string query)
        {
            try
            {
                Log.Error($"Frontend error{Environment.NewLine} {query}");
                return Ok();
            }
            catch (Exception ex)
            {
                Log.Error($"{ex.Message}{Environment.NewLine}{ex.StackTrace}");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
