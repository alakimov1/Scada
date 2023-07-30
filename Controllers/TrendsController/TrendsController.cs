using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Project1.Controllers.TrendsController
{
    [ApiController]
    [Route("api/[controller]")]
    public class TrendsController :ControllerBase
    {
        private readonly ILogger<TrendsController> _logger;
        private readonly Processor _processor;

        public TrendsController(Processor processor, ILogger<TrendsController> logger)
        {
            _processor = processor;
            _logger = logger;
        }

        // GET: TrendsController
        [HttpGet]
        public IActionResult Get(TrendsQuery query)
        {
            DateTime? start = query.Start == null
                ? null
                : DateTime.Parse(query.Start);
            DateTime? end = query.End == null
                ? null
                : DateTime.Parse(query.End);
            var variables = _processor.GetVariables(query.VariablesIds);

            if (variables == null
                || variables.Count == 0)
                return new NotFoundResult();

            var events = _processor.GetEvents(variables);
            var trends = _processor.GetTrend(variables, start, end);
            var responce = new List<TrendsResponce>();

            foreach(var variable in variables)
            {
                if (variable == null) 
                    continue;

                responce.Add(new TrendsResponce()
                    {
                        Trend = trends?.FirstOrDefault(_ => _.Variable.Id == variable.Id),
                        Alarms = events?
                            .Where(_ => _.Variable.Id == variable.Id && _.Type == Models.EventType.Alarm)?
                            .Select(_ => (double)_.Limit)?
                            .ToArray(),
                        Warnings = events?
                            .Where(_ => _.Variable.Id == variable.Id && _.Type == Models.EventType.Warning)?
                            .Select(_ => (double)_.Limit)?
                            .ToArray()
                    });
            }

            return responce==null ||responce.Count==0
                ? new NotFoundResult()
                : new JsonResult(responce);
        }
    }
}
