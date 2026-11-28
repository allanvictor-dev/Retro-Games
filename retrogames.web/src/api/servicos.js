import api from './api.js'

export const autenticacaoApi = {
  async entrar(email, senha) {
    const resposta = await api.post('/Autenticacao/login', { email, senha })
    return resposta.data.data
  },

  async cadastrar(nome, email, senha) {
    const resposta = await api.post('/Autenticacao/cadastrar', { nome, email, senha })
    return resposta.data.data
  },

  async sair() {
    await api.post('/Autenticacao/logout')
  },

  async usuarioAtual() {
    const resposta = await api.get('/Autenticacao/eu')
    return resposta.data.data
  },
}

export const jogoApi = {
  async listar(filtros = {}) {
    const resposta = await api.get('/Jogo', { params: filtros })
    return resposta.data.data
  },

  async buscarPorId(id) {
    const resposta = await api.get('/Jogo/' + id)
    return resposta.data.data
  },
  async inserir(dados) {
    const resposta = await api.post('/Jogo', dados)
    return resposta.data.data
  },
  async alterar(id, dados) {
    await api.put('/Jogo/' + id, dados)
  },
  async desativar(id) {
    await api.delete('/Jogo/' + id)
  },
}

export const plataformaApi = {
  async listar() {
    const resposta = await api.get('/Plataforma')
    return resposta.data.data
  },
  async inserir(dados) {
    const resposta = await api.post('/Plataforma', dados)
    return resposta.data.data
  },
  async alterar(id, dados) {
    await api.put('/Plataforma/' + id, dados)
  },
  async excluir(id) {
    await api.delete('/Plataforma/' + id)
  },
}

export const generoApi = {
  async listar() {
    const resposta = await api.get('/Genero')
    return resposta.data.data
  },
  async inserir(dados) {
    const resposta = await api.post('/Genero', dados)
    return resposta.data.data
  },
  async alterar(id, dados) {
    await api.put('/Genero/' + id, dados)
  },
  async excluir(id) {
    await api.delete('/Genero/' + id)
  },
}

export const condicaoJogoApi = {
  async listar() {
    const resposta = await api.get('/CondicaoJogo')
    return resposta.data.data
  },
}

export const situacaoPedidoApi = {
  async listar() {
    const resposta = await api.get('/SituacaoPedido')
    return resposta.data.data
  },
}

export const pedidoApi = {
  async listar(idSituacao) {
    const params = idSituacao ? { idSituacao } : {}
    const resposta = await api.get('/Pedido', { params })
    return resposta.data.data
  },
  async alterarSituacao(id, idSituacaoPedido) {
    await api.put('/Pedido/' + id + '/situacao', { idSituacaoPedido })
  },
}
