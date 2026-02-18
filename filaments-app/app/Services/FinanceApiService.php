<?php

namespace App\Services;

use Illuminate\Support\Facades\Http;
use Illuminate\Support\Facades\Session;

class FinanceApiService
{
    protected string $baseUrl;
    protected ?string $token;

    public function __construct()
    {
        $this->baseUrl = config('services.finance_api.url', 'http://localhost:5058');
        $this->token = Session::get('api_token');
    }

    protected function getHeaders(): array
    {
        $headers = [
            'Accept' => 'application/json',
            'Content-Type' => 'application/json',
        ];

        if ($this->token) {
            $headers['Authorization'] = "Bearer {$this->token}";
        }

        return $headers;
    }

    // ============ AUTH ============
    
    public function login(string $email, string $password): array
    {
        $response = Http::withHeaders($this->getHeaders())
            ->post("{$this->baseUrl}/api/auth/login", [
                'email' => $email,
                'password' => $password,
            ]);

        if ($response->successful()) {
            $data = $response->json();
            $this->token = $data['token'];
            Session::put('api_token', $this->token);
            Session::put('user', $data['user']);
            return $data;
        }

        throw new \Exception('Login failed');
    }

    public function logout(): void
    {
        Session::forget('api_token');
        Session::forget('user');
        $this->token = null;
    }

    public function getUser(): ?array
    {
        return Session::get('user');
    }

    // ============ DASHBOARD ============
    
    public function getDashboardStats(): array
    {
        $response = Http::withHeaders($this->getHeaders())
            ->get("{$this->baseUrl}/api/dashboard/stats");

        return $response->successful() ? $response->json() : [];
    }

    // ============ FATTURE ============
    
    public function getFatture(?string $tipo = null, ?int $clienteId = null, ?int $temaId = null): array
    {
        $params = array_filter([
            'tipo' => $tipo,
            'clienteId' => $clienteId,
            'temaId' => $temaId,
        ]);

        $response = Http::withHeaders($this->getHeaders())
            ->get("{$this->baseUrl}/api/fatture", $params);

        return $response->successful() ? $response->json() : [];
    }

    public function getFattura(int $id): ?array
    {
        $response = Http::withHeaders($this->getHeaders())
            ->get("{$this->baseUrl}/api/fatture/{$id}");

        return $response->successful() ? $response->json() : null;
    }

    public function createFattura(array $data): array
    {
        $response = Http::withHeaders($this->getHeaders())
            ->post("{$this->baseUrl}/api/fatture", $data);

        if ($response->successful()) {
            return $response->json();
        }

        throw new \Exception($response->json()['message'] ?? 'Failed to create fattura');
    }

    public function deleteFattura(int $id): bool
    {
        $response = Http::withHeaders($this->getHeaders())
            ->delete("{$this->baseUrl}/api/fatture/{$id}");

        return $response->successful();
    }

    public function deleteFattureBatch(array $ids): bool
    {
        $response = Http::withHeaders($this->getHeaders())
            ->delete("{$this->baseUrl}/api/fatture/batch", $ids);

        return $response->successful();
    }

    // ============ CLIENTI ============
    
    public function getClienti(): array
    {
        $response = Http::withHeaders($this->getHeaders())
            ->get("{$this->baseUrl}/api/clienti");

        return $response->successful() ? $response->json() : [];
    }

    public function createCliente(array $data): array
    {
        $response = Http::withHeaders($this->getHeaders())
            ->post("{$this->baseUrl}/api/clienti", $data);

        if ($response->successful()) {
            return $response->json();
        }

        throw new \Exception('Failed to create cliente');
    }

    // ============ LOOKUP DATA ============
    
    public function getTipiPagamento(): array
    {
        $response = Http::withHeaders($this->getHeaders())
            ->get("{$this->baseUrl}/api/tipipagamento");

        return $response->successful() ? $response->json() : [];
    }

    public function getAssoggettamenti(): array
    {
        $response = Http::withHeaders($this->getHeaders())
            ->get("{$this->baseUrl}/api/assoggettamenti");

        return $response->successful() ? $response->json() : [];
    }

    public function getTemi(): array
    {
        $response = Http::withHeaders($this->getHeaders())
            ->get("{$this->baseUrl}/api/temi");

        return $response->successful() ? $response->json() : [];
    }

    public function getValute(): array
    {
        $response = Http::withHeaders($this->getHeaders())
            ->get("{$this->baseUrl}/api/valute");

        return $response->successful() ? $response->json() : [];
    }

    public function getArrotondamenti(): array
    {
        $response = Http::withHeaders($this->getHeaders())
            ->get("{$this->baseUrl}/api/arrotondamenti");

        return $response->successful() ? $response->json() : [];
    }

    // ============ ACCOUNT ============
    
    public function getAccounts(): array
    {
        $response = Http::withHeaders($this->getHeaders())
            ->get("{$this->baseUrl}/api/account");

        return $response->successful() ? $response->json() : [];
    }

    public function createAccount(array $data): array
    {
        $response = Http::withHeaders($this->getHeaders())
            ->post("{$this->baseUrl}/api/account", $data);

        if ($response->successful()) {
            return $response->json();
        }

        throw new \Exception('Failed to create account');
    }

    public function updateAccount(int $id, array $data): array
    {
        $response = Http::withHeaders($this->getHeaders())
            ->put("{$this->baseUrl}/api/account/{$id}", $data);

        if ($response->successful()) {
            return $response->json();
        }

        throw new \Exception('Failed to update account');
    }

    public function deleteAccount(int $id): bool
    {
        $response = Http::withHeaders($this->getHeaders())
            ->delete("{$this->baseUrl}/api/account/{$id}");

        return $response->successful();
    }
}
