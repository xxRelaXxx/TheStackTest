# CrossDocker - Gestionale Finanziario

Sistema di gestione finanziaria per Azienda A con backend C# e **due frontend**: HTML/CSS/JavaScript tradizionale e **Laravel Filament**.

## 📋 Caratteristiche

- ✅ Dashboard con metriche economiche (Profitto, Entrate, Grafico mensile)
- ✅ Gestione completa fatture (Entrate/Uscite)
- ✅ Inserimento e modifica fatture
- ✅ Filtri per Cliente e Argomento
- ✅ Gestione account utenti
- ✅ Autenticazione Email + Password
- ✅ Tema Chiaro/Scuro
- ✅ Responsive (Web e Mobile)
- ✅ Integrazione Invoicetronic SDK
- ✅ **NUOVO**: Frontend Laravel Filament con dark mode

## 🛠️ Stack Tecnologico

### Backend
- C# / .NET 8
- Entity Framework Core
- SQLite Database
- JWT Authentication
- BCrypt Password Hashing

### Frontend (Scegli uno)

#### 1. Frontend Tradizionale
- HTML5 / CSS3 / JavaScript (Vanilla)
- Design moderno dark/light theme
- Responsive layout

#### 2. **Frontend Filament (NUOVO)**
- Laravel 12
- Filament 5.2
- Livewire 4
- Tailwind CSS
- Dark Mode by default
- Advanced forms and tables

## 📁 Struttura Progetto

```
SDI_FINANCE/
├── Finance-api/          # Backend C# API
│   ├── Controllers/      # API Controllers
│   ├── Models/          # Database Models
│   ├── Data/            # DbContext
│   ├── DTOs/            # Data Transfer Objects
│   ├── Services/        # Invoicetronic Integration
│   └── finance.db       # SQLite Database
│
├── frontend/            # Frontend Tradizionale
│   ├── index.html       # Main HTML
│   ├── css/
│   │   └── styles.css   # Styling
│   └── js/
│       ├── api.js       # API Communication
│       ├── app.js       # Main App Logic
│       ├── dashboard.js # Dashboard Logic
│
└── filaments-app/       # Frontend Laravel Filament (NUOVO)
    ├── app/
    │   ├── Filament/
    │   │   ├── Pages/   # Dashboard, Fatture, Profile
    │   │   └── Widgets/ # Stats, Charts
    │   └── Services/
    │       └── FinanceApiService.php
    ├── resources/
    │   └── views/
    │       └── filament/
    └── config/
        ├── fatture.js   # Invoice Management
        └── chart.js     # Chart Rendering
```

## 🚀 Installazione e Avvio

### Backend (API C#)

1. **Navigare alla cartella backend:**
   ```powershell
   cd Finance-api
   ```

2. **Restaurare pacchetti:**
   ```powershell
   dotnet restore
   ```

3. **Avviare l'API:**
   ```powershell
   dotnet run
   ```

   L'API sarà disponibile su:
   - HTTPS: `https://localhost:7241`
   - HTTP: `http://localhost:5058`

### Frontend Tradizionale

1. **Aprire il frontend:**
   
   Aprire il file `frontend/index.html` con un web server locale.

   **Opzione 1 - Visual Studio Code:**
   ```
   Installare l'estensione "Live Server"
   Click destro su index.html → "Open with Live Server"
   ```

   **Opzione 2 - Python:**
   ```powershell
   cd frontend
   python -m http.server 8000
   ```
   Aprire: `http://localhost:8000`

   **Opzione 3 - Node.js:**
   ```powershell
   npx http-server frontend -p 8000
   ```

2. **Configurare l'URL API:**
   
   Se necessario, modificare l'URL dell'API in `frontend/js/api.js`:
   ```javascript
   const API_BASE_URL = 'https://localhost:7241/api';
   ```

### **Frontend Filament (NUOVO)**

1. **Avvio rapido con script:**
   ```powershell
   start-filament.bat
   ```

2. **Avvio manuale:**
   ```powershell
   cd filaments-app
   composer install
   npm install
   npm run build
   php artisan serve
   ```

3. **Accesso:**
   - URL: `http://localhost:8000/admin`
   - Login con le credenziali del backend C#

**Per maggiori dettagli, vedere:** [FILAMENT_SETUP.md](FILAMENT_SETUP.md)


## 👤 Credenziali di Accesso

**Account Admin (predefinito):**
- Email: `admin@gmail.com`
- Password: `admin123`

## 📊 Database

Il database SQLite viene creato automaticamente all'avvio dell'applicazione con:

### 8 Tabelle Principali:
1. **Account** - Utenti del sistema
2. **Clienti** - Anagrafica clienti
3. **TipoPagamento** - Metodi di pagamento
4. **Assoggettamenti** - Aliquote IVA
5. **Tema** - Categorie/Argomenti fatture
6. **Valute** - Valute supportate
7. **Arrotondamenti** - Opzioni arrotondamento
8. **Fatture** - Fatture (Entrate/Uscite)

### Dati Iniziali (Seed):
- Account Admin
- Valute: EUR, USD, GBP
- IVA: 22%, 10%, 4%, Esente
- Metodi Pagamento: Bonifico, Contanti, Carta, PayPal
- Temi: Consulenza, Prodotti, Servizi, Altro
- Arrotondamenti: Nessuno, Al centesimo, All'euro

