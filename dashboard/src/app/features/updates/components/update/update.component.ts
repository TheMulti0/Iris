import { Component, HostBinding, Input } from '@angular/core';
import { Update } from 'src/app/models/updates';

@Component({
  selector: 'app-update',
  templateUrl: './update.component.html',
  styleUrls: ['./update.component.scss']
})
export class UpdateComponent {
  @Input()
  update!: Update;

  constructor() {}
}
