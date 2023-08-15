import { ClientPayDeviantDialogComponent, DataClientPayDeviantDialog } from './../client-pay-deviant-dialog/client-pay-deviant-dialog.component';
import { AppConsts } from './../../../../../shared/AppConsts';
import { CheckAutoPaidDto, InvoiceCreateEditDto, NInvoice, UpdateNoteDto, UpdateStatusInvoiceDto } from './../../../../service/model/n-revenue.model';
import { NRevenueService } from './../../../../service/api/new-versions/n-revenue.service';
import { CommonService } from './../../../../service/api/new-versions/common.service';
import { ValueAndNameModel } from '@app/service/model/common-DTO';
import { FilterDto, PagedListingComponentBase, PagedRequestDto } from '@shared/paged-listing-component-base';
import { Component, Injector, OnInit, ViewChild } from '@angular/core';
import { NRevenueByAccount } from '../../../../service/model/n-revenue.model';
import { CommandDialog, DateSelectorEnum, InvoiceStatus, RevenueManagedStatusAccount } from '@shared/AppEnums';
import { Utils } from '@app/service/helpers/utils';
import { EComparisor } from '@app/modules/revenue-managed/revenue-managed.component';
import { IFilterDateTimeParam } from '../../../../service/interfaces/filter-date.interface';
import { DateFormat, DateTimeSelector } from '@shared/date-selector/date-selector.component';
import { CreateEditNRevenueComponent, InvoiceDialogData } from '../create-edit-n-revenue/create-edit-n-revenue.component';
import { MatDialog } from '@angular/material/dialog';
import { EditNoteNRevenueComponent } from '../edit-note-n-revenue/edit-note-n-revenue.component';
import { EditStatusNRevenueComponent } from '../edit-status-n-revenue/edit-status-n-revenue.component';
import { UtilitiesService } from '@app/service/api/new-versions/utilities.service';
import { AccountDto } from '@app/modules/accountant-account/accountant-account.component';
import { AccountantAccountService } from '@app/service/api/accountant-account.service';
import { AutoPaymentDebtDialogComponent } from '../auto-payment-debt-dialog/auto-payment-debt-dialog.component';
import { CreateNRevenueByAccountComponent } from '../create-n-revenue-by-account/create-n-revenue-by-account.component';
import * as _ from 'lodash';
import { ActivatedRoute } from '@angular/router';
import { PERMISSIONS_CONSTANT } from '@app/constant/permission.constant';
import * as FileSaver from 'file-saver';

@Component({
  selector: 'app-list-n-revenue',
  templateUrl: './list-n-revenue.component.html',
  styleUrls: ['./list-n-revenue.component.css']
})
export class ListNRevenueComponent extends PagedListingComponentBase<NRevenueByAccount> implements OnInit {

  Finance_BankTransaction_ExportExcel = PERMISSIONS_CONSTANT.Finance_BankTransaction_ExportExcel;
  defaultDateFilterType: DateSelectorEnum = DateSelectorEnum.ALL;
  accounts = [];
  accountOptions: ValueAndNameModel[] = [];
  revenueStatusOptions: ValueAndNameModel[] = [];
  revenueByAccounts: NRevenueByAccount[] = [];
  searchWithDateTime: DateTimeSelector;
  searchDetail = {
    month: AppConsts.VALUE_OPTIONS_ALL,
    year: AppConsts.VALUE_OPTIONS_ALL,
    status: AppConsts.VALUE_OPTIONS_ALL
  };
  isDoneDebt?: boolean = false;

  public filteredListAccount: AccountDto[] = [];
  listRevenueStatuses: ValueAndNameModel[] = [];
  public statistics: StatisticsDto[] = [];
  public isCollapsed = false;
  years: ValueAndNameModel[] = [];
  public isShowPopAutoPayDebt: boolean = false;
  public multiValueStatusFilter: number[] = []
  months: ValueAndNameModel[] = [
    { value: AppConsts.VALUE_OPTIONS_ALL, name: "All" },
    { value: 1, name: "Tháng 1" },
    { value: 2, name: "Tháng 2" },
    { value: 3, name: "Tháng 3" },
    { value: 4, name: "Tháng 4" },
    { value: 5, name: "Tháng 5" },
    { value: 6, name: "Tháng 6" },
    { value: 7, name: "Tháng 7" },
    { value: 8, name: "Tháng 8" },
    { value: 9, name: "Tháng 9" },
    { value: 10, name: "Tháng 10" },
    { value: 11, name: "Tháng 11" },
    { value: 12, name: "Tháng 12" },
  ]

