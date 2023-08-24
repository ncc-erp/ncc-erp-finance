import { Component, Inject, Injector, OnInit } from "@angular/core";
import { MatDialogRef, MAT_DIALOG_DATA } from "@angular/material/dialog";
import { AppComponentBase } from "@shared/app-component-base";
import { catchError } from "rxjs/operators";
import { AccountTypeService } from "./../../../service/api/account-type.service";
import { AccountService } from "./../../../service/api/account.service";
import { NewAccountDto } from "../accountant-account.component";
import { Router } from "@angular/router";
import { ValueAndNameModel } from "@app/service/model/common-DTO";
import { CommonService } from "@app/service/api/new-versions/common.service";
import { MatGridTileHeaderCssMatStyler } from "@angular/material/grid-list";
import { AccountTypeEnum } from "@shared/AppEnums";
@Component({
  selector: "app-create-edit-accountant-account",
  templateUrl: "./create-edit-accountant-account.component.html",
  styleUrls: ["./create-edit-accountant-account.component.css"],
})
export class CreateEditAccountantAccountComponent
  extends AppComponentBase
  implements OnInit
{
  isDisable: boolean = false;
  account = {} as NewAccountDto;
  listAccounTypeEnum: ValueAndNameModel[] = [];
  bankAccount: ValueAndNameModel[] = [];
  currency: ValueAndNameModel[] = [];
  tempbankAccount: ValueAndNameModel[] = [];
  tempcurrency: ValueAndNameModel[] = [];
  typeCOMPANY = AccountTypeEnum.COMPANY;
  name = "";
  constructor(
    @Inject(MAT_DIALOG_DATA) public data: any,
    injector: Injector,
    private _accountTypeService: AccountTypeService,
    private _accountService: AccountService,
    private router: Router,
    private _common: CommonService,
    public dialogRef: MatDialogRef<CreateEditAccountantAccountComponent>
  ) {
    super(injector);
  }
  isEdit: boolean;
  accountTypes = [] as AccountTypeDto[];

  ngOnInit(): void {
    this.account = this.data.item;
    this.isEdit = this.data.item.id === undefined ? false : true;
    this.getAllAccountType();
    this.getAllAccounTypeEnum();
    this.getAllBankAccount();
    this.getAllCurrency();
    this.account.bankNumber = this.getRandomBankNumber();
  }

  getAllAccountType(): void {
    this._accountTypeService.getAll().subscribe((data) => {
      this.accountTypes = data.result;
    });
  }
  getAllBankAccount() {
    this._common.getAllBank().subscribe((data) => {
      this.bankAccount = this.tempbankAccount = data.result;
    });
  }
  isShowBankAccount(){
    return !this.isEdit && this.typeCOMPANY != Number(this.account.type)
  }
  bankNumberChange(event: any) {
    // if(!Number(event.key)){
    //   this.account.bankNumber = this.account.bankNumber.substring(0, this.account.bankNumber.length - 1)
    // }
  }
  getAllCurrency() {
    this._common.getAllCurrency().subscribe((data) => {
      this.currency = this.tempcurrency = data.result;
      if (!this.isEdit) {
        for (let i = 0; i < data.result.length; i++) {
          if (data.result[i].name === "VND") {
            this.account.currencyId = Number(data.result[i].value);
          }
        }
      }
    });
  }
  saveAndClose() {
    this.isDisable = true;
    if (this.data.command == "create") {
      this._accountService
        .create(this.account)
        .pipe(catchError(this._accountService.handleError))
        .subscribe(
          (res) => {
            abp.notify.success("created account successfully");
            this.dialogRef.close();
          },
          () => (this.isDisable = false)
        );
    } else {
      this._accountService
        .update(this.account)
        .pipe(catchError(this._accountService.handleError))
        .subscribe(
          (res) => {
            abp.notify.success("edited account successfully");
            this.dialogRef.close();
          },
          () => (this.isDisable = false)
        );
    }
  }

  private getAllAccounTypeEnum(): void {
    this._common.getAllAccounTypeEnum().subscribe((response) => {
      if (!response.success) return;
      this.listAccounTypeEnum = response.result;
    });
  }
  getRandomBankNumber() {
    return Math.floor(Math.random() * 1000000000000).toString();
  }
  nameChange() {
    const code = this.getCodeByName(this.name);
    if (this.account.code == code || !this.account.code) {
      this.account.code = this.getCodeByName(this.account.name);
    }
    if (this.account.holderName == this.name || !this.account.holderName) {
      this.account.holderName = this.account.name;
    }
    this.name = this.account.name;
  }
  getCodeByName(name: string) {
    return this.removeVietnameseTones(name.toLocaleLowerCase().trim()).replace(
      / /g,
      "_"
    );
  }
  accountTypeChange() {
    if (!this.account.accountTypeId || !this.listAccounTypeEnum.length) return;

    const accountTypeCode = this.accountTypes.find(
      (s) => s.id == this.account.accountTypeId
    ).code;
    const newType = Number(
      this.listAccounTypeEnum.find((x) => x.name == accountTypeCode).value
    );
    this.account.type = newType;
  }
}

export class AccountTypeDto {
  id: number;
  name: string;
  code: string;
}
