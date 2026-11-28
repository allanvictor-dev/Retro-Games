export const QUANTIDADE_MAXIMA = 10

export function carregarItens(chave) {
  if (!chave) return []
  try {
    const dados = JSON.parse(localStorage.getItem(chave) || '[]')
    if (!Array.isArray(dados)) return []

    return dados.filter((item, indice) =>
      item &&
      Number.isInteger(item.idJogo) && item.idJogo > 0 &&
      typeof item.nome === 'string' &&
      Number.isFinite(item.preco) && item.preco >= 0 &&
      Number.isInteger(item.quantidade) &&
      item.quantidade >= 1 && item.quantidade <= QUANTIDADE_MAXIMA &&
      dados.findIndex(outro => outro?.idJogo === item.idJogo) === indice
    ).map(({ idJogo, nome, preco, quantidade }) => ({ idJogo, nome, preco, quantidade }))
  } catch {
    return []
  }
}

export function adicionarItem(itens, jogo) {
  if (!(jogo.estoque > 0) || jogo.ativo === false) return itens

  const existente = itens.find(item => item.idJogo === jogo.id)
  if (existente) {
    if (existente.quantidade >= QUANTIDADE_MAXIMA) return itens

    return itens.map(item => item.idJogo === jogo.id
      ? { ...item, nome: jogo.nome, preco: jogo.preco, quantidade: item.quantidade + 1 }
      : item)
  }

  return [...itens, {
    idJogo: jogo.id,
    nome: jogo.nome,
    preco: jogo.preco,
    quantidade: 1,
  }]
}
