using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.IdentityModel.JsonWebTokens;
using System.Text;
using System.Security.Claims;
using Finance_api;

var builder = WebApplication.CreateBuilder(args);

// Services
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure JSON to handle circular references
builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    options.SerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
    options.SerializerOptions.WriteIndented = false;
    options.SerializerOptions.MaxDepth = 32;
});

// Database
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection") ?? "Data Source=finance.db"));

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:8000", "http://localhost:3000", "http://127.0.0.1:8000",
                          "http://127.0.0.1:5500", "http://localhost:5500",
                          "http://localhost:4200", "http://127.0.0.1:4200")
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});

// JWT Authentication
var jwtKey = builder.Configuration["Jwt:Key"] ?? "YourSuperSecretKeyForFinanceApiThatIsAtLeast32CharactersLong";
var jwtIssuer = builder.Configuration["Jwt:Issuer"] ?? "FinanceApi";
var jwtAudience = builder.Configuration["Jwt:Audience"] ?? "FinanceApp";

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtIssuer,
            ValidAudience = jwtAudience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
        };
    });

builder.Services.AddAuthorization();

// Invoicetronic Client
builder.Services.AddSingleton(new InvoicetronicClient(
    builder.Configuration["Invoicetronic:ApiKey"] ?? "ik_test_RkvptULx7x5czgd4j8LDStMKuoF6bRSm"));

var app = builder.Build();

// Middleware
app.UseSwagger();
app.UseSwaggerUI();
app.UseCors("AllowFrontend");
app.UseAuthentication();
app.UseAuthorization();

// Initialize database
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

// Helper function to generate JWT token
string GenerateJwtToken(Account account)
{
    var tokenHandler = new JsonWebTokenHandler();
    var key = Encoding.UTF8.GetBytes(jwtKey);
    var claims = new Dictionary<string, object>
    {
        [JwtRegisteredClaimNames.Sub] = account.Id.ToString(),
        [JwtRegisteredClaimNames.Email] = account.Email,
        ["username"] = account.Username,
        ["isAdmin"] = account.IsAdmin
    };
    
    var tokenDescriptor = new SecurityTokenDescriptor
    {
        Claims = claims,
        Expires = DateTime.UtcNow.AddDays(7),
        Issuer = jwtIssuer,
        Audience = jwtAudience,
        SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256)
    };
    
    return tokenHandler.CreateToken(tokenDescriptor);
}

// ============ AUTH ENDPOINTS ============

app.MapGet("/test-password/{password}", async (string password) =>
{
    var hash = BCrypt.Net.BCrypt.HashPassword(password);
    return Results.Ok(new { password, hash, verify = BCrypt.Net.BCrypt.Verify(password, hash) });
});

app.MapPost("/api/auth/login", async (LoginRequest req, AppDbContext db) =>
{
    var account = await db.Accounts.FirstOrDefaultAsync(a => a.Email == req.Email);
    if (account == null || !BCrypt.Net.BCrypt.Verify(req.Password, account.PasswordHash))
        return Results.Unauthorized();
    
    var token = GenerateJwtToken(account);
    return Results.Ok(new
    {
        token,
        user = new { account.Id, account.Username, account.Email, account.IsAdmin }
    });
});

// ============ DASHBOARD ENDPOINTS ============

