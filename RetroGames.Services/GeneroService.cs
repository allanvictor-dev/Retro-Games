using RetroGames.Model.Model;
using RetroGames.Repository;

namespace RetroGames.Services;

public class GeneroService : IGeneroService
{
    private readonly IRepositorySet _repositorio;

    public GeneroService(IRepositorySet repositorio)
    {
        _repositorio = repositorio;
    }

    public async Task<List<Genero>> ListarAsync()
    {
        return await _repositorio.Genero.ListarAsync();
    }

    public async Task<Genero> BuscarPorIdAsync(int id)
    {
        var genero = await _repositorio.Genero.BuscarPorIdAsync(id);

        if (genero == null)
            throw new BusinessRuleException(BusinessErrors.GeneroNaoEncontrado);

        return genero;
    }

    public async Task<Genero> InserirAsync(Genero genero)
    {
        genero.Nome = genero.Nome.Trim();

        if (await _repositorio.Genero.ExisteComNomeAsync(genero.Nome))
            throw new BusinessRuleException(BusinessErrors.GeneroNomeDuplicado);

        try
        {
            genero.Id = await _repositorio.Genero.InserirAsync(genero);
        }
        catch (RegistroDuplicadoException)
        {
            throw new BusinessRuleException(BusinessErrors.GeneroNomeDuplicado);
        }

        return genero;
    }

    public async Task AlterarAsync(Genero genero)
    {
        await BuscarPorIdAsync(genero.Id);

        genero.Nome = genero.Nome.Trim();

        if (await _repositorio.Genero.ExisteComNomeAsync(genero.Nome, genero.Id))
            throw new BusinessRuleException(BusinessErrors.GeneroNomeDuplicado);

        try
        {
            await _repositorio.Genero.AlterarAsync(genero);
        }
        catch (RegistroDuplicadoException)
        {
            throw new BusinessRuleException(BusinessErrors.GeneroNomeDuplicado);
        }
    }

    public async Task ExcluirAsync(int id)
    {
        await BuscarPorIdAsync(id);

        if (await _repositorio.Genero.PossuiJogosAsync(id))
            throw new BusinessRuleException(BusinessErrors.GeneroPossuiJogos);

        await _repositorio.Genero.ExcluirAsync(id);
    }
}
