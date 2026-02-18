using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Finance_api.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Accounts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Username = table.Column<string>(type: "TEXT", nullable: false),
                    Email = table.Column<string>(type: "TEXT", nullable: false),
                    PasswordHash = table.Column<string>(type: "TEXT", nullable: false),
                    IsAdmin = table.Column<bool>(type: "INTEGER", nullable: false),
                    Created = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Accounts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Arrotondamenti",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Nome = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Arrotondamenti", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Assoggettamenti",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Nome = table.Column<string>(type: "TEXT", nullable: false),
                    Percentuale = table.Column<double>(type: "REAL", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Assoggettamenti", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Clienti",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Nome = table.Column<string>(type: "TEXT", nullable: false),
                    CodiceFiscale = table.Column<string>(type: "TEXT", nullable: true),
                    PartitaIVA = table.Column<string>(type: "TEXT", nullable: true),
                    IBAN = table.Column<string>(type: "TEXT", nullable: true),
                    Email = table.Column<string>(type: "TEXT", nullable: true),
                    Telefono = table.Column<string>(type: "TEXT", nullable: true),
                    Indirizzo = table.Column<string>(type: "TEXT", nullable: true),
                    Created = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clienti", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Temi",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Nome = table.Column<string>(type: "TEXT", nullable: false),
                    Colore = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Temi", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TipiPagamento",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Nome = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TipiPagamento", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Valute",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Codice = table.Column<string>(type: "TEXT", nullable: false),
                    Nome = table.Column<string>(type: "TEXT", nullable: true),
                    Simbolo = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Valute", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Fatture",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    NumeroFattura = table.Column<string>(type: "TEXT", nullable: false),
                    TipoFattura = table.Column<string>(type: "TEXT", nullable: false),
                    DataEmissione = table.Column<DateTime>(type: "TEXT", nullable: false),
                    DataScadenza = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ImponibileNetto = table.Column<double>(type: "REAL", nullable: false),
                    TotaleLordo = table.Column<double>(type: "REAL", nullable: false),
                    ClienteId = table.Column<int>(type: "INTEGER", nullable: false),
                    MetodoPagamentoId = table.Column<int>(type: "INTEGER", nullable: false),
                    AssoggettamentoId = table.Column<int>(type: "INTEGER", nullable: false),
                    TemaId = table.Column<int>(type: "INTEGER", nullable: false),
                    ValutaId = table.Column<int>(type: "INTEGER", nullable: false),
                    ArrotondamentoId = table.Column<int>(type: "INTEGER", nullable: true),
                    Created = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Fatture", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Fatture_Arrotondamenti_ArrotondamentoId",
                        column: x => x.ArrotondamentoId,
                        principalTable: "Arrotondamenti",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Fatture_Assoggettamenti_AssoggettamentoId",
                        column: x => x.AssoggettamentoId,
                        principalTable: "Assoggettamenti",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Fatture_Clienti_ClienteId",
                        column: x => x.ClienteId,
                        principalTable: "Clienti",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Fatture_Temi_TemaId",
                        column: x => x.TemaId,
                        principalTable: "Temi",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Fatture_TipiPagamento_MetodoPagamentoId",
                        column: x => x.MetodoPagamentoId,
                        principalTable: "TipiPagamento",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Fatture_Valute_ValutaId",
                        column: x => x.ValutaId,
                        principalTable: "Valute",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Accounts",
                columns: new[] { "Id", "Created", "Email", "IsAdmin", "PasswordHash", "Username" },
                values: new object[] { 1, new DateTime(2026, 2, 16, 22, 17, 4, 949, DateTimeKind.Utc).AddTicks(5517), "admin@gmail.com", true, "$2a$11$/9MvscxNTYR1KAZlLqQML.NwWzEV.68hGOovzvCGlAl1xwU87Ceey", "Admin" });

            migrationBuilder.InsertData(
                table: "Arrotondamenti",
                columns: new[] { "Id", "Nome" },
                values: new object[,]
                {
                    { 1, "Nessuno" },
                    { 2, "Arrotonda per eccesso" },
                    { 3, "Arrotonda per difetto" }
                });

            migrationBuilder.InsertData(
                table: "Assoggettamenti",
                columns: new[] { "Id", "Nome", "Percentuale" },
                values: new object[,]
                {
                    { 1, "IVA 22%", 22.0 },
                    { 2, "IVA 10%", 10.0 },
                    { 3, "IVA 4%", 4.0 },
                    { 4, "IVA Esente", 0.0 }
                });

            migrationBuilder.InsertData(
                table: "Clienti",
                columns: new[] { "Id", "CodiceFiscale", "Created", "Email", "IBAN", "Indirizzo", "Nome", "PartitaIVA", "Telefono" },
                values: new object[,]
                {
                    { 1, "ACMCFF80A01H501Z", new DateTime(2026, 2, 16, 22, 17, 4, 949, DateTimeKind.Utc).AddTicks(6476), "info@acmecorp.it", "IT60X0542811101000000123456", "Via Roma 1, 20100 Milano MI", "Acme Corporation", "12345678901", "+39 02 1234567" },
                    { 2, "TCHSTF85B02F205W", new DateTime(2026, 2, 16, 22, 17, 4, 949, DateTimeKind.Utc).AddTicks(6478), "contact@techstart.io", "IT28W8000000292100645211151", "Piazza Venezia 10, 00100 Roma RM", "TechStart SRL", "98765432109", "+39 06 9876543" },
                    { 3, "INNDIG90C03L219K", new DateTime(2026, 2, 16, 22, 17, 4, 949, DateTimeKind.Utc).AddTicks(6480), "admin@innovazionedigitale.com", "IT40S0760101600000000123456", "Corso Francia 100, 10143 Torino TO", "Innovazione Digitale SpA", "11223344556", "+39 011 5551234" },
                    { 4, null, new DateTime(2026, 2, 16, 22, 17, 4, 949, DateTimeKind.Utc).AddTicks(6482), "billing@cloudsystems.it", null, "Via Indipendenza 8, 40121 Bologna BO", "CloudSystems Italia", "55667788990", "+39 051 8887766" },
                    { 5, "DGTMKT95D04A944H", new DateTime(2026, 2, 16, 22, 17, 4, 949, DateTimeKind.Utc).AddTicks(6483), "info@digitalmarketingpro.it", "IT10N0306909606100000000123", "Via Toledo 256, 80134 Napoli NA", "Digital Marketing Pro", "99887766554", "+39 081 6655443" }
                });

            migrationBuilder.InsertData(
                table: "Temi",
                columns: new[] { "Id", "Colore", "Nome" },
                values: new object[,]
                {
                    { 1, "#5865f2", "Servizi IT" },
                    { 2, "#43b581", "Consulenza" },
                    { 3, "#faa61a", "Sviluppo Software" },
                    { 4, "#f04747", "Design" }
                });

            migrationBuilder.InsertData(
                table: "TipiPagamento",
                columns: new[] { "Id", "Nome" },
                values: new object[,]
                {
                    { 1, "Bonifico Bancario" },
                    { 2, "Carta di Credito" },
                    { 3, "PayPal" },
                    { 4, "Contanti" }
                });

            migrationBuilder.InsertData(
                table: "Valute",
                columns: new[] { "Id", "Codice", "Nome", "Simbolo" },
                values: new object[,]
                {
                    { 1, "EUR", "Euro", "€" },
                    { 2, "USD", "US Dollar", "$" },
                    { 3, "GBP", "British Pound", "£" }
                });

            migrationBuilder.InsertData(
                table: "Fatture",
                columns: new[] { "Id", "ArrotondamentoId", "AssoggettamentoId", "ClienteId", "Created", "DataEmissione", "DataScadenza", "ImponibileNetto", "MetodoPagamentoId", "NumeroFattura", "TemaId", "TipoFattura", "TotaleLordo", "ValutaId" },
                values: new object[,]
                {
                    { 1, 1, 1, 1, new DateTime(2026, 1, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 1, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 2, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), 5000.0, 1, "2026/001", 1, "Entrata", 6100.0, 1 },
                    { 2, 1, 1, 2, new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 2, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), 3500.0, 1, "2026/002", 3, "Entrata", 4270.0, 1 },
                    { 3, null, 1, 4, new DateTime(2026, 1, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 1, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 2, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), 1500.0, 1, "F2026/001", 1, "Uscita", 1830.0, 1 },
                    { 4, 1, 1, 3, new DateTime(2026, 2, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 2, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 3, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), 7500.0, 1, "2026/003", 2, "Entrata", 9150.0, 1 },
                    { 5, 1, 1, 5, new DateTime(2026, 2, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 2, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 3, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), 4200.0, 2, "2026/004", 4, "Entrata", 5124.0, 1 },
                    { 6, null, 1, 4, new DateTime(2026, 2, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 2, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 3, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), 2000.0, 1, "F2026/002", 1, "Uscita", 2440.0, 1 },
                    { 7, null, 1, 1, new DateTime(2026, 2, 14, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 2, 14, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 3, 14, 0, 0, 0, 0, DateTimeKind.Unspecified), 1200.0, 3, "F2026/003", 2, "Uscita", 1464.0, 1 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Fatture_ArrotondamentoId",
                table: "Fatture",
                column: "ArrotondamentoId");

            migrationBuilder.CreateIndex(
                name: "IX_Fatture_AssoggettamentoId",
                table: "Fatture",
                column: "AssoggettamentoId");

            migrationBuilder.CreateIndex(
                name: "IX_Fatture_ClienteId",
                table: "Fatture",
                column: "ClienteId");

            migrationBuilder.CreateIndex(
                name: "IX_Fatture_MetodoPagamentoId",
                table: "Fatture",
                column: "MetodoPagamentoId");

            migrationBuilder.CreateIndex(
                name: "IX_Fatture_TemaId",
                table: "Fatture",
                column: "TemaId");

            migrationBuilder.CreateIndex(
                name: "IX_Fatture_ValutaId",
                table: "Fatture",
                column: "ValutaId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Accounts");

            migrationBuilder.DropTable(
                name: "Fatture");

            migrationBuilder.DropTable(
                name: "Arrotondamenti");

            migrationBuilder.DropTable(
                name: "Assoggettamenti");

            migrationBuilder.DropTable(
                name: "Clienti");

            migrationBuilder.DropTable(
                name: "Temi");

            migrationBuilder.DropTable(
                name: "TipiPagamento");

            migrationBuilder.DropTable(
                name: "Valute");
        }
    }
}
