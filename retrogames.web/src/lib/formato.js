const formatoPreco = new Intl.NumberFormat('pt-BR', {
  style: 'currency',
  currency: 'BRL',
})

export function formatarPreco(valor) {
  return formatoPreco.format(valor)
}
