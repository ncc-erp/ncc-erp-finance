export class BTransactionLog{
    id: number;
    content: string;
    isValid: boolean;
    status: string;
    timeAt: Date;
    errorMessage?: string;
    creationTime: Date;
    key: string;
}