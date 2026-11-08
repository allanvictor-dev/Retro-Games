INSERT INTO tb_jogo (
    no_jogo, ds_jogo, nu_ano_lancamento,
    vl_preco, qt_estoque, st_ativo, dt_cadastro,
    co_plataforma, co_condicao_jogo
)
SELECT
    dados.nome,
    dados.descricao,
    dados.ano,
    dados.preco,
    dados.estoque,
    dados.ativo,
    UTC_TIMESTAMP(),
    plataforma.co_seq_plataforma,
    condicao.co_condicao_jogo
FROM (
    SELECT 'Super Mario World' nome, 'Jogo de plataforma da Nintendo' descricao, 1990 ano, 149.90 preco, 5 estoque, 1 ativo, 'Super Nintendo' plataforma, 'Usado' condicao
    UNION ALL SELECT 'Donkey Kong Country', 'Aventura com Donkey Kong', 1994, 129.90, 3, 1, 'Super Nintendo', 'Usado'
    UNION ALL SELECT 'The Legend of Zelda: A Link to the Past', 'Aventura de Link', 1991, 199.90, 2, 1, 'Super Nintendo', 'Recondicionado'
    UNION ALL SELECT 'Super Metroid', 'Exploracao espacial', 1994, 179.90, 0, 1, 'Super Nintendo', 'Usado'
    UNION ALL SELECT 'Sonic the Hedgehog', 'Aventura de Sonic', 1991, 99.90, 6, 1, 'Mega Drive', 'Usado'
    UNION ALL SELECT 'Streets of Rage 2', 'Jogo de acao da Sega', 1992, 119.90, 4, 1, 'Mega Drive', 'Recondicionado'
    UNION ALL SELECT 'Mortal Kombat II', 'Jogo de luta', 1993, 109.90, 3, 1, 'Mega Drive', 'Usado'
    UNION ALL SELECT 'Golden Axe', 'Aventura de fantasia', 1989, 89.90, 0, 0, 'Mega Drive', 'Usado'
    UNION ALL SELECT 'Super Mario 64', 'Aventura tridimensional de Mario', 1996, 159.90, 5, 1, 'Nintendo 64', 'Usado'
    UNION ALL SELECT 'Mario Kart 64', 'Corridas com personagens Nintendo', 1996, 139.90, 4, 1, 'Nintendo 64', 'Recondicionado'
    UNION ALL SELECT 'The Legend of Zelda: Ocarina of Time', 'Aventura de Link em Hyrule', 1998, 229.90, 2, 1, 'Nintendo 64', 'Usado'
    UNION ALL SELECT 'Pokemon Stadium', 'Batalhas entre Pokemon', 1999, 119.90, 3, 1, 'Nintendo 64', 'Usado'
    UNION ALL SELECT 'Crash Bandicoot', 'Jogo de plataforma', 1996, 109.90, 7, 1, 'PlayStation 1', 'Usado'
    UNION ALL SELECT 'Final Fantasy VII', 'RPG da Square', 1997, 189.90, 2, 1, 'PlayStation 1', 'Recondicionado'
    UNION ALL SELECT 'Tekken 3', 'Jogo de luta', 1998, 129.90, 4, 1, 'PlayStation 1', 'Usado'
    UNION ALL SELECT 'Gran Turismo 2', 'Simulador de corridas', 1999, 99.90, 0, 1, 'PlayStation 1', 'Usado'
    UNION ALL SELECT 'God of War', 'Aventura de Kratos', 2005, 119.90, 5, 1, 'PlayStation 2', 'Usado'
    UNION ALL SELECT 'Shadow of the Colossus', 'Aventura com gigantes', 2005, 149.90, 2, 1, 'PlayStation 2', 'Recondicionado'
    UNION ALL SELECT 'Pokemon Emerald', 'RPG de Pokemon', 2004, 179.90, 3, 1, 'Game Boy Advance', 'Usado'
    UNION ALL SELECT 'Advance Wars', 'Jogo de estrategia', 2001, 129.90, 0, 0, 'Game Boy Advance', 'Novo'
) AS dados
INNER JOIN tb_plataforma AS plataforma
    ON plataforma.no_plataforma = dados.plataforma
INNER JOIN tb_condicao_jogo AS condicao
    ON condicao.ds_condicao_jogo = dados.condicao;
