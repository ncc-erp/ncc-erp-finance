import { Component, Injector, OnInit } from "@angular/core";
import { FormBuilder } from "@angular/forms";
import { MatDialog } from "@angular/material/dialog";
import { PERMISSIONS_CONSTANT } from "@app/constant/permission.constant";
import { COMPARISIONS, InputFilterDto } from "@shared/filter/filter.component";
import {
  PagedListingComponentBase,
  PagedRequestDto,
  PagedResultResultDto,
} from "@shared/paged-listing-component-base";
import { EComparisor } from "app/modules/revenue-managed/revenue-managed.component";
import { finalize } from "rxjs/operators";
import { AccountantAccountService } from "./../../service/api/accountant-account.service";
import {
  AccountTypeDto,
  CreateEditAccountantAccountComponent,
} from "./create-edit-accountant-account/create-edit-accountant-account.component";
import { ActivatedRoute } from "@angular/router";
import { AccountTypeService } from "@app/service/api/account-type.service";
import { PAGE_SIZE_OPTIONS } from "@app/modules/revenue-managed/revenue-managed.component";
import { TranslateService } from "@ngx-translate/core";
import { HttpParams } from "@angular/common/http";
@Component({
  selector: "app-accountant-account",
  templateUrl: "./accountant-account.component.html",
  styleUrls: ["./accountant-account.component.css"],
})
export class AccountantAccountComponent
  extends PagedListingComponentBase<any>
  implements OnInit
{
  Account_Directory_Account_Create =
    PERMISSIONS_CONSTANT.Account_Directory_FinanceAccount_Create;
  Account_Directory_Account_Delete =
    PERMISSIONS_CONSTANT.Account_Directory_FinanceAccount_Delete;
  Account_Directory_Account_Edit =
    PERMISSIONS_CONSTANT.Account_Directory_FinanceAccount_Edit;
  Account_Directory_BankAccount_ViewDetail =
  PERMISSIONS_CONSTANT.Account_Directory_BankAccount_ViewBankAccountDetail;
  routeTitleFirstLevel = this.APP_CONSTANT.TitleBreadcrumbFirstLevel.account;
  routeUrlFirstLevel = this.APP_CONSTANT.UrlBreadcrumbFirstLevel.account;
  routeTitleSecondLevel = this.APP_CONSTANT.TitleBreadcrumbSecondLevel.accountantAccount;
  routeUrlSecondLevel = this.APP_CONSTANT.UrlBreadcrumbSecondLevel.accountantAccount;

  public readonly PAGE_OPTIONS = PAGE_SIZE_OPTIONS;
  public accountTypes: AccountTypeDto[] = [];
  public selectedAccountType: string = DEFAULT_VALUES.FILTER_VALUE;
  constructor(
    injector: Injector,
    private _accountantAccountService: AccountantAccountService,
    private dialog: MatDialog,
    private _accountTypeService: AccountTypeService,
  ) {
    super(injector);
  }
  ngOnInit() {
    this.getAccountTypes();
    if (this.filterItems) {
      this.filterItems.forEach((item) => {
        if ((item.propertyName = FILTER_PROPERTY_NAME)) {
          this.selectedAccountType = item.value;
        }
      });
    }
    this.refresh();
  }
  accounts: AccountDto[] = [];
  searchStatus: number = 1;
  searchBankAccountStatus: number = 1;

  protected list(
    request: PagedRequestDto,
    pageNumber: number,
    finishedCallback: Function
  ): void {
    request.searchText = this.searchText;
    request.filterItems = this.filterItems;

    if (this.searchStatus !== -1) {
      this.filterItems.push({
        comparision: EComparisor.EQUAL,
        propertyName: "isActive",
        value: this.searchStatus,
      });
    }

    let filterAccount : FilterAccount = request;
    if (this.searchBankAccountStatus !== -1) {
      filterAccount.bankAccountStatus = this.searchBankAccountStatus == 1;
    }

    this._accountantAccountService
      .getAllPaging(filterAccount)
      .pipe(
        finalize(() => {
          finishedCallback();
        })
      )
      .subscribe((result: PagedResultResultDto) => {
        this.accounts = result.result.items;
        this.showPaging(result.result, pageNumber);
      });
    this.updateBreadCrumb();
  }

  onRefreshCurrentPage(){
    this.onResetFilter();
  }

  updateBreadCrumb() {
    this.listBreadCrumb = [
      { name: this.routeTitleFirstLevel , url: this.routeUrlFirstLevel },
      { name: ' <i class="fas fa-chevron-right"></i> ' },
      { name: this.routeTitleSecondLevel , url: this.routeUrlSecondLevel }
    ];
  }

  onResetFilter() {
    this.searchText = "";
    this.selectedAccountType = DEFAULT_VALUES.FILTER_VALUE;
    this.searchStatus = 1;
    this.searchBankAccountStatus = 1;
    this.handleSearch();
  }

  protected delete(account: AccountDto): void {
    abp.message.confirm(
      this.l("Delete account '") + account.name + "'?",
      "",
      (result: boolean) => {
        if (result) {
          this._accountantAccountService.delete(account.id).subscribe(() => {
            abp.notify.success(this.l("Deleted account successfully"));
            this.refresh();
          });
        }
      }
    );
  }
  getAccountTypes() {
    this._accountTypeService.getAll().subscribe((response) => {
      this.accountTypes = response.result;
      this.accountTypes.unshift({
        code: "",
        id: 0,
        name: "All",
      });
    });
  }
  editAccount(account: AccountDto): void {
    this.showDialogAccounts("Edit", account);
  }

  createAccount(): void {
    this.showDialogAccounts("create", {});
  }

  viewDetailBankAccount(id) {
    this.router.navigate(["/app/bank/detail-bank-account" + id]);
  }

  showDialogAccounts(command: string, item: any): void {
    let request = {} as AccountDto;
    if (command == "Edit") {
      request = {
        id: item.id,
        accountTypeId: item.accountTypeId,
        default: item.default,
        name: item.name,
        code: item.code,
        type: item.type,
        isActive: item.isActive,
      };
    }
    const dialogRef = this.dialog.open(CreateEditAccountantAccountComponent, {
      data: {
        command: command,
        item: request,
      },
      width: "600px",
      disableClose: true,
    });

    dialogRef.afterClosed().subscribe(() => {
      this.refresh();
    });
  }

  showDetail(id: number) {
    if (
      this.permission.isGranted(this.Account_Directory_BankAccount_ViewDetail)
    ) {
      this.router.navigate(["app/bankAccountDetail"], {
        queryParams: {
          id: id,
        },
      });
    }
  }

  handleChangeStatus(item: AccountDto) {
    abp.message.confirm(
      "Do you want to change the status?",
      "",
      (result: boolean) => {
        if (result) {
          item.isActive = !item.isActive;
          this._accountantAccountService.changeStatus(item).subscribe(() => {
            abp.notify.success("Change status successfully");
            this.refresh();
          });
        }
      }
    );
  }

  setParamToUrl() {
    this.router.navigate([], {
      queryParams: {
        accountTypeCode: this.selectedAccountTypeCode,
      },
      queryParamsHandling: "merge",
    });
  }

  setFilterItem() {
    this.filterItems = [];
    if (this.selectedAccountType) {
      this.filterItems.push({
        comparision: EComparisor.EQUAL,
        propertyName: FILTER_PROPERTY_NAME,
        value: this.selectedAccountType,
      });
    }
  }

  handleSearch() {
    this.setFilterItem();
    this.setParamToUrl();
    this.refresh();
  }
  isShowActiveDeactiveAction(){
    return this.isGranted(PERMISSIONS_CONSTANT.Account_Directory_BankAccount_ActiveDeactive);
  }
}
export class AccountDto {
  id: number;
  accountTypeId: number;
  default: boolean;
  name: string;
  code: string;
  type: number;
  isActive?: boolean;
}
export class NewAccountDto extends AccountDto {
  holderName: string;
  bankNumber: string;
  bankId: number;
  currencyId: number;
}

const FILTER_PROPERTY_NAME = "accountTypeCode";
const FIRST_PAGE = 1;
export const DEFAULT_VALUES = {
  SEARCH_VALUE: "",
  FILTER_VALUE: "",
};
export class FilterAccount extends PagedRequestDto{
  bankAccountStatus?: boolean;
}