app.MapGet("/api/dashboard/stats", async (AppDbContext db) =>
{
    var now = DateTime.UtcNow;
    var currentMonth = new DateTime(now.Year, now.Month, 1);
    var lastMonth = currentMonth.AddMonths(-1);
    
    var currentEntrate = await db.Fatture
        .Where(f => f.TipoFattura == "Entrata" && f.DataEmissione >= currentMonth)
        .SumAsync(f => f.TotaleLordo);
    
    var currentUscite = await db.Fatture
        .Where(f => f.TipoFattura == "Uscita" && f.DataEmissione >= currentMonth)
        .SumAsync(f => f.TotaleLordo);
    
    var lastEntrate = await db.Fatture
        .Where(f => f.TipoFattura == "Entrata" && f.DataEmissione >= lastMonth && f.DataEmissione < currentMonth)
        .SumAsync(f => f.TotaleLordo);
    
    var lastUscite = await db.Fatture
        .Where(f => f.TipoFattura == "Uscita" && f.DataEmissione >= lastMonth && f.DataEmissione < currentMonth)
        .SumAsync(f => f.TotaleLordo);
    
    var profitto = currentEntrate - currentUscite;
    var lastProfitto = lastEntrate - lastUscite;
    
    // Last 12 months data
    var twelveMonthsAgo = currentMonth.AddMonths(-12);
    var monthlyData = await db.Fatture
        .Where(f => f.TipoFattura == "Entrata" && f.DataEmissione >= twelveMonthsAgo)
        .GroupBy(f => new { f.DataEmissione.Year, f.DataEmissione.Month })
        .Select(g => new
        {
            Year = g.Key.Year,
            Month = g.Key.Month,
            Valore = g.Sum(f => f.TotaleLordo)
        })
        .OrderBy(x => x.Year).ThenBy(x => x.Month)
        .ToListAsync();
    
    var ultimiDodiciMesi = monthlyData.Select(m => new
    {
        Mese = new DateTime(m.Year, m.Month, 1).ToString("MMM yyyy"),
        Valore = m.Valore
    }).ToList();
    
    return Results.Ok(new
    {
        profitto,
        percentualeProfitto = lastProfitto > 0 ? ((profitto - lastProfitto) / lastProfitto * 100) : 0,
        entrate = currentEntrate,
        percentualeEntrate = lastEntrate > 0 ? ((currentEntrate - lastEntrate) / lastEntrate * 100) : 0,
        ultimiDodiciMesi
    });
});

// ============ FATTURE ENDPOINTS ============

app.MapGet("/api/fatture", async (string? tipo, int? clienteId, int? temaId, AppDbContext db) =>
{
    var query = db.Fatture
        .Include(f => f.Cliente)
        .Include(f => f.MetodoPagamento)
        .Include(f => f.Assoggettamento)
        .Include(f => f.Tema)
        .Include(f => f.Valuta)
        .Include(f => f.Arrotondamento)
        .AsQueryable();
    
    if (!string.IsNullOrEmpty(tipo))
        query = query.Where(f => f.TipoFattura == tipo);
    if (clienteId.HasValue)
        query = query.Where(f => f.ClienteId == clienteId.Value);
    if (temaId.HasValue)
        query = query.Where(f => f.TemaId == temaId.Value);
    
    var fatture = await query.OrderByDescending(f => f.DataEmissione).ToListAsync();
    return Results.Ok(fatture);
});

app.MapGet("/api/fatture/{id:int}", async (int id, AppDbContext db) =>
{
    var fattura = await db.Fatture
        .Include(f => f.Cliente)
        .Include(f => f.MetodoPagamento)
        .Include(f => f.Assoggettamento)
        .Include(f => f.Tema)
        .Include(f => f.Valuta)
        .Include(f => f.Arrotondamento)
        .FirstOrDefaultAsync(f => f.Id == id);
    
    return fattura == null ? Results.NotFound() : Results.Ok(fattura);
});

app.MapPost("/api/fatture", async (FatturaRequest req, AppDbContext db) =>
{
    var assoggettamento = await db.Assoggettamenti.FindAsync(req.AssoggettamentoId);
    if (assoggettamento == null) return Results.BadRequest("Assoggettamento non trovato");
    
    var totaleLordo = req.ImponibileNetto + (req.ImponibileNetto * assoggettamento.Percentuale / 100.0);
    
    var fattura = new Fattura
    {
        NumeroFattura = req.NumeroFattura,
        TipoFattura = req.TipoFattura,
        DataEmissione = req.DataEmissione,
        DataScadenza = req.DataScadenza,
        ImponibileNetto = req.ImponibileNetto,
        TotaleLordo = totaleLordo,
        ClienteId = req.ClienteId,
        MetodoPagamentoId = req.MetodoPagamentoId,
        AssoggettamentoId = req.AssoggettamentoId,
        TemaId = req.TemaId,
        ValutaId = req.ValutaId,
        ArrotondamentoId = req.ArrotondamentoId
    };
    
    db.Fatture.Add(fattura);
    await db.SaveChangesAsync();
    
    return Results.Created($"/api/fatture/{fattura.Id}", fattura);
});

app.MapDelete("/api/fatture/{id:int}", async (int id, AppDbContext db) =>
{
    var fattura = await db.Fatture.FindAsync(id);
    if (fattura == null) return Results.NotFound();
    
    db.Fatture.Remove(fattura);
    await db.SaveChangesAsync();
    
    return Results.Ok();
});

app.MapDelete("/api/fatture/batch", async (string ids, AppDbContext db) =>
{
    var idList = ids.Split(',').Select(int.Parse).ToList();
    var fatture = await db.Fatture.Where(f => idList.Contains(f.Id)).ToListAsync();
    db.Fatture.RemoveRange(fatture);
    await db.SaveChangesAsync();
    
    return Results.Ok(new { deleted = fatture.Count });
});

