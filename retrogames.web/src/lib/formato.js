const formatoPreco = new Intl.NumberFormat('pt-BR', {
  style: 'currency',
  currency: 'BRL',
})

const formatoDataHora = new Intl.DateTimeFormat('pt-BR', {
  dateStyle: 'short',
  timeStyle: 'short',
})

export function formatarPreco(valor) {
  return formatoPreco.format(valor)
}

export function formatarDataHora(valor) {
  return formatoDataHora.format(new Date(valor))
}

const formatoData = new Intl.DateTimeFormat('pt-BR', {
  day: '2-digit',
  month: '2-digit',
  year: 'numeric',
})

export function formatarData(valor) {
  // O banco armazena a data do pedido em UTC, mesmo quando não envia o sufixo Z.
  const data = new Date(typeof valor === 'string' && /^\d{4}-\d{2}-\d{2}T\d{2}:\d{2}:\d{2}(\.\d+)?$/.test(valor) ? valor + 'Z' : valor)
  return Number.isNaN(data.getTime()) ? '—' : formatoData.format(data)
}
