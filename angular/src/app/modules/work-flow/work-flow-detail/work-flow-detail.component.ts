import { Component, Injector, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { WorkFlowStatusService } from '../../../service/api/work-flow-status.service';
import { WorkFlowService } from '../../../service/api/work-flow.service';
import { WorkFlowTransitionService } from '../../../service/api/work-flow-transition.service';
import { RoleService } from '../../../service/api/role.service';
import { PERMISSIONS_CONSTANT } from '@app/constant/permission.constant';
import { AppComponentBase } from '@shared/app-component-base';
import { MatDialog } from '@angular/material/dialog';
import { CreateEditStatusComponent } from '../create-edit-status/create-edit-status.component';
import { CreateEditTransitionComponent } from '../create-edit-transition/create-edit-transition.component';
import { EditTransitionComponent } from '../edit-transition/edit-transition.component';

@Component({
  selector: 'app-work-flow-detail',
  templateUrl: './work-flow-detail.component.html',
  styleUrls: ['./work-flow-detail.component.css']
})
export class WorkFlowDetailComponent extends AppComponentBase implements OnInit {

  router: Router;
  searchText: string = '';
  statuses: any;
  transitions: WorkFlowStatus;


  constructor(injector: Injector, private _roleService: RoleService, private _workFlowStatusService: WorkFlowStatusService, private _workFlowTransitionService: WorkFlowTransitionService, private _workFlowService: WorkFlowService, private dialog: MatDialog, private route: ActivatedRoute) {
    super(injector);
  }
  paramId;
  paramId1;
  ngOnInit(): void {
    this.paramId = this.route.snapshot.queryParamMap.get('id');
    this.paramId1 = parseInt(this.paramId);
    this.refresh(this.paramId);
  }

  refresh(id): void {
    this._workFlowService.getById(id).subscribe((item) => {
      this.statuses = item.result;
    })
  }



  deleteTransition(transition: TransitionItemDto): void {
    abp.message.confirm(
      this.l("Delete transition ?"),
      '',
      (result: boolean) => {
        if (result) {
          this._workFlowTransitionService.delete(transition.id).subscribe(() => {
            abp.notify.success(this.l('Deleted transition successfully'));
            this.refresh(this.paramId1);
          });
        }
      }
    );
  }

  createWorkFlowTransition(): void {
    this.showDialogWorkFlowTransition('create', {});
  }
  showDialogWorkFlowTransition(command: string, item: any): void {
    let request = {} as TransitionItemDto;
    if (command == 'edit') {
      request = {
        name: item.name,
        fromStatusId: item.fromStatusId,
        fromStatusName: item.fromStatusName,
        fromStatusCode: item.fromStatusCode,
        toStatusId: item.toStatusId,
        toStatusName: item.toStatusName,
        id: item.id,
        roleId: item.roleId,
        toStatusCode: item.toStatusCode,
        roleName: item.roleName,
        workFLowId: item.workFLowId
      }
    }
    const dialogRef = this.dialog.open(CreateEditTransitionComponent, {
      data: {
        command: command,
        item: request
      },
      width: '500px',
      disableClose: true
    });

    dialogRef.afterClosed().subscribe(result => {
      this.refresh(this.paramId);
    });
  }
  editWorkFlowTransition(transitionItem: TransitionItemDto): void {
    this.EditshowDialogWorkFlowTransition('edit', transitionItem);
  }
  EditshowDialogWorkFlowTransition(command: string, item: any): void {
    let request = {} as TransitionItemDto;
    if (command == 'edit') {
      request = {
        name: item.name,
        fromStatusId: item.fromStatusId,
        fromStatusName: item.fromStatusName,
        fromStatusCode: item.fromStatusCode,
        toStatusId: item.toStatusId,
        toStatusName: item.toStatusName,
        id: item.id,
        roleId: item.roleId,
        toStatusCode: item.toStatusCode,
        roleName: item.roleName,
        workFLowId: item.workFLowId
      }
    }
    const dialogRef = this.dialog.open(EditTransitionComponent, {
      data: {
        command: command,
        item: request
      },
      width: '700px',
      disableClose: false
    });

    dialogRef.afterClosed().subscribe(() => {
      this.refresh(this.paramId);
    });
  }

  isShowActionBtns(){
    return this.isGranted(PERMISSIONS_CONSTANT.Admin_Workflow_WorkflowDetail_Edit);
  }
}
export class WorkFlowStatus {
  name: string;
  transitions: TransitionItemDto[];
  id: number;
  role: RoleDto[];
}
export class TransitionItemDto {
  name: string;
  fromStatusId: number;
  fromStatusName: string;
  fromStatusCode: string;
  toStatusId: number;
  toStatusName: string;
  id: number;
  roleId: number;
  toStatusCode: string;
  roleName: string;
  workFLowId: number;
}

export class RoleByPermission {
  id: number;
  permission: Permission[];
}

export class Permission {
  id: number;
  roleDisplayName: string;
  roleName: string;
}
export class RoleDto {
  name: string;
  displayName: string;
  id: number;
}
