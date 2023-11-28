using Microsoft.AspNetCore.Mvc;
using Project1.Models;
using Project1.Processors;
using Serilog;

namespace Project1.Controllers.SubgroupsController
{
    [ApiController]
    public class SubgroupsController : ControllerBase
    {
        private Processor? _processor => Processor.Instance;

        public SubgroupsController()
        {
        }

        [HttpPost]
        [Route("api/[controller]/get-by-groupId")]
        public async Task<IActionResult> GetSubgroups([FromBody] GetSubgroupByGroupIdQuery query)
        {
            try
            {
                if (query == null
                    || query.GroupId == null)
                    return new BadRequestResult();

                var result = query.GroupId == null
                    ? _processor.VariablesEntitiesProcessor.GetSubgroups
                    : _processor.VariablesEntitiesProcessor.GetSubgroupsByGroup((int)query.GroupId);

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
        [Route("api/[controller]/get-variables-entities-by-subgroup")]
        public async Task<IActionResult> GetVariablesBySubgroup([FromBody] GetVariablesEntitiesBySubgroupQuery query)
        {
            try
            {
                if (query == null
                    || query.SubgroupId == null)
                    return new BadRequestResult();

                var result = _processor.VariablesEntitiesProcessor.GetVariablesEntitiesBySubgroup((int)query.SubgroupId);
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