// ============ CLIENTI ENDPOINTS ============

app.MapGet("/api/clienti", async (AppDbContext db) =>
{
    var clienti = await db.Clienti.OrderBy(c => c.Nome).ToListAsync();
    return Results.Ok(clienti);
});

// ============ LOOKUP POST ENDPOINTS ============

app.MapPost("/api/tipipagamento", async (LookupRequest req, AppDbContext db) =>
{
    var item = new TipoPagamento { Nome = req.Nome };
    db.TipiPagamento.Add(item);
    await db.SaveChangesAsync();
    return Results.Created($"/api/tipipagamento/{item.Id}", item);
});

app.MapPost("/api/assoggettamenti", async (AssoggettamentoRequest req, AppDbContext db) =>
{
    var item = new Assoggettamento { Nome = req.Nome, Percentuale = req.Percentuale };
    db.Assoggettamenti.Add(item);
    await db.SaveChangesAsync();
    return Results.Created($"/api/assoggettamenti/{item.Id}", item);
});

app.MapPost("/api/temi", async (TemaRequest req, AppDbContext db) =>
{
    var item = new Tema { Nome = req.Nome, Colore = req.Colore };
    db.Temi.Add(item);
    await db.SaveChangesAsync();
    return Results.Created($"/api/temi/{item.Id}", item);
});

app.MapPost("/api/valute", async (ValutaRequest req, AppDbContext db) =>
{
    var item = new Valuta { Codice = req.Codice, Nome = req.Nome, Simbolo = req.Simbolo ?? "€" };
    db.Valute.Add(item);
    await db.SaveChangesAsync();
    return Results.Created($"/api/valute/{item.Id}", item);
});

app.MapPost("/api/arrotondamenti", async (LookupRequest req, AppDbContext db) =>
{
    var item = new Arrotondamento { Nome = req.Nome };
    db.Arrotondamenti.Add(item);
    await db.SaveChangesAsync();
    return Results.Created($"/api/arrotondamenti/{item.Id}", item);
});

app.MapGet("/api/scadenze", async (AppDbContext db) => 
    Results.Ok(await db.Temi.ToListAsync()));

// ============ CLIENT ENDPOINTS ============

app.MapPost("/api/clienti", async (ClienteRequest req, AppDbContext db) =>
{
    var cliente = new Cliente
    {
        Nome = req.Nome,
        CodiceFiscale = req.CodiceFiscale,
        PartitaIVA = req.PartitaIVA,
        IBAN = req.IBAN,
        Email = req.Email,
        Telefono = req.Telefono,
        Indirizzo = req.Indirizzo
    };
    
    db.Clienti.Add(cliente);
    await db.SaveChangesAsync();
    
    return Results.Created($"/api/clienti/{cliente.Id}", cliente);
});

// ============ LOOKUP ENDPOINTS ============

app.MapGet("/api/tipipagamento", async (AppDbContext db) => 
    Results.Ok(await db.TipiPagamento.ToListAsync()));

app.MapGet("/api/assoggettamenti", async (AppDbContext db) => 
    Results.Ok(await db.Assoggettamenti.ToListAsync()));

app.MapGet("/api/temi", async (AppDbContext db) => 
    Results.Ok(await db.Temi.ToListAsync()));

app.MapGet("/api/valute", async (AppDbContext db) => 
    Results.Ok(await db.Valute.ToListAsync()));

app.MapGet("/api/arrotondamenti", async (AppDbContext db) => 
    Results.Ok(await db.Arrotondamenti.ToListAsync()));

// ============ ACCOUNT ENDPOINTS ============

app.MapGet("/api/account", async (AppDbContext db) =>
{
    var accounts = await db.Accounts.Select(a => new
    {
        a.Id,
        a.Username,
        a.Email,
        a.IsAdmin,
        a.Created
    }).ToListAsync();
    
    return Results.Ok(accounts);
}).RequireAuthorization();

app.MapPost("/api/account", async (CreateAccountRequest req, AppDbContext db) =>
{
    if (await db.Accounts.AnyAsync(a => a.Email == req.Email))
        return Results.BadRequest("Email già in uso");
    
    var account = new Account
    {
        Username = req.Username,
        Email = req.Email,
        PasswordHash = BCrypt.Net.BCrypt.HashPassword(req.Password),
        IsAdmin = req.IsAdmin ?? false
    };
    
    db.Accounts.Add(account);
    await db.SaveChangesAsync();
    
    return Results.Created($"/api/account/{account.Id}", new
    {
        account.Id,
        account.Username,
        account.Email,
        account.IsAdmin
    });
}).RequireAuthorization();

