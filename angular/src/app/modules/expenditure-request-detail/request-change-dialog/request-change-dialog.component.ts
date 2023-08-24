import { HistoryRequestChangeInfomation } from "./../../expenditure-request/expenditure-request.component";
import {
  Component,
  Inject,
  Injector,
  Input,
  OnInit,
  ViewChild,
} from "@angular/core";
import { MatDialogRef, MAT_DIALOG_DATA } from "@angular/material/dialog";
import {
  DataRequestChangeDialog,
  NewExpenditureRequestDto,
} from "@app/modules/expenditure-request/expenditure-request.component";
import { ExpenditureRequestService } from "@app/service/api/expenditure-request.service";
import { AppComponentBase } from "@shared/app-component-base";
import { RequestChangeMainTabComponent } from "./request-change-main-tab/request-change-main-tab.component";

@Component({
  selector: "app-request-change-dialog",
  templateUrl: "./request-change-dialog.component.html",
  styleUrls: ["./request-change-dialog.component.css"],
})
export class RequestChangeDialogComponent
  extends AppComponentBase
  implements OnInit
{
  public mainEditMode = false;
  public detailEditMode = false;
  public selectedIndex: number;
  public status: StatusInfo;
  public isLoadding = true;

  isViewHistory: boolean;
  tempId?: number;
  currentRequestChange?: HistoryRequestChangeInfomation;
  previousRequestChange?: HistoryRequestChangeInfomation;
  expenditureRequest: NewExpenditureRequestDto;
  canCRUD: boolean = true;
  valueDifference: number;

  @ViewChild("requestChangeMain")
  requestChangeMain: RequestChangeMainTabComponent;

  constructor(
    injector: Injector,
    public dialogRef: MatDialogRef<RequestChangeDialogComponent>,
    private requestService: ExpenditureRequestService,
    @Inject(MAT_DIALOG_DATA) private data: DataRequestChangeDialog
  ) {
    super(injector);
    Object.assign(this, this.data);
  }

  ngOnInit(): void {
    this.setCanCRUD();
    this.selectedIndex = this.data.selectedIndex;
  }
  setCanCRUD(): void {
    if (this.isViewHistory) this.canCRUD = false;
  }
  setDetailEditMode(event: boolean) {
    this.detailEditMode = !event;
  }
  setMainEditMode(event: boolean) {
    this.mainEditMode = event;
  }
  isEditMode() {
    return this.detailEditMode || this.mainEditMode;
  }

  sendRequestChange() {
    this.isLoadding = true;
    this.requestChangeMain.sendRequestChange();
  }
  rejectRequestChange() {
    this.isLoadding = true;
    this.requestChangeMain.rejectRequestChange();
  }
  approveRequestChange() {
    this.isLoadding = true;
    this.requestChangeMain.approveRequestChange();
  }
  changeStatus(status: StatusInfo) {
    if (this.isViewHistory) return;
    this.status = status;
    this.getRequest();
  }
  getRequest() {
    this.requestService.getById(this.expenditureRequest.id).subscribe(
      (data) => {
        this.data = data.result;
      },
      () => {}
    );
    this.isLoadding = false;
  }
  reloadRequestChange() {
    if (this.isViewHistory) return;
    this.requestChangeMain.getRequestChangeOutcomingEntry();
    this.isLoadding = false;
  }
  isCloseChange(isClose: boolean) {
    if (isClose) this.dialogRef.close();
  }

  getDifferenceValue(value: number) {
    this.valueDifference = value;
  }

}
export class StatusInfo {
  Name: string;
  Code: string;
}
