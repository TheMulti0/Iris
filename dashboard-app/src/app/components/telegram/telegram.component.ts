import { Component, OnInit } from '@angular/core';
import { RefreshableObservable } from 'src/app/services/RefreshableObservable';
import { TelegramService } from '../../services/telegram.service';
import { TelegramSubscription } from '../../models/telegram.model';
import {
  trigger,
  state,
  style,
  transition,
  animate,
} from '@angular/animations';

@Component({
  selector: 'app-telegram',
  templateUrl: './telegram.component.html',
  styleUrls: ['./telegram.component.scss'],
  animations: [
    trigger('detailExpand', [
      state('collapsed', style({ height: '0px', minHeight: '0' })),
      state('expanded', style({ height: '*' })),
      transition(
        'expanded <=> collapsed',
        animate('225ms cubic-bezier(0.4, 0.0, 0.2, 1)')
      ),
    ]),
  ],
})
export class TelegramComponent implements OnInit {
  displayedColumns: string[] = ['id', 'userId', 'platform', 'chats'];
  subscriptions$: RefreshableObservable<TelegramSubscription[]>;

  constructor(private telegram: TelegramService) {
    this.subscriptions$ = this.telegram.getRefreshableSubscriptions();
  }

  ngOnInit(): void {}

  click(element: any) {
    if (element.expanded == true) {
      element.expanded = false;
      return;
    }
    element.expanded = true;
  }
}