## 📱 Pagine dell'Applicazione

### 1. **Login**
- Autenticazione via Email/Password
- Validazione credenziali

### 2. **Dashboard**
- Card Profitto (con trend %)
- Card Entrate (con trend %)
- Grafico a barre ultimi 12 mesi
- Aggiornamento solo su reload manuale

### 3. **Storico Fatture**
- Tab Entrate/Uscite
- Filtri per Cliente e Argomento
- Tabella con tutte le informazioni
- Cancellazione singola o multipla
- Click riga → modifica fattura

### 4. **Inserisci Fattura**
- Form completo inserimento
- Calcolo automatico Totale Lordo
- Dropdown con CTA "Aggiungi"
- Validazione campi
- Sovrascrittura se numero fattura esiste

### 5. **Account**
- Visualizzazione profilo utente
- Modifica username/email/password
- **Solo Admin:** Gestione account multipli

## 🔌 API Endpoints

### Autenticazione
- `POST /api/auth/login` - Login
- `GET /api/auth/me` - Utente corrente

### Dashboard
- `GET /api/dashboard/stats` - Statistiche dashboard

### Fatture
- `GET /api/fatture?tipo=Entrata&clienteId=1&temaId=2` - Lista fatture
- `GET /api/fatture/{id}` - Dettaglio fattura
- `POST /api/fatture` - Crea/Aggiorna fattura
- `DELETE /api/fatture/{id}` - Elimina fattura
- `DELETE /api/fatture` - Elimina multiple (body: array ids)

### Lookup APIs
- `GET /api/clienti` - Lista clienti
- `POST /api/clienti` - Aggiungi cliente
- `GET /api/tipipagamento` - Metodi pagamento
- `GET /api/assoggettamenti` - Aliquote IVA
- `GET /api/temi` - Categorie
- `GET /api/valute` - Valute
- `GET /api/arrotondamenti` - Arrotondamenti

### Account
- `GET /api/account` - Lista account (Admin)
- `POST /api/account` - Crea account (Admin)
- `PUT /api/account/{id}` - Modifica account
- `DELETE /api/account/{id}` - Elimina account (Admin)

### Invoicetronic
- `GET /api/invoicetronic/config` - Configurazione SDK
- `GET /api/invoicetronic/invoices` - Fatture da API
- `POST /api/invoicetronic/send` - Invia fattura

## 🔐 Sicurezza

- Password hashate con BCrypt
- JWT Token per autenticazione
- CORS configurato per frontend
- Database con dati criptati
- Validazione input su tutti i form

## 🎨 Tema e UI

- **Tema Scuro** (default): Design moderno con sfondo scuro
- **Tema Chiaro**: Toggle tramite icona Sole/Luna
- **Icone**: Emoji native per semplicità
- **Colori**:
  - Primary: #5865f2 (Blu)
  - Success: #43b581 (Verde)
  - Danger: #f04747 (Rosso)
  - Warning: #faa61a (Arancione)

## 📱 Responsive Design

- Desktop: Layout completo con sidebar
- Tablet: Sidebar collassabile
- Mobile: Menu hamburger, layout ottimizzato

## 🔄 Calcolo Automatico

**Totale Lordo Fattura:**
```
Lordo = Imponibile Netto × (1 + Percentuale IVA / 100)
```

**Profitto:**
```
Profitto = Σ(Entrate Nette) - Σ(Uscite Lorde)
```

**Trend Percentuale:**
```
Trend = ((Valore Attuale - Valore Precedente) / Valore Precedente) × 100
```

## ⚠️ Gestione Errori

Codici errore implementati:
- **400** - Dati errati o non validi
- **404** - Errore server interno
- **500** - Errore sconosciuto

## 📝 Note Implementative

### GDPR
- Dati conservati fino a cancellazione manuale
- Nessuna cancellazione automatica

### Internazionalizzazione
- Solo lingua italiana

### Esclusioni
- Funzionalità non descritte nel documento NON implementate
- No estensioni non richieste
- Implementazione strettamente aderente alle specifiche

## 🧪 Testing

### Test Login:
1. Aprire l'applicazione
2. Inserire: `admin@gmail.com` / `admin123`
3. Click "Entra"

### Test Fattura:
1. Navigare a "Inserisci Fattura"
2. Compilare tutti i campi obbligatori
3. Verificare calcolo automatico Totale Lordo
4. Click "Aggiungi Fattura"
5. Verificare in "Fatture" → Tab appropriato

### Test Dashboard:
1. Inserire alcune fatture di test
2. Ricaricare la pagina
3. Verificare aggiornamento statistiche
4. Controllare grafico ultimi 12 mesi

## 🔧 Troubleshooting

### API non raggiungibile:
- Verificare che `dotnet run` sia attivo
- Controllare firewall/antivirus
- Verificare porta 7241 libera

### Frontend non si connette:
- Aprire Developer Tools (F12)
- Verificare URL API in `js/api.js`
- Controllare CORS nella console

### Database vuoto:
- Eliminare `finance.db`
- Riavviare API per rigenerare

## 📞 Supporto

Per problemi o domande:
1. Verificare log console (F12)
2. Controllare file di log backend
3. Verificare connessione database

## 📄 Licenza

Proprietà di Azienda A - Uso interno

---

**Versione:** 1.0.0  
**Data Rilascio:** Febbraio 2026  
**Ambiente:** Sviluppo
