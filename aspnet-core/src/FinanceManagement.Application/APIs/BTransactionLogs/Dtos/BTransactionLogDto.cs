using FinanceManagement.Anotations;
using FinanceManagement.GeneralModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace FinanceManagement.APIs.BTransactionLogs.Dtos
{
    public class BTransactionLogDto : ICustomCreationAudited
    {
        public long Id { get; set; }
        [ApplySearch]
        public string Content { get; set; }
        public bool IsValid { get; set; }
        public string Status => IsValid ? "Thành công" : "Lỗi";
        [ApplySearch]
        public string ErrorMessage { get; set; }
        public DateTime TimeAt { get; set; }
        public DateTime CreationTime { get; set; }
        public long? CreationUserId { get; set; }
        public string CreationUser { get; set; }
        [ApplySearch]
        public string Key { get; set; }
    }
}
