import { useEffect, useState } from 'react'
import { autenticacaoApi } from '../api/servicos.js'
import { autenticacaoContext } from './autenticacaoContext.js'

export default function AutenticacaoProvider({ children }) {
  const [usuario, setUsuario] = useState(null)
  const [carregando, setCarregando] = useState(true)

  useEffect(() => {
    let ativo = true

    function limparSessao() {
      setUsuario(null)
    }

    async function carregarUsuario() {
      try {
        const dados = await autenticacaoApi.usuarioAtual()
        if (ativo) setUsuario(dados)
      } catch (erro) {
        if (ativo) {
          setUsuario(null)
          if (erro.response?.status !== 401) {
            console.error('Não foi possível consultar a sessão.', erro)
          }
        }
      } finally {
        if (ativo) setCarregando(false)
      }
    }

    window.addEventListener('sessao:expirada', limparSessao)
    carregarUsuario()

    return () => {
      ativo = false
      window.removeEventListener('sessao:expirada', limparSessao)
    }
  }, [])

  async function entrar(email, senha) {
    const dados = await autenticacaoApi.entrar(email, senha)
    setUsuario(dados)
    return dados
  }

  async function sair() {
    await autenticacaoApi.sair()
    setUsuario(null)
  }

  return (
    <autenticacaoContext.Provider value={{
      usuario,
      carregando,
      estaAutenticado: usuario !== null,
      ehAdministrador: usuario?.perfil === 'Administrador',
      entrar,
      sair,
    }}>
      {children}
    </autenticacaoContext.Provider>
  )
}
