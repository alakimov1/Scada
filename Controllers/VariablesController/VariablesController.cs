using Microsoft.AspNetCore.Mvc;
using Project1.Models;
using Project1.Processors;

namespace Project1.Controllers.VariablesController
{
    [ApiController]
    public class VariablesController : ControllerBase
    {
        private readonly ILogger<VariablesController> _logger;
        private Processor? _processor => Processor.Instance;

        public VariablesController(ILogger<VariablesController> logger)
        {
            _logger = logger;
        }

        [HttpPost]
        [Route("api/[controller]/variables/get")]
        public IActionResult Get([FromBody] int[]? ids = null)
        {
            if (_processor == null)
                return StatusCode(StatusCodes.Status500InternalServerError);

            var result = _processor.VariablesEntitiesProcessor.GetVariables(ids);

            return result == null
                ? new NotFoundResult()
                : new JsonResult(result);
        }



        [HttpPost]
        [Route("api/[controller]/change")]
        public async Task<IActionResult> Change([FromBody] VariableEntity[] variablesEntities)
        {
            if (_processor == null)
                return StatusCode(StatusCodes.Status500InternalServerError);

            if (variablesEntities == null)
                return new BadRequestResult();

             await _processor.ChangeVariables(variablesEntities.ToList());
            return new OkResult();
        }
    }
}