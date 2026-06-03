import { Component } from '@angular/core';
import { Router, RouterModule } from '@angular/router';
import { CommonModule } from '@angular/common';

interface TabItem {
  label: string;
  route: string;
  icon?: string;
}

@Component({
  selector: 'app-mozo-tab-navigation',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './mozo-tab-navigation.html',
  styleUrls: ['./mozo-tab-navigation.css']
})
export class MozoTabNavigation {
  tabs: TabItem[] = [
    { label: 'Mesas', route: 'mis-mesas' },
    { label: 'Comandas', route: 'comandas' },
    { label: 'Llamados', route: 'llamados' }
  ];

  constructor(private router: Router) {}

  isActive(route: string): boolean {
    return this.router.url.includes(route);
  }
}
