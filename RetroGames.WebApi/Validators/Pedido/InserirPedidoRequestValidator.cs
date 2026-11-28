using FluentValidation;
using RetroGames.WebApi.ApiModels;

namespace RetroGames.WebApi.Validators.Pedido;

public class InserirPedidoRequestValidator : AbstractValidator<InserirPedidoRequestApiModel>
{
    public InserirPedidoRequestValidator()
    {
        RuleFor(x => x.Itens)
            .NotEmpty()
            .WithMessage("O pedido deve conter ao menos um item.");

        RuleFor(x => x.Itens)
            .Must(itens => itens == null || itens.Select(i => i.IdJogo).Distinct().Count() == itens.Count)
            .WithMessage("Não é permitido repetir o mesmo jogo no pedido.");

        RuleForEach(x => x.Itens).ChildRules(item =>
        {
            item.RuleFor(i => i.IdJogo).GreaterThan(0);
            item.RuleFor(i => i.Quantidade).InclusiveBetween(1, 10);
        });
    }
}
