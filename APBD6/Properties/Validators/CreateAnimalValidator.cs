using APBD6.Properties.DTOs;
using FluentValidation;
    
namespace APBD6.Properties.Validators;

public class CreateAnimalValidator : AbstractValidator<CreateAnimalRequest>
{
    public CreateAnimalValidator()
    {
        RuleFor(e => e.name).MaximumLength(50).NotNull();
        RuleFor(e => e.description).MaximumLength(200);
        RuleFor(e => e.category).MaximumLength(50).NotNull();
        RuleFor(e => e.area).MaximumLength(50).NotNull();
    }
}