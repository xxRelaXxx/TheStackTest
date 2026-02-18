# Filament Frontend for SDI_FINANCE

This is a Laravel Filament frontend application that connects to the C# Finance API backend.

## Setup Instructions

1. **Install dependencies:**
   ```bash
   cd filaments-app
   composer install
   npm install
   ```

2. **Configure environment:**
   - The `.env` file is already configured
   - Backend API URL: `http://localhost:5058`

3. **Run migrations (if needed):**
   ```bash
   php artisan migrate
   ```

4. **Build assets:**
   ```bash
   npm run build
   ```

5. **Start the development server:**
   ```bash
   php artisan serve
   ```

6. **Access the application:**
   - Frontend: `http://localhost:8000/admin`
   - Login with credentials from the C# backend

## Features

- **Dashboard**: View profit, income, and monthly sales chart
- **Fatture (Invoices)**: List all invoices with filters for type, client, and theme
- **Aggiungi Fattura**: Add new invoices with automatic total calculation
- **Profile**: View and edit user profile, admins can add new accounts
- **Dark Mode**: Dark theme enabled by default

## Pages

- `/admin` - Dashboard
- `/admin/fatture` - Invoices list
- `/admin/aggiungi-fattura` - Add new invoice
- `/admin/profile` - User profile

## API Connection

The application connects to the C# backend API at `http://localhost:5058` using the `FinanceApiService` class.

All API calls are authenticated using JWT tokens stored in the session.

## Development

To start development:

```bash
# Terminal 1: Start Laravel server
php artisan serve

# Terminal 2: Watch and compile assets
npm run dev
```

## Backend

Make sure the C# Finance API is running on `http://localhost:5058` before using this frontend.
