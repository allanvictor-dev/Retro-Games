using RetroGames.Model.Model;

namespace RetroGames.Repository;

public class GeneroRepository : RepositoryBase, IGeneroRepository
{
    public GeneroRepository(RepositoryConfig config)
        : base(config)
    {
    }

    public async Task<List<Genero>> ListarAsync()
    {
        const string sql = """
            SELECT co_seq_genero, no_genero
            FROM tb_genero
            ORDER BY no_genero
            """;

        var generos = new List<Genero>();

        await using var conexao = await AbrirConexaoAsync();
        await using var comando = CriarComando(conexao, sql);
        await using var leitor = await comando.ExecuteReaderAsync();

        while (await leitor.ReadAsync())
        {
            generos.Add(new Genero
            {
                Id = leitor.GetInt32(0),
                Nome = leitor.GetString(1)
            });
        }

        return generos;
    }

    public async Task<Genero?> BuscarPorIdAsync(int id)
    {
        const string sql = """
            SELECT co_seq_genero, no_genero
            FROM tb_genero
            WHERE co_seq_genero = @id
            """;

        await using var conexao = await AbrirConexaoAsync();
        await using var comando = CriarComando(conexao, sql);

        AdicionarParametro(comando, "@id", id);

        await using var leitor = await comando.ExecuteReaderAsync();

        if (await leitor.ReadAsync())
        {
            return new Genero
            {
                Id = leitor.GetInt32(0),
                Nome = leitor.GetString(1)
            };
        }

        return null;
    }

    public async Task<bool> ExisteComNomeAsync(
        string nome,
        int? idIgnorado = null)
    {
        const string sql = """
            SELECT COUNT(*)
            FROM tb_genero
            WHERE no_genero = @nome
              AND (@idIgnorado IS NULL
                   OR co_seq_genero <> @idIgnorado)
            """;

        await using var conexao = await AbrirConexaoAsync();
        await using var comando = CriarComando(conexao, sql);

        AdicionarParametro(comando, "@nome", nome);
        AdicionarParametro(comando, "@idIgnorado", idIgnorado);

        var resultado = await comando.ExecuteScalarAsync();

        return Convert.ToInt64(resultado) > 0;
    }

    public async Task<bool> PossuiJogosAsync(int id)
    {
        const string sql = """
            SELECT COUNT(*)
            FROM tb_jogo_genero
            WHERE co_genero = @id
            """;

        await using var conexao = await AbrirConexaoAsync();
        await using var comando = CriarComando(conexao, sql);

        AdicionarParametro(comando, "@id", id);

        var resultado = await comando.ExecuteScalarAsync();

        return Convert.ToInt64(resultado) > 0;
    }

    public async Task<int> InserirAsync(Genero genero)
    {
        const string sql = """
            INSERT INTO tb_genero (no_genero)
            VALUES (@nome)
            """;

        await using var conexao = await AbrirConexaoAsync();
        await using var comando = CriarComando(conexao, sql);

        AdicionarParametro(comando, "@nome", genero.Nome);

        await ExecutarGravacaoAsync(comando);

        return (int)comando.LastInsertedId;
    }

    public async Task AlterarAsync(Genero genero)
    {
        const string sql = """
            UPDATE tb_genero
            SET no_genero = @nome
            WHERE co_seq_genero = @id
            """;

        await using var conexao = await AbrirConexaoAsync();
        await using var comando = CriarComando(conexao, sql);

        AdicionarParametro(comando, "@id", genero.Id);
        AdicionarParametro(comando, "@nome", genero.Nome);

        await ExecutarGravacaoAsync(comando);
    }

    public async Task ExcluirAsync(int id)
    {
        const string sql = """
            DELETE FROM tb_genero
            WHERE co_seq_genero = @id
            """;

        await using var conexao = await AbrirConexaoAsync();
        await using var comando = CriarComando(conexao, sql);

        AdicionarParametro(comando, "@id", id);

        await comando.ExecuteNonQueryAsync();
    }
}
