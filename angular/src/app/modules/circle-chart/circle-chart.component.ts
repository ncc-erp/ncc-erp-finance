import { MatDialog } from "@angular/material/dialog";
import { Injector } from "@angular/core";
import { CircleChartDto } from "../../service/model/circle-chart.dto";
import { CircleChartService } from "./../../service/api/circle-chart.service";
import { Component, OnInit } from "@angular/core";
import {
  PagedListingComponentBase,
  PagedRequestDto,
} from "@shared/paged-listing-component-base";
import { finalize } from "rxjs/operators";
import { CreateEditCircleChartComponent } from "./create-edit-circle-chart/create-edit-circle-chart.component";
import { PERMISSIONS_CONSTANT } from "@app/constant/permission.constant";

@Component({
  selector: 'app-circle-chart',
  templateUrl: './circle-chart.component.html',
  styleUrls: ['./circle-chart.component.css']
})
export class CircleChartComponent 
extends PagedListingComponentBase<CircleChartDto>
implements OnInit 
{
  Admin_CircleChart_CircleChartDetail_View = PERMISSIONS_CONSTANT.Admin_CircleChart_CircleChartDetail_View
  protected list(
    request: PagedRequestDto,
    pageNumber: number,
    finishedCallback: Function
  ): void {
    this.isTableLoading = true;
    this.circleChartService
      .getAllPaging(request)
      .pipe(
        finalize(() => {
          finishedCallback();
        })
      )
      .subscribe(
        (rs) => {
          this.circleChartDto = rs.result.items;
          this.showPaging(rs.result, pageNumber);
          this.isTableLoading = false;
        },
        () => (this.isTableLoading = false)
      );
    this.listBreadCrumb = [
      {name: '<i class="fas fa-home"></i>',url:''}, 
      {name: ' <i class="fas fa-chevron-right"></i> '}, 
      {name: 'Circle Chart' }];
  }
  protected delete(entity: CircleChartDto): void {
    abp.message.confirm(`Delete chart: ${entity.name}`, "", (rs) => {
      if(rs){
        this.circleChartService.delete(entity.id).subscribe((rs) => {
          if (rs) {
            abp.notify.success(`Deleted chart ${entity.name}`);
            this.refresh();
          }
        });
      }
    });
  }

  showDetail(id) {
    this.router.navigate(["/app/circleChartDetail"], {
      queryParams: {
        id: id,
      },
    });
  }

  public circleChartDto: CircleChartDto[] = [];

  constructor(
    injector: Injector,
    private dialog: MatDialog,
    private circleChartService: CircleChartService
  ) {
    super(injector);
  }

  ngOnInit(): void {
    this.refresh();
  }

  public onCreate() {
    let ref = this.dialog.open(CreateEditCircleChartComponent, {
      width: "540px",
      disableClose: true,
    });

    ref.afterClosed().subscribe((rs) => {
      this.refresh();
    });
  }

  public onEdit(setting: CircleChartDto) {
    let item = { ...setting };
    let ref = this.dialog.open(CreateEditCircleChartComponent, {
      width: "540px",
      data: item,
      disableClose: true,
    });

    ref.afterClosed().subscribe((rs) => {
      this.refresh();
    });
  }

  public onActive(chart: CircleChartDto) {
    this.circleChartService.Active(chart.id).subscribe((rs) => {
      abp.notify.success(`Active chart ${chart.name}`);
      this.refresh();
    });
  }

  public onDeActive(chart: CircleChartDto) {
    this.circleChartService.DeActive(chart.id).subscribe((rs) => {
      abp.notify.success(`DeActive chart ${chart.name}`);
      this.refresh();
    });
  }
  isShowCreateBtn(){
    return this.isGranted(PERMISSIONS_CONSTANT.Admin_CircleChart_Create);
  }
  isShowEditBtn(){
    return this.isGranted(PERMISSIONS_CONSTANT.Admin_CircleChart_Edit);
  }
  isShowDeleteBtn(){
    return this.isGranted(PERMISSIONS_CONSTANT.Admin_CircleChart_Delete);
  }
  isShowActiveDecativeActionBtns(){
    return this.isGranted(PERMISSIONS_CONSTANT.Admin_CircleChart_ActiveDeactive);
  }
  isShowActionMenu(){
    return this.isShowEditBtn() || this.isShowDeleteBtn() || this.isShowActiveDecativeActionBtns();
  }
}


