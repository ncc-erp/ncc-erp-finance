using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace FinanceManagement.Managers.BTransactions.Dtos
{
    public class ConversionTransactionDto
    {
        [Required(ErrorMessage = "Phải có biến động số dư âm")]
        public List<long> MinusBTransactionIds { get; set; }
        [Required(ErrorMessage = "Phải có biến động số dư dương")]
        public List<long> PlusBTransactionIds { get; set; }
        [Required(ErrorMessage = "Phải có yêu cầu chi")]
        [Range(1, long.MaxValue, ErrorMessage = "Phải có yêu cầu chi")]
        public long? OutcomingEntryId { get; set; }
        [Required(ErrorMessage = "Phải có tài khoản ngân hàng gửi")]
        [Range(1, long.MaxValue, ErrorMessage = "Phải có tài khoản ngân hàng gửi")]
        public long? FromBankAccountId { get; set; }
        [Required(ErrorMessage = "Phải có tài khoản ngân hàng nhận")]
        [Range(1, long.MaxValue, ErrorMessage = "Phải có tài khoản ngân hàng nhận")]
        public long? ToBankAccountId { get; set; }
        [Required(ErrorMessage = "Phải có loại thu")]
        [Range(1, long.MaxValue, ErrorMessage = "Phải có loại thu")]
        public long? IncomingEntryTypeId { get; set; }
        public List<long> BTransactionIds => MinusBTransactionIds == null || PlusBTransactionIds == null ? new List<long>() : MinusBTransactionIds.Concat(PlusBTransactionIds).ToList();
    }
}
