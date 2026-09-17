# Student Attendance Management System

A web-based application for managing student records and daily attendance in an academic institution, built with ASP.NET Core MVC and SQL Server.

## Table of Contents

- [Problem Statement](#problem-statement)
- [Objectives](#objectives)
- [Features](#features)
- [Technologies Used](#technologies-used)
- [System Workflow](#system-workflow)
- [Database Structure](#database-structure)
- [Project Structure](#project-structure)
- [Installation & Setup](#installation--setup)
- [How to Run in Visual Studio](#how-to-run-in-visual-studio)
- [Default Login Credentials](#default-login-credentials)
- [Git / GitHub Workflow](#git--github-workflow)
- [Team Member Contributions](#team-member-contributions)
- [Screenshots](#screenshots)

## Problem Statement

Manual attendance tracking in colleges using paper registers or spreadsheets is slow, error-prone, and makes it difficult to identify students with poor attendance in time for corrective action. Faculty and administrators need a centralized, reliable system to record daily attendance and generate accurate reports on demand.

## Objectives

- Provide a simple, professional system for managing student records.
- Allow faculty to mark daily attendance quickly by department, year, and section.
- Prevent duplicate attendance entries for the same student on the same date.
- Automatically calculate attendance percentage per student.
- Flag students whose attendance falls below a configurable threshold.
- Provide role-based access for Admin and Faculty users.

## Features

### Dashboard
- Total students, present/absent today, today's attendance percentage
- Recent attendance activity feed

### Student Management
- Add, edit, delete, view, search, and filter students
- Server-side validation on all required fields
- Duplicate register number prevention

### Attendance Management
- Filter class by date, department, year, and section
- Mark Present/Absent per student
- "Mark All Present / Absent" quick actions
- Duplicate attendance entries for the same date are prevented (marking again updates the existing record instead of duplicating it)

### Attendance Records
- View records by date, student, or register number
- Filter by department and date
- Update (toggle) a recorded status
- Paginated table view

### Attendance Reports
- Calculates Total Working Days, Days Present, Days Absent, and Attendance Percentage
- Formula: `Attendance % = (Days Present / Total Working Days) × 100`
- Filter by student, department, year, section, and date range

### Low Attendance
- Lists students below a configurable threshold (default 75%)
- Threshold is adjustable directly from the page

### Authentication & Roles
- Cookie-based login system
- **Admin**: manage students, manage users, view all reports
- **Faculty**: view students, mark attendance, view records and reports
- Passwords hashed using ASP.NET Core's `PasswordHasher` (never stored in plain text)

### UI
- Responsive layout with sidebar + top navigation
- Dashboard cards, paginated tables, search/filter bars
- Delete confirmation dialogs, form validation, success/error alerts

## Technologies Used

| Layer            | Technology                          |
|-------------------|--------------------------------------|
| Language          | C#                                    |
| Framework         | ASP.NET Core MVC (.NET 8)             |
| Database          | SQL Server (LocalDB by default)       |
| ORM               | Entity Framework Core 8               |
| Frontend          | HTML5, CSS3, Bootstrap 5, JavaScript  |
| Auth              | ASP.NET Core Cookie Authentication    |
| IDE               | Visual Studio 2022                    |
| Version Control   | Git & GitHub                          |

## System Workflow

```
Login
  ↓
Dashboard
  ↓
Student Management → Add/View Students
  ↓
Attendance Management → Select Date & Class → Mark Present/Absent → Save
  ↓
Attendance Records → Search / Filter / Update
  ↓
Attendance Reports
  ↓
Low Attendance Analysis
```

## Database Structure

### Students
| Column          | Type          | Notes                        |
|------------------|---------------|-------------------------------|
| StudentId        | int, PK       | Identity                      |
| RegisterNumber   | nvarchar(50)  | Unique                        |
| StudentName      | nvarchar(100) |                                |
| Department       | nvarchar(100) |                                |
| Year             | int           | 1–5                            |
| Section          | nvarchar(10)  |                                |
| Email            | nvarchar(150) |                                |
| PhoneNumber      | nvarchar(20)  |                                |

### Attendance
| Column          | Type       | Notes                                   |
|------------------|------------|-------------------------------------------|
| AttendanceId     | int, PK    | Identity                                  |
| StudentId        | int, FK    | References Students.StudentId (cascade)   |
| AttendanceDate   | date       |                                            |
| Status           | int        | 1 = Present, 0 = Absent                   |
| CreatedAt        | datetime2  | Default: current UTC time                 |

Unique constraint on `(StudentId, AttendanceDate)` prevents duplicate attendance for the same student on the same day.

### Users
| Column          | Type          | Notes                        |
|------------------|---------------|-------------------------------|
| UserId           | int, PK       | Identity                      |
| Username         | nvarchar(50)  | Unique                        |
| PasswordHash     | nvarchar(max) | Hashed, never plain text      |
| Role             | int           | 1 = Admin, 2 = Faculty         |

See `Database/Database.sql` for the full reference schema (EF Core Migrations generate this automatically — you do not need to run the SQL file manually if you use Migrations).

## Project Structure

```
Student-Attendance-Management-System/
│
├── SourceCode/
│   └── StudentAttendanceManagementSystem/   (ASP.NET Core MVC project)
│       ├── Controllers/
│       ├── Models/
│       │   └── ViewModels/
│       ├── Data/
│       ├── Services/
│       ├── Views/
│       │   ├── Dashboard/
│       │   ├── Students/
│       │   ├── Attendance/
│       │   ├── Reports/
│       │   ├── Users/
│       │   └── Account/
│       ├── wwwroot/
│       │   ├── css/
│       │   ├── js/
│       │   └── images/
│       ├── Migrations/
│       ├── appsettings.json
│       ├── Program.cs
│       └── StudentAttendanceManagementSystem.csproj
│
├── Database/
│   └── Database.sql
│
├── Screenshots/
│
├── README.md
└── .gitignore
```

## Installation & Setup

### Prerequisites
- [Visual Studio 2022](https://visualstudio.microsoft.com/) (Community or higher) with the **ASP.NET and web development** workload
- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- SQL Server LocalDB (installed with Visual Studio) or a full SQL Server instance
- Git

### Clone the repository
```bash
git clone https://github.com/<your-username>/Student-Attendance-Management-System.git
cd Student-Attendance-Management-System/SourceCode/StudentAttendanceManagementSystem
```

## How to Run in Visual Studio

1. Open `StudentAttendanceManagementSystem.csproj` (or the solution) in Visual Studio 2022.
2. Restore NuGet packages: right-click the solution → **Restore NuGet Packages** (or it happens automatically on build).
3. Check the connection string in `appsettings.json`. The default works with LocalDB out of the box:
   ```
   Server=(localdb)\mssqllocaldb;Database=StudentAttendanceDb;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True
   ```
4. Open **Package Manager Console** (Tools → NuGet Package Manager → Package Manager Console) and create the initial migration:
   ```powershell
   Add-Migration InitialCreate
   Update-Database
   ```
   (Alternatively, from a terminal in the project folder: `dotnet ef migrations add InitialCreate` then `dotnet ef database update`.)
5. Press **F5** (or click **Run**) to build and launch the application. The app applies any pending migrations and seeds default users automatically on startup.
6. Your browser opens to the Login page.

## Default Login Credentials

| Role    | Username | Password    |
|---------|----------|-------------|
| Admin   | admin    | Admin@123   |
| Faculty | faculty  | Faculty@123 |

**Change these passwords (or create new users via Manage Users) before using this in a real environment.**

## Git / GitHub Workflow

Suggested commit sequence for a clean, readable history:

```
Initial project setup
Created database models
Implemented student management
Implemented attendance module
Added attendance reports
Added authentication
Improved dashboard UI
Added project documentation
```

Recommended branching:
- `main` — stable, always-working code
- `feature/student-management`
- `feature/attendance-module`
- `feature/reports`
- `feature/authentication`
- `feature/ui-improvements`

Each team member works on their own feature branch and opens a pull request into `main` so their contribution is visible in the GitHub history.

## Team Member Contributions

| Name | Role | Contribution |
|------|------|---------------|
| _<team member 1>_ | _<e.g. Backend>_ | _<describe contribution>_ |
| _<team member 2>_ | _<e.g. Frontend>_ | _<describe contribution>_ |
| _<team member 3>_ | _<e.g. Database>_ | _<describe contribution>_ |
| _<team member 4>_ | _<e.g. Testing/Docs>_ | _<describe contribution>_ |

*(Fill in with actual team member names and contributions.)*

## Screenshots

_Add screenshots of the Dashboard, Student Management, Mark Attendance, Attendance Records, Reports, and Low Attendance pages here, e.g.:_

```
![Dashboard](Screenshots/dashboard.png)
![Student Management](Screenshots/students.png)
![Mark Attendance](Screenshots/mark-attendance.png)
```
