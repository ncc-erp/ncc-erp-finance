import { Component, OnInit, Injector } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { PERMISSIONS_CONSTANT } from '@app/constant/permission.constant';
import { AppComponentBase } from '@shared/app-component-base';
import { PagedListingComponentBase, PagedRequestDto } from '@shared/paged-listing-component-base';
import { catchError } from 'rxjs/operators';
import { StatusService } from '../../service/api/status.service';
import { CreateEditDialogStatusComponent } from './create-edit-dialog-status/create-edit-dialog-status.component';
import { TranslateService } from '@ngx-translate/core';
import { ActivatedRoute } from '@angular/router';
import { HttpParams } from '@angular/common/http';
@Component({
  selector: 'app-status',
  templateUrl: './status.component.html',
  styleUrls: ['./status.component.css']
})
export class StatusComponent extends AppComponentBase implements OnInit {
  statuses: StatusDto[] = [];
  title: any;
  routeTitleFirstLevel;
  routeUrlFirstLevel = this.APP_CONSTANT.UrlBreadcrumbFirstLevel.Menu2;
  routeUrlSecondLevel = this.APP_CONSTANT.UrlBreadcrumbSecondLevel.status;
  Admin_WorkflowStatus = PERMISSIONS_CONSTANT.Admin_WorkflowStatus;
  Admin_WorkflowStatus_Create = PERMISSIONS_CONSTANT.Admin_WorkflowStatus_Create;
  Admin_WorkflowStatus_Delete = PERMISSIONS_CONSTANT.Admin_WorkflowStatus_Delete;
  Admin_WorkflowStatus_Edit = PERMISSIONS_CONSTANT.Admin_WorkflowStatus_Edit;
  Admin_WorkflowStatus_ViewAll = PERMISSIONS_CONSTANT.Admin_WorkflowStatus_View;
  queryParams;

  constructor(private route: ActivatedRoute,private statusService: StatusService,private dialog: MatDialog,injector: Injector, private translate: TranslateService) {
    super(injector);
  }

  ngOnInit(): void {
    this.getStatus();
    this.translate.onLangChange.subscribe(() => {
      this.onLangChange();
    });
    this.route.queryParams.subscribe(params => {
      this.queryParams = new HttpParams({ fromObject: params });
      this.onLangChange();
    });
  }

  onLangChange(){
    this.translate.get("menu.menu2").subscribe((res: string) => {
      this.routeTitleFirstLevel = res;
      this.updateBreadCrumb();
    });
    this.translate.get("menu2.status").subscribe((res: string) => {
      this.title = res;
      this.updateBreadCrumb();
    });
  }

  updateBreadCrumb() {
    let queryParamsString = this.queryParams.toString();
    this.listBreadCrumb = [
      { name: this.routeTitleFirstLevel , url: this.routeUrlFirstLevel },
      { name: ' <i class="fas fa-chevron-right"></i> ' },
      { name: this.title , url: this.routeUrlSecondLevel + (queryParamsString ? '?' + queryParamsString : '')}
    ];
  }
  getStatus(): void {
    this.statusService.getAll().subscribe((item) => {
      this.statuses = item.result;
    })
  }
  createStatus(): void {
    this.showDialog("create",{});
  }
  editStatus(status: StatusDto): void {
    this.showDialog("edit",status)
  }
  showDialog(command: String, item: any): void {
    let request = {} as StatusDto
    if (command == "edit") {
      request = {
        name: item.name,
        code: item.code,
        id: item.id
      }
    }
    this.dialog.open(CreateEditDialogStatusComponent, {
      data: {
        item: request,
        command: command,
      },
      width: "700px",
      disableClose: true,
    });
  }
  deleteStatus(item: StatusDto): void {
    abp.message.confirm(
      "Delete status '" + item.name + "'?",
      "",
      (result: boolean) => {
        if (result) {
          this.statusService.delete(item.id).pipe(catchError(this.statusService.handleError)).subscribe(() => {
            abp.notify.success("Deleted status: " + item.name);
            this.getStatus();          }
            );
        }
      }
    );
  }
}
export class StatusDto {
  name: string;
  code: string;
  id: number;
}
