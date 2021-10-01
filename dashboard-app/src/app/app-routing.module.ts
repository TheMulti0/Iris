import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { PostsListenerComponent } from './components/posts-listener/posts-listener.component';

const routes: Routes = [
  {
    path: 'posts-listener',
    component: PostsListenerComponent,
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
