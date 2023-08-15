using Abp.Dependency;
using FinanceManagement.Entities.NewEntities;
using FinanceManagement.Managers.BTransactions.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceManagement.Managers.BTransactions
{
    public interface IBTransactionManager : ITransientDependency
    {
        Task SetDoneBTransaction(long bTransactionId);
        IQueryable<GetAllBTransactionDto> IQGetAllBTransaction();
        Task<GetAllBTransactionDto> CreateTransaction(CreateBTransactionDto input);
        Task<ResultClientPaidDto> AddClientPaid(long clientId, long btransactionId);
        Task<List<CurrencyNeedConvertDto>> CheckCurrencyBetweenAccountAndBTrasaction(long btransactionId, long accountId);
        Task<bool> PaymentInvoiceByAccount(PaymentInvoiceForAccountDto input);
        Task<bool> ProcessPayment(double money, BTransaction bTransaction, long accountId, List<CurrencyNeedConvertDto> currencyNeedConverts);
        Task<CreateBTransactionDto> UpdateTransaction(CreateBTransactionDto input);
        Task <long> Delete(long id);
        Task CheckCreateOutcomingBankTransaction(long outcomingEntryId, long? currencyIdOfBTrans, double valueOfBTrans);
        Task CreateOutcomingBankTransaction(long outcomingEntryId, long bankTransactionId);
        Task<LinkBTransactionInfomationDto> GetBTransactionInformation(long bTransactionId);
        Task LinkBankTransactionToBTransaction(LinkBankTransactionToBTransactionDto input);
        Task<DifferentBetweenBankTransAndBTransDto> CheckDifferentBetweenBankTransAndBTrans(LinkBankTransactionToBTransactionDto input);
        Task<object> ImportBTransaction(ImportBTransactionDto input);
        Task<int> CountBTransactionPendingStatus();
        Task<bool> HasBTransaction();
        Task<GetInfoRollbackOutcomingEntryWithBTransactionDto> GetInfoRollbankOutcomingEntryWithBTransaction(long bTransactionId);
        Task RollBackOutcomingEntryWithBTransaction(long bTransactionId, int currentPeriodId);
        Task<bool> CheckConversionTransaction(ConversionTransactionDto conversionTransactionDto);
        Task<bool> CheckMuaNgoaiTe(ConversionTransactionDto conversionTransactionDto);
        Task CheckCurrencyBTransactionWithOutCome(long bTransactionId, long outcomingEntryId);
        Task CheckCurrencyBTransactionWithBankAccount(long bTransactionId, long bankAccountId);
        Task<bool> CheckChiChuyenDoi(ChiChuyenDoiDto conversionTransactionDto);
    }
}
