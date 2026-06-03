import { Component } from '@angular/core';
import { RouterModule } from '@angular/router';
import { MozoTabNavigation } from '../../features/mozo/components/mozo-tab-navigation/mozo-tab-navigation';

@Component({
  selector: 'app-mozo-layout',
  standalone: true,
  imports: [RouterModule, MozoTabNavigation],
  templateUrl: './mozo-layout.html',
  styleUrl: './mozo-layout.css'
})
export class MozoLayout {}
