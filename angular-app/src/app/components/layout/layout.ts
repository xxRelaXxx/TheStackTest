import { Component, signal, inject, HostListener } from '@angular/core';
import { RouterOutlet, RouterLink, RouterLinkActive, Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-layout',
  standalone: true,
  imports: [RouterOutlet, RouterLink, RouterLinkActive, CommonModule],
  templateUrl: './layout.html',
})
export class LayoutComponent {
  private auth = inject(AuthService);
  private router = inject(Router);

  sidebarOpen = signal(true);
  darkMode = signal(true);

  get user() { return this.auth.user; }

  toggleSidebar() { this.sidebarOpen.update(v => !v); }

  toggleTheme() {
    this.darkMode.update(v => !v);
    // Add 'light' class when switching to light mode; dark is the default
    document.documentElement.classList.toggle('light', !this.darkMode());
    localStorage.setItem('theme', this.darkMode() ? 'dark' : 'light');
  }

  goToAccount() { this.router.navigate(['/account']); }

  logout() {
    this.auth.logout();
    this.router.navigate(['/login']);
  }

  @HostListener('window:resize')
  onResize() {
    if (window.innerWidth < 768) this.sidebarOpen.set(false);
  }

  ngOnInit() {
    if (window.innerWidth < 768) this.sidebarOpen.set(false);
    // Restore saved theme preference
    const saved = localStorage.getItem('theme');
    if (saved === 'light') {
      this.darkMode.set(false);
      document.documentElement.classList.add('light');
    }
  }
}
