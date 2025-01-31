using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

public class WeatherManager
{

    private const string ApiKey = "6689d43dbd8891a25a6f652e7e9908be";
    private const string GeocodeUrl = "http://api.openweathermap.org/geo/1.0/direct?q={0}&limit=1&appid=" + ApiKey;
    private const string OneCallUrl = "https://api.openweathermap.org/data/3.0/onecall/timemachine?lat={0}&lon={1}&dt={2}&appid=" + ApiKey;

    private readonly HttpClient _httpClient;

    public WeatherManager(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<WeatherComparisonResult> CompareWeather(string cityA, string cityB, int dayCount)
    {
        if (dayCount < 1 || dayCount > 5)
            throw new ArgumentException("Day count must be between 1 and 5.");

        var coordsA = await GetCityCoordinates(cityA);
        var coordsB = await GetCityCoordinates(cityB);

        var now = DateTimeOffset.UtcNow;
        var timestamps = Enumerable.Range(0, dayCount)
            .Select(d => now.AddDays(-d).ToUnixTimeSeconds()).ToList();

        var weatherA = await GetWeatherData(coordsA, timestamps);
        var weatherB = await GetWeatherData(coordsB, timestamps);

        int warmerDays = 0, rainierDays = 0;
        for (int i = 0; i < dayCount; i++)
        {
            var avgTempA = weatherA[i].Average(t => t.Temp);
            var avgTempB = weatherB[i].Average(t => t.Temp);
            if (avgTempA > avgTempB) warmerDays++;

            var totalRainA = weatherA[i].Sum(t => t.Rain);
            var totalRainB = weatherB[i].Sum(t => t.Rain);
            if (totalRainA > totalRainB) rainierDays++;
        }

        return new WeatherComparisonResult(cityA, cityB, warmerDays, rainierDays);
    }

    private async Task<(double lat, double lon)> GetCityCoordinates(string city)
    {
        var response = await _httpClient.GetAsync(string.Format(GeocodeUrl, city));
        if (!response.IsSuccessStatusCode)
            throw new Exception("Error fetching coordinates");

        var responseBody = await response.Content.ReadAsStringAsync();
        var locations = JsonSerializer.Deserialize<List<GeocodeResponse>>(responseBody);
        if (locations == null || locations.Count == 0)
            throw new ArgumentException("Invalid city name");

        return (locations[0].Lat, locations[0].Lon);
    }

    private async Task<List<List<HourlyWeather>>> GetWeatherData((double lat, double lon) coords, List<long> timestamps)
    {
        var results = new List<List<HourlyWeather>>();
        Console.WriteLine(OneCallUrl);
        foreach (var timestamp in timestamps)
        {
            var response = await _httpClient.GetAsync(string.Format(OneCallUrl, coords.lat, coords.lon, timestamp));
            if (!response.IsSuccessStatusCode)
                throw new Exception("Error fetching weather data");

            var responseBody = await response.Content.ReadAsStringAsync();
            var weatherData = JsonSerializer.Deserialize<OneCallResponse>(responseBody);
            if (weatherData?.Hourly == null)
                throw new Exception("Failed to fetch weather data");

            results.Add(weatherData.Hourly);
        }
        
        return results;
    }
}

public class WeatherComparisonResult
{
    public string CityA { get; }
    public string CityB { get; }
    public int WarmerDaysCount { get; }
    public int RainierDaysCount { get; }

    public WeatherComparisonResult(string cityA, string cityB, int warmerDays, int rainierDays)
    {
        CityA = cityA;
        CityB = cityB;
        WarmerDaysCount = warmerDays;
        RainierDaysCount = rainierDays;
    }
}

public class GeocodeResponse
{
    public double Lat { get; set; }
    public double Lon { get; set; }
}

public class OneCallResponse
{
    public List<HourlyWeather> Hourly { get; set; }
}

public class HourlyWeather
{
    public double Temp { get; set; }
    public double Rain { get; set; }
}
