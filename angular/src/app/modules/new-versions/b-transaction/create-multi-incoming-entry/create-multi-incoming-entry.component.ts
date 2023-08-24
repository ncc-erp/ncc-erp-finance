import { Component, ElementRef, Inject, Injector, OnInit, ViewChild } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { BtransactionService } from '@app/service/api/new-versions/btransaction.service';
import { CommonService } from '@app/service/api/new-versions/common.service';
import { UtilitiesService } from '@app/service/api/new-versions/utilities.service';
import { FilterBankAccount } from '@app/service/model/bank-account.dto';
import { ValueAndNameModel } from '@app/service/model/common-DTO';
import { AppComponentBase } from '@shared/app-component-base';
import { IncomingEntryTypeOptions, RevenueRecognitionDialogData } from '../link-revenue-ecognition-dialog/link-revenue-ecognition-dialog.component';

@Component({
  selector: 'app-create-multi-incoming-entry',
  templateUrl: './create-multi-incoming-entry.component.html',
  styleUrls: ['./create-multi-incoming-entry.component.css']
})
export class CreateMultiIncomingEntryComponent extends AppComponentBase implements OnInit {

  public incomingEntryTypeOptions: IncomingEntryTypeOptions;
  public createMultiIncomingEntryDto: CreateMultiIncomingEntryDto = new CreateMultiIncomingEntryDto();
  public searchBankAccount: "";
  public bankAccountOptions: ValueAndNameModel[] = [];
  public bankAccountFilters: ValueAndNameModel[] = [];
  @ViewChild('searchBankAccountInput') searchBankAccountInput: ElementRef;
  constructor(private _common: CommonService,
    public dialogRef: MatDialogRef<CreateMultiIncomingEntryComponent>,
    @Inject(MAT_DIALOG_DATA) public data: RevenueRecognitionDialogData,
    private _btransaction: BtransactionService,
    public _utilities: UtilitiesService,
    injector: Injector) {
    super(injector);
  }

  ngOnInit(): void {
    this.createMultiIncomingEntryDto.bTransactionId = this.data.bTransactionId;
    this.setBankAccountOptions();
    this.getTreeIncomingEntries();
    // Defaul 2 ghi nhận thu
    this.addMore()
    this.addMore()
  }
  getTreeIncomingEntries(){
    this._common.getTreeIncomingEntries().subscribe(response => {
      if (!response.success) return;
      this.incomingEntryTypeOptions = {item: {id: 0, name : "", parentId : null}, children: [...response.result]};
    })
  }
  totalMoney(){
    return this.createMultiIncomingEntryDto.incomingEntries.reduce((a, b) => {
      if(b.value)
        return a + Number(b.value);
      return a;
    }, 0);
  }
  addMore(){
    this.createMultiIncomingEntryDto.incomingEntries.unshift(new IncomingEntryInfoDto());
  }
  remove(index: number){
    const value = this.createMultiIncomingEntryDto.incomingEntries[index];
    this.createMultiIncomingEntryDto.incomingEntries = this.createMultiIncomingEntryDto.incomingEntries.filter(s => s != value);
  }
  isDisable(){
    if(!this.createMultiIncomingEntryDto.fromBankAccountId) return true;
    if(!this.createMultiIncomingEntryDto.incomingEntries) return true;
    if(this.createMultiIncomingEntryDto.incomingEntries.some(s => !s.value)) return true;
    if(this.createMultiIncomingEntryDto.incomingEntries.some(s => !s.name)) return true;
    if(this.createMultiIncomingEntryDto.incomingEntries.some(s => !s.incomingEntryTypeId)) return true;
    return false;
  }
  check(){
    this._btransaction.checkCreateMultiIncomingEntry(this.createMultiIncomingEntryDto).subscribe(response => {
      if (!response.success) {
        return;
      };
      abp.message.confirm(
        "",
        "",
        (result: boolean) => {
          if (result) {
            this.create();
          }
        }
      );
    })
  }
  create(){
    this._btransaction.createMultiIncomingEntry(this.createMultiIncomingEntryDto)
    .subscribe(response => {
      if (!response.success) return;
      abp.message.confirm(
        `Tạo thành công các ghi nhận thu có id ${response.result.incomingEntryIds.join(", ")}
        <br/>thuộc giao dịch ngân hàng: ${response.result.bankTransactionId}
        <br/> Bạn có muốn xem chi tiết?`,
        "Done",
        (result: boolean) => {
          if (result) {
            this.dialogRef.close(response.result.bankTransactionId);
            return;
          }
          this.dialogRef.close();
        },
        { isHTML: true}
      );
    })
  }
  bankAccountSelectOpenedChange(opened: boolean){
    if(opened){
      this.searchBankAccountInput.nativeElement.focus();
      if(this.searchBankAccount){
        this.filterBankAccount();
      }
    }else{
      this.bankAccountFilters = this.bankAccountOptions;
    }
  }
  setBankAccountOptions(): void {
    this._common.getBankAccountOptions({currencyId: this.data.currencyId, isActive: true} as FilterBankAccount).subscribe(response => {
      if (!response.success) return;
      this.bankAccountOptions = this.bankAccountFilters = response.result;
    })
  }
  filterBankAccount(): void {
    this.bankAccountFilters = this.bankAccountOptions.filter(item => item.name.trim().toLowerCase().includes(this.searchBankAccount.trim().toLowerCase()))
  }
  // Chỉ khi có 2 Incoming
  setValueIncoming(index = 0){
    if(this.createMultiIncomingEntryDto.incomingEntries.length != 2) return;
    if(this.createMultiIncomingEntryDto.incomingEntries[index].value > this.data.moneyNumber){
      this.createMultiIncomingEntryDto.incomingEntries[1 - index].value = undefined;
      return;
    }
    if(this.createMultiIncomingEntryDto.incomingEntries[index].value == 0){
      this.createMultiIncomingEntryDto.incomingEntries[1 - index].value = undefined;
      return;
    }
    this.createMultiIncomingEntryDto.incomingEntries[1 - index].value = this.data.moneyNumber - this.createMultiIncomingEntryDto.incomingEntries[index].value;
  }

}

export class CreateMultiIncomingEntryDto{
  fromBankAccountId: number;
  incomingEntries: IncomingEntryInfoDto[] = [];
  bTransactionId: number;
}
export class IncomingEntryInfoDto{
  name: string;
  incomingEntryTypeId: number;
  value: number;
}
export class  ResultCreateMultiIncoming{
  bankTransactionId: number;
  incomingEntryIds: number[];
}
