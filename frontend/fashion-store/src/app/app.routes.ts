import { Routes } from '@angular/router';
import { HomeComponent } from './pages/home/home.component';


export const routes: Routes = [
   
    {path:'home', component:HomeComponent},
   
     {
    path: 'admin',
    loadComponent: () =>
      import('./pages/admin/admin.component').then(m => m.AdminComponent),
    children: [
      { path: '', redirectTo: 'dashboard', pathMatch: 'full' },

      {
        path: 'dashboard',
        loadComponent: () =>
          import('./pages/admin/admin-dashboard/admin-dashboard.component')
            .then(m => m.AdminDashboardComponent)
      },
      {
        path: 'orders',
        loadComponent: () =>
          import('./pages/admin/admin-orders/admin-orders.component')
            .then(m => m.AdminOrdersComponent)
      },
      {
        path: 'products',
        loadComponent: () =>
          import('./pages/admin/admin-products/admin-products.component')
            .then(m => m.AdminProductsComponent)
      },



     
    ]
  }

];
