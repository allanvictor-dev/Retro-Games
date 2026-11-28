import { useState } from 'react'
import { Link, Navigate, useLocation } from 'react-router-dom'
import { obterMensagemErro } from '../../api/erros.js'
import { Carregando } from '../../components/MensagensEstado.jsx'
import { useAutenticacao } from '../../hooks/useAutenticacao.js'

// A página que mandou o visitante para o login guarda o caminho em state.de.
// Aceita o caminho em texto ('/admin/jogos') ou o objeto location do React Router.
function obterDestino(estado) {
  const de = estado?.de
  const caminho = typeof de === 'string' ? de : de?.pathname
  return caminho?.startsWith('/') ? caminho : '/'
}

export default function Login() {
  const { carregando, estaAutenticado, entrar } = useAutenticacao()
  const location = useLocation()
  const [email, setEmail] = useState('')
  const [senha, setSenha] = useState('')
  const [erro, setErro] = useState('')
  const [enviando, setEnviando] = useState(false)

  if (carregando) {
    return <Carregando />
  }

  // Quem já tem sessão não precisa do formulário. É também este redirecionamento
  // que leva ao destino depois do login: entrar() preenche o usuário no contexto,
  // a página renderiza de novo e cai aqui.
  if (estaAutenticado) {
    return <Navigate to={obterDestino(location.state)} replace />
  }

  async function enviar(evento) {
    evento.preventDefault()
    setErro('')
    setEnviando(true)
    try {
      await entrar(email, senha)
    } catch (erroLogin) {
      setErro(obterMensagemErro(erroLogin))
      setEnviando(false)
    }
  }

  return (
    <div className="row justify-content-center">
      <div className="col-12 col-sm-8 col-md-6 col-lg-4">
        <h1 className="mb-4">Entrar</h1>
        {erro && <div className="alert alert-danger" role="alert">{erro}</div>}
        <form onSubmit={enviar}>
          <div className="mb-3">
            <label htmlFor="email" className="form-label">E-mail</label>
            <input id="email" type="email" className="form-control" autoComplete="email"
              required value={email} onChange={evento => setEmail(evento.target.value)} />
          </div>
          <div className="mb-3">
            <label htmlFor="senha" className="form-label">Senha</label>
            <input id="senha" type="password" className="form-control" autoComplete="current-password"
              required value={senha} onChange={evento => setSenha(evento.target.value)} />
          </div>
          <button type="submit" className="btn btn-primary w-100" disabled={enviando}>
            {enviando ? 'Entrando...' : 'Entrar'}
          </button>
        </form>
        <p className="mt-3 text-center">
          Não tem conta? <Link to="/cadastro">Criar conta</Link>
        </p>
      </div>
    </div>
  )
}
