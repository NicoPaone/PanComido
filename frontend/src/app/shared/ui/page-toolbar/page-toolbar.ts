import { Component , ChangeDetectionStrategy} from '@angular/core';

@Component({
  changeDetection: ChangeDetectionStrategy.OnPush,
  selector: 'app-page-toolbar',
  imports: [],
  templateUrl: './page-toolbar.html',
  styleUrl: './page-toolbar.css',
})
export class PageToolbar {}
