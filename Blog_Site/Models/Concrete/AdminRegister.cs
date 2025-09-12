using Blog_Site.Models.Abstract;

namespace Blog_Site.Models.Concrete
{
    public class AdminRegister : IRegister
    {
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
        public string? ConfirmPassword { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
