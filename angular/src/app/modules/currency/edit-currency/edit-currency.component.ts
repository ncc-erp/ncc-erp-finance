import { PERMISSIONS_CONSTANT } from '@app/constant/permission.constant';
import { Component,
  Injector,
  OnInit,
  Output,
  EventEmitter } from '@angular/core';
import { AppComponentBase } from '@shared/app-component-base';
import { CurrencyConvertDto, CurrencyDto } from '../currency.component';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { CurrencyService } from '@app/service/api/currency.service';
import { CommonService } from '@app/service/api/new-versions/common.service';
import { ValueAndNameModel } from '@app/service/model/common-DTO';

@Component({
  selector: 'app-edit-currency',
  templateUrl: './edit-currency.component.html',
  styleUrls: ['./edit-currency.component.css']
})
export class EditCurrencyComponent extends AppComponentBase implements OnInit {

  saving = false;
  currency: CurrencyDto = new CurrencyDto();
  id: number;
  name: string = "";
  bankAccountIds: ValueAndNameModel[];
  bankAccountIdsAllData: ValueAndNameModel[];
  bankAccountAllData: ValueAndNameModel[];
  bankAccountIds_2: ValueAndNameModel[];
  searchBankAccountName : string;
  searchDefaulftBank : string;
  searchToBankAccountWhenBuy: string;
  searchbankAccountWhenSell:string;
  searchFromBankAccountWhenBuy:string;
  bankAccountsWhenSellByCurrencyVND: ValueAndNameModel[];
  tempBankAccountsWhenSellByCurrencyVND: ValueAndNameModel[];
  toBankAccountsWhenBuyByCurrencyVND: ValueAndNameModel[];
  tempToBankAccountsWhenBuyByCurrencyVND: ValueAndNameModel[];
  fromBankAccountsWhenBuy: ValueAndNameModel[];
  tempFromBankAccountsWhenBuy: ValueAndNameModel[];
  @Output() onSave = new EventEmitter<any>();
  Directory_Currency_ChangeDefaultCurrency = PERMISSIONS_CONSTANT.Directory_Currency_ChangeDefaultCurrency;

  constructor(
    injector: Injector,
    private _currencyService: CurrencyService,
    private _commonService: CommonService,
    public bsModalRef: BsModalRef
  ) {
    super(injector);
  }

  ngOnInit(): void {
    this._currencyService.getById(this.id).subscribe((result: any) => {
      this.currency = result.result;
      this.name = result.result.name;
    });
    this.setBankAccountIds();
    this.getAllBankAccountId();
    this.getBankAccountByCurrencyVND();
  }

  setBankAccountIds()
  {
    this._commonService.getBankAccountByCurrency(this.id).subscribe((result: any) => {
      this.bankAccountIdsAllData = this.bankAccountIds = result.result;
      this.fromBankAccountsWhenBuy = this.tempFromBankAccountsWhenBuy =result.result;
    });
  }
  getAllBankAccountId()
  {
    this._commonService.getAllBankAccount().subscribe((result: any) => {
      this.bankAccountAllData = this.bankAccountIds_2 = result.result;
    });
  }
  filterBankAccount()
  {
    this.bankAccountIds = this.bankAccountIdsAllData.filter(s => s.name.trim().toLowerCase().includes(this.searchBankAccountName.trim().toLowerCase()));
  }
  filterBankAccount2()
  {
    this.bankAccountIds_2 = this.bankAccountAllData.filter(s => s.name.trim().toLowerCase().includes(this.searchDefaulftBank.trim().toLowerCase()));
  }
  filterBankAccountWhenSell()
  {
    this.bankAccountsWhenSellByCurrencyVND = this.tempBankAccountsWhenSellByCurrencyVND.filter(s => s.name.trim().toLowerCase().includes(this.searchbankAccountWhenSell.trim().toLowerCase()));
  }
  filterToBankAccountWhenBuy(){
    this.toBankAccountsWhenBuyByCurrencyVND = this.tempToBankAccountsWhenBuyByCurrencyVND.filter(s => s.name.trim().toLowerCase().includes(this.searchToBankAccountWhenBuy.trim().toLowerCase()));
  }
  filterFromBankAccountWhenBuy(){
    this.fromBankAccountsWhenBuy = this.tempFromBankAccountsWhenBuy.filter(s => s.name.trim().toLowerCase().includes(this.searchFromBankAccountWhenBuy.trim().toLowerCase()));
  }
  defaultBankAccountIdWhenSellOpenChange(isOpen: boolean){
    if(isOpen && this.searchbankAccountWhenSell){
      this.filterBankAccountWhenSell();
    }else{
      this.bankAccountsWhenSellByCurrencyVND = this.tempBankAccountsWhenSellByCurrencyVND;
    }
  }
  defaultFromBankAccountIdWhenBuyOpenChange(isOpen: boolean){
    if(isOpen && this.searchFromBankAccountWhenBuy){
      this.filterFromBankAccountWhenBuy();
    }else{
      this.fromBankAccountsWhenBuy = this.tempFromBankAccountsWhenBuy;
    }
  }
  defaultToBankAccountIdWhenBuyOpenChange(isOpen: boolean){
    if(isOpen && this.searchToBankAccountWhenBuy){
      this.filterToBankAccountWhenBuy();
    }else{
      this.toBankAccountsWhenBuyByCurrencyVND = this.tempToBankAccountsWhenBuyByCurrencyVND;
    }
  }

  save(): void {
    this.saving = true;
    this._currencyService
      .update(this.currency)
      .subscribe(() => {
        this.notify.info(this.l('Update Successful'));
        this.bsModalRef.hide();
        this.onSave.emit();
      },()=>this.saving=false);
  }

  getBankAccountByCurrencyVND(){
    this._commonService.getBankAccountByCurrencyCode("VND").subscribe((rs)=>{
      this.bankAccountsWhenSellByCurrencyVND = this.tempBankAccountsWhenSellByCurrencyVND = rs.result;
      this.toBankAccountsWhenBuyByCurrencyVND = this.tempToBankAccountsWhenBuyByCurrencyVND = rs.result;
    })
  }
  handleChangeName() {
    console.log(this.name)
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
