import { useEffect, useState } from 'react'
import { Link, useLocation, useSearchParams } from 'react-router-dom'
import { obterMensagemErro } from '../../api/erros.js'
import { jogoApi } from '../../api/servicos.js'
import { Carregando, EstadoVazio, MensagemErro } from '../../components/MensagensEstado.jsx'
import Paginacao from '../../components/Paginacao.jsx'
import { formatarPreco } from '../../lib/formato.js'

const TAMANHO_PAGINA = 12

export default function Jogos() {
  const location = useLocation()
  const [parametros, setParametros] = useSearchParams()
  const busca = parametros.get('busca') || ''
  const numeroPagina = Number(parametros.get('pagina'))
  const pagina = Number.isSafeInteger(numeroPagina) && numeroPagina > 0 ? numeroPagina : 1

  const [resultado, setResultado] = useState(null)
  const [carregando, setCarregando] = useState(true)
  const [ocupado, setOcupado] = useState(false)
  const [erroLista, setErroLista] = useState('')
  const [erroAcao, setErroAcao] = useState('')
  const [sucesso, setSucesso] = useState(location.state?.sucesso || '')
  const [tentativa, setTentativa] = useState(0)

  useEffect(() => {
    let ativo = true
    jogoApi.listar({
      incluirInativos: true,
      busca: busca || undefined,
      pagina,
      tamanhoPagina: TAMANHO_PAGINA,
    })
      .then(dados => {
        if (ativo) {
          setResultado(dados)
          setErroLista('')
        }
      })
      .catch(erroApi => {
        if (ativo) setErroLista(obterMensagemErro(erroApi))
      })
      .finally(() => {
        if (ativo) setCarregando(false)
      })
    return () => { ativo = false }
  }, [busca, pagina, tentativa])

  function recarregar() {
    setCarregando(true)
    setErroLista('')
    setTentativa(valor => valor + 1)
  }

  function buscar(evento) {
    evento.preventDefault()
    const texto = new FormData(evento.currentTarget).get('busca').trim()
    const novos = new URLSearchParams(parametros)
    if (texto) novos.set('busca', texto)
    else novos.delete('busca')
    novos.delete('pagina')
    setCarregando(true)
    setParametros(novos)
  }

  function limparBusca() {
    const novos = new URLSearchParams(parametros)
    novos.delete('busca')
    novos.delete('pagina')
    setCarregando(true)
    setParametros(novos)
  }

  function alterarPagina(numero) {
    const novos = new URLSearchParams(parametros)
    if (numero > 1) novos.set('pagina', String(numero))
    else novos.delete('pagina')
    setCarregando(true)
    setParametros(novos)
  }

  async function desativar(jogo) {
    if (!window.confirm(`Deseja desativar o jogo "${jogo.nome}"? Ele deixará de aparecer no catálogo público.`)) {
      return
    }

    setOcupado(true)
    setErroAcao('')
    setSucesso('')
    try {
      await jogoApi.desativar(jogo.id)
      setSucesso(`Jogo "${jogo.nome}" desativado com sucesso.`)
      recarregar()
    } catch (erroApi) {
      setErroAcao(obterMensagemErro(erroApi))
    } finally {
      setOcupado(false)
    }
  }

  return (
    <>
      <div className="d-flex flex-wrap justify-content-between align-items-center gap-2 mb-4">
        <h1 className="mb-0">Gerenciamento de jogos</h1>
        <Link to="/admin/jogos/novo" className="btn btn-primary">Novo jogo</Link>
      </div>

      {erroAcao && <MensagemErro mensagem={erroAcao} />}
      {sucesso && <div className="alert alert-success" role="status">{sucesso}</div>}

      <form className="row g-2 mb-4" onSubmit={buscar}>
        <div className="col-12 col-md-6">
          <label htmlFor="busca-admin-jogos" className="form-label">Buscar por nome</label>
          <div className="input-group">
            <input key={busca} id="busca-admin-jogos" name="busca" type="text"
              className="form-control" defaultValue={busca} placeholder="Nome do jogo" />
            <button type="submit" className="btn btn-primary">Buscar</button>
            {busca && (
              <button type="button" className="btn btn-outline-secondary" onClick={limparBusca}>
                Limpar
              </button>
            )}
          </div>
        </div>
      </form>

      {carregando ? <Carregando /> : erroLista ? (
        <MensagemErro mensagem={erroLista} tentarNovamente={recarregar} />
      ) : !resultado || resultado.itens.length === 0 ? (
        <EstadoVazio mensagem="Nenhum jogo encontrado." />
      ) : (
        <>
          <div className="table-responsive">
            <table className="table table-striped align-middle">
              <thead>
                <tr>
                  <th>Nome</th>
                  <th>Plataforma</th>
                  <th>Ano</th>
                  <th>Preço</th>
                  <th>Estoque</th>
                  <th>Situação</th>
                  <th>Ações</th>
                </tr>
              </thead>
              <tbody>
                {resultado.itens.map(jogo => (
                  <tr key={jogo.id}>
                    <td>{jogo.nome}</td>
                    <td>{jogo.nomePlataforma}</td>
                    <td>{jogo.anoLancamento}</td>
                    <td>{formatarPreco(jogo.preco)}</td>
                    <td>{jogo.estoque}</td>
                    <td>
                      <span className={jogo.ativo ? 'badge text-bg-success' : 'badge text-bg-secondary'}>
                        {jogo.ativo ? 'Ativo' : 'Inativo'}
                      </span>
                    </td>
                    <td>
                      <div className="d-flex gap-2">
                        <Link to={`/admin/jogos/${jogo.id}/editar`}
                          className="btn btn-outline-primary btn-sm">
                          Editar
                        </Link>
                        {jogo.ativo && (
                          <button type="button" className="btn btn-outline-danger btn-sm"
                            disabled={ocupado} onClick={() => desativar(jogo)}>
                            Desativar
                          </button>
                        )}
                      </div>
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
          <Paginacao pagina={pagina} totalPaginas={resultado.totalPaginas} alterarPagina={alterarPagina} />
        </>
      )}
    </>
  )
}