app.MapPost("/api/account/register", async (RegisterRequest req, AppDbContext db) =>
{
    if (await db.Accounts.AnyAsync(a => a.Email == req.Email))
        return Results.BadRequest("Email già in uso");
    
    var account = new Account
    {
        Username = req.Username,
        Email = req.Email,
        PasswordHash = BCrypt.Net.BCrypt.HashPassword(req.Password),
        IsAdmin = req.IsAdmin
    };
    
    db.Accounts.Add(account);
    await db.SaveChangesAsync();
    
    return Results.Created($"/api/account/{account.Id}", new
    {
        account.Id,
        account.Username,
        account.Email,
        account.IsAdmin
    });
}).RequireAuthorization();

app.MapPut("/api/account/{id:int}", async (int id, UpdateAccountRequest req, AppDbContext db) =>
{
    var account = await db.Accounts.FindAsync(id);
    if (account == null) return Results.NotFound();
    
    if (!string.IsNullOrEmpty(req.Username))
        account.Username = req.Username;
    
    if (!string.IsNullOrEmpty(req.Email))
    {
        if (await db.Accounts.AnyAsync(a => a.Email == req.Email && a.Id != id))
            return Results.BadRequest("Email già in uso");
        account.Email = req.Email;
    }
    
    if (!string.IsNullOrEmpty(req.Password))
        account.PasswordHash = BCrypt.Net.BCrypt.HashPassword(req.Password);
    
    await db.SaveChangesAsync();
    
    return Results.Ok(new
    {
        account.Id,
        account.Username,
        account.Email,
        account.IsAdmin
    });
}).RequireAuthorization();

app.MapDelete("/api/account/{id:int}", async (int id, AppDbContext db) =>
{
    var account = await db.Accounts.FindAsync(id);
    if (account == null) return Results.NotFound();
    
    db.Accounts.Remove(account);
    await db.SaveChangesAsync();
    
    return Results.Ok();
}).RequireAuthorization();

// ============ INVOICETRONIC ENDPOINTS ============

app.MapGet("/api/invoicetronic/companies", async (InvoicetronicClient client) =>
{
    try
    {
        var companies = await client.ListCompaniesAsync();
        return Results.Ok(companies);
    }
    catch (Exception ex)
    {
        return Results.Problem(ex.Message);
    }
});

app.MapGet("/api/invoicetronic/sent", async (InvoicetronicClient client) =>
{
    try
    {
        var invoices = await client.ListSentInvoicesAsync();
        return Results.Ok(invoices);
    }
    catch (Exception ex)
    {
        return Results.Problem(ex.Message);
    }
});

app.MapGet("/api/invoicetronic/received", async (InvoicetronicClient client) =>
{
    try
    {
        var invoices = await client.ListIncomingInvoicesAsync();
        return Results.Ok(invoices);
    }
    catch (Exception ex)
    {
        return Results.Problem(ex.Message);
    }
});

app.MapGet("/api/invoicetronic/status", async (InvoicetronicClient client) =>
{
    try
    {
        var status = await client.GetAccountStatusAsync();
        return Results.Ok(status);
    }
    catch (Exception ex)
    {
        return Results.Problem(ex.Message);
    }
});

app.Run();

// ============ REQUEST/RESPONSE MODELS ============

record LoginRequest(string Email, string Password);
record RegisterRequest(string Username, string Email, string Password, bool IsAdmin);
record CreateAccountRequest(string Username, string Email, string Password, bool? IsAdmin);
record UpdateAccountRequest(string? Username, string? Email, string? Password);
record FatturaRequest(string NumeroFattura, string TipoFattura, DateTime DataEmissione, DateTime DataScadenza,
    double ImponibileNetto, int ClienteId, int MetodoPagamentoId, int AssoggettamentoId,
    int TemaId, int ValutaId, int? ArrotondamentoId);
record ClienteRequest(string Nome, string? CodiceFiscale, string? PartitaIVA, string? IBAN,
    string? Email, string? Telefono, string? Indirizzo);
record LookupRequest(string Nome);
record AssoggettamentoRequest(string Nome, double Percentuale);
record TemaRequest(string Nome, string? Colore);
record ValutaRequest(string Codice, string? Nome, string? Simbolo);
