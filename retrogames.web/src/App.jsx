import { Route, Routes } from 'react-router-dom'
import Carrinho from './pages/Carrinho/Carrinho.jsx'
import Plataformas from './pages/Admin/Plataformas.jsx'
import RotaProtegida from './components/RotaProtegida.jsx'
import Layout from './components/Layout.jsx'
import Catalogo from './pages/Catalogo/Catalogo.jsx'
import DetalheJogo from './pages/Catalogo/DetalheJogo.jsx'
import Login from './pages/Login/Login.jsx'
import NaoEncontrada from './pages/NaoEncontrada.jsx'

export default function App() {
  return (
    <Routes>
      <Route element={<Layout />}>
        <Route index element={<Catalogo />} />
        <Route path="jogos/:id" element={<DetalheJogo />} />
        <Route path="login" element={<Login />} />
        <Route element={<RotaProtegida somenteAdministrador={false} />}>
          <Route path="carrinho" element={<Carrinho />} />
        </Route>
        <Route element={<RotaProtegida />}>
          <Route path="admin/jogos" element={<h1>Gerenciamento de jogos</h1>} />
          <Route path="admin/plataformas" element={<Plataformas />} />
        </Route>
        <Route path="*" element={<NaoEncontrada />} />
      </Route>
    </Routes>
  )
}
