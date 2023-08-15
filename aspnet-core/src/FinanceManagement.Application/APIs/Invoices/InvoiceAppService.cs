using Abp.Authorization;
using Abp.UI;
using FinanceManagement.APIs.BankTransactions.Dto;
using FinanceManagement.APIs.Invoices.Dto;
using FinanceManagement.APIs.RevenueManageds;
using FinanceManagement.Authorization;
using FinanceManagement.Entities;
using FinanceManagement.Enums;
using FinanceManagement.Extension;
using FinanceManagement.GeneralModels;
using FinanceManagement.IoC;
using FinanceManagement.Managers.BTransactions;
using FinanceManagement.Managers.Invoices;
using FinanceManagement.Managers.Invoices.Dtos;
using FinanceManagement.Managers.Settings;
using FinanceManagement.Paging;
using FinanceManagement.Services.Project;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceManagement.APIs.Invoices
{
    [AbpAuthorize]
    public class InvoiceAppService : FinanceManagementAppServiceBase
    {
        private readonly IInvoiceManager _invoiceManager;
        private readonly IMySettingManager _mySettingManager;
        private readonly IBTransactionManager _bTransactionManager;
        public InvoiceAppService(
            IInvoiceManager invoiceManager,
            IMySettingManager mySettingManager,
            IBTransactionManager bTransactionManager,
            IWorkScope workScope) : base(workScope)
        {
            _invoiceManager = invoiceManager;
            _mySettingManager = mySettingManager;
            _bTransactionManager = bTransactionManager;
        }
        [HttpPost]
        [AbpAuthorize(PermissionNames.Finance_Invoice)]
        public OverviewInvoiceDto GetAllPaging(InvoiceGridParam gridParams)
        {
            using (CurrentUnitOfWork.DisableFilter(nameof(IMustHavePeriod)))
            {
                var query = _invoiceManager
                .IQGetAllInvoice()
                .IgnoreNull()
                .FiltersByAccount(gridParams)
                .FiltersByStatus(gridParams)
                .ApplySearchAndFilter(gridParams)
                .FiltersByStatusDebtAccount(gridParams, WorkScope)
                .FiltersByDateTime(gridParams)
                .AsEnumerable()
                .GroupBy(x => new { x.AccountId, x.AccountName })
                .Select(x => new InvoiceAndIncomByAccountDto
                {
                    AccountId = x.Key.AccountId,
                    AccountName = x.Key.AccountName,
                    Invoices = x.GroupBy(s => new
                    {
                        s.InvoiceCurrencyId,
                        s.InvoiceCurrencyName,
                        s.InvoiceId,
                        s.Status,
                        s.Deadline,
                        s.CollectionDebt,
                        s.InvoiceName,
                        s.InvoiceNumber,
                        s.Note,
                        s.NTF,
                        s.ITF,
                        s.Year,
                        s.Month,
                        s.InvoiceCreationTime,
                        s.InvoiceCreatedBy
                    })
                    .Select(gr => new InvoiceByAccountDto
                    {
                        InvoiceId = gr.Key.InvoiceId,
                        CollectionDebtNumber = gr.Key.CollectionDebt,
                        Deadline = gr.Key.Deadline,
                        InvoiceName = gr.Key.InvoiceName,
                        Note = gr.Key.Note,
                        Status = gr.Key.Status,
                        InvoiceCurrencyId = gr.Key.InvoiceCurrencyId,
                        InvoiceCurrencyName = gr.Key.InvoiceCurrencyName,
                        NTFNumber = gr.Key.NTF,
                        ITFNubmer = gr.Key.ITF,
                        Month = gr.Key.Month,
                        Year = gr.Key.Year,
                        InvoiceCreatedBy = gr.Key.InvoiceCreatedBy,
                        InvoiceCreationTime = gr.Key.InvoiceCreationTime,
                        InvoiceNumber = gr.Key.InvoiceNumber,
                        Incomings = gr.Where(s => s.IncomingEntryId.HasValue)
                                    .OrderByDescending(s => s.CreationTime)
                                    .Select(ic => new IncomingByAccountDto
                                    {
                                        CurrencyId = ic.CurrencyId,
                                        CurrencyName = ic.CurrencyName,
                                        IncomingEntryId = ic.IncomingEntryId,
                                        IncomingName = ic.IncomingName,
                                        MoneyNumber = ic.Money,
                                        TransactionInfo = ic.TransactionInfo,
                                        ExchangeRate = ic.ExchangeRate,
                                        CreatedBy = ic.CreatedBy,
                                        CreationTime = ic.CreationTime,
                                        BTransactionMoney = ic.BTransactionMoney,
                                        BTransactionId = ic.BTransactionId,
                                        BTransactionTimeAt = ic.BTransactionTimeAt,
                                        BTransactionNote = ic.BTransactionNote,
                                        IsShowBTransactionNote = false,
                                        BTransactionAccountName = ic.BTransactionAccountName,
                                        BTransactionAccountNumber = ic.BTransactionAccountNumber,
                                        InvoiceCurrencyName = gr.Key.InvoiceCurrencyName,
                                        BankTransactionId = ic.BankTransactionId
                                    })
                    })
                    .OrderByDescending(s => s.Deadline ?? DateTime.MaxValue),
                });
                return new OverviewInvoiceDto()
                {
                    Pagings = query.GridResultEnumerable(gridParams),
                    Statistics = InvoiceQueryEx.GetStatisticOverview(query)
                };
            }
        }
        [HttpPost]
        [AbpAuthorize(PermissionNames.Finance_Invoice_Create)]
        public async Task<GetInvoiceByIdDto> CreateInvoice(CreateInvoiceDto input)
        {
            return await _invoiceManager.CreateInvoice(input);
        }
        [HttpPut]
        [AbpAuthorize(PermissionNames.Finance_Invoice_Update)]
        public async Task<GetInvoiceByIdDto> UpdateInvoice(UpdateInvoiceDto input)
        {
            return await _invoiceManager.UpdateInvoice(input);
        }
        [HttpPut]
        [AbpAuthorize(PermissionNames.Finance_Invoice_EditNote)]
        public async Task UpdateNote(UpdateNoteInvoiceDto input)
        {
            await _invoiceManager.UpdateNote(input);
        }
        [HttpPut]
        [AbpAuthorize(PermissionNames.Finance_Invoice_EditStatusInvoice)]
        public async Task UpdateStatus(UpdateStatusInvoiceDto input)
        {
            await _invoiceManager.UpdateStatus(input);
        }
        [HttpGet]
        public async Task<CheckAutoPaidDto> CheckAutoPaid(long accountId)
        {
            using (CurrentUnitOfWork.DisableFilter(nameof(IMustHavePeriod)))
            {
                return await _invoiceManager.CheckAutoPaid(accountId);
            }
        }
        [HttpPost]
        [AbpAuthorize(PermissionNames.Finance_Invoice_AutoPay)]
        public async Task<string> AutoPaidForAccount(AutoPaidDto input)
        {
            using (CurrentUnitOfWork.DisableFilter(nameof(IMustHavePeriod)))
            {
                var balanceIncomingType = await _mySettingManager.GetBalanceClientAsync();
                if (string.IsNullOrEmpty(balanceIncomingType.Code))
                    throw new UserFriendlyException("Not Found Balance Incoming Type!");

                var currencyOfTransactions = await WorkScope.GetAll<IncomingEntry>()
                    .Where(s => s.BTransactions.FromAccountId == input.AccountId)
                    .Where(s => s.IncomingEntryTypeId == balanceIncomingType.Id)
                    .Include(s => s.BTransactions)
                    .ThenInclude(x => x.BankAccount)
                    .Include(s => s.BTransactions)
                    .ThenInclude(x => x.FromAccount)
                    .Select(s => new { Money = s.Value, BTransaction = s.BTransactions, s.Id })
                    .ToListAsync();

                input.CurrencyNeedConverts.ForEach(s =>
                {
                    if (s.IsReverseExchangeRate)
                    {
                        s.ExchangeRate = 1 / s.ExchangeRate;
                    }
                });

                foreach (var transaction in currencyOfTransactions)
                {
                    var isDone = await _bTransactionManager.ProcessPayment(transaction.Money, transaction.BTransaction, input.AccountId, input.CurrencyNeedConverts);
                    await WorkScope.DeleteAsync<IncomingEntry>(transaction.Id);
                }
                return "Auto Payment Successfully";
            }
        }
        [HttpGet]
        public async Task<GetInvoiceByIdDto> GetById(long id)
        {
            return await _invoiceManager.GetById(id);
        }
        [HttpDelete]
        [AbpAuthorize(PermissionNames.Finance_Invoice_Delete)]
        public async Task<string> Delete(long id)
        {
            await _invoiceManager.Delete(id);
            return "Deleted Successfully";
        }
        [HttpGet]
        public async Task<CheckSetDoneInvoiceDto> CheckSetDoneInvoice(long invoiceId)
        {
            return await _invoiceManager.CheckSetDoneInvoice(invoiceId);
        }
        [HttpPost]
        public async Task SetDoneInvoice(long invoiceId)
        {
            using (CurrentUnitOfWork.DisableFilter(nameof(IMustHavePeriod)))
            {
                await _invoiceManager.SetDoneInvoice(invoiceId);
            }
        }
        [HttpPost]
        [AbpAuthorize(PermissionNames.Finance_Invoice_KhachHangTraKenhTien)]
        public async Task SetClientPayDeviant(ClientPayDeviantDto input)
        {
            using (CurrentUnitOfWork.DisableFilter(nameof(IMustHavePeriod)))
            {
                await _invoiceManager.SetClientPayDeviant(input);
            }
        }
        [HttpGet]
        [AbpAuthorize(PermissionNames.Finance_Invoice_Export_Report)]
        public async Task<byte[]> ExportReport()
        {
            var accountIds = await WorkScope.GetAll<Account>().Select(s => s.Id).ToListAsync();
            return await _invoiceManager.ExportReport(GetAllPaging(new InvoiceGridParam
            {
                AccountIds = accountIds,
                IsDoneDebt = false,
                MaxResultCount = int.MaxValue,
                FilterDateTimeParam = default
            }));
        }

    }
}
