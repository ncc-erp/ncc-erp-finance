import { PERMISSIONS_CONSTANT } from "./../../constant/permission.constant";
import { AppComponentBase } from "@shared/app-component-base";
import { RevenueService } from "./../../service/api/revenue.service";
import { Component, Injector, OnInit } from "@angular/core";
import { MatDialog } from "@angular/material/dialog";
import { CreateEditRevenueComponent } from "./create-edit-revenue/create-edit-revenue.component";
import { AppConsts, OPTION_ALL } from "@shared/AppConsts";
import { StatusEnum } from "@shared/AppEnums";

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
  constructor(
    private dialog: MatDialog,
    private service: RevenueService,
    injector: Injector
  ) {
    super(injector);
  }
  outCommingList: any;
  inputFilter: InputFilterRevenue = new InputFilterRevenue();

  ngOnInit(): void {
    this.subscriptions.push(
      AppConsts.periodId.asObservable().subscribe((rs) => {
        this.getAllData();
      }));
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

export class InputFilterRevenue {
  constructor(){
    this.isActive = true;
    this.revenueCounted = "";
  }
  isActive?: boolean;
  revenueCounted?: boolean | string;
  searchText: string;
}
