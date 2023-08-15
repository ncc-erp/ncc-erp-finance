using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace FinanceManagement.Entities
{
    public class IncomingEntryType : FullAuditedEntity<long>, IMayHaveTenant
    {
        public int? TenantId { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string PathId { get; set; }
        public string PathName { get; set; }
        public long Level { get; set; }
        public long? ParentId { get; set; }
        public bool RevenueCounted { get; set; }
        public bool IsActive { get; set; }
        /// <summary>
        /// true : khach hang thanh toan invoice
        /// false: khach hang tra kenh tien, khach hang bonus cty/team, all loai thu con lai
        /// Quan ly tai chinh > khoan phai thu: lay ra cac khoan phai thu ma IncomingEntryType.IsClientPaid == true
        /// </summary>
        public bool IsClientPaid { get; set; }
        /// <summary>
        /// khach hang tra truoc
        /// </summary>
        public bool IsClientPrePaid { get; set; }
    }
}
