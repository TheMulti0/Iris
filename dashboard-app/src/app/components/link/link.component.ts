import {
  Component,
  EventEmitter,
  Input,
  OnInit,
  Output,
  ViewChild,
} from '@angular/core';
import { MatRipple } from '@angular/material/core';
import { Observable } from 'rxjs';

@Component({
  selector: 'app-link',
  templateUrl: './link.component.html',
  styleUrls: ['./link.component.scss'],
})
export class LinkComponent implements OnInit {
  @Input()
  path!: string;

  @Input()
  name!: string;

  @Input()
  selected!: boolean;

  @Output()
  press = new EventEmitter<string>();

  @ViewChild(MatRipple)
  ripple!: MatRipple;

  constructor() {}

  ngOnInit() {}

  onClick() {
    if (this.selected) {
      return;
    }

    const rippleRef = this.ripple.launch({});

    this.press.emit(this.path);

    rippleRef.fadeOut();
  }
}
