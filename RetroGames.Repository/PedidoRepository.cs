using MySqlConnector;
using RetroGames.Model.Model;

namespace RetroGames.Repository;

public class PedidoRepository : RepositoryBase, IPedidoRepository
{
    public PedidoRepository(RepositoryConfig config)
        : base(config)
    {
    }

    private static Pedido LerPedido(MySqlDataReader leitor)
    {
        return new Pedido
        {
            Id = leitor.GetInt32(0),
            Data = leitor.GetDateTime(1),
            ValorTotal = leitor.GetDecimal(2),
            IdUsuario = leitor.GetInt32(3),
            IdSituacaoPedido = leitor.GetInt32(4)
        };
    }

    public async Task<int> InserirAsync(Pedido pedido)
    {
        const string sql = """
            INSERT INTO tb_pedido
            (dt_pedido, vl_total, co_usuario, co_situacao_pedido)
            VALUES (@data, @valorTotal, @idUsuario, @idSituacaoPedido)
            """;

        await using var conexao = await AbrirConexaoAsync();
        await using var comando = CriarComando(conexao, sql);

        AdicionarParametro(comando, "@data", pedido.Data);
        AdicionarParametro(comando, "@valorTotal", pedido.ValorTotal);
        AdicionarParametro(comando, "@idUsuario", pedido.IdUsuario);
        AdicionarParametro(comando, "@idSituacaoPedido", pedido.IdSituacaoPedido);

        await ExecutarGravacaoAsync(comando);

        return checked((int)comando.LastInsertedId);
    }

    public async Task<List<Pedido>> ListarPorUsuarioAsync(int idUsuario)
    {
        const string sql = """
            SELECT co_seq_pedido, dt_pedido, vl_total,
                   co_usuario, co_situacao_pedido
            FROM tb_pedido
            WHERE co_usuario = @idUsuario
            ORDER BY dt_pedido DESC, co_seq_pedido DESC
            """;

        var pedidos = new List<Pedido>();

        await using var conexao = await AbrirConexaoAsync();
        await using var comando = CriarComando(conexao, sql);

        AdicionarParametro(comando, "@idUsuario", idUsuario);

        await using var leitor = await comando.ExecuteReaderAsync();

        while (await leitor.ReadAsync())
            pedidos.Add(LerPedido(leitor));

        return pedidos;
    }

    public async Task<List<Pedido>> ListarTodosAsync(int? idSituacaoPedido = null)
    {
        const string sql = """
            SELECT co_seq_pedido, dt_pedido, vl_total,
                   co_usuario, co_situacao_pedido
            FROM tb_pedido
            WHERE (@idSituacao IS NULL OR co_situacao_pedido = @idSituacao)
            ORDER BY dt_pedido DESC, co_seq_pedido DESC
            """;

        var pedidos = new List<Pedido>();

        await using var conexao = await AbrirConexaoAsync();
        await using var comando = CriarComando(conexao, sql);

        AdicionarParametro(comando, "@idSituacao", idSituacaoPedido);

        await using var leitor = await comando.ExecuteReaderAsync();

        while (await leitor.ReadAsync())
            pedidos.Add(LerPedido(leitor));

        return pedidos;
    }

    public async Task<Pedido?> BuscarPorIdAsync(int id)
    {
        const string sql = """
            SELECT co_seq_pedido, dt_pedido, vl_total,
                   co_usuario, co_situacao_pedido
            FROM tb_pedido
            WHERE co_seq_pedido = @id
            """;

        await using var conexao = await AbrirConexaoAsync();
        await using var comando = CriarComando(conexao, sql);

        AdicionarParametro(comando, "@id", id);

        await using var leitor = await comando.ExecuteReaderAsync();

        if (await leitor.ReadAsync())
            return LerPedido(leitor);

        return null;
    }

    public async Task AtualizarSituacaoAsync(int id, int idSituacaoPedido)
    {
        const string sql = """
            UPDATE tb_pedido
            SET co_situacao_pedido = @idSituacaoPedido
            WHERE co_seq_pedido = @id
            """;

        await using var conexao = await AbrirConexaoAsync();
        await using var comando = CriarComando(conexao, sql);

        AdicionarParametro(comando, "@id", id);
        AdicionarParametro(comando, "@idSituacaoPedido", idSituacaoPedido);

        await ExecutarGravacaoAsync(comando);
    }
}
