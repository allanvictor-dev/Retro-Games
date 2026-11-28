import { useEffect, useState } from 'react'
import { Link, useLocation, useParams } from 'react-router-dom'
import { jogoApi } from '../../api/servicos.js'
import { obterMensagemErro } from '../../api/erros.js'
import { formatarPreco } from '../../lib/formato.js'
import { Carregando, MensagemErro } from '../../components/MensagensEstado.jsx'

import { useAutenticacao } from '../../hooks/useAutenticacao.js'
import { useCarrinho } from '../../hooks/useCarrinho.js'
import { QUANTIDADE_MAXIMA } from '../../lib/carrinho.js'

export default function DetalheJogo() {
  const { id } = useParams()
  const location = useLocation()
  const { estaAutenticado, carregando: carregandoSessao } = useAutenticacao()
  const { itens, adicionar } = useCarrinho()
  const [jogo, setJogo] = useState(null)
  const [carregando, setCarregando] = useState(true)
  const [erro, setErro] = useState('')
  const [tentativa, setTentativa] = useState(0)

  useEffect(() => {
    let ativo = true
    setCarregando(true)
    setErro('')
    jogoApi.buscarPorId(id)
      .then(dados => { if (ativo) setJogo(dados) })
      .catch(erro => { if (ativo) setErro(obterMensagemErro(erro)) })
      .finally(() => { if (ativo) setCarregando(false) })
    return () => { ativo = false }
  }, [id, tentativa])

  const quantidade = itens.find(item => item.idJogo === jogo?.id)?.quantidade || 0
  const esgotado = jogo && (!(jogo.estoque > 0) || jogo.ativo === false)

  return (
    <>
      <Link to="/" className="btn btn-outline-secondary mb-4">Voltar ao catálogo</Link>
      {carregando ? <Carregando /> : erro ? (
        <MensagemErro mensagem={erro} tentarNovamente={() => setTentativa(valor => valor + 1)} />
      ) : jogo && (
        <article className="card">
          <div className="card-body">
            <h1 className="mb-3">{jogo.nome}</h1>
            <p><strong>Plataforma:</strong> {jogo.nomePlataforma}</p>
            <p><strong>Condição:</strong> {jogo.descricaoCondicao}</p>
            <p><strong>Ano de lançamento:</strong> {jogo.anoLancamento}</p>
            <p><strong>Gêneros:</strong> {jogo.generos.map(genero => genero.nome).join(', ')}</p>
            <p><strong>Descrição:</strong> {jogo.descricao || 'Sem descrição.'}</p>
            <p className="fs-4 fw-bold">{formatarPreco(jogo.preco)}</p>
            <p className={jogo.estoque > 0 ? 'text-success' : 'text-danger'}>
              {jogo.estoque > 0 ? `Estoque: ${jogo.estoque}` : 'Sem estoque'}
            </p>
            {!carregandoSessao && !estaAutenticado && !esgotado ? (
              <Link className="btn btn-primary" to="/login" state={{ de: location.pathname }}>
                Entrar para adicionar ao carrinho
              </Link>
            ) : (
              <button type="button" className="btn btn-primary"
                disabled={carregandoSessao || esgotado || quantidade >= QUANTIDADE_MAXIMA}
                onClick={() => adicionar(jogo)}>
                {esgotado ? 'Esgotado' : quantidade >= QUANTIDADE_MAXIMA ? 'Limite de 10 unidades' : 'Adicionar ao carrinho'}
              </button>
            )}
            <p className="mt-2 mb-0" aria-live="polite">No carrinho: {quantidade}</p>
          </div>
        </article>
      )}
    </>
  )
}
