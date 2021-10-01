import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { PostsListenerComponent } from './components/posts-listener/posts-listener.component';
import { TelegramComponent } from './components/telegram/telegram.component';

const routes: Routes = [
  {
    path: 'posts-listener',
    component: PostsListenerComponent,
  },
  {
    path: 'telegram',
    component: TelegramComponent,
  },
  {
    path: '',
    redirectTo: '/posts-listener',
    pathMatch: 'full',
  },
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule],
})
export class AppRoutingModule {}
