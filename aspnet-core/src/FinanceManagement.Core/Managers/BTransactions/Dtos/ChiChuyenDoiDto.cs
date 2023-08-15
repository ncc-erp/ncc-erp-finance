using System;
using System.Collections.Generic;
using System.Text;

namespace FinanceManagement.Managers.BTransactions.Dtos
{
    public class ChiChuyenDoiDto: ConversionTransactionDto
    {
        public string InComingEntryName { get; set; } = "Chi chuyển đổi";
    }
}
