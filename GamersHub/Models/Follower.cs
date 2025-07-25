namespace GamersHub.Models
{
    public class Follower
    {
        public int Id { get; set; }
        public string? FollowerId { get; set; }

        public ApplicationUser? FollowerUser { get; set; }

        public string? FollowingId { get; set; }
        public ApplicationUser? FollowingUser { get; set; }
    }
}
