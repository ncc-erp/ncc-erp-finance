import { Component, OnInit, Injector, ViewChild } from "@angular/core";
import { RevenueReportService } from "./../../service/api/revenue-report.service"
import { RevenueRecordService } from "./../../service/api/revenue-record.service";
import { Time } from "@angular/common";
import {
  PagedListingComponentBase,
  PagedRequestDto,
  FilterDto
} from "@shared/paged-listing-component-base";
import { AppComponentBase } from '@shared/app-component-base';
import { TreeInOutTypeComponent, TreeInOutTypeOption } from "@shared/components/tree-in-out-type/tree-in-out-type.component";
import { CommonService } from "@app/service/api/new-versions/common.service";
import { Utils } from "@app/service/helpers/utils";
import { UtilitiesService } from "@app/service/api/new-versions/utilities.service";
import { AppConsts, OPTION_ALL } from "@shared/AppConsts";
import { CurrencyColor, DateSelectorEnum, TypeFilterTypeOptions } from "@shared/AppEnums"
import {
  DateTimeSelector,
  DateFormat,
} from "@shared/date-selector/date-selector.component";
import { IOption } from "@shared/components/custome-select/custome-select.component";
import { CurrencyConvertDto } from "../currency/currency.component";
import { ActivatedRoute, Router } from "@angular/router";
import * as moment from "moment";
import { CurrencyService } from "@app/service/api/currency.service";

@Component({
  selector: 'app-revenue-report',
  templateUrl: './revenue-report.component.html',
  styleUrls: ['./revenue-report.component.css']
})
export class RevenueReportComponent extends AppComponentBase implements OnInit {
  @ViewChild('treeInOutType') treeInOutType: TreeInOutTypeComponent;

  routeTitleFirstLevel = this.APP_CONSTANT.TitleBreadcrumbFirstLevel.report;
  routeUrlFirstLevel = this.APP_CONSTANT.UrlBreadcrumbFirstLevel.report;
  routeTitleSecondLevel = this.APP_CONSTANT.TitleBreadcrumbSecondLevel.revenueReport;
  routeUrlSecondLevel = this.APP_CONSTANT.UrlBreadcrumbSecondLevel.revenueReport;

  constructor(
    private service: RevenueReportService,
    injector: Injector,
    public _utilities: UtilitiesService,
    private commonService: CommonService,
    private route: ActivatedRoute,
    private router: Router,
    private currencyService: CurrencyService,
    private recordService: RevenueRecordService
  ) {
    super(injector);
    this.applyUrlFilters();
  }

  iconCondition: string = "";
  transDate: string = "";
  sortDrirect: number = 0;
  iconSort: string = "";
  
  searchId: number;
  searchMoney: number;
  searchCurrency: number = OPTION_ALL;
  searchClient: string;
  selectedClient: number[];
  incomingEntryTypeIds: number[];
  searchIncoming: string = "";
  searchRevenueCounted: number = OPTION_ALL;
  treeInOutTypeOption = { isShowAll: false, type: TypeFilterTypeOptions.INCOMING_ENTRY_TYPE } as TreeInOutTypeOption;
  
  defaultDateFilterType: DateSelectorEnum = DateSelectorEnum.ALL;
  searchWithDateTime = {} as DateTimeSelector;
  listCurrency: CurrencyConvertDto[];
  listClient: IOption[];
  treeData: ReportTreeIncomingEntries[] = [];
  searchText: string;
  resetVersion = 0;

  ngOnInit(): void {
    this.updateBreadCrumb();
    this.getAllClient();
    this.getAllCurrency();
    this.loadTreeByFilter();
  }

  getAllCurrency() {
    this.currencyService.GetAllForDropdown().subscribe((data) => {
      this.listCurrency = data.result;
      const itemAll = { id: OPTION_ALL, name: "All" } as CurrencyConvertDto;
      this.listCurrency.unshift(itemAll);
    });
  }

