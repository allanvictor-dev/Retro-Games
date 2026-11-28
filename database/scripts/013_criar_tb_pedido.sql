CREATE TABLE tb_pedido (
    co_seq_pedido INT NOT NULL AUTO_INCREMENT,
    dt_pedido DATETIME NOT NULL,
    vl_total DECIMAL(10,2) NOT NULL,
    co_usuario INT NOT NULL,
    co_situacao_pedido INT NOT NULL,

    PRIMARY KEY (co_seq_pedido),

    FOREIGN KEY (co_usuario)
        REFERENCES tb_usuario(co_seq_usuario),

    FOREIGN KEY (co_situacao_pedido)
        REFERENCES tb_situacao_pedido(co_situacao_pedido)
);
