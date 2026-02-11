import { AppComponentBase } from "@shared/app-component-base";

import { ReportService } from "./../../service/api/report.service";
import { ActivatedRoute, Router } from "@angular/router";
import { StatusDto } from "./../status/status.component";
import { WorkFlowStatusService } from "./../../service/api/work-flow-status.service";
import { ExpenditureRequestService } from "./../../service/api/expenditure-request.service";
import {
  FilterDto,
  PagedRequestDto,
} from "@shared/paged-listing-component-base";
import { Component, OnInit, Injector, ViewChild } from "@angular/core";
import { Time } from "@angular/common";
import { StatusHistory } from "../expenditure-request-detail/main-tab/main-tab.component";
import { ValueAndNameModel } from "@app/service/model/common-DTO";
import { BranchService } from "@app/service/api/branch.service";
import { BranchDto } from "../branch/branch.component";
import { CommonService } from "@app/service/api/new-versions/common.service";
import { DateSelectorEnum, TypeFilterTypeOptions } from "@shared/AppEnums";
import * as moment from "moment";
import { DateTimeSelector } from "@shared/date-selector/date-selector.component";
import { UtilitiesService } from "@app/service/api/new-versions/utilities.service";
import { Utils } from "@app/service/helpers/utils";
import { TreeInOutTypeComponent, TreeInOutTypeOption } from "@shared/components/tree-in-out-type/tree-in-out-type.component";

@Component({
  selector: 'app-report',
  templateUrl: './report.component.html',
  styleUrls: ['./report.component.css']
})
export class ReportComponent extends AppComponentBase  implements OnInit {
  @ViewChild('treeInOutType') treeInOutType: TreeInOutTypeComponent;

  routeTitleFirstLevel = this.APP_CONSTANT.TitleBreadcrumbFirstLevel.report;
  routeUrlFirstLevel = this.APP_CONSTANT.UrlBreadcrumbFirstLevel.report;
  routeTitleSecondLevel = this.APP_CONSTANT.TitleBreadcrumbSecondLevel.expenditureReport;
  routeUrlSecondLevel = this.APP_CONSTANT.UrlBreadcrumbSecondLevel.expenditureReport;
  
  constructor(
    private service: ReportService,
    injector: Injector,
    private statusService: WorkFlowStatusService,
    private route: ActivatedRoute,
    public branchService: BranchService,
    public _utilities: UtilitiesService,
    private commonService: CommonService,
    public expenditureRequest: ExpenditureRequestService,
    private router: Router
  ) {
    super(injector);
    this.applyUrlFilters();
  }
  searchWithDateTime: DateTimeSelector = {
    dateType: DateSelectorEnum.ALL,
  } as DateTimeSelector;
    defaultDateFilterType: DateSelectorEnum = DateSelectorEnum.ALL;
    statusList: StatusDto[];
    selectedStatus: string = "";
    accreditation: boolean = false;
    requesterOptions: ValueAndNameModel[];
    tempRequesterOptions: ValueAndNameModel[];
    selectedRequester: number[];
    searchRequester: string = "";
    listCurrency: ValueAndNameModel[] = [];
    branchOptions: BranchDto[];
    tempBranchOptions: BranchDto[];
    selectedBranch: number[];
    searchBranch: string = "";
    searchMoney: number;
    treeInOutTypeOption = { isShowAll: false, type: TypeFilterTypeOptions.OUTCOMING_ENTRY_TYPE } as TreeInOutTypeOption;
  
    searchId: number;
    selectedStatusYCTD: string;
    YCTDStatusOptions: GetWorkflowStatusDto[];
    outcomingEntryTypeIds: number[];
    expenseType: number = OPTION_ALL
    selectedCurrencyId?: number = OPTION_ALL;
    treeData: ReportTreeOutcomingEntries[] = [];
  resetVersion = 0;
  searchText: string;

