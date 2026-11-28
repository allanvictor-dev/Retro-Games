using RetroGames.Model.Model;
using RetroGames.WebApi.ApiModels;

namespace RetroGames.WebApi.Mappers;

public static class GeneroMapper
{
    public static ListarGeneroResponseApiModel ParaResponse(this Genero entidade)
    {
        return new ListarGeneroResponseApiModel
        {
            Id = entidade.Id,
            Nome = entidade.Nome
        };
    }

    public static Genero ParaModel(this InserirGeneroRequestApiModel request)
    {
        return new Genero
        {
            Nome = request.Nome
        };
    }

    public static Genero ParaModel(this AlterarGeneroRequestApiModel request, int id)
    {
        return new Genero
        {
            Id = id,
            Nome = request.Nome
        };
    }
}
