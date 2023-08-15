export class NRevenueByAccount {
    accountId: number;
    accountName: string;
    invoices: NInvoice[];
    totalCollectionDebt: MoneyInfo[];
    totalPaid: MoneyInfo[];
    totalDebt: MoneyInfo[];
    isExpand: boolean = false;
    isShowMoreNote: boolean = false;
}
export class NInvoice {
    incomings: NIncominginvoice[];
    invoiceId?: number;
    invoiceName?: string;
    invoiceNumber?: string;
    collectionDebt: number;
    deadline?: string;
    note?: string;
    status?: number;
    statusName?: string;
    invoiceCurrencyId?: number;
    invoiceCurrencyName?: string;
    month: number;
    year: number;
    ntf?: number;
    itf?: number;
    invoiceCreationTime?: Date;
    invoiceCreatedBy?: string;
    id?: number; //invoiceId
    isEspecial?:boolean;
    invoiceTotal?:number;

}
export class NIncominginvoice {
    incomingEntryId?: number;
    invoiceId?: number;
    incomingName: string;
    money: number;
    currencyId: number;
    currencyName: string;
    transactionInfo: string;
    fromCurrencyName: string;
    toCurrencyName: string;
    exchangeRateDisplay: number;
    bankTransactionId: number;
}
export class MoneyInfo {
    currencyId: number;
    currencyName: string;
    totalMoney: number;
}

export class InvoiceCreateEditDto {
    id: number;
    nameInvoice: string;
    accountId: number;
    month: number;
    year: number;
    collectionDebt: number;
    currencyId: number;
    currencyName?: string;
    deadline: string;
    note: string;
    ntf: number;
    status?: number;
    statusName: string;
    monthAndYear: string;
    accountName?:string
  }

  export class UpdateNoteDto {
    id: number;
    note: string;
  }
  export class UpdateStatusInvoiceDto {
    id: number;
    status: number;
    statusName?: string;
  }
  export class CheckAutoPaidDto {
    accountId: number;
    moneyInfos: MoneyInfo[];
    currencyNeedConverts: CurrencyNeedConvertDto[];
    accountName?: string;
  }
  export class CurrencyNeedConvertDto {
    fromCurrencyId: number;
    fromCurrencyName: string;
    toCurrencyId: number;
    toCurrencyName: string;
    exchangeRate: number;
  }
  export class AutoPaidDto {
    accountId: number;
    currencyNeedConverts: CurrencyNeedConvertDto[];
  }

  export class UpdateIncomeTypeDto{
    id: number;
    incomeTypeId:number
  }
