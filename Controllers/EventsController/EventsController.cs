using Microsoft.AspNetCore.Mvc;
using Project1.Models;
using Project1.Processors;

namespace Project1.Controllers.EventsController
{
    [ApiController]
    public class EventsController : ControllerBase
    {
        private readonly ILogger<EventsController> _logger;
        private Processor? _processor => Processor.Instance;

        public EventsController(ILogger<EventsController> logger)
        {
            _logger = logger;
        }

        [HttpPost]
        [Route("api/[controller]/get")]
        public async Task<IActionResult> Get([FromBody]EventsQuery query)
        {
            if (_processor == null
                || _processor.EventsProcessor == null)
                return StatusCode(StatusCodes.Status500InternalServerError);

            DateTime? start = query.Start == null
                ? null
                : DateTime.Parse(query.Start);

            DateTime? end = query.End == null
                ? null
                : DateTime.Parse(query.End);

            var eventTypes = await _processor.EventsProcessor.GetEventTypes();

            if (eventTypes == null)
                return StatusCode(StatusCodes.Status500InternalServerError);

            List<int>? type = query.Type == null
                ? null
                : eventTypes!.Where(_ => query.Type.Contains((int)_.Id)).Select(_=>(int)_.Id).ToList();

            var result = await _processor
                                .EventsProcessor
                                .GetEventHistories(query.VariableId, start, end, type, query.Count);
            return  result==null
                ? new NotFoundResult()
                : new JsonResult(result);
        }

        [HttpGet]
        [Route(("api/[controller]/get-event-types"))]
        public async Task<IActionResult> GetEventTypes()
        {
            if (_processor == null
                || _processor.EventsProcessor == null)
                return StatusCode(StatusCodes.Status500InternalServerError);

            var result = await _processor.EventsProcessor.GetEventTypes();

            return result == null
                ? new NotFoundResult()
                : new JsonResult(result);
        }
    }
}
