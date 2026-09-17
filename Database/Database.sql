-- ============================================================
-- Student Attendance Management System - Database Schema
-- Run this manually only if you are NOT using EF Core Migrations.
-- If you are using EF Core Migrations (recommended), this file
-- is provided as a reference / documentation of the schema that
-- Migrations will generate.
-- ============================================================

IF DB_ID('StudentAttendanceDb') IS NULL
BEGIN
    CREATE DATABASE StudentAttendanceDb;
END
GO

USE StudentAttendanceDb;
GO

-- ============================================================
-- Table: Students
-- ============================================================
IF OBJECT_ID('dbo.Students', 'U') IS NOT NULL DROP TABLE dbo.Students;
GO

CREATE TABLE dbo.Students (
    StudentId       INT             IDENTITY(1,1) PRIMARY KEY,
    RegisterNumber  NVARCHAR(50)    NOT NULL,
    StudentName     NVARCHAR(100)   NOT NULL,
    Department      NVARCHAR(100)   NOT NULL,
    Year            INT             NOT NULL CHECK (Year BETWEEN 1 AND 5),
    Section         NVARCHAR(10)    NOT NULL,
    Email           NVARCHAR(150)   NOT NULL,
    PhoneNumber     NVARCHAR(20)    NOT NULL,
    CONSTRAINT UQ_Students_RegisterNumber UNIQUE (RegisterNumber)
);
GO

-- ============================================================
-- Table: Attendance
-- ============================================================
IF OBJECT_ID('dbo.Attendance', 'U') IS NOT NULL DROP TABLE dbo.Attendance;
GO

CREATE TABLE dbo.Attendance (
    AttendanceId    INT             IDENTITY(1,1) PRIMARY KEY,
    StudentId       INT             NOT NULL,
    AttendanceDate  DATE            NOT NULL,
    Status          INT             NOT NULL,     -- 1 = Present, 0 = Absent
    CreatedAt       DATETIME2       NOT NULL DEFAULT SYSUTCDATETIME(),
    CONSTRAINT FK_Attendance_Students FOREIGN KEY (StudentId)
        REFERENCES dbo.Students (StudentId) ON DELETE CASCADE,
    -- Prevent duplicate attendance entries for the same student on the same date
    CONSTRAINT UQ_Attendance_Student_Date UNIQUE (StudentId, AttendanceDate)
);
GO

CREATE INDEX IX_Attendance_AttendanceDate ON dbo.Attendance (AttendanceDate);
GO

-- ============================================================
-- Table: Users
-- ============================================================
IF OBJECT_ID('dbo.Users', 'U') IS NOT NULL DROP TABLE dbo.Users;
GO

CREATE TABLE dbo.Users (
    UserId          INT             IDENTITY(1,1) PRIMARY KEY,
    Username        NVARCHAR(50)    NOT NULL,
    PasswordHash    NVARCHAR(MAX)   NOT NULL,
    Role            INT             NOT NULL,     -- 1 = Admin, 2 = Faculty
    CONSTRAINT UQ_Users_Username UNIQUE (Username)
);
GO

-- ============================================================
-- Notes
-- ============================================================
-- Default seed users are created automatically by the application
-- on first run (see Data/DbInitializer.cs):
--   admin   / Admin@123   (Role = Admin)
--   faculty / Faculty@123 (Role = Faculty)
-- Passwords are hashed with ASP.NET Core's PasswordHasher, never stored in plain text.
