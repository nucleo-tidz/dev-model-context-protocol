-- V1: initial schema
-- Add your SQL here, e.g:
 CREATE TABLE bookings (
     id INT PRIMARY KEY IDENTITY,
     vessel_id NVARCHAR(50) NOT NULL,
     origin NVARCHAR(100) NOT NULL,
     destination NVARCHAR(100) NOT NULL,
     created_at DATETIME2 DEFAULT GETUTCDATE()
 );
