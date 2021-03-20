import { Pipe, PipeTransform } from '@angular/core';

const linkRegex = /(https:\/\/(\w|\.|\/)+)/;

@Pipe({
  name: 'format',
})
export class FormatPipe implements PipeTransform {
  transform(value: string, ...args: any[]): string {
    return value
      .split('\n').join('<br>')
      .replace(linkRegex, '<a href="$1">$1</a>');
  }
}
