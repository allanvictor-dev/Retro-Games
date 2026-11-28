import { useState } from 'react'
import { Link, Navigate } from 'react-router-dom'
import { obterMensagemErro } from '../../api/erros.js'
import { Carregando } from '../../components/MensagensEstado.jsx'
import { useAutenticacao } from '../../hooks/useAutenticacao.js'

const CODIGO_EMAIL_JA_CADASTRADO = 106
const MENSAGEM_EMAIL_JA_CADASTRADO = 'Já existe uma conta com este e-mail. Tente entrar ou use outro e-mail.'

// Mesmas regras do CadastrarUsuarioRequestValidator do back-end.
function validar({ nome, email, senha }) {
  const erros = {}
  const nomeLimpo = nome.trim()
  const emailLimpo = email.trim()

  if (!nomeLimpo) erros.nome = 'Informe o seu nome.'
  else if (nomeLimpo.length > 120) erros.nome = 'O nome deve ter no máximo 120 caracteres.'

  if (!emailLimpo) erros.email = 'Informe o seu e-mail.'
  else if (!/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(emailLimpo)) erros.email = 'Informe um e-mail válido.'
  else if (emailLimpo.length > 255) erros.email = 'O e-mail deve ter no máximo 255 caracteres.'

  if (!senha) erros.senha = 'Informe uma senha.'
  else if (senha.length < 6) erros.senha = 'A senha deve ter no mínimo 6 caracteres.'
  else if (senha.length > 100) erros.senha = 'A senha deve ter no máximo 100 caracteres.'

  return erros
}

export default function Cadastro() {
  const { carregando, estaAutenticado, cadastrar } = useAutenticacao()
  const [nome, setNome] = useState('')
  const [email, setEmail] = useState('')
  const [senha, setSenha] = useState('')
  const [errosCampos, setErrosCampos] = useState({})
  const [erro, setErro] = useState('')
  const [enviando, setEnviando] = useState(false)

  if (carregando) {
    return <Carregando />
  }

  // Quem já tem sessão vai para o catálogo. Isso também leva o cliente recém-cadastrado
  // para "/": cadastrar() preenche o usuário no contexto e a página renderiza de novo.
  if (estaAutenticado) {
    return <Navigate to="/" replace />
  }

  async function enviar(evento) {
    evento.preventDefault()
    setErro('')

    const erros = validar({ nome, email, senha })
    setErrosCampos(erros)
    if (Object.keys(erros).length > 0) return

    setEnviando(true)
    try {
      await cadastrar(nome.trim(), email.trim(), senha)
    } catch (erroCadastro) {
      if (erroCadastro.response?.data?.responseCode === CODIGO_EMAIL_JA_CADASTRADO) {
        setErrosCampos({ email: MENSAGEM_EMAIL_JA_CADASTRADO })
      } else {
        setErro(obterMensagemErro(erroCadastro))
      }
      setEnviando(false)
    }
  }

  return (
    <div className="row justify-content-center">
      <div className="col-12 col-sm-8 col-md-6 col-lg-4">
        <h1 className="mb-4">Criar conta</h1>
        {erro && <div className="alert alert-danger" role="alert">{erro}</div>}
        <form onSubmit={enviar} noValidate>
          <div className="mb-3">
            <label htmlFor="nome" className="form-label">Nome</label>
            <input id="nome" type="text" autoComplete="name"
              className={'form-control' + (errosCampos.nome ? ' is-invalid' : '')}
              aria-invalid={errosCampos.nome ? 'true' : undefined}
              aria-describedby={errosCampos.nome ? 'nome-erro' : undefined}
              value={nome} onChange={evento => setNome(evento.target.value)} />
            {errosCampos.nome && <div id="nome-erro" className="invalid-feedback text-danger-emphasis">{errosCampos.nome}</div>}
          </div>
          <div className="mb-3">
            <label htmlFor="email" className="form-label">E-mail</label>
            <input id="email" type="email" autoComplete="email"
              className={'form-control' + (errosCampos.email ? ' is-invalid' : '')}
              aria-invalid={errosCampos.email ? 'true' : undefined}
              aria-describedby={errosCampos.email ? 'email-erro' : undefined}
              value={email} onChange={evento => setEmail(evento.target.value)} />
            {errosCampos.email && <div id="email-erro" className="invalid-feedback text-danger-emphasis">{errosCampos.email}</div>}
          </div>
          <div className="mb-3">
            <label htmlFor="senha" className="form-label">Senha</label>
            <input id="senha" type="password" autoComplete="new-password"
              className={'form-control' + (errosCampos.senha ? ' is-invalid' : '')}
              aria-invalid={errosCampos.senha ? 'true' : undefined}
              aria-describedby={errosCampos.senha ? 'senha-ajuda senha-erro' : 'senha-ajuda'}
              value={senha} onChange={evento => setSenha(evento.target.value)} />
            <div id="senha-ajuda" className="form-text">Mínimo de 6 caracteres.</div>
            {errosCampos.senha && <div id="senha-erro" className="invalid-feedback text-danger-emphasis">{errosCampos.senha}</div>}
          </div>
          <button type="submit" className="btn btn-primary w-100" disabled={enviando}>
            {enviando ? 'Criando conta...' : 'Criar conta'}
          </button>
        </form>
        <p className="mt-3 text-center">
          Já tem conta? <Link to="/login">Entrar</Link>
        </p>
      </div>
    </div>
  )
}
