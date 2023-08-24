using FinanceManagement.Helper;
using FinanceManagement.Paging;
using System;
using System.Collections.Generic;
using System.Text;

namespace FinanceManagement.APIs.OutcomingEntryDetails.Dto
{
    public class GridResultOutcomingEntryDetail<T> where T : class
    {
        public int TotalCount { get; set; }
        public List<T> Items { get; set; }
        public double TotalMoney { get; set; }
        public string TotalMoneyString => Helpers.FormatMoney(TotalMoney);
        public GridResultOutcomingEntryDetail(List<T> items, int totalCount, double totalMoney)
        {
            TotalCount = totalCount;
            Items = items;
            TotalMoney = totalMoney;
        }
    }
}
