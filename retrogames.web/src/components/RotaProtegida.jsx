import { Navigate, Outlet, useLocation } from 'react-router-dom'
import { useAutenticacao } from '../hooks/useAutenticacao.js'
import { Carregando } from './MensagensEstado.jsx'

export default function RotaProtegida() {
  const { carregando, estaAutenticado, ehAdministrador } = useAutenticacao()
  const location = useLocation()

  if (carregando) return <Carregando />
  if (!estaAutenticado) {
    return <Navigate to="/login" state={{ de: location.pathname + location.search + location.hash }} replace />
  }
  if (!ehAdministrador) {
    return <div className="alert alert-danger" role="alert">Acesso restrito a administradores.</div>
  }

  return <Outlet />
}
