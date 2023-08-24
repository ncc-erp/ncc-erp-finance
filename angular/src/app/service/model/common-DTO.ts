export class FilterRequest {
    includes: string = '';
    filters: string = '';
    sorts: string = '';
    page: any = 1;
    pageSize: any = 10;
}

export class ValueAndNameModel {
    value?: number | string | boolean;
    name?: string;
}
export class AccountForDropdownDto extends ValueAndNameModel{
    accountType:number
}
export class BankAccountTransDto{
    bankAccountId : number
    bankAccountName : string
    accountId : number
    accountName : string
    accountTypeId : number
    accountTypeName : string
    name: string
    value: number
}
export class IncomingEntryTypeDto{
    id: number;
    name: string
    code: string
    pathId: string
    pathName: string
    level: number
    parentId: number
    revenueCounted: boolean
    children: IncomingEntryTypeDto[]
}
export class BankAccoutForCompanyDto{
    value: number;
    name: string;
    currentBalance?: number;
}
