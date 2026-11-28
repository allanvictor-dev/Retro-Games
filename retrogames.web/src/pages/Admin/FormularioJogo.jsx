import { useEffect, useState } from 'react'
import { Link, useNavigate, useParams } from 'react-router-dom'
import { obterErrosDosCampos, obterMensagemErro } from '../../api/erros.js'
import { condicaoJogoApi, generoApi, jogoApi, plataformaApi } from '../../api/servicos.js'
import { Carregando, MensagemErro } from '../../components/MensagensEstado.jsx'

const formularioVazio = {
  nome: '',
  descricao: '',
  anoLancamento: '',
  preco: '',
  estoque: '',
  idPlataforma: '',
  idCondicaoJogo: '',
  idsGeneros: [],
  ativo: true,
}

// Mesmas regras de InserirJogoRequestValidator / AlterarJogoRequestValidator do back-end.
function validar(formulario) {
  const erros = {}
  const nome = formulario.nome.trim()
  const descricao = formulario.descricao.trim()
  const ano = Number(formulario.anoLancamento)
  const precoTexto = String(formulario.preco).trim().replace(',', '.')
  const preco = Number(precoTexto)
  const estoque = Number(formulario.estoque)

  if (!nome) erros.nome = 'Informe o nome do jogo.'
  else if (nome.length > 150) erros.nome = 'O nome deve ter no máximo 150 caracteres.'

  if (descricao.length > 1000) erros.descricao = 'A descrição deve ter no máximo 1000 caracteres.'

  if (formulario.anoLancamento === '' || !Number.isInteger(ano) || ano < 1970 || ano > 2100) {
    erros.anoLancamento = 'Informe um ano entre 1970 e 2100.'
  }

  if (!precoTexto || !/^\d+(\.\d{1,2})?$/.test(precoTexto) || !(preco > 0)) {
    erros.preco = 'Informe um preço maior que zero com até duas casas decimais.'
  }

  if (formulario.estoque === '' || !Number.isInteger(estoque) || estoque < 0) {
    erros.estoque = 'Informe um estoque inteiro maior ou igual a zero.'
  }

  if (!Number(formulario.idPlataforma)) {
    erros.idPlataforma = 'Selecione uma plataforma.'
  }

  if (!Number(formulario.idCondicaoJogo)) {
    erros.idCondicaoJogo = 'Selecione uma condição.'
  }

  if (formulario.idsGeneros.length === 0) {
    erros.idsGeneros = 'Selecione ao menos um gênero.'
  }

  return erros
}

