using ArquiDesk.Application.DTOs;
using FluentValidation;

namespace ArquiDesk.Application.Validation;

public class TicketDtoValidator : AbstractValidator<TicketDto>
{
    public TicketDtoValidator()
    {
        RuleFor(x => x.ProjectId).NotEmpty();
        RuleFor(x => x.Type).IsInEnum();
        RuleFor(x => x.Priority).IsInEnum();
        RuleFor(x => x.Description).NotEmpty().MinimumLength(10).MaximumLength(4000);
    }
}
