using MySqlConnector;

namespace RetroGames.Repository;

public abstract class RepositoryBase
{
    private readonly RepositoryConfig _config;

    protected RepositoryBase(RepositoryConfig config)
    {
        _config = config;
    }

    // Abre uma conexão com o MySQL.
    protected async Task<MySqlConnection> AbrirConexaoAsync()
    {
        var conexao = new MySqlConnection(_config.ConnectionString);

        try
        {
            await conexao.OpenAsync();
            return conexao;
        }
        catch
        {
            await conexao.DisposeAsync();
            throw;
        }
    }

    // Cria um comando SQL associado à conexão.
    protected MySqlCommand CriarComando(
        MySqlConnection conexao,
        string sql)
    {
        return new MySqlCommand(sql, conexao);
    }

    // Adiciona um parâmetro ao comando.
    protected void AdicionarParametro(
        MySqlCommand comando,
        string nome,
        object? valor)
    {
        comando.Parameters.AddWithValue(
            nome,
            valor ?? DBNull.Value
        );
    }

    // Executa INSERT ou UPDATE e trata registros duplicados.
    protected async Task<int> ExecutarGravacaoAsync(
        MySqlCommand comando)
    {
        try
        {
            return await comando.ExecuteNonQueryAsync();
        }
        catch (MySqlException ex) when (ex.Number == 1062)
        {
            throw new RegistroDuplicadoException(
                "Já existe um registro com esse valor.",
                ex
            );
        }
    }
}
