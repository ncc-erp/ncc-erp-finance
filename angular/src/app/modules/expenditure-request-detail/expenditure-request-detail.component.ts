import { AppComponentBase } from 'shared/app-component-base';
import { ActivatedRoute, Router } from '@angular/router';
import { Component, OnInit, Injector } from '@angular/core';
import { PERMISSIONS_CONSTANT } from '@app/constant/permission.constant';

@Component({
  selector: 'app-expenditure-request-detail',
  templateUrl: './expenditure-request-detail.component.html',
  styleUrls: ['./expenditure-request-detail.component.css']
})
export class ExpenditureRequestDetailComponent extends AppComponentBase implements OnInit {

  requestId: any
  currentUrl: string = ""
  constructor(private route: ActivatedRoute, private router: Router, injector:Injector) {
    super(injector)
  }
  ngOnInit(): void {
    this.requestId = this.route.snapshot.queryParamMap.get("id")

    this.router.events.subscribe(res => {
      this.requestId = this.route.snapshot.queryParamMap.get("id")
      this.currentUrl = this.router.url
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

