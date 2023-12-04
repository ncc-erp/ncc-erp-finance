import { PERMISSIONS_CONSTANT } from '@app/constant/permission.constant';
import { FilterDto } from './../../../shared/paged-listing-component-base';
import { PagedListingComponentBase, PagedRequestDto, PagedResultResultDto } from '@shared/paged-listing-component-base';
import { finalize, filter } from 'rxjs/operators';
import { AuditLogService } from './../../service/api/auditlog.service';
import { Injector } from '@angular/core';
import { AuditlogDto } from './../../service/model/auditlog.model';
import { Component, OnInit } from '@angular/core';
import * as _ from 'lodash';
import { TranslateService } from '@ngx-translate/core';
import { HttpParams } from '@angular/common/http';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-auditlog',
  templateUrl: './auditlog.component.html',
  styleUrls: ['./auditlog.component.css']
})
export class AuditlogComponent extends PagedListingComponentBase<AuditlogDto> implements OnInit  {
  Admin_Auditlog = PERMISSIONS_CONSTANT.Admin_Auditlog;
  Admin_Auditlog_View = PERMISSIONS_CONSTANT.Admin_Auditlog_View;
  auditlogs = [] as AuditlogDto[];
  emailAddressFilter = [];
  emailAddress = [];
  selecteduserId = "";
  emailAddressSearch = "";
  listFullMethodName = [];
  listMethodNameFilter = [];
  methodNameSearch = "";
  methodNameSelected = "";
  listFullServiceName = [];
  listServiceNameFilter = [];
  serviceNameSearch = "";
  serviceNameSelected = "";

  transactionList: any;
  sortDrirect: number = 0;
  transDate: string = "";
  iconSort: string = "";
  iconCondition: string = "executionTime";
  routeUrlFirstLevel = this.APP_CONSTANT.UrlBreadcrumbFirstLevel.admin;
  routeUrlSecondLevel = this.APP_CONSTANT.UrlBreadcrumbSecondLevel.auditLog;
  queryParams;

  constructor(
    private route: ActivatedRoute,
    private auditlog: AuditLogService,
    injector: Injector,
    private translate: TranslateService
  ) {
    super(injector);
  }

  ngOnInit() {
    this.getListEmailAddress();
    this.getListMethodName();
    this.getListServiceName();
    this.refresh();
    this.sortData("executionTime");
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
    this.translate.get("menu2.auditLog").subscribe((res: string) => {
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

  getListEmailAddress() {
    this.auditlog.getEmailAddressInAuditLog().subscribe(data => {
      this.emailAddress  = this.emailAddressFilter = data.result
    })
  }

  getListMethodName() {
    this.auditlog.getAllMethodName().subscribe(data => {
      this.listFullMethodName = this.listMethodNameFilter = data.result
    })
  }

  getListServiceName() {
    this.auditlog.getAllServiceName().subscribe(data => {
      this.listFullServiceName = this.listServiceNameFilter = data.result
    })
  }

  handleSearchEmailAddress() {
    const textSearch = this.emailAddressSearch.toLowerCase().trim();
    if (textSearch) {
      this.emailAddress = this.emailAddressFilter
      .filter(item => item.emailAddress.toLowerCase().trim().includes(textSearch));
    } else {
      this.emailAddress = this.emailAddressFilter;
    }
  }

  handleSearchMethodName() {
    const textSearch = this.methodNameSearch.toLowerCase().trim();
    if (textSearch) {
      this.listMethodNameFilter = this.listFullMethodName.filter(item => item.toLowerCase().trim().includes(textSearch));
    } else {
      this.listMethodNameFilter = this.listFullMethodName;
    }
  }

  handleSearchServiceName() {
    const textSearch = this.serviceNameSearch.toLowerCase().trim();
    if (textSearch) {
      this.listServiceNameFilter = this.listFullServiceName.filter(item => item.toLowerCase().trim().includes(textSearch));
    } else {
      this.listServiceNameFilter = this.listFullServiceName;
    }
  }
  

  protected list(request: AuditLogPagedRequestDto, pageNumber: number, finishedCallback: Function): void {
    let filterItems: FilterDto[] = [];
    request.sort = this.transDate;
    request.sortDirection = this.sortDrirect;
    if(this.selecteduserId &&this.selecteduserId != 'null'){
      filterItems.push({
        comparision: 0,
        propertyName: "userId",
        value: this.selecteduserId
      })
    }
    if(this.selecteduserId == 'null'){
      filterItems.push({
        comparision: 0,
        propertyName: "userId",
        value: null
      })
    }

    request.filterItems = filterItems;
    if (this.searchText) {
      request.searchText = this.searchText;
    }
    request.MethodName = this.methodNameSelected;
    request.ServiceName = this.serviceNameSelected;

    this.auditlog
      .getAllPaging(request)
      .pipe(finalize(() => {
        finishedCallback();
      }))
      .subscribe((result: any) => {
        this.auditlogs = result.result.items;
        for (let i = 0; i < this.auditlogs.length; i++) {
          this.auditlogs[i].hideNote = false;
        }
        this.showPaging(result.result, pageNumber);
      });
  }
  
  changeStatusNote(data) {
    data.hideNote = !data.hideNote;
  }

  protected delete(item: AuditlogDto): void {
  }

  sortData(data) {
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
}

export class AuditLogPagedRequestDto extends PagedRequestDto
{
  MethodName: string;
  ServiceName: string;
}
