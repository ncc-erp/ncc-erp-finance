export class PeriodDto{
    id: number;
    name : string;
    startDate: string;
    endDate: string;
    isActive: boolean;
    statusName: string;
    creationTime: string;
    createByUserName: string;
}

export class CreateEditPeriodDto{
    name : string;
    startDate: string;
    periodBankAccounts : PeriodBankAccountForFirstTimeDto[];
}

export class CloseAndCreatePeriodDto{
    name : string;
    periodBankAccounts : PeriodBankAccountForCloseAndCreateDto[];
}
export class PeriodBankAccountForCloseAndCreateDto{
    baseBalance : number;
    bankAccountId: number;
}

export class PeriodBankAccountForFirstTimeDto{
    currentBalance : number;
    bankAccountId: number;
}
export class PreviewPeriodBeforeCloseDto{
    countBTransactionDiffDoneStatus: number;
    countOutcomingEntryDiffEndStatus: number;
    bankAccountInfos: BankAccountInfosDto[]
}
export class BankAccountInfosDto{
    bankAccountId: number;
    bankAccountName: string;
    bankNumber: string;
    currentBalance: number;
    balanceByBTransaction: number;
    diffMoney: number;
    checkMode?: boolean;
}

export class CheckDiffRealBalanceAndBTransactionDto{
    bankAccountId: number;
    bankAccountName: string;
    bankNumber: string;
    currentBalance: number;
    balanceByBTransaction: number;
}
