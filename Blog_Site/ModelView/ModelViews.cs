using Blog_Site.Models.Concrete;

namespace Blog_Site.ModelView
{
    public class ModelViews
    {
        public Admin? Admin { get; set; }
        public User? User { get; set; }
        public Post? Post { get; set; }
        public Comment? Comment { get; set; }
        public Tag? Tag { get; set; }
        public PostTag? PostTag { get; set; }
        public UserRegister? UserRegister { get; set; }
        public AdminRegister? AdminRegister { get; set; }
    }
}
