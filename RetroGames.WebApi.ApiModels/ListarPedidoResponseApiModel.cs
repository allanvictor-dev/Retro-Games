namespace RetroGames.WebApi.ApiModels;

public class ListarPedidoResponseApiModel
{
    public int Id { get; set; }
    public DateTime Data { get; set; }
    public decimal ValorTotal { get; set; }
    public int IdSituacaoPedido { get; set; }
    public string DescricaoSituacao { get; set; } = string.Empty;
    public int IdUsuario { get; set; }
    public string NomeUsuario { get; set; } = string.Empty;
    public string EmailUsuario { get; set; } = string.Empty;
    public List<ItemPedidoResponseApiModel> Itens { get; set; } = new();
}
