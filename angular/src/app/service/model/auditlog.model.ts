export class AuditlogDto {
    ExecutionDuration: number;
    ExecutionTime: string;
    MethodName: string;
    Parameters: string;
    ServiceName: string;
    UserId: number;
    Note: string;
    UserIdString: string;
    EmailAddress: string;
    hideNote: boolean;
}
export class EmailAddressInAuditLog {
    UserId: number;
    EmailAddress: string;
}