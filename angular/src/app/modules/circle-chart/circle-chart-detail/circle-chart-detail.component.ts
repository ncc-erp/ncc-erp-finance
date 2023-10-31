import { Component, Injector, OnInit } from '@angular/core';

import { MatDialog } from '@angular/material/dialog';
import { ActivatedRoute, Router } from '@angular/router';
import { PERMISSIONS_CONSTANT } from '@app/constant/permission.constant';
import { CircleChartInfoDto } from '@app/service/model/circle-chart.dto';
import { AppComponentBase } from '@shared/app-component-base';
import { CircleChartDetailService } from '../../../service/api/circle-chart-detail.service';
import { CreateEditCircleChartDetailComponent } from './create-edit-circle-chart-detail/create-edit-circle-chart-detail.component';


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
  constructor(
    injector: Injector, 
    private circleChartDetailService: CircleChartDetailService, 
    private dialog: MatDialog, 
    private route: ActivatedRoute) {
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
      this.isIncome = item.result.isIncome;
      this.listBreadCrumb = [
        {name: '<i class="fas fa-home fa-l"></i>',url:''}, 
        {name: ' <i class="fas fa-chevron-right"></i> '}, 
        {name: 'Circle Chart', url: '/app/circleChart'}, 
        {name: ' <i class="fas fa-chevron-right"></i> '}, 
        {name: this.circleChartInfo.name }];
      this.circleChartTypeName = this.circleChartInfo.circleChartTypeName;
      this.isActive = this.circleChartInfo.isActive;
    })
  }
  public onCreate() {
    let ref = this.dialog.open(CreateEditCircleChartDetailComponent, {
      width: "540px",
      disableClose: true
    });

    ref.afterClosed().subscribe((rs) => {
      this.refresh(this.paramId);
    });
  }

  public onEdit(setting: CircleChartInfoDto) {
    let item = { ...setting };
    let ref = this.dialog.open(CreateEditCircleChartDetailComponent, {
      width: "70vw",
      data: {
        item: item,
        isIncome: this.isIncome,
      },
      disableClose: true,
      
    });

    ref.afterClosed().subscribe((rs) => {
      this.refresh(this.paramId);
    });
  }

  protected delete(entity: CircleChartInfoDto): void {
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
  isShowCreateBtn(){
    return this.isGranted(PERMISSIONS_CONSTANT.Admin_CircleChart_CircleChartDetail_Create);
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
