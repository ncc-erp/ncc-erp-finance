import { Component, Inject, Injector, OnInit } from '@angular/core';
import { MAT_MOMENT_DATE_ADAPTER_OPTIONS, MomentDateAdapter } from '@angular/material-moment-adapter';
import { DateAdapter, MAT_DATE_FORMATS, MAT_DATE_LOCALE } from '@angular/material/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { CurrencyConvertService } from '@app/service/api/currency-convert.service';
import { CurrencyService } from '@app/service/api/currency.service';
import { AppComponentBase } from '@shared/app-component-base';
import * as moment from 'moment';
import { Moment } from 'moment';
import { CurrencyConvertDto } from '../currency-convert.component';

export const MY_FORMATS = {
  parse: {
    dateInput: 'YYYY',
  },
  display: {
    dateInput: 'YYYY',
    monthYearLabel: 'YYYY',
    monthYearA11yLabel: 'YYYY',
  },
};
@Component({
  selector: 'app-create-edit-currency-convert',
  templateUrl: './create-edit-currency-convert.component.html',
  styleUrls: ['./create-edit-currency-convert.component.css'],
  providers: [
    {
      provide: DateAdapter,
      useClass: MomentDateAdapter,
      deps: [MAT_DATE_LOCALE, MAT_MOMENT_DATE_ADAPTER_OPTIONS]
    },
    { 
     provide: MAT_DATE_FORMATS, useValue: MY_FORMATS
    },
    {
      provide: MAT_DATE_LOCALE, useValue: "YYYY"
    }
   ]
})

export class CreateEditCurrencyConvertComponent extends AppComponentBase implements OnInit {

  constructor(injector: Injector,
    @Inject(MAT_DIALOG_DATA) public data: any,
    public dialogRef: MatDialogRef<CreateEditCurrencyConvertComponent>,
    private _currencyService: CurrencyService,
    public currencyConvertService: CurrencyConvertService) {
    super(injector);
  }

  public currencyConvert = {} as CurrencyConvertDto;
  public title:string = "";
  public isEditing:boolean = false;
  public isLoading:boolean = false;
  public listCurrencies: CurrencyDto[] = [];
  public date = new Date()
  ngOnInit(): void {
    this.title = this.data.title;
    if(this.data.currencyConvert){
      this.currencyConvert = this.data.currencyConvert;
      this.isEditing = true;
    }
    this.getAllCurrency()
  }

  public saveAndClose(){
    if(this.isEditing){
      this.onUpdate();
    }else{
      this.onCreate();
    }
  }

  private onUpdate(){
    let input = {
      id: this.currencyConvert.id,
      value: this.currencyConvert.value
    }
    this.isLoading = true;
    this.currencyConvertService.update(input).subscribe((rs)=>{
      if(rs){
        abp.notify.success("Update successful");
        this.isLoading = false;
      }
      this.dialogRef.close(true);
    },()=> this.isLoading = false);
  }

  private onCreate(){
    this.isLoading = true;
    let input ={
      currencyId: this.currencyConvert.currencyId,
      value: this.currencyConvert.value,
      year:  Number(moment(this.date).format('YYYY'))
    }
    this.currencyConvertService.create(input).subscribe((rs)=>{
      if(rs){
        abp.notify.success("Create successful");
        this.isLoading = false;
      }
      this.dialogRef.close(true);
    },()=> this.isLoading = false);
  }

  public getAllCurrency(){
    this._currencyService.getAllCurrencyForDropdown().subscribe((rs)=>{
      this.listCurrencies = rs.result;
    })
  }

  public chosenYearHandler(normalizedYear: Moment, dp: any) {
    this.date = new Date(normalizedYear.format());
    dp.close();
  }


}
export class CurrencyDto {
  id: number;
  name: string;
  code: string;
}

