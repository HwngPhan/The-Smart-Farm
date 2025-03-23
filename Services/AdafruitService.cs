using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using TSF_mustidisProj.Models; 


namespace TSF_mustidisProj.Services  // Ensure this matches your project namespace
{
    

    public class AdafruitService
    {
        private readonly HttpClient _httpClient;
        private const string ApiUrl = "https://io.adafruit.com/api/v2/Hwng/feeds";
        private readonly string _apiKey;

        public AdafruitService(HttpClient httpClient)
        {
            _httpClient = httpClient;   
            //_httpClient.DefaultRequestHeaders.Add("X-AIO-Key", ApiKey);
            Console.Write("Enter your Adafruit IO API Key: ");
            _apiKey = Console.ReadLine()?.Trim();

            if (string.IsNullOrEmpty(_apiKey))
            {
                throw new Exception("API Key cannot be empty.");
            }
            _httpClient.DefaultRequestHeaders.Add("X-AIO-Key", _apiKey);
        }

        public async Task<List<FeedModel>> GetFeedsAsync()
        {
            var response = await _httpClient.GetAsync(ApiUrl);
            if (!response.IsSuccessStatusCode)
                throw new Exception("Failed to fetch data");

            var json = await response.Content.ReadAsStringAsync();
            //return JsonSerializer.Deserialize<List<FeedModel>>(json);
            return JsonSerializer.Deserialize<List<FeedModel>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        
        }
    }
}
