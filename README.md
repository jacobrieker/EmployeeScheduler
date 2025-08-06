# Employee Scheduler

**Employee Scheduler** is a full-stack web application that allows employees to view their upcoming shifts, clock in and out, track their hours worked, and view personal statistics — while giving managers and admins the ability to assign shifts, manage users, and monitor time logs across the organization.

This app is designed for individuals, teams, and organizations to efficiently manage work schedules and time tracking from a clean, intuitive interface.

The first user to register is automatically assigned the **"owner"** role and has full administrative access.

---

## Features

### For Employees
- Register and securely log in
- View your scheduled shifts on a calendar
- Clock in and clock out of assigned shifts
- Review personal time logs
- View statistics such as total hours worked, number of shifts, and more

### For Admins and Owners
- View and manage all users
- Create, edit, and delete shifts for any user
- Review time logs across the organization
- Promote, hire, and fire users
- Access full administrative dashboards and tools

---

## Tech Stack

- **Frontend**: React.js
- **Backend**: ASP.NET Core (C#)
- **Database**: SQLite using Entity Framework Core
- **Authentication**: Cookie-based login and role system

---

## Getting Started

Follow these steps to clone and run the project locally.

### 1. Clone the repository

```bash
git clone https://github.com/jacobrieker/employee-scheduler.git
cd employee-scheduler/EmployeeScheduler.Server
```

### 2. Restore dependencies
```bash
dotnet restore
```

### 3. Create local database
```bash
dotnet ef database update
```

### 4. Run the backend
```bash
dotnet run
```

### 5. Run the frontend
```bash
cd employee-scheduler/employeescheduler.client
npm install
npm run dev
```

After running the dev server, check your terminal for the local URL — it will look something like:
```bash
➜  Local:  https://localhost:5173/
```
Open that link in your browser to view the app.

---

## Demo

To see the application in action with preloaded sample data and all features demonstrated, watch the full demo video:

[Watch Demo Video](https://www.linkedin.com/posts/jacob-rieker_most-of-us-have-had-to-check-a-schedule-activity-7358894061090451458-KZYU?utm_source=social_share_send&utm_medium=member_desktop_web&rcm=ACoAAEHhdooB7v3-3FOJrl6x1NXx4y4YAtIxnec)

---

## Author

This application was designed and developed by **Jacob Rieker** as a full-stack project using ASP.NET Core, React, and SQLite.  
It was built to demonstrate end-to-end development, including authentication, role-based access, data tracking, and user-friendly UI/UX.

Feel free to reach out or connect:

- GitHub: [@jacobrieker](https://github.com/jacobrieker)
- Email: jacobrieker12@gmail.com
- Portfolio: [jacobrieker.com](https://jacobrieker.com)

