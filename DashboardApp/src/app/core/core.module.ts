import { APP_INITIALIZER, NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { AuthenticationService } from './services/authentication.service';
import { checkIfUserIsAuthenticated } from './services/check-login-intializer';
import { NotAuthenticatedInterceptor } from './interceptors/not-authenticated.interceptor';
import { getUserRoles } from './services/role-initializer';
import { MeService } from './services/me.service';


@NgModule({
  imports: [
    CommonModule,
    HttpClientModule
  ],
  providers: [
    { provide: APP_INITIALIZER, useFactory: checkIfUserIsAuthenticated, multi: true, deps: [AuthenticationService] },
    { provide: APP_INITIALIZER, useFactory: getUserRoles, multi: true, deps: [MeService] },
    { provide: HTTP_INTERCEPTORS, useClass: NotAuthenticatedInterceptor, multi: true }
  ],
})
export class CoreModule {
}
