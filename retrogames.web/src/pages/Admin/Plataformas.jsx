import { useEffect, useState } from 'react'
import { plataformaApi } from '../../api/servicos.js'
import { obterMensagemErro, obterErrosDosCampos } from '../../api/erros.js'
import { Carregando, MensagemErro, EstadoVazio } from '../../components/MensagensEstado.jsx'

const formularioVazio = { nome: '', fabricante: '', anoLancamento: '' }

export default function Plataformas() {
  const [plataformas, setPlataformas] = useState([])
  const [formulario, setFormulario] = useState(formularioVazio)
  const [idEdicao, setIdEdicao] = useState(null)
  const [carregando, setCarregando] = useState(true)
  const [ocupado, setOcupado] = useState(false)
  const [erroLista, setErroLista] = useState('')
  const [erro, setErro] = useState('')
  const [errosCampos, setErrosCampos] = useState({})
  const [sucesso, setSucesso] = useState('')

  async function carregarPlataformas() {
    setCarregando(true)
    setErroLista('')
    try {
      setPlataformas(await plataformaApi.listar())
    } catch (erroApi) {
      setErroLista(obterMensagemErro(erroApi))
    } finally {
      setCarregando(false)
    }
  }

  useEffect(() => {
    let ativo = true
    plataformaApi.listar()
      .then(dados => { if (ativo) setPlataformas(dados) })
      .catch(erroApi => { if (ativo) setErroLista(obterMensagemErro(erroApi)) })
      .finally(() => { if (ativo) setCarregando(false) })
    return () => { ativo = false }
  }, [])

  function limparFormulario() {
    setFormulario(formularioVazio)
    setIdEdicao(null)
    setErro('')
    setErrosCampos({})
  }

  function editar(plataforma) {
    setIdEdicao(plataforma.id)
    setFormulario({
      nome: plataforma.nome,
      fabricante: plataforma.fabricante,
      anoLancamento: String(plataforma.anoLancamento),
    })
    setErro('')
    setErrosCampos({})
    setSucesso('')
  }

  function alterarCampo(evento) {
    const { name, value } = evento.target
    setFormulario(anterior => ({ ...anterior, [name]: value }))
    setErrosCampos(anterior => ({ ...anterior, [name]: '' }))
  }

  async function salvar(evento) {
    evento.preventDefault()
    setOcupado(true)
    setErro('')
    setErrosCampos({})
    setSucesso('')
    const dados = {
      nome: formulario.nome.trim(),
      fabricante: formulario.fabricante.trim(),
      anoLancamento: Number(formulario.anoLancamento),
    }

    try {
      if (idEdicao === null) {
        await plataformaApi.inserir(dados)
        setSucesso('Plataforma cadastrada com sucesso.')
      } else {
        await plataformaApi.alterar(idEdicao, dados)
        setSucesso('Plataforma alterada com sucesso.')
      }
      limparFormulario()
      await carregarPlataformas()
    } catch (erroApi) {
      setErro(obterMensagemErro(erroApi))
      setErrosCampos(obterErrosDosCampos(erroApi))
    } finally {
      setOcupado(false)
    }
  }

  async function excluir(plataforma) {
    if (!window.confirm('Deseja excluir a plataforma ' + plataforma.nome + '?')) return
    setOcupado(true)
    setErro('')
    setErrosCampos({})
    setSucesso('')
    try {
      await plataformaApi.excluir(plataforma.id)
      if (idEdicao === plataforma.id) limparFormulario()
      setSucesso('Plataforma excluída com sucesso.')
      await carregarPlataformas()
    } catch (erroApi) {
      setErro(obterMensagemErro(erroApi))
    } finally {
      setOcupado(false)
    }
  }

  return (
    <>
      <h1 className="mb-4">Gerenciamento de plataformas</h1>
      {erro && <MensagemErro mensagem={erro} />}
      {sucesso && <div className="alert alert-success" role="status">{sucesso}</div>}
      <form className="card card-body mb-4" onSubmit={salvar} noValidate>
        <h2 className="h5 mb-3">{idEdicao === null ? 'Cadastrar plataforma' : 'Editar plataforma'}</h2>
        <fieldset disabled={ocupado}>
          <div className="row g-3">
            {[
              { nome: 'nome', rotulo: 'Nome', tipo: 'text' },
              { nome: 'fabricante', rotulo: 'Fabricante', tipo: 'text' },
              { nome: 'anoLancamento', rotulo: 'Ano de lançamento', tipo: 'number' },
            ].map(campo => (
              <div className="col-md-4" key={campo.nome}>
                <label htmlFor={campo.nome} className="form-label">{campo.rotulo}</label>
                <input id={campo.nome} name={campo.nome} type={campo.tipo}
                  className={'form-control' + (errosCampos[campo.nome] ? ' is-invalid' : '')}
                  value={formulario[campo.nome]} onChange={alterarCampo}
                  aria-invalid={Boolean(errosCampos[campo.nome])}
                  aria-describedby={errosCampos[campo.nome] ? campo.nome + '-erro' : undefined} />
                {errosCampos[campo.nome] && (
                  <div id={campo.nome + '-erro'} className="invalid-feedback">{errosCampos[campo.nome]}</div>
                )}
              </div>
            ))}
          </div>
          <div className="mt-3 d-flex gap-2">
            <button type="submit" className="btn btn-primary">{ocupado ? 'Aguarde...' : 'Salvar'}</button>
            {idEdicao !== null && <button type="button" className="btn btn-secondary" onClick={limparFormulario}>Cancelar</button>}
          </div>
        </fieldset>
      </form>
      {carregando ? <Carregando /> : erroLista ? (
        <MensagemErro mensagem={erroLista} tentarNovamente={carregarPlataformas} />
      ) : plataformas.length === 0 ? (
        <EstadoVazio mensagem="Nenhuma plataforma cadastrada." />
      ) : (
        <div className="table-responsive">
          <table className="table table-striped align-middle">
            <thead>
              <tr><th>Nome</th><th>Fabricante</th><th>Ano de lançamento</th><th>Ações</th></tr>
            </thead>
            <tbody>
              {plataformas.map(plataforma => (
                <tr key={plataforma.id}>
                  <td>{plataforma.nome}</td>
                  <td>{plataforma.fabricante}</td>
                  <td>{plataforma.anoLancamento}</td>
                  <td>
                    <div className="d-flex gap-2">
                      <button type="button" className="btn btn-outline-primary btn-sm" disabled={ocupado}
                        onClick={() => editar(plataforma)}>Editar</button>
                      <button type="button" className="btn btn-outline-danger btn-sm" disabled={ocupado}
                        onClick={() => excluir(plataforma)}>Excluir</button>
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