  invoiceStatusAccounts: ValueAndNameModel[] = [
    { value: AppConsts.VALUE_OPTIONS_ALL, name: "All" },
    { value: false, name: "Chưa trả hết" },
    { value: true, name: "Đã trả hết" },
  ];

  private currentYear = new Date().getFullYear();
  public selectedAccountId = 0;//default selected = 0

  public allSelected: boolean = true;
  public isSelectAllAccount: boolean = true;
  public listAccountSelected: number[] = [];
  private reaccountSl: number[] = [];
  private accountIdSelected: number = 0;
  public searchString: string = "";
  constructor(
    injector: Injector,
    private _common: CommonService,
    private _revenue: NRevenueService,
    private dialog: MatDialog,
    public _utilities: UtilitiesService,
    private route: ActivatedRoute
  ) {
    super(injector);
  }

  ngOnInit(): void {
    this.subscriptions.push(
      AppConsts.periodId.asObservable().subscribe((rs) => {
        this.refresh();
      }));
    this.setRevenueOptions();
    this.setInvoiceStatuses();
    this.setYears();
    this.getListAccount();
    this.getInvoiceRoutingFromHomepage();
  }

  setYears() {
    this.years.push({ value: AppConsts.VALUE_OPTIONS_ALL, name: 'All' } as ValueAndNameModel);
    for (let year = this.currentYear - 4; year < this.currentYear + 2; year++) {
      this.years.push({ value: year, name: year.toString() } as ValueAndNameModel)
    }
  }

  toggleAllSelection() {
    if (this.allSelected) {
      this.listAccountSelected = _.cloneDeep(this.accounts.map(item => item.id));
    } else {
      this.listAccountSelected = [];
    }
    this.getFirstPage();
  }

  optionClick() {
    if (!this.isSelectAllAccount) {
      const arr = this.filteredListAccount.filter(s => !this.listAccountSelected.includes(s.id)).map(item => item.id);
      _.remove(this.reaccountSl, (item) => arr.includes(item));
      this.reaccountSl.push(...this.listAccountSelected);
    }
    else {
      this.reaccountSl = this.listAccountSelected;
    }

    this.listAccountSelected = _.cloneDeep(this.reaccountSl);
    this.listAccountSelected = Array.from(new Set(this.listAccountSelected));
    let newStatus = false;
    if (this.listAccountSelected.length === this.accounts.length) {
      newStatus = true;
    }

    this.allSelected = newStatus;
    this.getFirstPage();
  }

  getListAccount() {
    this.accounts = this._utilities.catAccounts;
    this.filteredListAccount = _.cloneDeep(this.accounts);
    this.listAccountSelected = this.filteredListAccount.map((item) => item.id);
    this.reaccountSl = _.cloneDeep(this.listAccountSelected);
  }

  filterAccount(searchString: string) {
    this.isSelectAllAccount = true;
    if (searchString.trim()) {
      this.isSelectAllAccount = false;
    }
    const value = searchString.trim().toLowerCase();
    this.reaccountSl = _.cloneDeep(this.listAccountSelected);
    this.filteredListAccount = this.accounts.filter((account) =>
      account.name.trim().toLowerCase().includes(value)
    );
  }

  handleSelectAccountOpenedChange(opened: boolean) {
    if (!opened) {
      if (!this.filteredListAccount.length) {
        this.filteredListAccount = _.cloneDeep(this.accounts);
      }
      this.searchString = ""
      this.filterAccount(this.searchString)
    }
    else {
    }
  }

  onDateChange(searchDate: DateTimeSelector): void {
    this.searchWithDateTime = searchDate;
    this.getFirstPage();
  }

  getRowspan(item: NInvoice) {
    if (!item.incomings || item.incomings.length === 0) {
      return 1;
    }
    return item.incomings.length + 1;
  }

  private setRevenueOptions(): void {
    this._common.getAllRevenueStatuses()
      .subscribe(response => {
        if (!response.success) return;

        this.revenueStatusOptions = response.result;
      })
  }

