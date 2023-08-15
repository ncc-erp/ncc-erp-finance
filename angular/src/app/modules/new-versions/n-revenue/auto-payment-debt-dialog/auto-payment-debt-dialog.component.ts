import { Component, Inject, Injector, OnInit } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { NRevenueService } from '@app/service/api/new-versions/n-revenue.service';
import { UtilitiesService } from '@app/service/api/new-versions/utilities.service';
import { AutoPaidDto, CheckAutoPaidDto, CurrencyNeedConvertDto, MoneyInfo } from '@app/service/model/n-revenue.model';
import { AppComponentBase } from '@shared/app-component-base';
import { ListNRevenueComponent } from '../list-n-revenue/list-n-revenue.component';

@Component({
  selector: 'app-auto-payment-debt-dialog',
  templateUrl: './auto-payment-debt-dialog.component.html',
  styleUrls: ['./auto-payment-debt-dialog.component.css']
})
export class AutoPaymentDebtDialogComponent extends AppComponentBase implements OnInit {
  currencyNeedConverts: CurrencyNeedConvertDto[] = [];
  moneyInfo: MoneyInfo[] = [];
  accountId: number;
  accountName: string;
  public isDisable = false;
  constructor(
    injector: Injector,
    public dialogRef: MatDialogRef<ListNRevenueComponent>,
    @Inject(MAT_DIALOG_DATA) public data: CheckAutoPaidDto,
    private _revenue: NRevenueService,
    public _utilities: UtilitiesService,
  ) {
    super(injector);
    this.currencyNeedConverts = data.currencyNeedConverts;
    this.moneyInfo = data.moneyInfos;
    this.accountId = data.accountId;
    this.accountName = data.accountName;
  }

  ngOnInit(): void {

  }

  process() {
    this.isDisable = true
    abp.message.confirm(
      `Khách hàng <strong>${this.accountName}</strong> thanh toán`,
      "",
      (result: boolean) => {
        if (result) {
          this.doSave();
        }
        this.isDisable = false
      },
      true
    );
  }

  doSave() {
    let item = {
      accountId: this.accountId,
      currencyNeedConverts:  this.currencyNeedConverts,
    } as AutoPaidDto;
    this._revenue.autoPaidForAccount(item)
      .subscribe(response => {
        if (!response.success) return;
        abp.notify.success('Auto pay debt successfully');
        this.dialogRef.close(response.success);
      }, () => { this.isDisable = false });
  }
  checkValidate(){
    if(this.currencyNeedConverts && this.currencyNeedConverts.find(s => s.exchangeRate == undefined)) return true;
    return false;
  }
}