  getAllClient() {
    this.recordService.getAllClient().subscribe((data) => {
      this.listClient = data.result.map(item => {
        item.name = item.clientAccountName;
        item.value = item.clientAccountId
        return item;
      });
    });
  }

  onRefreshCurrentPage(){
    this.onResetFilter();
    this.OnResetSearch();
    this.loadTreeByFilter();
  }

  updateBreadCrumb() {
    this.listBreadCrumb = [
      { name: this.routeTitleFirstLevel , url: this.routeUrlFirstLevel },
      { name: ' <i class="fas fa-chevron-right"></i> ' },
      { name: this.routeTitleSecondLevel , url: this.routeUrlSecondLevel }
    ];
  }

  async onSearch() {
    this.searchId = null
    this.searchMoney = null
    await this.onResetFilter()

    this.setFilterToUrl('searchText', this.searchText);
    this.loadTreeByFilter();
  }

  onListIncomeTypeFilter() {
    if (this.incomingEntryTypeIds.length === 0) {
      this.onTreeFilter('incomingEntryTypeIds', OPTION_ALL)
    }
    else {
      this.onTreeFilter('incomingEntryTypeIds', this.incomingEntryTypeIds)
    }
  }

  onDateChange(event) {
      this.searchWithDateTime = event;
      this.defaultDateFilterType = event.dateType;
      this.searchWithDateTime.dateType = event.dateType
      let cloneDate = { ...event }
      cloneDate.fromDate = moment(cloneDate.fromDate).format("YYYY-MM-DD")
      cloneDate.toDate = moment(cloneDate.toDate).format("YYYY-MM-DD")
  
      this.onTreeFilter('dateFilter', cloneDate)
    }
  
  async onFilterId() {
    await this.onResetFilter()
    this.searchMoney = null
    this.searchText = ""
    this.onTreeFilter('searchId', this.searchId)
  }

  onClientSelect(ids: number[]) {
    this.selectedClient = ids;
    if (ids.length == 0) {
      this.onTreeFilter('clientIds', OPTION_ALL)
    }
    else {
      this.onTreeFilter('clientIds', this.selectedClient)
    }
  }

  async onFilterMoney() {
    await this.onResetFilter();
    this.searchId = null
    this.searchText = ""
    this.onTreeFilter('money', this.searchMoney);
  }

  onRevenueCountedFilter() {
    this.onTreeFilter('revenueCounted', this.searchRevenueCounted)
  }

  onCurrencyFilter() {
    this.onTreeFilter('currencyId', this.searchCurrency)
  }


  onCancelFilterListIncomeType() {
    this.incomingEntryTypeIds = []
    this.onTreeFilter('incomingEntryTypeIds', OPTION_ALL)
    this.treeInOutType.onClearSelected();
  }

  onTreeFilter(filterName: string, value: any) {
    this.setFilterToUrl(filterName, value);
    this.loadTreeByFilter();
  }

  loadTreeByFilter() {
    const req = this.buildTreeRequestFromFilters();
    this.service.getAllByFilter(req).subscribe((res) => {
      this.treeData = this.mapToTree(res.result || []);
      this.resetVersion++;
    });
  }

  mapToTree(arr: RevenuesDto[]): ReportTreeIncomingEntries[] {
    const map = new Map<number, ReportTreeIncomingEntries>();
    const roots: ReportTreeIncomingEntries[] = [];

    arr.forEach((x) => {
      map.set(x.id, { item: x, children: [], paddingLevel: "", level: 0 });
    });

    arr.forEach((x) => {
      const node = map.get(x.id);
      if (!x.parentId) {
        roots.push(node);
      } else {
        map.get(x.parentId)?.children.push(node);
      }
    });

    const setLevel = (node: ReportTreeIncomingEntries, level: number) => {
      node.level = level;
      node.children.forEach((c) => setLevel(c, level + 1));
    };

    roots.forEach((r) => setLevel(r, 0));
    return roots;
  }

