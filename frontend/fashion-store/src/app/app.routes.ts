import { Routes } from '@angular/router';
import { FooterComponent } from './component/footer/footer.component';
import { HomeComponent } from './pages/home/home.component';
import { AdminDashboardComponent } from './pages/admin/admin-dashboard/admin-dashboard.component';

export const routes: Routes = [
   
    {path:'home', component:HomeComponent},
   
   {path:'admin',component:AdminDashboardComponent}
];
