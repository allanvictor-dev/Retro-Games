import axios from 'axios'

const api = axios.create({
  baseURL: '/api',
  withCredentials: true,
})

api.interceptors.response.use(
  resposta => resposta,
  erro => {
    const rota = erro.config?.url

    if (erro.response?.status === 401 &&
        rota !== '/Autenticacao/eu' && rota !== '/Autenticacao/login') {
      window.dispatchEvent(new Event('sessao:expirada'))
    }

    return Promise.reject(erro)
  },
)

export default api
