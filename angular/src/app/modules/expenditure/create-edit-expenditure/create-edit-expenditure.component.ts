import { WorkFlowService } from "./../../../service/api/work-flow.service";
import { Component, Inject, OnInit, Injector } from "@angular/core";
import { MAT_DIALOG_DATA, MatDialogRef } from "@angular/material/dialog";
import { Router } from "@angular/router";
import { ExpenditureService } from "@app/service/api/expenditure.service";
import { catchError, map, startWith } from "rxjs/operators";
import { expenditureDto } from "../expenditure.component";
import { AppComponentBase } from "@shared/app-component-base";
import { IncomingEntryTypeOptions } from "@app/modules/new-versions/b-transaction/link-revenue-ecognition-dialog/link-revenue-ecognition-dialog.component";
import { CommonService } from "@app/service/api/new-versions/common.service";
import { HttpErrorResponse } from "@angular/common/http";
import { of } from "rxjs";


@Component({
  selector: "app-create-edit-expenditure",
  templateUrl: "./create-edit-expenditure.component.html",
  styleUrls: ["./create-edit-expenditure.component.css"],
})
export class CreateEditExpenditureComponent
  extends AppComponentBase
  implements OnInit
{
  expenditure = { parentId: null, isActive: true, expenseType: 1} as expenditureDto;
  isEdit: boolean = false;
  expenseTypeList: any;
  selectData: any;
  expenditureList: any;
  defaultSelect: any;
  outcomingEntryTypeOptions: IncomingEntryTypeOptions;
  title: string;
  isDisable: boolean = false;
  workFlowList: any;
  searchExpenditure: string = "";
  searchWorkFlow: string = "";
  name: string = "";
  constructor(
    @Inject(MAT_DIALOG_DATA) public data: any,
    private service: ExpenditureService,
    public dialogRef: MatDialogRef<CreateEditExpenditureComponent>,
    private router: Router,
    private _common: CommonService,
    injector: Injector,
    private workflowService: WorkFlowService
  ) {
    super(injector);
    this.expenditure.parentId = undefined;
  }

  ngOnInit(): void {
    this.getWorkflow();
    this.getTypeOfCosts();
    this.initDialog();
    this.getTreeOutcomingEntries();
    this.service
      .GetAllForDropdown()
      .subscribe(
        (data) =>
          (this.selectData = data.result.filter(
            (item) => item.id != this.expenditure.id
          ))
      );
  }
  private initDialog(){
    if (this.data.command == "edit") {
      this.expenditure = this.data.dataToEdit;
      this.isEdit = true;
      this.name = this.data.dataToEdit.name;
    }
    else {
      if (this.data.dataToEdit && this.data.dataToEdit.id != 0){
        this.expenditure.parentId = this.data.dataToEdit.id;
        this.expenditure.expenseType = this.data.dataToEdit.expenseType;
        this.expenditure.workflowId = this.data.dataToEdit.workflowId;
      }
      this.expenditure.isActive = true;
    }
    this.title = this.data.command;
  }
  getTreeOutcomingEntries(){
    this._common.getTreeOutcomingEntries().subscribe(response => {
      if (!response.success) return;
      this.outcomingEntryTypeOptions = {item: {id: 0, name : "", parentId : null}, children: [...response.result]};
    })
  }
  getWorkflow() {
    this.workflowService.GetAllForDropdown().subscribe((data) => {
      if(!data.success) return;
      this.workFlowList = data.result;
      if(this.data.command == "create" && this.workFlowList.length > 0) this.expenditure.workflowId = this.workFlowList[0].id;
    });
  }
  getTypeOfCosts() {
    this.service.GetComboxExpenseTypes().subscribe((data) => {
      if(!data.success) return;
      this.expenseTypeList = data.result;
    });
  }
  saveAndClose() {
    this.isDisable = true;
    if (this.data.command == "edit") {
      this.service
        .updateExpenditure(this.expenditure)
        .pipe(
          map(data => ({...data, loading: false})),
          startWith({loading: true, success: false, result: null }),
          catchError((error: HttpErrorResponse) => {
            return of({loading: false, success: false, result: null, error})
          })
        )
        .subscribe(
          (res) => {
            if(res.success){
              abp.notify.success("Edited expenditure ");
              this.reloadComponent();
              this.dialogRef.close();
            }
            this.isDisable = res.loading;
          }
        );
    } else {
      this.service
        .create(this.expenditure)
        .pipe(
          map(data => ({...data, loading: false})),
          startWith({loading: true, success: false, result: null }),
          catchError((error: HttpErrorResponse) => {
            return of({loading: false, success: false, result: null, error})
          })
        )
        .subscribe(res => {
          if(res.success){
            abp.notify.success("created expenditure ");
            this.reloadComponent();
            this.dialogRef.close();
          }
          this.isDisable = res.loading;
        },
        );
    }
  }

  handleChangeName() {
    const code = this.getCodeByName(this.name);
    if (this.expenditure.code == code || !this.expenditure.code) {
      this.expenditure.code = this.getCodeByName(this.expenditure.name);
    }
    this.name = this.expenditure.name;
  }

  getCodeByName(name: string) {
    return this.removeVietnameseTones(name.toLocaleLowerCase().trim()).replace(
      / /g,
      "_"
    );
  }

  reloadComponent() {
    this.router.navigateByUrl("", { skipLocationChange: true }).then(() => {
      this.router.navigate(["/app/outcomingType"]);
    });
  }
}
