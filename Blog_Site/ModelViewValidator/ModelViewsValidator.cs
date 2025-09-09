using Blog_Site.Models.Concrete;
using Blog_Site.ModelView;
using FluentValidation;

namespace Blog_Site.ModelViewValidator
{
    public class ModelViewsValidator : AbstractValidator<ModelViews>
    {
        public ModelViewsValidator()
        {
            RuleFor(a => a.Admin)
                .NotNull()
                .When(a => a.Admin != null);
            RuleFor(a => a.Admin.Name)
                .NotEmpty()
                .WithMessage("Admin adı boş olamaz!")
                .When(a => a.Admin != null);
            RuleFor(a => a.Admin.Email)
                .NotEmpty()
                .WithMessage("Admin email boş olamaz!")
                .EmailAddress()
                .WithMessage("Geçerli bir email adresi giriniz!")
                .When(a => a.Admin != null);

            RuleFor(u => u.User)
                .NotNull()
                .When(u => u.User != null);
            RuleFor(u => u.User.UserName)
                .NotEmpty()
                .WithMessage("Kullanıcı Adı boş bırakılamaz.")
                .MaximumLength(50)
                .WithMessage("Kullanıcı Adı en fazla 50 karakter olmalıdır.")
                .When(u => u.User != null);
            RuleFor(u => u.User.Email)
                .NotEmpty()
                .WithMessage("E-posta boş bırakılamaz.")
                .EmailAddress()
                .WithMessage("Geçerli bir E-posta adresi girin.")
                .When(u => u.User != null);
            RuleFor(u => u.User.PasswordHash)
                .NotEmpty()
                .WithMessage("Parola boş bırakılamaz.")
                .MinimumLength(6)
                .WithMessage("Parola en az 6 karakter olmalıdır.")
                .When(u => u.User != null);
            RuleFor(p => p.Post)
                .NotNull()
                .When(p => p.Post != null);
            RuleFor(p => p.Post.Title)
                .NotEmpty()
                .WithMessage("Başlık boş bırakılamaz.")
                .MaximumLength(200)
                .WithMessage("Başlık en fazla 200 karakter olmalıdır.")
                .When(p => p.Post != null);
            RuleFor(p => p.Post.Content)
                .NotEmpty()
                .WithMessage("İçerik boş bırakılamaz.")
                .When(p => p.Post != null);
            RuleFor(c => c.Comment)
                .NotNull()
                .When(c => c.Comment != null);
            RuleFor(c => c.Comment.AuthorName)
                .NotEmpty()
                .WithMessage("Yazar Adı boş bırakılamaz.")
                .When(c => c.Comment != null);
            RuleFor(c => c.Comment.Content)
                .NotEmpty()
                .WithMessage("Yorum içeriği boş bırakılamaz.")
                .When(c => c.Comment != null);
            RuleFor(t => t.Tag)
                .NotNull()
                .When(t => t.Tag != null);
            RuleFor(t => t.Tag.Name)
                .NotEmpty()
                .WithMessage("Etiket Adı boş bırakılamaz.")
                .MaximumLength(50)
                .WithMessage("Etiket Adı en fazla 50 karakter olmalıdır.")
                .When(t => t.Tag != null);
            RuleFor(r => r.Register.Name)
                .NotEmpty()
                .WithMessage("Kayıt adı boş bırakılamaz.");
            //diğer ruleları yaz register için
            //migration at
            //view sayfalarını oluştur.
        }
    }
}
