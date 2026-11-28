import { useEffect, useState } from 'react'
import { carrinhoContext } from './carrinhoContext.js'
import { adicionarItem, alterarQuantidadeItem, carregarItens, removerItem } from '../lib/carrinho.js'

export default function CarrinhoProvider({ children }) {
  const [itens, setItens] = useState(carregarItens)

  useEffect(() => {
    try {
      localStorage.setItem('retrogames.carrinho', JSON.stringify(itens))
    } catch (erro) {
      console.error('Não foi possível salvar o carrinho.', erro)
    }
  }, [itens])

  function adicionar(jogo) {
    setItens(anteriores => adicionarItem(anteriores, jogo))
  }

  function alterarQuantidade(idJogo, quantidade) {
    setItens(anteriores => alterarQuantidadeItem(anteriores, idJogo, quantidade))
  }

  function remover(idJogo) {
    setItens(anteriores => removerItem(anteriores, idJogo))
  }

  const valorTotal = itens.reduce((total, item) => total + Math.round(item.preco * 100) * item.quantidade, 0) / 100
  const quantidadeTotal = itens.reduce((total, item) => total + item.quantidade, 0)

  return (
    <carrinhoContext.Provider value={{ itens, adicionar, alterarQuantidade, remover, quantidadeTotal, valorTotal }}>
      {children}
    </carrinhoContext.Provider>
  )
}
