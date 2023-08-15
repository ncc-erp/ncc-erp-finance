import { HttpErrorResponse } from '@angular/common/http';
import { Component, Inject, Injector, OnInit } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { ActivatedRoute, Router } from '@angular/router';
import { WorkFlowStatusService } from '@app/service/api/work-flow-status.service';
import { ValueAndNameModel } from '@app/service/model/common-DTO';
import { AppComponentBase } from '@shared/app-component-base';
import { of } from 'rxjs';
import { catchError, map, startWith } from 'rxjs/operators';
import { RoleService } from '../../../service/api/role.service';
import { TransitionPermissionService } from '../../../service/api/transition-permission.service';
import { WorkFlowTransitionService } from '../../../service/api/work-flow-transition.service';
import { WorkFlowService } from '../../../service/api/work-flow.service';
import { RoleByPermission, RoleDto, TransitionItemDto } from '../work-flow-detail/work-flow-detail.component';

@Component({
  selector: 'app-edit-transition',
  templateUrl: './edit-transition.component.html',
  styleUrls: ['./edit-transition.component.css']
})
export class EditTransitionComponent extends AppComponentBase implements OnInit {
  isEdit: boolean;
  roleId: any;
  roleByTransition: RoleByPermission = new RoleByPermission();
  router: Router;
  roles : ValueAndNameModel[] = [];
  paramId;
  tabIndex: number;
  transitionId: number;
  permissionId: PermissionRole[] = [];
  isDisable: boolean = false;
  workFlowStatus = [];
  transition = {} as TransitionItemDto;
  isAdding = false;
  constructor(inject: Injector, @Inject(MAT_DIALOG_DATA) public data: any,
  private _roleService: RoleService,private route: ActivatedRoute,
  private _workFlowStatusService: WorkFlowStatusService,
  private _workFlowTransitionService: WorkFlowTransitionService,
  private _transitionPermission: TransitionPermissionService,
  private _workFlowService: WorkFlowService,
  public dialogRef: MatDialogRef<EditTransitionComponent>) {
    super(inject);
  }
  obj = {
    transitionId : this.data.item.id,
    roleId: undefined
  }
  changeRole(value:any) {
    this.obj.roleId = value;
  }
  onTabChanged(index): void {
    this.tabIndex = index.index;
  }
  ngOnInit(): void {
    this.paramId = this.route.snapshot.queryParamMap.get('id');
    this.isEdit = this.data.id === undefined ? false : true;
    this.getStatus();
    this.getRoleByTransition(this.data.item.id);
    this.transition = this.data.item;
  }
  getStatus(): void {
    this._workFlowStatusService.getAll().subscribe((item) => {
      this.workFlowStatus = item.result;
    })
  }
  getRoleByTransition(id: number): void {
    this._workFlowTransitionService.getRolesByTransition(id).subscribe((item) => {
      this.roleByTransition = item.result;
      this.permissionId = item.result.permission;
      this.transitionId = item.result.id;
      this.getRoles(item.result.permission);
    })
  }
  getRoles(permissions = []): void {
    let notExitRoleIds = []
    if(permissions){
      notExitRoleIds = permissions.map(s => s.roleId);
    }
    this._roleService.getRole()
    .pipe(
      map(data => ({...data, loading: false})),
      startWith({loading: true, success: false, result: null }),
      catchError((error: HttpErrorResponse) => {
        return of({loading: false, success: false, result: null, error})
      })
    )
    .subscribe((item) => {
      this.isDisable = item.loading;
      if(item.success){
        this.roles = item.result.items.filter(s => !notExitRoleIds.includes(s.id)).map(s => {return {name: s.name, value: s.id} as ValueAndNameModel});
      }
    });
  }
  addRole(): void {
    this._transitionPermission.create(this.obj)
    .pipe(
      map(data => ({...data, loading: false})),
      startWith({loading: true, success: false, result: null }),
      catchError((error: HttpErrorResponse) => {
        return of({loading: false, success: false, result: null, error})
      })
    )
    .subscribe((item) => {
      this.isDisable = item.loading;
      if(item.success){
        this.getRoleByTransition(this.transitionId);
        this.obj.roleId = undefined;
        abp.notify.success("Add role success");
      }
    })
  }
  clearRole(permissionRole: PermissionRole): void {
    this.isDisable = true
    this._transitionPermission.delete(permissionRole.id)
    .pipe(
      map(data => ({...data, loading: false})),
      startWith({loading: true, success: false, result: null }),
      catchError((error: HttpErrorResponse) => {
        return of({loading: false, success: false, result: null, error})
      })
    ).subscribe((item) => {
      this.isDisable = item.loading;
      if(item.success){
        this.getRoleByTransition(this.transitionId);
        abp.notify.success("Remove role success");
      }

    })
  }
  cancel(){
    if(this.obj.roleId){
      this.obj.roleId = undefined;
      return;
    }
    this.isAdding = false;
  }
  saveAndClose() {
    this.isDisable = true
    if (this.data.command == "create") {
      this._workFlowTransitionService.create(this.transition)
      .pipe(
        map(data => ({...data, loading: false})),
        startWith({loading: true, success: false, result: null }),
        catchError((error: HttpErrorResponse) => {
          return of({loading: false, success: false, result: null, error})
        })
      )
      .subscribe((res) => {
        this.isDisable = res.loading;
        if(res.success){
          abp.notify.success("created work flow successfully");
          this.dialogRef.close();
        }
      });
    }
    else {
      this.transition.workFLowId = this.paramId;
      this._workFlowTransitionService.update(this.transition)
      .pipe(
        map(data => ({...data, loading: false})),
        startWith({loading: true, success: false, result: null }),
        catchError((error: HttpErrorResponse) => {
          return of({loading: false, success: false, result: null, error})
        })
      ).subscribe((res) => {
        this.isDisable = res.loading;
        if(res.success){
          abp.notify.success("edited work flow successfully");
          this.dialogRef.close();
        }
      });
    }
  }
  }
  export class PermissionRole {
    id: number;
  }




