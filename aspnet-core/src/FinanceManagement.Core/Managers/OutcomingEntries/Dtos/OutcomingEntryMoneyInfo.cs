using System;
using System.Collections.Generic;
using System.Text;

namespace FinanceManagement.Managers.OutcomingEntries.Dtos
{
    public class OutcomingEntryMoneyInfo
    {
        /// <summary>
        /// Số tiền ở request chi
        /// </summary>
        public double NeedToSpend { get; set; }

        /// <summary>
        /// Số tiền đã giải ngân (giá trị âm) = tổng tiền nhận (giá trị âm) + tổng tiền nhận (chi chuyển đổi, giá trị âm) + ghi nhận thu
        /// 
        /// </summary>
        public double Spent { get; set; }

        /// <summary>
        /// Số tiền giải nhân còn thiếu của request chi
        /// </summary>
        public double Avalible => Spent + NeedToSpend;
    }
}
