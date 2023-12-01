import { ValueAndNameModel } from './../../../../service/model/common-DTO';
import { AppConsts } from './../../../../../shared/AppConsts';
import { BTransactionLogService } from './../../../../service/api/new-versions/b-transaction-log.service';
import { Component, Injector, OnInit } from '@angular/core';
import { FilterDto, PagedListingComponentBase, PagedRequestDto } from '@shared/paged-listing-component-base';
import { BTransactionLog } from '../../../../service/model/b-transaction-log.model';
import { Utils } from '@app/service/helpers/utils';
import { EComparisor } from '@app/modules/revenue-managed/revenue-managed.component';
import { DateSelectorEnum } from '@shared/AppEnums';
import { DateFormat, DateTimeSelector } from '@shared/date-selector/date-selector.component';
import { IFilterDateTimeParam } from '@app/service/interfaces/filter-date.interface';
import { TranslateService } from '@ngx-translate/core';
import { HttpParams } from '@angular/common/http';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-list-b-transaction-log',
  templateUrl: './list-b-transaction-log.component.html',
  styleUrls: ['./list-b-transaction-log.component.css']
})
export class ListBTransactionLogComponent extends PagedListingComponentBase<BTransactionLog> implements OnInit {
  public bTransactionLogs: BTransactionLog[] = [];
  routeUrlFirstLevel = this.APP_CONSTANT.UrlBreadcrumbFirstLevel.Menu2;
  routeUrlSecondLevel = this.APP_CONSTANT.UrlBreadcrumbSecondLevel.bTransactionLog;
  queryParams;
  searchDetail = {
    isValid: AppConsts.VALUE_OPTIONS_ALL
  };

  logStatuses: ValueAndNameModel[] = [
    {value: AppConsts.VALUE_OPTIONS_ALL, name: 'All'},
    {value: true, name: 'Thành công'},
    {value: false, name: 'Lỗi'},
  ];

  defaultDateFilterType = DateSelectorEnum.ALL;
  searchWithDateTime: DateTimeSelector;

  constructor(private route: ActivatedRoute, injector: Injector, private _bTransactionLogService: BTransactionLogService, private translate: TranslateService) {
    super(injector);
  }

  ngOnInit(): void {
    this.getFirstPage();
    this.translate.onLangChange.subscribe(() => {
      this.onLangChange();
    });
    this.route.queryParams.subscribe(params => {
      this.queryParams = new HttpParams({ fromObject: params });
      this.onLangChange();
    });
  }
  
  onLangChange(){
    this.translate.get("menu.menu2").subscribe((res: string) => {
      this.routeTitleFirstLevel = res;
      this.updateBreadCrumb();
    });
    this.translate.get("menu2.bTransactionLog").subscribe((res: string) => {
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

  onDateChange($event): void {
    this.searchWithDateTime = $event;
    this.getFirstPage();
  }

  protected list(request: PagedRequestDto, pageNumber: number, finishedCallback: Function): void {
    const payload = this.getRequestParams(request);
    this._bTransactionLogService.getAllPaging(payload).subscribe(response => {
      if (!response.success) return;

      this.bTransactionLogs = response.result.items;
      this.totalItems = response.result.totalCount;
      this.isTableLoading = false;
    })
  }
  private getRequestParams(request: PagedRequestDto): BTransactionLogPagedRequestDto {
    const payload = {...request} as BTransactionLogPagedRequestDto;
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
    return payload;
  }
  protected delete(entity: BTransactionLog): void {
    throw new Error('Method not implemented.');
  }

  getColor(isValid: boolean): string{
    if(isValid) return 'bg-success';

    return 'bg-danger';
  }
}

export class BTransactionLogPagedRequestDto extends PagedRequestDto {
  filterDateTimeParam: IFilterDateTimeParam;
}
