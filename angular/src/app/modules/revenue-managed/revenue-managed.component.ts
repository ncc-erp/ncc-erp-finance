import { UploadFileDialogComponent } from "./upload-file-dialog/upload-file-dialog.component";
import { Component, Injector, OnInit } from "@angular/core";
import { MatDialog } from "@angular/material/dialog";
import { PERMISSIONS_CONSTANT } from "@app/constant/permission.constant";
import { CurrencyService } from "@app/service/api/currency.service";
import { COMPARISIONS, InputFilterDto } from "@shared/filter/filter.component";
import {
  PagedListingComponentBase,
  PagedRequestDto,
} from "@shared/paged-listing-component-base";
import { finalize } from "rxjs/operators";
import { RevenueManagedService } from "../../service/api/revenue-managed.service";
import { CreateEditRevenuemanagedComponent } from "./create-edit-revenuemanaged/create-edit-revenuemanaged.component";
import { saveAs as importedSaveAs } from "file-saver";
import * as FileSaver from "file-saver";
import * as moment from "moment";
import { AccountTypeService } from "@app/service/api/account-type.service";
import { AccountantAccountService } from "@app/service/api/accountant-account.service";
import { AccountTypeDto } from "@app/modules/accountant-account/create-edit-accountant-account/create-edit-accountant-account.component";
import { AccountDto } from "@app/modules/accountant-account/accountant-account.component";
import { FormControl } from "@angular/forms";

