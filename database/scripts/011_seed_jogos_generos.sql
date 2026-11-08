INSERT INTO tb_jogo_genero (co_jogo, co_genero)
SELECT jogo.co_seq_jogo, genero.co_seq_genero
FROM (
    SELECT 'Super Mario World' AS jogo, 'Plataforma' AS genero
    UNION ALL SELECT 'Donkey Kong Country', 'Plataforma'
    UNION ALL SELECT 'The Legend of Zelda: A Link to the Past', 'Aventura'
    UNION ALL SELECT 'The Legend of Zelda: A Link to the Past', 'RPG'
    UNION ALL SELECT 'Super Metroid', 'Acao'
    UNION ALL SELECT 'Super Metroid', 'Aventura'
    UNION ALL SELECT 'Sonic the Hedgehog', 'Plataforma'
    UNION ALL SELECT 'Streets of Rage 2', 'Acao'
    UNION ALL SELECT 'Mortal Kombat II', 'Luta'
    UNION ALL SELECT 'Golden Axe', 'Acao'
    UNION ALL SELECT 'Golden Axe', 'Aventura'
    UNION ALL SELECT 'Super Mario 64', 'Plataforma'
    UNION ALL SELECT 'Super Mario 64', 'Aventura'
    UNION ALL SELECT 'Mario Kart 64', 'Corrida'
    UNION ALL SELECT 'The Legend of Zelda: Ocarina of Time', 'Aventura'
    UNION ALL SELECT 'The Legend of Zelda: Ocarina of Time', 'RPG'
    UNION ALL SELECT 'Pokemon Stadium', 'Estrategia'
    UNION ALL SELECT 'Crash Bandicoot', 'Plataforma'
    UNION ALL SELECT 'Final Fantasy VII', 'RPG'
    UNION ALL SELECT 'Tekken 3', 'Luta'
    UNION ALL SELECT 'Gran Turismo 2', 'Corrida'
    UNION ALL SELECT 'God of War', 'Acao'
    UNION ALL SELECT 'God of War', 'Aventura'
    UNION ALL SELECT 'Shadow of the Colossus', 'Aventura'
    UNION ALL SELECT 'Pokemon Emerald', 'RPG'
    UNION ALL SELECT 'Pokemon Emerald', 'Aventura'
    UNION ALL SELECT 'Advance Wars', 'Estrategia'
) AS dados
INNER JOIN tb_jogo AS jogo
    ON jogo.no_jogo = dados.jogo
INNER JOIN tb_genero AS genero
    ON genero.no_genero = dados.genero;
