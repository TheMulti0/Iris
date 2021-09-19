import { Component, OnInit } from '@angular/core';
import { ScraperService } from './services/scraper.service';
import { PostsListenerService } from './services/postslistener.service';
import { TelegramService } from './services/telegram.service';
import { NewPostSubscription } from './models/postslistener.model';
import { Observable } from 'rxjs';
import { ItemsObserver } from './services/itemsobserver';
import { TelegramSubscription } from './models/telegram.model';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss'],
})
export class AppComponent implements OnInit {
  title = 'dashboard-app';

  newPostSubscriptions!: ItemsObserver<NewPostSubscription[]>;
  telegramSubscriptions!: ItemsObserver<TelegramSubscription[]>;

  constructor(
    private scraper: ScraperService,
    private postsListener: PostsListenerService,
    private telegram: TelegramService
  ) {}

  ngOnInit() {
    this.newPostSubscriptions = new ItemsObserver(
      () => this.postsListener.getSubscriptions()
    );

    this.telegramSubscriptions = new ItemsObserver(
      () => this.telegram.getSubscriptions()
    );
  }
}
