// Services/FootballApiService.cs
using BetsiApp.Models;
using System.Text.Json;

namespace BetsiApp.Services
{
    public class FootballApiService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        private readonly string _baseUri = "https://api.football-data.org/v4/";
        // OPOMBA: Namesto Football-Data.org vstavite URL kateregakoli brezplačnega API-ja, ki ga izberete.

        // Konstruktor prejme HttpClient in konfiguracijo preko Dependency Injection
        public FootballApiService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            // PREVERI: Ključ je prebran iz appsettings.json, odsek "ApiKeys:FootballApi"
            _apiKey = configuration["ApiKeys:FootballApi"] ?? throw new InvalidOperationException("API Key ni konfiguriran.");
            
            _httpClient.BaseAddress = new Uri(_baseUri);
            // Dodajanje API ključa v header (Zahtevano pri večini API-jev za avtentikacijo)
            _httpClient.DefaultRequestHeaders.Add("X-Auth-Token", _apiKey);
        }

        public async Task<List<Match>> GetUpcomingMatchesAsync() 
        {
            // 1. Nastavimo filter za tekme V NASLEDNJIH 7 DNEH
            var dateFrom = DateTime.Today.ToString("yyyy-MM-dd");
            var dateTo = DateTime.Today.AddDays(7).ToString("yyyy-MM-dd");

            // 2. Definiramo URI za pridobivanje vseh tekem v tem obdobju
            // PREVERITE: Tukaj je requestUri definirana samo ENKRAT.
            string requestUri = $"matches?dateFrom={dateFrom}&dateTo={dateTo}"; 

            try
            {
                var response = await _httpClient.GetAsync(requestUri);

                // ... ostala koda ostane enaka (EnsureSuccessStatusCode, Deserialize, itd.)
                response.EnsureSuccessStatusCode(); 

                var jsonString = await response.Content.ReadAsStringAsync();

                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

                var apiResponse = JsonSerializer.Deserialize<ApiRootResponse>(jsonString, options);

                return apiResponse?.Matches ?? new List<Match>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Napaka pri pridobivanju podatkov: {ex.Message}");
                return new List<Match>();
            }
        }
        public async Task<List<Match>> GetMatchesByDateRangeAsync(DateTime from, DateTime to)
        {
            var dateFrom = from.ToString("yyyy-MM-dd");
            var dateTo = to.ToString("yyyy-MM-dd");
            string requestUri = $"matches?dateFrom={dateFrom}&dateTo={dateTo}";

            try
            {
                var response = await _httpClient.GetAsync(requestUri);
                response.EnsureSuccessStatusCode();
                var jsonString = await response.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var apiResponse = JsonSerializer.Deserialize<ApiRootResponse>(jsonString, options);
                return apiResponse?.Matches ?? new List<Match>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Napaka pri pridobivanju rezultatov: {ex.Message}");
                return new List<Match>();
            }
        }
    }
    
    
}