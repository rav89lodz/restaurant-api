using System;
using System.Collections.Generic;
using System.Linq;

namespace RestaurantAPI.WeatherForecastItems;

public interface IWeatherForecastService
{
    IEnumerable<WeatherForecast> Get(int tempMin, int tempMax, int count);
}

public class WeatherForecastService : IWeatherForecastService
{
    private static readonly string[] Summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    public IEnumerable<WeatherForecast> Get(int tempMin, int tempMax, int count)
    {
        return Enumerable.Range(1, count).Select(index => new WeatherForecast
        {
            Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            TemperatureC = Random.Shared.Next(tempMin, tempMax),
            Summary = Summaries[Random.Shared.Next(Summaries.Length)]
        })
        .ToArray();
    }
}