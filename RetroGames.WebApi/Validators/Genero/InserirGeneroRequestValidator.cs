using FluentValidation;
using RetroGames.WebApi.ApiModels;

namespace RetroGames.WebApi.Validators.Genero;

public class InserirGeneroRequestValidator : AbstractValidator<InserirGeneroRequestApiModel>
{
    public InserirGeneroRequestValidator()
    {
        RuleFor(x => x.Nome).NotEmpty().MaximumLength(80);
    }
}
