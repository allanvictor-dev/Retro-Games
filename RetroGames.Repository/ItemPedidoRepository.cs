using MySqlConnector;
using RetroGames.Model.Model;

namespace RetroGames.Repository;

public class ItemPedidoRepository : RepositoryBase, IItemPedidoRepository
{
    public ItemPedidoRepository(RepositoryConfig config)
        : base(config)
    {
    }

    private static ItemPedido LerItemPedido(MySqlDataReader leitor)
    {
        return new ItemPedido
        {
            Id = leitor.GetInt32(0),
            Quantidade = leitor.GetInt32(1),
            ValorUnitario = leitor.GetDecimal(2),
            IdPedido = leitor.GetInt32(3),
            IdJogo = leitor.GetInt32(4)
        };
    }

    public async Task<int> InserirAsync(ItemPedido item)
    {
        const string sql = """
            INSERT INTO tb_item_pedido
            (qt_item, vl_unitario, co_pedido, co_jogo)
            VALUES (@quantidade, @valorUnitario, @idPedido, @idJogo)
            """;

        await using var conexao = await AbrirConexaoAsync();
        await using var comando = CriarComando(conexao, sql);

        AdicionarParametro(comando, "@quantidade", item.Quantidade);
        AdicionarParametro(comando, "@valorUnitario", item.ValorUnitario);
        AdicionarParametro(comando, "@idPedido", item.IdPedido);
        AdicionarParametro(comando, "@idJogo", item.IdJogo);

        await ExecutarGravacaoAsync(comando);

        return checked((int)comando.LastInsertedId);
    }

    public async Task<List<ItemPedido>> ListarPorPedidoAsync(int idPedido)
    {
        const string sql = """
            SELECT co_seq_item_pedido, qt_item, vl_unitario,
                   co_pedido, co_jogo
            FROM tb_item_pedido
            WHERE co_pedido = @idPedido
            ORDER BY co_seq_item_pedido
            """;

        var itens = new List<ItemPedido>();

        await using var conexao = await AbrirConexaoAsync();
        await using var comando = CriarComando(conexao, sql);

        AdicionarParametro(comando, "@idPedido", idPedido);

        await using var leitor = await comando.ExecuteReaderAsync();

        while (await leitor.ReadAsync())
            itens.Add(LerItemPedido(leitor));

        return itens;
    }

    public async Task<List<ItemPedido>> ListarPorPedidosAsync(List<int> idsPedidos)
    {
        var itens = new List<ItemPedido>();

        if (idsPedidos.Count == 0)
            return itens;

        var nomesParametros = idsPedidos.Select((_, i) => $"@id{i}");

        string sql = $"""
            SELECT co_seq_item_pedido, qt_item, vl_unitario,
                   co_pedido, co_jogo
            FROM tb_item_pedido
            WHERE co_pedido IN ({string.Join(", ", nomesParametros)})
            ORDER BY co_pedido, co_seq_item_pedido
            """;

        await using var conexao = await AbrirConexaoAsync();
        await using var comando = CriarComando(conexao, sql);

        for (int i = 0; i < idsPedidos.Count; i++)
            AdicionarParametro(comando, $"@id{i}", idsPedidos[i]);

        await using var leitor = await comando.ExecuteReaderAsync();

        while (await leitor.ReadAsync())
            itens.Add(LerItemPedido(leitor));

        return itens;
    }
}
