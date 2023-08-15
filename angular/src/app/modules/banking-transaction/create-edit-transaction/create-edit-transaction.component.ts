import { ExpenditureRequestService } from "./../../../service/api/expenditure-request.service";
import { BankAccountDto } from "./../../../service/model/bank-account.dto";
import {
  PagedListingComponentBase,
  PagedRequestDto,
} from "@shared/paged-listing-component-base";
import { BankAccountService } from "./../../../service/api/bank-account.service";
import { Router } from "@angular/router";
import { MatDialogRef, MAT_DIALOG_DATA } from "@angular/material/dialog";
import {
  Component,
  ElementRef,
  Inject,
  Injector,
  OnInit,
  ViewChild,
} from "@angular/core";
import { BankTransactionDto } from "../banking-transaction.component";
import { TransactionService } from "@app/service/api/transaction.service";
import { catchError, filter } from "rxjs/operators";
import { DateAdapter } from "@angular/material/core";
import * as moment from "moment";
import { RequestDetailService } from "@app/service/api/request-detail.service";

@Component({
  selector: "app-create-edit-transaction",
  templateUrl: "./create-edit-transaction.component.html",
  styleUrls: ["./create-edit-transaction.component.css"],
})
export class CreateEditTransactionComponent implements OnInit {
  transaction = {} as BankTransactionDto;
  bankAccountList: any;
  fromBankList: any;
  toBankList: any;
  searchFromBank: string = "";
  searchToBank: string = "";
  fromBankAccountName: string;
  toBankAccountName: string;
  hideToValue: boolean = true;
  hideFromValue: boolean = true;

  isDisable: boolean = false;
  searchList: any;
  fromBankCurrency: string = "";
  toBankCurrency: string = "";
  constructor(
    @Inject(MAT_DIALOG_DATA) public data: any,
    private transactionService: TransactionService,
    private bankAccountService: BankAccountService,
    private requestDetailService: RequestDetailService,
    public dialogRef: MatDialogRef<CreateEditTransactionComponent>,
    private outcomeService: ExpenditureRequestService,
    private router: Router
  ) {}
  ngOnInit(): void {
    this.getBankAccount();
    if (this.data.command == "edit") {
      this.transaction = this.data.item;
      this.fromBankCurrency = this.data.item.fromBankAccountCurrency;
      this.toBankCurrency = this.data.item.toBankAccountCurrency;
      if (this.transaction.fromBankAccountTypeCode == "COMPANY") {
        this.hideFromValue = false;
      }
      if (this.transaction.toBankAccountTypeCode == "COMPANY") {
        this.hideToValue = false;
      }
    }
  }

  getBankAccount() {
    this.bankAccountService.getAll().subscribe((apiData) => {
      this.fromBankList = apiData.result.map((item) => {
        item.holderName = `${item.holderName}(${item.currencyName}) ${item.bankNumber} [${item.accountTypeCode}]`;
        return item;
      });
      this.toBankList = [...this.fromBankList];
    });
  }
  setFromValue(accountCode) {
    if (accountCode == "COMPANY") {
      this.hideFromValue = false;
    } else {
      this.hideFromValue = true;
      this.transaction.fromValue = null;
    }
  }
  setToValue(accountCode) {
    if (accountCode == "COMPANY") {
      this.hideToValue = false;
    } else {
      this.hideToValue = true;
      this.transaction.toValue = null;
    }
  }
  setFromBankCurrency(value: string) {
    this.fromBankCurrency = value;
  }
  setToBankCurrency(value: string) {
    this.toBankCurrency = value;
  }
  getBankName() {
    this.bankAccountService.getAll().subscribe((data) => {
      data.result.forEach((item) => {
        if (item.id == this.transaction.fromBankAccountId) {
          this.fromBankAccountName = item.holderName;
        }
        if (item.id == this.transaction.toBankAccountId) {
          this.toBankAccountName = item.holderName;
        }
      });
    });
    this.transaction.fromBankAccountName = this.fromBankAccountName;
    this.transaction.toBankAccountName = this.toBankAccountName;
  }
  saveAndClose() {
    if (this.hideFromValue == true && this.hideToValue == false) {
      this.transaction.fromValue = this.transaction.toValue;
    } else if (this.hideFromValue == false && this.hideToValue == true) {
      this.transaction.toValue = this.transaction.fromValue;
    }
    this.transaction.fromBankAccountCurrency = this.fromBankCurrency;
    this.transaction.toBankAccountCurrency = this.toBankCurrency;
    if (this.transaction.transactionDate) {
      this.transaction.transactionDate = moment(
        this.transaction.transactionDate
      ).format("YYYY-MM-DD");
    }
    this.isDisable = true;
    this.getBankName();
    if (this.data.command == "create") {
      if (this.data.target == "detailRequest") {
        this.requestDetailService
          .addBankTransaction(this.data.outcomingEntryId, this.transaction)
          .pipe(catchError(this.transactionService.handleError))
          .subscribe(
            (res) => {
              abp.notify.success("create and linked transaction ");

              this.router
                .navigateByUrl("", { skipLocationChange: true })
                .then(() => {
                  this.router.navigate(
                    ["/app/requestDetail/relevant-transaction"],
                    {
                      queryParams: {
                        id: this.data.outcomingEntryId,
                      },
                    }
                  );
                });
              this.dialogRef.close();
            },
            () => (this.isDisable = false)
          );
      } else if (this.data.target == "transferMoney") {
        this.outcomeService
          .TransferToEnd(this.transaction)
          .pipe(catchError(this.transactionService.handleError))
          .subscribe(
            (res) => {
              abp.notify.success(res.result);

              this.reloadComponent();
              this.dialogRef.close();
            },
            () => (this.isDisable = false)
          );
      } else if (this.data.target == null) {
        this.transactionService
          .create(this.transaction)
          .pipe(catchError(this.transactionService.handleError))
          .subscribe(
            (res) => {
              abp.notify.success("created transaction ");
              this.reloadComponent();
              this.dialogRef.close();
            },
            () => (this.isDisable = false)
          );
      }
    } else {
      this.transactionService
        .updateBankTransaction(this.transaction)
        .pipe(catchError(this.transactionService.handleError))
        .subscribe(
          (res) => {
            abp.notify.success("edited transaction ");
            this.reloadComponent();
            this.dialogRef.close();
          },
          () => (this.isDisable = false)
        );
    }
  }
  reloadComponent() {
    this.router.navigateByUrl("", { skipLocationChange: true }).then(() => {
      this.router.navigate(["/app/bank-transaction"]);
    });
  }
  fromBankAccountSelectOpenedChange() {
    this.searchFromBank = "";
  }
  toBankAccountSelectOpenedChange() {
    this.searchToBank = "";
  }
}
