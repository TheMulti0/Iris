import { DatePipe } from '@angular/common';
import { Injectable } from '@angular/core';

const milliSecond = 1;
const second = 1000 * milliSecond;
const minute = 60 * second;
const hour = 60 * minute;
const day = 24 * hour;
const week = 7 * day;

@Injectable({
  providedIn: 'root'
})
export class TimeService {

  constructor(
    private datePipe: DatePipe
  ) { }

  formatDateTime(dateTime: number) {
    const now = new Date().getTime();

    const format = this.getDateTimeFormat(now - dateTime);

    return this.datePipe.transform(dateTime, format);
  }

  getDateTimeFormat(rangeSpanMsec: number) {
    if (rangeSpanMsec >= 2 * week) {
      return 'dd/MM/yyyy';
    }
    if (rangeSpanMsec >= week) {
      return 'dd/MM';
    } else if (rangeSpanMsec >= day) {
      return 'HH:mm, dd/MM';
    } else if (rangeSpanMsec >= 10 * minute) {
      return 'HH:mm';
    }
    return 'HH:mm:ss';
  }
}
