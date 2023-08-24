import { DatePipe } from '@angular/common';
import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'dmyDateFormat'
})
export class DmyDateFormatPipe implements PipeTransform {

  transform(date:Date | string) {
    return new DatePipe('en-US').transform(date, "dd/MM/yyyy");
  }

}
