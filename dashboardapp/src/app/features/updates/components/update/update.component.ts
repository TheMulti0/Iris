import { HostListener, OnInit } from '@angular/core';
import { Component, HostBinding, Input } from '@angular/core';
import { Update } from 'src/app/models/updates.model';
import { TimeService } from 'src/app/shared/services/time.service';

@Component({
  selector: 'app-update',
  templateUrl: './update.component.html',
  styleUrls: ['./update.component.scss'],
})
export class UpdateComponent implements OnInit {
  @Input()
  update!: Update;

  dateTime!: string | null;

  constructor(private timeService: TimeService) {}

  ngOnInit() {
    this.dateTime = this.timeService.formatDateTime(
      new Date(this.update.creationDate).getTime()
    );
  }

  @HostListener('error', ['$event.target'])
  onError(video: any) {
    video.hide();
  }
}
