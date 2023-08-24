import { AppConsts } from '@shared/AppConsts';
import { Router } from '@angular/router';
import { AppComponentBase } from '@shared/app-component-base';
import { Component, Inject, Injector, OnInit } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialog, MatDialogRef } from '@angular/material/dialog';
import { CreateEditRequestComponent } from '../create-edit-request/create-edit-request.component';
import { ExpenditureRequestService } from '@app/service/api/expenditure-request.service';
import { ExpenditureService } from '@app/service/api/expenditure.service';
import { AccountService } from '@app/service/api/account.service';
import { CommonService } from '@app/service/api/new-versions/common.service';
import { BranchService } from '@app/service/api/branch.service';
import { SupplierService } from '@app/service/api/supplier.service';
import { CurrencyService } from '@app/service/api/currency.service';
import { ExpenditureRequestDto, OutcomingEntryDto } from '../expenditure-request.component';
import { PERMISSIONS_CONSTANT } from '@app/constant/permission.constant';

@Component({
    selector: 'app-check-warning-create-request',
    templateUrl: './check-warning-create-request.component.html',
    styleUrls: ['./check-warning-create-request.component.css']
})

  export class CheckWarningCreateRequestComponent extends AppComponentBase implements OnInit{

    requests : OutcomingEntryDto[];
    isProceedSave: boolean = false;

    Finance_OutcomingEntry_ViewDetail =
      PERMISSIONS_CONSTANT.Finance_OutcomingEntry_OutcomingEntryDetail;

    constructor(public dialogRef: MatDialogRef<CreateEditRequestComponent>, private router: Router,
        injector:Injector,
        private requestService: ExpenditureRequestService, private outcomeService: ExpenditureService, private accountService: AccountService,
        private commonService: CommonService,
        private branchService: BranchService, @Inject(MAT_DIALOG_DATA) public data: any, private currencyService: CurrencyService,
        private supplierService: SupplierService, private dialog: MatDialog) {
          super(injector);
         }
    
    ngOnInit(): void{
        this.requests = this.data;
    }

    proceedToSave(): void{
      this.isProceedSave = true;
      this.dialogRef.close(this.isProceedSave);
    }

    showDetail(id: any) {
      if (this.permission.isGranted(this.Finance_OutcomingEntry_ViewDetail)) {
        this.router.navigate(["app/requestDetail/main"], {
          queryParams: {
            id: id,
          },
        });
      }
    }
  }

