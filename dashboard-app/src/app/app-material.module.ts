import { NgModule } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatCardModule } from '@angular/material/card';
import { MatTableModule } from '@angular/material/table';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatInputModule } from '@angular/material/input';
import { MatSnackBarModule } from '@angular/material/snack-bar';
import { MatTooltipModule } from '@angular/material/tooltip';
import { MatRippleModule } from '@angular/material/core';
import { MatExpansionModule } from '@angular/material/expansion';
import { MatSortModule } from '@angular/material/sort';

const modules = [
  MatButtonModule,
  MatIconModule,
  MatCardModule,
  MatTableModule,
  MatFormFieldModule,
  MatToolbarModule,
  MatInputModule,
  MatSnackBarModule,
  MatTooltipModule,
  MatRippleModule,
  MatExpansionModule,
  MatSortModule,
];

@NgModule({
  imports: modules,
  exports: modules,
})
export class AppMaterialModule {}
