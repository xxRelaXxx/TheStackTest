namespace Finance_api;

using System.Text.Json.Serialization;

// Account (User)
public class Account
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public bool IsAdmin { get; set; }
    public DateTime Created { get; set; } = DateTime.UtcNow;
}

// Cliente (Customer)
public class Cliente
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string? CodiceFiscale { get; set; }
    public string? PartitaIVA { get; set; }
    public string? IBAN { get; set; }
    public string? Email { get; set; }
    public string? Telefono { get; set; }
    public string? Indirizzo { get; set; }
    public DateTime Created { get; set; } = DateTime.UtcNow;
    
    [JsonIgnore]
    public List<Fattura> Fatture { get; set; } = new();
}

// Fattura (Invoice)
public class Fattura
{
    public int Id { get; set; }
    public string NumeroFattura { get; set; } = string.Empty;
    public string TipoFattura { get; set; } = "Entrata"; // Entrata or Uscita
    public DateTime DataEmissione { get; set; } = DateTime.UtcNow;
    public DateTime DataScadenza { get; set; } = DateTime.UtcNow.AddDays(30);
    public double ImponibileNetto { get; set; }
    public double TotaleLordo { get; set; }
    
    public int ClienteId { get; set; }
    public Cliente Cliente { get; set; } = null!;
    
    public int MetodoPagamentoId { get; set; }
    public TipoPagamento MetodoPagamento { get; set; } = null!;
    
    public int AssoggettamentoId { get; set; }
    public Assoggettamento Assoggettamento { get; set; } = null!;
    
    public int TemaId { get; set; }
    public Tema Tema { get; set; } = null!;
    
    public int ValutaId { get; set; }
    public Valuta Valuta { get; set; } = null!;
    
    public int? ArrotondamentoId { get; set; }
    public Arrotondamento? Arrotondamento { get; set; }
    
    public DateTime Created { get; set; } = DateTime.UtcNow;
}

// Tipo Pagamento (Payment Type)
public class TipoPagamento
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    [JsonIgnore]
    public List<Fattura> Fatture { get; set; } = new();
}

// Assoggettamento (VAT Rate)
public class Assoggettamento
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public double Percentuale { get; set; }
    [JsonIgnore]
    public List<Fattura> Fatture { get; set; } = new();
}

// Tema (Category/Theme)
public class Tema
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string? Colore { get; set; }
    [JsonIgnore]
    public List<Fattura> Fatture { get; set; } = new();
}

// Valuta (Currency)
public class Valuta
{
    public int Id { get; set; }
    public string Codice { get; set; } = string.Empty; // EUR, USD, GBP
    public string? Nome { get; set; }
    public string Simbolo { get; set; } = "€";
    [JsonIgnore]
    public List<Fattura> Fatture { get; set; } = new();
}

// Arrotondamento (Rounding)
public class Arrotondamento
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    [JsonIgnore]
    public List<Fattura> Fatture { get; set; } = new();
}
