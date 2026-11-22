import { useEffect, useState } from 'react'
import { Link, useParams } from 'react-router-dom'
import { jogoApi } from '../../api/servicos.js'
import { obterMensagemErro } from '../../api/erros.js'
import { formatarPreco } from '../../lib/formato.js'
import { Carregando, MensagemErro } from '../../components/MensagensEstado.jsx'

export default function DetalheJogo() {
  const { id } = useParams()
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
          </div>
        </article>
      )}
    </>
  )
}
