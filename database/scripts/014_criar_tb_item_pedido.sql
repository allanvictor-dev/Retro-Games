CREATE TABLE tb_item_pedido (
    co_seq_item_pedido INT NOT NULL AUTO_INCREMENT,
    qt_item INT NOT NULL,
    vl_unitario DECIMAL(10,2) NOT NULL,
    co_pedido INT NOT NULL,
    co_jogo INT NOT NULL,

    PRIMARY KEY (co_seq_item_pedido),
    UNIQUE (co_pedido, co_jogo),

    FOREIGN KEY (co_pedido)
        REFERENCES tb_pedido(co_seq_pedido),

    FOREIGN KEY (co_jogo)
        REFERENCES tb_jogo(co_seq_jogo)
);
