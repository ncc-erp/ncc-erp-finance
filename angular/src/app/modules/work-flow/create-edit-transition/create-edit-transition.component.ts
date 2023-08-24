import { Component, Inject, Injector, OnInit } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { ActivatedRoute, Router } from '@angular/router';
import { WorkFlowTransitionService } from '@app/service/api/work-flow-transition.service';
import { AppComponentBase } from '@shared/app-component-base';
import { catchError } from 'rxjs/operators';
import { WorkFlowStatusService } from '../../../service/api/work-flow-status.service';
import { RoleDto, TransitionItemDto } from '../work-flow-detail/work-flow-detail.component';
@Component({
  selector: 'app-create-edit-transition',
  templateUrl: './create-edit-transition.component.html',
  styleUrls: ['./create-edit-transition.component.css']
})
export class CreateEditTransitionComponent extends AppComponentBase implements OnInit {
  router: Router;
  paramId;
  isDisable: boolean = false;
  roles: RoleDto[] = [];
  transition = {} as TransitionItemDto;
  workFlowStatus: StatusDto[] = [];
  constructor(inject: Injector, @Inject(MAT_DIALOG_DATA) public data: any,
    private _workFlowTransitionService: WorkFlowTransitionService,
    private route: ActivatedRoute,
    public dialogRef: MatDialogRef<CreateEditTransitionComponent>,
    private _workFlowStatusService: WorkFlowStatusService,) {
    super(inject);
  }
  isEdit: boolean;
  ngOnInit(): void {
    this.transition = this.data.item;
    this.paramId = this.route.snapshot.queryParamMap.get('id');
    this.isEdit = this.data.id === undefined ? false : true;
    this.getWorkFlowStatus();
  }
  getWorkFlowStatus(): void {
    this._workFlowStatusService.getAll().subscribe((item) => {
      this.workFlowStatus = item.result;
    })
  }
  saveAndClose() {

    this.isDisable = true
      this.transition.workFLowId = this.paramId;
      this._workFlowTransitionService.create(this.transition).pipe(catchError(this._workFlowTransitionService.handleError)).subscribe(() => {
        abp.notify.success("created transition successfully");
        this.dialogRef.close();
      }, () => this.isDisable = false);
  }
}
export class StatusDto {
  name: string;
  code: string;
  id: number;
}

