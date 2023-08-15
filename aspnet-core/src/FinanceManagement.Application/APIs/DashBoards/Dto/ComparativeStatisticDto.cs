using System;
using System.Collections.Generic;
using System.Text;

namespace FinanceManagement.APIs.DashBoards.Dto
{
    public class ComparativeStatisticDto
    {
        public long ComparativeStatisticId { get; set; }
        public double TotalIncoming { get; set; }
        public double TotalOutcoming { get; set; }
        public List<BalanceBankAccountDto> StartBalance { get; set; }
        //public List<BalanceBankAccountDto> RealEndBalance { get; set; }
        //public List<BalanceBankAccountDto> ConvertStartBalance { get; set; }
        //public List<BalanceBankAccountDto> ConvertEndBalance { get; set; }
        public List<BalanceBankAccountDto> RealOutcomingByTransactions { get; set; }
        public List<BalanceBankAccountDto> ConvertOutcomingByTransactions { get; set; }

        public double TotalVNDIn { get; set; }
        public double TotalVNDOut { get; set; }
        public double TotalUSDIn { get; set; }
        public double TotalUSDOut { get; set; }
        public double TotalVNDInTransaction { get; set; }
        public double TotalVNDOutTransaction { get; set; }
        public double TotalUSDInTransaction { get; set; }
        public double TotalUSDOutTransaction { get; set; }
        public double StartBalanceVND {get;set;}
        public double StartBalanceUSD { get; set; }
        public double ExchangeUSD { get; set; }
        public double ExchangeVND { get; set; }
        public double CurrencyRate { get; set; }
        public double Total
        {
            get
            {
                return StartBalanceVND + (StartBalanceUSD + TotalUSDIn - TotalUSDOut - ExchangeUSD) * CurrencyRate + TotalVNDIn - TotalVNDOut + ExchangeVND;
            }
        }
        public double TotalTransaction
        {
            get
            {
                return StartBalanceVND + (StartBalanceUSD + TotalUSDInTransaction - TotalUSDOutTransaction - ExchangeUSD) * CurrencyRate + TotalVNDInTransaction - TotalVNDOutTransaction + ExchangeVND;
            }
        }
    }
}
