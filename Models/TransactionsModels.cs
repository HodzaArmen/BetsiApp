using System;

namespace BetsiApp.Models
{
    public class Transaction
    {
        public int Id { get; set; }
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
        public string Type { get; set; }   // "Deposit" ali "Withdraw"
        public string Description { get; set; }

        // povezava na uporabnika
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
    }
}