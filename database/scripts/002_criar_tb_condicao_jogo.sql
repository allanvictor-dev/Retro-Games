CREATE TABLE tb_condicao_jogo (
    co_condicao_jogo INT NOT NULL,
    ds_condicao_jogo VARCHAR(80) NOT NULL,

    PRIMARY KEY (co_condicao_jogo),
    UNIQUE (ds_condicao_jogo)
);
