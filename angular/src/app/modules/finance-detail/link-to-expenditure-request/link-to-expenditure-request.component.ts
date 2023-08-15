import { ExpenditureRequestService } from '@app/service/api/expenditure-request.service';
import { PagedListingComponentBase, PagedRequestDto } from '@shared/paged-listing-component-base';
import { StatusDto } from './../../work-flow/create-edit-transition/create-edit-transition.component';
import { WorkFlowStatusService } from '@app/service/api/work-flow-status.service';
import { Component, Inject, OnInit, Injector } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { ActivatedRoute, Router } from '@angular/router';
import { catchError, finalize } from 'rxjs/operators';
import * as _ from 'lodash';

import { OutcomingEntryBankTransactionServiceService } from '../../../service/api/outcoming-entry-bank-transaction-service.service';
@Component({
  selector: 'app-link-to-expenditure-request',
  templateUrl: './link-to-expenditure-request.component.html',
  styleUrls: ['./link-to-expenditure-request.component.css']
})
export class LinkToExpenditureRequestComponent extends PagedListingComponentBase<expenditureRequestDto> implements OnInit {
  protected list(request: PagedRequestDto, pageNumber: number, finishedCallback: Function): void {
    this.requestParam = request
    this.isTableLoading = true
    request.maxResultCount =200
    request.sort = this.createDate;
    request.sortDirection = this.sortDrirect;
    this.requestService.getAllPagingByStatus(request, this.selectedStatus,false).pipe(finalize(() => {
      finishedCallback();
    })).subscribe(data => {
      this.expenditureRequestList = data.result.items;
      this.expenditureRequestList = this.expenditureRequestList.filter(item=>{
        return !this.outcomingEntrysByTransaction.map(outcome=>{
          return outcome.id
        }).includes(item.id)
      })
      this.showPaging(data.result, pageNumber);
      this.isTableLoading =false
    },
    ()=>{this.isTableLoading= false});

  }
  protected delete(entity: any): void {
    throw new Error('Method not implemented.');
  }
  outcomingEntrysByTransaction
  requestParam: PagedRequestDto
  createDate: string
  sortDrirect: number
  expenditureRequestList:expenditureRequestDto[] = [];
  isDisable: boolean = false;
  isEdit: boolean = false;
  indexEx = []
  bankTransactionId;
  statusList:StatusDto[]=[]
  selectedStatus:string=""
  expenditureRequest=[]
  outcomingEntrysByStatusEnd: expenditureRequestDto[] = [];
  constructor(public outComingEntryBankTransactionService: OutcomingEntryBankTransactionServiceService, private statusService:WorkFlowStatusService,
    public dialogRef: MatDialogRef<LinkToExpenditureRequestComponent>,  private requestService: ExpenditureRequestService, private outcomingEntryBankTransactionServiceService: OutcomingEntryBankTransactionServiceService,
    @Inject(MAT_DIALOG_DATA) public data: any,
    public route: ActivatedRoute, injector:Injector) {
      super(injector)
     }

  ngOnInit(): void {
    this.bankTransactionId = this.route.snapshot.queryParamMap.get('id');
    // this.getOutcomingEntryByStatusEnd();

    this.getStatusList()
    this.getOutComingEntryByTransaction(this.bankTransactionId)
  }
  getOutcomingEntryByStatusEnd(): void {
    this.outComingEntryBankTransactionService.getAllOutcomingEntryByStatusEND(this.bankTransactionId).subscribe(item => {
      this.outcomingEntrysByStatusEnd = item.result;
    })
  }
  checkval(outComingEntryId: number, event: any) {
    let body = {}
    if (event.checked == true) {
      body = {
        bankTransactionId: this.bankTransactionId,
        outComingEntryId: outComingEntryId
      }
      this.expenditureRequest.push(body);
      this.expenditureRequest = _.uniqBy(this.expenditureRequest, "outComingEntryId");
    }
    else {
      let temp = this.expenditureRequest.filter(item => item.outComingEntryId == outComingEntryId)[0];
      let index = this.expenditureRequest.indexOf(temp);
      this.expenditureRequest.splice(index, 1);
    }
  }

  saveAndClose() {
    this.isDisable = true;
    this.outComingEntryBankTransactionService.saveMultipleRequest(this.expenditureRequest).pipe(catchError(this.outComingEntryBankTransactionService.handleError)).subscribe((res) => {
      abp.notify.success("Link to expenditure request successfully");
      this.dialogRef.close();
    }, () => this.isDisable = false);
  }
  getStatusList(){
    this.statusService.getAll().subscribe(data=>{
      this.statusList = data.result
    })
  }

  getOutComingEntryByTransaction(id: number): void {
    this.outcomingEntryBankTransactionServiceService.getAllOutcomingEntryByTransaction(id).subscribe(item => {
      this.outcomingEntrysByTransaction = item.result;
      this.refresh();
    });

  }

}


export class expenditureRequestDto {
  outcomingEntryTypeId: number;
  name: string;
  requester: string;
  branchId: number;
  branchName: string;
  accountId: number;
  accountName: string;
  value: number;
  workflowStatusId: number;
  workflowStatusName: string;
  workflowStatusCode: string;
  outcomingEntryTypeCode: string;
  currencyId: number;
  currencyName: string;
  supplierId: number;
  id: number
}
