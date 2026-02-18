using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace Finance_api;

public class InvoicetronicClient
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;
    private const string BaseUrl = "https://api.invoicetronic.com/v1";

    public InvoicetronicClient(string apiKey)
    {
        _apiKey = apiKey;
        _httpClient = new HttpClient { BaseAddress = new Uri(BaseUrl) };
        
        // Basic Authentication: API Key as username, empty password
        var authToken = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{_apiKey}:"));
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authToken);
    }

    // ============ COMPANY ENDPOINTS ============

    public async Task<List<Company>> ListCompaniesAsync(int page = 1, int pageSize = 100, string? sort = null)
    {
        var query = $"?page={page}&page_size={pageSize}";
        if (!string.IsNullOrEmpty(sort)) query += $"&sort={sort}";
        
        var response = await _httpClient.GetAsync($"/company{query}");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<List<Company>>() ?? new();
    }

    public async Task<Company> GetCompanyAsync(int id)
    {
        var response = await _httpClient.GetAsync($"/company/{id}");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<Company>() ?? throw new Exception("Company not found");
    }

    public async Task<Company> AddCompanyAsync(CompanyRequest request)
    {
        var response = await _httpClient.PostAsJsonAsync("/company", request);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<Company>() ?? throw new Exception("Failed to create company");
    }

    public async Task<Company> UpdateCompanyAsync(CompanyUpdateRequest request)
    {
        var response = await _httpClient.PutAsJsonAsync("/company", request);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<Company>() ?? throw new Exception("Failed to update company");
    }

    public async Task DeleteCompanyAsync(int id, bool force = false)
    {
        var url = $"/company/{id}";
        if (force) url += "?force=true";
        
        var response = await _httpClient.DeleteAsync(url);
        response.EnsureSuccessStatusCode();
    }

    // ============ LOG ENDPOINTS ============

    public async Task<List<Event>> ListEventsAsync(int page = 1, int pageSize = 100, DateTime? createdAfter = null, DateTime? createdBefore = null)
    {
        var query = $"?page={page}&page_size={pageSize}";
        if (createdAfter.HasValue) query += $"&created_after={createdAfter.Value:yyyy-MM-ddTHH:mm:ss}";
        if (createdBefore.HasValue) query += $"&created_before={createdBefore.Value:yyyy-MM-ddTHH:mm:ss}";
        
        var response = await _httpClient.GetAsync($"/log{query}");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<List<Event>>() ?? new();
    }

    public async Task<Event> GetEventAsync(int id)
    {
        var response = await _httpClient.GetAsync($"/log/{id}");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<Event>() ?? throw new Exception("Event not found");
    }

    // ============ RECEIVE ENDPOINTS (Incoming Invoices) ============

    public async Task<List<Receive>> ListIncomingInvoicesAsync(int page = 1, int pageSize = 100, bool? unread = null, bool includePayload = false)
    {
        var query = $"?page={page}&page_size={pageSize}&include_payload={includePayload}";
        if (unread.HasValue) query += $"&unread={unread.Value}";
        
        var response = await _httpClient.GetAsync($"/receive{query}");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<List<Receive>>() ?? new();
    }

    public async Task<Receive> GetIncomingInvoiceAsync(int id, bool includePayload = true)
    {
        var response = await _httpClient.GetAsync($"/receive/{id}?include_payload={includePayload}");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<Receive>() ?? throw new Exception("Invoice not found");
    }

    public async Task DeleteIncomingInvoiceAsync(int id)
    {
        var response = await _httpClient.DeleteAsync($"/receive/{id}");
        response.EnsureSuccessStatusCode();
    }

    // ============ SEND ENDPOINTS (Outgoing Invoices) ============

    public async Task<List<Send>> ListSentInvoicesAsync(int page = 1, int pageSize = 100, string? sort = null)
    {
        var query = $"?page={page}&page_size={pageSize}";
        if (!string.IsNullOrEmpty(sort)) query += $"&sort={sort}";
        
        var response = await _httpClient.GetAsync($"/send{query}");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<List<Send>>() ?? new();
    }

    public async Task<Send> GetSentInvoiceAsync(int id)
    {
        var response = await _httpClient.GetAsync($"/send/{id}");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<Send>() ?? throw new Exception("Invoice not found");
    }

    public async Task<Send> SendInvoiceAsync(SendInvoiceRequest request)
    {
        var response = await _httpClient.PostAsJsonAsync("/send", request);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<Send>() ?? throw new Exception("Failed to send invoice");
    }

    public async Task<Send> SendInvoiceFileAsync(Stream fileStream, string fileName)
    {
        using var content = new MultipartFormDataContent();
        var streamContent = new StreamContent(fileStream);
        streamContent.Headers.ContentType = new MediaTypeHeaderValue("application/xml");
        content.Add(streamContent, "file", fileName);
        
        var response = await _httpClient.PostAsync("/send/file", content);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<Send>() ?? throw new Exception("Failed to upload invoice");
    }

    public async Task<ValidationResult> ValidateInvoiceAsync(SendInvoiceRequest request)
    {
        var response = await _httpClient.PostAsJsonAsync("/send/validate", request);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<ValidationResult>() ?? throw new Exception("Validation failed");
    }

    // ============ STATUS ENDPOINT ============

    public async Task<AccountStatus> GetAccountStatusAsync()
    {
        var response = await _httpClient.GetAsync("/status");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<AccountStatus>() ?? throw new Exception("Failed to get status");
    }

    // ============ UPDATE ENDPOINTS (Invoice Status Updates) ============

    public async Task<List<Update>> ListUpdatesAsync(int page = 1, int pageSize = 100, int? sendId = null)
    {
        var query = $"?page={page}&page_size={pageSize}";
        if (sendId.HasValue) query += $"&send_id={sendId.Value}";
        
        var response = await _httpClient.GetAsync($"/update{query}");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<List<Update>>() ?? new();
    }

    public async Task<Update> GetUpdateAsync(int id)
    {
        var response = await _httpClient.GetAsync($"/update/{id}");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<Update>() ?? throw new Exception("Update not found");
    }

    // ============ WEBHOOK ENDPOINTS ============

    public async Task<List<Webhook>> ListWebhooksAsync()
    {
        var response = await _httpClient.GetAsync("/webhook");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<List<Webhook>>() ?? new();
    }

    public async Task<Webhook> AddWebhookAsync(WebhookRequest request)
    {
        var response = await _httpClient.PostAsJsonAsync("/webhook", request);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<Webhook>() ?? throw new Exception("Failed to create webhook");
    }

    public async Task<Webhook> UpdateWebhookAsync(WebhookUpdateRequest request)
    {
        var response = await _httpClient.PutAsJsonAsync("/webhook", request);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<Webhook>() ?? throw new Exception("Failed to update webhook");
    }

    public async Task DeleteWebhookAsync(int id)
    {
        var response = await _httpClient.DeleteAsync($"/webhook/{id}");
        response.EnsureSuccessStatusCode();
    }
}

