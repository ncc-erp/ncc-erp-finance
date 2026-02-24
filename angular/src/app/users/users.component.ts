import { Component, Injector } from '@angular/core';
import { finalize } from 'rxjs/operators';
import { BsModalService, BsModalRef } from 'ngx-bootstrap/modal';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import {
  PagedListingComponentBase,
  PagedRequestDto
} from 'shared/paged-listing-component-base';
import {
  UserServiceProxy,
  UserDto,
  UserDtoPagedResultDto
} from '@shared/service-proxies/service-proxies';
import { CreateUserDialogComponent } from './create-user/create-user-dialog.component';
import { EditUserDialogComponent } from './edit-user/edit-user-dialog.component';
import { ResetPasswordDialogComponent } from './reset-password/reset-password.component';
import { PERMISSIONS_CONSTANT } from '@app/constant/permission.constant';
import { TranslateService } from '@ngx-translate/core';
import { HttpParams } from '@angular/common/http';
import { ActivatedRoute } from '@angular/router';


const OPTION_ALL = -1

class PagedUsersRequestDto extends PagedRequestDto {
  keyword: string;
  isActive: boolean | null;
}

@Component({
  templateUrl: './users.component.html',
  animations: [appModuleAnimation()],
  styleUrls: ["./users.component.css"],
})
export class UsersComponent extends PagedListingComponentBase<UserDto> {
  users: UserDto[] = [];
  keyword = '';
  isActive: boolean | number = OPTION_ALL;
  advancedFiltersVisible = false;
  iconCondition: string = '';
  sortDrirect: number = -1;
  iconSort: string = '';
  routeTitleFirstLevel = this.APP_CONSTANT.TitleBreadcrumbFirstLevel.admin;
  routeUrlFirstLevel = this.APP_CONSTANT.UrlBreadcrumbFirstLevel.admin;
  routeTitleSecondLevel = this.APP_CONSTANT.TitleBreadcrumbSecondLevel.user;
  routeUrlSecondLevel = this.APP_CONSTANT.UrlBreadcrumbSecondLevel.user;
  
  constructor(
    injector: Injector,
    private _userService: UserServiceProxy,
    private _modalService: BsModalService
  ) {
    super(injector);

  }

  createUser(): void {
    this.showCreateOrEditUserDialog();
  }

  editUser(user: UserDto): void {
    this.showCreateOrEditUserDialog(user.id);
  }

  public resetPassword(user: UserDto): void {
    this.showResetPasswordUserDialog(user.id);
  }

  clearFilters(): void {
    this.keyword = '';
    this.isActive = undefined;
    this.getDataPage(1);
  }



  protected list(
    request: PagedUsersRequestDto,
    pageNumber: number,
    finishedCallback: Function
  ): void {
    request.keyword = this.keyword;
    if (this.isActive !== OPTION_ALL) {
      request.isActive = this.isActive as boolean;
    }
    this._userService
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
      .subscribe((result: UserDtoPagedResultDto) => {
        this.users = result.items;
        this.applySortUsers();
        this.showPaging(result, pageNumber);
      });
    this.updateBreadCrumb()
  }

  sortUser(column: 'emailAddress' | 'komuUserId') {
    if (this.iconCondition !== column) {
      this.sortDrirect = -1;
    }

    this.iconCondition = column;
    this.sortDrirect++;

    if (this.sortDrirect > 1) {
      this.iconCondition = '';
      this.iconSort = '';
      this.sortDrirect = -1;
    }

    if (this.sortDrirect === 1) {
      this.iconSort = 'fas fa-sort-amount-down';
    } else if (this.sortDrirect === 0) {
      this.iconSort = 'fas fa-sort-amount-up';
    } else {
      this.iconSort = 'fas fa-sort';
    }

    this.applySortUsers();
  }

  private applySortUsers() {
    if (!this.iconCondition || this.sortDrirect < 0) {
      this.refresh();
      return;
    }

    const factor = this.sortDrirect === 1 ? -1 : 1;
    this.users = [...this.users].sort((a, b) => {
      if (this.iconCondition === 'emailAddress') {
        const av = (a.emailAddress || '').toLowerCase().trim();
        const bv = (b.emailAddress || '').toLowerCase().trim();
        const lengthDiff = av.length - bv.length;
        if (lengthDiff !== 0) {
          return lengthDiff * factor;
        }
        if (av > bv) {
          return 1 * factor;
        }
        if (av < bv) {
          return -1 * factor;
        }
        return 0;
      }

      const av = a.komuUserId ?? Number.MIN_SAFE_INTEGER;
      const bv = b.komuUserId ?? Number.MIN_SAFE_INTEGER;
      return (av - bv) * factor;
    });
  }

  onRefreshCurrentPage(){
    this.clearFilters();
  }

  updateBreadCrumb() {
    this.listBreadCrumb = [
      { name: this.routeTitleFirstLevel , url: this.routeUrlFirstLevel },
      { name: ' <i class="fas fa-chevron-right"></i> ' },
      { name: this.routeTitleSecondLevel , url: this.routeUrlSecondLevel }
    ];
  }



  protected delete(user: UserDto): void {
    abp.message.confirm(
      this.l('UserDeleteWarningMessage', user.fullName),
      undefined,
      (result: boolean) => {
        if (result) {
          this._userService.delete(user.id).subscribe(() => {
            abp.notify.success(this.l('SuccessfullyDeleted'));
            this.refresh();
          });
        }
      }
    );
  }

  private showResetPasswordUserDialog(id?: number): void {
    this._modalService.show(ResetPasswordDialogComponent, {
      class: 'modal-lg',
      initialState: {
        id: id,
      },
    });
  }

  private showCreateOrEditUserDialog(id?: number): void {
    let createOrEditUserDialog: BsModalRef;
    if (!id) {
      createOrEditUserDialog = this._modalService.show(
        CreateUserDialogComponent,
        {
          class: 'modal-lg',
        }
      );
    } else {
      createOrEditUserDialog = this._modalService.show(
        EditUserDialogComponent,
        {
          class: 'modal-lg',
          initialState: {
            id: id,
          },
        }
      );
    }

    createOrEditUserDialog.content.onSave.subscribe(() => {
      this.refresh();
    });
  }
  selectionActivechange(){
    this.refresh();
  }
  isShowCreateBtn(){
    return this.isGranted(PERMISSIONS_CONSTANT.Admin_User_Create);
  }

  isShowEditBtn(){
    return this.isGranted(PERMISSIONS_CONSTANT.Admin_User_Edit);
  }

  isShowDeleteBtn(){
    return this.isGranted(PERMISSIONS_CONSTANT.Admin_User_Delete);
  }

  isShowResetPasswordBtn(){
    return this.isGranted(PERMISSIONS_CONSTANT.Admin_User_ResetPassword);
  }

  isShowMenuAction(){
    return this.isShowEditBtn() || this.isShowDeleteBtn() || this.isShowResetPasswordBtn();
  }
}

