using Abp.Domain.Entities;
using FinanceManagement.Enums;
using FinanceManagement.GeneralModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace FinanceManagement.Entities.NewEntities
{
    public class CircleChartDetail : FKFullAuditedEntity, IMayHaveTenant
    {
        public int? TenantId { get; set; }
        public long CircleChartId { get; set; }
        [MaxLength(500)]
        public string Name { get; set; }
        public string Color { get; set; }
        /// <summary>
        /// null: không filter theo chi nhánh
        /// khác null: filter theo branchId
        /// </summary>
        public long? BranchId { get; set; }

        /// <summary>
        /// null: theo loại chi
        /// NON_EXPENSE: chi không thực, không tính vào chi phí
        /// REAL_EXPENSE: chi thực, tính vào chi phí
        /// </summary>
        public RevenueExpenseType? RevenueExpenseType { get; set; }
        public string ClientIds { get; set; }
        /// <summary>
        /// lưu dạng Json String, ví dụ: [1, 3, 100]
        /// </summary>
        public string InOutcomeTypeIds { get; set; }

        #region Foreign Key
        [ForeignKey(nameof(CircleChartId))]
        public virtual CircleChart CircleChart { get; set; }
        [ForeignKey(nameof(BranchId))]
        public virtual Branch Branch { get; set; }
        #endregion
    }
}

