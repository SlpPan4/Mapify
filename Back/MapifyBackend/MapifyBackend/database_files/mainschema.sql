PRAGMA FOREIGN_KEYS = ON;

CREATE TABLE IF NOT EXISTS maps (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    name TEXT NOT NULL UNIQUE
);

CREATE TABLE IF NOT EXISTS bombsites (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    map_id INT NOT NULL,
    name TEXT NOT NULL,
    UNIQUE(map_id, name),
    FOREIGN KEY (map_id) REFERENCES maps(id) ON DELETE CASCADE
);

CREATE TABLE IF NOT EXISTS strats (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    name TEXT NOT NULL,
    video_url TEXT NOT NULL,
    map_id INT NOT NULL,
    description TEXT,
    bombsite_id INT NULL,
    FOREIGN KEY (map_id) REFERENCES maps(id),
    FOREIGN KEY (bombsite_id) REFERENCES bombsites(id)
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
    bombsite_id INT NULL,
    submitted_at TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (map_id) REFERENCES maps(id),
    FOREIGN KEY (bombsite_id) REFERENCES bombsites(id)
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

-- Strats are seeded with explicit ids so INSERT OR IGNORE actually dedupes on restart
-- (without an id, AUTOINCREMENT would insert a fresh copy on every container start).
INSERT OR IGNORE INTO strats(id, name, video_url, map_id, description)
VALUES (1, 'Cool Ash Rush', 'https://www.youtube.com/watch?v=dQw4w9WgXcQ', 7, 'Full five-man rush through the main door: Ash leads with flashes and the plant goes down before the roamers can rotate back.'),
       (2, 'Thermite breach', 'https://www.youtube.com/watch?v=9bZkp7q19f0', 2, 'Open the garage wall with an exothermic charge, smoke off the site and plant behind the car.'),
       (3, 'Lesion 2f setup', 'https://www.youtube.com/watch?v=aqz-KE-bpKQ', 4, 'Blanket the second floor with Gu mines to slow the push and feed the anchors a steady stream of intel.'),
       (4, 'Nokk Silent Kitchen Push', 'https://www.youtube.com/watch?v=oavMtUWDBTM', 1, 'HEL muffles Nokk''s steps while the rest of the team baits noise on the far side — she walks straight into kitchen unnoticed.'),
       (5, 'Buck Vertical Mayhem', 'https://www.youtube.com/watch?v=ZZ5LpwO-An4', 9, 'Buck opens the floor above site with the Skeleton Key and deletes anchors from below their feet.'),
       (6, 'Thermite Exothermic Execute', 'https://www.youtube.com/watch?v=feA64wXhbjo', 4, 'Hard-breach the garage wall, smoke the plant spot and stick the default plant behind the car.'),
       (7, 'Doc Stim Anchor Clinic', 'https://www.youtube.com/watch?v=dQw4w9WgXcQ', 2, 'Hold the pixel angle, stim teammates through the execute and outlast the retake.'),
       (8, 'Azami Kiba Fortress', 'https://www.youtube.com/watch?v=9bZkp7q19f0', 6, 'Kiba barriers rebuild the site mid-round: block the doorway, force the funnel, win the trade.'),
       (9, 'Lesion Gu Mine Parade', 'https://www.youtube.com/watch?v=oavMtUWDBTM', 15, 'A carpet of Gu mines across the approach routes turns the clock into your sixth teammate.'),
       (10, 'Skopos Double Trouble Hold', 'https://www.youtube.com/watch?v=ZZ5LpwO-An4', 12, 'Swap between shells to hold two angles at once — attackers never know which one is live.');


INSERT OR IGNORE INTO categories(id, name, side)
VALUES (1, 'Rush', 'Attack'),
       (2, 'Default', 'Attack'),
       (3, 'Fast plant', 'Attack'),
       (4, 'Frag grenade lineup', 'Attack'),
       (5, 'Power Position Hold', 'Defense'),
       (6, 'Deep roam', 'Defense'),
       (7, 'Soft roam', 'Defense'),
       (8, 'Anchoring', 'Defense'),
       (9, 'Defense', 'Defense'),
       (10, 'Attack', 'Attack'),
       (11, 'Execute', 'Attack'),
       (12, 'Vertical Play', 'Attack'),
       (13, 'Hard Breach', 'Attack'),
       (14, 'Site Setup', 'Defense'),
       (15, 'Breach Denial', 'Defense'),
       (16, 'Info Game', 'Defense');

INSERT OR IGNORE INTO strat_categories(strat_id, category_id)
VALUES (1, 1),
       (2, 3),
       (3, 5),
       (4, 1),
       (4, 11),
       (5, 12),
       (6, 13),
       (6, 3),
       (7, 8),
       (7, 14),
       (8, 14),
       (8, 5),
       (9, 6),
       (9, 16),
       (10, 5),
       (10, 14);

INSERT OR IGNORE INTO strat_operators(strat_id, operator_id)
VALUES (1, 1),
       (2, 4),
       (3, 6),
       (4, 2),
       (5, 3),
       (6, 4),
       (7, 5),
       (8, 7),
       (9, 6),
       (10, 8);

-- Seed pending strat submissions for admin testing
INSERT OR IGNORE INTO pending_strat_submissions(id, name, video_url, map_id, description)
VALUES (1, 'Nokk silent push', 'https://www.youtube.com/watch?v=dQw4w9WgXcQ', 1, 'Flank with Nokk and catch defenders off guard on Oregon.'),
       (2, 'Buck vertical clear', 'https://www.youtube.com/watch?v=9bZkp7q19f0', 2, 'Use Buck skeleton key to clear out common defender positions from above.'),
       (3, 'Thermite garage breach', 'https://www.youtube.com/watch?v=aqz-KE-bpKQ', 3, 'Open garage wall with Thermite for a fast plant.'),
       (4, 'Doc anchor hold', 'https://www.youtube.com/watch?v=dQw4w9WgXcQ', 4, 'Hold site with Doc and keep teammates alive.'),
       (5, 'Azami barricade setup', 'https://www.youtube.com/watch?v=9bZkp7q19f0', 5, 'Use Azami Kiba barriers to delay attackers on Border.'),
       (6, 'Lesion deep roam', 'https://www.youtube.com/watch?v=dQw4w9WgXcQ', 7, 'Place Gu mines and waste attacker time on Coastline.');

INSERT OR IGNORE INTO pending_strat_submission_categories(submission_id, category_id)
VALUES (1, 1),
       (2, 4),
       (3, 3),
       (4, 8),
       (5, 5),
       (6, 6);

INSERT OR IGNORE INTO pending_strat_submission_operators(submission_id, operator_id)
VALUES (1, 2),
       (2, 3),
       (3, 4),
       (4, 5),
       (5, 7),
       (6, 6);
