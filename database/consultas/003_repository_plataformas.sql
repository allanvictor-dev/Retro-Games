-- Aula 2: consultas do PlataformaRepository.
-- Apenas leituras, sem alterar os dados.

-- 1. Listar plataformas.
SELECT co_seq_plataforma, no_plataforma,
       no_fabricante, nu_ano_lancamento
FROM tb_plataforma
ORDER BY no_plataforma;

-- 2. Buscar plataforma pelo ID.
SET @id = 1;

SELECT co_seq_plataforma, no_plataforma,
       no_fabricante, nu_ano_lancamento
FROM tb_plataforma
WHERE co_seq_plataforma = @id;

-- 3. Verificar se existe uma plataforma com o nome.
-- O ID ignorado é opcional.
SET @nome = 'PlayStation 1';
SET @id_ignorado = NULL;

SELECT COUNT(*) > 0 AS existe
FROM tb_plataforma
WHERE no_plataforma = @nome COLLATE utf8mb4_unicode_ci
  AND (@id_ignorado IS NULL
       OR co_seq_plataforma <> @id_ignorado);

-- 4. Verificar se a plataforma possui jogos.
-- Esta consulta acessa somente tb_jogo.
SET @id_plataforma = 1;

SELECT COUNT(*) > 0 AS possui_jogos
FROM tb_jogo
WHERE co_plataforma = @id_plataforma;
