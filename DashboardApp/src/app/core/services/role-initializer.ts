import { MeService } from './me.service';

// Initializes user's roles, if not authenticated it is catched by the not authenticated interceptor
export function getUserRoles(meService: MeService) {
    return () => meService.updateRoles().toPromise();
}