import { of } from 'rxjs';
import { catchError } from 'rxjs/operators';

import { AccountService } from './account.service';


export function checkIfUserIsAuthenticated(accountService: AccountService) {
    return () => {
        return accountService.updateUserAuthenticationStatus().pipe(catchError(error => {
            console.error('Error trying to validate if the user is authenticated');
            return of(null);
        })).toPromise();                    
    };
}