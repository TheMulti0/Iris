import { Component, ViewChild } from '@angular/core';
import { CdkVirtualScrollViewport } from '@angular/cdk/scrolling';
import { map } from 'rxjs/operators';
import { Update } from 'src/app/models/updates';
import { UpdatesService } from '../../services/updates.service';
import { SlicedDataSource } from '../../../../shared/utils/sliced-datasource';
import { Observable } from 'rxjs';
import {
  animate,
  state,
  style,
  transition,
  trigger,
} from '@angular/animations';
import { Slice } from 'src/app/models/slice';

@Component({
  selector: 'app-updates',
  templateUrl: './updates.component.html',
  styleUrls: ['./updates.component.scss'],
  animations: [
    trigger('hiddenShown', [
      transition(':enter', [
        style({
          opacity: 0.9,
          marginLeft: '12%',
        }),
        animate(
          '2s ease-out',
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

  dataSource: SlicedDataSource<Update>;

  constructor(private service: UpdatesService) {
    this.dataSource = new SlicedDataSource((index, limit) =>
      this.getUpdates(index, limit)
    );
  }

  private getUpdates(index: number, size: number): Observable<Slice<Update>> {
    return this.service
      .getUpdates(index, size);
  }

  trackByUrl(index: number, update: Update) {
    return update.url;
  }
}
