using Microsoft.AspNetCore.Mvc;
using Project1.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Project1.Controllers.EventsController
{
    [Route("api/[controller]")]
    [ApiController]
    public class EventsController : ControllerBase
    {
        private readonly ILogger<EventsController> _logger;
        private readonly Processor _processor;

        public EventsController(Processor processor, ILogger<EventsController> logger)
        {
            _processor = processor;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Get(EventsQuery query)
        {
            if (query.VariableId == null)
                return new BadRequestResult();

            DateTime? start = query.Start == null
                ? null
                : DateTime.Parse(query.Start);

            DateTime? end = query.End == null
                ? null
                : DateTime.Parse(query.End);

            EventType? type = query.Type == null
                ? null
                : (EventType)query.Type;

            var result = _processor.GetEventHistories(query.VariableId, start, end, type, query.Count);
            return  result==null
                ? new NotFoundResult()
                : new JsonResult(result);
        }
    }
}
