import { Component, Injector, OnInit } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { PERMISSIONS_CONSTANT } from '@app/constant/permission.constant';
import { InputFilterDto } from '@shared/filter/filter.component';
import { PagedListingComponentBase, PagedRequestDto, PagedResultResultDto } from '@shared/paged-listing-component-base';
import { finalize } from 'rxjs/operators';
import { WorkFlowService } from './../../service/api/work-flow.service';
import { CreateEditWorkFlowComponent } from './create-edit-work-flow/create-edit-work-flow.component';
import { TranslateService } from '@ngx-translate/core';
import { HttpParams } from '@angular/common/http';
import { ActivatedRoute } from '@angular/router';
@Component({
  selector: 'app-work-flow',
  templateUrl: './work-flow.component.html',
  styleUrls: ['./work-flow.component.css']
})
export class WorkFlowComponent extends PagedListingComponentBase<any> implements OnInit {

  Admin_Workflow_Create =PERMISSIONS_CONSTANT.Admin_Workflow_Create
  Admin_Workflow_Delete=PERMISSIONS_CONSTANT.Admin_Workflow_Delete
  Admin_Workflow_ViewAll=PERMISSIONS_CONSTANT.Admin_Workflow_View
  Admin_Workflow_ViewDetail=PERMISSIONS_CONSTANT.Admin_Workflow_ViewDetail
  routeUrlFirstLevel = this.APP_CONSTANT.UrlBreadcrumbFirstLevel.Menu2;
  routeUrlSecondLevel = this.APP_CONSTANT.UrlBreadcrumbSecondLevel.workFlow;
  queryParams;

  workFlows = [];
  public readonly FILTER_CONFIG: InputFilterDto[] = [
    { propertyName: 'Name', comparisions: [0, 6, 7, 8], displayName: "filterWorkFlow.Name" },
    { propertyName: 'OutType', comparisions: [0, 6, 7, 8], displayName: "filterWorkFlow.OutType" },
  ];

  constructor(private route: ActivatedRoute, injector: Injector, private _workFlowServices: WorkFlowService, private dialog: MatDialog, private translate: TranslateService) {
    super(injector);
  }

  ngOnInit(): void {
    this.refresh();
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
    this.translate.get("menu2.workFlow").subscribe((res: string) => {
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

  protected list(request: PagedRequestDto, pageNumber: number, finishedCallback: Function): void {
    this._workFlowServices.getAllPaging(request).pipe(finalize(() => {
      finishedCallback();
    })).subscribe((result: PagedResultResultDto) => {
      this.workFlows = result.result.items;
      this.showPaging(result.result, pageNumber);
    })
  }
  delete(workFlow: WorkFlowDto): void {
    abp.message.confirm(
      this.l("Delete work flow '") + workFlow.name + "'?",
      '',
      (result: boolean) => {
        if (result) {
          this._workFlowServices.delete(workFlow.id).subscribe(() => {
            abp.notify.success(this.l('Deleted work flow ') + workFlow.name + this.l(' successfully'));
            this.refresh();
          });
        }
      }
    );
  }
  editWorkFlow(workFlow: WorkFlowDto): void {
    this.showDialogWorkFlow("Edit",workFlow);
  }

  createWorkFlow(): void {
    this.showDialogWorkFlow("create",{});
  }
  showDetail(id) {
    this.router.navigate(["/app/workFlowDetail"], {
      queryParams: {
        index: 0,
        id: id,
      },
    });
  }

  showDialogWorkFlow(command: string,item: any): void {
    let request = {} as WorkFlowDto;
    if(command == "Edit") {
      request = {
        id: item.id,
        name: item.name,
        outType: item.outType
      }
    }
    const dialogRef = this.dialog.open(CreateEditWorkFlowComponent, {
      data: {
        command: command,
        item: request
      },
      width: '600px',
      disableClose: true
    });

    dialogRef.afterClosed().subscribe(result => {
      this.refresh();
    });
  }
}
export class WorkFlowDto {
  id: number;
  name: string;
  outType: string;
}
