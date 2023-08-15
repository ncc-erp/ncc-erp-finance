using System.Collections.Generic;
using System.Text;
using FinanceManagement.Anotations;
using FinanceManagement.Paging;
using FinanceManagement.Uitls;

namespace FinanceManagement.APIs.Auditlog.Dto
{
    public class AuditLogGridParamDTO : GridParam
    {
        public string MethodName { get; set; }
        public string ServiceName { get; set; }
    }
}
