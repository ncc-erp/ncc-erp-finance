import { Component, Inject, Injector, OnInit } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { AppComponentBase } from '@shared/app-component-base';
import { catchError } from 'rxjs/operators';
import { WorkFlowService } from '../../../service/api/work-flow.service';
import { WorkFlowDto } from '../work-flow.component';
@Component({
  selector: 'app-create-edit-work-flow',
  templateUrl: './create-edit-work-flow.component.html',
})
export class CreateEditWorkFlowComponent extends AppComponentBase implements OnInit {
  isDisable: boolean = false;
  workFlow = {} as WorkFlowDto;
  constructor(inject: Injector,
    @Inject(MAT_DIALOG_DATA) public data: any,
    private workFlowService: WorkFlowService,
    public dialogRef: MatDialogRef<CreateEditWorkFlowComponent>) {
    super(inject);
  }

  isEdit: boolean;
  ngOnInit(): void {
    this.workFlow = this.data.item;
    this.isEdit = this.data.id === undefined ? false : true;
  }
  saveAndClose() {
    this.isDisable = true
    if (this.data.command == "create") {
      this.workFlowService.create(this.workFlow).pipe(catchError(this.workFlowService.handleError)).subscribe((res) => {
        abp.notify.success("created work flow successfully");
        this.dialogRef.close();
      }, () => this.isDisable = false);
    }
    else {
      this.workFlowService.update(this.workFlow).pipe(catchError(this.workFlowService.handleError)).subscribe((res) => {
        abp.notify.success("edited work flow successfully");
        this.dialogRef.close();
      }, () => this.isDisable = false);
    }
  }

}
