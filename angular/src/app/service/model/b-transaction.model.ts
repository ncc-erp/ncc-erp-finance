import { CurrencyNeedConvert } from '@app/modules/new-versions/b-transaction/payment-dialog/payment-dialog.component';
import { ILastModifiedAudited } from '../interfaces/last-modified-audited.interface';
export class BTransaction implements ILastModifiedAudited {
    lastModifiedTime: Date;
    lastModifiedUserId: number;
    lastModifiedUser: string;
    bTransactionId: number;
    bankAccountId: number;
    bankAccountNumber: string;
    bankAccountName: string;
    currencyId: number;
    currencyName: string;
    currencyMoney: string;
    money: string;
    timeAt: Date;
    bTransactionStatusName: string;
    note: string;
    bTransactionStatus: number;
    moneyNumber: number;
    bankTransactionId: number;
    bankTransactionName: number;
}
export class PaymentInvoiceForAccount {
    bTransactionId: number;
    accountId: number;
    incomingEntryTypeId: number;
    incomingEntryValue: number;
    incomingEntryName: string;
    isCreateBonus: boolean;
    currencyNeedConverts: CurrencyNeedConvert[];
}
export class PaymentInvoiceForAccountMapping {
  bTransactionId: number;
  accountId: number;
  isCreateBonus: boolean;
  incomingEntryTypeId?: number;
  incomingEntryName: string;
  incomingEntryValue?: number;
  customerAdvanceValue?: number;
  currencyNeedConverts: CurrencyNeedConvert[] = [];
  invoiceMappings: InvoicePaymentMapping[] = [];
}

export class InvoicePaymentMapping {
  invoiceId: number;
  value: number;
}

export class InComingAndBTransactionDto {
    incomingEntryTypeId: number;
    BTransactionId: number;
    name: string;
    fromBankAccountId: number;
}

export class ImportBTransactionResult{
    success: string[];
    pending: string[];
    exists: string[];
    errors: string[];
}
