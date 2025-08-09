using Microsoft.AspNetCore.Identity;

namespace GamersHub.Models
{
    public class ApplicationUser : IdentityUser
    {
        public ICollection<UserGame> UserGames { get; set; }

        public ICollection<Review> Reviews { get; set; }


        // Optional for navigation (additional feature)
        public ICollection<Follower> Followers { get; set; }  // People who follow me
        public ICollection<Follower> Following { get; set; }  // People I follow
    }


}
