import { Link } from 'react-router-dom'
import { useCarrinho } from '../../hooks/useCarrinho.js'
import { formatarPreco } from '../../lib/formato.js'
import { QUANTIDADE_MAXIMA } from '../../lib/carrinho.js'
import { EstadoVazio } from '../../components/MensagensEstado.jsx'

export default function Carrinho() {
  const { itens, alterarQuantidade, remover, valorTotal } = useCarrinho()

  return (
    <>
      <h1 className="mb-4">Carrinho</h1>
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
                      <select className="form-select w-auto" value={item.quantidade}
                        aria-label={'Quantidade de ' + item.nome}
                        onChange={evento => alterarQuantidade(item.idJogo, Number(evento.target.value))}>
                        {Array.from({ length: QUANTIDADE_MAXIMA }, (_, indice) => indice + 1)
                          .map(quantidade => <option key={quantidade} value={quantidade}>{quantidade}</option>)}
                      </select>
                    </td>
                    <td className="text-nowrap">{formatarPreco(item.preco * item.quantidade)}</td>
                    <td>
                      <button type="button" className="btn btn-outline-danger btn-sm"
                        aria-label={'Remover ' + item.nome}
                        onClick={() => remover(item.idJogo)}>Remover</button>
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
          <p className="fs-4 fw-bold text-end" aria-live="polite">Total: {formatarPreco(valorTotal)}</p>
          <Link className="btn btn-outline-secondary" to="/">Continuar comprando</Link>
        </>
      )}
    </>
  )
}
