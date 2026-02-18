import { Component, signal, inject } from '@angular/core';
import { Router } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { ApiService } from '../../services/api.service';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [FormsModule, CommonModule],
  templateUrl: './login.html',
})
export class LoginComponent {
  private api = inject(ApiService);
  private auth = inject(AuthService);
  private router = inject(Router);

  email = '';
  password = '';
  showPassword = signal(false);
  loading = signal(false);
  error = signal('');

  togglePassword() { this.showPassword.update(v => !v); }

  login() {
    if (!this.email || !this.password) {
      this.error.set('Inserisci email e password.');
      return;
    }
    this.loading.set(true);
    this.error.set('');

    this.api.login(this.email, this.password).subscribe({
      next: res => {
        this.auth.setAuth(res.token, res.user);
        this.router.navigate(['/dashboard']);
      },
      error: () => {
        this.loading.set(false);
        this.error.set('Credenziali non valide. Riprova.');
      }
    });
  }
}
