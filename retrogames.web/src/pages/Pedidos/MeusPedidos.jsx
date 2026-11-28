import { useEffect, useState } from 'react'
import { Link } from 'react-router-dom'
import { pedidoApi } from '../../api/servicos.js'
import { obterMensagemErro } from '../../api/erros.js'
import { Carregando, EstadoVazio, MensagemErro } from '../../components/MensagensEstado.jsx'
import { formatarData, formatarPreco } from '../../lib/formato.js'

export default function MeusPedidos() {
  const [pedidos, setPedidos] = useState([])
  const [carregando, setCarregando] = useState(true)
  const [erro, setErro] = useState('')
  const [tentativa, setTentativa] = useState(0)

  useEffect(() => {
    let ativo = true
    pedidoApi.listarMeus()
      .then(dados => { if (ativo) setPedidos(dados) })
      .catch(erroApi => { if (ativo) setErro(obterMensagemErro(erroApi)) })
      .finally(() => { if (ativo) setCarregando(false) })
    return () => { ativo = false }
  }, [tentativa])

  function recarregar() {
    setCarregando(true)
    setErro('')
    setTentativa(valor => valor + 1)
  }

  return (
    <>
      <h1 className="mb-4">Meus pedidos</h1>
      {carregando ? <Carregando /> : erro ? (
        <MensagemErro mensagem={erro} tentarNovamente={recarregar} />
      ) : pedidos.length === 0 ? (
        <>
          <EstadoVazio mensagem="Você ainda não possui pedidos." />
          <Link className="btn btn-primary" to="/">Voltar ao catálogo</Link>
        </>
      ) : (
        <div className="row g-3">
          {pedidos.map(pedido => (
            <div key={pedido.id} className="col-12 col-md-6">
              <article className="card h-100">
                <div className="card-body d-flex flex-column">
                  <h2 className="h5">Pedido #{pedido.id}</h2>
                  <p className="mb-1"><strong>Data:</strong> {formatarData(pedido.data)}</p>
                  <p className="mb-1"><strong>Situação:</strong> {pedido.descricaoSituacao}</p>
                  <p className="mb-2"><strong>Total:</strong> {formatarPreco(pedido.valorTotal)}</p>
                  <p className="mb-1 fw-semibold">Itens</p>
                  <ul className="mb-3">
                    {pedido.itens.map(item => (
                      <li key={item.idJogo}>{item.nomeJogo} — {item.quantidade} un.</li>
                    ))}
                  </ul>
                  <Link className="btn btn-outline-primary align-self-start mt-auto"
                    to={'/meus-pedidos/' + pedido.id}>Ver detalhes</Link>
                </div>
              </article>
            </div>
          ))}
        </div>
      )}
    </>
  )
}
