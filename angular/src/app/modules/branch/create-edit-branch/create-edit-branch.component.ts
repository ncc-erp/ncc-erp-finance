import { Component, Inject, Injector, OnInit } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { AppComponentBase } from '@shared/app-component-base';
import { catchError } from 'rxjs/operators';
import { BranchService } from '../../../service/api/branch.service';
import { BranchDto } from '../branch.component';
@Component({
  selector: 'app-create-edit-branch',
  templateUrl: './create-edit-branch.component.html',
  styleUrls: ['./create-edit-branch.component.css']
})
export class CreateEditBranchComponent extends AppComponentBase implements OnInit {
  isDisable: boolean = false;
  branch = {} as BranchDto
  constructor(
    @Inject(MAT_DIALOG_DATA) public data: any,
    private branchService: BranchService,
    injector: Injector,
    public dialogRef: MatDialogRef<CreateEditBranchComponent>
  ) { super(injector); }
  isEdit: boolean;

  ngOnInit(): void {
    this.branch = this.data.item;
    this.isEdit = this.data.item.id === undefined ? false : true;
  }
  saveAndClose() {
    this.isDisable = true
    if (this.data.command == "create") {
      this.branchService.create(this.branch).pipe(catchError(this.branchService.handleError)).subscribe((res) => {
        abp.notify.success("created branch successfully");
        this.dialogRef.close();
      }, () => this.isDisable = false);
    }
    else {
      this.branchService.update(this.branch).pipe(catchError(this.branchService.handleError)).subscribe((res) => {
        abp.notify.success("edited branch successfully");
        this.dialogRef.close();
      }, () => this.isDisable = false);
    }
  }

}
