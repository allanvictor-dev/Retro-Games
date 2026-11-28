using FluentValidation;
using RetroGames.WebApi.ApiModels;

namespace RetroGames.WebApi.Validators.Autenticacao;

public class CadastrarUsuarioRequestValidator : AbstractValidator<CadastrarUsuarioRequestApiModel>
{
    public CadastrarUsuarioRequestValidator()
    {
        RuleFor(x => x.Nome).NotEmpty().MaximumLength(120);
        RuleFor(x => x.Email).NotEmpty().EmailAddress().MaximumLength(255);
        RuleFor(x => x.Senha).NotEmpty().MinimumLength(6).MaximumLength(100);
    }
}
