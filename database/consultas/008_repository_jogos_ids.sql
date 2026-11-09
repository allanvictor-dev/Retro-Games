-- Aula 2: filtro de jogos por lista de IDs.
-- Cada consulta acessa somente tb_jogo.

-- 1. Buscar jogos pelos IDs 1, 3 e 5.
SELECT co_seq_jogo, no_jogo, st_ativo
FROM tb_jogo
WHERE co_seq_jogo IN (1, 3, 5)
ORDER BY no_jogo, co_seq_jogo;

-- 2. Aplicar filtro por IDs e somente jogos ativos.
SELECT co_seq_jogo, no_jogo, st_ativo
FROM tb_jogo
WHERE co_seq_jogo IN (1, 3, 5)
  AND st_ativo = TRUE
ORDER BY no_jogo, co_seq_jogo;

-- 3. Contar os jogos com os mesmos filtros.
SELECT COUNT(*) AS total_encontrado
FROM tb_jogo
WHERE co_seq_jogo IN (1, 3, 5)
  AND st_ativo = TRUE;

-- 4. Confirmar que nenhum dado foi alterado.
SELECT COUNT(*) AS total_jogos
FROM tb_jogo;
