CREATE TABLE tb_jogo_genero (
    co_jogo INT NOT NULL,
    co_genero INT NOT NULL,

    PRIMARY KEY (co_jogo, co_genero),

    FOREIGN KEY (co_jogo)
        REFERENCES tb_jogo(co_seq_jogo),

    FOREIGN KEY (co_genero)
        REFERENCES tb_genero(co_seq_genero)
);
