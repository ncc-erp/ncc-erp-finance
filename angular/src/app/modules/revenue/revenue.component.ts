import { PERMISSIONS_CONSTANT } from "./../../constant/permission.constant";
import { AppComponentBase } from "@shared/app-component-base";
import { RevenueService } from "./../../service/api/revenue.service";
import { Component, Injector, OnInit } from "@angular/core";
import { MatDialog } from "@angular/material/dialog";
import { CreateEditRevenueComponent } from "./create-edit-revenue/create-edit-revenue.component";
import { AppConsts, OPTION_ALL } from "@shared/AppConsts";
import { StatusEnum } from "@shared/AppEnums";
import { InputFilterEntryTypeDto } from "@app/service/model/common-DTO";
import { TranslateService } from "@ngx-translate/core";
import { HttpParams } from "@angular/common/http";
import { ActivatedRoute } from "@angular/router";

@Component({
  selector: "app-revenue",
  templateUrl: "./revenue.component.html",
  styleUrls: ["./revenue.component.css"],
})
export class RevenueComponent extends AppComponentBase implements OnInit {
  dataToFill: any;
  tempData: any;

  data: any = {
    name: "all",
  };
  type = 2;
  searchText: string = "";
  title: any;
  routeTitleFirstLevel;
  routeUrlFirstLevel = this.APP_CONSTANT.UrlBreadcrumbFirstLevel.Menu3;
  routeUrlSecondLevel = this.APP_CONSTANT.UrlBreadcrumbSecondLevel.incomingType;
  queryParams;
  constructor(
    private route: ActivatedRoute,
    private dialog: MatDialog,
    private service: RevenueService,
    injector: Injector,
    private translate: TranslateService
  ) {
    super(injector);
  }
  outCommingList: any;
  inputFilter: InputFilterEntryTypeDto = new InputFilterEntryTypeDto();

  ngOnInit(): void {
    this.subscriptions.push(
      AppConsts.periodId.asObservable().subscribe((rs) => {
        this.getAllData();
      }));
    this.translate.onLangChange.subscribe(() => {
      this.onLangChange();
    });
    this.route.queryParams.subscribe(params => {
      this.queryParams = new HttpParams({ fromObject: params });
      this.onLangChange();
    });
  }
  
  onLangChange(){
    this.translate.get("menu.menu3").subscribe((res: string) => {
      this.routeTitleFirstLevel = res;
      this.updateBreadCrumb();
    });
    this.translate.get("menu3.m3_child5").subscribe((res: string) => {
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
  getAllData() {
    this.service
      .getAllByStatus(this.inputFilter)
      .subscribe(
        (apiData) => (
          (this.data.children = this.filterIncome(apiData.result)),
          (this.tempData = apiData.result)
        )
      );
  }

  filterIncome(arr) {
    let rs = arr
      .filter((item) => {
        if (item.parentId == null) {
          return (item.children = arr.filter((child) => {
            return child.parentId == item.id;
          }));
        } else {
          return (item.children = arr.filter((child) => {
            return child.parentId == item.id;
          }));
        }
      })
      .filter((finalItem) => {
        return finalItem.parentId == null;
      });
    return rs;
  }

  showDialog(command: String): void {
    this.dialog.open(CreateEditRevenueComponent, {
      data: {
        command: command,
      },
      width: "700px",
      disableClose: true,
    });
  }
  createRevenues() {
    this.showDialog("create");
  }

  isShowCreateBtn(){
    return this.isGranted(PERMISSIONS_CONSTANT.Directory_IncomingEntryType_Create);
  }
}
export class RevenuesDto {
  name: string;
  code: string;
  pathId: string;
  pathName: string;
  level: 0;
  parentId: 0;
  id: 0;
  revenueCounted: boolean;
  isActive: boolean;
  isClientPaid: boolean;
  isClientPrePaid: boolean;
}
export class RevenueListDto {
  name: string;
  children: RevenuesDto[];
}

