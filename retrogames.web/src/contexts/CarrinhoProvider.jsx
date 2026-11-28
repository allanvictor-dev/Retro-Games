import { useEffect, useState } from 'react'
import { carrinhoContext } from './carrinhoContext.js'
import { adicionarItem, carregarItens } from '../lib/carrinho.js'

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

  const quantidadeTotal = itens.reduce((total, item) => total + item.quantidade, 0)

  return (
    <carrinhoContext.Provider value={{ itens, adicionar, quantidadeTotal }}>
      {children}
    </carrinhoContext.Provider>
  )
}
