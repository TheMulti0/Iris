import {
  AfterViewInit,
  Component,
  OnInit,
  ViewChild,
  OnDestroy,
} from '@angular/core';
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
import { MatSort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
import { Subscription } from 'rxjs';

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
export class TelegramComponent implements OnInit, AfterViewInit, OnDestroy {
  displayedColumns: string[] = ['id', 'userId', 'platform', 'chats'];
  dataSource = new MatTableDataSource<TelegramSubscription>();

  subscriptions$: RefreshableObservable<TelegramSubscription[]>;
  subscription!: Subscription;

  @ViewChild(MatSort) sort!: MatSort;

  constructor(private telegram: TelegramService) {
    this.subscriptions$ = this.telegram.getRefreshableSubscriptions();
  }
  ngOnInit() {
    this.subscription = this.subscriptions$.subscribe(
      (items) => (this.dataSource.data = items)
    );
  }

  ngAfterViewInit() {
    this.dataSource.sort = this.sort;
    this.dataSource.sortingDataAccessor = (data: TelegramSubscription, header: string) => {
      if (header == 'chats') {
        return data.chats.length;
      }

      const subscription: any = data;
      return subscription[header];
    };
  }

  ngOnDestroy() {
    this.subscription.unsubscribe();
  }

  click(element: any) {
    if (element.expanded == true) {
      element.expanded = false;
      return;
    }
    element.expanded = true;
  }
}
