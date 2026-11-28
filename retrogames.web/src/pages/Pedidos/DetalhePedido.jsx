import { useEffect, useState } from 'react'
import { Link, useLocation, useParams } from 'react-router-dom'
import { pedidoApi } from '../../api/servicos.js'
import { obterMensagemErro } from '../../api/erros.js'
import { useAutenticacao } from '../../hooks/useAutenticacao.js'
import { Carregando, MensagemErro } from '../../components/MensagensEstado.jsx'
import { formatarData, formatarPreco } from '../../lib/formato.js'

export default function DetalhePedido() {
  const { id } = useParams()
  return <ConsultaPedido key={id} id={id} />
}

function ConsultaPedido({ id }) {
  const { state } = useLocation()
  const { usuario } = useAutenticacao()
  const compraRealizada = state?.compraRealizada && state.idUsuario === usuario?.id
  const [pedido, setPedido] = useState(null)
  const [carregando, setCarregando] = useState(true)
  const [erro, setErro] = useState('')
  const [podeTentarNovamente, setPodeTentarNovamente] = useState(true)
  const [tentativa, setTentativa] = useState(0)

  useEffect(() => {
    let ativo = true

    async function consultar() {
      try {
        if (!/^\d+$/.test(id) || Number(id) < 1 || Number(id) > 2147483647) {
          if (ativo) {
            setErro('Pedido não encontrado.')
            setPodeTentarNovamente(false)
          }
          return
        }

        const dados = await pedidoApi.buscarPorId(id)
        if (ativo) setPedido(dados)
      } catch (erroApi) {
        if (!ativo) return
        const status = erroApi.response?.status
        if (status === 403) {
          setErro('Você não tem permissão para visualizar este pedido.')
          setPodeTentarNovamente(false)
        } else if (status === 404) {
          setErro('Pedido não encontrado.')
          setPodeTentarNovamente(false)
        } else {
          setErro(obterMensagemErro(erroApi))
        }
      } finally {
        if (ativo) setCarregando(false)
      }
    }

    consultar()
    return () => { ativo = false }
  }, [id, tentativa])

  function recarregar() {
    setCarregando(true)
    setErro('')
    setTentativa(valor => valor + 1)
  }

  return (
    <>
      <h1 className="mb-4">Detalhe do pedido</h1>
      {carregando ? <Carregando /> : erro ? (
        <MensagemErro mensagem={erro} tentarNovamente={podeTentarNovamente ? recarregar : undefined} />
      ) : pedido && (
        <>
          {compraRealizada && <div className="alert alert-success" role="status">Compra realizada com sucesso!</div>}
          <h2 className="h5">Pedido #{pedido.id}</h2>
          <p><strong>Data:</strong> {formatarData(pedido.data)}</p>
          <p><strong>Situação:</strong> {pedido.descricaoSituacao}</p>
          <div className="table-responsive">
            <table className="table table-striped align-middle">
              <thead>
                <tr>
                  <th scope="col">Jogo</th>
                  <th scope="col">Quantidade</th>
                  <th scope="col">Valor unitário pago</th>
                  <th scope="col">Subtotal</th>
                </tr>
              </thead>
              <tbody>
                {pedido.itens.map(item => (
                  <tr key={item.idJogo}>
                    <td>{item.nomeJogo}</td>
                    <td>{item.quantidade}</td>
                    <td className="text-nowrap">{formatarPreco(item.valorUnitario)}</td>
                    <td className="text-nowrap">{formatarPreco(item.subtotal)}</td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
          <p className="fs-4 fw-bold text-end">Total: {formatarPreco(pedido.valorTotal)}</p>
        </>
      )}
      <Link className="btn btn-outline-secondary" to="/meus-pedidos">Voltar aos meus pedidos</Link>
    </>
  )
}
