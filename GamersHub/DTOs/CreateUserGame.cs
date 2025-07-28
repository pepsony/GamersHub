using GamersHub.Models;

namespace GamersHub.DTOs
{
    public class CreateUserGame
    {        
        public int GameId { get; set; }
        
        public string Status { get; set; }  // owned, wishlist, completed
    }
}
