# Módulo de Compras

## 1. Escopo

A API permite cadastrar clientes, finalizar compras, consultar pedidos e administrar suas situações e os gêneros do catálogo. As consultas do cliente fazem parte da entrega da task #17. O contrato abaixo considera essa implementação junto ao restante do backend.

## 2. Tabelas e relacionamentos

Os scripts [012 a 015](../database/scripts) estendem o catálogo. Todas as colunas abaixo são obrigatórias.

| Tabela | Chaves e campos | Uso |
| --- | --- | --- |
| `tb_situacao_pedido` | PK `co_situacao_pedido`, `ds_situacao_pedido VARCHAR(80) UNIQUE` | Domínio fixo. 1 Pendente, 2 Pago, 3 Enviado, 4 Cancelado. |
| `tb_pedido` | PK automática `co_seq_pedido`, `dt_pedido DATETIME`, `vl_total DECIMAL(10,2)`, FKs `co_usuario` e `co_situacao_pedido` | Cada pedido pertence a um usuário e tem uma situação. |
| `tb_item_pedido` | PK automática `co_seq_item_pedido`, `qt_item INT`, `vl_unitario DECIMAL(10,2)`, FKs `co_pedido` e `co_jogo` | Um item por jogo em cada pedido. O par de FKs é único. |

O cadastro usa `tb_usuario` e `tb_perfil`. A compra consulta preço, disponibilidade e estoque em `tb_jogo`. A manutenção de gêneros usa `tb_genero` e verifica os vínculos em `tb_jogo_genero` antes da exclusão.

## 3. Endpoints

Todos os caminhos têm o prefixo `/api`. A sessão usa o cookie HTTP-only `retrogames.sessao`. O backend obtém o comprador da sessão.

| Método e caminho | Acesso | Entrada ou filtro | Resposta de sucesso |
| --- | --- | --- | --- |
| POST `/Autenticacao/cadastrar` | Público | `nome`, `email`, `senha` | 201, cliente autenticado com id, nome, e-mail e perfil |
| POST `/Pedido` | Autenticado | `{"itens":[{"idJogo":1,"quantidade":2}]}` | 201, pedido e itens. O cabeçalho Location aponta ao detalhe. |
| GET `/Pedido/meus` | Autenticado | Sem parâmetros | 200, pedidos da sessão ou lista vazia |
| GET `/Pedido/{id}` | Dono ou administrador | ID do pedido | 200, detalhe de um pedido |
| GET `/Pedido` | Administrador | `idSituacao` opcional | 200, lista de pedidos com dados do comprador |
| PUT `/Pedido/{id}/situacao` | Administrador | `{"idSituacaoPedido":2}` | 200, confirmação |
| GET `/SituacaoPedido` | Administrador | Sem parâmetros | 200, lista de IDs e descrições |
| GET `/Genero` | Público | Sem parâmetros | 200, lista de gêneros |
| GET `/Genero/{id}` | Público | ID do gênero | 200, id e nome |
| POST `/Genero` | Administrador | `nome` | 201, gênero criado |
| PUT `/Genero/{id}` | Administrador | `nome` | 200, confirmação |
| DELETE `/Genero/{id}` | Administrador | ID do gênero | 200, confirmação |

As consultas de pedidos devolvem `id`, `data`, `valorTotal`, `idSituacaoPedido`, `descricaoSituacao`, `idUsuario`, `nomeUsuario`, `emailUsuario` e `itens`. Cada item contém `idJogo`, `nomeJogo`, `quantidade`, `valorUnitario` e `subtotal`. O preço unitário é o registrado na compra.

## 4. Regras de negócio

- **Cadastro.** O nome aceita até 120 caracteres, o e-mail até 255 e a senha entre 6 e 100. O serviço remove espaços nas extremidades de nome e e-mail, normaliza o e-mail para minúsculas, verifica a unicidade e persiste o hash da senha. A conta nasce ativa com perfil Cliente.
- **Compra.** O corpo contém ao menos um item, com ID positivo, quantidade entre 1 e 10 e jogos distintos. O serviço valida todos os jogos e o estoque antes de gravar. Calcula o total pela soma de preço × quantidade, registra a data UTC e cria o pedido como Pendente. A inserção do pedido, dos itens e a baixa do estoque compartilham uma transação.
- **Consulta.** O repositório filtra pelo ID do cliente e ordena por data decrescente, com desempate por ID decrescente. O serviço combina pedidos, situações, usuários, itens e jogos com LINQ, a partir de consultas separadas. Na busca por ID, valida o dono ou o perfil administrador antes de carregar os detalhes.
- **Gestão.** A alteração exige pedido e situação existentes. O serviço permite escolher qualquer situação cadastrada. A alteração atualiza o campo de situação, inclusive no cancelamento.
- **Gêneros.** O nome é obrigatório, aceita até 80 caracteres e deve ser único. Um gênero vinculado a jogo não pode ser excluído.

## 5. Envelope e códigos

