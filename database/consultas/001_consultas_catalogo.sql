-- 1. Catálogo: jogos ativos, plataforma e condição.
SELECT
    j.no_jogo AS jogo,
    p.no_plataforma AS plataforma,
    c.ds_condicao_jogo AS condicao,
    j.vl_preco AS preco
FROM tb_jogo j
JOIN tb_plataforma p ON p.co_seq_plataforma = j.co_plataforma
JOIN tb_condicao_jogo c ON c.co_condicao_jogo = j.co_condicao_jogo
WHERE j.st_ativo = TRUE
ORDER BY j.no_jogo;

-- 2. Gêneros de um jogo informado.
SET @nome_jogo = 'Super Mario World';

SELECT g.no_genero AS genero
FROM tb_genero g
JOIN tb_jogo_genero jg ON jg.co_genero = g.co_seq_genero
JOIN tb_jogo j ON j.co_seq_jogo = jg.co_jogo
WHERE j.no_jogo = @nome_jogo COLLATE utf8mb4_unicode_ci
ORDER BY g.no_genero;

-- 3. Jogos de um gênero informado.
SET @nome_genero = 'RPG';

SELECT j.no_jogo AS jogo
FROM tb_jogo j
JOIN tb_jogo_genero jg ON jg.co_jogo = j.co_seq_jogo
JOIN tb_genero g ON g.co_seq_genero = jg.co_genero
WHERE g.no_genero = @nome_genero COLLATE utf8mb4_unicode_ci
ORDER BY j.no_jogo;

-- 4. Quantidade de jogos por plataforma, inclusive sem jogos.
SELECT
    p.no_plataforma AS plataforma,
    COUNT(j.co_seq_jogo) AS quantidade
FROM tb_plataforma p
LEFT JOIN tb_jogo j ON j.co_plataforma = p.co_seq_plataforma
GROUP BY p.co_seq_plataforma, p.no_plataforma
ORDER BY p.no_plataforma;

-- 5. Busca por parte do nome, 5 resultados por página.
SET @busca = 'a';
SET @pagina = 1;
SET @tamanho_pagina = 5;
SET @deslocamento = (@pagina - 1) * @tamanho_pagina;

SELECT co_seq_jogo, no_jogo
FROM tb_jogo
WHERE no_jogo LIKE CONCAT('%', @busca COLLATE utf8mb4_unicode_ci, '%')
ORDER BY no_jogo, co_seq_jogo
LIMIT 5 OFFSET 0;

-- Segunda página da mesma busca.
SET @pagina = 2;
SET @deslocamento = (@pagina - 1) * @tamanho_pagina;

SELECT co_seq_jogo, no_jogo
FROM tb_jogo
WHERE no_jogo LIKE CONCAT('%', @busca COLLATE utf8mb4_unicode_ci, '%')
ORDER BY no_jogo, co_seq_jogo
LIMIT 5 OFFSET 5;
