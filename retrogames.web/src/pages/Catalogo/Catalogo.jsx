import { useEffect, useState } from 'react'
import { useSearchParams } from 'react-router-dom'
import { jogoApi, plataformaApi, generoApi } from '../../api/servicos.js'
import { obterMensagemErro } from '../../api/erros.js'
import { Carregando, MensagemErro, EstadoVazio } from '../../components/MensagensEstado.jsx'
import CardJogo from './CardJogo.jsx'
import Paginacao from '../../components/Paginacao.jsx'

export default function Catalogo() {
  const [parametros, setParametros] = useSearchParams()
  const busca = parametros.get('busca') || ''
  const idPlataforma = parametros.get('idPlataforma') || ''
  const idGenero = parametros.get('idGenero') || ''
  const numeroPagina = Number(parametros.get('pagina'))
  const pagina = Number.isSafeInteger(numeroPagina) && numeroPagina > 0 ? numeroPagina : 1
  const [opcoes, setOpcoes] = useState({ plataformas: [], generos: [] })
  const [resultado, setResultado] = useState(null)
  const [carregando, setCarregando] = useState(true)
  const [erro, setErro] = useState('')
  const [erroOpcoes, setErroOpcoes] = useState('')
  const [tentativa, setTentativa] = useState(0)
  const [tentativaOpcoes, setTentativaOpcoes] = useState(0)

  useEffect(() => {
    let ativo = true
    Promise.all([plataformaApi.listar(), generoApi.listar()])
      .then(([plataformas, generos]) => {
        if (ativo) {
          setOpcoes({ plataformas, generos })
          setErroOpcoes('')
        }
      })
      .catch(erro => { if (ativo) setErroOpcoes(obterMensagemErro(erro)) })
    return () => { ativo = false }
  }, [tentativaOpcoes])

  useEffect(() => {
    let ativo = true
    setCarregando(true)
    setErro('')
    jogoApi.listar({
      busca: busca || undefined,
      idPlataforma: idPlataforma || undefined,
      idGenero: idGenero || undefined,
      pagina,
      tamanhoPagina: 12,
    })
      .then(dados => { if (ativo) setResultado(dados) })
      .catch(erro => { if (ativo) setErro(obterMensagemErro(erro)) })
      .finally(() => { if (ativo) setCarregando(false) })
    return () => { ativo = false }
  }, [busca, idPlataforma, idGenero, pagina, tentativa])

  function alterarFiltro(nome, valor) {
    const novos = new URLSearchParams(parametros)
    if (valor) novos.set(nome, valor)
    else novos.delete(nome)
    novos.delete('pagina')
    setParametros(novos)
  }

  function alterarPagina(numero) {
    const novos = new URLSearchParams(parametros)
    if (numero > 1) novos.set('pagina', String(numero))
    else novos.delete('pagina')
    setParametros(novos)
  }

  return (
    <>
      <h1 className="mb-4">Catálogo de jogos</h1>
      <div className="row g-3 mb-4">
        <form className="col-md-6" onSubmit={evento => {
          evento.preventDefault()
          alterarFiltro('busca', new FormData(evento.currentTarget).get('busca').trim())
        }}>
          <label htmlFor="busca" className="form-label">Buscar por nome</label>
          <div className="input-group">
            <input key={busca} id="busca" name="busca" className="form-control" defaultValue={busca} placeholder="Nome do jogo" />
            <button className="btn btn-primary" type="submit">Buscar</button>
          </div>
        </form>
        <div className="col-md-3">
          <label htmlFor="plataforma" className="form-label">Plataforma</label>
          <select id="plataforma" className="form-select" value={idPlataforma} onChange={evento => alterarFiltro('idPlataforma', evento.target.value)}>
            <option value="">Todas</option>
            {opcoes.plataformas.map(plataforma => <option key={plataforma.id} value={plataforma.id}>{plataforma.nome}</option>)}
          </select>
        </div>
        <div className="col-md-3">
          <label htmlFor="genero" className="form-label">Gênero</label>
          <select id="genero" className="form-select" value={idGenero} onChange={evento => alterarFiltro('idGenero', evento.target.value)}>
            <option value="">Todos</option>
            {opcoes.generos.map(genero => <option key={genero.id} value={genero.id}>{genero.nome}</option>)}
          </select>
        </div>
      </div>
      {erroOpcoes && <MensagemErro mensagem={erroOpcoes} tentarNovamente={() => setTentativaOpcoes(valor => valor + 1)} />}
      {carregando ? <Carregando /> : erro ? (
        <MensagemErro mensagem={erro} tentarNovamente={() => setTentativa(valor => valor + 1)} />
      ) : resultado && (
        <>
          {resultado.itens.length === 0 ? <EstadoVazio /> : (
            <div className="row row-cols-1 row-cols-md-2 row-cols-lg-3 g-3">
              {resultado.itens.map(jogo => <div className="col" key={jogo.id}><CardJogo jogo={jogo} /></div>)}
            </div>
          )}
          <Paginacao pagina={pagina} totalPaginas={resultado.totalPaginas} alterarPagina={alterarPagina} />
        </>
      )}
    </>
  )
}
