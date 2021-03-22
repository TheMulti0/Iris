import { NgModule } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatTooltipModule } from '@angular/material/tooltip';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatCardModule } from '@angular/material/card';
import { MatListModule } from '@angular/material/list';
import { MatMenuModule } from '@angular/material/menu';
import { MatChipsModule } from '@angular/material/chips';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSidenavModule } from '@angular/material/sidenav';
import { MatRippleModule } from '@angular/material/core';
import { OverlayModule } from '@angular/cdk/overlay';

const modules = [
  MatButtonModule,
  MatIconModule,
  MatTooltipModule,
  MatToolbarModule,
  MatCardModule,
  MatListModule,
  MatMenuModule,
  MatRippleModule,
  MatChipsModule,
  MatProgressSpinnerModule,
  MatSidenavModule
];

@NgModule({
  imports: [...modules],
  exports: [...modules],
})
export class AppMaterialModule {}
