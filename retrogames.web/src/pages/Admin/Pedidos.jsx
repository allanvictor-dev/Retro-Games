import { Fragment, useEffect, useState } from 'react'
import { useSearchParams } from 'react-router-dom'
import { obterMensagemErro } from '../../api/erros.js'
import { pedidoApi, situacaoPedidoApi } from '../../api/servicos.js'
import { Carregando, EstadoVazio, MensagemErro } from '../../components/MensagensEstado.jsx'
import { formatarDataHora, formatarPreco } from '../../lib/formato.js'

function classeSituacao(descricao) {
  const texto = (descricao || '').toLowerCase()
  if (texto.includes('cancel')) return 'badge text-bg-danger'
  if (texto.includes('entreg')) return 'badge text-bg-success'
  if (texto.includes('envi')) return 'badge text-bg-info'
  if (texto.includes('pago') || texto.includes('aprov') || texto.includes('confirm')) {
    return 'badge text-bg-primary'
  }
  return 'badge text-bg-secondary'
}

export default function Pedidos() {
  const [parametros, setParametros] = useSearchParams()
  const idSituacaoFiltro = parametros.get('idSituacao') || ''

  const [situacoes, setSituacoes] = useState([])
  const [pedidos, setPedidos] = useState([])
  const [carregando, setCarregando] = useState(true)
  const [erroLista, setErroLista] = useState('')
  const [erroAcao, setErroAcao] = useState('')
  const [sucesso, setSucesso] = useState('')
  const [salvandoId, setSalvandoId] = useState(null)
  const [pedidoAbertoId, setPedidoAbertoId] = useState(null)
  const [tentativa, setTentativa] = useState(0)

  useEffect(() => {
    let ativo = true
    situacaoPedidoApi.listar()
      .then(dados => { if (ativo) setSituacoes(dados) })
      .catch(erroApi => { if (ativo) setErroAcao(obterMensagemErro(erroApi)) })
    return () => { ativo = false }
  }, [])

  useEffect(() => {
    let ativo = true
    pedidoApi.listar(idSituacaoFiltro || undefined)
      .then(dados => {
        if (ativo) {
          setPedidos(dados)
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
  }, [idSituacaoFiltro, tentativa])

  function recarregar() {
    setCarregando(true)
    setErroLista('')
    setTentativa(valor => valor + 1)
  }

  function alterarFiltro(evento) {
    const valor = evento.target.value
    const novos = new URLSearchParams(parametros)
    if (valor) novos.set('idSituacao', valor)
    else novos.delete('idSituacao')
    setCarregando(true)
    setErroAcao('')
    setSucesso('')
    setParametros(novos)
  }

  async function alterarSituacao(pedido, novoIdTexto) {
    const novoId = Number(novoIdTexto)
    if (!novoId || novoId === pedido.idSituacaoPedido) return

    setSalvandoId(pedido.id)
    setErroAcao('')
    setSucesso('')

    try {
      await pedidoApi.alterarSituacao(pedido.id, novoId)
      const situacaoEscolhida = situacoes.find(item => item.id === novoId)
      const novaDescricao = situacaoEscolhida ? situacaoEscolhida.descricao : ''

      setPedidos(anterior => {
        const atualizados = anterior.map(item =>
          item.id === pedido.id
            ? { ...item, idSituacaoPedido: novoId, descricaoSituacao: novaDescricao }
            : item
        )
        return idSituacaoFiltro
          ? atualizados.filter(item => String(item.idSituacaoPedido) === idSituacaoFiltro)
          : atualizados
      })

      setSucesso(`Situação do pedido #${pedido.id} atualizada para "${novaDescricao}".`)
    } catch (erroApi) {
      setErroAcao(obterMensagemErro(erroApi))
    } finally {
      setSalvandoId(null)
    }
  }

  function alternarDetalhe(idPedido) {
    setPedidoAbertoId(atual => (atual === idPedido ? null : idPedido))
  }

  return (
    <>
      <h1 className="mb-4">Gestão de pedidos</h1>

      {erroAcao && <MensagemErro mensagem={erroAcao} />}
      {sucesso && <div className="alert alert-success" role="status">{sucesso}</div>}

      <div className="row g-3 mb-4">
        <div className="col-12 col-sm-6 col-md-4">
          <label htmlFor="filtro-situacao" className="form-label">Filtrar por situação</label>
          <select id="filtro-situacao" className="form-select"
            value={idSituacaoFiltro} onChange={alterarFiltro}>
            <option value="">Todas as situações</option>
            {situacoes.map(situacao => (
              <option key={situacao.id} value={situacao.id}>{situacao.descricao}</option>
            ))}
          </select>
        </div>
      </div>

      {carregando ? <Carregando /> : erroLista ? (
        <MensagemErro mensagem={erroLista} tentarNovamente={recarregar} />
      ) : pedidos.length === 0 ? (
        <EstadoVazio mensagem="Nenhum pedido encontrado." />
      ) : (
        <div className="table-responsive">
          <table className="table table-striped align-middle">
            <thead>
              <tr>
                <th>Pedido</th>
                <th>Data</th>
                <th>Cliente</th>
                <th>Total</th>
                <th>Situação</th>
                <th>Alterar situação</th>
                <th>Detalhes</th>
              </tr>
            </thead>
            <tbody>
              {pedidos.map(pedido => {
                const aberto = pedidoAbertoId === pedido.id
                const salvando = salvandoId === pedido.id
                return (
                  <Fragment key={pedido.id}>
                    <tr>
                      <td className="fw-semibold">#{pedido.id}</td>
                      <td>{formatarDataHora(pedido.data)}</td>
                      <td>
                        <div>{pedido.nomeUsuario}</div>
                        <small className="text-body-secondary">{pedido.emailUsuario}</small>
                      </td>
                      <td>{formatarPreco(pedido.valorTotal)}</td>
                      <td>
                        <span className={classeSituacao(pedido.descricaoSituacao)}>
                          {pedido.descricaoSituacao}
                        </span>
                      </td>
                      <td>
                        <div className="d-flex align-items-center gap-2">
                          <select
                            aria-label={`Situação do pedido #${pedido.id}`}
                            className="form-select form-select-sm"
                            style={{ minWidth: '10rem' }}
                            disabled={salvando || situacoes.length === 0}
                            value={pedido.idSituacaoPedido}
                            onChange={evento => alterarSituacao(pedido, evento.target.value)}
                          >
                            {situacoes.map(situacao => (
                              <option key={situacao.id} value={situacao.id}>
                                {situacao.descricao}
                              </option>
                            ))}
                          </select>
                          {salvando && <span className="small text-body-secondary">Salvando...</span>}
                        </div>
                      </td>
                      <td>
                        <button type="button" className="btn btn-outline-primary btn-sm"
                          aria-expanded={aberto}
                          onClick={() => alternarDetalhe(pedido.id)}>
                          {aberto ? 'Ocultar itens' : 'Ver itens'}
                        </button>
                      </td>
                    </tr>
                    {aberto && (
                      <tr>
                        <td colSpan={7} className="bg-body-tertiary">
                          <p className="fw-semibold mb-2">
                            Itens do pedido #{pedido.id} ({pedido.itens.length})
                          </p>
                          <div className="table-responsive">
                            <table className="table table-sm mb-0">
                              <thead>
                                <tr>
                                  <th>Jogo</th>
                                  <th>Quantidade</th>
                                  <th>Valor unitário</th>
                                  <th>Subtotal</th>
                                </tr>
                              </thead>
                              <tbody>
                                {pedido.itens.map(item => (
                                  <tr key={item.idJogo}>
                                    <td>{item.nomeJogo}</td>
                                    <td>{item.quantidade}</td>
                                    <td>{formatarPreco(item.valorUnitario)}</td>
                                    <td>{formatarPreco(item.subtotal)}</td>
                                  </tr>
                                ))}
                              </tbody>
                            </table>
                          </div>
                        </td>
                      </tr>
                    )}
                  </Fragment>
                )
              })}
            </tbody>
          </table>
        </div>
      )}
    </>
  )
}
