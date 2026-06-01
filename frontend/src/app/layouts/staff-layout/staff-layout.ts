import { CommonModule } from '@angular/common';
import { Component, computed, viewChild } from '@angular/core';
import { RouterModule } from '@angular/router';
import { SidebarComponent } from '../../shared/components/sidebar/sidebar';

@Component({
  selector: 'app-staff-layout',
  imports: [CommonModule,RouterModule,SidebarComponent],
  templateUrl: './staff-layout.html',
  styleUrl: './staff-layout.css',
})
export class StaffLayout {

  sidebar = viewChild(SidebarComponent);
  mainContentCollapsed = computed(() => this.sidebar()?.isCollapsed() ?? true);


}
