-- Aula 2: consultas do JogoGeneroRepository.

-- 1. Listar todos os vínculos.
SELECT co_jogo, co_genero
FROM tb_jogo_genero
ORDER BY co_jogo, co_genero;

-- 2. Listar vínculos de vários jogos.
SELECT co_jogo, co_genero
FROM tb_jogo_genero
WHERE co_jogo IN (1, 2, 3)
ORDER BY co_jogo, co_genero;

-- 3. Testar inserção e remoção sem deixar alterações.
START TRANSACTION;

-- Cria um jogo temporário para testar os vínculos.
INSERT INTO tb_jogo
(no_jogo, nu_ano_lancamento, vl_preco,
 qt_estoque, st_ativo, dt_cadastro,
 co_plataforma, co_condicao_jogo)
VALUES
('Teste Vinculos Aula 2', 2000, 10.00,
 1, TRUE, UTC_TIMESTAMP(), 1, 1);

SET @jogo_teste = LAST_INSERT_ID();

-- Inserir vínculos.
INSERT INTO tb_jogo_genero (co_jogo, co_genero)
VALUES (@jogo_teste, 1), (@jogo_teste, 2);

SELECT COUNT(*) AS vinculos_inseridos
FROM tb_jogo_genero
WHERE co_jogo = @jogo_teste;

-- Remover vínculos do jogo.
DELETE FROM tb_jogo_genero
WHERE co_jogo = @jogo_teste;

SELECT COUNT(*) AS vinculos_restantes
FROM tb_jogo_genero
WHERE co_jogo = @jogo_teste;

ROLLBACK;

-- Conferir os totais originais.
SELECT COUNT(*) AS total_jogos FROM tb_jogo;
SELECT COUNT(*) AS total_vinculos FROM tb_jogo_genero;
