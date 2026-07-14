# Real-Time Complaint Management System

A robust, production-ready Real-Time Complaint Management System built using ASP.NET Core MVC, SignalR, and SQL Server. This project is designed as a plug-and-play module, allowing developers to easily integrate a complete complaint-handling infrastructure (including User and Admin portals) into their existing web applications.

## Features

- **Real-Time Communication:** Powered by ASP.NET Core SignalR Hubs for instantaneous message delivery and status updates between users and administrators without page refreshes.
- **Dual-Portal Architecture:** - **User Portal:** Allows anonymous or registered public users to submit complaints, track existing ticket statuses, and chat with support.
  - **Admin Panel (Areas):** Comprehensive dashboard for support agents and administrators to manage, assign, and resolve incoming complaints in real time.
- **Database Architecture:** Structured with Entity Framework Core Migrations, featuring relational models for Users, Complaints, and Chat Messages.
- **Extensible & Modular:** Decoupled architecture utilizing the Repository Pattern for easy modification and integration.

---

## Project Structure

The repository follows the standard ASP.NET Core architectural pattern with an isolated Admin Area:

```text
📁 WebApplication1/
 ┣ 📁 Areas/
 ┃ ┗ 📁 Admin/                  # Isolated Admin Panel (Controllers, Models, Views)
 ┣ 📁 Controllers/              # Core application controllers (Home, Account, Complaints)
 ┣ 📁 Data/                     # DbContext, Repository interfaces, and Service implementations
 ┣ 📁 Hubs/                     # SignalR Hubs handling real-time WebSocket connections
 ┣ 📁 Migrations/               # Entity Framework Core database schema snapshots
 ┣ 📁 Models/                   # Data transfer objects (DTOs) and Domain Models
 ┣ 📁 Properties/               # Environment and launch configuration profiles
 ┣ 📁 Services/                 # Background workers and seeding logic
 ┣ 📁 Views/                    # User-facing MVC Razor Views
 ┗ 📁 wwwroot/                  # Static assets (CSS, custom vanilla JavaScript, and libraries)

```


##Prerequisites

Before running this project, ensure you have the following installed on your local environment:

.NET 8.0 SDK (or higher)

SQL Server (Express or Developer Edition)

Visual Studio 2022 / VS Code

---

##Installation and Setup

Follow these steps to set up and run the project locally:

1. Clone the Repository
git clone [https://github.com/mahsa9731/RealTimeSubmitingComplainSystemUsing-C-.git](https://github.com/mahsa9731/RealTimeSubmitingComplainSystemUsing-C-.git)
cd RealTimeSubmitingComplainSystemUsing-C-

2.Build and Run the Application

dotnet build
dotnet run --project WebApplication1

---


## Integration Guide

To embed this module into your existing ASP.NET Core ecosystem:

Copy the Data, Hubs, Models, and Areas/Admin directories into your target project.

Register the SignalR middleware in your Program.cs:

builder.Services.AddSignalR();
// ...
app.MapHub<ComplaintChatHub>("/complaintChatHub");

Configure your pipeline to support Areas routing for the Admin dashboard.
