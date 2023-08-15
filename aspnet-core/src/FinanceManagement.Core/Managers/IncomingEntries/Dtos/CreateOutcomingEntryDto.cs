using Abp.AutoMapper;
using FinanceManagement.Entities;
using FinanceManagement.Extension;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace FinanceManagement.Managers.IncomingEntries.Dtos
{
    [AutoMapTo(typeof(IncomingEntry))]
    public class CreateIncomingEntryDto
    {
        public long IncomingEntryTypeId { get; set; }
        public long? BankTransactionId { get; set; }
        public string Name { get; set; }
        public double Value { get; set; }
        public long? CurrencyId { get; set; }
        #region new version
        public double? ExchangeRate { get; set; } = FinanceManagementConsts.DEFAULT_EXCHANGE_RATE;
        public long? BTransactionId { get; set; }
        #endregion
    }
    public class LinkIncomingEntryDto
    {
        public string Name { get; set; }
        public long IncomingEntryTypeId { get; set; }
        public long BTransactionId { get; set; }
        public long FromBankAccountId { get; set; }
        public bool BankTransactionNameEqualOutcomingName { get; set; } = false;
    }
    public class LinkMultiIncomingEntryDto
    {
        [Required(ErrorMessage = "Phải có ít nhât 1 ghi nhận thu được tạo")]
        public List<CreateIncomingEntryDto> IncomingEntries { get; set; }
        [Required(ErrorMessage = "Phải có tài khoản ngân hàng gửi")]
        [Range(1, long.MaxValue, ErrorMessage = "Phải có tài khoản ngân hàng gửi")]
        public long? FromBankAccountId { get; set; }
        [Required(ErrorMessage = "Phải có biến động số dư")]
        [Range(1, long.MaxValue, ErrorMessage = "Phải có biến động số dư")]
        public long? BTransactionId { get; set; }
        public double IncomingTotalMoney => (IncomingEntries != null && !IncomingEntries.IsEmpty()) ? IncomingEntries.Sum(s => s.Value) : 0;
    }
}