  protected list(request: PagedRequestDto, pageNumber: number, finishedCallback: Function): void {
    const payload = this.getRequestParams(request);
    this._revenue.getAllPaging(payload).subscribe(response => {
      if (!response.success) return;

      this.revenueByAccounts = response.result.pagings.items;
      if (this.accountIdSelected != 0) {
        var accountSelected = this.revenueByAccounts.find((item) => item.accountId == this.accountIdSelected);
        if (accountSelected) {
          accountSelected.isExpand = true;
        }
      }
      this.totalItems = response.result.pagings.totalCount;
      this.statistics = response.result.statistics
      this.isTableLoading = false;
    });
  }
  private getRequestParams(request: PagedRequestDto): NRevenuePagedRequestDto {
    const payload = { ...request } as NRevenuePagedRequestDto;
    const filterItems: FilterDto[] = [];

    for (const property in this.searchDetail) {
      if (!Utils.isSelectedOptionsAll(this.searchDetail[property])) {
        const filterObj = {
          propertyName: property,
          value: this.searchDetail[property],
          comparision: EComparisor.EQUAL,
        }
        filterItems.push(filterObj);
      }
    }
    payload.filterItems = filterItems;

    if (this.searchWithDateTime?.dateType !== DateSelectorEnum.ALL) {
      payload.filterDateTimeParam = {
        fromDate: this.searchWithDateTime?.fromDate?.format(DateFormat.YYYY_MM_DD),
        toDate: this.searchWithDateTime?.toDate?.format(DateFormat.YYYY_MM_DD)
      } as IFilterDateTimeParam;
    }

    if (!Utils.isSelectedOptionsAll(this.isDoneDebt)) {
      payload.isDoneDebt = this.isDoneDebt;
    }

    if (this.listAccountSelected.length != 0) {
      payload.accountIds = this.listAccountSelected;
    }

    if(this.multiValueStatusFilter.length != 0){
      payload.statuses = this.multiValueStatusFilter;
    }

    return payload;
  }
  protected delete(entity: NRevenueByAccount): void {
    //not using;
  }
  handleAutoPayDebt(accountId: number, accountName: string) {
    this._revenue.checkAutoPaid(accountId).subscribe(response => {
      if (!response.success) return;
      if (response.result.hasCollectionDebt) {
        this.showAutoPayDebt(accountId, accountName, response.result);
        return;
      }
      this.isShowPopAutoPayDebt = true;
      return;
    });
  }
  private showAutoPayDebt(accountId: number, accountName: string, data: CheckAutoPaidDto): void {
    data.accountId = accountId;
    data.accountName = accountName;
    let dialogRef = this.dialog.open(AutoPaymentDebtDialogComponent, {
      data,
      width: "550px",
      disableClose: true,
    });
    dialogRef.afterClosed().subscribe((rs) => {
      if (rs) {
        this.getFirstPage();
      }
    });
  }

  handleSelectionChange() {
    this.getFirstPage();
  }

  private setInvoiceStatuses(): void {
    this._common.getAllInvoiceStatuses()
      .subscribe(response => {
        if (!response.success) return;

        this.listRevenueStatuses = [
          // { value: AppConsts.VALUE_OPTIONS_ALL, name: "All" },
          ...response.result
        ];
      })
  }

  createInvoice(item: NRevenueByAccount) {
    const invoice = {
      id: 0,
      accountId: item.accountId,
      accountName: item.accountName,
      ntf: 0
    } as InvoiceCreateEditDto;

    this.showDialog(CommandDialog.CREATE, item, invoice);
  }
  editInvoice(item: NRevenueByAccount, invoice: NInvoice) {
    const { invoiceId, invoiceName, invoiceCurrencyId, deadline, note, month, year, ntf } = invoice;
    const revenueEdit = {
      id: invoiceId,
      nameInvoice: invoiceName,
      accountId: item.accountId,
      month: month,
      year: year,
      collectionDebt: invoice.collectionDebt,
      currencyId: invoiceCurrencyId,
      deadline: deadline,
      note: note,
      ntf: ntf,
      accountName: item.accountName
    } as InvoiceCreateEditDto;
    this.showDialog(CommandDialog.EDIT, item, revenueEdit);
  }

  deleteInvoice(item: NRevenueByAccount, invoice: NInvoice) {
    abp.message.confirm(
      `Delete <strong>${invoice.invoiceName}?</strong>`,
      '',
      (result: boolean) => {
        if (result) {
          this._revenue.deleteInvoice(invoice.invoiceId).subscribe(() => {
            abp.notify.success('Delete invoice successfully');
            this.accountIdSelected = item.accountId;
            this.refresh();
          });
        }
      },
      true
    );
  }

  private showDialog(command: CommandDialog, item: NRevenueByAccount, invoice: InvoiceCreateEditDto): void {
    let dialogRef = this.dialog.open(CreateEditNRevenueComponent, {
      data: {
        item: invoice,
        command: command,
      } as InvoiceDialogData,
      width: "700px",
      disableClose: true,
    });
    dialogRef.afterClosed().subscribe((rs: InvoiceCreateEditDto) => {
      if (rs) {
        this.accountIdSelected = rs.accountId;
        this.refresh();
      }
    });
  }

