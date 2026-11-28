using System.Net;

namespace RetroGames.Services;

public static class BusinessErrors
{
    public static readonly BusinessError PlataformaNaoEncontrada =
     new(100, "Plataforma não encontrada.", HttpStatusCode.NotFound);

    public static readonly BusinessError PlataformaNomeDuplicado =
     new(101, "Já existe uma plataforma com este nome.", HttpStatusCode.Conflict);

    public static readonly BusinessError PlataformaPossuiJogos =
     new(102, "Não é possível excluir: existem jogos desta plataforma.", HttpStatusCode.Conflict);

    public static readonly BusinessError JogoNaoEncontrado =
     new(103, "Jogo não encontrado.", HttpStatusCode.NotFound);

    public static readonly BusinessError CondicaoNaoEncontrada =
     new(104, "Condição de jogo não encontrada.", HttpStatusCode.NotFound);

    public static readonly BusinessError GeneroNaoEncontrado =
     new(105, "Gênero não encontrado.", HttpStatusCode.NotFound);

    public static readonly BusinessError EmailJaCadastrado =
     new(106, "Já existe uma conta com este e-mail.", HttpStatusCode.Conflict);

    public static readonly BusinessError GeneroNomeDuplicado =
     new(107, "Já existe um gênero com este nome.", HttpStatusCode.Conflict);

    public static readonly BusinessError GeneroPossuiJogos =
     new(108, "Não é possível excluir: existem jogos com este gênero.", HttpStatusCode.Conflict);

    public static readonly BusinessError PedidoNaoEncontrado =
     new(109, "Pedido não encontrado.", HttpStatusCode.NotFound);

    public static readonly BusinessError JogoIndisponivel =
     new(110, "Um ou mais jogos não estão disponíveis para venda.", HttpStatusCode.BadRequest);

    public static readonly BusinessError EstoqueInsuficiente =
     new(111, "Estoque insuficiente.", HttpStatusCode.Conflict);

    public static readonly BusinessError SituacaoNaoEncontrada =
     new(112, "Situação de pedido não encontrada.", HttpStatusCode.NotFound);

    public static readonly BusinessError PedidoDeOutroUsuario =
     new(113, "Você não tem acesso a este pedido.", HttpStatusCode.Forbidden);
}