import { Routes } from '@angular/router';
import { Home } from './pages/home/home';
import { Shop } from './pages/shop/shop';
import { Degustazione } from './pages/degustazione/degustazione';
import { Contattaci } from './pages/contattaci/contattaci';
import { Admin } from './pages/admin/admin';

export const routes: Routes = [
 { path: '', component: Home },
 { path: 'shop', component: Shop },
 { path: 'degustazione', component: Degustazione },
 { path: 'contattaci', component: Contattaci },
 { path: 'admin', component: Admin },
 { path: '**', redirectTo: '' }
];
