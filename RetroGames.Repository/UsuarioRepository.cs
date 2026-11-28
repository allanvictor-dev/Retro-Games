using MySqlConnector;
using RetroGames.Model.Constantes;
using RetroGames.Model.Model;

namespace RetroGames.Repository;

public class UsuarioRepository : RepositoryBase, IUsuarioRepository
{
    public UsuarioRepository(RepositoryConfig config) : base(config)
    {
    }

    private static Usuario LerUsuario(MySqlDataReader leitor)
    {
        return new Usuario
        {
            Id = leitor.GetInt32(0),
            Nome = leitor.GetString(1),
            Email = leitor.GetString(2),
            SenhaHash = leitor.GetString(3),
            Ativo = leitor.GetBoolean(4),
            DataCadastro = leitor.GetDateTime(5),
            IdPerfil = leitor.GetInt32(6)
        };
    }

    public async Task<Usuario?> BuscarPorEmailAsync(string email)
    {
        const string sql = """
            SELECT co_seq_usuario, no_usuario, ds_email, ds_senha_hash,
                   st_ativo, dt_cadastro, co_perfil
            FROM tb_usuario
            WHERE ds_email = @email
            """;

        await using var conexao = await AbrirConexaoAsync();
        await using var comando = CriarComando(conexao, sql);
        AdicionarParametro(comando, "@email", email);
        await using var leitor = await comando.ExecuteReaderAsync();

        return await leitor.ReadAsync() ? LerUsuario(leitor) : null;
    }

    public async Task<Usuario?> BuscarPorIdAsync(int id)
    {
        const string sql = """
            SELECT co_seq_usuario, no_usuario, ds_email, ds_senha_hash,
                   st_ativo, dt_cadastro, co_perfil
            FROM tb_usuario
            WHERE co_seq_usuario = @id
            """;

        await using var conexao = await AbrirConexaoAsync();
        await using var comando = CriarComando(conexao, sql);
        AdicionarParametro(comando, "@id", id);
        await using var leitor = await comando.ExecuteReaderAsync();

        return await leitor.ReadAsync() ? LerUsuario(leitor) : null;
    }

    public async Task<bool> ExisteComEmailAsync(string email)
    {
        const string sql = """
            SELECT COUNT(*)
            FROM tb_usuario
            WHERE ds_email = @email
            """;

        await using var conexao = await AbrirConexaoAsync();
        await using var comando = CriarComando(conexao, sql);
        AdicionarParametro(comando, "@email", email);

        return Convert.ToInt64(await comando.ExecuteScalarAsync()) > 0;
    }

    public async Task<bool> ExisteAdministradorAsync()
    {
        const string sql = """
            SELECT COUNT(*)
            FROM tb_usuario
            WHERE co_perfil = @perfil
            """;

        await using var conexao = await AbrirConexaoAsync();
        await using var comando = CriarComando(conexao, sql);
        AdicionarParametro(comando, "@perfil", PerfilIds.Administrador);

        return Convert.ToInt64(await comando.ExecuteScalarAsync()) > 0;
    }

    public async Task<int> InserirAsync(Usuario usuario)
    {
        const string sql = """
            INSERT INTO tb_usuario
            (no_usuario, ds_email, ds_senha_hash, st_ativo, dt_cadastro, co_perfil)
            VALUES (@nome, @email, @hash, @ativo, @data, @perfil)
            """;

        await using var conexao = await AbrirConexaoAsync();
        await using var comando = CriarComando(conexao, sql);
        AdicionarParametro(comando, "@nome", usuario.Nome);
        AdicionarParametro(comando, "@email", usuario.Email);
        AdicionarParametro(comando, "@hash", usuario.SenhaHash);
        AdicionarParametro(comando, "@ativo", usuario.Ativo);
        AdicionarParametro(comando, "@data", usuario.DataCadastro);
        AdicionarParametro(comando, "@perfil", usuario.IdPerfil);

        await ExecutarGravacaoAsync(comando);
        return checked((int)comando.LastInsertedId);
    }
}
