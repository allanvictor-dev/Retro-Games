import { Route, Routes } from 'react-router-dom'
import FormularioJogo from './pages/Admin/FormularioJogo.jsx'
import Generos from './pages/Admin/Generos.jsx'
import Jogos from './pages/Admin/Jogos.jsx'
import Pedidos from './pages/Admin/Pedidos.jsx'
import Plataformas from './pages/Admin/Plataformas.jsx'
import RotaProtegida from './components/RotaProtegida.jsx'
import Layout from './components/Layout.jsx'
import Catalogo from './pages/Catalogo/Catalogo.jsx'
import DetalheJogo from './pages/Catalogo/DetalheJogo.jsx'
import Cadastro from './pages/Cadastro/Cadastro.jsx'
import Login from './pages/Login/Login.jsx'
import NaoEncontrada from './pages/NaoEncontrada.jsx'

export default function App() {
  return (
    <Routes>
      <Route element={<Layout />}>
        <Route index element={<Catalogo />} />
        <Route path="jogos/:id" element={<DetalheJogo />} />
        <Route path="login" element={<Login />} />
        <Route path="cadastro" element={<Cadastro />} />
        <Route element={<RotaProtegida />}>
          <Route path="admin/jogos" element={<Jogos />} />
          <Route path="admin/jogos/novo" element={<FormularioJogo />} />
          <Route path="admin/jogos/:id/editar" element={<FormularioJogo />} />
          <Route path="admin/plataformas" element={<Plataformas />} />
          <Route path="admin/generos" element={<Generos />} />
          <Route path="admin/pedidos" element={<Pedidos />} />
        </Route>
        <Route path="*" element={<NaoEncontrada />} />
      </Route>
    </Routes>
  )
}
