using System;
using System.Collections.Generic;
using System.Text;

namespace FinanceManagement.Managers.OutcomingEntries.Dtos
{
    public class OutcomingEntryMoneyInfomation
    {
        /// <summary>
        /// Tiền của request chi
        /// </summary>
        public double Value { get; set; }
        /// <summary>
        /// Tổng tiền nhận của giao dịch ngân hàng trong request chi lấy tovalue với các logic cũ
        /// </summary>
        public double TotalPaidToValue { get; set; }
        /// <summary>
        /// dùng trong trường hợp Request chi có thể thay đổi được loại currency
        /// </summary>
        public double TotalPaidFromValue { get; set; }
        /// <summary>
        /// Tổng tiền ghi nhận thu là hoàn tiền (RelationInOut.IsRefund == true) trong request chi 
        /// </summary>
        public double TotalIncomingRefund { get; set; }
    }
}
