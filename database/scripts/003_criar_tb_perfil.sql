CREATE TABLE tb_perfil (
    co_perfil INT NOT NULL,
    ds_perfil VARCHAR(80) NOT NULL,

    PRIMARY KEY (co_perfil),
    UNIQUE (ds_perfil)
);
