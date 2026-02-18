# Podere Sopra la Ripa - Documentazione del Progetto

## Panoramica del Progetto

Questo progetto Angular implementa completamente tutte le specifiche richieste nell'Analisi Funzionale (AFU) per il sito web di Podere Sopra la Ripa.

## Struttura del Progetto

### Pagine Implementate

#### 1. **Home Page** (`src/app/pages/home/`)
- **Effetto Parallax**: Implementato con tracciamento del movimento del mouse
- **Sezioni**:
  - Hero section con effetto parallax e presentazione dell'azienda
  - Sezione esperienze con 4 card:
    - Degustazione di Vini (reindirizza a Contattaci con checkbox "Degustazione" pre-selezionata)
    - Vieni in Negozio (reindirizza a Shop)
    - Notte con noi (reindirizza a Contattaci con "Degustazione" e "Notte con noi" pre-selezionati)
    - Visita alla Tomba Etrusca (reindirizza a Contattaci con "Visita alla tomba" pre-selezionata)
  - Showcase prodotti con link diretti ad Amazon
  - Sezione Wine Club

#### 2. **Shop Page** (`src/app/pages/shop/`)
- Catalogo prodotti completo
- Filtro per categorie
- Link diretti ad Amazon per ogni prodotto
- Banner Wine Club con informazioni sui codici sconto

#### 3. **Degustazione Page** (`src/app/pages/degustazione/`)
- **Selezione tipo prenotazione**: 4 opzioni con checkbox multipla
  - Degustazione Standard (€25)
  - Degustazione Premium (€45)
  - Visita alla Cantina (€20)
  - Esperienza Completa (€150)
- **Calendario dinamico**: Si popola in base alle selezioni (esclude domeniche)
- **Form dati cliente**: Nome, Cognome, Email, Telefono, Numero persone, Note
- **Messaggio conferma**: Dopo l'invio viene mostrato il messaggio "Ti contatteremo presto per confermarti la prenotazione"
- I dati vengono inviati al sistema gestionale

#### 4. **Contattaci Page** (`src/app/pages/contattaci/`)
- Form di contatto completo
- **Pre-compilazione automatica** tramite query parameters:
  - `?degustazione=true` - Pre-seleziona checkbox Degustazione
  - `?notteConNoi=true` - Pre-seleziona checkbox Notte con noi
  - `?visitaTomba=true` - Pre-seleziona checkbox Visita alla tomba
- Messaggio di conferma dopo l'invio

#### 5. **Admin/Gestionale Page** (`src/app/pages/admin/`)
Sistema completo di gestione prenotazioni con 3 sezioni:

##### **Nuove Prenotazioni**
- Lista di tutte le prenotazioni in attesa
- Visualizzazione completa dei dati
- Azioni disponibili:
  - ✓ Conferma: Sposta la prenotazione in "Confermate"
  - ✗ Rifiuta: Sposta la prenotazione in "Archiviate"
- Dati modificabili

##### **Prenotazioni Confermate**
- Lista delle prenotazioni confermate
- Sola lettura (non modificabile)
- Visualizzazione completa dei dettagli

##### **Prenotazioni Archiviate**
- Lista delle prenotazioni archiviate o rifiutate
- Sola lettura (non modificabile)
- Visualizzazione completa dei dettagli

### Componenti Condivisi

#### **Navbar** (`src/app/shared/navbar/`)
- Logo cliccabile per tornare alla Home
- Link di navigazione:
  - Home
  - Shop
  - Degustazione
  - Contattaci
- Responsive con layout mobile

#### **Footer** (`src/app/shared/footer/`)
- Informazioni aziendali
- Contatti (Email, Telefono, WhatsApp)
- Orari di apertura
- Informazioni Wine Club
- Link a Privacy Policy, Cookie Policy, GDPR

## Servizi Implementati

### **BookingService** (`src/app/services/booking.service.ts`)
Gestisce tutte le prenotazioni con:
- Storage in-memory (da sostituire con backend reale)
- Signal-based state management
- Metodi per:
  - `addBooking()`: Aggiunge nuova prenotazione
  - `confirmBooking()`: Conferma una prenotazione
  - `rejectBooking()`: Rifiuta una prenotazione
  - `archiveBooking()`: Archivia una prenotazione
- Computed signals per filtrare le prenotazioni per stato

### **ProductService** (`src/app/services/product.service.ts`)
Gestisce il catalogo prodotti:
- Lista prodotti con dettagli (nome, descrizione, prezzo, URL Amazon, categoria)
- Signal-based per reattività

## Modelli Dati

### **Booking** (`src/app/models/models.ts`)
```typescript
interface Booking {
  id: string;
  nome: string;
  cognome: string;
  email: string;
  telefono: string;
  tipoPrenotazione: string[];
  data: Date;
  numeroPersone: number;
  messaggio?: string;
  status: BookingStatus;
  createdAt: Date;
}
```

### **Product** (`src/app/models/models.ts`)
```typescript
interface Product {
  id: string;
  nome: string;
  descrizione: string;
  prezzo: number;
  amazonUrl: string;
  immagine: string;
  categoria: string;
}
```

## Routing

Tutte le rotte sono configurate in `src/app/app.routes.ts`:
- `/` - Home
- `/shop` - Shop
- `/degustazione` - Prenotazione Degustazione
- `/contattaci` - Form di contatto
- `/admin` - Gestionale Prenotazioni

## Conformità con i Requisiti

### ✅ Requisiti Implementati

