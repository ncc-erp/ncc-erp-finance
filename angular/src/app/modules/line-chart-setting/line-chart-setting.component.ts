import { MatDialog } from "@angular/material/dialog";
import { Injector } from "@angular/core";
import { LineChartSettingDto } from "./../../service/model/linechartSetting.dto";
import { LinechartSettingService } from "./../../service/api/linechart-setting.service";
import { Component, OnInit } from "@angular/core";
import {
  PagedListingComponentBase,
  PagedRequestDto,
} from "@shared/paged-listing-component-base";
import { finalize } from "rxjs/operators";
import { CreateEditLineChartSettingComponent } from "./create-edit-line-chart-setting/create-edit-line-chart-setting.component";
import { PERMISSIONS_CONSTANT } from "@app/constant/permission.constant";
import { TranslateService } from "@ngx-translate/core";
import { HttpParams } from "@angular/common/http";
import { ActivatedRoute } from "@angular/router";

@Component({
  selector: "app-line-chart-setting",
  templateUrl: "./line-chart-setting.component.html",
  styleUrls: ["./line-chart-setting.component.css"],
})
export class LineChartSettingComponent
  extends PagedListingComponentBase<LineChartSettingDto>
  implements OnInit
{
  protected list(
    request: PagedRequestDto,
    pageNumber: number,
    finishedCallback: Function,
  ): void {
    this.isTableLoading = true;
    this.linechartsettingService
      .getAllPaging(request)
      .pipe(
        finalize(() => {
          finishedCallback();
        })
      )
      .subscribe(
        (rs) => {
          this.lineChartSetting = rs.result.items;
          this.showPaging(rs.result, pageNumber);
          this.isTableLoading = false;
        },
        () => (this.isTableLoading = false)
      );
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
    this.translate.get("menu2.lineChart").subscribe((res: string) => {
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

  protected delete(entity: LineChartSettingDto): void {
    abp.message.confirm(`Delete setting: ${entity.name}`, "", (rs) => {
      if(rs){
        this.linechartsettingService.delete(entity.id).subscribe((rs) => {
          if (rs) {
            abp.notify.success(`Deleted setting ${entity.name}`);
            this.refresh();
          }
        });
      }
    });
  }

  public lineChartSetting: LineChartSettingDto[] = [];
  routeUrlFirstLevel = this.APP_CONSTANT.UrlBreadcrumbFirstLevel.Menu2;
  routeUrlSecondLevel = this.APP_CONSTANT.UrlBreadcrumbSecondLevel.lineChart;
  queryParams;

  constructor(
    private route: ActivatedRoute,
    injector: Injector,
    private dialog: MatDialog,
    private linechartsettingService: LinechartSettingService,
    private translate: TranslateService
  ) {
    super(injector);
  }

  ngOnInit(): void {
    this.refresh();
  }

  public onCreate() {
    let ref = this.dialog.open(CreateEditLineChartSettingComponent, {
      width: "540px",
      disableClose: true,
    });

    ref.afterClosed().subscribe((rs) => {
      this.refresh();
    });
  }

  public onEdit(setting: LineChartSettingDto) {
    let item = { ...setting };
    let ref = this.dialog.open(CreateEditLineChartSettingComponent, {
      width: "70vw",
      data: item,
      disableClose: true,
    });

    ref.afterClosed().subscribe((rs) => {
      this.refresh();
    });
  }

  public onActive(chart: LineChartSettingDto) {
    this.linechartsettingService.Active(chart.id).subscribe((rs) => {
      abp.notify.success(`Active chart ${chart.name}`);
      this.refresh();
    });
  }

  public onDeActive(chart: LineChartSettingDto) {
    this.linechartsettingService.DeActive(chart.id).subscribe((rs) => {
      abp.notify.success(`DeActive chart ${chart.name}`);
      this.refresh();
    });
  }
  isShowCreateBtn(){
    return this.isGranted(PERMISSIONS_CONSTANT.Admin_LineChartSetting_Create);
  }
  isShowEditBtn(){
    return this.isGranted(PERMISSIONS_CONSTANT.Admin_LineChartSetting_Edit);
  }
  isShowDeleteBtn(){
    return this.isGranted(PERMISSIONS_CONSTANT.Admin_LineChartSetting_Delete);
  }
  isShowActiveDecativeActionBtns(){
    return this.isGranted(PERMISSIONS_CONSTANT.Admin_LineChartSetting_ActiveDeactive);
  }
  isShowActionMenu(){
    return this.isShowEditBtn() || this.isShowDeleteBtn() || this.isShowActiveDecativeActionBtns();
  }
}
