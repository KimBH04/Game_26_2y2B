CREATE TABLE `player` (
	`player_id` INT NOT NULL AUTO_INCREMENT,
	`username` VARCHAR(50) NOT NULL DEFAULT '0' COLLATE 'utf8mb4_0900_ai_ci',
	`email` VARCHAR(50) NOT NULL COLLATE 'utf8mb4_0900_ai_ci',
	`password_hash` VARCHAR(255) NOT NULL COLLATE 'utf8mb4_0900_ai_ci',
	`created_at` TIMESTAMP NOT NULL DEFAULT (now()),
	`last_login` TIMESTAMP NULL DEFAULT NULL,
	PRIMARY KEY (`player_id`) USING BTREE,
	UNIQUE INDEX `username` (`username`) USING BTREE,
	UNIQUE INDEX `email` (`email`) USING BTREE
)
COLLATE='utf8mb4_0900_ai_ci'
ENGINE=InnoDB
;

INSERT INTO player (username,email,password_hash) VALUES ('kimbh','kimbh0437@gmail.com','disisnotpassword')

-- 데이터 삽입
INSERT INTO player (username,email,password_hash) VALUES
('kimbh1','kimbh04371@gmail.com','disisnotpassword'),
('kimbh2','kimbh04372@gmail.com','disisnotpassword'),
('kimbh3','kimbh04373@gmail.com','disisnotpassword'),
('kimbh4','kimbh04374@gmail.com','disisnotpassword'),
('kimbh5','kimbh04375@gmail.com','disisnotpassword'),
('kimbh6','kimbh04376@gmail.com','disisnotpassword')

SELECT username, last_login FROM player

UPDATE player SET last_login = CURRENT_TIMESTAMP WHERE username = 'kimbh'

SELECT username, email FROM player WHERE username LIKE '%kimbh%'

DELETE FROM player WHERE username = 'kimbh'