  editNote(invoiceId: number, note: string, item: NRevenueByAccount) {
    const revenue = {
      id: invoiceId,
      note: note
    } as UpdateNoteDto;
    this.showDialogNote(revenue, item);
  }
  private showDialogNote(revenue: UpdateNoteDto, item: NRevenueByAccount): void {
    let dialogRef = this.dialog.open(EditNoteNRevenueComponent, {
      data: {
        item: revenue
      },
      width: "700px",
      disableClose: true,
    });
    dialogRef.afterClosed().subscribe((rs: UpdateNoteDto) => {
      const indexRevenue = item.invoices.findIndex(item => item.invoiceId === rs.id)
      item.invoices[indexRevenue].note = rs.note;
    });
  }
  editStatus(invoiceId: number, status: number, item: NRevenueByAccount) {
    const revenue = {
      id: invoiceId,
      status: status
    } as UpdateStatusInvoiceDto;
    this.showDialogStatus(revenue, item);
  }

  showDialogClientPayDeviant(incomingEntryId: number, accountName: string, currencyName: string, money: string, incomingName: string): void {
    const title = `<b>Khách hàng trả kênh tiền: </b>#${incomingEntryId} ${incomingName} ${money}${currencyName}`;
    const _dialogRef = this.dialog.open(ClientPayDeviantDialogComponent, {
      data: { incomingEntryId, accountName, title } as DataClientPayDeviantDialog,
      width: '800px'
    });

    _dialogRef.afterClosed().subscribe(res => {
      if(!res) return;
      this.refresh();
    });
  }

  private showDialogStatus(revenue: UpdateStatusInvoiceDto, item: NRevenueByAccount): void {
    let dialogRef = this.dialog.open(EditStatusNRevenueComponent, {
      data: {
        item: revenue
      },
      width: "500px",
      disableClose: true,
    });
    dialogRef.afterClosed().subscribe((rs: UpdateStatusInvoiceDto) => {
      this.accountIdSelected = item.accountId;
      this.refresh();
    });
  }

  createRevenueByAccount() {
    this.showCreateRevenueByAccountDialog();
  }

  private showCreateRevenueByAccountDialog(): void {
    let dialogRef = this.dialog.open(CreateNRevenueByAccountComponent, {
      data: "",
      width: "700px",
      disableClose: true,
    });
    dialogRef.afterClosed().subscribe((rs) => {
      if (rs) {
        this.accountIdSelected = rs.accountId;
        this.refresh();
      }
    });
  }

  onDropdownMultiFilter(e){
    this.multiValueStatusFilter = e.value;
    this.refresh();
  }
  getInvoiceRoutingFromHomepage(){
    let status = this.route.snapshot.queryParamMap.get("status")

    if(status){
      this.multiValueStatusFilter = [InvoiceStatus.TRA_1_PHAN, InvoiceStatus.CHUA_TRA];
      this.refresh();
    }

  }
  isShowCreateBtn(){
    return this.isGranted(PERMISSIONS_CONSTANT.Finance_Invoice_Create);
  }
  isShowEditBtn(){
    return this.isGranted(PERMISSIONS_CONSTANT.Finance_Invoice_Update);
  }
  isShowDeleteBtn(){
    return this.isGranted(PERMISSIONS_CONSTANT.Finance_Invoice_Delete);
  }
  isShowAutoPayBtn(){
    return this.isGranted(PERMISSIONS_CONSTANT.Finance_Invoice_AutoPay);
  }
  isShowKhacHangTraKenhTienBtn(){
    return this.isGranted(PERMISSIONS_CONSTANT.Finance_Invoice_KhachHangTraKenhTien);
  }
  isShowEditNote(){
    return this.isGranted(PERMISSIONS_CONSTANT.Finance_Invoice_EditNote);
  }
  isShowEditInvoiceBtn(){
    return this.isGranted(PERMISSIONS_CONSTANT.Finance_Invoice_EditStatusInvoice);
  }
  ExportReport(){
    this._revenue.ExportReport().subscribe((data) => {
      const file = new Blob([this.convertFile(atob(data.result))], {
        type: "application/vnd.ms-excel;charset=utf-8",
      });
      FileSaver.saveAs(file, `Báo cáo công nợ.xlsx`);
    });
  }
  isShowBtnExportReport(){
    return this.permission.isGranted(this.Finance_BankTransaction_ExportExcel);
  }

}

export class NRevenuePagedRequestDto extends PagedRequestDto {
  filterDateTimeParam: IFilterDateTimeParam;
  accountIds: number[];
  isDoneDebt: boolean;
  statuses: number[];
}
export class StatisticsDto {
  collectionDebt: number;
  paid: number;
  debt: number;
  currencyName: string;
}
export class InputToGetAllPaging{
  statuses: number[];
  request: PagedRequestDto
}
