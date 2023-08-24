import { NGX_MAT_DATE_FORMATS } from '@angular-material-components/datetime-picker';
import { Component, Inject, Injector, LOCALE_ID, OnInit, ViewChild } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { BtransactionService } from '@app/service/api/new-versions/btransaction.service';
import { ValueAndNameModel } from '@app/service/model/common-DTO';
import { AppComponentBase } from '@shared/app-component-base';
import { CommandDialog } from '@shared/AppEnums';
import * as moment from 'moment';
import { CreateEditBTransactionDto, ListBtransactionComponent } from '../list-btransaction/list-btransaction.component';
import { CommonService } from './../../../../service/api/new-versions/common.service';
import * as _ from 'lodash';
import { LableDirection } from '@shared/selection-customs/selection-customs.component';
import { DatePipe } from '@angular/common';
import { FormatMoneyPipe } from '@shared/pipes/format-money.pipe';

@Component({
  selector: 'app-create-edit-b-transaction',
  templateUrl: './create-edit-b-transaction.component.html',
  styleUrls: ['./create-edit-b-transaction.component.css'],
  providers: [DatePipe]

})
export class CreateEditBTransactionComponent extends AppComponentBase implements OnInit {
  public isDisable = false;
  btransaction = {} as CreateEditBTransactionDto;
  public filteredListBankAccount: ValueAndNameModel[] = [];
  listBankAccount: ValueAndNameModel[] = [];
  listCurrency: any[] = [];
  listBTransactionStatus: ValueAndNameModel[] = [];
  title: string;
  seachBankAccount : string;
  viewMode: CommandDialog;

  public showSpinners = true;
  public touchUi = false;
  public enableMeridian = false;
  public stepHour = 1;
  public stepMinute = 1;
  public stepSecond = 1;
  public minDate: moment.Moment;
  public maxDate: moment.Moment;
  public LableDirectionLeft = LableDirection.Left;
  private formatMoneyPipe: FormatMoneyPipe

  constructor(injector: Injector, public dialogRef: MatDialogRef<ListBtransactionComponent>,
    private _common: CommonService,
    private datePipe:DatePipe,

    @Inject(MAT_DIALOG_DATA)
    public data: any,

    private _btransaction: BtransactionService,
  ) {
    super(injector);
    this.viewMode = this.data.command;
    this.formatMoneyPipe = new FormatMoneyPipe()
  }

  ngOnInit(): void {
    this.initPopUp();
    this.getListBankAccount();
  }
  initPopUp(){
    if (this.viewMode == CommandDialog.CREATE) {
      this.title = 'Thêm mới biến động số dư';
    }
    else {
      this.title = 'Sửa biến động số dư';
      this.btransaction = this.data.item;
    }
  }

  getListBankAccount() {
    this._common
      .gettAllBankAccount()
      .subscribe((res) => {
        this.listBankAccount = res.result;
        this.filteredListBankAccount = this.listBankAccount;
      });
  }

  filterBankAccount(searchString: string) {
    let value = searchString.trim().toLowerCase();
    this.filteredListBankAccount = this.listBankAccount.filter((bankAccount) =>
      bankAccount.name.trim().toLowerCase().includes(value)
    );
  }

  saveAndClose() {
    this.btransaction.timeAt = moment(this.btransaction.timeAt).format("YYYY/MM/DD HH:mm:ss")
    this.isDisable = true;
    if (this.viewMode== CommandDialog.CREATE) {
      this._btransaction.createTransaction(this.btransaction).subscribe(rs => {
        if (rs.success) {
          abp.notify.success("Created transaction successfully");
          this.btransaction = rs.result;
          this.dialogRef.close(true);
        }
      }, () => { this.isDisable = false })
    }
    else {
      this._btransaction.updateTransaction(this.btransaction).subscribe(rs => {
        if (rs.success) {
          abp.notify.success("Update transaction successfully");
          this.btransaction = rs.result;
          this.dialogRef.close(true);
        }
      }, () => { this.isDisable = false })
    }
  }
  bankAccountSelectOpenedChange(){
    if(this.filteredListBankAccount.length) return;
    this.seachBankAccount = "";
    this.filteredListBankAccount = _.cloneDeep(this.listBankAccount);
  }
  generateNote(){

    let bankAccountName = this.btransaction.bankAccountId? this.filteredListBankAccount.find(s => s.value == this.btransaction.bankAccountId)?.name : ""
    let money = this.btransaction.money ? "số tiền: " + this.formatMoneyPipe.transform(Number(this.btransaction.money)) : ""
    let date = this.btransaction.timeAt ? "ngày " + this.datePipe.transform(this.btransaction.timeAt, "dd/MM/yyyy HH:mm") : ""

    this.btransaction.note = `${bankAccountName} ${money} ${date}`
  }
}
