using Microsoft.AspNetCore.Mvc;
using Project1.Models;

namespace Project1.Controllers.VariablesController
{
    [ApiController]
    [Route("api/[controller]")]
    public class VariablesController : ControllerBase
    {
        private readonly ILogger<VariablesController> _logger;
        private readonly Processor _processor;

        public VariablesController(Processor processor, ILogger<VariablesController> logger)
        {
            _processor = processor;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Get([FromBody] int[] ids = null)
        {
            var result = _processor.GetVariables(ids);
            return result == null
                ? new NotFoundResult()
                : new JsonResult(result);
        }

        [HttpPost]
        public IActionResult Post([FromBody] Variable[] variables)
        {
            if (variables == null)
                return new BadRequestResult();

            _processor.ChangeVariables(variables.ToList());
            return new OkResult();
        }
    }
}