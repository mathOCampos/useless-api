using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace WeatherAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly HttpClient _httpClient;

        // Inject HttpClient via constructor
        public UserController(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        // GET api/user
        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            // The URL to the Random User Generator API
            var apiUrl = "https://randomuser.me/api/?results=20";  // You can adjust the 'results' query parameter

            try
            {
                // Send GET request to the Random User API
                var response = await _httpClient.GetAsync(apiUrl);
                
                if (!response.IsSuccessStatusCode)
                {
                    return BadRequest($"Error fetching user data: {response.StatusCode} - {response.ReasonPhrase}");
                }

                var content = await response.Content.ReadAsStringAsync();

                // Parse the JSON response using JObject
                var jsonResponse = JObject.Parse(content);

                // Extract 'results' values from the JSON
                var results = jsonResponse["results"]?.ToObject<List<JObject>>();

                if (results == null)
                {
                    return BadRequest("Data mismatch or missing values in the API response.");
                }

                var result = new List<object>();
                foreach (var user in results)
                {
                    var name = user["name"]?["title"]?.ToString() + " " + user["name"]?["first"]?.ToString() + " " + user["name"]?["last"]?.ToString();
                    var email = user["email"]?.ToString();
                    var country = user["location"]?["country"]?.ToString();

                    if (name == null || email == null || country == null)
                    {
                        return BadRequest("Missing necessary user information.");
                    }

                    result.Add(new
                    {
                        Name = name,
                        Email = email,
                        Country = country
                    });
                }

                return Ok(result);
            }
            catch (HttpRequestException ex)
            {
                // Handle errors if the request fails
                return BadRequest($"Error retrieving data: {ex.Message}");
            }
        }
    }
}
