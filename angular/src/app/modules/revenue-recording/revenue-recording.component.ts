import { async } from '@angular/core/testing';
import { PERMISSIONS_CONSTANT } from "@app/constant/permission.constant";
import { InputFilterDto } from "./../../../shared/filter/filter.component";
import { MatDialog } from "@angular/material/dialog";
import { RevenueRecordService } from "./../../service/api/revenue-record.service";
import {
  PagedListingComponentBase,
  PagedRequestDto,
} from "@shared/paged-listing-component-base";
import {
  Component,
  OnInit,
  Injector,
  inject,
  ViewChild,
  ElementRef,
} from "@angular/core";
import { catchError, finalize } from "rxjs/operators";
import * as FileSaver from "file-saver";
import { Time } from "@angular/common";
import { CurrencyColor, DateSelectorEnum, TypeFilterTypeOptions } from "@shared/AppEnums";
import { CurrencyService } from "@app/service/api/currency.service";
import { CurrencyConvertDto } from "../currency/currency.component";
import { CommonService } from "@app/service/api/new-versions/common.service";
import {
  DateTimeSelector,
  DateFormat,
} from "@shared/date-selector/date-selector.component";
import * as moment from "moment";
import { ActivatedRoute, Params } from "@angular/router";
import { AppConsts, OPTION_ALL } from "@shared/AppConsts";
import { IOption } from "@shared/components/custome-select/custome-select.component";
import { Utils } from "@app/service/helpers/utils";
import { TreeInOutTypeOption } from '@shared/components/tree-in-out-type/tree-in-out-type.component';
import { TranslateService } from '@ngx-translate/core';
import { HttpParams } from '@angular/common/http';

