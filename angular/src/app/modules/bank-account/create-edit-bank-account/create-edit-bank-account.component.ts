import { Component, Inject, Injector, OnInit } from "@angular/core";
import { MatDialogRef, MAT_DIALOG_DATA } from "@angular/material/dialog";
import { AccountantAccountService } from "@app/service/api/accountant-account.service";
import { BankAccountService } from "@app/service/api/bank-account.service";
import { BankService } from "@app/service/api/bank.service";
import { CurrencyService } from "@app/service/api/currency.service";
import { CommonService } from "@app/service/api/new-versions/common.service";
import { AccountForDropdownDto, ValueAndNameModel } from "@app/service/model/common-DTO";
import { AppComponentBase } from "@shared/app-component-base";
import { Subscriber } from "rxjs";

@Component({
  selector: "app-create-edit-bank-account",
  templateUrl: "./create-edit-bank-account.component.html",
  styleUrls: ["./create-edit-bank-account.component.css"],
})
export class CreateEditBankAccountComponent
  extends AppComponentBase
  implements OnInit
{
  constructor(
    @Inject(MAT_DIALOG_DATA) public data: any,
    private bankService: BankService,
    private bankAccountService: BankAccountService,
    private currencyService: CurrencyService,
    private accountService: AccountantAccountService,
    private commonService: CommonService,
    injector: Injector,
    public dialogRef: MatDialogRef<CreateEditBankAccountComponent>
  ) {
    super(injector);
  }
  banks: [];
  accounts: AccountForDropdownDto[];
  currencies: ValueAndNameModel[];
  searchValue: "";
  searchValue2: "";
  searchValue3: "";
  isEdit: boolean;
  isDisabled: boolean = false;
  ngOnInit(): void {
    this.isEdit = this.data.id === undefined ? false : true;
    this.getAllAccount();
    this.getAllBank();
    this.getAllCurrency();
    if(this.data.id === undefined ){
      this.data.baseBalance = 0;
    }
  }
  getAllBank() {
    this.bankService.getAll().subscribe((data) => {
      this.banks = data.result;
    });
  }
  getAllCurrency() {
    this.commonService.getAllCurrency().subscribe((data) => {
      this.currencies = data.result;
    });
  }

  getAllAccount() {
    // this.accountService.getAll().subscribe((data) => {
    //   this.accounts = data.result;
    // });
    this.commonService.getAllAccounts(this.isEdit).subscribe((data) => {
      this.accounts = data.result;
    });
  }

  save() {
    if (!this.data.bankId || !this.data.currencyId || !this.data.accountId) {
      return;
    }
    this.isDisabled = false;
    if (this.data.id == null) {
      this.bankAccountService.create(this.data).subscribe((res) => {
        this.notify.success("Create bank account successfully");
        this.dialogRef.close();
      });
    } else {
      this.bankAccountService.update(this.data).subscribe((res) => {
        this.notify.success("Update bank account successfully");
        this.dialogRef.close();
      });
    }
  }
}
