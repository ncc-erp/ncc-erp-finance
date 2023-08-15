import { Component, Inject, Injector, OnInit } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { AccountDto } from '@app/modules/accountant-account/accountant-account.component';
import { AccountantAccountService } from '@app/service/api/accountant-account.service';
import { CurrencyService } from '@app/service/api/currency.service';
import { NRevenueService } from '@app/service/api/new-versions/n-revenue.service';
import { InvoiceCreateEditDto } from '@app/service/model/n-revenue.model';
import { AppComponentBase } from '@shared/app-component-base';
import * as moment from 'moment';
import * as _ from 'lodash';
import { ListNRevenueComponent } from '../list-n-revenue/list-n-revenue.component';
import { LableDirection } from '@shared/selection-customs/selection-customs.component';
import { CommonService } from '@app/service/api/new-versions/common.service';
import { ValueAndNameModel } from '@app/service/model/common-DTO';
@Component({
  selector: 'app-create-n-revenue-by-account',
  templateUrl: './create-n-revenue-by-account.component.html',
  styleUrls: ['./create-n-revenue-by-account.component.css']
})
export class CreateNRevenueByAccountComponent extends AppComponentBase implements OnInit {
  public isDisable = false;
  revenue = new InvoiceCreateEditDto();
  customerList = []
  selectedAccountType = {} as any
  tempAccountTypeList: any[] = []
  searchAccount: string = ""
  revenueManagedId: number = 0;
  public filteredListAccount: ValueAndNameModel[];
  listCurrency: ValueAndNameModel[] = [];
  seachaccount: string;
  public LableDirectionLeft = LableDirection.Left;

  listMonthAndYear: any[] = [];

  private currentYear = new Date().getFullYear()
  private currentMonth = new Date().getMonth();

  listRemindStatus = [
    { value: 1, text: "Nhắc lần 1" },
    { value: 2, text: "Nhắc lần 2" },
    { value: 3, text: "Nhắc lần 3" },
  ]

  title: string;

  constructor(injector: Injector, public dialogRef: MatDialogRef<ListNRevenueComponent>,
    @Inject(MAT_DIALOG_DATA)
    public data: any,
    private _revenue: NRevenueService,
    private _currencyService: CurrencyService,
    private _accountantAccountService: AccountantAccountService,
    private comom: CommonService
  ) {
    super(injector);
  }

  ngOnInit(): void {
    this.title = 'Thêm mới khoản phải thu';
    this.getAllForDropdown();
    this.getListAccount();
    this.getListMonthAndYear();
    this.revenue.monthAndYear = `${this.currentMonth}_${this.currentYear}`;
    this.revenue.ntf = 0;
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
    this.comom.getAllCurrency().subscribe((res) => {
      if(!res.success) return;
      this.listCurrency = res.result;
    })
  }

  getListAccount() {
    this.comom.getAllClient().subscribe(response => {
      this.filteredListAccount = response.result;
    })
  }
  saveAndClose() {
    this.revenue.deadline = moment(this.revenue.deadline).format("YYYY-MM-DD");
    this.isDisable = true;
    let monthAndYear = this.revenue.monthAndYear.split('_');
    this.revenue.month = parseInt(monthAndYear[0]);
    this.revenue.year = parseInt(monthAndYear[1]);
    this._revenue.createInvoice(this.revenue).subscribe(rs => {
      if (rs.success) {
        abp.notify.success("Created revenue successfully");
        this.revenue = rs.result;
        this.dialogRef.close(this.revenue);
      }
    }, () => { this.isDisable = false })
  }
}
