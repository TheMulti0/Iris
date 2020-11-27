import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { AppActions } from './app.constants';
import { AuthorizeGuard } from './core/guards/authorize.guard';
import { SuperUserGuard } from './core/guards/superuser.guard';
import { HomeComponent } from './home/home.component';
import { LayoutComponent } from './shared/components/layout/layout.component';
import { UpdatesComponent } from './updates/updates.component';
import { UsersComponent } from './users/users.component';

const routes: Routes = [
  {
    path: '',
    component: LayoutComponent,
    children: [
      {
        path: AppActions.Home,
        component: HomeComponent,
        pathMatch: 'full' 
      },
      { 
        path: AppActions.Updates,
        canActivate: [ AuthorizeGuard ],
        component: UpdatesComponent 
      },
      {
        path: AppActions.Users,
        canActivate: [ SuperUserGuard ],
        component: UsersComponent
      }
    ]
  }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
