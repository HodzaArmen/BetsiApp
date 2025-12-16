using System.Text.Json.Serialization;

namespace BetsiApp.Models
{
    public class ApiSportsBasketballResponse
    {
        [JsonPropertyName("response")]
        public List<BasketballGame> Response { get; set; } = new();
    }

    public class BasketballGame
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("date")]
        public required DateTime Date { get; set; }

        [JsonPropertyName("league")]
        public required LeagueInfo League { get; set; }

        [JsonPropertyName("teams")]
        public required BasketballTeams Teams { get; set; }

        [JsonPropertyName("scores")]
        public required BasketballScores Scores { get; set; }
    }

    public class LeagueInfo
    {
        [JsonPropertyName("name")]
        public required string Name { get; set; }
    }

    public class BasketballTeams
    {
        [JsonPropertyName("home")]
        public required TeamName Home { get; set; }

        [JsonPropertyName("away")]
        public required TeamName Away { get; set; }
    }

    public class TeamName
    {
        [JsonPropertyName("name")]
        public required string Name { get; set; }
    }

    public class BasketballScores
    {
        [JsonPropertyName("home")]
        public required ScoreValue Home { get; set; }

        [JsonPropertyName("away")]
        public required ScoreValue Away { get; set; }
    }

    public class ScoreValue
    {
        [JsonPropertyName("total")]
        public int? Total { get; set; }
    }
}
