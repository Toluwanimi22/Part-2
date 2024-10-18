CREATE DATABASE claimsystem;
USE claimsystem;
-- drop schema claimsystem;
CREATE TABLE users (
    id INT AUTO_INCREMENT PRIMARY KEY,
    full_name VARCHAR(100) NOT NULL,
    email VARCHAR(100) NOT NULL UNIQUE,
    password VARCHAR(255) NOT NULL
);

CREATE TABLE lecturer (
    id INT AUTO_INCREMENT PRIMARY KEY,
    name VARCHAR(100) NOT NULL,
    department VARCHAR(100) NOT NULL,
    email VARCHAR(100) NOT NULL UNIQUE
);

CREATE TABLE claims (
    id INT AUTO_INCREMENT PRIMARY KEY,
    lecturer_id INT,
    hours_worked DECIMAL(10, 2) NOT NULL,
    hourly_rate DECIMAL(10, 2) NOT NULL,
    total_claim DECIMAL(10, 2) NOT NULL,
    status VARCHAR(50) DEFAULT 'Pending',
    FOREIGN KEY (lecturer_id) REFERENCES lecturer(id)
);

ALTER TABLE claims ADD COLUMN rejection_reason VARCHAR(255);


CREATE TABLE supporting_documents (
    id INT AUTO_INCREMENT PRIMARY KEY,
    claim_id INT,
    file_name VARCHAR(255),
    file_path VARCHAR(255),
    FOREIGN KEY (claim_id) REFERENCES claims(id)
);

insert into lecturer(id, name, department, email)
values(1, 'Tumi', 'IT', 'tumi@gmail.com');

INSERT INTO claims (lecturer_id, hours_worked, hourly_rate, total_claim, status) 
VALUES 
(1, 10, 50, 500, 'Pending'),
(1, 15, 45, 675, 'Pending'),
(1, 20, 60, 1200, 'Pending'),
(1, 8, 55, 440, 'Pending'),
(1, 12, 50, 600, 'Pending');

select * from users;
select * from lecturer;
select * from claims;
select * from supporting_documents;