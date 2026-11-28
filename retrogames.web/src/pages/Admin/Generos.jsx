import { useEffect, useState } from 'react'
import { obterErrosDosCampos, obterMensagemErro } from '../../api/erros.js'
import { generoApi } from '../../api/servicos.js'
import { Carregando, EstadoVazio, MensagemErro } from '../../components/MensagensEstado.jsx'

const CODIGO_GENERO_NOME_DUPLICADO = 107
const CODIGO_GENERO_POSSUI_JOGOS = 108

const MENSAGEM_GENERO_DUPLICADO = 'Já existe um gênero com este nome.'
const MENSAGEM_GENERO_POSSUI_JOGOS = 'Não é possível excluir: existem jogos vinculados a este gênero.'

function validar(nome) {
  const limpo = nome.trim()
  if (!limpo) return { nome: 'Informe o nome do gênero.' }
  if (limpo.length > 80) return { nome: 'O nome do gênero deve ter no máximo 80 caracteres.' }
  return {}
}

export default function Generos() {
  const [generos, setGeneros] = useState([])
  const [nome, setNome] = useState('')
  const [idEdicao, setIdEdicao] = useState(null)
  const [carregando, setCarregando] = useState(true)
  const [ocupado, setOcupado] = useState(false)
  const [erroLista, setErroLista] = useState('')
  const [erro, setErro] = useState('')
  const [errosCampos, setErrosCampos] = useState({})
  const [sucesso, setSucesso] = useState('')

  async function carregarGeneros() {
    setCarregando(true)
    setErroLista('')
    try {
      setGeneros(await generoApi.listar())
    } catch (erroApi) {
      setErroLista(obterMensagemErro(erroApi))
    } finally {
      setCarregando(false)
    }
  }

  useEffect(() => {
    let ativo = true
    generoApi.listar()
      .then(dados => { if (ativo) setGeneros(dados) })
      .catch(erroApi => { if (ativo) setErroLista(obterMensagemErro(erroApi)) })
      .finally(() => { if (ativo) setCarregando(false) })
    return () => { ativo = false }
  }, [])

  function limparFormulario() {
    setNome('')
    setIdEdicao(null)
    setErro('')
    setErrosCampos({})
  }

  function editar(genero) {
    setIdEdicao(genero.id)
    setNome(genero.nome)
    setErro('')
    setErrosCampos({})
    setSucesso('')
  }

  function alterarNome(evento) {
    setNome(evento.target.value)
    setErrosCampos({})
  }

  async function salvar(evento) {
    evento.preventDefault()
    setErro('')
    setSucesso('')

    const errosValidacao = validar(nome)
    setErrosCampos(errosValidacao)
    if (Object.keys(errosValidacao).length > 0) return

    setOcupado(true)
    const dados = { nome: nome.trim() }

    try {
      if (idEdicao === null) {
        await generoApi.inserir(dados)
        setSucesso('Gênero cadastrado com sucesso.')
      } else {
        await generoApi.alterar(idEdicao, dados)
        setSucesso('Gênero alterado com sucesso.')
      }
      limparFormulario()
      await carregarGeneros()
    } catch (erroApi) {
      if (erroApi.response?.data?.responseCode === CODIGO_GENERO_NOME_DUPLICADO) {
        setErrosCampos({ nome: MENSAGEM_GENERO_DUPLICADO })
      } else {
        setErro(obterMensagemErro(erroApi))
        setErrosCampos(obterErrosDosCampos(erroApi))
      }
    } finally {
      setOcupado(false)
    }
  }

  async function excluir(genero) {
    if (!window.confirm(`Deseja excluir o gênero "${genero.nome}"?`)) return
    setOcupado(true)
    setErro('')
    setErrosCampos({})
    setSucesso('')
    try {
      await generoApi.excluir(genero.id)
      if (idEdicao === genero.id) limparFormulario()
      setSucesso('Gênero excluído com sucesso.')
      await carregarGeneros()
    } catch (erroApi) {
      if (erroApi.response?.data?.responseCode === CODIGO_GENERO_POSSUI_JOGOS) {
        setErro(MENSAGEM_GENERO_POSSUI_JOGOS)
      } else {
        setErro(obterMensagemErro(erroApi))
      }
    } finally {
      setOcupado(false)
    }
  }

  return (
    <>
      <h1 className="mb-4">Gerenciamento de gêneros</h1>
      {erro && <MensagemErro mensagem={erro} />}
      {sucesso && <div className="alert alert-success" role="status">{sucesso}</div>}

      <form className="card card-body mb-4" onSubmit={salvar} noValidate>
        <h2 className="h5 mb-3">{idEdicao === null ? 'Cadastrar gênero' : 'Editar gênero'}</h2>
        <fieldset disabled={ocupado}>
          <div className="row g-3">
            <div className="col-12 col-md-6">
              <label htmlFor="nome" className="form-label">Nome</label>
              <input id="nome" name="nome" type="text" maxLength={80}
                className={'form-control' + (errosCampos.nome ? ' is-invalid' : '')}
                value={nome} onChange={alterarNome}
                aria-invalid={Boolean(errosCampos.nome)}
                aria-describedby={errosCampos.nome ? 'nome-erro' : undefined} />
              {errosCampos.nome && (
                <div id="nome-erro" className="invalid-feedback text-danger-emphasis">
                  {errosCampos.nome}
                </div>
              )}
            </div>
          </div>
          <div className="mt-3 d-flex gap-2">
            <button type="submit" className="btn btn-primary">{ocupado ? 'Aguarde...' : 'Salvar'}</button>
            {idEdicao !== null && (
              <button type="button" className="btn btn-secondary" onClick={limparFormulario}>Cancelar</button>
            )}
          </div>
        </fieldset>
      </form>

      {carregando ? <Carregando /> : erroLista ? (
        <MensagemErro mensagem={erroLista} tentarNovamente={carregarGeneros} />
      ) : generos.length === 0 ? (
        <EstadoVazio mensagem="Nenhum gênero cadastrado." />
      ) : (
        <div className="table-responsive">
          <table className="table table-striped align-middle">
            <thead>
              <tr>
                <th>Nome</th>
                <th>Ações</th>
              </tr>
            </thead>
            <tbody>
              {generos.map(genero => (
                <tr key={genero.id}>
                  <td>{genero.nome}</td>
                  <td>
                    <div className="d-flex gap-2">
                      <button type="button" className="btn btn-outline-primary btn-sm" disabled={ocupado}
                        onClick={() => editar(genero)}>Editar</button>
                      <button type="button" className="btn btn-outline-danger btn-sm" disabled={ocupado}
                        onClick={() => excluir(genero)}>Excluir</button>
                    </div>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      )}
    </>
  )
}
