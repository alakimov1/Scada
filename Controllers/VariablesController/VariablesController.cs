using Microsoft.AspNetCore.Mvc;
using Project1.Models;
using Project1.Processors;
using Serilog;

namespace Project1.Controllers.VariablesController
{
    [ApiController]
    public class VariablesController : ControllerBase
    {
        private Processor? _processor => Processor.Instance;

        public VariablesController()
        {
        }

        [HttpPost]
        [Route("api/[controller]/get")]
        public IActionResult Get([FromBody] int[]? ids = null)
        {
            try
            {
                if (_processor == null)
                    return StatusCode(StatusCodes.Status500InternalServerError);

                var result = _processor.VariablesEntitiesProcessor.GetVariables(ids);

                return result == null
                    ? new NotFoundResult()
                    : new JsonResult(result);
            }
            catch (Exception ex)
            {
                Log.Error($"{ex.Message}{Environment.NewLine}{ex.StackTrace}");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpPost]
        [Route("api/[controller]/get-values")]
        public IActionResult GetValues([FromBody] int[]? ids = null)
        {
            try
            {
                if (_processor == null)
                    return StatusCode(StatusCodes.Status500InternalServerError);

                var result = _processor.GetVariablesValues(ids);

                return result == null
                    ? new NotFoundResult()
                    : new JsonResult(result);
            }
            catch (Exception ex)
            {
                Log.Error($"{ex.Message}{Environment.NewLine}{ex.StackTrace}");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpPost]
        [Route("api/[controller]/change")]
        public async Task<IActionResult> Change([FromBody] VariableChangeQuery[] variables)
        {
            try
            {
                if (_processor == null)
                    return StatusCode(StatusCodes.Status500InternalServerError);

                if (variables == null)
                    return new BadRequestResult();

                var resultDictionary = await _processor.ChangeVariables(variables);
                var result = resultDictionary.Select(_ => new { Id = _.Key, Result = _.Value }).ToArray();
                return new JsonResult(result);
            }
            catch (Exception ex)
            {
                Log.Error($"{ex.Message}{Environment.NewLine}{ex.StackTrace}");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
    }
}