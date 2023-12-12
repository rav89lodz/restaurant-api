using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RestaurantAPI.WeatherForecastItems;
using System.Collections.Generic;

namespace RestaurantAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class WeatherForecastController : ControllerBase
{
    private readonly IWeatherForecastService _service;
    private readonly ILogger<WeatherForecastController> _logger;

    public WeatherForecastController(ILogger<WeatherForecastController> logger, IWeatherForecastService service)
    {
        _logger = logger;
        _service = service;
    }

    [HttpGet]
    public IEnumerable<WeatherForecast> Get()
    {
        var result = _service.Get(-21, 50, 5);
        return result;
    }

    [HttpGet]
    [Route("currentDay/{max}")]
    public IEnumerable<WeatherForecast> Get2([FromQuery]int take, [FromRoute]int max)
    {
        var result = _service.Get(-21, max, take);
        return result;
    }

    [HttpPost]
    public ActionResult<string> Hello([FromBody] string name)
    {
        // HttpContext.Response.StatusCode = 401;
        // return $"Twoje imiê to {name}";

        // return StatusCode(401, $"Twoje imiê to {name}");

        return Unauthorized($"Twoje imiê to {name}");
    }

    [HttpPost("generate")]
    public ActionResult<IEnumerable<WeatherForecast>> Generate([FromQuery] int count, [FromBody] TemperatureRequest request)
    {
        if(count < 0 || request.Max < request.Min)
        {
            return BadRequest();
        }
        var result = _service.Get(request.Min, request.Max, count);
        return Ok(result);
    }
}
