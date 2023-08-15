import { DatePipe } from '@angular/common';
import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'MMyyyyDateFormat'
})
export class MyDateFormatPipe implements PipeTransform {

  transform(date:Date | string) {
    return new DatePipe('en-US').transform(date, "MM/yyyy");
  }

}
