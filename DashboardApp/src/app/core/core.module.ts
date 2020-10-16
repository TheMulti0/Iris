import { APP_INITIALIZER, NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { AuthenticationService } from './services/authentication.service';
import { checkIfUserIsAuthenticated } from './services/check-login-intializer';
import { NotAuthenticatedInterceptor } from './services/not-authenticated.interceptor';


@NgModule({
  imports: [
    CommonModule,
    HttpClientModule
  ],
  providers: [
    { provide: APP_INITIALIZER, useFactory: checkIfUserIsAuthenticated, multi: true, deps: [AuthenticationService] },
    { provide: HTTP_INTERCEPTORS, useClass: NotAuthenticatedInterceptor, multi: true }
  ],
})
export class CoreModule {
}
