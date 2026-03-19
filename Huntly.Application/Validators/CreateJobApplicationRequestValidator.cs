using Huntly.Application.DTOs.JobApplication;
using FluentValidation;

namespace Huntly.Application.Validators
{
    public class CreateJobApplicationRequestValidator : AbstractValidator<CreateJobApplicationRequest>
    {
        public CreateJobApplicationRequestValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Назва позиції обов'язкова")
                .MaximumLength(200).WithMessage("Назва не може бути довшою за 200 символів");

            RuleFor(x => x.CompanyId)
                .NotEmpty().WithMessage("Компанія обов'язкова");

            RuleFor(x => x.SalaryFrom)
                .GreaterThan(0).WithMessage("Зарплата має бути більше 0")
                .When(x => x.SalaryFrom.HasValue);

            RuleFor(x => x.SalaryTo)
                .GreaterThan(x => x.SalaryFrom ?? 0)
                .WithMessage("Максимальна зарплата має бути більше мінімальної")
                .When(x => x.SalaryTo.HasValue && x.SalaryFrom.HasValue);
        }
    }
}
