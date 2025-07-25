namespace GamersHub.Models
{
    public class Review
    {
        public int Id { get; set; }

        public string? UserId { get; set; }
        public ApplicationUser? User { get; set; }
        public int GameId { get; set; }
        public Game? Game { get; set; }

        public string? Content { get; set; }
        public int Rating { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
