import { Component, ViewChild } from '@angular/core';
import { CdkVirtualScrollViewport } from '@angular/cdk/scrolling';
import { Update } from 'src/app/models/updates.model';
import { UpdatesService } from '../../services/updates.service';
import { SlicedDataSource } from '../../../../shared/utils/sliced-datasource';
import { Observable } from 'rxjs';
import { Slice } from 'src/app/models/slice.model';

@Component({
  selector: 'app-updates',
  templateUrl: './updates.component.html',
  styleUrls: ['./updates.component.scss']
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
