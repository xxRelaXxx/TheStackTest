import { Routes } from '@angular/router';
import { authGuard } from './guards/auth.guard';

export const routes: Routes = [
  { path: 'login', loadComponent: () => import('./pages/login/login').then(m => m.LoginComponent) },
  {
    path: '',
    loadComponent: () => import('./components/layout/layout').then(m => m.LayoutComponent),
    canActivate: [authGuard],
    children: [
      { path: '', redirectTo: 'dashboard', pathMatch: 'full' },
      { path: 'dashboard', loadComponent: () => import('./pages/dashboard/dashboard').then(m => m.DashboardComponent) },
      { path: 'fatture', loadComponent: () => import('./pages/fatture/fatture').then(m => m.FattureComponent) },
      { path: 'inserisci-fattura', loadComponent: () => import('./pages/inserisci-fattura/inserisci-fattura').then(m => m.InserisciFatturaComponent) },
      { path: 'account', loadComponent: () => import('./pages/account/account').then(m => m.AccountComponent) },
    ]
  },
  { path: '**', redirectTo: '' }
];
