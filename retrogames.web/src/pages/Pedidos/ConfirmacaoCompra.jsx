import { Link, useLocation, useParams } from 'react-router-dom'
import { useAutenticacao } from '../../hooks/useAutenticacao.js'
import { formatarPreco } from '../../lib/formato.js'

export default function ConfirmacaoCompra() {
  const { id } = useParams()
  const { state } = useLocation()
  const { usuario } = useAutenticacao()
  const pedido = state?.idUsuario === usuario?.id ? state?.pedidoCriado : null

  return (
    <>
      <h1 className="mb-4">Detalhe do pedido</h1>
      {pedido && String(pedido.id) === id ? (
        <>
          <div className="alert alert-success" role="status">Compra realizada com sucesso!</div>
          <p><strong>Pedido:</strong> #{pedido.id}</p>
          <p><strong>Situação:</strong> {pedido.descricaoSituacao}</p>
          <p><strong>Total:</strong> {formatarPreco(pedido.valorTotal)}</p>
        </>
      ) : (
        <div className="alert alert-secondary" role="status">
          A consulta de pedidos ainda não está disponível.
        </div>
      )}
      <Link className="btn btn-primary" to="/">Voltar ao catálogo</Link>
    </>
  )
}
