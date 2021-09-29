import { Component, Inject, OnInit } from '@angular/core';
import { MAT_SNACK_BAR_DATA } from '@angular/material/snack-bar';

export interface Notification {
  message: string;
  id: string;
  platform: string;
  type: 'Success' | 'Error';
}

@Component({
  selector: 'app-notification',
  templateUrl: './notification.component.html',
  styleUrls: ['./notification.component.scss'],
})
export class NotificationComponent implements OnInit {
  background!: string;

  constructor(
    @Inject(MAT_SNACK_BAR_DATA) public data: Notification
  ) { }

  ngOnInit() {
    if (this.data.type === 'Success'){
      this.background = '#a8e999';
    }
    else {
      this.background = '#e3999b';
    }
  }
}
