import { Component, OnInit } from '@angular/core';
import { RefreshableObservable } from 'src/app/services/RefreshableObservable';
import { TelegramService } from '../../services/telegram.service';
import { TelegramSubscription } from '../../models/telegram.model';
import { environment } from 'src/environments/environment';

@Component({
  selector: 'app-telegram',
  templateUrl: './telegram.component.html',
  styleUrls: ['./telegram.component.scss'],
})
export class TelegramComponent implements OnInit {
  displayedColumns: string[] = ['id', 'userId', 'platform', 'chats'];
  subscriptions$: RefreshableObservable<TelegramSubscription[]>;

  constructor(private telegram: TelegramService) {
    this.subscriptions$ = new RefreshableObservable(
      this.telegram.getSubscriptions(),
      environment.pollingIntervalMs
    );
  }

  ngOnInit(): void {}
}
