using RetroGames.WebApi.ApiModels;
using RetroGames.WebApi.Validators.Autenticacao;

namespace RetroGames.WebApi.UnitTests;

public class CadastrarUsuarioRequestValidatorTests
{
    private readonly CadastrarUsuarioRequestValidator _validator = new();

    [Fact]
    public void Validar_DadosValidos_RetornaValido()
    {
        var request = new CadastrarUsuarioRequestApiModel
        {
            Nome = "Maria Silva",
            Email = "maria@exemplo.com",
            Senha = "senha123"
        };

        var resultado = _validator.Validate(request);

        Assert.True(resultado.IsValid);
    }

    [Theory]
    [InlineData("", "maria@exemplo.com", "senha123")]
    [InlineData("   ", "maria@exemplo.com", "senha123")]
    [InlineData("Maria", "", "senha123")]
    [InlineData("Maria", "email-invalido", "senha123")]
    [InlineData("Maria", "maria@exemplo.com", "")]
    [InlineData("Maria", "maria@exemplo.com", "12345")]
    public void Validar_DadosInvalidos_RetornaInvalido(string nome, string email, string senha)
    {
        var request = new CadastrarUsuarioRequestApiModel
        {
            Nome = nome,
            Email = email,
            Senha = senha
        };

        var resultado = _validator.Validate(request);

        Assert.False(resultado.IsValid);
    }

    [Fact]
    public void Validar_NomeAcimaDoLimite_RetornaInvalido()
    {
        var request = new CadastrarUsuarioRequestApiModel
        {
            Nome = new string('a', 121),
            Email = "maria@exemplo.com",
            Senha = "senha123"
        };

        var resultado = _validator.Validate(request);

        Assert.False(resultado.IsValid);
    }

    [Fact]
    public void Validar_SenhaAcimaDoLimite_RetornaInvalido()
    {
        var request = new CadastrarUsuarioRequestApiModel
        {
            Nome = "Maria Silva",
            Email = "maria@exemplo.com",
            Senha = new string('a', 101)
        };

        var resultado = _validator.Validate(request);

        Assert.False(resultado.IsValid);
    }
}
