-- PSQL scratch pad

CREATE ROLE myuser LOGIN PASSWORD 'strongPassword!234';
CREATE DATABASE SafeStates WITH OWNER = myuser;

CREATE TABLE safe (
    safe_id SERIAL PRIMARY KEY,
    password TEXT NOT NULL,
    admin_password TEXT NOT NULL
);

CREATE TABLE safe_state_history (
    history_id SERIAL PRIMARY KEY,
    safe_id INT NOT NULL,
    state INT NOT NULL,
    created_at TIMESTAMPTZ DEFAULT NOW(),
    FOREIGN KEY (safe_id) REFERENCES safe(safe_id) ON DELETE CASCADE
);

-- Mock data insert statements
INSERT INTO safe (password, admin_password) VALUES 
('1234', '1111'),
('5678', '2222'),
('9012', '3333')
RETURNING safe_id;

INSERT INTO safe_history (safe_id, state) VALUES 
(1, 1),
(2, 1),
(3, 1);

-- Update state to safe 1 
INSERT INTO safe_history (safe_id, state) VALUES 
(1, 2);

-- Query to find the safe id and the current state of a safe. 
SELECT sh.safe_id, sh.state, sh.created_at
FROM safe_history sh
WHERE sh.created_at = (
    SELECT MAX(created_at) 
    FROM safe_history 
    WHERE safe_id = sh.safe_id
) AND sh.safe_id = 1 ; 
