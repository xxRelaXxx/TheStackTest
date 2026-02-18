import { Injectable, signal } from '@angular/core';
import { Account } from '../models';

const TOKEN_KEY = 'finance_token';
const USER_KEY = 'finance_user';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private _token = signal<string | null>(localStorage.getItem(TOKEN_KEY));
  private _user = signal<Account | null>(this.loadUser());

  get token() { return this._token(); }
  get user() { return this._user(); }
  get isLoggedIn() { return !!this._token(); }

  setAuth(token: string, user: Account) {
    localStorage.setItem(TOKEN_KEY, token);
    localStorage.setItem(USER_KEY, JSON.stringify(user));
    this._token.set(token);
    this._user.set(user);
  }

  logout() {
    localStorage.removeItem(TOKEN_KEY);
    localStorage.removeItem(USER_KEY);
    this._token.set(null);
    this._user.set(null);
  }

  private loadUser(): Account | null {
    const raw = localStorage.getItem(USER_KEY);
    return raw ? JSON.parse(raw) : null;
  }
}
