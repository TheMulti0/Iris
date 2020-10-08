import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { LoginComponent } from './components/login/login.component';
import { LogoutComponent } from './components/logout/logout.component';
import { HttpClientModule } from '@angular/common/http';
import { ApiAuthorizationRoutingModule } from './api-authorization-routing.module';

@NgModule({
  imports: [
    CommonModule,
    HttpClientModule,
    ApiAuthorizationRoutingModule
  ],
  declarations: [LoginComponent, LogoutComponent],
  exports: [LoginComponent, LogoutComponent]
})
export class ApiAuthorizationModule { }
