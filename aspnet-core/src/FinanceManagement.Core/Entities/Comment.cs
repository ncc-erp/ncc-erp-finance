using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using FinanceManagement.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace FinanceManagement.Entities
{
    public class Comment : FullAuditedEntity<long>, IMayHaveTenant
    {
        public int? TenantId { get; set; }
        public CommentType CommentType { get; set; }
        public long PostId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
    }
}
