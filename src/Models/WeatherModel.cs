namespace WeatherApp.Models;

public record class WeatherModel
{
    public string? location { get; set; }
    public float temperature { get; set; }
    public string? condition { get; set; }
    public string? icon { get; set; }
}
