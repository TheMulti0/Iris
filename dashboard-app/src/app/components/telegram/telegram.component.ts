import { Component, OnInit } from '@angular/core';
import { ItemsObserver } from 'src/app/services/itemsobserver';
import { TelegramService } from '../../services/telegram.service';
import { TelegramSubscription } from '../../models/telegram.model';

@Component({
  selector: 'app-telegram',
  templateUrl: './telegram.component.html',
  styleUrls: ['./telegram.component.scss'],
})
export class TelegramComponent implements OnInit {
  displayedColumns: string[] = ['id', 'userId', 'platform', 'chats'];
  subscriptions: ItemsObserver<TelegramSubscription[]>;

  constructor(private telegram: TelegramService) {
    this.subscriptions = new ItemsObserver(() =>
      this.telegram.getSubscriptions()
    );
  }

  ngOnInit(): void {}
}
