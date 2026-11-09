-- Aula 2: testes de gravação do JogoRepository.
-- Todas as alterações são desfeitas com ROLLBACK.

START TRANSACTION;

-- 1. Inserir um jogo.
INSERT INTO tb_jogo
(no_jogo, ds_jogo, nu_ano_lancamento,
 vl_preco, qt_estoque, st_ativo,
 dt_cadastro, co_plataforma, co_condicao_jogo)
VALUES
('Jogo Teste Aula 2', 'Teste do Repository', 2000,
 79.90, 5, TRUE,
 UTC_TIMESTAMP(), 1, 1);

SET @id_teste = LAST_INSERT_ID();

SELECT @id_teste AS id_inserido;

-- 2. Alterar o jogo.
UPDATE tb_jogo
SET no_jogo = 'Jogo Teste Alterado',
    ds_jogo = 'Descricao alterada',
    nu_ano_lancamento = 2001,
    vl_preco = 89.90,
    qt_estoque = 3,
    st_ativo = TRUE,
    co_plataforma = 2,
    co_condicao_jogo = 2
WHERE co_seq_jogo = @id_teste;

SELECT no_jogo, vl_preco, qt_estoque,
       co_plataforma, co_condicao_jogo
FROM tb_jogo
WHERE co_seq_jogo = @id_teste;

-- 3. Desativar o jogo sem excluí-lo.
UPDATE tb_jogo
SET st_ativo = FALSE
WHERE co_seq_jogo = @id_teste;

SELECT no_jogo, st_ativo
FROM tb_jogo
WHERE co_seq_jogo = @id_teste;

-- 4. Desfazer todas as alterações.
ROLLBACK;

-- 5. Confirmar que os dados originais continuam intactos.
SELECT COUNT(*) AS total_jogos
FROM tb_jogo;

SELECT COUNT(*) AS total_jogos_ativos
FROM tb_jogo
WHERE st_ativo = TRUE;

SELECT COUNT(*) AS jogos_teste_restantes
FROM tb_jogo
WHERE no_jogo LIKE 'Jogo Teste%';
