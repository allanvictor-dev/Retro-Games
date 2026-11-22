export default function Paginacao({ pagina, totalPaginas, alterarPagina }) {
  if (totalPaginas <= 1) return null

  return (
    <nav className="mt-4" aria-label="Paginação do catálogo">
      <ul className="pagination flex-wrap justify-content-center">
        <li className="page-item">
          <button type="button" className="page-link" disabled={pagina <= 1} onClick={() => alterarPagina(pagina - 1)}>Anterior</button>
        </li>
        {Array.from({ length: totalPaginas }, (_, indice) => indice + 1).map(numero => (
          <li key={numero} className={`page-item${numero === pagina ? ' active' : ''}`}>
            <button type="button" className="page-link" aria-current={numero === pagina ? 'page' : undefined} onClick={() => alterarPagina(numero)}>{numero}</button>
          </li>
        ))}
        <li className="page-item">
          <button type="button" className="page-link" disabled={pagina >= totalPaginas} onClick={() => alterarPagina(pagina + 1)}>Próxima</button>
        </li>
      </ul>
    </nav>
  )
}
