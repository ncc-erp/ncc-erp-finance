import { Component, Injector } from '@angular/core';
import { finalize } from 'rxjs/operators';
import { BsModalService, BsModalRef } from 'ngx-bootstrap/modal';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import {
  PagedListingComponentBase,
  PagedRequestDto,
} from '@shared/paged-listing-component-base';
import {
  TenantServiceProxy,
  TenantDto,
  TenantDtoPagedResultDto,
} from '@shared/service-proxies/service-proxies';
import { CreateTenantDialogComponent } from './create-tenant/create-tenant-dialog.component';
import { EditTenantDialogComponent } from './edit-tenant/edit-tenant-dialog.component';
import { PERMISSIONS_CONSTANT } from '@app/constant/permission.constant';

class PagedTenantsRequestDto extends PagedRequestDto {
  keyword: string;
  isActive: boolean | null;
}

const OPTION_ALL = -1;

@Component({
  templateUrl: './tenants.component.html',
  animations: [appModuleAnimation()],
  styleUrls: ["./tenants.component.css"],
})
export class TenantsComponent extends PagedListingComponentBase<TenantDto> {
  tenants: TenantDto[] = [];
  keyword = '';
  isActive: boolean | number = -1;
  advancedFiltersVisible = false;

  constructor(
    injector: Injector,
    private _tenantService: TenantServiceProxy,
    private _modalService: BsModalService
  ) {
    super(injector);
  }

  list(
    request: PagedTenantsRequestDto,
    pageNumber: number,
    finishedCallback: Function
  ): void {
    request.keyword = this.keyword;
    if (this.isActive !== OPTION_ALL) {
      request.isActive = this.isActive as boolean;
    }

    this._tenantService
      .getAll(
        request.keyword,
        request.isActive,
        request.skipCount,
        request.maxResultCount
      )
      .pipe(
        finalize(() => {
          finishedCallback();
        })
      )
      .subscribe((result: TenantDtoPagedResultDto) => {
        this.tenants = result.items;
        this.showPaging(result, pageNumber);
      });
  }

  delete(tenant: TenantDto): void {
    abp.message.confirm(
      this.l('TenantDeleteWarningMessage', tenant.name),
      undefined,
      (result: boolean) => {
        if (result) {
          this._tenantService
            .delete(tenant.id)
            .pipe(
              finalize(() => {
                abp.notify.success(this.l('SuccessfullyDeleted'));
                this.refresh();
              })
            )
            .subscribe(() => {});
        }
      }
    );
  }

  createTenant(): void {
    this.showCreateOrEditTenantDialog();
  }

  editTenant(tenant: TenantDto): void {
    this.showCreateOrEditTenantDialog(tenant.id);
  }

  showCreateOrEditTenantDialog(id?: number): void {
    let createOrEditTenantDialog: BsModalRef;
    if (!id) {
      createOrEditTenantDialog = this._modalService.show(
        CreateTenantDialogComponent,
        {
          class: 'modal-lg',
        }
      );
    } else {
      createOrEditTenantDialog = this._modalService.show(
        EditTenantDialogComponent,
        {
          class: 'modal-lg',
          initialState: {
            id: id,
          },
        }
      );
    }

    createOrEditTenantDialog.content.onSave.subscribe(() => {
      this.refresh();
    });

  }

  clearFilters(): void {
    this.keyword = '';
    this.isActive = undefined;
    this.getDataPage(1);
  }
  selectionActivechange(){
    if(this.isActive == null) this.isActive = undefined;
    this.getDataPage(1);
  }

  isShowCreateBtn(){
    return this.isGranted(PERMISSIONS_CONSTANT.Admin_Tenant_Create);
  }

  isShowEditBtn(){
    return this.isGranted(PERMISSIONS_CONSTANT.Admin_Tenant_Edit);
  }

  isShowDeleteBtn(){
    return this.isGranted(PERMISSIONS_CONSTANT.Admin_Tenant_Delete);
  }
}
