import { useEffect, useState } from 'react'
import { useAutenticacao } from '../hooks/useAutenticacao.js'
import { carrinhoContext } from './carrinhoContext.js'
import { adicionarItem, alterarQuantidadeItem, carregarItens, removerItem } from '../lib/carrinho.js'

export default function CarrinhoProvider({ children }) {
  const { usuario, carregando } = useAutenticacao()
  const chave = !carregando && usuario?.id != null ? 'retrogames.carrinho.' + usuario.id : null

  return <CarrinhoDaConta key={chave || 'visitante'} chave={chave}>{children}</CarrinhoDaConta>
}

function CarrinhoDaConta({ children, chave }) {
  const [itens, setItens] = useState(() => carregarItens(chave))

  useEffect(() => {
    if (!chave) return
    try {
      localStorage.setItem(chave, JSON.stringify(itens))
    } catch (erro) {
      console.error('Não foi possível salvar o carrinho.', erro)
    }
  }, [chave, itens])

  function adicionar(jogo) {
    if (!chave) return
    setItens(anteriores => adicionarItem(anteriores, jogo))
  }

  function alterarQuantidade(idJogo, quantidade) {
    if (!chave) return
    setItens(anteriores => alterarQuantidadeItem(anteriores, idJogo, quantidade))
  }

  function remover(idJogo) {
    if (!chave) return
    setItens(anteriores => removerItem(anteriores, idJogo))
  }

  function limpar() {
    if (!chave) return
    setItens([])
    try {
      localStorage.setItem(chave, '[]')
    } catch (erro) {
      console.error('Não foi possível limpar o carrinho salvo.', erro)
    }
  }

  const valorTotal = itens.reduce((total, item) => total + Math.round(item.preco * 100) * item.quantidade, 0) / 100
  const quantidadeTotal = itens.reduce((total, item) => total + item.quantidade, 0)

  return (
    <carrinhoContext.Provider value={{ itens, adicionar, alterarQuantidade, remover, limpar, quantidadeTotal, valorTotal }}>
      {children}
    </carrinhoContext.Provider>
  )
}
