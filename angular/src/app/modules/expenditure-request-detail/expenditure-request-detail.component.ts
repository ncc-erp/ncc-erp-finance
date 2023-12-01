import { AppComponentBase } from 'shared/app-component-base';
import { ActivatedRoute, Router } from '@angular/router';
import { Component, OnInit, Injector } from '@angular/core';
import { PERMISSIONS_CONSTANT } from '@app/constant/permission.constant';
import { TranslateService } from '@ngx-translate/core';
import { ExpenditureRequestService } from '@app/service/api/expenditure-request.service';
import { HttpParams } from '@angular/common/http';

@Component({
  selector: 'app-expenditure-request-detail',
  templateUrl: './expenditure-request-detail.component.html',
  styleUrls: ['./expenditure-request-detail.component.css']
})
export class ExpenditureRequestDetailComponent extends AppComponentBase implements OnInit {

  requestId: any
  currentUrl: string = ""
  requestDetail: any;
  title: any;
  routeTitleFirstLevel;
  routeUrlFirstLevel = this.APP_CONSTANT.UrlBreadcrumbFirstLevel.Menu5;
  routeUrlSecondLevel = this.APP_CONSTANT.UrlBreadcrumbSecondLevel.expenditureRequest;
  routeUrlThirdLevel = this.APP_CONSTANT.UrlBreadcrumbThirdLevel.expenditureRequestDetail;
  queryParams;
  constructor(private route: ActivatedRoute, private router: Router, injector:Injector, private translate: TranslateService, private requestService: ExpenditureRequestService,) {
    super(injector)
  }
  ngOnInit(): void {
    this.requestId = this.route.snapshot.queryParamMap.get("id")

    this.router.events.subscribe(res => {
      this.requestId = this.route.snapshot.queryParamMap.get("id")
      this.currentUrl = this.router.url
    });
    this.translate.onLangChange.subscribe(() => {
      this.onLangChange();
    });
    this.getRequestById();
  }
  
  onLangChange(){
    this.translate.get("menu.menu5").subscribe((res: string) => {
      this.routeTitleFirstLevel = res;
      this.updateBreadCrumb();
    });
    this.translate.get("menu5.m5_child2").subscribe((res: string) => {
      this.title = res;
      this.updateBreadCrumb();
    });
  }

  updateBreadCrumb() {
    let queryParamsString = this.queryParams.toString();
    this.listBreadCrumb = [
      { name: this.routeTitleFirstLevel , url: this.routeUrlFirstLevel },
      { name: ' <i class="fas fa-chevron-right"></i> ' },
      { name: this.title, url: this.routeUrlSecondLevel },
      { name: ' <i class="fas fa-chevron-right"></i> ' },
      { name: this.requestDetail.name , url: this.routeUrlThirdLevel + (queryParamsString ? '?' + queryParamsString : '') },
    ];
  }
  getRequestById() {
    this.requestService.getById(this.requestId).subscribe(data => {
      this.requestDetail = data.result;
      this.route.queryParams.subscribe(params => {
        this.queryParams = new HttpParams({ fromObject: params });
        this.onLangChange();
      });
    })
  }

  routingMainTab() {
    this.router.navigate(['main'], {
      relativeTo: this.route, queryParams: {
        id: this.requestId
      },
      // replaceUrl: true
    })
  }
  routingDetailTab() {
    this.router.navigate(['detail'], {
      relativeTo: this.route, queryParams: {
        id: this.requestId
      },
      // replaceUrl: true
    })
  }
  routingRelevantTab() {
    this.router.navigate(['relevant-transaction'], {
      relativeTo: this.route, queryParams: {
        id: this.requestId
      },
      // replaceUrl: true
    })
  }
  routingRevenuesTab() {
    this.router.navigate(['revenues-transaction'], {
      relativeTo: this.route, queryParams: {
        id: this.requestId
      },
      // replaceUrl: true
    })
  }
  routingSupplierTab(){
    this.router.navigate(['supplier'], {
      relativeTo: this.route, queryParams: {
        id: this.requestId
      },
      // replaceUrl: true
    })
  }

  isShowMainTab(){
    return this.permission.isGranted(PERMISSIONS_CONSTANT.Finance_OutcomingEntry_OutcomingEntryDetail_TabGeneral);
  }

  isShowDetailTab(){
    return this.permission.isGranted(PERMISSIONS_CONSTANT.Finance_OutcomingEntry_OutcomingEntryDetail_TabDetailInfo);
  }

  isShowSuplierTab(){
    return this.permission.isGranted(PERMISSIONS_CONSTANT.Finance_OutcomingEntry_OutcomingEntryDetail_TabSupplier);
  }

  isShowRelevantBankTab(){
    return this.permission.isGranted(PERMISSIONS_CONSTANT.Finance_OutcomingEntry_OutcomingEntryDetail_TabRelatedBankTransaction);
  }

  isShowRevenuesTab(){
    return this.permission.isGranted(PERMISSIONS_CONSTANT.Finance_OutcomingEntry_OutcomingEntryDetail_TabRelatedIncomingEntrySource);
  }
}

