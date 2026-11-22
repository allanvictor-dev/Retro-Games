import { Link } from 'react-router-dom'
import { formatarPreco } from '../../lib/formato.js'

export default function CardJogo({ jogo }) {
  return (
    <article className="card h-100">
      <div className="card-body d-flex flex-column">
        <div><span className="badge text-bg-secondary mb-2">{jogo.nomePlataforma}</span></div>
        <h2 className="h5 card-title">{jogo.nome}</h2>
        <p className="text-muted mb-2">{jogo.anoLancamento} · {jogo.descricaoCondicao}</p>
        <p className="mb-2">{jogo.generos.join(', ')}</p>
        <p className="fw-bold fs-5 mb-2">{formatarPreco(jogo.preco)}</p>
        {jogo.estoque <= 0 && <p className="text-danger">Sem estoque</p>}
        <Link className="btn btn-outline-primary mt-auto" to={`/jogos/${jogo.id}`}>Ver detalhes</Link>
      </div>
    </article>
  )
}
