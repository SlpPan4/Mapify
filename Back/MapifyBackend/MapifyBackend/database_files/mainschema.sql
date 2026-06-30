PRAGMA FOREIGN_KEYS = ON;

CREATE TABLE IF NOT EXISTS maps (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    name TEXT NOT NULL UNIQUE
);

CREATE TABLE IF NOT EXISTS strats (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    name TEXT NOT NULL,
    video_url TEXT NOT NULL,
    map_id INT NOT NULL,
    description TEXT,
    FOREIGN KEY (map_id) REFERENCES maps(id)
);


CREATE TABLE IF NOT EXISTS operators (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    name TEXT NOT NULL UNIQUE ,
    side TEXT CHECK(side IN ('Attack', 'Defense'))
);

CREATE TABLE IF NOT EXISTS categories (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    name TEXT NOT NULL UNIQUE,
    side TEXT CHECK(side IN ('Attack', 'Defense'))
);

CREATE TABLE IF NOT EXISTS strat_categories (
    strat_id INT NOT NULL,
    category_id INT NOT NULL,
    PRIMARY KEY (strat_id, category_id),
    FOREIGN KEY (strat_id) REFERENCES strats(id) ON DELETE CASCADE,
    FOREIGN KEY (category_id) REFERENCES categories(id) ON DELETE CASCADE
);

CREATE TABLE IF NOT EXISTS strat_operators (
    strat_id INT NOT NULL,
    operator_id INT NOT NULL,
    PRIMARY KEY (strat_id, operator_id),
    FOREIGN KEY (strat_id) REFERENCES strats(id) ON DELETE CASCADE,
    FOREIGN KEY (operator_id) REFERENCES operators(id) ON DELETE CASCADE
);

CREATE TABLE IF NOT EXISTS admin_api_keys (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    name TEXT,
    key_hash TEXT NOT NULL UNIQUE,
    created_at TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP,
    revoked_at TEXT
);

CREATE TABLE IF NOT EXISTS pending_category_submissions (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    name TEXT NOT NULL,
    side TEXT NOT NULL CHECK(side IN ('Attack', 'Defense')),
    submitted_at TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP
);

CREATE TABLE IF NOT EXISTS pending_strat_submissions (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    name TEXT NOT NULL,
    video_url TEXT NOT NULL,
    map_id INT NOT NULL,
    description TEXT,
    submitted_at TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (map_id) REFERENCES maps(id)
);

CREATE TABLE IF NOT EXISTS pending_strat_submission_categories (
    submission_id INT NOT NULL,
    category_id INT NOT NULL,
    PRIMARY KEY (submission_id, category_id),
    FOREIGN KEY (submission_id) REFERENCES pending_strat_submissions(id) ON DELETE CASCADE,
    FOREIGN KEY (category_id) REFERENCES categories(id) ON DELETE CASCADE
);

CREATE TABLE IF NOT EXISTS pending_strat_submission_operators (
    submission_id INT NOT NULL,
    operator_id INT NOT NULL,
    PRIMARY KEY (submission_id, operator_id),
    FOREIGN KEY (submission_id) REFERENCES pending_strat_submissions(id) ON DELETE CASCADE,
    FOREIGN KEY (operator_id) REFERENCES operators(id) ON DELETE CASCADE
);

INSERT OR IGNORE INTO maps(name)
VALUES ('Oregon'),
       ('Consulate'),
       ('Bank'),
       ('Clubhouse'),
       ('Border'),
       ('Fortress'),
       ('Coastline'),
       ('Chalet'),
       ('Kafe'),
       ('Outback'),
       ('Nighthaven Labs'),
       ('Lair'),
       ('Kanal'),
       ('Villa'),
       ('Skyscraper'),
       ('Theme park'),
       ('Emerald Plains'),
       ('Favela'),
       ('Tower'),
       ('Yacht'),
       ('Presidential Plane'),
       ('Stadium Bravo'),
       ('Stadium 2020');

INSERT OR IGNORE INTO operators(name, side)
VALUES ('Ash', 'Attack'),
       ('Nokk', 'Attack'),
       ('Buck', 'Attack'),
       ('Thermite', 'Attack'),
       ('Doc', 'Defense'),
       ('Lesion', 'Defense'),
       ('Azami', 'Defense'),
       ('Skopos', 'Defense');

INSERT OR IGNORE INTO strats(name, video_url, map_id, description)
VALUES ('Cool Ash Rush', 'youtube.com', '7', ''),
       ('Thermite breach', 'linkedin.com', '2', 'cool description'),
       ('Lesion 2f setup', 'instagram.com', '4', '');
       
       
INSERT OR IGNORE INTO categories(name, side)
VALUES ('Rush', 'Attack'),
       ('Default', 'Attack'),
       ('Fast plant', 'Attack'),
       ('Frag grenade lineup', 'Attack'),
       ('Power Position Hold', 'Defense'),
       ('Deep roam', 'Defense'),
       ('Soft roam', 'Defense'),
       ('Anchoring', 'Defense');
       
INSERT OR IGNORE INTO strat_categories(strat_id, category_id)
VALUES (1, 1),
       (2, 3),
       (3, 5);
       
INSERT OR IGNORE INTO strat_operators(strat_id, operator_id)
VALUES (1, 1),
       (2, 4),
       (3, 6)
