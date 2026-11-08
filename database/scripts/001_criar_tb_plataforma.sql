CREATE TABLE tb_plataforma (
    co_seq_plataforma INT NOT NULL AUTO_INCREMENT,
    no_plataforma VARCHAR(80) NOT NULL,
    no_fabricante VARCHAR(80) NOT NULL,
    nu_ano_lancamento INT NOT NULL,

    PRIMARY KEY (co_seq_plataforma),
    UNIQUE (no_plataforma)
);
