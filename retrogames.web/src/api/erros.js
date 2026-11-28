export function obterMensagemErro(erro) {
  if (!erro.response) {
    return 'Não foi possível conectar ao servidor.'
  }

  return erro.response.data?.errorMessage || 'Não foi possível concluir a operação.'
}

export function obterErrosDosCampos(erro) {
  const dados = erro.response?.data?.data
  if (erro.response?.status !== 400 || !dados || typeof dados !== 'object' || Array.isArray(dados)) {
    return {}
  }

  return Object.fromEntries(
    Object.entries(dados)
      .filter(([, mensagens]) => Array.isArray(mensagens) && mensagens.every(mensagem => typeof mensagem === 'string'))
      .map(([campo, mensagens]) => [campo, mensagens.join(' ')])
  )
}
