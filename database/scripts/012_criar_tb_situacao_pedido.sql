CREATE TABLE tb_situacao_pedido (
    co_situacao_pedido INT NOT NULL,
    ds_situacao_pedido VARCHAR(80) NOT NULL,

    PRIMARY KEY (co_situacao_pedido),
    UNIQUE (ds_situacao_pedido)
);
