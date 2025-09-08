namespace Blog_Site.Models.Concrete
{
    public class Post
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? Content { get; set; }
        public DateTime PublishedDate { get; set; } = DateTime.Now;
        // Navigation properties
        public List<Comment>? Comments { get; set; } = new List<Comment>();
        public List<Tag>? Tags { get; set; } = new List<Tag>();
    }
}
