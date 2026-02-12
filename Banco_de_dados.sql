DROP DATABASE IF EXISTS SISTEMA; -- Remove o banco antigo para recriar com a nova estrutura
CREATE DATABASE SISTEMA;
USE SISTEMA;

-- Tabela de Porteiros (Usuários do sistema)
CREATE TABLE IF NOT EXISTS porteiros (
    id INT AUTO_INCREMENT PRIMARY KEY,
    nome_completo VARCHAR(100) NOT NULL, -- Adicionei o nome real também, é bom ter
    username VARCHAR(50) NOT NULL UNIQUE,
    email VARCHAR(100) NOT NULL UNIQUE, -- NOVO CAMPO DE EMAIL
    password_hash VARCHAR(255) NOT NULL,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- Tabela de Moradores
CREATE TABLE IF NOT EXISTS moradores (
    id INT AUTO_INCREMENT PRIMARY KEY,
    nome_completo VARCHAR(100) NOT NULL,
    bloco VARCHAR(10),
    apartamento VARCHAR(10),
    telefone VARCHAR(20),
    
    -- Biometria (IDs do sensor)
    fingerprint_id_1 INT UNIQUE,
    fingerprint_id_2 INT UNIQUE,
    
    ativo BOOLEAN DEFAULT TRUE,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- Tabela de Acessos
CREATE TABLE IF NOT EXISTS acessos (
    id INT AUTO_INCREMENT PRIMARY KEY,
    fingerprint_id INT NOT NULL,
    data_hora DATETIME NOT NULL,
    tipo ENUM('ENTRADA', 'SAIDA') NOT NULL
);

-- ---------------------------------------------------------
-- INSERTS DE TESTE
-- ---------------------------------------------------------

-- 1. Porteiro Admin (Senha: admin123)
-- OBS: Em produção, você deve gravar a senha como HASH, não texto puro.
INSERT INTO porteiros (nome_completo, username, email, password_hash)
VALUES ('Porteiro Chefe', 'admin', 'admin@condominio.com', 'admin123');

-- 2. Morador Teste
INSERT INTO moradores
(
    nome_completo,
    bloco,
    apartamento,
    telefone,
    fingerprint_id_1,
    fingerprint_id_2,
    ativo
)
VALUES
(
    'João da Silva',
    'A',
    '101',
    '(11) 99999-9999',
    0,
    1,
    TRUE
);