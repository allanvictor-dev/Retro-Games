import { Link } from 'react-router-dom'

export default function NaoEncontrada() {
  return (
    <div>
      <h1>Página não encontrada</h1>
      <p>O endereço informado não existe.</p>
      <Link to="/" className="btn btn-primary mt-3">Voltar ao catálogo</Link>
    </div>
  )
}
