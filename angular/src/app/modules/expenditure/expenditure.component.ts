import { PERMISSIONS_CONSTANT } from "@app/constant/permission.constant";
import { AppComponentBase } from "@shared/app-component-base";

import { CreateEditExpenditureComponent } from "./create-edit-expenditure/create-edit-expenditure.component";
import { ExpenditureService } from "./../../service/api/expenditure.service";
import { MatDialog } from "@angular/material/dialog";
import { Component, Injector, OnInit } from "@angular/core";
import { StatusEnum } from "@shared/AppEnums";
import * as _ from "lodash";
import { InputFilterEntryTypeDto } from "@app/service/model/common-DTO";
import { TranslateService } from "@ngx-translate/core";
import { HttpParams } from "@angular/common/http";
import { ActivatedRoute } from "@angular/router";

@Component({
  selector: "app-expenditure",
  templateUrl: "./expenditure.component.html",
  styleUrls: ["./expenditure.component.css"],
})
export class ExpenditureComponent extends AppComponentBase implements OnInit {
  Directory_OutcomingEntryType_Create =
    PERMISSIONS_CONSTANT.Directory_OutcomingEntryType_Create;
  dataToFill: any;
  tempData: any = {
    name: "all",
  };
  data: any = {
    name: "all",
  };
  inputFilter: InputFilterEntryTypeDto = new InputFilterEntryTypeDto();
  title: any;
  routeTitleFirstLevel;
  routeUrlFirstLevel = this.APP_CONSTANT.UrlBreadcrumbFirstLevel.Menu3;
  routeUrlSecondLevel = this.APP_CONSTANT.UrlBreadcrumbSecondLevel.outcomingType;
  queryParams;

  constructor(
    private route: ActivatedRoute,
    private dialog: MatDialog,
    private service: ExpenditureService,
    injector: Injector,
    private translate: TranslateService
  ) {
    super(injector);
  }
  outCommingList: any;
  searchStatus: StatusEnum = StatusEnum.ACTIVE;

  ngOnInit(): void {
    this.getAllData();
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
    this.translate.get("menu3.m3_child6").subscribe((res: string) => {
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
  getByNum(enumValue: number, objectEnum: any) {
    for (let key in objectEnum) {
      if (enumValue == objectEnum[key]) {
        return key;
      }
    }
  }

  getAllData() {
    this.service
      .getAllByStatus(this.inputFilter)
      .subscribe(
        (apiData) => (
          (this.data.children = this.filterExpenditure(apiData.result)),
          (this.dataToFill = apiData.result),
          (this.tempData = apiData.result)
        )
      );
  }
  filterExpenditure(arr) {
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
  type = 2;
  showDialog(command: String): void {
    this.dialog.open(CreateEditExpenditureComponent, {
      data: {
        command: command,
      },
      width: "700px",
      disableClose: true,
    });
  }
  createOutcome() {
    this.showDialog("create");
  }
}
export class expenditureDto {
  name: string;
  code: string;
  pathId: string;
  pathName: string;
  parentId: number;
  workflowId: number;
  id: number;
  expenseType: number;
  isActive: boolean;
}
export class expenditureListDto {
  name: string;
  children: expenditureDto[];
}
