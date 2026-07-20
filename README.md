# 🚀 Task Management System

A modern Full Stack Task Management System built using **ASP.NET Core Web API (.NET 10)**, **Angular 20**, **SQL Server**, and **JWT Authentication**. The application enables organizations to efficiently manage tasks, assign work, monitor progress, and maintain audit logs with secure role-based access control.

---

# 📌 Project Overview

The Task Management System is designed to simplify task planning, assignment, tracking, and monitoring within an organization.

The application provides:

- Secure JWT Authentication
- Role-Based Authorization
- Task Management
- Dashboard & Statistics
- Audit Logs
- Server-side Filtering & Pagination
- Clean Layered Architecture

---

# 🏗 Architecture

The project follows a **Layered Architecture** to ensure separation of concerns, maintainability, scalability, and testability.

```
Presentation Layer
│
├── Angular 20 Frontend
│
▼
ASP.NET Core Web API
│
▼
Application Layer
│
▼
Domain Layer
│
▼
Infrastructure Layer
│
▼
SQL Server
```

### Backend Structure

```
BackEnd
│
├── TaskManagement.API
├── TaskManagement.Application
├── TaskManagement.Domain
└── TaskManagement.Infrastructure
```

### Frontend Structure

```
FrontEnd
│
└── task-management-ui
```

---

# 🛠 Technology Stack

## Backend

- ASP.NET Core Web API (.NET 10)
- C#
- Entity Framework Core
- SQL Server
- JWT Authentication
- Dependency Injection
- Repository Pattern
- Unit of Work
- Swagger

## Frontend

- Angular 20
- TypeScript
- Angular Routing
- Angular Guards
- HTTP Client
- Reactive Forms
- SCSS

## Database

- SQL Server
- Entity Framework Core
- Code First

---

# ✨ Features

## Authentication

- User Registration
- Secure Login
- JWT Token Authentication
- Password Hashing
- Role-Based Authorization

---

## Dashboard

- Total Tasks
- Pending Tasks
- In Progress Tasks
- Completed Tasks
- Overdue Tasks

---

## Task Management

- Create Task
- Update Task
- Delete Task
- Assign Users
- Set Priority
- Set Status
- Due Date Management

---

## Search & Filtering

- Search Tasks
- Filter by Status
- Filter by Priority
- Sort Tasks
- Pagination

---

## Audit Logs

Tracks important system activities such as:

- User Login
- Task Creation
- Task Updates
- Task Deletion

---

## Security

- JWT Authentication
- Authorization Policies
- Protected APIs
- Route Guards
- Password Hashing

---

# 📂 Folder Structure

```
TaskManagementSystem
│
├── BackEnd
│   ├── TaskManagement.API
│   ├── TaskManagement.Application
│   ├── TaskManagement.Domain
│   └── TaskManagement.Infrastructure
│
├── FrontEnd
│   └── task-management-ui
│
├── DataBase
│   └── TaskManagement_Database_Schema.sql
│
└── TaskFlow_Design_Document.docx
```

---

# 🚀 Getting Started

## Backend

### Clone Repository

```bash
git clone https://github.com/alaukikkumarsharma-max/TaskManagementSystem.git
```

### Navigate

```bash
cd TaskManagementSystem/BackEnd
```

### Restore Packages

```bash
dotnet restore
```

### Update Connection String

Edit

```
appsettings.json
```

```json
"ConnectionStrings": {
  "DefaultConnection": "Your SQL Server Connection String"
}
```

### Apply Database

Run the provided SQL script

```
DataBase/TaskManagement_Database_Schema.sql
```

or

```bash
dotnet ef database update
```

### Run Backend

```bash
dotnet run
```

Swagger

```
https://localhost:5001/swagger
```

---

# Angular Frontend

Navigate

```bash
cd FrontEnd/task-management-ui
```

Install Packages

```bash
npm install
```

Run Angular

```bash
ng serve
```

Application URL

```
http://localhost:4200
```

---

# 🔐 Demo Credentials

## Administrator

```
Email:
admin@taskflow.com

Password:
Admin@123
```

## Manager

```
Email:
manager@taskflow.com

Password:
Manager@123
```

## Employee

```
Email:
employee@taskflow.com

Password:
Employee@123
```

> Replace these credentials with the seeded users from your database if they differ.

---

# 📸 Screenshots

## Login

_Add Login Page Screenshot_

---

## Dashboard

_Add Dashboard Screenshot_

---

## Task List

_Add Task List Screenshot_

---

## Create Task

_Add Create Task Screenshot_

---

## Audit Logs

_Add Audit Logs Screenshot_

---

# 📊 API Documentation

Swagger UI

```
https://localhost:5001/swagger
```

---

# 🔄 Request Flow

```
Angular UI

      │

HTTP Request

      │

ASP.NET Core API

      │

Application Layer

      │

Repository

      │

Entity Framework Core

      │

SQL Server
```

---

# 📈 Future Enhancements

- Email Notifications
- File Attachments
- SignalR Real-time Updates
- Azure Deployment
- Docker Support
- Kubernetes Deployment
- Redis Caching
- Background Jobs
- CI/CD Pipeline
- Unit Testing
- Integration Testing

---

# 👨‍💻 Author

**Alaukik Kumar Sharma**

.NET Full Stack Developer

- ASP.NET Core
- Angular
- SQL Server
- Azure
- Microservices
- System Design

GitHub:
https://github.com/alaukikkumarsharma-max

---

# ⭐ Thank You

Thank you for reviewing this project.

I hope this application demonstrates my skills in:

- Full Stack Development
- Clean Architecture
- REST API Design
- Angular Development
- Database Design
- Authentication & Authorization
- Enterprise Application Development
