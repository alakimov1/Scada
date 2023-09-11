using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Project1.Models;
using Project1.Processors;

namespace Project1.Controllers.VariablesController
{
    [ApiController]
    public class VariablesController : ControllerBase
    {
        private readonly ILogger<VariablesController> _logger;
        private Processor? _processor=> Processor.Instance;

        public VariablesController(ILogger<VariablesController> logger)
        {
            _logger = logger;
        }

        [HttpPost]
        [Route("api/[controller]/get")]
        public IActionResult Get([FromBody] int[]? ids = null)
        {
            if (_processor == null)
                return StatusCode(StatusCodes.Status500InternalServerError);

            var result = _processor.GetVariables(ids);
            return result == null
                ? new NotFoundResult()
                : new JsonResult(result);
        }

        [HttpPost]
        [Route("api/[controller]/change")]
        public IActionResult Change([FromBody] Variable[] variables)
        {
            if (_processor == null)
                return StatusCode(StatusCodes.Status500InternalServerError);

            if (variables == null)
                return new BadRequestResult();

            _processor.ChangeVariables(variables.ToList());
            return new OkResult();
        }

        [HttpGet]
        [Route("api/[controller]/settings")]
        public IActionResult GetSettings()
        {

        }
    }
}