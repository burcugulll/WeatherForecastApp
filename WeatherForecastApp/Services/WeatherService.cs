using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using WeatherForecastApp.Models;
using Microsoft.Extensions.Configuration;

namespace WeatherForecastApp.Services
{
    public class WeatherService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey; 


        public WeatherService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _apiKey = configuration["WeatherApi:ApiKey"]; 

        }

        public async Task<WeatherInfo> GetWeatherFromApi(string cityName, DateTime date)
        {
            
            var requestUrl = $"https://api.openweathermap.org/data/2.5/weather?q={cityName}&appid={_apiKey}&units=metric&lang=tr";

            var response = await _httpClient.GetAsync(requestUrl);
            if (response.IsSuccessStatusCode)
            {
                var weatherData = await response.Content.ReadFromJsonAsync<WeatherApiResponse>();

                var weatherInfo = new WeatherInfo
                {
                    WeatherDate = date,
                    CityName = cityName,
                    Temperature = weatherData.Main.Temp,
                    MainStatus = weatherData.Weather[0].Description,
                    Icon = $"http://openweathermap.org/img/wn/{weatherData.Weather[0].Icon}.png"

                };

                return weatherInfo;
            }

            return null; 
        }
    }

    
    public class WeatherApiResponse
    {
        public Main Main { get; set; }
        public Weather[] Weather { get; set; }
    }

    public class Main
    {
        public float Temp { get; set; }
    }

    public class Weather
    {
        public string Description { get; set; }
        public string Icon { get; set; }  // API'deki "icon" alanı

    }
}
