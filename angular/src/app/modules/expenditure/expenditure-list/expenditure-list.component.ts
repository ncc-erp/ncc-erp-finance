import { AppComponentBase } from "@shared/app-component-base";
import { PERMISSIONS_CONSTANT } from "./../../../constant/permission.constant";
import { CreateEditExpenditureComponent } from "./../create-edit-expenditure/create-edit-expenditure.component";
import { catchError } from "rxjs/operators";
import { expenditureDto } from "./../expenditure.component";
import { ExpenditureService } from "./../../../service/api/expenditure.service";
import { MatDialog } from "@angular/material/dialog";
import { Component, Input, OnInit, Injector } from "@angular/core";
import { Router } from "@angular/router";

@Component({
  selector: "app-expenditure-list",
  templateUrl: "./expenditure-list.component.html",
  styleUrls: ["./expenditure-list.component.css"],
})
export class ExpenditureListComponent extends AppComponentBase {
  Directory_OutcomingEntryType_Create =
    PERMISSIONS_CONSTANT.Directory_OutcomingEntryType_Create;
  Directory_OutcomingEntryType_Delete =
    PERMISSIONS_CONSTANT.Directory_OutcomingEntryType_Delete;
  Directory_OutcomingEntryType_Edit =
    PERMISSIONS_CONSTANT.Directory_OutcomingEntryType_Edit;

  @Input() items: any;
  searchValue: string = "";
  constructor(
    private dialog: MatDialog,
    private service: ExpenditureService,
    private router: Router,
    injector: Injector
  ) {
    super(injector);
  }

  delete(item: expenditureDto): void {
    abp.message.confirm(
      "delete expenditure '" + item.name + "'?",
      "",
      (result: boolean) => {
        if (result) {
          this.service
            .delete(item.id)
            .pipe(catchError(this.service.handleError))
            .subscribe(() => {
              abp.notify.success("Deleted expenditure: " + item.name);
              this.reloadComponent();
            });
        }
      }
    );
  }
  create(expenditure: expenditureDto) {
    this.showDialog(expenditure, "create");
  }
  edit(expenditure: any) {
    this.showDialog(expenditure, "edit");
  }

  showDialog(expenditure: expenditureDto, command: String): void {
    let item = {
      id: expenditure.id,
      name: expenditure.name,
      code: expenditure.code,
      parentId: expenditure.parentId,
      workflowId: expenditure.workflowId,
      expenseType: expenditure.expenseType,
      isActive: expenditure.isActive,
    } as expenditureDto;

    this.dialog.open(CreateEditExpenditureComponent, {
      data: {
        dataToEdit: item,
        command: command,
      },
      width: "700px",
      disableClose: true,
    });
  }
  reloadComponent() {
    this.router.navigateByUrl("", { skipLocationChange: true }).then(() => {
      this.router.navigate(["/app/outcomingType"]);
    });
  }
}
