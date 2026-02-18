import { Component, signal, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ApiService } from '../../services/api.service';
import { AuthService } from '../../services/auth.service';
import { Account } from '../../models';

@Component({
  selector: 'app-account',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './account.html',
})
export class AccountComponent implements OnInit {
  private api = inject(ApiService);
  private auth = inject(AuthService);

  currentUser = signal<Account | null>(null);
  accounts = signal<Account[]>([]);
  accountsLoading = signal(false);
  accountsError = signal('');

  // Edit profile
  editingProfile = signal(false);
  editUsername = '';
  editEmail = '';
  editPassword = '';
  profileSaving = signal(false);
  profileSuccess = signal(false);
  profileError = signal('');

  // Add account modal (admin only)
  showAddModal = signal(false);
  newUsername = '';
  newEmail = '';
  newPassword = '';
  newIsAdmin = false;
  addSaving = signal(false);
  addError = signal('');

  get isAdmin() { return !!this.auth.user?.isAdmin; }

  ngOnInit() {
    this.currentUser.set(this.auth.user);
    if (this.isAdmin) {
      this.loadAccounts();
    }
  }

  loadAccounts() {
    this.accountsLoading.set(true);
    this.accountsError.set('');
    this.api.getAccounts().subscribe({
      next: data => {
        this.accounts.set(data);
        this.accountsLoading.set(false);
      },
      error: () => {
        this.accountsError.set('Impossibile caricare gli account. Verifica che il backend sia attivo.');
        this.accountsLoading.set(false);
      }
    });
  }

  startEditProfile() {
    const u = this.currentUser();
    this.editUsername = u?.username ?? '';
    this.editEmail = u?.email ?? '';
    this.editPassword = '';
    this.profileSuccess.set(false);
    this.profileError.set('');
    this.editingProfile.set(true);
  }

  cancelEditProfile() { this.editingProfile.set(false); }

  saveProfile() {
    const id = this.auth.user?.id;
    if (!id) return;
    this.profileSaving.set(true);
    this.profileError.set('');
    const req: { username?: string; email?: string; password?: string } = {};
    if (this.editUsername.trim()) req.username = this.editUsername.trim();
    if (this.editEmail.trim()) req.email = this.editEmail.trim();
    if (this.editPassword.trim()) req.password = this.editPassword.trim();

    this.api.updateAccount(id, req).subscribe({
      next: updated => {
        this.auth.setAuth(this.auth.token!, updated);
        this.currentUser.set(updated);
        this.profileSaving.set(false);
        this.profileSuccess.set(true);
        this.editingProfile.set(false);
      },
      error: (err) => {
        this.profileSaving.set(false);
        this.profileError.set(err?.error || 'Errore durante il salvataggio.');
      }
    });
  }

  openAddModal() {
    this.newUsername = '';
    this.newEmail = '';
    this.newPassword = '';
    this.newIsAdmin = false;
    this.addError.set('');
    this.showAddModal.set(true);
  }

  closeAddModal() { this.showAddModal.set(false); }

  addAccount() {
    if (!this.newEmail.trim() || !this.newUsername.trim() || !this.newPassword.trim()) {
      this.addError.set('Compila tutti i campi obbligatori.');
      return;
    }
    this.addSaving.set(true);
    this.addError.set('');
    this.api.createAccount(this.newUsername.trim(), this.newEmail.trim(), this.newPassword.trim(), this.newIsAdmin).subscribe({
      next: a => {
        this.accounts.update(arr => [...arr, a]);
        this.addSaving.set(false);
        this.closeAddModal();
      },
      error: () => {
        this.addSaving.set(false);
        this.addError.set("Errore nella creazione. L'email potrebbe gia' essere in uso.");
      }
    });
  }

  deleteAccount(id: number) {
    if (id === this.auth.user?.id) {
      alert('Non puoi eliminare il tuo account.');
      return;
    }
    if (!confirm('Eliminare questo account?')) return;
    this.api.deleteAccount(id).subscribe({
      next: () => this.accounts.update(arr => arr.filter(a => a.id !== id)),
      error: () => alert("Errore durante l'eliminazione.")
    });
  }

  initials(name: string): string {
    return (name || '?').charAt(0).toUpperCase();
  }
}
