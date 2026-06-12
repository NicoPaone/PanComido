import { CommonModule } from '@angular/common';
import { Component, computed, inject, viewChild , ChangeDetectionStrategy} from '@angular/core';
import { RouterModule } from '@angular/router';
import { SidebarComponent } from '../../shared/components/sidebar/sidebar';
import { AuthService } from '../../core/services/auth.service';

@Component({
  changeDetection: ChangeDetectionStrategy.OnPush,
  selector: 'app-staff-layout',
  imports: [CommonModule,RouterModule,SidebarComponent],
  templateUrl: './staff-layout.html',
  styleUrl: './staff-layout.css',
})
export class StaffLayout {
  private authService = inject(AuthService);

  sidebar = viewChild(SidebarComponent);
  mainContentCollapsed = computed(() => this.sidebar()?.isCollapsed() ?? true);

  currentRole = computed(() => this.authService.rol());
  
  userProfile = computed(() => this.authService.obtenerPerfilUsuario());

  logout(): void {
    this.authService.logout();
  }
}
