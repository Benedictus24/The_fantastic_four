import { Routes } from '@angular/router';
import { LoginComponent } from './Pages/login/login.component';
import { RegisterComponent } from './Pages/register/register.component';
import { LandingPageComponent } from './Pages/landing-page/landing-page.component';

export const routes: Routes = [

{path: 'login', component: LoginComponent},
{path: 'register', component: RegisterComponent},
{ path: '', redirectTo: '/login', pathMatch: 'full' },
{path: 'landing', component: LandingPageComponent}

];