1. **Vendita Online via Amazon**: Link diretti ai prodotti
2. **Codici Sconto Wine Club**: Sezione dedicata e banner informativi
3. **Sistema Prenotazioni**: Completo con form, calendario e gestionale
4. **Canale WhatsApp**: Collegamento nel footer
5. **Wine Club**: Sezioni informative e CTA per iscrizione
6. **Immagini Grafiche**: Placeholder pronti per sostituzione
7. **Home con Parallax**: Implementato con tracking del mouse
8. **Toolbar con Logo e Link**: Navbar responsive completo
9. **Link Azione Multipli**: Tutti implementati con pre-compilazione form
10. **Gestionale Prenotazioni**: Sistema completo con 3 tab
11. **GDPR**: Nota in footer, gestione manuale dati
12. **Lingua Italiana**: Tutto in italiano

### 📋 Flusso di Processo

```
Home → Shop (acquisto Amazon)
     → Degustazione → Form Prenotazione → Gestionale
     → Contattaci (con pre-compilazione)
     
Admin → Nuove Prenotazioni → [Conferma/Rifiuta]
      → Prenotazioni Confermate (solo lettura)
      → Prenotazioni Archiviate (solo lettura)
```

## Tecnologie Utilizzate

- **Angular 21**: Framework principale (ultima versione con standalone components)
- **TypeScript 5.9**: Linguaggio
- **RxJS 7.8**: Gestione asincrona
- **Angular Signals**: State management reattivo
- **Reactive Forms**: Gestione form
- **CSS3**: Styling custom
- **Tailwind CSS 4**: Utility CSS (configurato)

## Stile e Design

### Palette Colori
- **Primario**: `#8b2e2e` (Rosso vino)
- **Secondario**: `#d4af37` (Oro)
- **Scuro**: `#4a1a1a` (Marrone scuro)
- **Chiaro**: `#f9f6f1` (Beige)
- **Sfondo**: `#fafafa` (Grigio chiaro)

### Tipografia
- **Titoli**: Playfair Display (serif elegante)
- **Testo**: Inter (sans-serif moderna)

## Come Avviare il Progetto

### Prerequisiti
- Node.js 18+ installato
- npm 11.6.2+

### Comandi

```bash
# Installare le dipendenze
npm install

# Avviare il server di sviluppo
npm start

# L'applicazione sarà disponibile su http://localhost:4200
```

### Build per Produzione

```bash
npm run build
```

## Prossimi Passi / Todo

### 1. Backend Integration
- Sostituire `BookingService` in-memory storage con API REST
- Implementare database per prenotazioni e prodotti
- Configurare autenticazione per l'admin

### 2. Immagini e Media
- Sostituire placeholder immagini in:
  - `/images/vineyard-hero.jpg` (hero home)
  - `/images/degustazione.jpg`
  - `/images/negozio.jpg`
  - `/images/notte.jpg`
  - `/images/tomba.jpg`
  - `/products/*.jpg` (foto prodotti)
  - `/logo.svg` (logo aziendale)

### 3. Amazon Integration
- Aggiornare URL Amazon con link prodotti reali
- Implementare tracking codici sconto Wine Club

### 4. Email & Notifiche
- Configurare invio email conferma prenotazioni
- Sistema notifiche per admin su nuove prenotazioni
- Email welcome per Wine Club

### 5. WhatsApp Integration
- Configurare link WhatsApp Business
- Implementare chat widget (opzionale)

### 6. Analytics & SEO
- Aggiungere Google Analytics
- Ottimizzare meta tags
- Implementare sitemap.xml
- Ottimizzare performance (lazy loading immagini)

### 7. Testing
- Unit test per servizi
- E2E test per flussi principali

### 8. Legal
- Aggiungere pagine Privacy Policy, Cookie Policy
- Form consenso GDPR completo
- Cookie banner

## Note Importanti

### GDPR
Come specificato nei requisiti:
- I dati dei clienti NON vengono cancellati automaticamente
- La cancellazione è solo manuale da parte dell'amministratore
- Necessario implementare funzionalità di cancellazione dati nel gestionale

### Internazionalizzazione
Come specificato nei requisiti:
- NON sono previste traduzioni
- L'applicazione è solo in italiano

### Esclusioni
Tutto ciò che non è stato indicato nell'Analisi Funzionale NON è incluso nel progetto.

## Struttura File

```
src/
├── app/
│   ├── models/
│   │   └── models.ts                 # Interfacce TypeScript
│   ├── pages/
│   │   ├── home/                     # Home page con parallax
│   │   ├── shop/                     # Catalogo prodotti
│   │   ├── degustazione/             # Sistema prenotazioni
│   │   ├── contattaci/               # Form contatto
│   │   └── admin/                    # Gestionale prenotazioni
│   ├── services/
│   │   ├── booking.service.ts        # Gestione prenotazioni
│   │   └── product.service.ts        # Gestione prodotti
│   ├── shared/
│   │   ├── navbar/                   # Barra di navigazione
│   │   └── footer/                   # Footer
│   ├── app.ts                        # App component
│   ├── app.html                      # App template
│   ├── app.css                       # App styles
│   ├── app.routes.ts                 # Routing configuration
│   └── app.config.ts                 # App configuration
├── styles.css                        # Stili globali
└── index.html                        # HTML principale
```

## Supporto e Contatti

Per qualsiasi domanda o supporto tecnico sul progetto, contattare il team di sviluppo.

---

**Data creazione**: 8 Febbraio 2026  
**Versione Angular**: 21.1.0  
**Stato**: ✅ Completato secondo specifiche AFU
