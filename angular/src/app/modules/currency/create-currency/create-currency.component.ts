import { PERMISSIONS_CONSTANT } from '@app/constant/permission.constant';
import { Component, OnInit, Output,Injector,EventEmitter } from '@angular/core';
import { CurrencyConvertDto } from '../currency.component';
import { AppComponentBase } from '@shared/app-component-base';
import { CurrencyService } from '@app/service/api/currency.service';
import { BsModalRef } from 'ngx-bootstrap/modal';

@Component({
  selector: 'app-create-currency',
  templateUrl: './create-currency.component.html',
})
export class CreateCurrencyComponent extends AppComponentBase implements OnInit {

  saving = false;
  currency: CurrencyConvertDto = new CurrencyConvertDto();
  name: string = "";
  id: number;
  @Output() onSave = new EventEmitter<any>();
  Directory_Currency_ChangeDefaultCurrency = PERMISSIONS_CONSTANT.Directory_Currency_ChangeDefaultCurrency;

  constructor(
    injector: Injector,
    private _currencyService: CurrencyService,
    public bsModalRef: BsModalRef
  ) {
    super(injector);
  }

  ngOnInit(): void {
    this.currency.maxITF = 1;
  }

  save(): void {
    this.saving = true;
    this._currencyService
      .create(this.currency)
      .subscribe(() => {
        this.notify.info(this.l('Saved Successfully'));
        this.bsModalRef.hide();
        this.onSave.emit();
      },()=>this.saving=false);
  }
  handleChangeName() {
    const code = this.getCodeByName(this.name);
    if (this.currency.code == code || !this.currency.code) {
      this.currency.code = this.getCodeByName(this.currency.name);
    }
    this.name = this.currency.name;
  }
  getCodeByName(name: string) {
    return this.removeVietnameseTones(name.toLocaleLowerCase().trim()).replace(
      / /g,
      "_"
    );
  }
}
