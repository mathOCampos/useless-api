using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace WeatherAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WeatherController : ControllerBase
    {
        private readonly HttpClient _httpClient;

        // Constructor to inject HttpClient
        public WeatherController(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        [HttpGet]
        public async Task<IActionResult> GetWeather()
        {
            var apiUrl = "https://api.open-meteo.com/v1/forecast?latitude=-23.9608&longitude=-46.3336&hourly=temperature_2m&timezone=America%2FSao_Paulo&forecast_days=1";

            try
            {
                // Call the weather API
                var response = await _httpClient.GetAsync(apiUrl);

                // Check for errors in the API response
                if (!response.IsSuccessStatusCode)
                {
                    return BadRequest($"Error fetching weather data: {response.StatusCode} - {response.ReasonPhrase}");
                }

                var content = await response.Content.ReadAsStringAsync();

                // Parse the JSON response using JObject
                var jsonResponse = JObject.Parse(content);

                // Extract 'time' and 'temperature_2m' values from the JSON
                var times = jsonResponse["hourly"]?["time"]?.ToObject<List<string>>();
                var temperatures = jsonResponse["hourly"]?["temperature_2m"]?.ToObject<List<double>>();

                if (times == null || temperatures == null || times.Count != temperatures.Count)
                {
                    return BadRequest("Data mismatch or missing values in the API response.");
                }

                // Combine the time and temperature values into a list of objects
                var result = new List<object>();
                for (int i = 0; i < times.Count; i++)
                {
                    result.Add(new
                    {
                        Time = times[i],
                        Temperature = temperatures[i]
                    });
                }

                // Return the combined result
                return Ok(result);
            }
            catch (HttpRequestException ex)
            {
                return StatusCode(500, $"Request failed: {ex.Message}");
            }
        }

    }
}
