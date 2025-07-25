namespace GamersHub.Models
{
    public class Genre
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public ICollection<Game>? Games { get; set; }
    }
}
