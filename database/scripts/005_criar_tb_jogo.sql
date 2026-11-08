CREATE TABLE tb_jogo (
    co_seq_jogo INT NOT NULL AUTO_INCREMENT,
    no_jogo VARCHAR(150) NOT NULL,
    ds_jogo TEXT NULL,
    nu_ano_lancamento INT NOT NULL,
    vl_preco DECIMAL(10,2) NOT NULL,
    qt_estoque INT NOT NULL,
    st_ativo BOOLEAN NOT NULL DEFAULT TRUE,
    dt_cadastro DATETIME NOT NULL,
    co_plataforma INT NOT NULL,
    co_condicao_jogo INT NOT NULL,

    PRIMARY KEY (co_seq_jogo),
    INDEX idx_no_jogo (no_jogo),

    FOREIGN KEY (co_plataforma)
        REFERENCES tb_plataforma(co_seq_plataforma),

    FOREIGN KEY (co_condicao_jogo)
        REFERENCES tb_condicao_jogo(co_condicao_jogo)
);
