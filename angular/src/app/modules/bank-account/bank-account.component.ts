import { Component, Injector, OnInit } from "@angular/core";
import { MatDialog } from "@angular/material/dialog";
import { BankAccountService } from "@app/service/api/bank-account.service";
import { BaseApiService } from "@app/service/api/base-api.service";
import { BankAccountDto } from "@app/service/model/bank-account.dto";
import { accountModuleAnimation } from "@shared/animations/routerTransition";
import { CreateEditBankAccountComponent } from "./create-edit-bank-account/create-edit-bank-account.component";
import {
  PagedListingComponentBase,
  PagedRequestDto,
  PagedResultResultDto,
} from "@shared/paged-listing-component-base";
import { catchError, finalize, map, startWith } from "rxjs/operators";
import { InputFilterDto } from "@shared/filter/filter.component";
import { ActivatedRoute } from "@angular/router";
import { PERMISSIONS_CONSTANT } from "@app/constant/permission.constant";
import * as FileSaver from "file-saver";
import { EComparisor } from "../revenue-managed/revenue-managed.component";
import { CurrencyService } from "@app/service/api/currency.service";
import { CommonService } from "@app/service/api/new-versions/common.service";
import { data } from "jquery";
import { CurrencyConvertDto } from "../currency/currency.component";
import { AccountTypeDto } from "../account-type/account-type.component";
import { AccountForDropdownDto, ValueAndNameModel } from "@app/service/model/common-DTO";
import { AccountService } from "@app/service/api/account.service";
import { ActiveCompanyBankAccountComponent } from "./active-company-bank-account/active-company-bank-account.component";
import { AccountTypeEnum } from "@shared/AppEnums";
import { HttpErrorResponse, HttpParams } from "@angular/common/http";
import { of } from "rxjs";
import { TranslateService } from "@ngx-translate/core";

