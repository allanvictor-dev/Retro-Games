-- Aula 2: consultas do JogoRepository.
-- Sem JOIN e sem subconsultas.

-- 1. Listagem com filtros opcionais.
SET @nome = NULL;
SET @plataforma = NULL;
SET @incluir_inativos = FALSE;
SET @limite = 5;
SET @deslocamento = 0;

SELECT co_seq_jogo, no_jogo, ds_jogo,
       nu_ano_lancamento, vl_preco, qt_estoque,
       st_ativo, dt_cadastro, co_plataforma,
       co_condicao_jogo
FROM tb_jogo
WHERE (@nome IS NULL
       OR no_jogo LIKE CONCAT(
           '%', CONVERT(@nome USING utf8mb4) COLLATE utf8mb4_unicode_ci, '%'))
  AND (@plataforma IS NULL
       OR co_plataforma = @plataforma)
  AND (@incluir_inativos = TRUE OR st_ativo = TRUE)
ORDER BY no_jogo, co_seq_jogo
LIMIT 5 OFFSET 0;

-- 2. Contagem com os mesmos filtros.
SELECT COUNT(*) AS total_encontrado
FROM tb_jogo
WHERE (@nome IS NULL
       OR no_jogo LIKE CONCAT(
           '%', CONVERT(@nome USING utf8mb4) COLLATE utf8mb4_unicode_ci, '%'))
  AND (@plataforma IS NULL
       OR co_plataforma = @plataforma)
  AND (@incluir_inativos = TRUE OR st_ativo = TRUE);

-- 3. Segunda página.
SELECT co_seq_jogo, no_jogo
FROM tb_jogo
WHERE (@nome IS NULL
       OR no_jogo LIKE CONCAT(
           '%', CONVERT(@nome USING utf8mb4) COLLATE utf8mb4_unicode_ci, '%'))
  AND (@plataforma IS NULL
       OR co_plataforma = @plataforma)
  AND (@incluir_inativos = TRUE OR st_ativo = TRUE)
ORDER BY no_jogo, co_seq_jogo
LIMIT 5 OFFSET 5;

-- 4. Filtro por parte do nome.
SET @nome = 'Mario';

SELECT co_seq_jogo, no_jogo
FROM tb_jogo
WHERE no_jogo LIKE CONCAT(
    '%', CONVERT(@nome USING utf8mb4) COLLATE utf8mb4_unicode_ci, '%')
ORDER BY no_jogo;

-- 5. Filtro por plataforma.
SET @plataforma = 1;

SELECT co_seq_jogo, no_jogo
FROM tb_jogo
WHERE co_plataforma = @plataforma
ORDER BY no_jogo;

-- 6. Buscar jogo por ID.
SET @id = 1;

SELECT *
FROM tb_jogo
WHERE co_seq_jogo = @id;

-- 7. Listar jogos inativos.
SELECT co_seq_jogo, no_jogo, st_ativo
FROM tb_jogo
WHERE st_ativo = FALSE;
