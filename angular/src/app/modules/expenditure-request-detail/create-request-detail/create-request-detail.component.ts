import { ExpenditureRequestDto } from './../../expenditure-request/expenditure-request.component';
import { RequestDetailService } from './../../../service/api/request-detail.service';
import { Router, ActivatedRoute } from '@angular/router';
import { catchError } from 'rxjs/operators';
import { MatDialog, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { AccountService } from './../../../service/api/account.service';
import { Component, OnInit, Inject } from '@angular/core';
import { ExpenditureRequestService } from '@app/service/api/expenditure-request.service';
import { RequestDetailDto, SendTempOutcomingEntryDetailDto } from '../detail-tab/detail-tab.component';
import { BranchService } from '@app/service/api/branch.service';

@Component({
  selector: 'app-create-request-detail',
  templateUrl: './create-request-detail.component.html',
  styleUrls: ['./create-request-detail.component.css']
})
export class CreateRequestDetailComponent implements OnInit {
  requestDetail = {} as RequestDetailDto
  accountList: any
  isDisable = false
  outcomingEntryId: string
  searchBranch: string;
  branchList: any
  tempBranchList: any;
  isUpdate: boolean;
  outcomeEntryRequest:ExpenditureRequestDto
  constructor(private accountService: AccountService, private branchService: BranchService, private requestService: ExpenditureRequestService,public dialog: MatDialog,
    @Inject(MAT_DIALOG_DATA) public data: any, public dialogRef: MatDialogRef<CreateRequestDetailComponent>,
    private router: Router, private route: ActivatedRoute, private detailService: RequestDetailService) { }
  searchAccount: string = ""
  ngOnInit(): void {
    if (this.data.command == "edit") {
      this.requestDetail = this.data.item
    }
    this.outcomeEntryRequest =this.data.request
    this.getAllAccount()
    this.requestDetail.outcomingEntryId = this.data.item.id;
    this.getAllBranch();
  }
  getAllAccount() {
    this.accountService.getAll().subscribe(data => (this.accountList = data.result))
  }
  saveAndClose() {
    if(this.data.isTemp) {
      this.saveTempAndClose();
      return;
    }
    this.requestDetail.total = this.requestDetail.unitPrice * this.requestDetail.quantity
    this.isDisable = true
    if (this.data.command == "create") {
      this.detailService.create(this.requestDetail).pipe(catchError(this.requestService.handleError)).subscribe((res) => {
        abp.notify.success("created transaction ");
        this.reloadComponent()
        this.dialogRef.close();
      }, () => this.isDisable = false);
    }
    else {
      this.detailService.update(this.requestDetail).pipe(catchError(this.detailService.handleError)).subscribe((res) => {
        abp.notify.success("edited transaction ");
        this.reloadComponent()
        this.dialogRef.close();
      }, () => this.isDisable = false);
    }
  }
  saveTempAndClose(){
    this.isDisable = true
    const payload = {
      accountId : this.requestDetail.accountId,
      branchId : this.requestDetail.branchId,
      name : this.requestDetail.name,
      quantity: this.requestDetail.quantity,
      outcomingEntryId: this.requestDetail.outcomingEntryId,
      rootTempOutcomingEntryId : this.data.rootTempOutcomingEntryId,
      total: this.requestDetail.unitPrice * this.requestDetail.quantity,
      unitPrice: this.requestDetail.unitPrice
    } as SendTempOutcomingEntryDetailDto
    if (this.data.command == "create") {
      this.requestService.createTempOutcomingEntryDetail(payload).subscribe((res) => {
        abp.notify.success("created transaction ");
        this.reloadComponent()
        this.dialogRef.close();
      }, () => this.isDisable = false);
    }
    else {
      this.requestService.updateTempOutcomingEntryDetail(payload).subscribe((res) => {
        abp.notify.success("edited transaction ");
        this.reloadComponent()
        this.dialogRef.close();
      }, () => this.isDisable = false);
    }
  }

  reloadComponent() {
    this.router.navigateByUrl('', { skipLocationChange: true }).then(() => {
      this.router.navigate(['/app/requestDetail/detail'], {
        queryParams: {
          id: this.data.item.id,
        }
      });
    });
  }
  getAllBranch() {
    this.branchService.GetAllForDropdown().subscribe(data => (this.branchList = data.result, this.tempBranchList = data.result))
  }
  filterBranch() {
    this.branchList = this.tempBranchList.filter(item => item.name.trim().toLowerCase().includes(this.searchBranch.trim().toLowerCase()))
  }
  branchSelectOpenedChange(){
    if(this.branchList.length) return;
    this.searchBranch = "";
    this.filterBranch();
  }
}
