namespace GamersHub.Models
{
    public class UserGame
    {
        public int Id { get; set; }
        public  string UserId { get; set; }
        public virtual ApplicationUser? User { get; set; }

        public int GameId { get; set; }
        public virtual Game? Game { get; set; }

        public string Status { get; set; }  // owned, wishlist, completed
    }
}