export default function FormularioJogo() {
  const { id } = useParams()
  const ehEdicao = Boolean(id)
  const navigate = useNavigate()

  const [formulario, setFormulario] = useState(formularioVazio)
  const [opcoes, setOpcoes] = useState({ plataformas: [], condicoes: [], generos: [] })
  const [carregando, setCarregando] = useState(true)
  const [erroCarga, setErroCarga] = useState('')
  const [salvando, setSalvando] = useState(false)
  const [erro, setErro] = useState('')
  const [errosCampos, setErrosCampos] = useState({})
  const [tentativa, setTentativa] = useState(0)

  useEffect(() => {
    let ativo = true
    const promessas = [
      plataformaApi.listar(),
      condicaoJogoApi.listar(),
      generoApi.listar(),
    ]
    if (ehEdicao) promessas.push(jogoApi.buscarPorId(id))

    Promise.all(promessas)
      .then(([plataformas, condicoes, generos, jogo]) => {
        if (!ativo) return
        setOpcoes({ plataformas, condicoes, generos })
        if (jogo) {
          setFormulario({
            nome: jogo.nome,
            descricao: jogo.descricao ?? '',
            anoLancamento: String(jogo.anoLancamento),
            preco: String(jogo.preco),
            estoque: String(jogo.estoque),
            idPlataforma: String(jogo.idPlataforma),
            idCondicaoJogo: String(jogo.idCondicaoJogo),
            idsGeneros: jogo.generos.map(genero => genero.id),
            ativo: jogo.ativo,
          })
        } else {
          setFormulario(formularioVazio)
        }
        setErroCarga('')
      })
      .catch(erroApi => {
        if (ativo) setErroCarga(obterMensagemErro(erroApi))
      })
      .finally(() => {
        if (ativo) setCarregando(false)
      })

    return () => { ativo = false }
  }, [ehEdicao, id, tentativa])

  function recarregar() {
    setCarregando(true)
    setErroCarga('')
    setTentativa(valor => valor + 1)
  }

  function alterarCampo(evento) {
    const { name, type, value, checked } = evento.target
    setFormulario(anterior => ({ ...anterior, [name]: type === 'checkbox' ? checked : value }))
    setErrosCampos(anterior => ({ ...anterior, [name]: '' }))
  }

  function alternarGenero(idGenero) {
    setFormulario(anterior => {
      const existe = anterior.idsGeneros.includes(idGenero)
      const idsGeneros = existe
        ? anterior.idsGeneros.filter(item => item !== idGenero)
        : [...anterior.idsGeneros, idGenero]
      return { ...anterior, idsGeneros }
    })
    setErrosCampos(anterior => ({ ...anterior, idsGeneros: '' }))
  }

  async function salvar(evento) {
    evento.preventDefault()
    setErro('')

    const errosValidacao = validar(formulario)
    setErrosCampos(errosValidacao)
    if (Object.keys(errosValidacao).length > 0) return

    setSalvando(true)
    const descricaoLimpa = formulario.descricao.trim()
    const dados = {
      nome: formulario.nome.trim(),
      descricao: descricaoLimpa || null,
      anoLancamento: Number(formulario.anoLancamento),
      preco: Number(String(formulario.preco).trim().replace(',', '.')),
      estoque: Number(formulario.estoque),
      idPlataforma: Number(formulario.idPlataforma),
      idCondicaoJogo: Number(formulario.idCondicaoJogo),
      idsGeneros: formulario.idsGeneros,
    }

    try {
      if (ehEdicao) {
        await jogoApi.alterar(id, { ...dados, ativo: formulario.ativo })
        navigate('/admin/jogos', { state: { sucesso: 'Jogo alterado com sucesso.' } })
      } else {
        await jogoApi.inserir(dados)
        navigate('/admin/jogos', { state: { sucesso: 'Jogo cadastrado com sucesso.' } })
      }
    } catch (erroApi) {
      setErro(obterMensagemErro(erroApi))
      setErrosCampos(obterErrosDosCampos(erroApi))
      setSalvando(false)
    }
  }

  if (carregando) return <Carregando />

  if (erroCarga) {
    return (
      <>
        <h1 className="mb-4">{ehEdicao ? 'Editar jogo' : 'Cadastrar jogo'}</h1>
        <MensagemErro mensagem={erroCarga} tentarNovamente={recarregar} />
        <Link to="/admin/jogos" className="btn btn-secondary">Voltar</Link>
      </>
    )
  }

  return (
    <div className="row justify-content-center">
      <div className="col-12 col-lg-8">
        <div className="d-flex flex-wrap justify-content-between align-items-center gap-2 mb-4">
          <h1 className="mb-0">{ehEdicao ? 'Editar jogo' : 'Cadastrar jogo'}</h1>
          <Link to="/admin/jogos" className="btn btn-outline-secondary">Voltar</Link>
        </div>

        {erro && <MensagemErro mensagem={erro} />}

        <form className="card card-body" onSubmit={salvar} noValidate>
          <fieldset disabled={salvando}>
            <div className="mb-3">
              <label htmlFor="nome" className="form-label">Nome</label>
              <input id="nome" name="nome" type="text" maxLength={150}
                className={'form-control' + (errosCampos.nome ? ' is-invalid' : '')}
                aria-invalid={Boolean(errosCampos.nome)}
                aria-describedby={errosCampos.nome ? 'nome-erro' : undefined}
                value={formulario.nome} onChange={alterarCampo} />
              {errosCampos.nome && (
                <div id="nome-erro" className="invalid-feedback text-danger-emphasis">{errosCampos.nome}</div>
              )}
            </div>

            <div className="mb-3">
              <label htmlFor="descricao" className="form-label">Descrição</label>
              <textarea id="descricao" name="descricao" rows={3} maxLength={1000}
                className={'form-control' + (errosCampos.descricao ? ' is-invalid' : '')}
                aria-invalid={Boolean(errosCampos.descricao)}
                aria-describedby={errosCampos.descricao ? 'descricao-erro' : undefined}
                value={formulario.descricao} onChange={alterarCampo} />
              {errosCampos.descricao && (
                <div id="descricao-erro" className="invalid-feedback text-danger-emphasis">{errosCampos.descricao}</div>
              )}
            </div>

            <div className="row g-3 mb-3">
              <div className="col-12 col-md-4">
                <label htmlFor="anoLancamento" className="form-label">Ano de lançamento</label>
                <input id="anoLancamento" name="anoLancamento" type="number" min={1970} max={2100}
                  className={'form-control' + (errosCampos.anoLancamento ? ' is-invalid' : '')}
                  aria-invalid={Boolean(errosCampos.anoLancamento)}
                  aria-describedby={errosCampos.anoLancamento ? 'anoLancamento-erro' : undefined}
                  value={formulario.anoLancamento} onChange={alterarCampo} />
                {errosCampos.anoLancamento && (
                  <div id="anoLancamento-erro" className="invalid-feedback text-danger-emphasis">
                    {errosCampos.anoLancamento}
                  </div>
                )}
              </div>

              <div className="col-12 col-md-4">
                <label htmlFor="preco" className="form-label">Preço (R$)</label>
                <input id="preco" name="preco" type="number" step="0.01" min="0.01"
                  className={'form-control' + (errosCampos.preco ? ' is-invalid' : '')}
                  aria-invalid={Boolean(errosCampos.preco)}
                  aria-describedby={errosCampos.preco ? 'preco-erro' : undefined}
                  value={formulario.preco} onChange={alterarCampo} />
                {errosCampos.preco && (
                  <div id="preco-erro" className="invalid-feedback text-danger-emphasis">{errosCampos.preco}</div>
                )}
              </div>

              <div className="col-12 col-md-4">
                <label htmlFor="estoque" className="form-label">Estoque</label>
                <input id="estoque" name="estoque" type="number" min={0} step={1}
                  className={'form-control' + (errosCampos.estoque ? ' is-invalid' : '')}
                  aria-invalid={Boolean(errosCampos.estoque)}
                  aria-describedby={errosCampos.estoque ? 'estoque-erro' : undefined}
                  value={formulario.estoque} onChange={alterarCampo} />
                {errosCampos.estoque && (
                  <div id="estoque-erro" className="invalid-feedback text-danger-emphasis">{errosCampos.estoque}</div>
                )}
              </div>
            </div>

            <div className="row g-3 mb-3">
              <div className="col-12 col-md-6">
                <label htmlFor="idPlataforma" className="form-label">Plataforma</label>
                <select id="idPlataforma" name="idPlataforma"
                  className={'form-select' + (errosCampos.idPlataforma ? ' is-invalid' : '')}
                  aria-invalid={Boolean(errosCampos.idPlataforma)}
                  aria-describedby={errosCampos.idPlataforma ? 'idPlataforma-erro' : undefined}
                  value={formulario.idPlataforma} onChange={alterarCampo}>
                  <option value="">Selecione...</option>
                  {opcoes.plataformas.map(plataforma => (
                    <option key={plataforma.id} value={plataforma.id}>{plataforma.nome}</option>
                  ))}
                </select>
                {errosCampos.idPlataforma && (
                  <div id="idPlataforma-erro" className="invalid-feedback text-danger-emphasis">
                    {errosCampos.idPlataforma}
                  </div>
                )}
              </div>

              <div className="col-12 col-md-6">
                <label htmlFor="idCondicaoJogo" className="form-label">Condição</label>
                <select id="idCondicaoJogo" name="idCondicaoJogo"
                  className={'form-select' + (errosCampos.idCondicaoJogo ? ' is-invalid' : '')}
                  aria-invalid={Boolean(errosCampos.idCondicaoJogo)}
                  aria-describedby={errosCampos.idCondicaoJogo ? 'idCondicaoJogo-erro' : undefined}
                  value={formulario.idCondicaoJogo} onChange={alterarCampo}>
                  <option value="">Selecione...</option>
                  {opcoes.condicoes.map(condicao => (
                    <option key={condicao.id} value={condicao.id}>{condicao.descricao}</option>
                  ))}
                </select>
                {errosCampos.idCondicaoJogo && (
                  <div id="idCondicaoJogo-erro" className="invalid-feedback text-danger-emphasis">
                    {errosCampos.idCondicaoJogo}
                  </div>
                )}
              </div>
            </div>

            <fieldset className="mb-3">
              <legend className="form-label fs-6">Gêneros</legend>
              <div className={'border rounded p-3' + (errosCampos.idsGeneros ? ' border-danger' : '')}
                aria-describedby={errosCampos.idsGeneros ? 'idsGeneros-erro' : undefined}>
                <div className="row g-2">
                  {opcoes.generos.map(genero => (
                    <div className="col-6 col-sm-4" key={genero.id}>
                      <div className="form-check">
                        <input id={`genero-${genero.id}`} type="checkbox" className="form-check-input"
                          checked={formulario.idsGeneros.includes(genero.id)}
                          onChange={() => alternarGenero(genero.id)} />
                        <label htmlFor={`genero-${genero.id}`} className="form-check-label">
                          {genero.nome}
                        </label>
                      </div>
                    </div>
                  ))}
                </div>
              </div>
              {errosCampos.idsGeneros && (
                <div id="idsGeneros-erro" className="invalid-feedback d-block text-danger-emphasis">
                  {errosCampos.idsGeneros}
                </div>
              )}
            </fieldset>

            {ehEdicao && (
              <div className="form-check mb-3">
                <input id="ativo" name="ativo" type="checkbox" className="form-check-input"
                  checked={formulario.ativo} onChange={alterarCampo} />
                <label htmlFor="ativo" className="form-check-label">
                  Jogo ativo (visível no catálogo público)
                </label>
              </div>
            )}

            <div className="d-flex gap-2">
              <button type="submit" className="btn btn-primary">
                {salvando ? 'Salvando...' : 'Salvar'}
              </button>
              <Link to="/admin/jogos" className="btn btn-secondary">Cancelar</Link>
            </div>
          </fieldset>
        </form>
      </div>
    </div>
  )
}
