import { Component, Injector, OnInit } from '@angular/core';
import { FormBuilder } from '@angular/forms';
import { MatDialog } from '@angular/material/dialog';
import { PERMISSIONS_CONSTANT } from '@app/constant/permission.constant';
import { InputFilterDto } from '@shared/filter/filter.component';
import { PagedListingComponentBase, PagedRequestDto, PagedResultResultDto } from '@shared/paged-listing-component-base';
import { BsModalRef, BsModalService } from 'ngx-bootstrap/modal';
import { finalize } from 'rxjs/operators';
import { BranchService } from '../../service/api/branch.service';
import { CreateEditBranchComponent } from './create-edit-branch/create-edit-branch.component';

@Component({
  selector: 'app-branch',
  templateUrl: './branch.component.html',
  styleUrls: ['./branch.component.css']
})
export class BranchComponent extends PagedListingComponentBase<any> {

  Directory_Branch_Create = PERMISSIONS_CONSTANT.Directory_Branch_Create;
  Directory_Branch_Delete = PERMISSIONS_CONSTANT.Directory_Branch_Delete;
  Directory_Branch_Edit = PERMISSIONS_CONSTANT.Directory_Branch_Edit;
  public readonly FILTER_CONFIG: InputFilterDto[] = [
    { propertyName: 'Name', comparisions: [0, 6, 7, 8], displayName: "filterDirectory.Name" },
    { propertyName: 'Code', comparisions: [0, 6, 7, 8], displayName: "filterDirectory.Code" },
  ];
  constructor(
    injector: Injector,
    private _branchService: BranchService,
    private dialog: MatDialog,

  ) {
    super(injector);
  }

  branches: BranchDto[] = [];

  protected list(
    request: PagedRequestDto,
    pageNumber: number,
    finishedCallback: Function
  ): void {
    this._branchService
      .getAllPaging(request)
      .pipe(finalize(() => {
        finishedCallback();
      }))
      .subscribe((result: PagedResultResultDto) => {
        this.branches = result.result.items;
        this.showPaging(result.result, pageNumber);
      });
  }

  delete(branch: BranchDto): void {
    abp.message.confirm(
      this.l("Delete branch '") + branch.name + "'?",
      '',
      (result: boolean) => {
        if (result) {
          this._branchService.delete(branch.id).subscribe(() => {
            abp.notify.success(this.l('Deleted branch successfully'));
            this.refresh();
          });
        }
      }
    );
  }

  editBranch(branch: BranchDto): void {
    this.showDialogBranch('Edit',branch);
  }

  createBranch(): void {
    this.showDialogBranch('create',{});
  }

  showDialogBranch(command: string,item: any): void {
    let request = {} as BranchDto;
    if(command == "Edit") {
      request = {
        id: item.id,
        name: item.name,
        code: item.code,
        default: item.default
      }
    }
    const dialogRef = this.dialog.open(CreateEditBranchComponent, {
      data: {
        command: command,
        item: request
      },
      width: '500px',
      disableClose: true
    });
    dialogRef.afterClosed().subscribe(()=> {
      this.refresh();
    });
  }
}

export class BranchDto {
  id: number;
  name: string;
  code: string;
  default: boolean;
}


