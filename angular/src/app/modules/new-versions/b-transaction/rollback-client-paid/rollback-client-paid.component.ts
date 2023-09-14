import { AppComponentBase } from 'shared/app-component-base';
import { Component, Inject, Injector, OnInit } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { BtransactionService } from '@app/service/api/new-versions/btransaction.service';

@Component({
  selector: 'rollback-client-paid',
  templateUrl: './rollback-client-paid.component.html',
  styleUrls: ['./rollback-client-paid.component.css']
})
export class RollbackClientPaidComponent extends AppComponentBase implements OnInit {
  public model= {} as GetInfoRollBackClientPaidDto;
  // public bTransactionId: number = 0;
  constructor(
    injector: Injector,
    public dialogRef: MatDialogRef<RollbackClientPaidComponent>,
    @Inject(MAT_DIALOG_DATA) public data: any,
    private _bTransaction: BtransactionService
  ) {
    super(injector);
  }

  ngOnInit(): void {
    this.setModel();
  }

  private setModel(): void {
    this._bTransaction.GetInfoRollbackClientPaid(this.data)
      .subscribe((response) => {
        if (!response.success) return;
        console.log(this.model);
        this.model = response.result;
        console.log(this.model);
      });
  }
  onClose(){
    this.dialogRef.close();
  }

  public rollBackClientPaidConfirm(){
    abp.message.confirm(
      "Bạn có muốn thu hồi khách hàng thanh toán?",
      '',
      (result: boolean) => {
        if (result) {
          this._bTransaction.rollbackLinkOutcomingEntry(this.data.id)
          .subscribe((rs)=>{
            if(rs){
              abp.notify.success("Thu hồi link request chi thành công")
              this.dialogRef.close(true);
            }
          })
        }
      }
    );
    
  }

  public getUrlLinkTransaction(id: number){
    return "app/detail?id=" + id;
  }
}

export interface GetInfoRollBackClientPaidDto{
  bTransactionInfo: BTransactionInforDto;
  bankTransactionInfo: BankTransactionInforDto;
  rollbackIncomingEntryInfos: GetInfoRollbackIncomingEntryInforDto[];
}

export interface BankTransactionInforDto{
  bankTransactionId: number;
  bankTransactionName: string,
  fromBankAccountId: number,
  fromBankAccountName: string,
  fromValue: number,
  fromValueFormat: string,
  fromCurrencyName: string,
  toBankAccountId: number,
  toBankAccountName: string,
  toValue: number,
  toValueFormat: number,
  toCurrencyName: string
}
export interface BTransactionInforDto{
  bTransactionId: number,
  bankAccountName: string,
  bankNumber: string,
  note: string,
  timeAt: string,
  money: number,
  moneyFormat: string,
  currencyName: string
}
export interface GetInfoRollbackIncomingEntryInforDto {
  IncomingEntryId: number;
  IncomingEntryName: string;
  Money: number;
  MoneyFormat: string;
  CurrencyId: number;
  CurrencyName: string;
  ExchangeRate: number;
  BankTransactionId: number;
  InvoiceId: number;
  InvoiceName: string;
  InvoiceDateMonth: number;
  InvoiceDateYear: number;
  InvoiceCurrencyName: string;
  InvoiceStatus: number;
  InvoiceStatusName: string;
}
