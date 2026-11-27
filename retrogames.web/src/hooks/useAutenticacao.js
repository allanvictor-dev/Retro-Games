import { useContext } from 'react'
import { autenticacaoContext } from '../contexts/autenticacaoContext.js'

export function useAutenticacao() {
  return useContext(autenticacaoContext)
}
