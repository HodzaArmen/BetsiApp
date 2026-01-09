namespace BetsiApp.Models
{
    public class Notification
    {
        public int Id { get; set; }
        public string UserId { get; set; } // Povezava na uporabnika
        public string Message { get; set; } // Vsebina obvestila
        public DateTime CreatedAt { get; set; } // Čas ustvarjanja
        public bool IsRead { get; set; } // Ali je obvestilo prebrano
    }
}