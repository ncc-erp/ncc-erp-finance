import { Component, Injector, OnInit } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { PERMISSIONS_CONSTANT } from '@app/constant/permission.constant';
import { InputFilterDto } from '@shared/filter/filter.component';
import { PagedListingComponentBase, PagedRequestDto, PagedResultResultDto } from '@shared/paged-listing-component-base';
import { finalize } from 'rxjs/operators';
import { AccountTypeService } from './../../service/api/account-type.service';
import { CreateEditAccountTypeComponent } from './create-edit-account-type/create-edit-account-type.component';

@Component({
  selector: 'app-account-type',
  templateUrl: './account-type.component.html',
  styleUrls: ['./account-type.component.css']
})
export class AccountTypeComponent extends PagedListingComponentBase<any>  {
  isActive: boolean | null;
  Directory_AccountType_Create = PERMISSIONS_CONSTANT.Directory_AccountType_Create;
  Directory_AccountType_Delete = PERMISSIONS_CONSTANT.Directory_AccountType_Delete;
  Directory_AccountType_Edit = PERMISSIONS_CONSTANT.Directory_AccountType_Edit;

  public readonly FILTER_CONFIG: InputFilterDto[] = [
    { propertyName: 'Name', comparisions: [0, 6, 7, 8], displayName: "filterDirectory.Name" },
    { propertyName: 'Code', comparisions: [0, 6, 7, 8], displayName: "filterDirectory.Code" },
  ];

  constructor(injector: Injector, private _accountTypeServices: AccountTypeService, private dialog: MatDialog) {
    super(injector);
  }
  accountTypes: AccountTypeDto[] = [];

  protected list(
    request: PagedRequestDto,
    pageNumber: number,
    finishedCallback: Function
  ): void {
    this._accountTypeServices
      .getAllPaging(request)
      .pipe(finalize(() => {
        finishedCallback();
      }))
      .subscribe((result: PagedResultResultDto) => {
        this.accountTypes = result.result.items;
        this.showPaging(result.result, pageNumber);
      });
  }
  delete(accountType: AccountTypeDto): void {
    abp.message.confirm(
      this.l("Delete account type '") + accountType.name + "'?",
      '',
      (result: boolean) => {
        if (result) {
          this._accountTypeServices.delete(accountType.id).subscribe(() => {
            abp.notify.success(this.l('Deleted account type ') + accountType.name + this.l('successfully'));
            this.refresh();
          });
        }
      }
    );
  }

  editAccountType(accountType: AccountTypeDto): void {
    this.showDialogAccountType('Edit',accountType);
  }

  createAccountType(): void {
    this.showDialogAccountType('create',{});
  }

  showDialogAccountType(command: string,item: any): void {
    let request = { } as AccountTypeDto;
    if(command == "Edit") {
      request = {
        id: item.id,
        name: item.name,
        code: item.code
      }
    }
    this.dialog.open(CreateEditAccountTypeComponent, {
      data: {
        command : command,
        item: request
      },
      width: '500px',
      disableClose: true
    });
  }

}

export class AccountTypeDto {
  id: number;
  name: string;
  code: string;
}
