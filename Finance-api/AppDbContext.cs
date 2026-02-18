using Microsoft.EntityFrameworkCore;

namespace Finance_api;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Account> Accounts { get; set; }
    public DbSet<Cliente> Clienti { get; set; }
    public DbSet<Fattura> Fatture { get; set; }
    public DbSet<TipoPagamento> TipiPagamento { get; set; }
    public DbSet<Assoggettamento> Assoggettamenti { get; set; }
    public DbSet<Tema> Temi { get; set; }
    public DbSet<Valuta> Valute { get; set; }
    public DbSet<Arrotondamento> Arrotondamenti { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Seed Admin Account
        modelBuilder.Entity<Account>().HasData(
            new Account
            {
                Id = 1,
                Username = "Admin",
                Email = "admin@gmail.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin123"),
                IsAdmin = true,
                Created = DateTime.UtcNow
            }
        );

        // Seed Valute (Currencies)
        modelBuilder.Entity<Valuta>().HasData(
            new Valuta { Id = 1, Codice = "EUR", Nome = "Euro", Simbolo = "€" },
            new Valuta { Id = 2, Codice = "USD", Nome = "US Dollar", Simbolo = "$" },
            new Valuta { Id = 3, Codice = "GBP", Nome = "British Pound", Simbolo = "£" }
        );

        // Seed Assoggettamenti (VAT Rates)
        modelBuilder.Entity<Assoggettamento>().HasData(
            new Assoggettamento { Id = 1, Nome = "IVA 22%", Percentuale = 22 },
            new Assoggettamento { Id = 2, Nome = "IVA 10%", Percentuale = 10 },
            new Assoggettamento { Id = 3, Nome = "IVA 4%", Percentuale = 4 },
            new Assoggettamento { Id = 4, Nome = "IVA Esente", Percentuale = 0 }
        );

        // Seed Tipi Pagamento
        modelBuilder.Entity<TipoPagamento>().HasData(
            new TipoPagamento { Id = 1, Nome = "Bonifico Bancario" },
            new TipoPagamento { Id = 2, Nome = "Carta di Credito" },
            new TipoPagamento { Id = 3, Nome = "PayPal" },
            new TipoPagamento { Id = 4, Nome = "Contanti" }
        );

        // Seed Temi
        modelBuilder.Entity<Tema>().HasData(
            new Tema { Id = 1, Nome = "Servizi IT", Colore = "#5865f2" },
            new Tema { Id = 2, Nome = "Consulenza", Colore = "#43b581" },
            new Tema { Id = 3, Nome = "Sviluppo Software", Colore = "#faa61a" },
            new Tema { Id = 4, Nome = "Design", Colore = "#f04747" }
        );

        // Seed Arrotondamenti
        modelBuilder.Entity<Arrotondamento>().HasData(
            new Arrotondamento { Id = 1, Nome = "Nessuno" },
            new Arrotondamento { Id = 2, Nome = "Arrotonda per eccesso" },
            new Arrotondamento { Id = 3, Nome = "Arrotonda per difetto" }
        );

        // Seed Test Clienti
        modelBuilder.Entity<Cliente>().HasData(
            new Cliente
            {
                Id = 1,
                Nome = "Acme Corporation",
                CodiceFiscale = "ACMCFF80A01H501Z",
                PartitaIVA = "12345678901",
                IBAN = "IT60X0542811101000000123456",
                Email = "info@acmecorp.it",
                Telefono = "+39 02 1234567",
                Indirizzo = "Via Roma 1, 20100 Milano MI",
                Created = DateTime.UtcNow
            },
            new Cliente
            {
                Id = 2,
                Nome = "TechStart SRL",
                CodiceFiscale = "TCHSTF85B02F205W",
                PartitaIVA = "98765432109",
                IBAN = "IT28W8000000292100645211151",
                Email = "contact@techstart.io",
                Telefono = "+39 06 9876543",
                Indirizzo = "Piazza Venezia 10, 00100 Roma RM",
                Created = DateTime.UtcNow
            },
            new Cliente
            {
                Id = 3,
                Nome = "Innovazione Digitale SpA",
                CodiceFiscale = "INNDIG90C03L219K",
                PartitaIVA = "11223344556",
                IBAN = "IT40S0760101600000000123456",
                Email = "admin@innovazionedigitale.com",
                Telefono = "+39 011 5551234",
                Indirizzo = "Corso Francia 100, 10143 Torino TO",
                Created = DateTime.UtcNow
            },
            new Cliente
            {
                Id = 4,
                Nome = "CloudSystems Italia",
                PartitaIVA = "55667788990",
                Email = "billing@cloudsystems.it",
                Telefono = "+39 051 8887766",
                Indirizzo = "Via Indipendenza 8, 40121 Bologna BO",
                Created = DateTime.UtcNow
            },
            new Cliente
            {
                Id = 5,
                Nome = "Digital Marketing Pro",
                CodiceFiscale = "DGTMKT95D04A944H",
                PartitaIVA = "99887766554",
                IBAN = "IT10N0306909606100000000123",
                Email = "info@digitalmarketingpro.it",
                Telefono = "+39 081 6655443",
                Indirizzo = "Via Toledo 256, 80134 Napoli NA",
                Created = DateTime.UtcNow
            }
        );

        // Seed Test Fatture (Invoices)
        modelBuilder.Entity<Fattura>().HasData(
            // January 2026 - Entrate
            new Fattura
            {
                Id = 1,
                NumeroFattura = "2026/001",
                TipoFattura = "Entrata",
                DataEmissione = new DateTime(2026, 1, 15),
                DataScadenza = new DateTime(2026, 2, 15),
                ImponibileNetto = 5000,
                TotaleLordo = 6100,
                ClienteId = 1,
                MetodoPagamentoId = 1,
                AssoggettamentoId = 1,
                TemaId = 1,
                ValutaId = 1,
                ArrotondamentoId = 1,
                Created = new DateTime(2026, 1, 15)
            },
            new Fattura
            {
                Id = 2,
                NumeroFattura = "2026/002",
                TipoFattura = "Entrata",
                DataEmissione = new DateTime(2026, 1, 20),
                DataScadenza = new DateTime(2026, 2, 20),
                ImponibileNetto = 3500,
                TotaleLordo = 4270,
                ClienteId = 2,
                MetodoPagamentoId = 1,
                AssoggettamentoId = 1,
                TemaId = 3,
                ValutaId = 1,
                ArrotondamentoId = 1,
                Created = new DateTime(2026, 1, 20)
            },
            // January - Uscite
            new Fattura
            {
                Id = 3,
                NumeroFattura = "F2026/001",
                TipoFattura = "Uscita",
                DataEmissione = new DateTime(2026, 1, 10),
                DataScadenza = new DateTime(2026, 2, 10),
                ImponibileNetto = 1500,
                TotaleLordo = 1830,
                ClienteId = 4,
                MetodoPagamentoId = 1,
                AssoggettamentoId = 1,
                TemaId = 1,
                ValutaId = 1,
                Created = new DateTime(2026, 1, 10)
            },
            // February 2026 - Entrate
            new Fattura
            {
                Id = 4,
                NumeroFattura = "2026/003",
                TipoFattura = "Entrata",
                DataEmissione = new DateTime(2026, 2, 5),
                DataScadenza = new DateTime(2026, 3, 5),
                ImponibileNetto = 7500,
                TotaleLordo = 9150,
                ClienteId = 3,
                MetodoPagamentoId = 1,
                AssoggettamentoId = 1,
                TemaId = 2,
                ValutaId = 1,
                ArrotondamentoId = 1,
                Created = new DateTime(2026, 2, 5)
            },
            new Fattura
            {
                Id = 5,
                NumeroFattura = "2026/004",
                TipoFattura = "Entrata",
                DataEmissione = new DateTime(2026, 2, 12),
                DataScadenza = new DateTime(2026, 3, 12),
                ImponibileNetto = 4200,
                TotaleLordo = 5124,
                ClienteId = 5,
                MetodoPagamentoId = 2,
                AssoggettamentoId = 1,
                TemaId = 4,
                ValutaId = 1,
                ArrotondamentoId = 1,
                Created = new DateTime(2026, 2, 12)
            },
            // February - Uscite
            new Fattura
            {
                Id = 6,
                NumeroFattura = "F2026/002",
                TipoFattura = "Uscita",
                DataEmissione = new DateTime(2026, 2, 8),
                DataScadenza = new DateTime(2026, 3, 8),
                ImponibileNetto = 2000,
                TotaleLordo = 2440,
                ClienteId = 4,
                MetodoPagamentoId = 1,
                AssoggettamentoId = 1,
                TemaId = 1,
                ValutaId = 1,
                Created = new DateTime(2026, 2, 8)
            },
            new Fattura
            {
                Id = 7,
                NumeroFattura = "F2026/003",
                TipoFattura = "Uscita",
                DataEmissione = new DateTime(2026, 2, 14),
                DataScadenza = new DateTime(2026, 3, 14),
                ImponibileNetto = 1200,
                TotaleLordo = 1464,
                ClienteId = 1,
                MetodoPagamentoId = 3,
                AssoggettamentoId = 1,
                TemaId = 2,
                ValutaId = 1,
                Created = new DateTime(2026, 2, 14)
            }
        );
    }
}
