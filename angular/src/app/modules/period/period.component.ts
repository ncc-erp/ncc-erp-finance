import { Component, Injector, OnInit } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { PERMISSIONS_CONSTANT } from '@app/constant/permission.constant';
import { AppConsts } from '@shared/AppConsts';
import { PagedListingComponentBase, PagedRequestDto } from '@shared/paged-listing-component-base';
import { PeriodService } from '../../service/api/period.service';
import { PeriodDto } from '../../service/model/period.dto'
import { PAGE_SIZE_OPTIONS } from '../revenue-managed/revenue-managed.component';
import { CloseAndCreatePeriodComponent } from './close-and-create-period/close-and-create-period.component';
import { CreateEditPeriodComponent } from './create-edit-period/create-edit-period.component';
import { TranslateService } from '@ngx-translate/core';
import { HttpParams } from '@angular/common/http';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-period',
  templateUrl: './period.component.html',
  styleUrls: ['./period.component.css']
})
export class PeriodComponent extends PagedListingComponentBase<PeriodDto> implements OnInit {
  constructor(
    private route: ActivatedRoute,
    injector: Injector,
    private periodService : PeriodService,
    private dialog: MatDialog,
    private translate: TranslateService) {
    super(injector);
  }
  public listPeriods: PeriodDto[] = [];
  public isTheFirstCreate :boolean = false;
  routeUrlFirstLevel = this.APP_CONSTANT.UrlBreadcrumbFirstLevel.Menu5;
  routeUrlSecondLevel = this.APP_CONSTANT.UrlBreadcrumbSecondLevel.period;
  queryParams;
  protected list(request: PagedRequestDto, pageNumber: number, finishedCallback: Function): void {
    this.isTableLoading = true;
    this.periodService.getAllPaging(request).subscribe((rs)=>{
      this.listPeriods = rs.result.items;
      this.showPaging(rs.result, pageNumber);
      this.isTableLoading = false;
    },()=> this.isTableLoading = false)
  }
  protected delete(entity: any): void {
    throw new Error('Method not implemented.');
  }


  public readonly LIST_PAGE_SIZE_OPTIONS = PAGE_SIZE_OPTIONS;
  ngOnInit(): void {
    this.refresh();
    this.isTheFirstRecord();
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
    this.translate.get("menu5.m5_child9").subscribe((res: string) => {
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

  private onOpenDialog(period : PeriodDto, title: string, isEditting){
    const dl = this.dialog.open(CreateEditPeriodComponent,{
      data: {
        period: {...period},
        title: title,
        isEditting: isEditting

      },
      width: "800px"
    })

    dl.afterClosed().subscribe((rs)=>{
      if(rs){
        this.refresh();
        this.isTheFirstRecord();
        AppConsts.periodId.next(-1)
      }
    })
  }

  public onCreate(){
    this.onOpenDialog(null, 'Tạo kì mới lần đầu tiên', false);
  }
  public onUpdate(period: PeriodDto){
    this.onOpenDialog(period,"Sửa kì", true);
  }
  public onCloseAndCreate(){
    const dl = this.dialog.open(CloseAndCreatePeriodComponent, {
      width: "1200px"
    })
    dl.afterClosed().subscribe((rs)=>{
      if(rs){
        this.refresh();
        AppConsts.periodId.next(-1)
      }
    })
  }

  public isTheFirstRecord(){
    this.periodService.isTheFirstRecord().subscribe((rs)=>{
      this.isTheFirstCreate = rs.result;
    })
  }

  public isShowCreateBtn(){
    return this.permission.isGranted(PERMISSIONS_CONSTANT.Finance_Period_Create) && this.isTheFirstCreate;
  }
  public isShowCloseAndCreate(){
    return this.permission.isGranted(PERMISSIONS_CONSTANT.Finance_Period_CloseAndCreate);
  }
  public isShowEditBtn(){
    return this.permission.isGranted(PERMISSIONS_CONSTANT.Finance_Period_Edit);
  }

}
