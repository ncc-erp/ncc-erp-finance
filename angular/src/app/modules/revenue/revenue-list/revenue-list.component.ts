import { PERMISSIONS_CONSTANT } from "./../../../constant/permission.constant";
import { catchError } from "rxjs/operators";
import { RevenueService } from "./../../../service/api/revenue.service";
import { AppComponentBase } from "@shared/app-component-base";
import { Component, Injector, Input, OnInit } from "@angular/core";
import { RevenuesDto } from "../revenue.component";
import { MatDialog } from "@angular/material/dialog";
import { Router } from "@angular/router";
import { CreateEditRevenueComponent } from "../create-edit-revenue/create-edit-revenue.component";

@Component({
  selector: "app-revenue-list",
  templateUrl: "./revenue-list.component.html",
  styleUrls: ["./revenue-list.component.css"],
})
export class RevenueListComponent extends AppComponentBase {
  @Input() items: any;
  isDisable: boolean = false;

  constructor(
    private dialog: MatDialog,
    private service: RevenueService,
    private router: Router,
    injector: Injector
  ) {
    super(injector);
  }

  delete(item: RevenuesDto): void {
    abp.message.confirm(
      "deleted revenue '" + item.name + "'?",
      "",
      (result: boolean) => {
        if (result) {
          this.service
            .delete(item.id)
            .pipe(catchError(this.service.handleError))
            .subscribe(() => {
              abp.notify.success("Deleted revenue: " + item.name);
              this.reloadComponent();
            });
        }
      }
    );
  }
  create(revenues: any) {
    this.showDialog(revenues, "create");
  }
  edit(revenues: any) {
    this.showDialog(revenues, "edit");
  }

  showDialog(revenues: RevenuesDto, command: String): void {
    let item = {
      id: revenues.id,
      name: revenues.name,
      code: revenues.code,
      parentId: revenues.parentId,
      revenueCounted: revenues.revenueCounted,
      isActive: revenues.isActive,
      isClientPaid: revenues.isClientPaid
    } as RevenuesDto;

    this.dialog.open(CreateEditRevenueComponent, {
      data: {
        dataToEdit: item,
        command: command,
      },
      width: "700px",
      disableClose: true,
    });
  }

  update(item: RevenuesDto, isClientPaid: boolean, message: string) {
    this.isDisable = true;
    let isCancelRequest = "";
    if ((isClientPaid && !item.isClientPaid) || (!isClientPaid && !item.isClientPrePaid)) {
      isCancelRequest = " hủy";
    }

    let isChangeAll = false;
    if(item.isClientPaid && item.isClientPrePaid){
      isChangeAll = true;

      if(isClientPaid) item.isClientPrePaid = false;
      else item.isClientPaid = false;
    }

    abp.message.confirm(
      `Bạn có chắc chắn${isCancelRequest} xét loại thu <b>" ${item.name} "</b> là ${message}? <br/> Việc này có thể làm ảnh hưởng: <br/> <b>Quản lý tài chính > Khoản phải thu</b>`,
      "Xác nhận",
      (result: boolean) => {
        if (result) {
          this.service
            .updateRevenue(item)
            .pipe(catchError(this.service.handleError))
            .subscribe(
              (res) => {
                if (!res.success) return;
                abp.notify.success("Edited revenue ");
              },
              () => (this.isDisable = false)
            );
        }
        else {
          if(isChangeAll){
            item.isClientPaid = !item.isClientPaid;
            item.isClientPrePaid = !item.isClientPrePaid;
            return;
          }
          if(isClientPaid) item.isClientPaid = !item.isClientPaid;
          else item.isClientPrePaid = !item.isClientPrePaid;
        }
      },
      { isHtml: true }
    );

  }

  reloadComponent() {
    this.router.navigateByUrl("", { skipLocationChange: true }).then(() => {
      this.router.navigate(["/app/incomingType"]);
    });
  }
  isShowEditBtn() {
    return this.isGranted(PERMISSIONS_CONSTANT.Directory_IncomingEntryType_Edit);
  }
  isShowDeleteBtn() {
    return this.isGranted(PERMISSIONS_CONSTANT.Directory_IncomingEntryType_Delete);
  }
  get isShowCreateBtn() {
    return this.isGranted(PERMISSIONS_CONSTANT.Directory_IncomingEntryType_Create);
  }
}
