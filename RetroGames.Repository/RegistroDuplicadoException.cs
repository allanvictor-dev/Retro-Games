namespace RetroGames.Repository;

public class RegistroDuplicadoException : Exception
{
    public RegistroDuplicadoException(
        string mensagem,
        Exception? excecaoInterna = null)
        : base(mensagem, excecaoInterna)
    {
    }
}
