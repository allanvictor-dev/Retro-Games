-- Aula 2: testes das operações de gravação.
-- ROLLBACK garante que os testes não permaneçam no banco.

START TRANSACTION;

-- 5. Inserir uma plataforma.
INSERT INTO tb_plataforma
(no_plataforma, no_fabricante, nu_ano_lancamento)
VALUES
('Plataforma Aula 2', 'Fabricante Teste', 2000);

SET @id_teste = LAST_INSERT_ID();

SELECT @id_teste AS id_inserido;

-- 6. Alterar a plataforma.
UPDATE tb_plataforma
SET no_plataforma = 'Plataforma Alterada',
    no_fabricante = 'Fabricante Alterado',
    nu_ano_lancamento = 2001
WHERE co_seq_plataforma = @id_teste;

SELECT no_plataforma, no_fabricante, nu_ano_lancamento
FROM tb_plataforma
WHERE co_seq_plataforma = @id_teste;

-- 7. Excluir a plataforma.
DELETE FROM tb_plataforma
WHERE co_seq_plataforma = @id_teste;

SELECT COUNT(*) AS registros_restantes
FROM tb_plataforma
WHERE co_seq_plataforma = @id_teste;

ROLLBACK;
