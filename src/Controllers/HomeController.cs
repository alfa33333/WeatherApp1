using System.Diagnostics;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using WeatherApp.Models;

namespace WeatherApp.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;

    public HomeController(
        ILogger<HomeController> logger,
        HttpClient httpClient,
        IConfiguration configuration
    )
    {
        _logger = logger;
        _httpClient = httpClient;
        _configuration = configuration;
    }

    public IActionResult Index()
    {
        return View();
    }

    public async Task<IActionResult> GetWeather()
    {
        string? baseUrl = _configuration["WeatherApi:BaseUrl"];
        string? apiKey = _configuration["WeatherApi:ApiKey"];
        string? location = _configuration["WeatherApi:Location"];
        string apiUrl = $"{baseUrl}?key={apiKey}&q={location}";
        var response = await _httpClient.GetAsync(apiUrl);
        if (response.IsSuccessStatusCode)
        {
            var weatherData = await response.Content.ReadAsStringAsync();
            using var jsonDoc = JsonDocument.Parse(weatherData);
            // Deserialize JSON to WeatherModel
            var currentWeather = jsonDoc.RootElement.GetProperty("current");
            WeatherModel currentWeatherModel = new WeatherModel
            {
                location = jsonDoc
                    .RootElement.GetProperty("location")
                    .GetProperty("name")
                    .GetString(), // Extract "name" property
                temperature = currentWeather.GetProperty("temp_c").GetSingle(), // Extract "temp_c" property
                condition = currentWeather.GetProperty("condition").GetProperty("text").GetString(),
                icon =
                    $"http:{currentWeather.GetProperty("condition").GetProperty("icon").GetString()}",
            };
            // return Content(
            //     $"Location: {currentWeatherModel.location} Current temperature: {currentWeatherModel.temperature}°C"
            // ); // For demonstration, return raw string
            return View(currentWeatherModel); // Return the view with the weather data
        }

        return StatusCode((int)response.StatusCode, "Error fetching weather data");
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(
            new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier }
        );
    }
}
