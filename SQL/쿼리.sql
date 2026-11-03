-- 테이블 생성
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

-- 데이터 삽입
INSERT INTO player (username,email,password_hash) VALUES ('kimbh','kimbh0437@gmail.com','disisnotpassword')

-- 데이터 삽입
INSERT INTO player (username,email,password_hash) VALUES
('kimbh1','kimbh04371@gmail.com','disisnotpassword'),
('kimbh2','kimbh04372@gmail.com','disisnotpassword'),
('kimbh3','kimbh04373@gmail.com','disisnotpassword'),
('kimbh4','kimbh04374@gmail.com','disisnotpassword'),
('kimbh5','kimbh04375@gmail.com','disisnotpassword'),
('kimbh6','kimbh04376@gmail.com','disisnotpassword')

-- 유저 이름, 마지막 로그인 시간 가져오기
SELECT username, last_login FROM player

-- 플레이어 테이블에서 조건에 맞는 행의 시간 변경
UPDATE player SET last_login = CURRENT_TIMESTAMP WHERE username = 'kimbh'

-- 와일드 카드로 찾기
SELECT username, email FROM player WHERE username LIKE '%kimbh%'

-- 행 삭제
DELETE FROM player WHERE username = 'kimbh'

SELECT * FROM `gametest`.`player` LIMIT 1000;

-- 플레이어 테이블에 레벨 키 추가
ALTER TABLE player ADD COLUMN `level` INT DEFAULT 1

UPDATE player SET `level` = `level` + 1

SELECT username, `level` FROM player ORDER BY `level` DESC LIMIT 1

-- 아이템 테이블 추가
CREATE TABLE items(item_id INT AUTO_INCREMENT PRIMARY KEY, `name` VARCHAR(100) NOT NULL, DESCRIPTION TEXT, VALUE INT DEFAULT 0)

-- 아이템 정보 추가
INSERT INTO items(NAME, DESCRIPTION, VALUE) VALUES
('검', '기본무기', 10),
('방패', '기본 방어구', 15),
('물약', '체력을 회복', 5)

SELECT * FROM items

-- 인벤토리 테이블 추가
CREATE TABLE inventories(
	inventory_id INT AUTO_INCREMENT PRIMARY KEY,
	player_id INT,
	item_id INT,
	quantity INT DEFAULT 1,
	FOREIGN KEY(player_id) REFERENCES player(player_id),
	FOREIGN KEY(item_id) REFERENCES items(item_id)
)

INSERT INTO inventories(player_id, item_id, quantity) VALUES
(2, 1, 1),	-- 2번 플레이어 검
(3, 2, 5),	-- 3번 플레이어 방패
(4, 3, 2)	-- 4번 플레이어 물약

-- 새로운 아이템 추가
INSERT INTO items(NAME, DESCRIPTION, VALUE) VALUES
('용검', '용 기본 무기', 20)
-- 특정 플레이어의 인벤토리에 새 아이템 추가
INSERT INTO inventories(player_id, item_id, quantity) VALUES (5, 4, 2)
-- 가장 가치있는 아이템 찾기
SELECT NAME, VALUE FROM items ORDER BY VALUE DESC LIMIT 1

-- 퀘스트 테이블 생성
gametest

-- 퀘스트 데이터 삽입
INSERT INTO quests(title, DESCRIPTION, reward_exp, reward_item_id) VALUES
('초보자 퀘스트', '첫번째 퀘스트를 완료 하세요', 100, 3),
('용사의 검', '전설의 검을 찾아보세요', 500, 1)

-- 플레이어 퀘스트 진행 상황 테이블
CREATE TABLE player_quests(
	player_id INT,
	quest_id INT,
	STATUS ENUM ('시작', '진행중', '완료') DEFAULT '시작',
	start_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP(),
	completed_at TIMESTAMP NULL,
	PRIMARY KEY (player_id, quest_id),
	FOREIGN KEY (player_id) REFERENCES player(player_id),
	FOREIGN KEY (quest_id) REFERENCES quests(quest_id)
)

INSERT INTO player_quests(player_id, quest_id) VALUES
(2, 1),
(3, 2)

-- 진행중인 퀘스트 조회
SELECT p.username, q.title, pq.STATUS
FROM player p
JOIN player_quests pq ON p.player_id = pq.player_id
JOIN quests q ON pq.quest_id = q.quest_id
WHERE pq.STATUS != '완료'