@Component({
  selector: "app-revenue-managed",
  templateUrl: "./revenue-managed.component.html",
  styleUrls: ["./revenue-managed.component.css"],
})
export class RevenueManagedComponent
  extends PagedListingComponentBase<RevenueManagedDto>
  implements OnInit
{
  // Finance_RevenueManaged = PERMISSIONS_CONSTANT.Finance_RevenueManaged;
  // Finance_RevenueManaged_Create =
  //   PERMISSIONS_CONSTANT.Finance_RevenueManaged_Create;
  // Finance_RevenueManaged_Edit =
  //   PERMISSIONS_CONSTANT.Finance_RevenueManaged_Edit;
  // Finance_RevenueManaged_Delete =
  //   PERMISSIONS_CONSTANT.Finance_RevenueManaged_Delete;
  // Finance_RevenueManaged_ViewDetail =
  //   PERMISSIONS_CONSTANT.Finance_RevenueManaged_ViewDetail;
  revenueManagedList: RevenueManagedDto[] = [];

  currencies: any[] = [];
  public searchText: string = "";
  public remainingDebts: RemainDebt[] = [];
  public totalCountRows: number;
  public listStatus = [
    {
      value: "",
      name: "All",
    },
    {
      value: 0,
      name: "Chưa trả",
    },
    {
      value: 1,
      name: "Trả một phần",
    },
    {
      value: 2,
      name: "Hoàn thành",
    },
    {
      value: 3,
      name: "Không trả",
    },
  ];
  public listAccountType: AccountTypeDto[] = [];
  public filteredListAccountType: AccountTypeDto[];
  public listAccount: AccountDto[] = [];
  public filteredListAccount: AccountDto[] = [];
  public selectedStatus: string = DEFAULT_VALUES.STATUS;
  public selectedAccountTypeCode: string = DEFAULT_VALUES.ACCOUNT_TYPE_CODE;
  public selectedAccountId = 0;
  public selectedCurrency: string = DEFAULT_VALUES.CURRENCY;
  public currentSortField: string = DEFAULT_VALUES.SORT_FIELD;
  public currentSortOption: number = ESortDirection.ASCENDING;
  public currentSearchAccountType: string = DEFAULT_VALUES.SEARCH_ACCOUNT_TYPE;
  public currentSearchAccount: string = DEFAULT_VALUES.SEARCH_ACCOUNT;
  public searchType: FormControl = new FormControl("");
  public readonly FIRST_PAGE = 1;
  public readonly DEFAULT_FILTER_TYPE = 0;
  public readonly SORT_DIRECTION = ESortDirection;
  public readonly DEFAULT_SEARCH_VALUE = "";
  public readonly PAGE_SIZE_OPTIONS = PAGE_SIZE_OPTIONS;
  public readonly SORTABLE_FIELD = {
    SendInvoiceDate: "sendInvoiceDate",
    Deadline: "deadline",
    RemainDebt: "remainDebt"
  };
  isCollapsed = false;
  protected list(
    request: PagedRequestDto,
    pageNumber: number,
    finishedCallback: Function
  ): void {
    this.requestParam = request;
    this.requestParam.filterItems = this.generateFilterItems();
    if (this.currentSortField) {
      this.requestParam.sort = this.currentSortField;
      this.requestParam.sortDirection = this.currentSortOption;
    }

    this._service
      .getAllPaging(this.requestParam)
      .pipe(
        finalize(() => {
          finishedCallback();
        })
      )
      .subscribe((res) => {
        this.revenueManagedList = res.result.revenueManagedDtos.items;
        this.totalCountRows = res.result.revenueManagedDtos.totalCount;
        this.remainingDebts = res.result.remainingDebts;
        this.remainingDebts.sort((value1,value2)=>{
            if(value1.remainDebt > value2.remainDebt) return -1;
            return 1;
        })
        this.showPaging(res.result.revenueManagedDtos, pageNumber);
      });
  }
  protected delete(entity: any): void {
    abp.message.confirm(
      "Delete invoice " + entity.nameInvoice + "?",
      "",
      (result) => {
        if (result) {
          this._service.delete(entity.id).subscribe((res) => {
            if (res.success) {
              abp.message.success(res.result);
              this.refresh();
            } else {
              abp.message.error(res.result);
            }
          });
        }
      }
    );
  }
  constructor(
    private _service: RevenueManagedService,
    injector: Injector,
    private dialog: MatDialog,
    private _currencyService: CurrencyService,
    private _accountTypeServices: AccountTypeService,
    private _accountantAccountService: AccountantAccountService
  ) {
    super(injector);
  }
  requestParam: PagedRequestDto;

  ngOnInit(): void {
    this.getCurrencyDropDown();
    this.refresh();
    this.getListAccountTypeCode();
    this.getAccountsByAccountTypeCode();
  }
  createRevenueManaged() {
    this.showDialog("create");
  }
  editInvoice(invoice) {
    this.showDialog("edit", invoice);
  }
  setParamToUrl() {
    this.router.navigate([], {
      queryParams: {
        status: this.selectedStatus,
        accountId: this.selectedAccountId,
        accountTypeCode: this.selectedAccountTypeCode,
        currencyCode: this.selectedCurrency,
        sort: this.currentSortField,
        sortDirection: this.currentSortOption,
      },
      queryParamsHandling: "merge",
    });
  }
  getListAccountTypeCode() {
    this._accountTypeServices.getAll().subscribe((res) => {
      this.listAccountType = res.result;
      this.listAccountType.unshift({ code: "", name: "All", id: 0 });
      this.filteredListAccountType = this.listAccountType;
    });
  }
  getAccountsByAccountTypeCode() {
    this._accountantAccountService
      .getAllPaging({
        sort: DEFAULT_VALUES.SORT_FIELD,
        sortDirection: ESortDirection.ASCENDING,
        filterItems: [
          {
            propertyName: "accountTypeCode",
            value: this.selectedAccountTypeCode,
            comparision: EComparisor.EQUAL,
          },
        ],
        maxResultCount: 10000,
        searchText: this.DEFAULT_SEARCH_VALUE,
        skipCount: 0,
      })
      .subscribe((res) => {
        this.listAccount = res.result.items;
        this.listAccount.unshift({
          accountTypeId: 0,
          code: "",
          default: false,
          name: "All",
          id: 0,
          type:0
        });
        this.filteredListAccount = this.listAccount;
        this.selectedAccountId = 0;
      });
  }
  showDialog(command: String, item?: any): void {
    let invoice = {} as RevenueManagedDto;
    if (command == "edit") {
      invoice = {
        id: item.id,
        nameInvoice: item.nameInvoice,
        accountId: item.accountId,
        accountName: item.accountName,
        accountTypeCode: item.accountTypeCode,
        note: item.note,
        status: item.status,
        month: item.month,
        collectionDebt: item.collectionDebt,
        debtReceived: item.debtReceived,
        unitId: item.unitId,
        sendInvoiceDate: item.sendInvoiceDate,
        deadline: item.deadline,
        remindStatus: item.remindStatus,
      };
    }
    let dialogRef = this.dialog.open(CreateEditRevenuemanagedComponent, {
      data: {
        item: invoice,
        command: command,
      },
      width: "800px",
      disableClose: true,
    });
    dialogRef.afterClosed().subscribe((rs) => {
      if (rs) {
        this.refresh();
      }
    });
  }
  getCurrencyDropDown() {
    this._currencyService.GetAllForDropdown().subscribe((res) => {
      this.currencies = res.result;
      this.currencies.unshift({
        name: "All",
        code: this.DEFAULT_SEARCH_VALUE,
      });
    });
  }
  getCurrencyName(unitId) {
    return this.currencies.find((currency) => currency.id == unitId)?.code;
  }
  downloadFile(name) {
    this._service.revenueDownloadFile(name).subscribe((response: any) => {
      const file = new Blob([this.s2ab(atob(response.result))], {
        type: "",
      });
      FileSaver.saveAs(file, name);
    });
  }
  s2ab(s) {
    var buf = new ArrayBuffer(s.length);
    var view = new Uint8Array(buf);
    for (var i = 0; i != s.length; ++i) view[i] = s.charCodeAt(i) & 0xff;
    return buf;
  }
  uploadFile(revenue) {
    let ref = this.dialog.open(UploadFileDialogComponent, {
      width: "700px",
      data: revenue,
    });
    ref.afterClosed().subscribe((rs) => {
      if (rs) {
        this.refresh();
      }
    });
  }
  removeFile(fileToDelete, id) {
    let listFile = [];
    this._service.revenueGetFiles(id).subscribe((res: any) => {
      if (res.success) {
        listFile = res.result.map((item) => {
          let file = new Blob([this.s2ab(atob(item.bytes))], {
            type: "",
          });
          let arrayOfBlob = new Array<Blob>();
          arrayOfBlob.push(file);
          item = new File(arrayOfBlob, item.fileName);
          return item;
        });
        listFile = listFile.filter((fileItem) => fileItem.name != fileToDelete);
        this._service.revenueUploadFiles(listFile, id).subscribe((rs) => {
          if (rs.success) {
            abp.notify.success(rs.result);
            this.refresh();
          }
        });
      }
    });
  }
  isDeadlinePassed(inputDate: string) {
    const diff = moment().startOf("day").diff(moment(inputDate), "day");
    return diff >= 0;
  }
  getDateDiff(inputDate: string) {
    const deadline = moment(inputDate);
    return moment().startOf("day").diff(deadline, "days");
  }
  handleAccountTypeChange() {
    this.getDataPage(this.FIRST_PAGE);
    this.setParamToUrl();
    if(!this.selectedAccountTypeCode){
      this.selectedAccountId = 0;
      this.setParamToUrl()
      this.getDataPage(this.FIRST_PAGE)
    }
    this.getAccountsByAccountTypeCode();
  }
  filterAccountType(searchString: string) {
    let value = searchString.trim().toLowerCase();
    this.filteredListAccountType = this.listAccountType.filter((type) =>
      type.name.trim().toLowerCase().includes(value)
    );
  }
  filterAccount(searchString: string) {
    let value = searchString.trim().toLowerCase();
    this.filteredListAccount = this.listAccount.filter((account) =>
      account.name.trim().toLowerCase().includes(value)
    );
  }
  sort(fieldName: string) {
    if (fieldName !== this.currentSortField) {
      this.currentSortField = fieldName;
      this.currentSortOption = ESortDirection.ASCENDING;
    } else {
      if (this.currentSortOption === ESortDirection.DESCENDING)
        this.currentSortOption = ESortDirection.ASCENDING;
      else this.currentSortOption = ESortDirection.DESCENDING;
    }
    this.setParamToUrl();
    this.getDataPage(this.FIRST_PAGE);
  }
  generateFilterItems(): FilterItem[] {
    const filterItems = [] as FilterItem[];
    if (this.selectedAccountTypeCode) {
      filterItems.push({
        propertyName: "accountTypeCode",
        value: this.selectedAccountTypeCode,
        comparision: EComparisor.EQUAL,
      });
    }
    if (this.selectedStatus !== DEFAULT_VALUES.STATUS) {
      filterItems.push({
        propertyName: "status",
        value: this.selectedStatus,
        comparision: EComparisor.EQUAL,
      });
    }
    if (this.selectedAccountId) {
      filterItems.push({
        propertyName: "accountId",
        value: this.selectedAccountId,
        comparision: EComparisor.EQUAL,
      });
    }
    if (this.selectedCurrency) {
      filterItems.push({
        propertyName: "currencyCode",
        value: this.selectedCurrency,
        comparision: EComparisor.EQUAL,
      });
    }
    return filterItems;
  }
  handleSelectionChange() {
    this.getDataPage(this.FIRST_PAGE);
    this.setParamToUrl();
  }
  handleSelectAccountTypeOpenedChange(opened: boolean) {
    if (!opened) {
      if (!this.filteredListAccountType.length) {
        this.filteredListAccountType = this.listAccountType;
      }
    }
  }
  handleSelectAccountIdOpenedChange(opened: boolean) {
    if (!opened) {
      if (!this.filteredListAccount.length) {
        this.filteredListAccount = this.listAccount;
      }
    }
  }
}

