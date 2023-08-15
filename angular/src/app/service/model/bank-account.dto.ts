import { AccountTypeEnum } from "@shared/AppEnums";
import { extend } from "jquery";

export class BankAccountDto {
  holderName: string;
  bankNumber: string;
  bankId: number;
  bankName: string;
  currencyId: number;
  currencyName: string;
  accountId: number;
  accountName: string;
  amount: number;
  id: number;
  baseBalance: number;
  currentBalance?: number;
  isActive: boolean;
  lockedStatus: boolean;
  accountTypeEnum: AccountTypeEnum;
}
export class BankAcountTransactionDto {
  startDate: string;
  endDate: string;
  beginningBalance: number;
  lastBalance: number;
  bankTransaction: [
    {
      id: number;
      transactionDate: string;
      bankTransactionName: string;
      increase: number;
      reduce: number;
      afterBalance: number;
    }
  ];
}
export class BankAccountForPeriodDto {
  holderName: string;
  id: number;
  baseBalance: number;
}
export class BankAccountForCreatePeriodAtFirstTimeDto extends BankAccountForPeriodDto {
  currentBalance: number;
}
export class FilterBankAccount
{
  currencyId: number;
  currencyNameOrCode: string;
  isActive: boolean;
  isAccountTypeNotCompany: boolean;
  orderByType: AccountTypeEnum;
}
