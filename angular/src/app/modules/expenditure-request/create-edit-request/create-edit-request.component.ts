import { AppConsts } from '@shared/AppConsts';
import { SupplierDto } from './../../expenditure-request-detail/supplier/supplier.component';
import { SupplierService } from './../../../service/api/supplier.service';
import { Router } from '@angular/router';
import { catchError } from 'rxjs/operators';
import { ExpenditureRequestService } from './../../../service/api/expenditure-request.service';
import { BranchService } from './../../../service/api/branch.service';
import { AccountService } from './../../../service/api/account.service';
import { ExpenditureRequestDto, GetAccountCompanyForDropdownDto, OutcomingEntryDto } from './../expenditure-request.component';
import { Component, Inject, Injector, OnInit } from '@angular/core';
import { ExpenditureService } from '@app/service/api/expenditure.service';
import { MAT_DIALOG_DATA, MatDialogRef, MatDialog } from '@angular/material/dialog';
import { CurrencyService } from '@app/service/api/currency.service';
import { CreateEditSupplierComponent } from '@app/modules/supplier-list/create-edit-supplier/create-edit-supplier.component';
import { CommonService } from '@app/service/api/new-versions/common.service';
import { ValueAndNameModel } from '@app/service/model/common-DTO';
import { AppComponentBase } from '@shared/app-component-base';
import { LableDirection } from '@shared/selection-customs/selection-customs.component';
import { CheckWarningCreateRequestComponent } from '../check-warning-create-request/check-warning-create-request.component';

@Component({
  selector: 'app-create-edit-request',
  templateUrl: './create-edit-request.component.html',
  styleUrls: ['./create-edit-request.component.css']
})
export class CreateEditRequestComponent extends AppComponentBase implements OnInit {
  outcomeRequest = {} as ExpenditureRequestDto
  matchingRequests : OutcomingEntryDto[] = [];
  outcomingTypeList: any
  accountList: any
  allAccountList: GetAccountCompanyForDropdownDto[];
  branchList: any
  isDisable = false
  searchOutcome: string = ""
  searchAccount: string = ""
  searchBranch: string = ""
  currencyList: any
  searchCurrency: string = ""
  searchSupplier: string = ""
  supplierList: SupplierDto[]
  finalOutcomeTypeList=[]
  public selectionList = ["Read", "Write", "Total"];
  isEditing :boolean =false;
  LableDirectionLeft = LableDirection.Left;
  listCurrency: ValueAndNameModel[] = [];
  constructor(public dialogRef: MatDialogRef<CreateEditRequestComponent>, private router: Router,
    injector:Injector,
    private requestService: ExpenditureRequestService, private outcomeService: ExpenditureService, private accountService: AccountService,
    private commonService: CommonService,
    private branchService: BranchService, @Inject(MAT_DIALOG_DATA) public data: any, private currencyService: CurrencyService,
    private supplierService: SupplierService, private dialog: MatDialog) {
      super(injector);
     }

  ngOnInit(): void {
    this.outcomeRequest.currencyId = this.defaultCurrencyId;
    if (this.data.command == "edit") {
      this.outcomeRequest = this.data.item
      this.outcomeRequest.supplierId = this.data.item.supplierId
      this.isEditing = true;
      //this.getCurrencyToEdit()
    }
    else {
      // this.getCurrency();
      this.getAccountToEdit();
    }
    if(this.isEnableMultiCurrency){
      this.getAllCurrency();
    }
    this.getOutcomingType()
    this.getAccount()
    this.getBranch()
    this.getSupplier()
  }
  getAllCurrency(){
    this.commonService.getAllCurrency().subscribe((data) => {
      this.listCurrency = data.result;
    });
  }

