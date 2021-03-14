import { Component, HostBinding, Input } from '@angular/core';
import { Update } from 'src/app/models/updates';
import {
  animate,
  state,
  style,
  transition,
  trigger,
} from '@angular/animations';

@Component({
  selector: 'app-update',
  templateUrl: './update.component.html',
  styleUrls: ['./update.component.scss'],
  animations: [
    trigger('hiddenShown', [
      transition(':enter', [
        style({
          opacity: 0.9,
          marginLeft: '5%',
        }),
        animate(
          '0.25s ease-out',
          style({
            opacity: 1,
            marginLeft: '0%',
          })
        ),
      ]),
    ]),
  ],
})
export class UpdateComponent {
  @Input()
  update!: Update;

  @HostBinding('@hiddenShown')
  get getHiddenShown(): boolean {
    return true;
  }

  constructor() {}
}