  buildTreeRequestFromFilters(): GetAllPagingInComingEntry {
    const req = new GetAllPagingInComingEntry();
    const filterItems: FilterDto[] = [];

    req.searchText = this.searchText;
    req.id = this.searchId;
    req.money = this.searchMoney || undefined;
    req.clientAccountIds = this.selectedClient || [];
    req.incomingEntryTypeIds = this.incomingEntryTypeIds || [];

    if (this.searchCurrency !== OPTION_ALL) {
      req.currencyId = this.searchCurrency;
      filterItems.push({
        comparision: 0,
        propertyName: "currencyId",
        value: this.searchCurrency,
      });
    }

    if (this.searchRevenueCounted !== OPTION_ALL) {
      filterItems.push({
        comparision: 0,
        propertyName: "revenueCounted",
        value: this.searchRevenueCounted,
      });
    }

    if (
      this.searchWithDateTime?.dateType !== undefined &&
      this.searchWithDateTime.dateType !== DateSelectorEnum.ALL
    ) {
      req.filterDateTimeParam = {
        dateTimeType: 2,
        fromDate: moment(this.searchWithDateTime.fromDate).format("YYYY-MM-DD"),
        toDate: moment(this.searchWithDateTime.toDate).format("YYYY-MM-DD"),
      };
    }

    req.filterItems = filterItems;
    return req;
  }

  applyUrlFilters() {
    const querySnapshot = this.route.snapshot.queryParams;

    this.searchText = querySnapshot["searchText"] ? querySnapshot["searchText"] : "";
    this.incomingEntryTypeIds = querySnapshot["incomingEntryTypeIds"]
      ? JSON.parse(querySnapshot["incomingEntryTypeIds"])
      : [];
    this.searchRevenueCounted = querySnapshot["revenueCounted"]
      ? Utils.toNumber(querySnapshot["revenueCounted"])
      : OPTION_ALL;
    this.searchCurrency = querySnapshot["currencyId"]
      ? Utils.toNumber(querySnapshot["currencyId"])
      : OPTION_ALL;
    this.searchMoney = querySnapshot["money"]
      ? Utils.toNumber(querySnapshot["money"])
      : null;
    this.searchId = querySnapshot["searchId"]
      ? Utils.toNumber(querySnapshot["searchId"])
      : null;
    this.selectedClient = querySnapshot["clientIds"]
      ? JSON.parse(querySnapshot["clientIds"])
      : [];

    const dateFilterParam = querySnapshot["dateFilter"]
      ? JSON.parse(querySnapshot["dateFilter"])
      : ({} as DateTimeSelector);

    if (dateFilterParam.dateType) {
      this.searchWithDateTime = dateFilterParam;
      this.searchWithDateTime.fromDate = moment(this.searchWithDateTime.fromDate);
      this.searchWithDateTime.toDate = moment(this.searchWithDateTime.toDate);
      this.defaultDateFilterType = this.searchWithDateTime.dateType;
    } else {
      this.searchWithDateTime = {
        dateType: DateSelectorEnum.ALL,
      } as DateTimeSelector;
      this.defaultDateFilterType = DateSelectorEnum.ALL;
    }
  }

  async applyFiltersAndLoadTree() {
    const queryParams: any = {
      searchText: this.searchText?.trim() ? this.searchText.trim() : null,
      searchId: (this.searchId as any) === "" || this.searchId == null ? null : this.searchId,
      money: (this.searchMoney as any) === "" || this.searchMoney == null ? null : this.searchMoney,
      clientIds: this.selectedClient?.length ? JSON.stringify(this.selectedClient) : null,
      incomingEntryTypeIds: this.incomingEntryTypeIds?.length ? JSON.stringify(this.incomingEntryTypeIds) : null,
      currencyId: this.searchCurrency !== OPTION_ALL ? this.searchCurrency : null,
      revenueCounted: this.searchRevenueCounted !== OPTION_ALL ? this.searchRevenueCounted : null,
      dateFilter: null
    };

    if (
      this.searchWithDateTime?.dateType !== undefined &&
      this.searchWithDateTime.dateType !== DateSelectorEnum.ALL
    ) {
      queryParams.dateFilter = JSON.stringify({
        ...this.searchWithDateTime,
        fromDate: moment(this.searchWithDateTime.fromDate).format("YYYY-MM-DD"),
        toDate: moment(this.searchWithDateTime.toDate).format("YYYY-MM-DD")
      });
    }

    await this.router.navigate([], {
      queryParamsHandling: "merge",
      replaceUrl: true,
      queryParams
    });

    this.loadTreeByFilter();
  }