// ============ DATA MODELS ============

public class Company
{
    public int Id { get; set; }
    public DateTime Created { get; set; }
    public DateTime Updated { get; set; }
    public string Vat { get; set; } = string.Empty;
    public string TaxCode { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public int Counter { get; set; }
    public string Owner { get; set; } = string.Empty;
}

public class CompanyRequest
{
    public string Vat { get; set; } = string.Empty;
    public string? TaxCode { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Email { get; set; }
    public int? Counter { get; set; }
}

public class CompanyUpdateRequest : CompanyRequest
{
    public int Id { get; set; }
}

public class Event
{
    public int Id { get; set; }
    public DateTime Created { get; set; }
    public string Version { get; set; } = string.Empty;
    public string Method { get; set; } = string.Empty;
    public string Endpoint { get; set; } = string.Empty;
    public int StatusCode { get; set; }
    public int? CompanyId { get; set; }
    public string? Error { get; set; }
    public bool Success { get; set; }
}

public class Receive
{
    public int Id { get; set; }
    public DateTime Created { get; set; }
    public string Encoding { get; set; } = "Xml";
    public string FileName { get; set; } = string.Empty;
    public string? Payload { get; set; }
    public string Sender { get; set; } = string.Empty;
    public string RecipientCode { get; set; } = string.Empty;
    public bool IsRead { get; set; }
}

public class Send
{
    public int Id { get; set; }
    public DateTime Created { get; set; }
    public string Encoding { get; set; } = "Xml";
    public string FileName { get; set; } = string.Empty;
    public string? Payload { get; set; }
    public Dictionary<string, string>? MetaData { get; set; }
    public string State { get; set; } = string.Empty;
}

public class SendInvoiceRequest
{
    public string? FileName { get; set; }
    public string Payload { get; set; } = string.Empty;
    public string Encoding { get; set; } = "Xml";
    public Dictionary<string, string>? MetaData { get; set; }
    public bool Validate { get; set; } = false;
}

public class ValidationResult
{
    public bool Success { get; set; }
    public List<string>? Errors { get; set; }
}

public class AccountStatus
{
    public bool Active { get; set; }
    public int RemainingCalls { get; set; }
    public int RemainingCredits { get; set; }
    public string Plan { get; set; } = string.Empty;
}

public class Update
{
    public int Id { get; set; }
    public DateTime Created { get; set; }
    public int SendId { get; set; }
    public string State { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? Xml { get; set; }
}

public class Webhook
{
    public int Id { get; set; }
    public DateTime Created { get; set; }
    public string Url { get; set; } = string.Empty;
    public List<string> Events { get; set; } = new();
    public string Secret { get; set; } = string.Empty;
    public bool Enabled { get; set; }
}

public class WebhookRequest
{
    public string Url { get; set; } = string.Empty;
    public List<string> Events { get; set; } = new();
    public bool Enabled { get; set; } = true;
}

public class WebhookUpdateRequest : WebhookRequest
{
    public int Id { get; set; }
}
