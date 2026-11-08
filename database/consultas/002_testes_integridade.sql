-- Testes de integridade da Aula 1.
-- Executar com mysql --force para continuar após os erros esperados.
-- Todos os testes são realizados dentro de uma transação,
-- encerrada com ROLLBACK.

START TRANSACTION;

-- Teste 1: plataforma inexistente.
-- Esperado: ERROR 1452 (chave estrangeira).
INSERT INTO tb_jogo
(no_jogo, nu_ano_lancamento, vl_preco, qt_estoque,
 st_ativo, dt_cadastro, co_plataforma, co_condicao_jogo)
VALUES
('Teste Plataforma Invalida', 2000, 50.00, 1,
 TRUE, UTC_TIMESTAMP(), 999999, 1);

-- Teste 2: plataforma com nome duplicado.
-- Esperado: ERROR 1062 (UNIQUE).
INSERT INTO tb_plataforma
(no_plataforma, no_fabricante, nu_ano_lancamento)
VALUES
('PlayStation 1', 'Sony', 1994);

-- Teste 3: excluir plataforma que possui jogos.
-- Esperado: ERROR 1451 (chave estrangeira).
DELETE FROM tb_plataforma
WHERE no_plataforma = 'PlayStation 1';

-- Teste 4: jogo com nome nulo.
-- Esperado: ERROR 1048 (NOT NULL).
INSERT INTO tb_jogo
(no_jogo, nu_ano_lancamento, vl_preco, qt_estoque,
 st_ativo, dt_cadastro, co_plataforma, co_condicao_jogo)
VALUES
(NULL, 2000, 50.00, 1,
 TRUE, UTC_TIMESTAMP(), 1, 1);

-- Teste 5: jogo com preço zero.
-- Esperado: inserção aceita.
-- A validação do preço será feita na camada Service.
INSERT INTO tb_jogo
(no_jogo, nu_ano_lancamento, vl_preco, qt_estoque,
 st_ativo, dt_cadastro, co_plataforma, co_condicao_jogo)
VALUES
('Teste Preco Zero', 2000, 0.00, 1,
 TRUE, UTC_TIMESTAMP(), 1, 1);

SELECT COUNT(*) AS jogos_teste_preco_zero
FROM tb_jogo
WHERE no_jogo = 'Teste Preco Zero';

ROLLBACK;

-- Confirma que nenhum registro de teste permaneceu.
SELECT COUNT(*) AS jogos_teste_restantes
FROM tb_jogo
WHERE no_jogo LIKE 'Teste%';
