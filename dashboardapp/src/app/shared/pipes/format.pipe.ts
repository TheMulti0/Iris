import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'format',
})
export class FormatPipe implements PipeTransform {
  transform(value: string, ...args: any[]): string {
    return value.replace('\n', '<br></br>');
  }
}
