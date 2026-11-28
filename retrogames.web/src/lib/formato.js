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
