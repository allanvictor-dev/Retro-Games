CREATE TABLE tb_usuario (
    co_seq_usuario INT NOT NULL AUTO_INCREMENT,
    no_usuario VARCHAR(120) NOT NULL,
    ds_email VARCHAR(255) NOT NULL,
    ds_senha_hash VARCHAR(255) NOT NULL,
    st_ativo BOOLEAN NOT NULL DEFAULT TRUE,
    dt_cadastro DATETIME NOT NULL,
    co_perfil INT NOT NULL,

    PRIMARY KEY (co_seq_usuario),
    UNIQUE (ds_email),

    FOREIGN KEY (co_perfil)
        REFERENCES tb_perfil(co_perfil)
);