@Component({
  selector: "app-bank-account",
  templateUrl: "./bank-account.component.html",
  styleUrls: ["./bank-account.component.css"],
})
export class BankAccountComponent
  extends PagedListingComponentBase<BankAccountDto>
  implements OnInit
{
  Account_Directory_BankAccount =
    PERMISSIONS_CONSTANT.Account_Directory_BankAccount;
  Account_Directory_BankAccount_Create =
    PERMISSIONS_CONSTANT.Account_Directory_BankAccount_Create;
  Account_Directory_BankAccount_Delete =
    PERMISSIONS_CONSTANT.Account_Directory_BankAccount_Delete;
  Account_Directory_BankAccount_Edit =
    PERMISSIONS_CONSTANT.Account_Directory_BankAccount_Edit;
  Account_Directory_BankAccount_ViewAll =
    PERMISSIONS_CONSTANT.Account_Directory_BankAccount_View;
  Account_Directory_BankAccount_ViewDetail =
    PERMISSIONS_CONSTANT.Account_Directory_BankAccount_ViewBankAccountDetail;
  Account_Directory_BankAccount_Lock =
    PERMISSIONS_CONSTANT.Account_Directory_BankAccount_LockUnlock;
  Account_Directory_BankAccount_Unlock =
    PERMISSIONS_CONSTANT.Account_Directory_BankAccount_LockUnlock;
  Account_Directory_BankAccount_ActiveDeactive = PERMISSIONS_CONSTANT.Account_Directory_BankAccount_ActiveDeactive;
  Account_Directory_BankAccount_Export = PERMISSIONS_CONSTANT.Account_Directory_BankAccount_Export;
  routeUrlFirstLevel = this.APP_CONSTANT.UrlBreadcrumbFirstLevel.Menu4;
  routeUrlSecondLevel = this.APP_CONSTANT.UrlBreadcrumbSecondLevel.bankAccount;
  queryParams;
  public readonly FILTER_CONFIG: InputFilterDto[] = [
    {
      propertyName: "holderName",
      comparisions: [0, 6, 7, 8],
      displayName: "filterBankAccount.HolderName",
    },
    {
      propertyName: "bankNumber",
      comparisions: [0, 6, 7, 8],
      displayName: "filterBankAccount.BankNumber",
    },
    {
      propertyName: "bankName",
      comparisions: [0, 1, 2, 3, 4, 5],
      displayName: "filterBankAccount.Bank",
    },
    {
      propertyName: "currencyName",
      comparisions: [0, 6, 7, 8],
      displayName: "filterBankAccount.Currency",
    },
    {
      propertyName: "accountName",
      comparisions: [0, 6, 7, 8],
      displayName: "filterBankAccount.Account",
    },
    {
      propertyName: "amount",
      comparisions: [0, 1, 2, 3, 4, 5],
      displayName: "filterBankAccount.Amount",
    },
    {
      propertyName: "isActive",
      comparisions: [0],
      displayName: "filterBankAccount.IsActive",
    },
  ];
  requestParam: PagedRequestDto;
  constructor(
    private route: ActivatedRoute,
    private bankaccountService: BankAccountService,
    private accountService: AccountService,
    private commonService: CommonService,
    private dialog: MatDialog,
    injector: Injector,
    private translate: TranslateService
  ) {
    super(injector);
  }
  accounts: BankAccountDto[] = [];
  searchStatus: Status = Status.ACTIVE;
  listCurrency: ValueAndNameModel[] = [];
  listAccountTypeEnum: ValueAndNameModel[] = [];
  listAccountTemp: AccountForDropdownDto[] = [];
  listAccount: AccountForDropdownDto[] = [];
  optionAll: ValueAndNameModel = {value: -1, name: "All"};

  searchCurrency: number = -1;
  searchAccountTypeEnum: number = Number(this.optionAll.value);
  selectedAccountIds: number[] = [];
  //protected companyAccountType = 2

  ngOnInit() {
    this.commonService.getAllCurrency().subscribe((data) => {
      this.listCurrency = data.result;
      const itemAll = this.optionAll;
      this.listCurrency.unshift(itemAll);
    });

    this.commonService.getAllAccounTypeEnum().subscribe((data) => {
      this.listAccountTypeEnum = data.result;
      const itemAll = this.optionAll;
      this.listAccountTypeEnum.unshift(itemAll);
    });

    this.commonService.getAllAccounts().subscribe((data) => {
      this.listAccountTemp = this.listAccount = data.result;
      this.getAccountDefault();
    });

    this.refresh();
    this.translate.onLangChange.subscribe(() => {
      this.onLangChange();
    });
    this.route.queryParams.subscribe(params => {
      this.queryParams = new HttpParams({ fromObject: params });
      this.onLangChange();
    });
  }
  
  onLangChange(){
    this.translate.get("menu.menu4").subscribe((res: string) => {
      this.routeTitleFirstLevel = res;
      this.updateBreadCrumb();
    });
    this.translate.get("menu4.m4_child1").subscribe((res: string) => {
      this.title = res;
      this.updateBreadCrumb();
    });
  }
  getQueryParam(){

  }

  updateBreadCrumb() {
    let queryParamsString = this.queryParams.toString();
    this.listBreadCrumb = [
      { name: this.routeTitleFirstLevel , url: this.routeUrlFirstLevel },
      { name: ' <i class="fas fa-chevron-right"></i> ' },
      { name: this.title , url: this.routeUrlSecondLevel + (queryParamsString ? '?' + queryParamsString : '')}
    ];
  }

  protected list(
    request: PagedRequestBankAccount,
    pageNumber: number,
    finishedCallback: Function
  ): void {
    this.requestParam = request;
    if (this.searchStatus !== Status.ALL) {
      request.isActive = Boolean(this.searchStatus);
    }

    if (this.searchCurrency !== -1) {
      request.currencyIds = [this.searchCurrency];
    }

    if (this.searchAccountTypeEnum !== this.optionAll.value) {
      request.accountTypeEnum = this.searchAccountTypeEnum;
    }

    request.accountIds = this.selectedAccountIds;

    this.bankaccountService
      .getAllPaging(request)
      .pipe(
        finalize(() => {
          finishedCallback();
        })
      )
      .subscribe((result: PagedResultResultDto) => {
        this.accounts = result.result.items;
        this.showPaging(result.result, pageNumber);
      });
  }

  creatBankAccount(): void {
    let accounts = {} as BankAccountDto;
    this.showDialogBanks(accounts);
  }

  editBankAccount(accounts: BankAccountDto): void {
    this.showDialogBanks(accounts);
  }
  getAccountDefault(){
    this.accountService.getAccountDefault().subscribe((data) => {
      if(!data.result) return;
      this.selectedAccountIds = [data.result.id];
      this.searchAccountTypeEnum = data.result.type;
      this.refresh();
    });
  }

  showDialogBanks(accounts: BankAccountDto): void {
    let item = {
      id: accounts.id,
      holderName: accounts.holderName,
      bankNumber: accounts.bankNumber,
      bankId: accounts.bankId,
      currencyId: accounts.currencyId,
      accountId: accounts.accountId,
      baseBalance: accounts.baseBalance,
      amount: accounts.amount,
      isActive: accounts.isActive,
    };
    const dialogRef = this.dialog.open(CreateEditBankAccountComponent, {
      data: item,
      width: "800px",
    });

    dialogRef.afterClosed().subscribe((result) => {
      this.refresh();
    });
  }

  delete(accounts: BankAccountDto): void {
    abp.message.confirm(
      "Delete name '" + accounts.holderName + "'?",
      "",
      (result: boolean) => {
        if (result) {
          this.bankaccountService.delete(accounts.id).subscribe(() => {
            abp.notify.info(
              "Deleted bank account " + accounts.amount + " successfully"
            );
            this.refresh();
          });
        }
      }
    );
  }

  handleChangeStatus(item: BankAccountDto) {
    if(item.lockedStatus){
      return;
    }
    abp.message.confirm(
      "Do you want to change the status?",
      "",
      (result: boolean) => {
        if (result) {
          item.isActive = !item.isActive;
          if(item.isActive){
            this.activeCompanyBankAccount(item);
          }else{
            this.bankaccountService.deActive(item.id)
            .pipe(
              map(data => ({...data, loading: false})),
              startWith({loading: true, success: false, result: null }),
              catchError((error: HttpErrorResponse) => {
                return of({loading: false, success: false, result: null, error})
              })
            )
            .subscribe(response => {
              if(response.success){
                abp.notify.success(response.result);
              }
              if(!response.loading){
                this.refresh();
              }
            })
          }
        }
      }
    );
  }
  accountTypeEnumChange(){
    this.listAccount = this.listAccountTemp.filter(s => this.searchAccountTypeEnum == this.optionAll.value || s.accountType == this.searchAccountTypeEnum);
    this.selectedAccountIds = [];
    this.getFirstPage();
  }

  showDetail(id: any) {
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

  lockBankAccount(account) {
    abp.message.confirm(
      "Lock bank account: " + account.holderName + "?",
      ``,
      (result: boolean) => {
        if (result) {
          this.bankaccountService
            .lockBankAccount(account.id)
            .subscribe((rs) => {
              abp.notify.success("Locked bank account: " + account.holderName);
              this.refresh();
            });
        }
      }
    );
  }
  unlockBankAccount(account) {
    abp.message.confirm(
      "Unlock bank account: " + account.holderName + "?",
      "",
      (result: boolean) => {
        if (result) {
          this.bankaccountService
            .unlockBankAccount(account.id)
            .subscribe((rs) => {
              abp.notify.success(
                "Unlocked bank account: " + account.holderName
              );
              this.refresh();
            });
        }
      }
    );
  }
  downloadFile() {
    this.bankaccountService
      .exportBankAccount(this.requestParam)
      .subscribe((data) => {
        const file = new Blob([this.convertFile(atob(data.result))], {
          type: "application/vnd.ms-excel;charset=utf-8",
        });
        FileSaver.saveAs(file, `Bank account.xlsx`);
      });
  }
  onAccountSelect(ids:number[]){
    this.selectedAccountIds = ids
    this.getFirstPage();
  }

  activeCompanyBankAccount(bankAccount: BankAccountDto ){
    const activeCompanyBankAccountComponent = this.dialog.open(
      ActiveCompanyBankAccountComponent,
      {
        width: "550px",
        data: bankAccount,
        disableClose: true,
      }
    );

    activeCompanyBankAccountComponent
      .afterClosed()
      .subscribe(() => {
        this.refresh()
      });
  }
}

export enum Status {
  ALL = -1,
  ACTIVE = 1,
  INACTIVE = 0,
}

export class PagedRequestBankAccount extends PagedRequestDto {
  currencyIds: number[];
  accountTypeEnum: number;
  isActive: boolean;
  accountIds: number[];
}
export class ActiveBankAccountDto {
  bankAccountId: number;
  baseBalance: number;
}
