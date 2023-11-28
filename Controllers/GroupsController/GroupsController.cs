using Microsoft.AspNetCore.Mvc;
using Project1.Models;
using Project1.Processors;
using Serilog;

namespace Project1.Controllers.GroupsController
{
    [ApiController]
    public class GroupsController : ControllerBase
    {
        private Processor? _processor => Processor.Instance;

        public GroupsController()
        {
        }

        [HttpPost]
        [Route("api/[controller]/get")]
        public async Task<IActionResult> GetGroups()
        {
            try
            {

                var result = _processor.VariablesEntitiesProcessor.GetGroups;

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
        [Route("api/[controller]/get-variables-by-groups")]
        public async Task<IActionResult> GetVariablesByGroups([FromBody] int[]? ids = null)
        {
            try
            {
                if (ids == null
                    || ids.Length == 0)
                    return new BadRequestResult();

                var result = _processor.VariablesEntitiesProcessor.GetVariableEntitiesByGroups(ids.ToList());
                return new JsonResult(result);
            }
            catch (Exception ex)
            {
                Log.Error($"{ex.Message}{Environment.NewLine}{ex.StackTrace}");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpPost]
        [Route("api/[controller]/get-variables-by-group")]
        public async Task<IActionResult> GetVariablesByGroup([FromBody] int? id = null)
        {
            try
            {
                if (id == null)
                    return new BadRequestResult();

                var result = _processor.VariablesEntitiesProcessor.GetVariableEntitiesByGroup((int)id);
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