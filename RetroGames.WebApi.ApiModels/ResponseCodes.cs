namespace RetroGames.WebApi.ApiModels;

public static class ResponseCodes
{
    public const int SUCESSO = 0;
    public const int ERRO = 1;
    public static readonly (int Code, string DefaultMessage) MODEL_NULL =
        (2, "O modelo veio nulo.");
    public static readonly (int Code, string DefaultMessage) MODEL_STATE_INVALIDO =
        (3, "Há um erro com o modelo enviado.");
    public static readonly (int Code, string DefaultMessage) NAO_AUTENTICADO =
        (4, "Faça login para continuar.");
    public static readonly (int Code, string DefaultMessage) ACESSO_NEGADO =
        (5, "Você não tem permissão para esta operação.");
    public static readonly (int Code, string DefaultMessage) CREDENCIAIS_INVALIDAS =
        (6, "E-mail ou senha inválidos.");
    public static readonly (int Code, string DefaultMessage) EMAIL_JA_CADASTRADO =
        (106, "Já existe uma conta com este e-mail.");
    public static readonly (int Code, string DefaultMessage) GENERO_NOME_DUPLICADO =
        (107, "Já existe um gênero com este nome.");
    public static readonly (int Code, string DefaultMessage) GENERO_POSSUI_JOGOS =
        (108, "Não é possível excluir: existem jogos com este gênero.");
    public static readonly (int Code, string DefaultMessage) PEDIDO_NAO_ENCONTRADO =
        (109, "Pedido não encontrado.");
    public static readonly (int Code, string DefaultMessage) JOGO_INDISPONIVEL =
        (110, "Um ou mais jogos não estão disponíveis para venda.");
    public static readonly (int Code, string DefaultMessage) ESTOQUE_INSUFICIENTE =
        (111, "Estoque insuficiente.");
    public static readonly (int Code, string DefaultMessage) SITUACAO_NAO_ENCONTRADA =
        (112, "Situação de pedido não encontrada.");
    public static readonly (int Code, string DefaultMessage) PEDIDO_DE_OUTRO_USUARIO =
        (113, "Você não tem acesso a este pedido.");
}
