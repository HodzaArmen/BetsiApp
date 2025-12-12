// Models/BettingModels.cs

using System.ComponentModel.DataAnnotations.Schema;

namespace BetsiApp.Models
{
    // --- 1. Kvota (Odd) ---
    // Kvota je povezana z eno tekmo (Match) in ponuja razmerja.
    public class Odd
    {
        public int Id { get; set; }
        
        // MatchId poveže kvote z določeno tekmo, pridobljeno iz API-ja.
        public int MatchId { get; set; } 
        // Opomba: Ker tekme ne shranjujemo v bazo, bo to samo Id iz zunanjega API-ja.
        
        // Razmerje za zmago domače ekipe (1)
        [Column(TypeName = "decimal(5, 2)")]
        public decimal HomeWin { get; set; } 
        
        // Razmerje za neodločen rezultat (X)
        [Column(TypeName = "decimal(5, 2)")]
        public decimal Draw { get; set; }
        
        // Razmerje za zmago gostujoče ekipe (2)
        [Column(TypeName = "decimal(5, 2)")]
        public decimal AwayWin { get; set; }

        // Dodate lahko še tip kvote (npr. 'Končni rezultat', 'Gol prednost' itd.)
        public string? MarketType { get; set; } = "FullTimeResult";
        
        // Datum, kdaj so bile kvote nazadnje posodobljene
        public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
    }

    // --- 2. Postavka Stave (BetItem) ---
    // Vsak posamezen izbor (npr. 'Stavim na zmago Mure' s kvoto 2.50)
    public class BetItem
    {
        public int Id { get; set; }
        
        public int MatchId { get; set; } 
        public string? MatchDescription { get; set; } // Npr. "Mura vs Olimpija"
        
        // Izbrani rezultat (npr. '1', 'X', '2')
        public string? SelectedOutcome { get; set; } 
        
        // Kvota, po kateri je bila stava sprejeta (ključen podatek!)
        [Column(TypeName = "decimal(5, 2)")]
        public decimal PlacedOdd { get; set; }
        
        // Povezava na Stavni listič
        public int BetSlipId { get; set; }
        public BetSlip? BetSlip { get; set; } // Navigacijska lastnost
    }

    // --- 3. Stavni Listič (BetSlip) ---
    // Glavni dokument, ki ga uporabnik odda (Lahko vsebuje eno ali več BetItem-ov)
    public class BetSlip
    {
        public int Id { get; set; }
        
        // Povezava na uporabnika, ki je stavo oddal (UserID je string v Identity)
        public string? UserId { get; set; } 
        
        // Znesek, ki ga uporabnik stavi
        [Column(TypeName = "decimal(10, 2)")]
        public decimal Stake { get; set; } 
        
        // Skupna kvota lističa (produkt vseh PlacedOdd)
        [Column(TypeName = "decimal(10, 2)")]
        public decimal TotalOdd { get; set; }
        
        // Datum in čas oddaje
        public DateTime PlacementTime { get; set; } = DateTime.UtcNow;

        // Status (OPEN, WON, LOST, CANCELLED)
        public string? Status { get; set; } = "OPEN";

        // Kolekcija vseh izbir na tem lističu
        public ICollection<BetItem> BetItems { get; set; } = new List<BetItem>();
    }
}