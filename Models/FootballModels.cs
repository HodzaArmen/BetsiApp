// Models/FootballModels.cs
using System.Text.Json.Serialization;

namespace BetsiApp.Models
{
    // Glavni Response objekt, ki ga vrne API
    public class ApiRootResponse
    {
        [JsonPropertyName("matches")]
        public List<Match> Matches { get; set; } = new List<Match>();
    }

    // Podatki o posamezni tekmi
    public class Match
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("utcDate")]
        public DateTime UtcDate { get; set; }

        [JsonPropertyName("status")]
        public string? Status { get; set; } 

        // NOVE LASTNOSTI:
        [JsonPropertyName("matchday")]
        public int? Matchday { get; set; } // Katera runda tekmovanja je

        [JsonPropertyName("competition")]
        public Competition? Competition { get; set; } // Informacije o ligi

        [JsonPropertyName("venue")]
        public string? Venue { get; set; } // Ime stadiona (Pri Football-Data.org je to včasih kar string)
        // Konec NOVIH LASTNOSTI

        [JsonPropertyName("homeTeam")]
        public Team? HomeTeam { get; set; }

        [JsonPropertyName("awayTeam")]
        public Team? AwayTeam { get; set; }

        [JsonPropertyName("score")]
        public Score? Score { get; set; }
    }

    // Podatki o ekipi
    public class Team
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }
    }

    // Podatki o rezultatu
    public class Score
    {
        [JsonPropertyName("fullTime")]
        public Goals FullTime { get; set; }
    }

    // Končni rezultat
    public class Goals
    {
        [JsonPropertyName("home")]
        public int? Home { get; set; } // Nullable int - če ni rezultata
        
        [JsonPropertyName("away")]
        public int? Away { get; set; } // Nullable int - če ni rezultata
    }

    public class Competition
    {
        [JsonPropertyName("name")]
        public string? Name { get; set; }
        
        [JsonPropertyName("code")]
        public string? Code { get; set; }
    }

    public class Venue
    {
        [JsonPropertyName("name")]
        public string? Name { get; set; }
    }
}