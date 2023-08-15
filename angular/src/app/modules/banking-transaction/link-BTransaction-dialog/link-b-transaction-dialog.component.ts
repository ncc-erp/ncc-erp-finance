import { Component, Inject, Injector, OnInit } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { CommonService } from '../../../service/api/new-versions/common.service';
import { AppComponentBase } from '@shared/app-component-base';
import { BtransactionService } from '@app/service/api/new-versions/btransaction.service';
import { Time } from '@angular/common';
import * as moment from 'moment';
import * as _ from 'lodash';

@Component({
  selector: 'app-link-b-transaction-dialog',
  templateUrl: './link-b-transaction-dialog.component.html',
  styleUrls: ['./link-b-transaction-dialog.component.css']
})
export class LinkBTransactionDialogComponent extends AppComponentBase implements OnInit {
  public bTransactionOptions: BTransactionDto[] = [];
  public filteredBTransactionBackData: BTransactionDto[] = [];
  public isDisable = true;
  public searchBTransaction: string = "";
  public differentBetweenBankTransAndBTrans: DifferentBetweenBankTransAndBTrans;
  public errorMess: string = ""
  public linkBankTransactionToBTransactionDto: LinkBankTransactionToBTransactionDto = new LinkBankTransactionToBTransactionDto();
  constructor(
    injector: Injector,
    public dialogRef: MatDialogRef<LinkBTransactionDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: LinkBTransactionDialogData,
    private _common: CommonService,
    private _btransaction: BtransactionService,
  ) {
    super(injector);
  }

  ngOnInit(): void {
    this.setBTransactionOptions()
    this.linkBankTransactionToBTransactionDto.bankTransactionId = this.data.bankTransactionId;
  }
  setBTransactionOptions() {
    this._common.getBTransactionOptions().subscribe(response => {
      if (!response.success) return;
      this.filteredBTransactionBackData = this.bTransactionOptions = response.result.map(item => {
        item.searchText = "#" + item.id.toString()
          + " " + item.name
          + " " + item.moneyDisplay
          + " " + item.currencyName
          + " " + moment(item.timeAt).format("YYYY/MM/DD HH:mm:ss");

        item.timeAtDisplay = moment(item.timeAt).format("YYYY/MM/DD HH:mm:ss")
        return item;
      });
    })
  }
  filterBTransactionOptions() {
    this.bTransactionOptions = this.filteredBTransactionBackData.filter(s => s.searchText.trim().toLowerCase().includes(this.searchBTransaction.trim().toLowerCase()))
  }
  bTransSelectOpenedChange(isOpen: boolean){
    if(this.bTransactionOptions.length) return;
      this.searchBTransaction = ""
      this.bTransactionOptions = _.cloneDeep(this.filteredBTransactionBackData);
  }
  onBTransactionChange(event) {
    this.linkBankTransactionToBTransactionDto.bTransactionId = event.value;
    this.isDisable = false;
    this.errorMess = '';
    this.differentBetweenBankTransAndBTrans = new DifferentBetweenBankTransAndBTrans();
    this._btransaction.checkDifferentBetweenBankTransAndBTrans(this.linkBankTransactionToBTransactionDto)
      .subscribe(response => {
        this.differentBetweenBankTransAndBTrans = response.result;
        this.linkBankTransactionToBTransactionDto.exchangeRate = this.differentBetweenBankTransAndBTrans.exchangeRate
      }, (err) => {
        this.errorMess = err.error.error.message
      });
  }

  confirmLinkBankTransactionToBTransaction() {
    abp.message.confirm(
      `Xác nhận <br/>
      Link GDNH(#${this.linkBankTransactionToBTransactionDto.bankTransactionId}) với BDSD(#${this.linkBankTransactionToBTransactionDto.bTransactionId})
      `,
      " ",
      (result: boolean) => {
        if (result) {
          this.linkBankTransactionToBTransaction();
        }
      },
      { isHTML: true });
  }
  linkBankTransactionToBTransaction() {
    this._btransaction.linkBankTransactionToBTransaction(this.linkBankTransactionToBTransactionDto)
      .subscribe(response => {
        if (!response.success) return;
        abp.notify.success('Updated successfully');
        this.dialogRef.close();
      }, () => { });

  }
  isShowTableCheck() {
    return this.differentBetweenBankTransAndBTrans && this.differentBetweenBankTransAndBTrans.isDifferentValue
  }
  isShowColMoneyInTableCheck() {
    return this.differentBetweenBankTransAndBTrans.bankTransactionValueNumber && this.differentBetweenBankTransAndBTrans.bTransactionValueNumber;
  }
  isShowColTimeInTableCheck() {
    return this.differentBetweenBankTransAndBTrans.bankTransactionTimeAt && this.differentBetweenBankTransAndBTrans.bTransactionTimeAt;
  }

}
export interface LinkBTransactionDialogData {
  bankTransactionId: number;
  title: string;
}
export class DifferentBetweenBankTransAndBTrans {
  bankTransactionValue: string;
  bTransactionValue: string;
  bankTransactionTimeAt: Time;
  bTransactionTimeAt: Time;
  fromCurrencyName: string;
  toCurrencyName: string;
  exchangeRate: number;
  isDifferentValue: boolean;
  isDifferentCurrency: boolean;
  bTransactionValueNumber: number;
  bankTransactionValueNumber: number;
}
export class LinkBankTransactionToBTransactionDto {
  bankTransactionId: number;
  bTransactionId: number;
  exchangeRate: number;
}
export class BTransactionDto {
  id: number;
  name: string;
  mone: number;
  currencyName: string;
  currencyColor: string;
  moneyDisplay: string;
  timeAt: Time;
  searchText: string;
  timeAtDisplay: string;
}



