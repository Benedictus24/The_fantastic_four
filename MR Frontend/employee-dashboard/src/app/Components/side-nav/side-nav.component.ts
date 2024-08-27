import { Component } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatSidenavModule } from '@angular/material/sidenav';
import { MatListModule } from '@angular/material/list';
import { MatIconModule } from '@angular/material/icon';
import { RouterModule } from '@angular/router';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-side-nav',
  standalone: true,
  imports: [
    CommonModule,
    MatButtonModule,
    MatSidenavModule,
    MatListModule,
    MatIconModule,
    RouterModule
  ],
  templateUrl: './side-nav.component.html',
  styleUrl: './side-nav.component.css'
})
export class SideNavComponent {
  isOpen: boolean = false;
  isAdmin: boolean = false; // This should be set based on user role

  navItems = [
    { name: 'Dashboard', link: '/dashboard', icon: 'dashboard' },
    { name: 'Profile', link: '/profile', icon: 'person' },
    { name: 'Time Tracking', link: '/time-tracking', icon: 'access_time' },
    { name: 'Calendar', link: '/calendar', icon: 'calendar_today' },
  ];

  adminItems = [
    { name: 'Admin Dashboard', link: '/admin', icon: 'admin_panel_settings' },
  ];

  toggleSidenav() {
    this.isOpen = !this.isOpen;
  }
}