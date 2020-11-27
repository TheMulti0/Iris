import { Component, OnInit, ViewChild } from '@angular/core';
import { MatPaginator } from '@angular/material/paginator';
import { interval, Observable } from 'rxjs';
import { tap } from 'rxjs/operators';
import { ServerSideDataSource } from '../core/services/server-side-data-source';
import { PageSearchParams } from '../models/page.model';
import { Update } from '../models/update.model';
import { UpdatesService } from '../services/updates.service';

@Component({
  selector: 'app-updates',
  templateUrl: './updates.component.html',
  styleUrls: ['./updates.component.scss']
})
export class UpdatesComponent implements OnInit {
  dataSource: ServerSideDataSource<Update> = new ServerSideDataSource();
  updatesLength = 0;

  displayedColumns = [
    'content',
    'authorId',
    'creationDate',
    'url',
    'repost',
    'actions'
  ];
  
  @ViewChild(MatPaginator, { static: true })
  private paginator: MatPaginator;
  
  pageSizeOptions: number[] = [5, 20, 30, 50, 100];

  constructor(
    private updatesService: UpdatesService
  ) { }

  ngOnInit() {
    this.requestCurrentAsync();

    this.paginator.page.subscribe(
      () => this.requestCurrentAsync());

    interval(1000).subscribe(_ => this.onInterval())
  }

  private onInterval() {
    return this.updatesService.getUpdatesCount().subscribe(count => this.onCount(count));
  }

  private onCount(count: number) {
    this.paginator.length = count;

    if (count < this.paginator.pageSize) {
      this.requestCurrentAsync();
    }
  }

  private requestCurrentAsync() {
    this.requestAsync(
      this.createSearchParams(this.paginator.pageIndex));
  }

  private requestAsync(searchParams: PageSearchParams) {
    this.dataSource.pushAsync(() => this.getUpdates(searchParams));
  }

  private getUpdates(searchParams: PageSearchParams): Observable<Update[]> { 
    return this.updatesService.getUpdates(searchParams)
      .pipe(
        tap(updates => this.updatesLength = updates.length)
      );
  }

  private createSearchParams(pageIndex: number): PageSearchParams {
    return {
      pageIndex: pageIndex,
      pageSize: this.paginator.pageSize ?? this.pageSizeOptions[0]
    }
  }

  async remove(update: Update) {
    await this.updatesService.removeUpdate(update.id).toPromise();

    this.requestCurrentAsync();
  }
}