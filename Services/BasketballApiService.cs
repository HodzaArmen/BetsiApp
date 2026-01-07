using BetsiApp.Models;
using System.Text.Json;

namespace BetsiApp.Services
{
    public class BasketballApiService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;

        private readonly string _baseUri = "https://v1.basketball.api-sports.io/";

        public BasketballApiService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;

            _apiKey = configuration["ApiKeys:BasketballApi"]
                     ?? throw new InvalidOperationException("Basketball API Key ni konfiguriran.");

            _httpClient.BaseAddress = new Uri(_baseUri);

            // API-Sports zahteva točne headerje:
            _httpClient.DefaultRequestHeaders.Add("x-rapidapi-key", _apiKey);
            _httpClient.DefaultRequestHeaders.Add("x-rapidapi-host", "v1.basketball.api-sports.io");
        }

        public async Task<List<BasketballGame>> GetUpcomingGamesAsync()
        {
            var today = DateTime.Today;
            var games = new List<BasketballGame>();

            // pridobi tekme za naslednjih 7 dni
            for (int i = 0; i < 7; i++)
            {
                string date = today.AddDays(i).ToString("yyyy-MM-dd");

                string requestUri = $"games?date={date}&timezone=Europe/Ljubljana";

                try
                {
                    var response = await _httpClient.GetAsync(requestUri);
                    response.EnsureSuccessStatusCode();

                    string json = await response.Content.ReadAsStringAsync();

                    var result = JsonSerializer.Deserialize<ApiSportsBasketballResponse>(json, 
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                    if (result?.Response != null)
                    {
                        games.AddRange(result.Response);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Napaka pri pridobivanju košarkaških tekem: {ex.Message}");
                }
            }

            return games.OrderBy(g => g.Date).ToList();
        }
    }
}
