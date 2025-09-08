namespace Blog_Site.Models.Concrete
{
    public class Comment
    {
        public int Id { get; set; }
        public string? AuthorName { get; set; }
        public string? Content { get; set; }
        public DateTime PostedDate { get; set; } = DateTime.Now;
        // Foreign key
        public int PostId { get; set; }
        // Navigation property
        public Post? Post { get; set; }
    }
}