  ngOnInit(): void {
    this.updateBreadCrumb();
    this.getStatusForSearch();
    this.getAllRequester();
    this.getAllBranch();
    this.getTempOutComingEntryStatusOptions();
    this.getAllCurrency();
    this.loadTreeByFilter();
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

  onFilterStatus() {
    this.onTreeFilter('status', this.selectedStatus);
  }

  onFilterCurrency() {
    this.onTreeFilter('currencyId', this.selectedCurrencyId);
  }

  onFilterListOutcomeType() {
      if (this.outcomingEntryTypeIds.length === 0) {
        this.onTreeFilter('outcomingEntryTypeIds', OPTION_ALL)
      }
      else {
        this.onTreeFilter('outcomingEntryTypeIds', this.outcomingEntryTypeIds)
      }
    }

  onFilterExpenseType() {
    this.onTreeFilter('expenseType', this.expenseType);
  }

  onFilterRequester() {
    this.onTreeFilter('requesters', this.selectedRequester.length ? this.selectedRequester : OPTION_ALL);
  }

  onFilterBranch() {
    this.onTreeFilter('branchs', this.selectedBranch.length ? this.selectedBranch : OPTION_ALL);
  }

  async onFilterMoney() {
    await this.onResetFilter()
    this.searchId = null
    this.searchText = ""
    this.onTreeFilter('money', this.searchMoney);
  }

  async onSearch() {
  this.searchId = null;
  this.searchMoney = null;
  await this.onResetFilter();
  this.setFilterToUrl('searchText', this.searchText);
  this.loadTreeByFilter();
  }

  async onFilterId() {
    await this.onResetFilter()
    this.searchMoney = null
    this.searchText = ""
    this.onTreeFilter('searchId', this.searchId)
  }

  onFilterYCTDStatus() {
    this.onTreeFilter('statusRequestChange', this.selectedStatusYCTD);
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

  onCancelFilterListOutcomeType() {
      this.outcomingEntryTypeIds = []
      this.onTreeFilter('outcomingEntryTypeIds', OPTION_ALL)
      this.treeInOutType.onClearSelected();
    }


  selectionRequesterOpenChange(isOpen: boolean) {
    if (isOpen) {
      this.searchRequesterChange();
      return;
    }
    this.requesterOptions = this.tempRequesterOptions;
  }

  searchRequesterChange() {
    this.requesterOptions = this.tempRequesterOptions.filter((s) =>
      s.name
        .toLocaleLowerCase()
        .includes(this.searchRequester.toLocaleLowerCase())
    );
  }

  selectionBranchOpenChange(isOpen: boolean) {
    if (isOpen) {
      this.searchBranchChange();
      return;
    }
    this.branchOptions = this.tempBranchOptions;
  }

  searchBranchChange() {
    this.branchOptions = this.tempBranchOptions.filter((s) =>
      s.name.toLocaleLowerCase().includes(this.searchBranch.toLocaleLowerCase())
    );
  }

  onTreeFilter(filterName: string, value: any) {
    this.setFilterToUrl(filterName, value);
    this.loadTreeByFilter();
  }

  loadTreeByFilter() {
    const req = this.buildTreeRequestFromFilters();
    this.service.getAllByFilter(req).subscribe(res => {
      this.treeData = this.mapToTree(res.result);
      this.resetVersion++;
    });
  }

  mapToTree(arr: expenditureDto[]): ReportTreeOutcomingEntries[] {
    const map = new Map<number, ReportTreeOutcomingEntries>();
    const roots: ReportTreeOutcomingEntries[] = [];

    arr.forEach(x => {
      map.set(x.id, { item: x, children: [], paddingLevel: '' });
    });

    arr.forEach(x => {
      const node = map.get(x.id);
      if (!x.parentId) roots.push(node);
      else map.get(x.parentId)?.children.push(node);
    });

    return roots;
  }

  buildTreeRequestFromFilters(): GetAllPagingOutComingEntryDto {
    const req = new GetAllPagingOutComingEntryDto();
    const filterItems: FilterDto[] = [];

    req.searchText = this.searchText;

    if (this.selectedCurrencyId !== OPTION_ALL) {
      filterItems.push({
        comparision: 0,
        propertyName: "currencyId",
        value: this.selectedCurrencyId,
      });
      req.currencyId = this.selectedCurrencyId;
    }

    req.outComingStatusCode = this.selectedStatus;
    req.tempStatusCode = this.selectedStatusYCTD;
    req.accreditation = this.accreditation;
    req.money = this.searchMoney || undefined;
    req.branchs = this.selectedBranch;
    req.requesters = this.selectedRequester;
    req.id = this.searchId;
    req.outComingEntryTypeIds = this.outcomingEntryTypeIds;

    if (this.expenseType !== OPTION_ALL) {
      req.expenseType = this.expenseType;
    }

    if (
      this.searchWithDateTime?.dateType !== undefined &&
      this.searchWithDateTime.dateType !== DateSelectorEnum.ALL
    ) {
      req.filterDateTimeParam = {
        dateTimeType: 1,
        fromDate: moment(this.searchWithDateTime.fromDate).format('YYYY-MM-DD'),
        toDate: moment(this.searchWithDateTime.toDate).format('YYYY-MM-DD')
      };
    }

    req.filterItems = filterItems;
    return req;
  }

  async OnResetSearch() {
    this.searchId = null;
    this.searchMoney = null
    this.searchText = ""
  }

  async onResetFilter() {
      this.selectedCurrencyId = OPTION_ALL
      this.outcomingEntryTypeIds = []
      this.expenseType = OPTION_ALL
      this.selectedStatus = ""
      this.selectedStatusYCTD = ""
      this.accreditation = false
      this.selectedBranch = []
      this.selectedRequester = []
  
      this.searchWithDateTime = {
        dateType: DateSelectorEnum.ALL
      } as DateTimeSelector;
  
      this.defaultDateFilterType = DateSelectorEnum.ALL;
      this.resetQueryParams(['currencyId', 'outcomingEntryTypeIds', 'expenseType', 'searchId', 'money', 'status', 'statusRequestChange', 'accreditation', 'branchs', 'requesters', 'dateFilter'])
    }

    resetQueryParams(keys: string[]) {
      const queryParams = {};

      keys.forEach(k => {
        queryParams[k] = null;
      });

      this.router.navigate([], {
        queryParamsHandling: 'merge',
        replaceUrl: true,
        queryParams
      });
    }
    

  getStatusForSearch() {
    this.statusService
      .GetAllForDropDownAndNotEqualsEnd()
      .subscribe((data) => {
        this.statusList = data.result 
        this.statusList.push({name: 'Chờ CEO duyệt cả YCTĐ', code: 'PENDINGCEO_OR_YCTDPENDINGCEO', id: 2})
      });
  }
  getAllRequester() {
    this.expenditureRequest.getAllRequester().subscribe((data) => {
      this.tempRequesterOptions = this.requesterOptions = data.result;
    });
  }
  getAllBranch() {
    this.branchService
      .GetAllForDropdown()
      .subscribe(
        (data) => (this.tempBranchOptions = this.branchOptions = data.result)
      );
  }
  getTempOutComingEntryStatusOptions() {
    this.statusService
      .GetStatusForOutcomeFilter()
      .subscribe((data) => (this.YCTDStatusOptions = data.result));
  }
  getAllCurrency() {
      this.commonService.getAllCurrency().subscribe((data) => {
        this.listCurrency = data.result;
        const itemAll = { value: OPTION_ALL, name: "All" };
        this.listCurrency.unshift(itemAll);
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

  applyUrlFilters() {
      var querySnapshot = this.route.snapshot.queryParams
      this.searchText = querySnapshot['searchText'] ? querySnapshot['searchText'] : "";
      this.selectedCurrencyId = querySnapshot['currencyId'] ? Utils.toNumber(querySnapshot['currencyId']) : OPTION_ALL;
      this.outcomingEntryTypeIds = querySnapshot['outcomingEntryTypeIds'] ? JSON.parse(querySnapshot['outcomingEntryTypeIds']) : [];
      this.expenseType = querySnapshot['expenseType'] ? Utils.toNumber(querySnapshot['expenseType']) : OPTION_ALL;
      this.searchId = querySnapshot['searchId'] ? Utils.toNumber(querySnapshot['searchId']) : null;
      this.searchMoney = querySnapshot['money'] ? Utils.toNumber(querySnapshot['money']) : null;
      this.selectedStatus = querySnapshot['status'] ? querySnapshot['status'] : "";
      this.selectedStatusYCTD = querySnapshot['statusRequestChange'] ? querySnapshot['statusRequestChange'] : "";
      this.accreditation = querySnapshot['accreditation'] ? querySnapshot['accreditation'] === 'true' : false;
      this.selectedBranch = querySnapshot['branchs'] ? JSON.parse(querySnapshot['branchs']) : [];
      this.selectedRequester = querySnapshot['requesters'] ? JSON.parse(querySnapshot['requesters']) : [];
      let dateFilterParam = querySnapshot['dateFilter'] ? JSON.parse(querySnapshot['dateFilter']) : {} as DateTimeSelector;
  
      if (dateFilterParam.dateType) {
        this.searchWithDateTime = dateFilterParam
        this.searchWithDateTime.fromDate = moment(this.searchWithDateTime.fromDate)
        this.searchWithDateTime.toDate = moment(this.searchWithDateTime.toDate)
        this.defaultDateFilterType = this.searchWithDateTime.dateType
      } else {
        this.searchWithDateTime = {
          dateType: DateSelectorEnum.ALL,
        } as DateTimeSelector;
      }
    }

}

export class GetAllPagingOutComingEntryDto extends PagedRequestDto {
    currencyId?: number;
    money: number;
    requesters: number[];
    branchs: number[];
    outComingEntryTypeIds: number[];
    tempStatusCode: string;
    outComingStatusCode: string;
    accreditation: boolean;
    id: number;
    expenseType: number;
    filterDateTimeParam: {
      dateTimeType: number;
      fromDate: string;
      toDate: string;
    };
  }
  export class GetWorkflowStatusDto {
    name: string;
    code: string;
  }
  export interface TreeOutcomingEntries {
    item: OptionOutcomingEntriesDto;
    children: TreeOutcomingEntries[];
    paddingLevel: string;
  }
  export interface OptionOutcomingEntriesDto {
  name: string;
  id: number;
  parentId: number;
  level: number;
 }

 export interface ReportTreeOutcomingEntries {
  item: expenditureDto;
  children: ReportTreeOutcomingEntries[];
  paddingLevel: string;
}

 export const OPTION_ALL: number = -1

 export class expenditureDto {
  name: string;
  code: string;
  pathId: string;
  pathName: string;
  level: number;
  parentId: number;
  workflowId: number;
  id: number;
  expenseType: number;
  isActive: boolean;
  totalCurrencies?: {
    currencyId: number;
    currencyName: string;
    value: number;
    valueFormat: string;
  }[];
  vndConvert?: number;
}
export class expenditureListDto {
  name: string;
  children: expenditureDto[];
}

export class OutcomingEntryDto {
  id: number;
  outcomingEntryTypeId: number;
  name: string;
  requester: string;
  branchId: number;
  branchName: string;
  accountId: number;
  accountName: string;
  currencyId: number;
  currencyName: string;
  value: number;
  expenseType?: number;
  workflowStatusId: number;
  workflowStatusName: string;
  workflowStatusCode: string;
  action: ActionDto[];
  outcomingEntryTypeCode: string;
  supplierId: number;
  createdAt: Time;
  sendTime: Time;
  approveTime: Time;
  executeTime: Time;
  isAcceptFile: number;
  paymentCode: string;
  creatorUserId: number;
  requestInBankTransaction: number;
  accreditation: true;
  updatedBy: string;
  updatedTime: Time;
  creationTime: Time;
  creationUserId: number;
  creationUser: string;
  lastModifiedTime: Time;
  lastModifiedUserId: number;
  lastModifiedUser: string;
  statusHistories: StatusHistory[];
  tempOutcomingEntryId: number;
  reportDate?: number;
}

export class ActionDto {
  fromStatusId: number;
  name: string;
  statusTransitionId: string;
  toStatusId: number;
  workflowId: number;
}

export class ResultGetOutcomingEntryDto {
  resultPaging: {
    totalCount: number;
    items: OutcomingEntryDto[];
  };
}
