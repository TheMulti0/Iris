import { Component, OnInit } from '@angular/core';
import { NewPostSubscription } from 'src/app/models/posts-listener.model';
import { ItemsObserver } from 'src/app/services/itemsobserver';
import { PostsListenerService } from 'src/app/services/posts-listener.service';

@Component({
  selector: 'app-posts-listener',
  templateUrl: './posts-listener.component.html',
  styleUrls: ['./posts-listener.component.scss']
})
export class PostsListenerComponent implements OnInit {
  
  displayedColumns: string[] = ['id', 'platform', 'pollInterval', 'actions'];
  newPostSubscriptions!: ItemsObserver<NewPostSubscription[]>;

  constructor(
    private postsListener: PostsListenerService
  ) {}

  ngOnInit() {
    this.newPostSubscriptions = new ItemsObserver(
      () => this.postsListener.getSubscriptions()
    );
  }

  async remove(element: NewPostSubscription) {
    await this.postsListener
      .removeSubscription(element.id, element.platform)
      .toPromise();
  }

  async increment(element: NewPostSubscription) {
    await this.postsListener
      .addOrUpdateSubscription(element.id, element.platform, element.pollInterval + 1, '')
      .toPromise();
    
    this.newPostSubscriptions.next();
  }

}
