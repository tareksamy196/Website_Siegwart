# Siegwart Website

[![.NET](https://img.shields.io/badge/.NET-8.0-512BD4?logo=dotnet)](https://dotnet.microsoft.com/)
[![ASP.NET Core](https://img.shields.io/badge/ASP.NET%20Core-MVC-512BD4?logo=dotnet)](https://docs.microsoft.com/aspnet/core/)
[![SQL Server](https://img.shields.io/badge/SQL%20Server-Database-CC2927?logo=microsoft-sql-server)](https://www.microsoft.com/sql-server)
[![License](https://img.shields.io/badge/License-Proprietary-gray)](LICENSE)

> Official corporate website for **Siegwart - Egyptian Company for Pipes & Cement Products**, a leading manufacturer of railway and infrastructure solutions since 1932.

## 🏗️ Architecture

This solution follows a clean **3-tier architecture** for better separation of concerns, maintainability, and testability:

```
┌─────────────────────────────────────────────────────────────┐
│                    Presentation Layer (PL)                   │
│  ┌─────────────────────────────────────────────────────┐    │
│  │  ASP.NET Core MVC Controllers, Views, wwwroot       │    │
│  │  Authentication, Session, Localization (EN/AR)      │    │
│  └─────────────────────────────────────────────────────┘    │
└─────────────────────────────────────────────────────────────┘
                              │
                              ▼
┌─────────────────────────────────────────────────────────────┐
│                   Business Logic Layer (BLL)                 │
│  ┌─────────────────────────────────────────────────────┐    │
│  │  Services, DTOs, AutoMapper Profiles, Validation    │    │
│  │  Email Service (MailKit), Business Rules            │    │
│  └─────────────────────────────────────────────────────┘    │
└─────────────────────────────────────────────────────────────┘
                              │
                              ▼
┌─────────────────────────────────────────────────────────────┐
│                    Data Access Layer (DAL)                   │
│  ┌─────────────────────────────────────────────────────┐    │
│  │  Entity Framework Core, DbContext, Repositories     │    │
│  │  Models, Migrations, Configurations                 │    │
│  └─────────────────────────────────────────────────────┘    │
└─────────────────────────────────────────────────────────────┘
```

## ✨ Features

### User-Facing Website
- 🏠 **Homepage** - Company overview with latest news slider
- 📦 **Products Catalog** - Categorized product listing with details
- 📰 **News & Media** - Company news and video gallery
- 👥 **Team Directory** - Meet our team members
- 📧 **Contact Form** - Secure contact with honeypot protection
- 🌐 **Bilingual Support** - Full Arabic (RTL) and English support

### Admin Dashboard
- 🔐 **Secure Authentication** - ASP.NET Core Identity with role-based access
- 📊 **Dashboard** - Overview and statistics
- 🛠️ **Content Management** - Full CRUD for Products, News, Team, Videos
- 📂 **Category Management** - Organize products by categories
- 🖼️ **Image Upload** - Secure file upload with validation
- 📝 **Audit Trail** - Track all admin actions

### Technical Features
- ⚡ **Performance Optimized** - Brotli/Gzip compression, response caching
- 🔒 **Security Hardened** - HTTPS, CSP headers, XSS protection
- 📱 **Responsive Design** - Mobile-first Bootstrap 5
- 🔍 **SEO Ready** - Meta tags, OpenGraph, structured data, sitemap
- 📊 **Logging** - Serilog with file and console sinks
- 🚦 **Rate Limiting** - Protect against abuse

## 🛠️ Technology Stack

| Layer | Technology |
|-------|------------|
| **Frontend** | Bootstrap 5, Font Awesome, Vanilla JS |
| **Backend** | ASP.NET Core 8.0 MVC |
| **Database** | SQL Server + Entity Framework Core 8.0 |
| **Authentication** | ASP.NET Core Identity |
| **Email** | MailKit SMTP |
| **Logging** | Serilog |
| **Mapping** | AutoMapper |

## 📋 Prerequisites

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [SQL Server](https://www.microsoft.com/sql-server) (LocalDB, Express, or full)
- [Visual Studio 2022](https://visualstudio.microsoft.com/) or [VS Code](https://code.visualstudio.com/)

## 🚀 Getting Started

### 1. Clone the repository

```bash
git clone https://github.com/tareksamy196/Website_Siegwart.git
cd Website_Siegwart
```

### 2. Configure the database

Update the connection string in `Website.Siegwart.PL/appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=.;Database=Siegwart Website;Trusted_Connection=true;TrustServerCertificate=true;"
  }
}
```

### 3. Apply database migrations

```bash
cd Website.Siegwart.PL
dotnet ef database update --project ../Website.Siegwart.DAL
```

### 4. Run the application

```bash
dotnet run
```

The website will be available at `https://localhost:5001` or `http://localhost:5000`.

### 5. Default Admin Account

On first run, the system seeds a default Super Admin account. Check the `SeedData.cs` for credentials and update as needed.

## 📁 Project Structure

```
Website.Siegwart/
├── Website.Siegwart.PL/          # Presentation Layer
│   ├── Controllers/              # MVC Controllers
│   ├── Views/                    # Razor Views
│   ├── Helper/                   # View helpers & utilities
│   ├── wwwroot/                  # Static files (CSS, JS, images)
│   ├── Program.cs                # Application entry point
│   └── appsettings.json          # Configuration
│
├── Website.Siegwart.BLL/         # Business Logic Layer
│   ├── Services/                 # Business services
│   │   ├── Interfaces/           # Service contracts
│   │   └── Classes/              # Service implementations
│   ├── Dtos/                     # Data Transfer Objects
│   │   ├── Admin/                # Admin DTOs
│   │   └── User/                 # User-facing DTOs
│   └── Profiles/                 # AutoMapper profiles
│
├── Website.Siegwart.DAL/         # Data Access Layer
│   ├── Data/
│   │   ├── Contexts/             # DbContext
│   │   └── Configurations/       # Entity configurations
│   ├── Models/                   # Entity models
│   ├── Repositories/             # Repository pattern
│   │   ├── Interfaces/           # Repository contracts
│   │   └── Classes/              # Repository implementations
│   └── Migrations/               # EF Core migrations
│
└── Website.Siegwart.sln          # Solution file
```

## ⚙️ Configuration

### SMTP Settings (for email)

```json
{
  "Smtp": {
    "Host": "smtp.example.com",
    "Port": 465,
    "EnableSsl": true,
    "FromName": "Siegwart",
    "FromEmail": "noreply@example.com",
    "UserName": "smtp-user",
    "Password": "smtp-password"
  }
}
```

### File Upload Settings

```json
{
  "FileUpload": {
    "MaxFileSizeMB": 5,
    "AllowedExtensions": [".jpg", ".jpeg", ".png", ".webp"]
  }
}
```

## 🔧 Development

### Build the solution

```bash
dotnet build
```

### Run tests (if available)

```bash
dotnet test
```

### Add a new migration

```bash
cd Website.Siegwart.PL
dotnet ef migrations add MigrationName --project ../Website.Siegwart.DAL
```

## 📄 License

This project is proprietary software owned by Siegwart - Egyptian Company for Pipes & Cement Products. All rights reserved.

## 📞 Contact

- **Website**: [siegwarteg.com](https://www.siegwarteg.com)
- **Email**: info@siegwarteg.com
- **Location**: EL Masara, Helwan, Cairo, Egypt

---

<p align="center">
  <strong>Siegwart</strong> - Engineering Excellence Since 1932
</p>