export class RevenueManagedDto {
  id: number;
  nameInvoice: string;
  accountId: number;
  accountName: string;
  accountTypeCode: string;
  month: number = 1;
  collectionDebt: number;
  debtReceived: number;
  unitId: number;
  sendInvoiceDate: string;
  deadline: string;
  status: number;
  note: string;
  remindStatus: number;
}
export interface RemainDebt{
  collectionDebt: number;
  currencyCode: string;
  debtReceived: number;
  remainDebt: number;
}
export class FilterItem {
  propertyName: string;
  value: number | string;
  comparision: number;
}

export enum EQueryParamsHandling {
  MERGE = "merge",
}

export enum ESortDirection {
  ASCENDING = 0,
  DESCENDING = 1,
}

export enum EComparisor {
  EQUAL = 0,
  LESS_THAN = 1,
  LESS_THAN_OR_EQUAL = 2,
  GREATER_THAN = 3,
  GREATER_THAN_OR_EQUAL = 4,
  NOTE_EQUAL = 5,
  CONTAINS = 6,
  STARTS_WITH = 7,
  ENDS_WITH = 8,
  IN = 9,
}

export const PAGE_SIZE_OPTIONS = [5, 10, 20, 50, 100];

export const DEFAULT_VALUES = {
  ACCOUNT_TYPE_CODE: "CLIENT",
  ACCOUNT_ID: "",
  STATUS: "",
  CURRENCY: "",
  SORT_FIELD: "",
  SEARCH_ACCOUNT_TYPE: "",
  SEARCH_ACCOUNT: "",
};
