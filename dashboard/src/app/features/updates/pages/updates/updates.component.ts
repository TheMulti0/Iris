import {
  Component,
  ViewChild,
} from '@angular/core';
import {
  CdkVirtualScrollViewport,
} from '@angular/cdk/scrolling';
import { Subject } from 'rxjs';
import { map } from 'rxjs/operators';
import { Update } from 'src/app/models/updates';
import { UpdatesService } from '../../services/updates.service';
import { PaginatedDataSource } from './PaginatedDataSource';

enum Direction {
  Up = 'Up',
  Down = 'Down',
}

@Component({
  selector: 'app-updates',
  templateUrl: './updates.component.html',
  styleUrls: ['./updates.component.scss'],
})
export class UpdatesComponent {
  @ViewChild(CdkVirtualScrollViewport)
  virtualScroll!: CdkVirtualScrollViewport;

  dataSource: PaginatedDataSource<Update>;

  constructor(private service: UpdatesService) {
    this.dataSource = new PaginatedDataSource((index, size) =>
      this.service.getUpdates(index, size).pipe(map((page) => page.content))
    );
  }
  trackByUrl(index: number, update: Update) {
    return update.url;
  }
}


