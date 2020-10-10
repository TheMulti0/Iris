import { Component, OnInit } from '@angular/core';
import { Update } from '../models/update.model';
import { UpdatesService } from '../services/updates.service';

@Component({
  selector: 'app-updates',
  templateUrl: './updates.component.html',
  styleUrls: ['./updates.component.scss']
})
export class UpdatesComponent implements OnInit {
  public updates: Update[];

  constructor(
    private updatesService: UpdatesService
  ) { }

  async ngOnInit() {
    this.updates = await this.updatesService.getUpdates().toPromise();
  }
}