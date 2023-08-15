import { MAT_DATE_FORMATS } from '@angular/material/core';
import { Directive } from '@angular/core';

@Directive({
  selector: '[MMYYDateFormat]',
  providers: [
    {
      provide: MAT_DATE_FORMATS,
      useValue: {
        parse: {
          dateInput: 'MM/YYYY',
        },
        display: {
          dateInput: 'MM/YYYY',
          monthYearLabel: 'MMM YYYY',
          dateA11yLabel: 'LL',
          monthYearA11yLabel: 'MMMM YYYY',
        },
      },
    },

  ]
})
export class MMYYDateFormatDirective {

  constructor() { }

}
