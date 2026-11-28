import { useContext } from 'react'
import { carrinhoContext } from '../contexts/carrinhoContext.js'

export function useCarrinho() {
  return useContext(carrinhoContext)
}
