import { InvoiceStatus } from './../../../../shared/AppEnums';
import { Injectable } from '@angular/core';
import { BTransactionStatusColor, RevenueManagedStatus } from '../../../../shared/AppEnums';
import { AccountantAccountService } from '../accountant-account.service';
import { ApiResponse } from '@app/service/model/api-response.model';

@Injectable({
  providedIn: 'root'
})
export class UtilitiesService {

  public catAccounts = [];
  constructor(private _accountantAccountService: AccountantAccountService) { }

  getBgTagBTransactionStatusColor(name: string) {
    return (
      BTransactionStatusColor[name?.split(" ").join("")] ||
      BTransactionStatusColor.DEFAULT
    );
  }
  getBgTagInvoiceStatus(status: number) {
    switch (status) {
      case InvoiceStatus.CHUA_TRA:
        return '#dc3545';
      case InvoiceStatus.TRA_1_PHAN:
        return '#fd7e14';
      case InvoiceStatus.HOAN_THANH:
        return '#28a745';
      case InvoiceStatus.KHONG_TRA:
        return '#6c757d';
      default:
        return '#fd7e14';
    }
  }
  getColorByCurrency(code: string) {
    switch (code) {
      case 'VND':
        return 'black';
      case 'USD':
        return 'blue';
      case 'GBP':
        return 'rgb(246, 117, 168)';
      case 'EURO':
        return 'orange';
      case 'BATH':
        return 'rgb(23, 162, 184)';
      case 'YEN':
        return 'rgb(66, 3, 44)';
      default:
        return '#8758FF';
    }
  }
  formatMoneyCustom(money: number) {
    if(!money) return;
    if (0 < money  && money < 1) return money;
    var moneyString = money.toString();
    if(moneyString.includes(".")){
      return moneyString.split('.')[0].toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",") + "." + moneyString.split('.')[1]
    }
    return moneyString.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",");
  }
  async loadCatalogForRevenue(){
    const promises = [
      this._accountantAccountService.getAll().toPromise()
    ];
    return await Promise.all(promises).then((res: ApiResponse<any>[]) => {
      this.catAccounts = res[0].result;
    });
  }
}
