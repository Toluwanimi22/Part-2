CREATE DATABASE claimsystem;
USE claimsystem;

CREATE TABLE users (
    id INT AUTO_INCREMENT PRIMARY KEY,
    email VARCHAR(255) NOT NULL UNIQUE,
    password VARCHAR(255) NOT NULL
);


CREATE TABLE lecturer (
    id INT AUTO_INCREMENT PRIMARY KEY,
    name VARCHAR(255) NOT NULL,
    department VARCHAR(255) NOT NULL,
    email VARCHAR(255) NOT NULL,
    user_id INT,
    FOREIGN KEY (user_id) REFERENCES users(id)
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
    FOREIGN KEY (id) REFERENCES claims(id)
);

-- Create Admin table
CREATE TABLE Admin (
    AdminId INT AUTO_INCREMENT PRIMARY KEY,
    id INT NOT NULL,
    Role VARCHAR(100),  -- Additional admin-specific role details, if needed
    FOREIGN KEY (id) REFERENCES Users(id) ON DELETE CASCADE
);


ALTER TABLE claims ADD COLUMN month VARCHAR(20) NOT NULL;

ALTER TABLE claims
ADD COLUMN file_name VARCHAR(255),
ADD COLUMN file_path VARCHAR(255);


select * from users;
select * from Admin;
select * from lecturer;
select * from claims;
select * from supporting_documents;