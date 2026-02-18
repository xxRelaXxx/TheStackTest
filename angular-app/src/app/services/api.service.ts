import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import {
  Account, Cliente, TipoPagamento, Assoggettamento, Tema, Valuta,
  Arrotondamento, Fattura, DashboardStats, LoginResponse, FatturaRequest
} from '../models';
import { AuthService } from './auth.service';

const API = 'http://localhost:5241';

@Injectable({ providedIn: 'root' })
export class ApiService {
  private http = inject(HttpClient);
  private auth = inject(AuthService);

  private authHeaders(): HttpHeaders {
    return new HttpHeaders({ Authorization: `Bearer ${this.auth.token}` });
  }

  // ─── Auth ─────────────────────────────────────────────────────────────────
  login(email: string, password: string): Observable<LoginResponse> {
    return this.http.post<LoginResponse>(`${API}/api/auth/login`, { email, password });
  }

  // ─── Dashboard ────────────────────────────────────────────────────────────
  getDashboardStats(): Observable<DashboardStats> {
    return this.http.get<DashboardStats>(`${API}/api/dashboard/stats`);
  }

  // ─── Fatture ──────────────────────────────────────────────────────────────
  getFatture(tipo?: string, clienteId?: number, temaId?: number): Observable<Fattura[]> {
    let url = `${API}/api/fatture`;
    const params: string[] = [];
    if (tipo) params.push(`tipo=${tipo}`);
    if (clienteId) params.push(`clienteId=${clienteId}`);
    if (temaId) params.push(`temaId=${temaId}`);
    if (params.length) url += '?' + params.join('&');
    return this.http.get<Fattura[]>(url);
  }

  getFattura(id: number): Observable<Fattura> {
    return this.http.get<Fattura>(`${API}/api/fatture/${id}`);
  }

  createFattura(req: FatturaRequest): Observable<Fattura> {
    return this.http.post<Fattura>(`${API}/api/fatture`, req);
  }

  deleteFattura(id: number): Observable<void> {
    return this.http.delete<void>(`${API}/api/fatture/${id}`);
  }

  deleteFattureBatch(ids: number[]): Observable<{ deleted: number }> {
    return this.http.delete<{ deleted: number }>(`${API}/api/fatture/batch?ids=${ids.join(',')}`);
  }

  // ─── Clienti ──────────────────────────────────────────────────────────────
  getClienti(): Observable<Cliente[]> {
    return this.http.get<Cliente[]>(`${API}/api/clienti`);
  }

  createCliente(req: Partial<Cliente>): Observable<Cliente> {
    return this.http.post<Cliente>(`${API}/api/clienti`, req);
  }

  // ─── Lookup tables ────────────────────────────────────────────────────────
  getTipiPagamento(): Observable<TipoPagamento[]> {
    return this.http.get<TipoPagamento[]>(`${API}/api/tipipagamento`);
  }
  createTipoPagamento(nome: string): Observable<TipoPagamento> {
    return this.http.post<TipoPagamento>(`${API}/api/tipipagamento`, { nome });
  }

  getAssoggettamenti(): Observable<Assoggettamento[]> {
    return this.http.get<Assoggettamento[]>(`${API}/api/assoggettamenti`);
  }
  createAssoggettamento(nome: string, percentuale: number): Observable<Assoggettamento> {
    return this.http.post<Assoggettamento>(`${API}/api/assoggettamenti`, { nome, percentuale });
  }

  getTemi(): Observable<Tema[]> {
    return this.http.get<Tema[]>(`${API}/api/temi`);
  }
  createTema(nome: string, colore?: string): Observable<Tema> {
    return this.http.post<Tema>(`${API}/api/temi`, { nome, colore });
  }

  getValute(): Observable<Valuta[]> {
    return this.http.get<Valuta[]>(`${API}/api/valute`);
  }
  createValuta(codice: string, nome: string, simbolo: string): Observable<Valuta> {
    return this.http.post<Valuta>(`${API}/api/valute`, { codice, nome, simbolo });
  }

  getArrotondamenti(): Observable<Arrotondamento[]> {
    return this.http.get<Arrotondamento[]>(`${API}/api/arrotondamenti`);
  }
  createArrotondamento(nome: string): Observable<Arrotondamento> {
    return this.http.post<Arrotondamento>(`${API}/api/arrotondamenti`, { nome });
  }

  // ─── Account ──────────────────────────────────────────────────────────────
  getAccounts(): Observable<Account[]> {
    return this.http.get<Account[]>(`${API}/api/account`, { headers: this.authHeaders() });
  }

  createAccount(username: string, email: string, password: string, isAdmin = false): Observable<Account> {
    return this.http.post<Account>(`${API}/api/account`, { username, email, password, isAdmin },
      { headers: this.authHeaders() });
  }

  updateAccount(id: number, req: { username?: string; email?: string; password?: string }): Observable<Account> {
    return this.http.put<Account>(`${API}/api/account/${id}`, req,
      { headers: this.authHeaders() });
  }

  deleteAccount(id: number): Observable<void> {
    return this.http.delete<void>(`${API}/api/account/${id}`, { headers: this.authHeaders() });
  }
}
