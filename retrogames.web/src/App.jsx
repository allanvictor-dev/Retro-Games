import { Route, Routes } from 'react-router-dom'
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
        <Route element={<RotaProtegida />}>
          <Route path="admin/jogos" element={<h1>Gerenciamento de jogos</h1>} />
          <Route path="admin/plataformas" element={<h1>Gerenciamento de plataformas</h1>} />
        </Route>
        <Route path="*" element={<NaoEncontrada />} />
      </Route>
    </Routes>
  )
}
