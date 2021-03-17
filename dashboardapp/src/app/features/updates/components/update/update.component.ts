import { HostListener } from '@angular/core';
import { Component, HostBinding, Input } from '@angular/core';
import { Update } from 'src/app/models/updates.model';

@Component({
  selector: 'app-update',
  templateUrl: './update.component.html',
  styleUrls: ['./update.component.scss']
})
export class UpdateComponent {
  @Input()
  update!: Update;

  constructor() {}

  @HostListener('error', ['$event.target'])
  onError(video: any) {
    video.hide();
 }
}