A API retorna `responseCode` e `errorMessage`. Respostas com conteúdo incluem `data`. No sucesso, o código é 0 e a mensagem é nula. Erros de negócio usam o envelope simples. Erros de validação pelo FluentValidation incluem em `data` as mensagens agrupadas por campo.

| HTTP | Código | Significado |
| --- | --- | --- |
| 400 | 2 / 3 | Corpo nulo tratado pelo filtro / modelo inválido |
| 401 | 4 / 6 | Sessão exigida / credenciais inválidas no login |
| 403 | 5 | Perfil sem autorização para a rota administrativa |
| 404 | 105 | Gênero inexistente |
| 409 | 106 | E-mail já cadastrado |
| 409 | 107 | Nome de gênero duplicado |
| 409 | 108 | Gênero vinculado a jogos |
| 404 | 109 | Pedido inexistente |
| 400 | 110 | Jogo inexistente ou inativo na compra |
| 409 | 111 | Estoque insuficiente |
| 404 | 112 | Situação inexistente |
| 403 | 113 | Pedido pertence a outro cliente |
| 500 | 1 | Erro interno inesperado |

Catálogos de referência: [BusinessErrors](../RetroGames.Services/BusinessErrors.cs) e [ResponseCodes](../RetroGames.WebApi.ApiModels/ResponseCodes.cs).


## 6. Validação pelo Swagger UI

Em 28/11/2026, o cliente do Swagger UI executou 30 chamadas HTTP contra a API da revisão `84b2895`, com MySQL 8.0 isolado e scripts 001–015. As verificações compararam o status HTTP, o código no JSON e a presença de mensagem em cada erro. A sequência abaixo pode ser reproduzida em `/swagger`, em ambiente de desenvolvimento, com um banco de teste.

| Sequência | Resultado observado |
| --- | --- |
| Consultar meus pedidos sem sessão. Cadastrar o cliente A e repetir seu e-mail. Tentar listar todos os pedidos como A. | 401/4, 201/0, 409/106 e 403/5, respectivamente. |
| Como A, enviar carrinho vazio, jogo inexistente e quantidade acima do estoque. Comprar uma unidade disponível. | 400/3, 400/110, 409/111 e 201/0. |
| Consultar a lista e o detalhe de A, depois um ID inexistente. Cadastrar B, listar seus pedidos e tentar abrir o pedido de A. | A recebeu o próprio pedido com total e preço pago corretos. ID inexistente retornou 404/109. B recebeu lista vazia e 403/113 no detalhe de A. |
| Entrar como administrador, abrir o detalhe de A, listar situações, tentar uma situação inexistente e alterar para Pago. Filtrar a listagem por Pago. | Detalhe e situações retornaram 200/0. A situação inexistente retornou 404/112. A alteração retornou 200/0 e o pedido apareceu no filtro como Pago. |
| Listar gêneros, criar um nome, repeti-lo, buscar e renomear. Excluir um gênero vinculado e depois o gênero de teste. Consultar o excluído. | 200/0, 201/0, 409/107, 200/0, 200/0, 409/108, 200/0 e 404/105, respectivamente. |

As chamadas de apoio consultaram jogos disponíveis, autenticaram o administrador e encerraram sua sessão. Os dados e as credenciais da execução eram temporários.

## 7. Rastreabilidade

### 7.1 Matriz do Módulo de Compras

| Requisito | Entrega | Evidência |
| --- | --- | --- |
| C01 — Cadastro de cliente | Cadastro autenticado por cookie, perfil Cliente e e-mail único | AutenticacaoController e AutenticacaoService. Swagger 201/0 e 409/106. |
| C02 — Menu por perfil | O backend fornece a identidade e o perfil em `/api/Autenticacao/eu`. O menu responsivo tem acompanhamento próprio no frontend. | AutenticacaoController e [task de interface #21](https://github.com/allanvictor-dev/Retro-Games/issues/21). |
| C03 — Carrinho | A API recebe os itens na finalização. A persistência do carrinho no navegador pertence ao frontend. | InserirPedidoRequestValidator e [task de carrinho #25](https://github.com/allanvictor-dev/Retro-Games/issues/25). |
| C04 — Finalizar compra | Pedido, itens, preço pago e baixa de estoque em transação | PedidoService.InserirAsync. Swagger 201/0, 400/110 e 409/111. |
| C05 — Meus pedidos | Listagem exclusiva da sessão em ordem cronológica inversa | PedidoRepository.ListarPorUsuarioAsync e PedidoService.ListarMeusAsync. Swagger com dois clientes. |
| C06 — Detalhe | Consulta pelo dono ou administrador | PedidoService.BuscarPorIdAsync. Swagger 200/0, 404/109 e 403/113. |
| C07 — Gestão de pedidos | Listagem administrativa, filtro e alteração da situação | PedidoController e SituacaoPedidoController. Swagger 403/5, 404/112 e 200/0. |
| C08 — Gestão de gêneros | Cadastro, consulta, alteração e exclusão condicionada aos vínculos | GeneroController e GeneroService. Swagger 404/105, 409/107 e 409/108. |
