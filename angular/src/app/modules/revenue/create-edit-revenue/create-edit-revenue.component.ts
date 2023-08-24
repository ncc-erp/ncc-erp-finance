import { RevenueService } from "./../../../service/api/revenue.service";
import { Component, Inject, OnInit, Injector } from "@angular/core";
import { MAT_DIALOG_DATA, MatDialogRef } from "@angular/material/dialog";
import { Router, ActivatedRoute } from "@angular/router";
import { catchError } from "rxjs/operators";
import { RevenuesDto } from "../revenue.component";
import { AppComponentBase } from "@shared/app-component-base";
import { CommonService } from "@app/service/api/new-versions/common.service";
import { IncomingEntryTypeOptions } from "@app/modules/new-versions/b-transaction/link-revenue-ecognition-dialog/link-revenue-ecognition-dialog.component";

@Component({
  selector: "app-create-edit-revenue",
  templateUrl: "./create-edit-revenue.component.html",
  styleUrls: ["./create-edit-revenue.component.css"],
})
export class CreateEditRevenueComponent
  extends AppComponentBase
  implements OnInit {
  revenues = { isActive: true } as RevenuesDto;
  isEdit: boolean = false;

  selectData: any;
  incomingEntryTypeOptions: IncomingEntryTypeOptions;
  title: string;
  searchRevenue: string = "";
  isDisable: boolean = false;
  name: string = "";
  constructor(
    @Inject(MAT_DIALOG_DATA) public data: any,
    private service: RevenueService,
    injector: Injector,
    private router: Router,
    private route: ActivatedRoute,
    private _common: CommonService,
    public dialogRef: MatDialogRef<CreateEditRevenueComponent>
  ) {
    super(injector);
    this.revenues.parentId = null;
  }

  ngOnInit(): void {
    this.initDialog();
    this.service
      .GetAllForDropdown()
      .subscribe(
        (data) =>
        (this.selectData = data.result.filter(
          (item) => item.id != this.revenues.id
        ))
      );
    this.getTreeIncomingEntries();
  }
  private initDialog() {
    if (this.data.command == "edit") {
      this.revenues = this.data.dataToEdit;
      this.isEdit = true;
      this.name = this.data.dataToEdit.name
    }
    else {
      if (this.data.dataToEdit && this.data.dataToEdit.id > 0) {
        this.revenues.parentId = this.data.dataToEdit.id;
        this.revenues.revenueCounted = this.data.dataToEdit.revenueCounted;
      }
    }
    this.title = this.data.command;
  }
  getTreeIncomingEntries() {
    this._common.getTreeIncomingEntries().subscribe(response => {
      if (!response.success) return;
      this.incomingEntryTypeOptions = { item: { id: 0, name: "", parentId: null }, children: [...response.result] };
    })
  }
  saveAndClose() {
    this.isDisable = true;
    if (this.data.command == "edit") {
      this.service
        .updateRevenue(this.revenues)
        .pipe(catchError(this.service.handleError))
        .subscribe(
          (res) => {
            abp.notify.success("Edited revenue ");
            this.reloadComponent();
            this.dialogRef.close();
          },
          () => (this.isDisable = false)
        );
    } else {
      this.service
        .create(this.revenues)
        .pipe(catchError(this.service.handleError))
        .subscribe(
          (res) => {
            abp.notify.success("created revenue ");
            this.reloadComponent();
            this.dialogRef.close();
          },
          () => (this.isDisable = false)
        );
    }
  }

  handleChangeName() {
    const code = this.getCodeByName(this.name);
    if (this.revenues.code == code || !this.revenues.code) {
      this.revenues.code = this.getCodeByName(this.revenues.name);
    }
    this.name = this.revenues.name;
  }

  getCodeByName(name: string) {
    return this.removeVietnameseTones(name.toLocaleLowerCase().trim()).replace(
      / /g,
      "_"
    );
  }

  reloadComponent() {
    this.router.navigateByUrl("", { skipLocationChange: true }).then(() => {
      this.router.navigate(["/app/incomingType"]);
    });
  }
}
