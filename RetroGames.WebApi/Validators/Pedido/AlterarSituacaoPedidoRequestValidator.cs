using FluentValidation;
using RetroGames.WebApi.ApiModels;

namespace RetroGames.WebApi.Validators.Pedido;

public class AlterarSituacaoPedidoRequestValidator : AbstractValidator<AlterarSituacaoPedidoRequestApiModel>
{
    public AlterarSituacaoPedidoRequestValidator()
    {
        RuleFor(x => x.IdSituacaoPedido).GreaterThan(0);
    }
}
