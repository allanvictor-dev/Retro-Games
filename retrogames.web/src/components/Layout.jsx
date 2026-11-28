import { NavLink, Outlet, useNavigate } from 'react-router-dom'
import { useAutenticacao } from '../hooks/useAutenticacao.js'

export default function Layout() {
  const { usuario, carregando, estaAutenticado, ehAdministrador, sair } = useAutenticacao()
  const navigate = useNavigate()

  async function encerrarSessao() {
    try {
      await sair()
    } catch (erro) {
      console.error('Não foi possível encerrar a sessão.', erro)
    }
    navigate('/')
  }

  return (
    <div className="d-flex flex-column min-vh-100">
      <nav className="navbar navbar-expand-lg navbar-dark bg-dark">
        <div className="container">
          <NavLink className="navbar-brand" to="/">RetroGames</NavLink>
          <button className="navbar-toggler" type="button" data-bs-toggle="collapse"
            data-bs-target="#menuPrincipal" aria-controls="menuPrincipal"
            aria-expanded="false" aria-label="Abrir menu">
            <span className="navbar-toggler-icon"></span>
          </button>
          <div className="collapse navbar-collapse" id="menuPrincipal">
            <div className="navbar-nav">
              <NavLink className="nav-link" to="/">Catálogo</NavLink>
              {ehAdministrador && (
                <>
                  <NavLink className="nav-link" to="/admin/jogos">Administração</NavLink>
                  <NavLink className="nav-link" to="/admin/plataformas">Plataformas</NavLink>
                  <NavLink className="nav-link" to="/admin/generos">Gêneros</NavLink>
                </>
              )}
            </div>
            {/* Enquanto a sessão é consultada, não se sabe se mostra Entrar ou Sair. */}
            {!carregando && (
              <div className="navbar-nav ms-lg-auto align-items-lg-center">
                {estaAutenticado ? (
                  <>
                    <span className="navbar-text text-white text-truncate me-lg-3"
                      style={{ maxWidth: '15rem' }} title={usuario.nome}>{usuario.nome}</span>
                    <button type="button"
                      className="btn btn-outline-light btn-sm my-2 my-lg-0 align-self-start align-self-lg-auto"
                      onClick={encerrarSessao}>Sair</button>
                  </>
                ) : (
                  <NavLink className="nav-link" to="/login">Entrar</NavLink>
                )}
              </div>
            )}
          </div>
        </div>
      </nav>
      <main className="container py-4 flex-grow-1">
        <Outlet />
      </main>
      <footer className="bg-dark text-light text-center py-3">
        <div className="container">RetroGames</div>
      </footer>
    </div>
  )
}
