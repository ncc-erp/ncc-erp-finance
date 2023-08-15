import { Component, Injector, OnInit } from '@angular/core';
import { MatDialog, MatDialogRef } from '@angular/material/dialog';
import { AppConfigurationService } from '@app/service/api/app-configuration.service';
import { PeriodService } from '@app/service/api/period.service';
import { BankAccoutForCompanyDto } from '@app/service/model/common-DTO';
import { BankAccountInfosDto, CheckDiffRealBalanceAndBTransactionDto, CloseAndCreatePeriodDto, PeriodBankAccountForCloseAndCreateDto, PeriodDto, PreviewPeriodBeforeCloseDto } from '@app/service/model/period.dto';
import { AppComponentBase } from '@shared/app-component-base';
import { AppConsts } from '@shared/AppConsts';
import { BTransactionStatus } from '@shared/AppEnums';

@Component({
  selector: 'app-close-and-create-period',
  templateUrl: './close-and-create-period.component.html',
  styleUrls: ['./close-and-create-period.component.css']
})
export class CloseAndCreatePeriodComponent extends AppComponentBase implements OnInit {

  constructor(injector: Injector,
    private periodService: PeriodService,
    public dialogRef: MatDialogRef<CloseAndCreatePeriodComponent>,
    private settingService: AppConfigurationService,
    public dialog: MatDialog) {
    super(injector);
  }
  public btransPendingStatus:number = BTransactionStatus.PENDING
  public periodInfo = {} as PreviewPeriodBeforeCloseDto;
  public title = "";
  public period = {} as PeriodDto;
  public listBankAccounts: BankAccoutForCompanyDto[] = [];

  ngOnInit(): void {
    this.previewBeforeWhenClosePeriod();
    this.title = "Đóng kì hiện tại và tạo kì mới"
  }
  public previewBeforeWhenClosePeriod() {
    this.periodService.previewBeforeWhenClosePeriod().subscribe((rs) => {
      this.periodInfo = rs.result;
    })
  }
  public onCheck(account: BankAccountInfosDto) {
    let input = {
      bankAccountId: account.bankAccountId,
      bankAccountName: account.bankAccountName,
      bankNumber: account.bankNumber,
      currentBalance: account.currentBalance,
      balanceByBTransaction: account.balanceByBTransaction
    } as CheckDiffRealBalanceAndBTransactionDto;
    this.periodService.checkDiffRealBalanceAndBTransaction(input).subscribe((rs) => {
      account.balanceByBTransaction = rs.result.balanceByBTransaction;
      account.diffMoney = rs.result.diffMoney
    })
  }


  public onCloseAndCreate() {
    abp.message.confirm("Bạn có chắc là muốn đóng kì hiện tại và tạo kì mới không?", "", ((rs) => {
      if (rs) {
        let periodBankAccounts = this.periodInfo.bankAccountInfos.map(x => {
          return {
            bankAccountId: x.bankAccountId,
            baseBalance: x.currentBalance
          } as PeriodBankAccountForCloseAndCreateDto;
        })
        var input = {
          name: this.period.name,
          periodBankAccounts: periodBankAccounts
        } as CloseAndCreatePeriodDto;
        this.periodService.closeAndCreatePeriod(input).subscribe((rs) => {
          abp.notify.success("Close and create period success!");
          this.settingService.GetApplyToMultiCurrencyOutcome().subscribe(response => {
            if(!response.success) return;
            AppConsts.CONFIG_CURRENCY_OUTCOMINGENTRY = response.result == "true";
            AppConsts.periodId.next(AppConsts.RefreshPeriod);
          })
          this.dialogRef.close(true);
        });
      }
    }))
  }

  public onFillCurrentBalance(account: BankAccountInfosDto) {
    if (account.currentBalance) {
      account.checkMode = true;
    } else {
      account.checkMode = false;
    }
  }


}
