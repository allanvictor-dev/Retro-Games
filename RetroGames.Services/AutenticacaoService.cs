using Microsoft.AspNetCore.Identity;
using RetroGames.Model.Constantes;
using RetroGames.Model.Model;
using RetroGames.Repository;

namespace RetroGames.Services;

public class AutenticacaoService : IAutenticacaoService
{
    private readonly IRepositorySet _repositorio;
    private readonly IPasswordHasher<Usuario> _passwordHasher;

    public AutenticacaoService(IRepositorySet repositorio, IPasswordHasher<Usuario> passwordHasher)
    {
        _repositorio = repositorio;
        _passwordHasher = passwordHasher;
    }

    public async Task<Usuario?> AutenticarAsync(string email, string senha)
    {
        var usuario = await _repositorio.Usuario.BuscarPorEmailAsync(email.Trim());

        if (usuario == null || !usuario.Ativo)
            return null;

        var resultado = _passwordHasher.VerifyHashedPassword(usuario, usuario.SenhaHash, senha);
        return resultado == PasswordVerificationResult.Failed ? null : usuario;
    }

    public async Task GarantirAdministradorInicialAsync(string? email, string? senha)
    {
        if (await _repositorio.Usuario.ExisteAdministradorAsync())
            return;

        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(senha)
            || senha == "SUA_SENHA")
            throw new InvalidOperationException("Configure AdminInicial__Email e AdminInicial__Senha no arquivo .env.");

        var usuario = new Usuario
        {
            Nome = Perfis.Administrador,
            Email = email.Trim(),
            Ativo = true,
            DataCadastro = DateTime.UtcNow,
            IdPerfil = PerfilIds.Administrador
        };

        usuario.SenhaHash = _passwordHasher.HashPassword(usuario, senha);
        await _repositorio.Usuario.InserirAsync(usuario);
    }
}
