import { BankTransactionDto } from '@app/modules/banking-transaction/banking-transaction.component';
import { PERMISSIONS_CONSTANT } from '@app/constant/permission.constant';
import { Router } from '@angular/router';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { RevenueRecordService } from './../../../service/api/revenue-record.service';
import { BranchService } from '@app/service/api/branch.service';
import { AccountantAccountService } from '@app/service/api/accountant-account.service';
import { Component, Inject, OnInit } from '@angular/core';
import { RevenueService } from '@app/service/api/revenue.service';
import { catchError } from 'rxjs/operators';
import { AccountDto } from '@app/modules/accountant-account/accountant-account.component';
import { CurrencyService } from '@app/service/api/currency.service';

@Component({
  selector: 'app-create-edit-record',
  templateUrl: './create-edit-record.component.html',
  styleUrls: ['./create-edit-record.component.css']
})
export class CreateEditRecordComponent implements OnInit {
  incomeTypeList: any=[]
  accountList: AccountDto[]
  branchList: any
  isDisable: boolean = false
  searchIncomingType: string = ""
  searchAccountName: string = ""
  searchBranch: string = ""
  revenueRecord = {} as revenueRecordDto
  tempIncomeList: any
  tempAccountList: any
  tempBranchList: any
  transaction: BankTransactionDto
  searchCurrency: string = ""
  currencyList: any
  finalIncomeTypeList=[]

  constructor(private incomeService: RevenueService, private accountService: AccountantAccountService,
    private revenueRecordService: RevenueRecordService,
    private branchService: BranchService, @Inject(MAT_DIALOG_DATA) public data: any,
    public dialogRef: MatDialogRef<CreateEditRecordComponent>, private router: Router, private currencyService: CurrencyService) { }

  ngOnInit(): void {



    if (this.data.command == "edit") {
      this.revenueRecord = this.data.item
      this.getAccount()
      this.getCurrencyToEdit()
    }
    else {
      this.transaction = this.data.transaction
      this.getAccountToEdit()
      this.getCurrency()
    }

    this.getIncomeType()
    this.getBranch()
    this.fillDefault()


  }
  getCurrency() {
    this.currencyService.GetAllForDropdown().subscribe(data => (this.currencyList = data.result,
      this.revenueRecord.currencyId = data.result.filter(currency => currency.code == this.data.fromBankAccountCurrency)[0].id)
    )
  }
  getCurrencyToEdit() {
    this.currencyService.GetAllForDropdown().subscribe(data => (this.currencyList = data.result
    )
    )
  }
  fillDefault() {
    this.revenueRecord.name = this.transaction.name
    this.revenueRecord.value = this.transaction.toValue
  }
  filterRevenue() {
    this.incomeTypeList = this.tempIncomeList.filter(item => item.name.trim().toLowerCase().includes(this.searchIncomingType.trim().toLowerCase()))
  }
  filterAccountName() {
    this.accountList = this.tempAccountList.filter(item => item.name.trim().toLowerCase().includes(this.searchAccountName.trim().toLowerCase()))
  }
  filterBranch() {
    this.branchList = this.tempBranchList.filter(item => item.name.trim().toLowerCase().includes(this.searchBranch.trim().toLowerCase()))
  }
  getIncomeType() {
    this.incomeService.getAll().subscribe(data => (this.incomeTypeList = data.result, this.tempIncomeList = data.result,
      this.incomeTypeList= this.filterIncome(this.incomeTypeList),
      this.incomeTypeList.forEach(item => {
        this.filterData(item)
      })
      ))
  }
  filterData(data) {
    data.paddingLevel = ""
    for (let i = 1; i < data.level; i++) {
      data.paddingLevel += "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;"
    }
    this.finalIncomeTypeList.push(data)
    if (data.children.length > 0) {
      data.children.forEach(item => {
        this.filterData(item)
      })
    }
  }
  filterIncome(arr) {
    let rs = arr.filter((item) => {
        if (item.parentId == null) {
          return (item.children = arr.filter((child) => {
            return child.parentId == item.id;
          }));
        } else {
          return (item.children = arr.filter((child) => {
            return child.parentId == item.id;
          }));
        }
      }).filter((finalItem) => {
        return finalItem.parentId == null;
      });
    return rs;
  }
  getAccountToEdit() {
    this.accountService.getAll().subscribe(data => {
      this.accountList = data.result;
      this.tempAccountList = data.result;
      this.revenueRecord.accountId = this.accountList.filter(account => (account.default == true))[0].id})
  }
  getAccount() {
    this.accountService.getAll().subscribe(data => {
      this.accountList = data.result;
      this.tempAccountList = data.result;

    })
  }
  getBranch() {
    this.branchService.GetAllForDropdown().subscribe(data => (this.branchList = data.result, this.tempBranchList = data.result))
  }
  saveAndClose() {
    this.isDisable = true
    if (this.data.command == "create") {
      this.revenueRecord.status = false
      this.revenueRecord.bankTransactionId = this.data.transactionId
      this.revenueRecordService.create(this.revenueRecord)
        .pipe(catchError(this.revenueRecordService.handleError)).subscribe((res) => {
          abp.notify.success("created record ");
          this.reloadComponent()
          this.dialogRef.close();
        }, () => this.isDisable = false);
    }
    else {
      this.revenueRecordService.update(this.revenueRecord)
        .pipe(catchError(this.revenueRecordService.handleError)).subscribe((res) => {
          abp.notify.success("edited record ");
          this.reloadComponent()
          this.dialogRef.close();
        }, () => this.isDisable = false);
    }
  }
  reloadComponent() {
    this.router.navigateByUrl('', { skipLocationChange: true }).then(() => {
      this.router.navigate(['/app/detail'], {
        queryParams: {
          id: this.data.transactionId,
          index: 1
        }
      });
    });
  }
}
export class revenueRecordDto {
  incomingEntryTypeId: number;
  bankTransactionId: number;
  name: string;
  status: boolean;
  accountId: number;
  branchId: number;
  value: number;
  currencyId: number;
  currencyName: string;
  id: number;
}
