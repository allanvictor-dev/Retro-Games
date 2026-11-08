CREATE TABLE tb_genero (
    co_seq_genero INT NOT NULL AUTO_INCREMENT,
    no_genero VARCHAR(80) NOT NULL,

    PRIMARY KEY (co_seq_genero),
    UNIQUE (no_genero)
);
