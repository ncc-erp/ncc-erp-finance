export class InvoiceDto {
    name: string;
    timeAt: string;
    accountCode: number;
    totalPrice: number;
    status: number;
    note: string;
    clientName :string;
    project :any[];
    id: number;
    createdBy?:number
}