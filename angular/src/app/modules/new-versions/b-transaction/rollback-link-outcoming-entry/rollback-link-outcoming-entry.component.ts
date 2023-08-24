import { AppComponentBase } from 'shared/app-component-base';
import { Component, Inject, Injector, OnInit } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { BtransactionService } from '@app/service/api/new-versions/btransaction.service';

@Component({
  selector: 'app-rollback-link-outcoming-entry',
  templateUrl: './rollback-link-outcoming-entry.component.html',
  styleUrls: ['./rollback-link-outcoming-entry.component.css']
})
export class RollbackLinkOutcomingEntryComponent extends AppComponentBase implements OnInit {
  public model= {} as GetInfoRollbackDto;
  // public bTransactionId: number = 0;
  constructor(
    injector: Injector,
    public dialogRef: MatDialogRef<RollbackLinkOutcomingEntryComponent>,
    @Inject(MAT_DIALOG_DATA) public bTransactionId: number,
    private _bTransaction: BtransactionService
  ) {
    super(injector);
  }

  ngOnInit(): void {
    this.setModel();
  }

  private setModel(): void {
    this._bTransaction.getInfoRollbankOutcomingEntryWithBTransaction(this.bTransactionId)
      .subscribe((response) => {
        if (!response.success) return;
        this.model = response.result;
      });
  }
  onClose(){
    this.dialogRef.close();
  }

  public rollbackLinkOutcomingEntry(){
    abp.message.confirm(
      "Bạn có muốn thu hồi link request chi?",
      '',
      (result: boolean) => {
        if (result) {
          this._bTransaction.rollbackLinkOutcomingEntry(this.bTransactionId)
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

export interface GetInfoRollbackDto{
  bTransactionInfo: BTransactionInfoDto;
  bankTransactionInfo: BankTransactionInfoDto;
  rollbackOutcomingEntryInfos: GetInfoRollbackOutcomingEntryDto[];

}

export interface GetInfoRollbackOutcomingEntryDto{
  name: string;
  value: number;
  valueFormat: string;
  workFlowStatus: string;
  outCommingEntryTypeName: string;
  workflowStatusCode: string;
}


export interface BankTransactionInfoDto{
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
export interface BTransactionInfoDto{
  bTransactionId: number,
  bankAccountName: string,
  bankNumber: string,
  note: string,
  timeAt: string,
  money: number,
  moneyFormat: string,
  currencyName: string
}
export interface GetInfoRollbackOutcomingEntry {
  bTransactionId: number;
  note: string
  timeAt: Date
  moneyBTransaction: number;
  moneyBTransactionFormat: string;
  bTransactionCurrencyName: string;
  bankTransactionName: string;
  fromBankAccountId: number;
  fomBankAccountName: string;
  fromValue: number;
  fromValueFormat: string;
  fromCurrencyName: string;
  toBankAccountId: number;
  toBankAccountName: string;
  toValue: number;
  toValueFormat: string;
  toCurrencyName: string;
  rollbackOutcomingEntryInfos: RollbackOutcomingEntryDetail[];
}
export interface RollbackOutcomingEntryDetail{
  name: string;
  value: number;
  valueFormat: string;
  workflowStatus: string;
  outcomingEntryTypeName: string;
}
