using Abp.Domain.Services;
using FinanceManagement.Enums;
using FinanceManagement.Managers.Dashboards.Dtos;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace FinanceManagement.Managers.Dashboards
{
    public interface IDashboardManager : IDomainService
    {
        /// <summary>
        /// bankAccount - số dư đầu kì - tăng trong kì - giảm trong kì - số dư hiện tại
        /// </summary>
        /// <param name="periodId"></param>
        /// <returns></returns>
        Task<ResultNewComparativeStatisticDto> GetBankAccountStatistics(bool isIncludeBTransPending);
        /// <summary>
        /// đối soát ghi nhận thu trong kì
        /// Tiền - tổng ghi nhận thu - tổng biến động số dư -> chênh lệch
        /// </summary>
        /// <returns></returns>
        Task<List<NewComparativeStatisticsIncomingEntryDto>> GetComparativeStatisticsIncomingEntry();
        /// <summary>
        /// đối soát ghi nhận chi trong kì
        /// tổng tiền chi - tổng tiền trong giao dịch ngân hàng -> chênh lệch
        /// </summary>
        /// <returns></returns>
        Task<NewComparativeStatisticsOutcomingEntryDto> GetComparativeStatisticsOutcomingEntry();
        /// <summary>
        /// lấy những request chi chưa THỰC THI nhưng đã link giao dịch ngân hàng
        /// -> cần bằng với chênh lệch trong đối soát chi
        /// </summary>
        /// <returns></returns>
        Task<ResultComparativeStatisticsOutBankTransactionDto> GetComparativeStatisticsOutBankTransaction();
        Task<ResultComparativeStatisticByCurrencyDto> GetComparativeStatisticByCurrency();
        /// <summary>
        /// lấy thống kê request chi ở các trạng thái: START, PENDINGCEO, APPROVED, REJECTED, EXCUTED
        /// </summary>
        /// <returns></returns>
        Task<List<OverviewOutcomingEntryStatisticDto>> OverviewOutcomingEntryStatistics();
        /// <summary>
        /// lấy thống kê số invoice còn nợ, số tiền cần thu từ các invoice theo các loại tiền
        /// </summary>
        /// <returns></returns>
        Task<OverviewInvoiceStatisticDto> OverviewInvoiceStatistics();
        /// <summary>
        /// lấy thống kê biến động số dư ở các trạng thái: PENDING, DONE
        /// </summary>
        /// <returns></returns>
        Task<List<OverviewBTransactionStatisticDto>> OverviewBTransactionStatistics();
        Dictionary<CurrencyYearMonthDto, double> GetDictionaryCurrencyConvertByYearMonth(DateTime startDate, DateTime endDate);
        void CheckDictionaryCurrencyConvertByYearMonth(Dictionary<CurrencyYearMonthDto, double> dicCurrencyConvert, DateTime startDate, DateTime endDate);
        List<double> GetLineChartIncomingEntry(DateTime startDate, DateTime endDate, HashSet<long> incomingEntryTypeIds, IEnumerable<string> labels, Dictionary<CurrencyYearMonthDto, double> dicCurrencyConvert);
        List<double> GetLineChartOutcomingEntry(DateTime startDate, DateTime endDate, HashSet<long> outcomingEntryTypeIds, IEnumerable<string> labels, Dictionary<CurrencyYearMonthDto, double> dicCurrencyConvert, long statusEndId);
        NewChartDto GetBarChartIncoming(DateTime startDate, DateTime endDate, IEnumerable<string> labels, Dictionary<CurrencyYearMonthDto, double> dicCurrencyConvert);
        NewChartDto GetBarChartOutcomingEntry(DateTime startDate, DateTime endDate, IEnumerable<string> labels, Dictionary<CurrencyYearMonthDto, double> dicCurrencyConvert, long statusEndId);
        Task<List<PieChartDto>> GetPieChartIncoming(long? rootId, DateTime startDate, DateTime endDate);
        Task<List<PieChartDto>> GetPieChartOutcoming(long? rootId, DateTime startDate, DateTime endDate);
        Task<List<BaoCaoChungDto>> GetDataBaoCaoChung(DateTime startDate, DateTime endDate, long branchId, ExpenseType? isExpense);
        Task<IEnumerable<GetThongTinRequestChi>> GetAllRequestChiForBaoCao(DateTime startDate, DateTime endDate, long branchId, ExpenseType? isExpense);
        Task<List<BaoCaoThuDto>> GetDataBaoCaoThu(DateTime startDate, DateTime endDate, bool? isDoanhThu);
        Task<DebtStatisticFromHRMDto> GetHRMDebtStatistic(int? tenantId);

        //Task GetDataBaoCaoChi();
    }
}
