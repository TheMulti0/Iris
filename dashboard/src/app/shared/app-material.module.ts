import { NgModule } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatTooltipModule } from '@angular/material/tooltip';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatCardModule } from '@angular/material/card';
import { MatListModule } from '@angular/material/list';
import { MatChipsModule } from '@angular/material/chips';

const modules = [
  MatButtonModule,
  MatIconModule,
  MatTooltipModule,
  MatToolbarModule,
  MatCardModule,
  MatListModule,
  MatChipsModule
];

@NgModule({
  imports: [...modules],
  exports: [...modules],
})
export class AppMaterialModule {}
