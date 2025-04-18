using System.Diagnostics;
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
            return Content(weatherData); // For demonstration, return raw JSON
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
