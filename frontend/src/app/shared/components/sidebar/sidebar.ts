import { Component, signal, computed, HostListener, inject, ChangeDetectionStrategy, input, output } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, Router } from '@angular/router';
import { MenuItem, UserProfile } from '../../../core/models/domain/menu-item';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { IconProp } from '@fortawesome/fontawesome-svg-core';
import { AuthService } from '../../../core/services/auth.service';
import {
  faUsers,
  faCog,
  faChartBar,
  faBox,
  faTruck,
  faReceipt,
  faUtensils,
  faClipboardList,
  faTag,
  faTableCells,
  faRightFromBracket,
  faChevronDown,
  faChevronRight,
  faBell,
  faFireBurner,
  faCarrot,
  faListCheck
} from '@fortawesome/free-solid-svg-icons';

@Component({
  changeDetection: ChangeDetectionStrategy.OnPush,
  selector: 'app-sidebar',
  standalone: true,
  imports: [CommonModule, RouterModule, FontAwesomeModule],
  templateUrl: './sidebar.html',
  styleUrls: ['./sidebar.css']
})
export class SidebarComponent {
  private readonly authService = inject(AuthService);

  // Íconos de FontAwesome
  readonly faUsers = faUsers;
  readonly faCog = faCog;
  readonly faChartBar = faChartBar;
  readonly faBox = faBox;
  readonly faTruck = faTruck;
  readonly faReceipt = faReceipt;
  readonly faUtensils = faUtensils;
  readonly faClipboardList = faClipboardList;
  readonly faTag = faTag;
  readonly faTableCells = faTableCells;
  readonly faRightFromBracket = faRightFromBracket;
  readonly faChevronDown = faChevronDown;
  readonly faChevronRight = faChevronRight;
  readonly faBell = faBell;
  readonly faFireBurner = faFireBurner;
  readonly faCarrot = faCarrot;
  readonly faListCheck = faListCheck;


  isCollapsed = signal(true);
  isHovered = signal(false);
  expandedMenus = signal<string[]>([]);
  currentRole = input.required<string>();
  userProfile = input.required<UserProfile>();
  onLogout = output<void>();

  // Configuración de menú por rol
  private menuConfig: Record<string, MenuItem[]> = {
    Gerente: [
      { label: 'Dashboard', icon: 'faChartBar', route: '/staff/gerente/dashboard', roles: ['Gerente'] },
      { label: 'Sistema de avisos', icon: 'faBell', route: '/staff/gerente/avisos', roles: ['Gerente'], dividerAfter: true },
      
      { label: 'Mapa de mesas', icon: 'faTableCells', route: '/staff/gerente/mapa-de-mesas', roles: ['Gerente'] },
      { label: 'Cerrar Caja', icon: 'faReceipt', route: '/staff/gerente/caja', roles: ['Gerente'], dividerAfter: true },
      
      { label: 'Plato del día', icon: 'faTag', route: '/staff/gerente/plato-dia', roles: ['Gerente'] },
      { label: 'Platos y Precios', icon: 'faUtensils', route: '/staff/gerente/platos', roles: ['Gerente'] },
      {
        label: 'Modificar Carta',
        icon: 'faClipboardList',
        route: '/staff/gerente/modificar-carta',
        roles: ['Gerente'],
        children: [
          { label: 'Ver platos', icon: '', route: '/staff/gerente/modificar-carta', roles: ['Gerente'] },
          { label: 'Nuevo plato', icon: '', route: '/staff/gerente/crear-plato', roles: ['Gerente'] }
        ],
        dividerAfter: true
      },
      
      { label: 'Stock/Mercadería', icon: 'faBox', route: '/staff/gerente/stock-mercaderia', roles: ['Gerente'] },
      {
        label: 'Pedidos y Proveedor',
        icon: 'faTruck',
        roles: ['Gerente'],
        children: [
          { label: 'Ver proveedores', icon: '', route: '/staff/gerente/ver-proveedores', roles: ['Gerente'] },
          { label: 'Nuevo proveedor', icon: '', route: '/staff/gerente/nuevo-proveedor', roles: ['Gerente'] }
        ],
        dividerAfter: true
      },
      
      { label: 'Usuarios', icon: 'faUsers', route: '/staff/gerente/usuarios', roles: ['Gerente'] },
      { label: 'Configuración', icon: 'faCog', route: '/staff/gerente/configuracion', roles: ['Gerente'] }
    ],
    Mozo: [
      { label: 'Mesas', icon: 'faTableCells', route: '/staff/mozo/mis-mesas', roles: ['Mozo'] },
      { label: 'Comandas', icon: 'faListCheck', route: '/staff/mozo/comandas', roles: ['Mozo'] },
      { label: 'Llamados', icon: 'faBell', route: '/staff/mozo/llamados', roles: ['Mozo'] }
    ],
    Cocina: [
      { label: 'Comandas', icon: 'faListCheck', route: '/staff/cocina/comandas', roles: ['Cocina'] },
      { label: 'Ingredientes', icon: 'faCarrot', route: '/staff/cocina/ingredientes', roles: ['Cocina'] },
      { label: 'Platos', icon: 'faUtensils', route: '/staff/cocina/platos', roles: ['Cocina'] },
      { label: 'Mise and place', icon: 'faFireBurner', route: '/staff/cocina/mise-and-place', roles: ['Cocina'] }
    ]
  };

  // Computed: Menú filtrado por rol actual
  menuItems = computed(() => {
    const role = this.currentRole();
    return this.menuConfig[role] || [];
  });

  // Computed: Sidebar expandido si está hover o no colapsado
  isExpanded = computed(() => !this.isCollapsed() || this.isHovered());

  constructor(private router: Router) {}



  onMouseEnter(): void {
    if (this.isCollapsed()) {
      this.isHovered.set(true);
    }
  }

  onMouseLeave(): void {
    if (this.isCollapsed()) {
      this.isHovered.set(false);
      this.expandedMenus.set([]);
    }
  }

  toggleSubmenu(label: string): void {
    if (!this.isExpanded()) return;

    this.expandedMenus.update(menus => {
      const index = menus.indexOf(label);
      if (index > -1) {
        return menus.filter(m => m !== label);
      } else {
        return [...menus, label];
      }
    });
  }

  isSubmenuExpanded(label: string): boolean {
    return this.expandedMenus().includes(label);
  }

  isActive(route?: string): boolean {
    if (!route) return false;
    return this.router.url === route;
  }

  isParentActive(item: MenuItem): boolean {
    if (!item.children) return false;
    return item.children.some(child => this.isActive(child.route));
  }

  logout(): void {
    this.onLogout.emit();
  }

  getIconComponent(iconName: string): IconProp | undefined {
    const iconMap: Record<string, IconProp> = {
      'faUsers': this.faUsers,
      'faCog': this.faCog,
      'faChartBar': this.faChartBar,
      'faBox': this.faBox,
      'faTruck': this.faTruck,
      'faReceipt': this.faReceipt,
      'faUtensils': this.faUtensils,
      'faClipboardList': this.faClipboardList,
      'faTag': this.faTag,
      'faTableCells': this.faTableCells,
      'faBell': this.faBell,
      'faFireBurner': this.faFireBurner,
      'faCarrot': this.faCarrot,
      'faListCheck': this.faListCheck
    };
    return iconMap[iconName] || undefined;
  }
}
