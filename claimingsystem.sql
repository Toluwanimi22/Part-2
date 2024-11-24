CREATE DATABASE ClaimSystem;
USE ClaimSystem;

-- Users Table
CREATE TABLE Users (
    UserId INT AUTO_INCREMENT PRIMARY KEY,
    Email VARCHAR(255) NOT NULL UNIQUE,
    Password VARCHAR(255) NOT NULL -- Renamed for clarity, indicating it's hashed
);

-- Lecturer Table
CREATE TABLE Lecturers (
    LecturerId INT AUTO_INCREMENT PRIMARY KEY,
    FullName VARCHAR(255) NOT NULL, -- Renamed for better readability
    Department VARCHAR(255) NOT NULL,
    Email VARCHAR(255) NOT NULL,
    UserId INT,
    FOREIGN KEY (UserId) REFERENCES Users(UserId)
);

-- Claims Table
CREATE TABLE Claims (
    ClaimId INT AUTO_INCREMENT PRIMARY KEY,
    LecturerId INT,
    HoursWorked DECIMAL(10, 2) NOT NULL,
    HourlyRate DECIMAL(10, 2) NOT NULL,
    TotalClaim DECIMAL(10, 2) AS (HoursWorked * HourlyRate) STORED, -- Calculated column
    Status ENUM('Pending', 'Approved', 'Rejected') DEFAULT 'Pending', -- ENUM for status
    RejectionReason VARCHAR(255) DEFAULT NULL, -- Optional rejection reason
    ClaimMonth VARCHAR(20) NOT NULL,
    FileName VARCHAR(255),
    FilePath VARCHAR(255),
    FOREIGN KEY (LecturerId) REFERENCES Lecturers(LecturerId) -- Assuming 'Lecturers' table exists with 'LecturerId'
);

-- Supporting Documents Table
CREATE TABLE SupportingDocuments (
    DocumentId INT AUTO_INCREMENT PRIMARY KEY,
    ClaimId INT,
    FileName VARCHAR(255),
    FilePath VARCHAR(255),
    FOREIGN KEY (ClaimId) REFERENCES Claims(ClaimId) -- Corrected the reference
);

-- Admins Table with Roles
CREATE TABLE Admins (
    AdminId INT AUTO_INCREMENT PRIMARY KEY,
    UserId INT NOT NULL, -- Links to the Users table
    Role ENUM('Program Coordinator', 'Academic Manager', 'HR') NOT NULL, -- Predefined roles
    FOREIGN KEY (UserId) REFERENCES Users(UserId) ON DELETE CASCADE
);


-- Sample Queries
SELECT * FROM Users;
SELECT * FROM Admins;
SELECT * FROM Lecturers;
SELECT * FROM Claims;
SELECT * FROM SupportingDocuments;


SELECT u.UserID, u.Password, a.Role 
FROM Users u 
JOIN Admins a ON u.UserID = a.AdminId 
WHERE u.Email = @Email;
-- Joining Claims with Lecturer for Summary
SELECT 
    c.ClaimId,
    c.LecturerId,
    c.HoursWorked,
    c.HourlyRate,
    c.TotalClaim,
    c.Status,
    c.ClaimMonth,
    c.FileName AS SupportingDocumentName,
    c.FilePath AS SupportingDocumentPath,
    c.RejectionReason,
    l.FullName AS LecturerName,
    l.Department AS LecturerDepartment,
    l.Email AS LecturerEmail
FROM Claims c
JOIN Lecturers l ON c.LecturerId = l.LecturerId;


SELECT c.ClaimId, l.FullName, l.Department, c.ClaimMonth, c.HoursWorked, c.HourlyRate, c.TotalClaim, c.Status 
FROM claims c
JOIN lecturers l ON c.LecturerId = l.LecturerId;
