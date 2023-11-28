import { Component, Injector, OnInit } from '@angular/core';

import { MatDialog } from '@angular/material/dialog';
import { ActivatedRoute, Router } from '@angular/router';
import { PERMISSIONS_CONSTANT } from '@app/constant/permission.constant';
import { CircleChartDetailInfoDto, CircleChartInfoDto } from '@app/service/model/circle-chart.dto';
import { AppComponentBase } from '@shared/app-component-base';
import { CircleChartDetailService } from '../../../service/api/circle-chart-detail.service';
import { CreateEditCircleChartDetailComponent } from './create-edit-circle-chart-detail/create-edit-circle-chart-detail.component';
import { RevenueExpenseType } from '@shared/AppEnums';
import { TranslateService } from '@ngx-translate/core';


@Component({
  selector: 'app-circle-chart-detail',
  templateUrl: './circle-chart-detail.component.html',
  styleUrls: ['./circle-chart-detail.component.css']
})
export class CircleChartDetailComponent extends AppComponentBase implements OnInit {

  router: Router;
  searchText: string = '';
  circleChartTypeName: string = '';
  circleChartInfo: CircleChartInfoDto;
  isIncome: boolean;
  isActive: boolean;
  title: any;
  constructor(
    injector: Injector, 
    private circleChartDetailService: CircleChartDetailService, 
    private dialog: MatDialog, 
    private route: ActivatedRoute,
    private translate: TranslateService
    ) {
    super(injector);
  }
  paramId;
  ngOnInit(): void {
    this.paramId = this.route.snapshot.queryParamMap.get('id');
    this.refresh(this.paramId);
  }

  refresh(id): void {
    this.circleChartDetailService.GetCircleChartDetailsByChartId(id).subscribe((item) => {
      this.circleChartInfo = item.result;
      this.circleChartInfo.details?.forEach(item => {
        item.hideClient = false;
        item.hideEntryType = false;
      })
      this.isIncome = item.result.isIncome;
      // this.listBreadCrumb = [
      //   {name: '<i class="fas fa-home fa-l"></i>',url:''}, 
      //   {name: ' <i class="fas fa-chevron-right"></i> '}, 
      //   {name: 'Circle Chart', url: '/app/circleChart'}, 
      //   {name: ' <i class="fas fa-chevron-right"></i> '}, 
      //   {name: this.circleChartInfo.name }];
      this.circleChartTypeName = this.circleChartInfo.circleChartTypeName;
      this.isActive = this.circleChartInfo.isActive;
      this.translate.onLangChange.subscribe(() => {
        this.onLangChange();
      });
      this.onLangChange();
    })
  }

  onLangChange(){
    this.translate.get("menu2.circleChart").subscribe((res: string) => {
      this.title = res;
      this.updateBreadCrumb();
    });
  }
  updateBreadCrumb() {
    this.listBreadCrumb = [
      { name: '<i class="fas fa-home"></i>', url: '' },
      { name: ' <i class="fas fa-chevron-right"></i> ' },
      { name: this.title, url: '/app/circleChart' },
      {name: ' <i class="fas fa-chevron-right"></i> '}, 
      {name: this.circleChartInfo.name }];
  }
  public onCreate() {
    let ref = this.dialog.open(CreateEditCircleChartDetailComponent, {
      width: "540px",
      data: {
        isIncome: this.isIncome,
      },
      disableClose: true
    });

    ref.afterClosed().subscribe((rs) => {
      this.refresh(this.paramId);
    });
  }

  public onEdit(setting: CircleChartDetailInfoDto) {
    let item = { ...setting };
    let ref = this.dialog.open(CreateEditCircleChartDetailComponent, {
      width: "70vw",
      data: {
        item: item,
        isIncome: this.isIncome,
        isViewOnly: false
      },
      disableClose: true,
      
    });

    ref.afterClosed().subscribe((rs) => {
      this.refresh(this.paramId);
    });
  }

  public onView(setting: CircleChartDetailInfoDto) {
    let item = { ...setting };
    let ref = this.dialog.open(CreateEditCircleChartDetailComponent, {
      width: "70vw",
      data: {
        item: item,
        isIncome: this.isIncome,
        isViewOnly: true
      },
      disableClose: true,
      
    });

    ref.afterClosed().subscribe((rs) => {
      this.refresh(this.paramId);
    });
  }

  protected delete(entity: CircleChartDetailInfoDto): void {
    abp.message.confirm(`Delete detail: ${entity.name}`, "", (rs) => {
      if(rs){
        this.circleChartDetailService.delete(entity.id).subscribe((rs) => {
          if (rs) {
            abp.notify.success(`Deleted detail ${entity.name}`);
            this.refresh(this.paramId);
          }
        });
      }
    });
  }

  toggleEntryType(item){
    item.hideEntryType = !item.hideEntryType;
    console.log(item.hideEntryType)
  }

  toggleClient(item){
    item.hideClient = !item.hideClient;
    console.log(item.hideClient)
  }

  getRevenueExpenseTypeText(revenueExpenseType: RevenueExpenseType): string {
    if (this.isIncome){
      switch (revenueExpenseType) {
        case RevenueExpenseType.REAL_REVENUE_EXPENSE:
          return 'Thu thực';
        case RevenueExpenseType.NON_REVENUE_EXPENSE:
          return 'Thu không thực';
        case RevenueExpenseType.ALL_REVENUE_EXPENSE:
          return 'Không phân biệt';
        default:
          return '';
      }
    } else{
      switch (revenueExpenseType) {
        case RevenueExpenseType.REAL_REVENUE_EXPENSE:
          return 'Chi thực';
        case RevenueExpenseType.NON_REVENUE_EXPENSE:
          return 'Chi không thực';
        case RevenueExpenseType.ALL_REVENUE_EXPENSE:
          return 'Không phân biệt';
        default:
          return '';
      }
    }
  }

  isShowCreateBtn(){
    return this.isGranted(PERMISSIONS_CONSTANT.Admin_CircleChart_CircleChartDetail_Create);
  }
  isShowViewBtn(){
    return this.isGranted(PERMISSIONS_CONSTANT.Admin_CircleChart_CircleChartDetail_View);
  }
  isShowEditBtn(){
    return this.isGranted(PERMISSIONS_CONSTANT.Admin_CircleChart_CircleChartDetail_Edit);
  }
  isShowDeleteBtn(){
    return this.isGranted(PERMISSIONS_CONSTANT.Admin_CircleChart_CircleChartDetail_Delete);
  }
  isShowActionMenu(){
    return this.isShowEditBtn() || this.isShowDeleteBtn();
  }
}
