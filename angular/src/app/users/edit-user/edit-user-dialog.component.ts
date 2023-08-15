import { expenditureDto } from './../../modules/expenditure/expenditure.component';
import { ExpenditureService } from '@app/service/api/expenditure.service';
import {
  Component,
  Injector,
  OnInit,
  EventEmitter,
  Output
} from '@angular/core';
import { finalize } from 'rxjs/operators';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { forEach as _forEach, includes as _includes, map as _map } from 'lodash-es';
import { AppComponentBase } from '@shared/app-component-base';
import {
  UserServiceProxy,
  UserDto,
  RoleDto
} from '@shared/service-proxies/service-proxies';
import { NestedTreeControl } from '@angular/cdk/tree';
import { MatTreeNestedDataSource } from '@angular/material/tree';
import { SelectionModel } from '@angular/cdk/collections';

@Component({
  templateUrl: './edit-user-dialog.component.html',
  styleUrls: ['edit-user-dialog.component.css']
})
export class EditUserDialogComponent extends AppComponentBase
  implements OnInit {
  saving = false;
  user = new UserDto();
  roles: RoleDto[] = [];
  checkedRolesMap: { [key: string]: boolean } = {};
  id: number;

  checkIsGranted: boolean | number = -1;

  inputFilter: InputFilterOutcomingEntryType = new InputFilterOutcomingEntryType();

  selection = new SelectionModel<string>(true, []);
  public treeControl: NestedTreeControl<any>;
  public dataSource: MatTreeNestedDataSource<any>;
  public outcomingEntry:OutcomingEntryTypesDto[] = [];

  @Output() onSave = new EventEmitter<any>();

  constructor(
    injector: Injector,
    public _userService: UserServiceProxy,
    public bsModalRef: BsModalRef,
    private outcomeService: ExpenditureService
  ) {
    super(injector);
    this.dataSource = new MatTreeNestedDataSource<OutcomingEntryTypesDto>();
    this.treeControl = new NestedTreeControl<any>(node => node.children);
  }
  hasChild = (_: number, node: any) => !!node.children && node.children.length > 0;

  ngOnInit(): void {
    this.inputFilter.userId = this.id;
    this._userService.get(this.id).subscribe((result) => {
      this.user = result;
      this._userService.getRoles().subscribe((result2) => {
        this.roles = result2.items;
        this.setInitialRolesStatus();
      });
    });
    this.getAllData();
  }

  getAllData() {
    if(this.checkIsGranted == -1){
      this.inputFilter.isGranted = undefined;
    }else{
      this.inputFilter.isGranted = this.checkIsGranted == true;
    }
    this.outcomeService
      .getAllByGranted(this.inputFilter)
      .subscribe(data => {
        this.dataSource.data = data.result;
        this.treeControl.dataNodes = data.result;
        this.treeControl.dataNodes.forEach(node =>
          this.initSelectionList(node)
        );
    });
  }

  initSelectionList(node) {
    const selectedList = this.outcomingEntry.map(x=> x.name);
    if (selectedList.includes(node.item.name)) {
      this.selected(node);
    }
    if (!node.children || node.children.length === 0) {
      return;
    } else {
      node.children.forEach(child => this.initSelectionList(child));
    }
  }

  selected(node: any) {
    this.selection.select(node.item.name);
  }

  isSelected(node: any) {
    return node.item.isGranted;
  }

  deselected(node: any) {
    this.selection.deselect(node.item.name);
  }

  todoLeafItemSelectionToggle(node: any) {
    this.isSelected(node) ? this.deselected(node) : this.selected(node);
    this.descendantsPartiallySelected(node);
    this.onSaveData(node);
  }

  todoItemSelectionToggle(node: any) {
    this.isSelected(node) ? this.deselected(node) : this.selected(node);
    const descendants = this.treeControl.getDescendants(node);
    descendants.forEach(child => {
      this.isSelected(node) ? this.selected(child) : this.deselected(child);
    });
    this.onSaveData(node);
  }

  descendantsPartiallySelected(node: any): boolean {
    const descendants = this.treeControl.getDescendants(node);
    const result = descendants.some(child => this.isSelected(child));
    return result && !this.descendantsAllSelected(node);
  }

  descendantsAllSelected(node: any): boolean {
    const descendants = this.treeControl.getDescendants(node);
    const descAllSelected = descendants.every(child => this.isSelected(child));
    descAllSelected ? this.selected(node) : this.deselected(node);
    return descAllSelected;
  }

  selectOrDeselectTheIndeterminateParent(node: any, doSelect: boolean) {
    if (!node.childrens || node.childrens.length < 1) return;

    const descendants = this.treeControl.getDescendants(node);
    const descSomeSelected = descendants.some(child => this.isSelected(child));

    if (descSomeSelected) {
      if (doSelect) {
        this.selected(node);
      } else if (!this.descendantsAllSelected(node)) {
        this.deselected(node);
      }
    }
  }

  selectOrDeselectAllIndeterminateParents(doSelect: boolean) {
    this.treeControl.dataNodes.forEach(
      parent => { this.selectOrDeselectTheIndeterminateParent(parent, doSelect); }
    );
  }

  addOutcomeEntryType(node){
    node.item.isTableLoading = true;
    let descendants = this.treeControl.getDescendants(node);
    if (descendants) {
      descendants.forEach(child => child.item.isTableLoading = true);
    }
    let input = {
      userId: this.id,
      outcomingEntryTypeId: node.item.id
    } as InputToAddOrRemoveOutcomeTypeDto;
    this.outcomeService.updateOutcomingEntryTypeByUserId(input).subscribe((rs)=>{
      if(rs){
        node.item.isTableLoading = false;
        if (descendants) {
          descendants.forEach(child => child.item.isTableLoading = false);
        }
      }
      abp.notify.success("Save successfully");
    this.getAllData();
    },()=> node.item.isTableLoading = false);
    node.item.isGranted = true;
  }

  addAllOutcomeTypeToUser() {
    let requestBody = {
      userId: this.user.id
    }
    this.outcomeService.addAllOutcomingToUser(requestBody).subscribe(() => {
      abp.notify.success("Add all successful")
      this.getAllData();
    });
  }


  removeOutcomeEntryType(node){
    node.item.isTableLoading = true;
    const descendants = this.treeControl.getDescendants(node);
    if (descendants) {
      descendants.forEach(child => child.item.isTableLoading = true);
    }

    const input = {
      userId: this.id,
      outcomingEntryTypeId: node.item.id
    } as InputToAddOrRemoveOutcomeTypeDto;
    this.outcomeService.updateOutcomingEntryTypeByUserId(input).subscribe((rs)=>{
      if(rs){
        node.item.isTableLoading = false;
        if (descendants) {
          descendants.forEach(child => child.item.isTableLoading = false);
        }
      }
      abp.notify.success("Save successfully");
      this.getAllData();
    },()=> node.item.isTableLoading = false);
    node.item.isGranted = false;
  }

  onSaveData(node: any) {
    this.selectOrDeselectAllIndeterminateParents(true);
    if(node.item.isGranted){
      this.removeOutcomeEntryType(node);
    }else{
      this.addOutcomeEntryType(node);
    }
  }

  getClassByExpenseType(item) {
    switch(item.item.expenseType) {
      case 0:
        return 'text-color-green';
      case 1:
        return 'text-color-gray';
      default:
        return 'd-none';
    }
  }
  getTitleByExpenseType(item) {
    switch(item.item.expenseType) {
      case 0:
        return 'Tính vào chi phí';
      case 1:
        return 'Không tính vào chi phí';
      default:
        return 'Không xác định';
    }
  }

  setInitialRolesStatus(): void {
    _map(this.roles, (item) => {
      this.checkedRolesMap[item.normalizedName] = this.isRoleChecked(
        item.normalizedName
      );
    });
  }

  isRoleChecked(normalizedName: string): boolean {
    return _includes(this.user.roleNames, normalizedName);
  }

  onRoleChange(role: RoleDto, $event) {
    this.checkedRolesMap[role.normalizedName] = $event.target.checked;
  }

  getCheckedRoles(): string[] {
    const roles: string[] = [];
    _forEach(this.checkedRolesMap, function (value, key) {
      if (value) {
        roles.push(key);
      }
    });
    return roles;
  }

  save(): void {
    this.saving = true;

    this.user.roleNames = this.getCheckedRoles();

    this._userService
      .update(this.user)
      .pipe(
        finalize(() => {
          this.saving = false;
        })
      )
      .subscribe(() => {
        this.notify.info(this.l('Saved Successfully'));
        this.bsModalRef.hide();
        this.onSave.emit();
      });
  }
}
export class OutcomingEntryTypeOptionsDto{
  children : OutcomingEntryTypesDto[];
  item: OutcomingEntryTypesDto;
}
export class OutcomingEntryTypesDto{
  code : string;
  expenseType : number;
  isActive : boolean;
  isGranted : boolean;
  level : number;
  name : string;
  parentId: number;
  pathName: string;
  workflowId: number;
  id: number;
}
export class InputToAddOrRemoveOutcomeTypeDto{
  userId: number;
  outcomingEntryTypeId: number;
}

export class InputFilterOutcomingEntryType {
  isGranted?: boolean;
  expenseType?: number;
  searchText: string;
  userId: number;
}

