using RetroGames.Model.Model;

namespace RetroGames.Repository;

public class JogoGeneroRepository : RepositoryBase, IJogoGeneroRepository
{
    public JogoGeneroRepository(RepositoryConfig config)
        : base(config)
    {
    }

    public async Task<List<JogoGenero>> ListarTodosAsync()
    {
        const string sql = """
            SELECT co_jogo, co_genero
            FROM tb_jogo_genero
            ORDER BY co_jogo, co_genero
            """;

        var vinculos = new List<JogoGenero>();

        await using var conexao = await AbrirConexaoAsync();
        await using var comando = CriarComando(conexao, sql);
        await using var leitor = await comando.ExecuteReaderAsync();

        while (await leitor.ReadAsync())
        {
            vinculos.Add(new JogoGenero
            {
                IdJogo = leitor.GetInt32(0),
                IdGenero = leitor.GetInt32(1)
            });
        }

        return vinculos;
    }

    public async Task<List<JogoGenero>> ListarPorJogosAsync(
        List<int> idsJogos)
    {
        var vinculos = new List<JogoGenero>();

        if (idsJogos.Count == 0)
            return vinculos;

        var nomesParametros = idsJogos.Select(
            (_, i) => $"@id{i}"
        );

        string sql = $"""
            SELECT co_jogo, co_genero
            FROM tb_jogo_genero
            WHERE co_jogo IN ({string.Join(", ", nomesParametros)})
            ORDER BY co_jogo, co_genero
            """;

        await using var conexao = await AbrirConexaoAsync();
        await using var comando = CriarComando(conexao, sql);

        for (int i = 0; i < idsJogos.Count; i++)
            AdicionarParametro(comando, $"@id{i}", idsJogos[i]);

        await using var leitor = await comando.ExecuteReaderAsync();

        while (await leitor.ReadAsync())
        {
            vinculos.Add(new JogoGenero
            {
                IdJogo = leitor.GetInt32(0),
                IdGenero = leitor.GetInt32(1)
            });
        }

        return vinculos;
    }

    public async Task InserirAsync(List<JogoGenero> vinculos)
    {
        const string sql = """
            INSERT INTO tb_jogo_genero (co_jogo, co_genero)
            VALUES (@jogo, @genero)
            """;

        await using var conexao = await AbrirConexaoAsync();

        foreach (var vinculo in vinculos)
        {
            await using var comando = CriarComando(conexao, sql);

            AdicionarParametro(comando, "@jogo", vinculo.IdJogo);
            AdicionarParametro(comando, "@genero", vinculo.IdGenero);

            await ExecutarGravacaoAsync(comando);
        }
    }

    public async Task RemoverPorJogoAsync(int idJogo)
    {
        const string sql = """
            DELETE FROM tb_jogo_genero
            WHERE co_jogo = @jogo
            """;

        await using var conexao = await AbrirConexaoAsync();
        await using var comando = CriarComando(conexao, sql);

        AdicionarParametro(comando, "@jogo", idJogo);

        await ExecutarGravacaoAsync(comando);
    }
}
