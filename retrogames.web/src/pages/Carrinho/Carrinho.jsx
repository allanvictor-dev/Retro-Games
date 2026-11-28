import { useRef, useState } from 'react'
import { Link, useNavigate } from 'react-router-dom'
import { pedidoApi } from '../../api/servicos.js'
import { obterMensagemErro } from '../../api/erros.js'
import { useAutenticacao } from '../../hooks/useAutenticacao.js'
import { useCarrinho } from '../../hooks/useCarrinho.js'
import { formatarPreco } from '../../lib/formato.js'
import { QUANTIDADE_MAXIMA } from '../../lib/carrinho.js'
import { EstadoVazio } from '../../components/MensagensEstado.jsx'

export default function Carrinho() {
  const { itens, alterarQuantidade, remover, limpar, valorTotal } = useCarrinho()
  const { usuario } = useAutenticacao()
  const navigate = useNavigate()
  const [enviando, setEnviando] = useState(false)
  const [erro, setErro] = useState('')
  const envioEmAndamento = useRef(false)

  async function finalizarCompra() {
    if (envioEmAndamento.current || itens.length === 0) return

    envioEmAndamento.current = true
    setEnviando(true)
    setErro('')

    try {
      const pedido = await pedidoApi.inserir(itens.map(({ idJogo, quantidade }) => ({
        idJogo, quantidade,
      })))
      limpar()
      navigate('/meus-pedidos/' + pedido.id, {
        replace: true,
        state: { compraRealizada: true, idUsuario: usuario.id },
      })
    } catch (erroCompra) {
      const codigo = erroCompra.response?.data?.responseCode
      if (codigo === 110) {
        setErro('Um jogo do carrinho não está mais disponível. Confira os jogos e remova o item indisponível antes de tentar novamente.')
      } else if (codigo === 111) {
        setErro('Estoque insuficiente para um dos jogos. Confira o estoque no detalhe do jogo e reduza a quantidade antes de tentar novamente.')
      } else {
        setErro(obterMensagemErro(erroCompra))
      }
    } finally {
      envioEmAndamento.current = false
      setEnviando(false)
    }
  }

  return (
    <>
      <h1 className="mb-4">Carrinho</h1>
      {erro && <div className="alert alert-danger" role="alert">{erro}</div>}
      {itens.length === 0 ? (
        <>
          <EstadoVazio mensagem="Seu carrinho está vazio." />
          <Link className="btn btn-primary" to="/">Voltar ao catálogo</Link>
        </>
      ) : (
        <>
          <div className="table-responsive">
            <table className="table table-striped align-middle">
              <thead>
                <tr>
                  <th scope="col">Jogo</th>
                  <th scope="col">Preço unitário</th>
                  <th scope="col">Quantidade</th>
                  <th scope="col">Subtotal</th>
                  <th scope="col">Ações</th>
                </tr>
              </thead>
              <tbody>
                {itens.map(item => (
                  <tr key={item.idJogo}>
                    <td><Link to={'/jogos/' + item.idJogo}>{item.nome}</Link></td>
                    <td className="text-nowrap">{formatarPreco(item.preco)}</td>
                    <td>
                      <select className="form-select w-auto" value={item.quantidade} disabled={enviando}
                        aria-label={'Quantidade de ' + item.nome}
                        onChange={evento => alterarQuantidade(item.idJogo, Number(evento.target.value))}>
                        {Array.from({ length: QUANTIDADE_MAXIMA }, (_, indice) => indice + 1)
                          .map(quantidade => <option key={quantidade} value={quantidade}>{quantidade}</option>)}
                      </select>
                    </td>
                    <td className="text-nowrap">{formatarPreco(item.preco * item.quantidade)}</td>
                    <td>
                      <button type="button" className="btn btn-outline-danger btn-sm" disabled={enviando}
                        aria-label={'Remover ' + item.nome}
                        onClick={() => remover(item.idJogo)}>Remover</button>
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
          <p className="fs-4 fw-bold text-end" aria-live="polite">Total: {formatarPreco(valorTotal)}</p>
          <div className="d-flex flex-wrap gap-2 justify-content-between">
            {enviando ? (
              <button type="button" className="btn btn-outline-secondary" disabled>Continuar comprando</button>
            ) : (
              <Link className="btn btn-outline-secondary" to="/">Continuar comprando</Link>
            )}
            <button type="button" className="btn btn-primary" disabled={enviando}
              onClick={finalizarCompra}>
              {enviando ? 'Finalizando...' : 'Finalizar compra'}
            </button>
          </div>
        </>
      )}
    </>
  )
}
