import { AppComponentBase } from 'shared/app-component-base';
import { Component, Inject, Injector, OnInit } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { BtransactionService } from '@app/service/api/new-versions/btransaction.service';
import { UtilitiesService } from '@app/service/api/new-versions/utilities.service';

@Component({
  selector: 'rollback-client-paid',
  templateUrl: './rollback-client-paid.component.html',
  styleUrls: ['./rollback-client-paid.component.css']
})
export class RollbackClientPaidComponent extends AppComponentBase implements OnInit {
  public model= {} as GetInfoIncomingEntryDto;
  // public bTransactionId: number = 0;
  constructor(
    injector: Injector,
    public dialogRef: MatDialogRef<RollbackClientPaidComponent>,
    @Inject(MAT_DIALOG_DATA) public data: any,
    private _bTransaction: BtransactionService,
    public _utilities: UtilitiesService,
  ) {
    super(injector);
  }

  ngOnInit(): void {
    this.setModel();
  }

  private setModel(): void {
    this._bTransaction.getInfoRollbackBTransactionHasIncomingEntry(this.data)
      .subscribe((response) => {
        if (!response.success) return;;
        this.model = response.result;
      });
  }
  onClose(){
    this.dialogRef.close();
  }

  public rollBackClientPaidConfirm(){
    abp.message.confirm(
      "Bạn có muốn thu hồi ghi nhận thu?",
      '',
      (result: boolean) => {
        if (result) {
          this._bTransaction.rollbackBTransactionHasIncomingEntry(this.data)
          .subscribe((rs)=>{
            if(rs){
              abp.notify.success("Thu hồi ghi nhận thu thành công")
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

  public getUrlLinkRevenue(id: number){
    return "app/detail?id=" + id + "&index=1";
  }

  public getUrlLinkRequestDetail(id: number){
    return "app/requestDetail/main?id=" +id;
  }

  
}

export interface GetInfoIncomingEntryDto{
  bTransactionInfor: BTransactionInforDto;
  bankTransactionInfor: BankTransactionInforDto;
  incomingEntrieInfors: GetInfoRollbackIncomingEntryInforDto[];
  outComingEntryInfors: OutComingEntryInforDto[];
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
  incomingEntryId: number;
  incomingEntryName: string;
  money: number;
  moneyFormat: string;
  currencyId: number;
  currencyName: string;
  exchangeRate: number;
  bankTransactionId: number;
  invoiceId: number;
  invoiceName: string;
  invoiceDateMonth: number;
  invoiceDateYear: number;
  invoiceCurrencyName: string;
  invoiceStatus: number;
  invoiceStatusName: string;
  accountId: number;
  accountName: string;
}

export interface OutComingEntryInforDto {
  outcomingEntryId: number;
  outcomingEntryName: string;
  branchId: number;
  branchName: string;
  value: number;
  valueFormat: string;
  outcomingEntryTypeId: number;
  outcomingEntryTypeCode: string;
  outcomingEntryTypeName: string;
  currencyId: number;
  currencyName: string;
  workflowStatusId: number;
  workflowStatusName: string;
  workflowStatusCode: string;
  createdAt: string;
}
