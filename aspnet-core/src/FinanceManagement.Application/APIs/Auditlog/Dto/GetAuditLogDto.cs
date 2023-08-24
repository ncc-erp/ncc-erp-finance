using FinanceManagement.Anotations;
using FinanceManagement.Paging;
using System;
using System.Collections.Generic;
using System.Text;
using FinanceManagement.Uitls;

namespace FinanceManagement.APIs.Auditlog.Dto
{
    public class GetAuditLogDto
    {
        public int ExecutionDuration { get; set; }
        public DateTime ExecutionTime { get; set; }
        public string MethodName { get; set; }
        [ApplySearch]
        public string Parameters { get; set; }
        public string ServiceName { get; set; }
        public long? UserId { get; set; }
        public string UserIdString { get; set; }
        public string EmailAddress { get; set; }
        public string Note => AuditLogUtils.GetNote(ServiceName, MethodName);

    }
    public class GetAllEmailAddressInAuditLogDto
    {
        public long UserId { get; set; }
        public string EmailAddress { get; set; }
    }
}
