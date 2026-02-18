# CrossDocker

Financial management system with a C# REST API, an Angular client, and a Laravel Filament admin panel.

---

## Stack

| Layer | Technology |
|---|---|
| API | C# / .NET 8, Entity Framework Core, SQLite, JWT |
| Angular client | Angular 19, TypeScript |
| Admin panel | Laravel 12, Filament 5, Livewire 4, Tailwind CSS |

---

## Features

- Invoice management (income / expenses) with full CRUD
- Dashboard with revenue, profit, and monthly chart
- Client, category, and payment method lookup tables
- Role-based access  admin users can manage accounts
- Invoicetronic SDK integration for electronic invoicing
- JWT authentication with BCrypt password hashing
- Dark mode support

---

## Project Structure

```
SDI_FINANCE/
 Finance-api/       # .NET 8 REST API
 angular-app/       # Angular 19 frontend
 filaments-app/     # Laravel Filament admin panel
```

---

## Getting Started

### 1. API

```powershell
cd Finance-api
dotnet run
# Runs on https://localhost:7241
```

### 2. Angular client

```powershell
cd angular-app
npm install
ng serve
# Runs on http://localhost:4200
```

### 3. Filament admin panel

```powershell
cd filaments-app
composer install
npm install && npm run build
php artisan serve
# Runs on http://localhost:8000/admin
```

Or use the provided scripts: `start-backend.bat` / `start-filament.bat`.

---

## Default credentials

```
Email:    admin@gmail.com
Password: admin123
```

---

## API Overview

| Group | Endpoints |
|---|---|
| Auth | `POST /api/auth/login`, `GET /api/auth/me` |
| Dashboard | `GET /api/dashboard/stats` |
| Invoices | `GET /api/fatture`, `POST /api/fatture`, `DELETE /api/fatture/{id}` |
| Accounts | `GET /api/account`, `POST /api/account`, `PUT /api/account/{id}` |
| Lookups | `/api/clienti`, `/api/temi`, `/api/valute`, `/api/assoggettamenti` |
| Invoicetronic | `GET /api/invoicetronic/config`, `POST /api/invoicetronic/send` |

---

**Version:** 1.0.0  February 2026