  getOutcomingType() {
    this.outcomeService.GetAllForDropdownByUser().subscribe(data => (this.outcomingTypeList = data.result,
      this.outcomingTypeList = this.filterExpenditure(data.result), this.outcomingTypeList.forEach(item => {
        this.filterData(item)
      })),
    )
  }
  getAccount() {
    this.commonService.getAllAccountCompany().subscribe(data => {
      this.allAccountList = data.result;
    })
  }
  getBranch() {
    this.branchService.GetAllForDropdown().subscribe(data => this.branchList = data.result)
  }
  // getCurrencyToEdit() {
  //   this.currencyService.GetAllForDropdown().subscribe(data => (this.currencyList = data.result))
  // }
  // getCurrency() {
  //   this.currencyService.GetAllForDropdown().subscribe(data => (this.currencyList = data.result,
  //     this.outcomeRequest.currencyId = data.result.filter(currency => currency.code == AppConsts.DEFAULT_CURRENCY_CODE)[0].id))
  // }
  getSupplier() {
    this.supplierService.GetAllForDropdown().subscribe(data => (this.supplierList = data.result))
  }
  getSupplierToFill() {
    this.supplierService.GetAllForDropdown().subscribe(data => (this.supplierList = data.result,
      this.outcomeRequest.supplierId = data.result[data.result.length - 1].id)
    )
  }

  getAccountToEdit() {
    this.commonService.getAllAccountCompany().subscribe((data) => {
      this.allAccountList = data.result;
      const defaultCompany = data.result.find(s => s.isDefault);
      this.outcomeRequest.accountId = defaultCompany ? Number(defaultCompany.value) : Number(data.result[0].value);
    });
    // this.accountService.getAll().subscribe(item => {
    //   this.allAccountList = item.result.map(s =>{return {name: s.name, value: s.id}});
    //   this.outcomeRequest.accountId = this.allAccountList.filter(account => (account.default == true && account.accountTypeCode == "COMPANY"))[0].id;
    // })
  }

  filterData(data) {
    data.paddingLevel = ""
    for (let i = 1; i < data.level; i++) {
      data.paddingLevel += "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;"
    }
    this.finalOutcomeTypeList.push(data)
    if (data.children.length > 0) {
      data.children.forEach(item => {
        this.filterData(item)
      })
    }
  }
  createSupplier() {
    let dialogRef = this.dialog.open(CreateEditSupplierComponent, {
      data: {
        item: {},
        command: "create"
      },
      width: "700px"
    })
    dialogRef.afterClosed().subscribe(result => {
      if (result != '') {
        this.getSupplierToFill()
      }

    });
  }
  setAccreditation(e){
    if(e.checked == true){
      this.outcomeRequest.accreditation = true;
    }else{
      this.outcomeRequest.accreditation = false;
    }

    //console.log(this.outcomeRequest.accreditation)
  }
  saveAndClose() {
    this.isDisable = true
    if (this.data.command == "create") {
      this.requestService.create(this.outcomeRequest).pipe(catchError(this.requestService.handleError)).subscribe((res) => {
        abp.notify.success("created outcomeRequest ");
        this.dialogRef.close();
      }, () => this.isDisable = false);
    }
    else {
      this.requestService.update(this.outcomeRequest).pipe(catchError(this.requestService.handleError)).subscribe((res) => {
        abp.notify.success("edited outcomeRequest ");
        this.dialogRef.close();
      }, () => this.isDisable = false);
    }
  }
  filterExpenditure(arr) {
    let rs = arr
      .filter((item) => {
        if (item.parentId == null) {
          return (item.children = arr.filter((child) => {
            return child.parentId == item.id;
          }));
        } else {
          return (item.children = arr.filter((child) => {
            return child.parentId == item.id;
          }));
        }
      })
      .filter((finalItem) => {
        return finalItem.parentId == null;
      });
    return rs;
  }

  checkWarning()
  {
    this.getWarnings();
  }

  getWarnings()
  {
    this.requestService
        .checkWarningCreateRequest(this.outcomeRequest)
        .pipe(catchError(this.requestService.handleError))
        .subscribe(data => 
        {
          this.matchingRequests = data.result;
          if (this.matchingRequests.length == 0)
          {
            this.saveAndClose();
          }
          else
          {
            this.showWarningDialog();
          }
        });
  }

  showWarningDialog(){
    const checkWarningCreateRequestComponent = this.dialog.open(CheckWarningCreateRequestComponent, {
        data: this.matchingRequests,
        disableClose: false,
    });
  
    checkWarningCreateRequestComponent
    .afterClosed()
    .subscribe((proceedSave)=> 
    {
      if (proceedSave == true)
      {
        this.saveAndClose();
      }
    })
  }
}