@Component({
  selector: "app-revenue-recording",
  templateUrl: "./revenue-recording.component.html",
  styleUrls: ["./revenue-recording.component.css"],
})
export class RevenueRecordingComponent
  extends PagedListingComponentBase<RevenueRecordDto>
  implements OnInit {
  Finance_IncomingEntry = PERMISSIONS_CONSTANT.Finance_IncomingEntry;
  Finance_IncomingEntry_ExportExcel =
    PERMISSIONS_CONSTANT.Finance_IncomingEntry_ExportExcel;
  requestParam: PagedRequestDto;
  recordList: IncomingEntryDto[];
  totalValue: TotalByCurrencyDto[] = [];
  totalByCurrency: number = 0;
  CurrencyColor = CurrencyColor;
  routeUrlFirstLevel = this.APP_CONSTANT.UrlBreadcrumbFirstLevel.financeManagement;
  routeUrlSecondLevel = this.APP_CONSTANT.UrlBreadcrumbSecondLevel.revenueRecord;
  queryParams;


  @ViewChild("inputSearchClient") inputSearchClient: ElementRef;
  @ViewChild("inputSearchIncoming") inputSearchIncoming: ElementRef;

  public readonly FILTER_CONFIG: InputFilterDto[] = [
    {
      propertyName: "name",
      displayName: "filterRevenueRecord.Name",
      comparisions: [0, 6, 7, 8],
    },
    {
      propertyName: "value",
      displayName: "filterRevenueRecord.Value",
      comparisions: [0, 1, 2, 3, 4],
    },
    {
      propertyName: "accountName",
      displayName: "filterRevenueRecord.Account",
      comparisions: [0, 6, 7, 8],
    },
    {
      propertyName: "branchName",
      displayName: "filterRevenueRecord.Branch",
      comparisions: [0, 6, 7, 8],
    },
    {
      propertyName: "clientName",
      displayName: "filterRevenueRecord.clientName",
      comparisions: [0, 6, 7, 8],
    },
    {
      propertyName: "incomingEntryTypeName",
      displayName: "filterRevenueRecord.incomeType",
      comparisions: [0, 1, 3, 6, 7, 8],
    },
    {
      propertyName: "date",
      displayName: "filterRevenueRecord.createDate",
      comparisions: [0, 1, 2, 3, 4],
      filterType: 1,
    },
  ];

  iconCondition: string = "";
  transDate: string = "";
  sortDrirect: number = 0;
  iconSort: string = "";

  searchId: number;
  searchMoney: number;
  searchCurrency: number = OPTION_ALL;
  searchClient: string;
  selectedClient: number[];
  incomingEntryTypeId: number = OPTION_ALL;
  searchIncoming: string = "";
  searchRevenueCounted: number = OPTION_ALL;

  defaultDateFilterType: DateSelectorEnum = DateSelectorEnum.ALL;
  searchWithDateTime = {} as DateTimeSelector;
  treeInOutTypeOption = { isShowAll: false, type: TypeFilterTypeOptions.INCOMING_ENTRY_TYPE } as TreeInOutTypeOption;
  protected list(
    request: GetAllPagingInComingEntry,
    pageNumber: number,
    finishedCallback: Function
  ): void {
    request.filterItems = [];
    this.requestParam = request;
    request.sort = this.transDate;
    request.sortDirection = this.sortDrirect;
    request.id = this.searchId;
    request.money = this.searchMoney;
    request.clientAccountIds = this.selectedClient;

    if (this.incomingEntryTypeId !== OPTION_ALL) {
      request.incomingEntryTypeId = this.incomingEntryTypeId;
    }

    if (this.searchCurrency !== OPTION_ALL) {
      request.currencyId = this.searchCurrency;
    }

    if (this.searchRevenueCounted !== OPTION_ALL) {
      request.filterItems.push({ propertyName: 'revenueCounted', comparision: 0, value: this.searchRevenueCounted })
    }

    if (this.searchWithDateTime.dateType !== DateSelectorEnum.ALL) {
      request.filterDateTimeParam = {
        dateTimeType: 2,
        fromDate: moment(this.searchWithDateTime.fromDate).format(
          DateFormat.YYYY_MM_DD
        ),
        toDate: moment(this.searchWithDateTime.toDate).format(
          DateFormat.YYYY_MM_DD
        ),
      };
    }

    this.service
      .getAllPaging(request)
      .pipe(
        finalize(() => {
          finishedCallback();
        })
      )
      .subscribe((data) => {
        this.recordList = data.result.items;
        this.showPaging(data.result, pageNumber);
      });
    this.service
      .GetTotalByCurrency(request)
      .pipe(
        finalize(() => {
          finishedCallback();
        })
      )
      .subscribe((data) => {
        this.totalValue = data.result;
        this.totalByCurrency = this.totalValue.reduce((sum, item) => {
          return (sum += item.totalValueToVND);
        }, 0);
      });
    this.translate.onLangChange.subscribe(() => {
      this.onLangChange();
    });
    this.route.queryParams.subscribe(params => {
      this.queryParams = new HttpParams({ fromObject: params });
      this.onLangChange();
    });
  }
  
  onLangChange(){
    this.translate.get("menu.menu5").subscribe((res: string) => {
      this.routeTitleFirstLevel = res;
      this.updateBreadCrumb();
    });
    this.translate.get("menu5.m5_child1").subscribe((res: string) => {
      this.title = res;
      this.updateBreadCrumb();
    });
  }

  updateBreadCrumb() {
    let queryParamsString = this.queryParams.toString();
    this.listBreadCrumb = [
      { name: this.routeTitleFirstLevel , url: this.routeUrlFirstLevel },
      { name: ' <i class="fas fa-chevron-right"></i> ' },
      { name: this.title , url: this.routeUrlSecondLevel + (queryParamsString ? '?' + queryParamsString : '')}
    ];
  }
  protected delete(entity: RevenueRecordDto): void { }

  constructor(
    private service: RevenueRecordService,
    private currencyService: CurrencyService,
    private commonService: CommonService,
    private dialog: MatDialog,
    private route: ActivatedRoute,
    injector: Injector,
    private translate: TranslateService
  ) {
    super(injector);
    const id = this.route.snapshot.queryParamMap.get("id");
    if (id) {
      this.searchId = Number(id);
    }
    this.searchWithDateTime.dateType = this.defaultDateFilterType
    this.applyUrlFilters()
  }

  sortDate(data) {
    if (this.iconCondition !== data) {
      this.sortDrirect = -1;
    }
    this.iconCondition = data;
    this.transDate = data;
    this.sortDrirect++;
    if (this.sortDrirect > 1) {
      this.transDate = "";
      this.iconSort = "";
      this.sortDrirect = -1;
    }
    if (this.sortDrirect == 1) {
      this.iconSort = "fas fa-sort-amount-down";
    } else if (this.sortDrirect == 0) {
      this.iconSort = "fas fa-sort-amount-up";
    } else {
      this.iconSort = "fas fa-sort";
    }
    this.refresh();
  }

  listCurrency: CurrencyConvertDto[];
  listClient: IOption[];
  treeIncomingEntries: TreeIncomingEntries[] = [];
  tmpTreeIncomingEntries: TreeIncomingEntries[] = [];

  ngOnInit(): void {
    this.subscriptions.push(
      AppConsts.periodId.asObservable().subscribe(() => {
        this.refresh()
      }))
    this.currencyService.GetAllForDropdown().subscribe((data) => {
      this.listCurrency = data.result;
      const itemAll = { id: OPTION_ALL, name: "All" } as CurrencyConvertDto;
      this.listCurrency.unshift(itemAll);
    });
    this.service.getAllClient().subscribe((data) => {
      this.listClient = data.result.map(item => {
        item.name = item.clientAccountName;
        item.value = item.clientAccountId
        return item;
      });
    });
    this.commonService.getTreeIncomingEntries().subscribe((data) => {
      data.result.forEach((item) =>
        this.filterData(item as TreeIncomingEntries, 1)
      );
      this.tmpTreeIncomingEntries = [...this.treeIncomingEntries];

    });
  }

  filterData(data: TreeIncomingEntries, level: number) {
    data.paddingLevel = "";
    for (let i = 1; i < level; i++) {
      data.paddingLevel += "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;";
    }
    this.treeIncomingEntries.push(data);
    if (data.children.length > 0) {
      data.children.forEach((item) => {
        this.filterData(item, level + 1);
      });
    }
  }



  selectionIncomingOpenChange(isOpen: boolean) {
    if (isOpen) {
      this.inputSearchIncoming.nativeElement.focus();
      return;
    }
    this.treeIncomingEntries = this.tmpTreeIncomingEntries;
  }

  onDateChange(event) {
    this.searchWithDateTime = event;
    this.defaultDateFilterType = event.dateType;
    this.searchWithDateTime.dateType = event.dateType
    let cloneDate = { ...event }
    cloneDate.fromDate = moment(cloneDate.fromDate).format("YYYY-MM-DD")
    cloneDate.toDate = moment(cloneDate.toDate).format("YYYY-MM-DD")

    this.onPageFilter('dateFilter', cloneDate)
  }

  onClientSelect(ids: number[]) {
    this.selectedClient = ids;
    if (ids.length == 0) {
      this.onPageFilter('clientIds', OPTION_ALL)
    }
    else {
      this.onPageFilter('clientIds', this.selectedClient)
    }
  }

  handleClearFilter() {
    this.searchId = undefined;
    this.searchMoney = undefined;
    this.selectedClient = [];
    this.incomingEntryTypeId = OPTION_ALL;
    this.searchCurrency = OPTION_ALL;
    this.defaultDateFilterType = DateSelectorEnum.ALL;
    this.searchWithDateTime = {
      dateType: DateSelectorEnum.ALL,
    } as DateTimeSelector;
    this.defaultDateFilterType = DateSelectorEnum.ALL;
    this.searchRevenueCounted = OPTION_ALL
    this.refresh();
  }

  export() { }
  downloadFile() {
    this.service.exportExcel(this.requestParam).subscribe((data) => {
      const file = new Blob([this.convertFile(atob(data.result))], {
        type: "application/vnd.ms-excel;charset=utf-8",
      });
      FileSaver.saveAs(file, `Ghi nhận thu.xlsx`);
    });
  }
  convertFile(fileData) {
    var buf = new ArrayBuffer(fileData.length);
    var view = new Uint8Array(buf);
    for (var i = 0; i != fileData.length; ++i)
      view[i] = fileData.charCodeAt(i) & 0xff;
    return buf;
  }

  onCancelFilterOutcomeType() {
    this.incomingEntryTypeId = OPTION_ALL
    this.refresh()
    this.setFilterToUrl('incomingEntryTypeId', OPTION_ALL)
  }

  onIncomeTypeFilter() {
    this.onPageFilter('incomingEntryTypeId', this.incomingEntryTypeId)
  }

  onRevenueCountedFilter() {
    this.onPageFilter('revenueCounted', this.searchRevenueCounted)
  }

  onCurrencyFilter() {
    this.onPageFilter('currencyId', this.searchCurrency)
  }

  async onSearch(){
    this.searchId = null
    this.searchMoney = null
    await this.onResetFilter()

    this.getDataPage(1)
  }

  async onfillterMoney() {
    this.searchId = null
    this.searchText = ""
    await this.onResetFilter()
    this.onPageFilter('money', this.searchMoney)
  }

  async onfillterId() {
    this.searchMoney = null
    this.searchText = ""
    await this.onResetFilter()

    this.onPageFilter('searchId', this.searchId)
  }

  applyUrlFilters() {
    var querySnapshot = this.route.snapshot.queryParams

    this.incomingEntryTypeId = querySnapshot['incomingEntryTypeId'] ? Utils.toNumber(querySnapshot['incomingEntryTypeId']) : OPTION_ALL;
    this.searchRevenueCounted = querySnapshot['revenueCounted'] ? Utils.toNumber(querySnapshot['revenueCounted']) : OPTION_ALL;
    this.searchCurrency = querySnapshot['currencyId'] ? Utils.toNumber(querySnapshot['currencyId']) : OPTION_ALL;
    this.searchMoney = querySnapshot['money'] ? Utils.toNumber(querySnapshot['money']) : null;
    this.searchId = querySnapshot['searchId'] ? Utils.toNumber(querySnapshot['searchId']) : null;
    this.selectedClient = querySnapshot['clientIds'] ? JSON.parse(querySnapshot['clientIds']) : [];
    let dateFilterParam = querySnapshot['dateFilter'] ? JSON.parse(querySnapshot['dateFilter']) : {} as DateTimeSelector;

    if (dateFilterParam.dateType) {
      this.searchWithDateTime = dateFilterParam
      this.searchWithDateTime.fromDate = moment(this.searchWithDateTime.fromDate)
      this.searchWithDateTime.toDate = moment(this.searchWithDateTime.toDate)
      this.defaultDateFilterType = this.searchWithDateTime.dateType
    }
  }

  async onResetFilter() {

    this.incomingEntryTypeId = OPTION_ALL
    this.searchRevenueCounted = OPTION_ALL
    this.searchCurrency = OPTION_ALL
    this.selectedClient = []
    this.searchWithDateTime = {
      dateType: DateSelectorEnum.ALL
    } as DateTimeSelector;

    this.defaultDateFilterType = DateSelectorEnum.ALL;
    await this.resetQueryParams(['incomingEntryTypeId', 'revenueCounted', 'currencyId', 'money', 'searchId', 'clientIds', 'dateFilter'])
  }

  isShowDetail(){
    return this.isGranted(PERMISSIONS_CONSTANT.Finance_BankTransaction_BankTransactionDetail_TabIncommingEntry_View)
  }

}
export class RevenueRecordDto {
  incomingEntryTypeId: number;
  bankTransactionId: number;
  name: string;
  status: boolean;
  accountId: number;
  accountName: string;
  branchId: number;
  branchName: string;
  value: number;
  currencyId: number;
  currencyName: string;
  id: number;
}
export class TotalByCurrencyDto {
  currencyId: number;
  currencyName: string;
  totalValue: number;
  totalValueToVND: number;
  currencyCode: string;
}
export class IncomingEntryDto {
  incomingEntryTypeId: number;
  incomingEntryTypeName: string;
  bankTransactionId: number;
  name: string;
  status: boolean;
  accountId: number;
  accountName: string;
  branchId: number;
  currencyId: number;
  clientName: string;
  currencyName: string;
  date: Time;
  branchName: string;
  value: number;
  valueToVND: number;
  updatedBy: string;
  updatedTime: Time;
  creationTime: Time;
  creationUserId: number;
  creationUser: string;
  lastModifiedTime: Time;
  lastModifiedUserId: number;
  lastModifiedUser: string;
  id: number;
  revenueCounted: boolean
}

export class GetAllPagingInComingEntry extends PagedRequestDto {
  id: number;
  clientAccountIds: number[];
  money: number;
  incomingEntryTypeId: number;
  currencyId: number;
  filterDateTimeParam: {
    dateTimeType: number;
    fromDate: string;
    toDate: string;
  };
}

export interface ClientDto {
  clientAccountCode: string;
  clientAccountId: number;
  clientAccountName: string;
}

export interface OptionIncomingEntriesDto {
  name: string;
  id: number;
  parentId: number;
  level: number;
}

export interface TreeIncomingEntries {
  item: OptionIncomingEntriesDto;
  children: TreeIncomingEntries[];
  paddingLevel: string;
}
