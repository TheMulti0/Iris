import { Component, OnInit } from '@angular/core';
import { MatTableDataSource } from '@angular/material/table';
import { AuthenticationService } from '../core/services/authentication.service';
import { UsersService } from '../core/services/users.service';
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
    private usersService: UsersService
  ) { }

  async ngOnInit() {
    this.users = await this.usersService.getUsers().toPromise();

    this.dataSource = new MatTableDataSource(this.users);
  }
}