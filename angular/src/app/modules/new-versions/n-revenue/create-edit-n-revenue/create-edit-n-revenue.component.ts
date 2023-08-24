import { CommandDialog } from './../../../../../shared/AppEnums';
import { Component, Inject, Injector, OnInit } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { CurrencyService } from '@app/service/api/currency.service';
import { NRevenueService } from '@app/service/api/new-versions/n-revenue.service';
import { ValueAndNameModel } from '@app/service/model/common-DTO';
import { InvoiceCreateEditDto, NRevenueByAccount } from '@app/service/model/n-revenue.model';
import { AppComponentBase } from '@shared/app-component-base';
import { ListNRevenueComponent } from '../list-n-revenue/list-n-revenue.component';
import { CommonService } from './../../../../service/api/new-versions/common.service';
import * as moment from 'moment';
import { LableDirection } from '@shared/selection-customs/selection-customs.component';
@Component({
  selector: 'app-create-edit-n-revenue',
  templateUrl: './create-edit-n-revenue.component.html',
  styleUrls: ['./create-edit-n-revenue.component.css']
})
export class CreateEditNRevenueComponent extends AppComponentBase implements OnInit {
  public isDisable = false;
  invoice = new InvoiceCreateEditDto();
  customerList = []
  selectedAccountType = {} as any;
  tempAccountTypeList: any[] = [];
  searchAccount: string = "";
  revenueManagedId: number = 0;
  listMonthAndYear: ValueAndNameModel[] = [];
  listCurrency: ValueAndNameModel[] = [];
  viewMode: CommandDialog;
  private currentYear = new Date().getFullYear();
  private currentMonth = new Date().getMonth();
  title: string;
  public LableDirectionLeft = LableDirection.Left;
  constructor(injector: Injector, public dialogRef: MatDialogRef<ListNRevenueComponent>,
    @Inject(MAT_DIALOG_DATA) public data: InvoiceDialogData,
    private _revenue: NRevenueService,
    private _currencyService: CurrencyService,
    private _common: CommonService,
  ) {
    super(injector);
    this.invoice = this.data.item;
    this.viewMode = this.data.command;
  }

  ngOnInit(): void {
    this.setTitleDialog();
    this.getAllForDropdown();
    this.getListMonthAndYear();
  }

  private setTitleDialog(): void{
    if(this.viewMode == CommandDialog.EDIT){
      this.title = `Update ${this.invoice.nameInvoice} of <strong>${this.invoice.accountName}</strong>`;
      this.invoice.monthAndYear = `${this.invoice.month}_${this.invoice.year}`;
    }
    else if(this.viewMode == CommandDialog.CREATE){
      this.title = `Thêm mới khoản phải thu of <strong>${this.invoice.accountName}</strong>`;
      this.invoice.monthAndYear = `${this.currentMonth}_${this.currentYear}`;
    }
    else{
      throw("Not implement view mode");
    }
  }

  getListMonthAndYear() {
    for (let year = this.currentYear - 4; year < this.currentYear + 2; year++) {
      for (let month = 1; month <= 12; month++) {
        this.listMonthAndYear.push({
          name: `${month}/${year}`,
          value: `${month}_${year}`
        })
      }
    }
  }
  getAllForDropdown() {
    this._common.getAllCurrency().subscribe((res) => {
      if(!res.success) return;
      this.listCurrency = res.result;
    })
  }

  saveAndClose() {
    this.invoice.deadline = moment(this.invoice.deadline).format("YYYY-MM-DD");
    this.isDisable = true
    let monthAndYear = this.invoice.monthAndYear.split('_');
    this.invoice.month = parseInt(monthAndYear[0]);
    this.invoice.year = parseInt(monthAndYear[1]);
    if (this.viewMode == CommandDialog.CREATE) {
      this._revenue.createInvoice(this.invoice).subscribe(rs => {
        if (rs.success) {
          abp.notify.success("Created invoice successfully");
          this.invoice = rs.result;
          this.renderInvoice();
          this.dialogRef.close(this.invoice);
        }
      }, () => { this.isDisable = false })
    }
    else {
      this._revenue.updateInvoice(this.invoice).subscribe(rs => {
        if (rs.success) {
          abp.notify.success("Updated invoice successfully");
          this.invoice = rs.result;
          this.renderInvoice();
          this.dialogRef.close(this.invoice);
        }
      }, () => { this.isDisable = false })
    }
  }

  private renderInvoice() {
    let currencyName = this.listCurrency.find(x => x.value == this.invoice.currencyId).name;
    this.invoice = { ...this.invoice, currencyName: currencyName };
  }
}


export interface InvoiceDialogData{
  item: InvoiceCreateEditDto;
  command: CommandDialog;
}
