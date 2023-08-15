import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'formatMoney'
})
export class FormatMoneyPipe implements PipeTransform {

  transform(value: number | undefined, ...args: unknown[]): unknown {
    if (isNaN(value)) return '';
    return value.toLocaleString("en-US", {
      currency: "USD",
    })
  }

}
