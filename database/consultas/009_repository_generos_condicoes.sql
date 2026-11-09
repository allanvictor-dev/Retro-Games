-- Aula 2: consultas de gêneros e condições dos jogos.
-- Cada consulta acessa apenas uma tabela.

-- 1. Listar todos os gêneros em ordem alfabética.
SELECT co_seq_genero, no_genero
FROM tb_genero
ORDER BY no_genero;

-- 2. Listar todas as condições dos jogos.
SELECT co_condicao_jogo, ds_condicao_jogo
FROM tb_condicao_jogo
ORDER BY ds_condicao_jogo;
