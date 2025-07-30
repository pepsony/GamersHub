namespace GamersHub.Models
{
    public class Game
    {
        public int? Id { get; set; }
        public string? Title {  get; set; }
        public DateTime? ReleaseDate { get; set; }

        public string? ImagePath { get; set; }

        public int? GenreId { get; set; }
        public Genre? Genre { get; set; }

        public ICollection<GamePlatform>? GamePlatforms { get; set; }

        public ICollection<Review>? Reviews { get; set; }
    }
}
