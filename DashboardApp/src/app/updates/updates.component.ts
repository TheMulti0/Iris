import { Component, OnInit } from '@angular/core';
import { MatTableDataSource } from '@angular/material/table';
import { Update } from '../models/update.model';
import { UpdatesService } from '../services/updates.service';

@Component({
  selector: 'app-updates',
  templateUrl: './updates.component.html',
  styleUrls: ['./updates.component.scss']
})
export class UpdatesComponent implements OnInit {
  updates: Update[] = [];
  dataSource: MatTableDataSource<Update> = new MatTableDataSource();
  displayedColumns = [
    "content",
    "authorId",
    "creationDate",
    "url",
    "repost",
    "actions"
  ];

  constructor(
    private updatesService: UpdatesService
  ) { }

  async ngOnInit() {
    this.updates = await this.updatesService.getUpdates({ pageSize: 20, pageIndex: 0 }).toPromise();

    this.dataSource = new MatTableDataSource(this.updates);
  }

  async remove(update: Update) {
    this.updates.splice(this.updates.indexOf(update), 1);

    this.dataSource = new MatTableDataSource(this.updates);

    await this.updatesService.removeUpdate(update.idd).toPromise();
  }
}