export function Carregando() {
  return <div className="text-center py-4" role="status">Carregando...</div>
}

export function MensagemErro({ mensagem, tentarNovamente }) {
  return (
    <div className="alert alert-danger" role="alert">
      <p className="mb-2">{mensagem}</p>
      {tentarNovamente && (
        <button type="button" className="btn btn-outline-danger"
          onClick={tentarNovamente}>Tentar novamente</button>
      )}
    </div>
  )
}

export function EstadoVazio({ mensagem = 'Nenhum jogo encontrado.' }) {
  return <div className="alert alert-secondary">{mensagem}</div>
}
