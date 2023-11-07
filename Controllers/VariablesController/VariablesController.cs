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
        [Route("api/[controller]/variables/get-values")]
        public IActionResult GetValues([FromBody] int[]? ids = null)
        {
            if (_processor == null)
                return StatusCode(StatusCodes.Status500InternalServerError);

            var result = _processor.GetVariablesValues(ids);

            return result == null
                ? new NotFoundResult()
                : new JsonResult(result);
        }

        [HttpPost]
        [Route("api/[controller]/change")]
        public async Task<IActionResult> Change([FromBody] VariableChangeQuery[] variables)
        {
            if (_processor == null)
                return StatusCode(StatusCodes.Status500InternalServerError);

            if (variables == null)
                return new BadRequestResult();

            var resultDictionary = await _processor.ChangeVariables(variables);
            var result = resultDictionary.Select(_ => new { Id = _.Key, Result = _.Value }).ToArray();
            return new JsonResult(result);
        }
    }
}