  clearFilterToUrl() {
    this.router.navigate([], {
      queryParamsHandling: "merge",
      replaceUrl: true,
      queryParams: {
        searchText: null,
        incomingEntryTypeIds: null,
        revenueCounted: null,
        currencyId: null,
        money: null,
        searchId: null,
        clientIds: null,
        dateFilter: null
      }
    });
  }

  handleClearFilter() {
    this.searchText = "";
    this.searchId = undefined;
    this.searchMoney = undefined;
    this.selectedClient = [];
    this.incomingEntryTypeIds = [];
    this.searchCurrency = OPTION_ALL;
    this.defaultDateFilterType = DateSelectorEnum.ALL;
    this.searchWithDateTime = {
      dateType: DateSelectorEnum.ALL,
    } as DateTimeSelector;
    this.searchRevenueCounted = OPTION_ALL;
    this.clearFilterToUrl();
    this.loadTreeByFilter();
  }

  async OnResetSearch() {
    this.searchId = null;
    this.searchMoney = null
    this.searchText = ""
  }

  async onResetFilter() {
    this.incomingEntryTypeIds = []
    this.searchRevenueCounted = OPTION_ALL
    this.searchCurrency = OPTION_ALL
    this.selectedClient = []

    this.searchWithDateTime = {
      dateType: DateSelectorEnum.ALL
    } as DateTimeSelector;

    this.defaultDateFilterType = DateSelectorEnum.ALL

    await this.resetQueryParams(['incomingEntryTypeIds', 'revenueCounted', 'currencyId', 'money', 'searchId', 'clientIds', 'dateFilter'])
  }

  resetQueryParams(keys: string[]) {
    const queryParams = {};

    keys.forEach((k) => {
      queryParams[k] = null;
    });

    this.router.navigate([], {
      queryParamsHandling: "merge",
      replaceUrl: true,
      queryParams,
    });
  }

  
  setFilterToUrl(filterName: string, value: any) {
      let queryParams = {};
      if (value === 0 || (value && value !== OPTION_ALL)) {
        queryParams[filterName] = typeof value !== 'string' ? JSON.stringify(value) : value;
      } else {
        queryParams[filterName] = null;
      }
  
      this.router.navigate([], {
        queryParamsHandling: 'merge',
        replaceUrl: true,
        queryParams
      });
    }
}

export class RevenuesDto {
  name: string;
  code: string;
  level: 0;
  parentId: 0;
  id: 0;
  revenueCounted: boolean;
  isActive: boolean;
  totalCurrencies?: {
    currencyId: number;
    currencyName: string;
    currencyCode: string;
    totalValue: number;
    valueFormat: string;
  }
  currencyConvert: number;
}
export class RevenueListDto {
  name: string;
  children: RevenuesDto[];
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
  revenueCounted: boolean;
}

export class ResultGetIncomingEntryDto {
  resultPagging: {
    totalCount: number;
    items: IncomingEntryDto[];
  };
}

export class GetAllPagingInComingEntry extends PagedRequestDto {
  id: number;
  clientAccountIds: number[];
  money: number;
  incomingEntryTypeIds: number[];
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

export interface ReportTreeIncomingEntries {
  item: RevenuesDto;
  children: ReportTreeIncomingEntries[];
  paddingLevel: string;
  level: number;
}
