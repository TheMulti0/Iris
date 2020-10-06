import { CommonModule } from '@angular/common';

import { NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { LayoutComponent } from './components/layout/layout.component';
import { MaterialModule } from './material.module';
import { ApiAuthorizationModule } from 'src/api-authorization/api-authorization.module';


@NgModule({
  declarations: [LayoutComponent],
  exports: [
    LayoutComponent,
    MaterialModule,
    FormsModule,
    ReactiveFormsModule
  ],
  imports: [
    CommonModule,
    MaterialModule,
    RouterModule,
    FormsModule,
    ReactiveFormsModule,
    ApiAuthorizationModule
  ]
})
export class SharedModule { }
