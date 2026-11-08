# RetroGames

Projeto acadêmico de uma loja web de jogos retrô usados.

## Escopo desta versão

Módulo de Catálogo e Administração:

- Catálogo público de jogos, com busca textual, filtros por plataforma e gênero e paginação.
- Consulta de detalhe de jogo.
- Manutenção de plataformas.
- Manutenção de jogos, com associação a gêneros e exclusão lógica.
- Consulta de gêneros e condições de jogo.
- Autenticação e autorização do perfil Administrador.

## Tecnologias

- .NET 10 / ASP.NET Core Web API com controllers
- ADO.NET com MySqlConnector e MySQL 8
- FluentValidation
- xUnit e Moq
- React, Vite, JavaScript, Bootstrap, React Router e Axios

## Organização

A solução e os componentes serão construídos conforme a especificação técnica e os roteiros de aula fornecidos pelo professor. Consulte [AGENTS.md](AGENTS.md) para as regras obrigatórias de desenvolvimento.

## Configuração

As credenciais e configurações locais ficam em `.env`, que não deve ser versionado. O repositório conterá apenas o modelo `.env.example`, sem senhas reais.
