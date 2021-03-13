import { Component, ViewChild } from '@angular/core';
import { CdkVirtualScrollViewport } from '@angular/cdk/scrolling';
import { map } from 'rxjs/operators';
import { Update } from 'src/app/models/updates';
import { UpdatesService } from '../../services/updates.service';
import { PaginatedDataSource } from '../../../../shared/utils/paginated-datasource';
import { Observable } from 'rxjs';
import {
  animate,
  state,
  style,
  transition,
  trigger,
} from '@angular/animations';

@Component({
  selector: 'app-updates',
  templateUrl: './updates.component.html',
  styleUrls: ['./updates.component.scss'],
  animations: [
    trigger('hiddenShown', [
      transition(':enter', [
        style({
          opacity: 0.9,
          marginLeft: '2%',
        }),
        animate(
          '0.25s ease-out',
          style({
            opacity: 1,
            marginLeft: '0%',
          })
        ),
      ]),
    ]),
  ],
})
export class UpdatesComponent {
  @ViewChild(CdkVirtualScrollViewport)
  virtualScroll!: CdkVirtualScrollViewport;

  dataSource: PaginatedDataSource<Update>;

  constructor(private service: UpdatesService) {
    this.dataSource = new PaginatedDataSource(20, (index, size) =>
      this.getUpdates(index, size)
    );
  }

  private getUpdates(index: number, size: number): Observable<Update[]> {
    return this.service
      .getUpdates(index, size)
      .pipe(map((page) => page.content));
  }

  trackByUrl(index: number, update: Update) {
    return update.url;
  }
}
