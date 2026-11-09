using MySqlConnector;
using RetroGames.Model.Model;

namespace RetroGames.Repository;

public class JogoRepository : RepositoryBase, IJogoRepository
{
    public JogoRepository(RepositoryConfig config)
        : base(config)
    {
    }

    private static Jogo LerJogo(MySqlDataReader leitor)
    {
        return new Jogo
        {
            Id = leitor.GetInt32(0),
            Nome = leitor.GetString(1),
            Descricao = leitor.IsDBNull(2)
                ? null : leitor.GetString(2),
            AnoLancamento = leitor.GetInt32(3),
            Preco = leitor.GetDecimal(4),
            Estoque = leitor.GetInt32(5),
            Ativo = leitor.GetBoolean(6),
            DataCadastro = leitor.GetDateTime(7),
            IdPlataforma = leitor.GetInt32(8),
            IdCondicaoJogo = leitor.GetInt32(9)
        };
    }

    public async Task<(List<Jogo> Jogos, int Total)> ListarAsync(
        int pagina = 1,
        int tamanhoPagina = 5,
        string? nome = null,
        int? idPlataforma = null,
        List<int>? ids = null,
        bool incluirInativos = false)
    {
        // Uma lista vazia significa nenhum jogo encontrado.
        if (ids is { Count: 0 })
            return (new List<Jogo>(), 0);

        // Evita valores inválidos para LIMIT e OFFSET.
        if (pagina < 1 || tamanhoPagina < 1)
            throw new ArgumentOutOfRangeException(
                nameof(pagina), "Página e tamanho devem ser positivos.");

        const string colunas = """
            co_seq_jogo, no_jogo, ds_jogo,
            nu_ano_lancamento, vl_preco, qt_estoque,
            st_ativo, dt_cadastro, co_plataforma,
            co_condicao_jogo
            """;

        string filtro = """
            WHERE (@nome IS NULL OR no_jogo LIKE CONCAT('%', @nome, '%'))
              AND (@plataforma IS NULL OR co_plataforma = @plataforma)
              AND (@incluirInativos = TRUE OR st_ativo = TRUE)
            """;

        // Cria um parâmetro separado para cada ID.
        if (ids != null)
        {
            var parametrosIds = ids.Select(
                (_, i) => $"@id{i}"
            );

            filtro += " AND co_seq_jogo IN ("
                + string.Join(", ", parametrosIds) + ")";
        }

        string sqlListagem = $"""
            SELECT {colunas}
            FROM tb_jogo
            {filtro}
            ORDER BY no_jogo, co_seq_jogo
            LIMIT @limite OFFSET @deslocamento
            """;

        string sqlContagem = $"""
            SELECT COUNT(*)
            FROM tb_jogo
            {filtro}
            """;

        void PreencherFiltros(MySqlCommand comando)
        {
            AdicionarParametro(comando, "@nome",
                string.IsNullOrWhiteSpace(nome) ? null : nome);
            AdicionarParametro(comando, "@plataforma", idPlataforma);
            AdicionarParametro(comando, "@incluirInativos",
                incluirInativos);

            if (ids != null)
            {
                for (int i = 0; i < ids.Count; i++)
                    AdicionarParametro(comando, $"@id{i}", ids[i]);
            }
        }

        var jogos = new List<Jogo>();

        await using var conexao = await AbrirConexaoAsync();

        await using (var comando = CriarComando(conexao, sqlListagem))
        {
            PreencherFiltros(comando);

            AdicionarParametro(comando, "@limite", tamanhoPagina);
            AdicionarParametro(comando, "@deslocamento",
                checked((pagina - 1) * tamanhoPagina));

            await using var leitor = await comando.ExecuteReaderAsync();

            while (await leitor.ReadAsync())
                jogos.Add(LerJogo(leitor));
        }

        await using var comandoTotal = CriarComando(
            conexao, sqlContagem);

        PreencherFiltros(comandoTotal);

        var resultado = await comandoTotal.ExecuteScalarAsync();
        int total = Convert.ToInt32(resultado);

        return (jogos, total);
    }

    public async Task<Jogo?> BuscarPorIdAsync(int id)
    {
        const string sql = """
            SELECT co_seq_jogo, no_jogo, ds_jogo,
                   nu_ano_lancamento, vl_preco, qt_estoque,
                   st_ativo, dt_cadastro, co_plataforma,
                   co_condicao_jogo
            FROM tb_jogo
            WHERE co_seq_jogo = @id
            """;

        await using var conexao = await AbrirConexaoAsync();
        await using var comando = CriarComando(conexao, sql);

        AdicionarParametro(comando, "@id", id);

        await using var leitor = await comando.ExecuteReaderAsync();

        if (await leitor.ReadAsync())
            return LerJogo(leitor);

        return null;
    }

    public async Task<int> InserirAsync(Jogo jogo)
    {
        const string sql = """
            INSERT INTO tb_jogo
            (no_jogo, ds_jogo, nu_ano_lancamento,
             vl_preco, qt_estoque, st_ativo,
             dt_cadastro, co_plataforma, co_condicao_jogo)
            VALUES
            (@nome, @descricao, @ano, @preco,
             @estoque, @ativo, @data,
             @plataforma, @condicao)
            """;

        await using var conexao = await AbrirConexaoAsync();
        await using var comando = CriarComando(conexao, sql);

        PreencherDados(comando, jogo);

        await ExecutarGravacaoAsync(comando);

        return checked((int)comando.LastInsertedId);
    }

    public async Task AlterarAsync(Jogo jogo)
    {
        const string sql = """
            UPDATE tb_jogo
            SET no_jogo = @nome,
                ds_jogo = @descricao,
                nu_ano_lancamento = @ano,
                vl_preco = @preco,
                qt_estoque = @estoque,
                st_ativo = @ativo,
                co_plataforma = @plataforma,
                co_condicao_jogo = @condicao
            WHERE co_seq_jogo = @id
            """;

        await using var conexao = await AbrirConexaoAsync();
        await using var comando = CriarComando(conexao, sql);

        PreencherDados(comando, jogo);
        AdicionarParametro(comando, "@id", jogo.Id);

        await ExecutarGravacaoAsync(comando);
    }

    public async Task DesativarAsync(int id)
    {
        const string sql = """
            UPDATE tb_jogo
            SET st_ativo = FALSE
            WHERE co_seq_jogo = @id
            """;

        await using var conexao = await AbrirConexaoAsync();
        await using var comando = CriarComando(conexao, sql);

        AdicionarParametro(comando, "@id", id);

        await ExecutarGravacaoAsync(comando);
    }

    private void PreencherDados(MySqlCommand comando, Jogo jogo)
    {
        AdicionarParametro(comando, "@nome", jogo.Nome);
        AdicionarParametro(comando, "@descricao", jogo.Descricao);
        AdicionarParametro(comando, "@ano", jogo.AnoLancamento);
        AdicionarParametro(comando, "@preco", jogo.Preco);
        AdicionarParametro(comando, "@estoque", jogo.Estoque);
        AdicionarParametro(comando, "@ativo", jogo.Ativo);
        AdicionarParametro(comando, "@data", jogo.DataCadastro);
        AdicionarParametro(comando, "@plataforma", jogo.IdPlataforma);
        AdicionarParametro(comando, "@condicao", jogo.IdCondicaoJogo);
    }
}
