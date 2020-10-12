import { Component, OnInit } from '@angular/core';
import { MatTableDataSource } from '@angular/material/table';
import { AccountService } from '../core/services/account.service';
import { Update } from '../models/update.model';
import { User } from '../models/user.model';
import { UpdatesService } from '../services/updates.service';

@Component({
  selector: 'app-users',
  templateUrl: './users.component.html',
  styleUrls: ['./users.component.scss']
})
export class UsersComponent implements OnInit {
  users: User[] = []
  dataSource: MatTableDataSource<User> = new MatTableDataSource();
  displayedColumns = [
    "userName",
    "email"
  ];

  constructor(
    private accountService: AccountService
  ) { }

  async ngOnInit() {
    this.users = await this.accountService.getUsers().toPromise();

    this.dataSource = new MatTableDataSource(this.users);
  }
}