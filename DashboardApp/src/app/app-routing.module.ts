import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { AppActions } from './app.constants';
import { CounterComponent } from './counter/counter.component';
import { HomeComponent } from './home/home.component';
import { LayoutComponent } from './shared/components/layout/layout.component';
import { UpdatesComponent } from './updates/updates.component';

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
        path: AppActions.Counter,
        component: CounterComponent
      },
      { 
        path: AppActions.Updates,
        component: UpdatesComponent 
      }
    ]
  }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
