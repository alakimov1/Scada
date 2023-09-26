using Microsoft.AspNetCore.Mvc;
using Project1.Models;
using Project1.Processors;

namespace Project1.Controllers.GroupsController
{
    [ApiController]
    public class GroupsController : ControllerBase
    {
        private readonly ILogger<GroupsController> _logger;
        private Processor? _processor => Processor.Instance;

        public GroupsController(ILogger<GroupsController> logger)
        {
            _logger = logger;
        }

        [HttpPost]
        [Route("api/[controller]/get")]
        public async Task<IActionResult> GetGroups()
        {
            var result = _processor.VariablesEntitiesProcessor.GetGroups;

            return result == null
                ? new NotFoundResult()
                : new JsonResult(result);
        }

        [HttpPost]
        [Route("api/[controller]/get-variables-by-groups")]
        public async Task<IActionResult> GetVariablesByGroups([FromBody] int[]? ids = null)
        {
            if (ids == null
                || ids.Length == 0)
                return new BadRequestResult();

            var result = _processor.VariablesEntitiesProcessor.GetVariableEntitiesByGroups(ids.ToList());
            return new JsonResult(result);
        }

        [HttpPost]
        [Route("api/[controller]/get-variables-by-group")]
        public async Task<IActionResult> GetVariablesByGroup([FromBody] int? id = null)
        {
            if (id == null)
                return new BadRequestResult();

            var result = _processor.VariablesEntitiesProcessor.GetVariableEntitiesByGroup((int)id);
            return new JsonResult(result);
        }

    }
}