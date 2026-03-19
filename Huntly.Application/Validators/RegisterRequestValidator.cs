using Huntly.Application.DTOs.Auth;
using FluentValidation;

namespace Huntly.Application.Validators
{
    public class RegisterRequestValidator : AbstractValidator<RegisterRequest>
    {
        public RegisterRequestValidator()
        {
            RuleFor(x => x.FirstName)
                .NotEmpty().WithMessage("Ім'я обов'язкове")
                .MaximumLength(100).WithMessage("Ім'я не може бути довше 100 символів");

            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage("Прізвище обов'язкове")
                .MaximumLength(100).WithMessage("Прізвище не може бути довше за 100 символів");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email обов'язковий")
                .MaximumLength(256).WithMessage("Email не може бути довшим за 256 символів");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Пароль обов'язковий")
                .MinimumLength(6).WithMessage("Пароль має бути не менше 6 символів");
        }
    }
}
