using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Project1.Processors;
using Serilog;

namespace Project1.Controllers.TrendsController
{
    [ApiController]
    public class TrendsController : ControllerBase
    {
        private Processor? _processor => Processor.Instance;
        
        public TrendsController()
        {
        }

        [HttpGet]
        [Route("api/[controller]/get-variables")]
        public IActionResult Get()
        {
            try
            {
                if (_processor == null)
                    return StatusCode(StatusCodes.Status500InternalServerError);

                return new JsonResult(_processor.VariablesEntitiesProcessor.GetVariables()?.Where(_ => _.TrendingPeriod > 0).ToArray());
            }
            catch (Exception ex)
            {
                Log.Error($"{ex.Message}{Environment.NewLine}{ex.StackTrace}");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpPost]
        [Route("api/[controller]/get")]
        public async Task<IActionResult> Post([FromBody] TrendsQuery query)
        {
            try
            {
                if (_processor == null)
                    return StatusCode(StatusCodes.Status500InternalServerError);

                DateTime? start = query.Start == null
                    ? null
                    : DateTime.Parse(query.Start, null, System.Globalization.DateTimeStyles.RoundtripKind);
                DateTime? end = query.End == null
                    ? null
                    : DateTime.Parse(query.End);

                if (query.VariableId == null)
                    return new BadRequestResult();

                var variables = _processor.VariablesEntitiesProcessor.GetVariables(new int[] { (int)query.VariableId });

                if (variables == null
                    || variables.Count == 0)
                    return new NotFoundResult();

                var events = await _processor.EventsProcessor.GetEvents(variables);
                var trends = await _processor.TrendsProcessor.ReadTrends(variables, start, end);
                var responce = new List<TrendsResponce>();

                foreach (var variable in variables)
                {
                    if (variable == null)
                        continue;

                    var eventLines = new List<TrendsResponceEventLine>();

                    foreach (var variableEvent in events?.Where(_ => _.Variable.Id == variable.Id))
                    {
                        eventLines.Add(new TrendsResponceEventLine()
                        {
                            EventType = variableEvent.Type,
                            Value = Convert.ToDouble(variableEvent.Limit)
                        }
                        );
                    }

                    responce.Add(new TrendsResponce()
                    {
                        Trend = trends?.FirstOrDefault(_ => _.Variable.Id == variable.Id),
                        EventLines = eventLines.ToArray()
                    });
                }

                return responce == null || responce.Count == 0
                    ? new NotFoundResult()
                    : new JsonResult(responce);
            }
            catch (Exception ex)
            {
                Log.Error($"{ex.Message}{Environment.NewLine}{ex.StackTrace}");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
