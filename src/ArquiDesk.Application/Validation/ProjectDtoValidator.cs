using ArquiDesk.Application.DTOs;
using FluentValidation;

namespace ArquiDesk.Application.Validation;

public class ProjectDtoValidator : AbstractValidator<ProjectDto>
{
    public ProjectDtoValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(160);
        RuleFor(x => x.ClientId).NotEmpty();
        RuleFor(x => x.Address).NotEmpty().MaximumLength(240);
        RuleFor(x => x.ExpectedDeliveryDate)
            .GreaterThanOrEqualTo(x => x.StartDate)
            .WithMessage("A entrega prevista deve ser maior ou igual a data de inicio.");
        RuleFor(x => x.ResponsibleUserId).NotEmpty();
    }
}
