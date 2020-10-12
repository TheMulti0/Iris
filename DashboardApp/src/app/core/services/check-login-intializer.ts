import { of } from 'rxjs';
import { catchError } from 'rxjs/operators';

import { AuthenticationService } from './authentication.service';


export function checkIfUserIsAuthenticated(accountService: AuthenticationService) {
    return () => {
        return accountService.updateAuthenticationStatus().pipe(catchError(error => {
            console.error('Error trying to validate if the user is authenticated');
            return of(null);
        })).toPromise();                    
    };
}