// BlogProject.Service/FluentValidations/CommentValidator.cs
using FluentValidation;
using BlogProject.Entity.Entities;

namespace BlogProject.Service.FluentValidations
{
    public class CommentValidator : AbstractValidator<Comment>
    {
        public CommentValidator()
        {
            RuleFor(c => c.Text)
                .NotEmpty()
                .NotNull()
                .MinimumLength(3)
                .MaximumLength(1000)
                .WithName("Yorum");
        }
    }
}