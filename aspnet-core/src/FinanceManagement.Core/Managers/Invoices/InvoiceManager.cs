using Abp.UI;
using FinanceManagement.Authorization.Users;
using FinanceManagement.Entities;
using FinanceManagement.Entities.NewEntities;
using FinanceManagement.Enums;
using FinanceManagement.Helper;
using FinanceManagement.IoC;
using FinanceManagement.Managers.BTransactions.Dtos;
using FinanceManagement.Managers.Invoices.Dtos;
using FinanceManagement.Managers.Settings;
using FinanceManagement.Uitls;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace FinanceManagement.Managers.Invoices
{
    public class InvoiceManager : DomainManager, IInvoiceManager
    {
        private readonly IMySettingManager _mySettingManager;
        private readonly IWebHostEnvironment _env;
        public InvoiceManager(IWorkScope ws, 
            IMySettingManager mySettingManager,
            IWebHostEnvironment webHostEnvironment
            ) : base(ws)
        {
            _mySettingManager = mySettingManager;
            _env = webHostEnvironment;
        }
        public IQueryable<AllPropInvoiceAndIncomByAccountDto> IQGetAllInvoice()
        {
            var qallUsers = _ws.GetAll<User>()
                .Select(s => new { s.Name, s.Id });

            var qaccounts = from acc in _ws.GetAll<Account>()
                            select new { AccountName = acc.Name, AccountId = acc.Id };

            var qInvoiceByAccount = from inv in _ws.GetAll<Invoice>()
                                    join acc in qaccounts on inv.AccountId equals acc.AccountId
                                    select new
                                    {
                                        AccountId = acc.AccountId,
                                        AccountName = acc.AccountName,
                                        InvoiceId = inv.Id,
                                        CollectionDebt = inv.CollectionDebt,
                                        Deadline = inv.Deadline,
                                        Name = inv.NameInvoice,
                                        InvoiceNumber = inv.InvoiceNumber,
                                        Note = inv.Note,
                                        Status = inv.Status,
                                        RevenueCurrencyId = inv.CurrencyId,
                                        RevenueCurrencyName = inv.Currency.Name,
                                        NTF = inv.NTF,
                                        ITF = inv.ITF,
                                        Month = inv.Month,
                                        Year = inv.Year,
                                        RevenueCreationTime = inv.CreationTime,
                                        RevenueCreatedBy = qallUsers.Where(s => s.Id == inv.CreatorUserId).Select(s => s.Name).FirstOrDefault()
                                    };
            var qIncomingByAccount = from transaction in _ws.GetAll<BTransaction>()
                                     join acc in qaccounts on transaction.FromAccountId equals acc.AccountId
                                     join incom in _ws.GetAll<IncomingEntry>()
                                                        .Where(x => x.IncomingEntryType.IsClientPaid || x.IncomingEntryType.IsClientPrePaid)
                                                on transaction.Id equals incom.BTransactionId
                                     select new
                                     {
                                         AccountId = acc.AccountId,
                                         AccountName = acc.AccountName,
                                         CurrencyId = transaction.BankAccount.CurrencyId.Value,
                                         CurrencyName = transaction.BankAccount.Currency.Name,
                                         IncomingEntryId = incom.Id,
                                         InvoiceId = incom.InvoiceId,
                                         IncomingName = incom.Name,
                                         ExchangeRate = incom.ExchangeRate,
                                         Money = incom.Value,
                                         TransactionInfo = string.Empty,
                                         BTransactionMoney = transaction.Money,
                                         BTransactionId = transaction.Id,
                                         BTransactionTimeAt = transaction.TimeAt,
                                         BTransactionNote = transaction.Note,
                                         BankTransactionId = incom.BankTransactionId,
                                         CreationTime = incom.CreationTime,
                                         CreatedBy = qallUsers.Where(s => s.Id == incom.CreatorUserId).Select(s => s.Name).FirstOrDefault(),
                                         BTransactionAccountName = transaction.BankAccount.HolderName,
                                         BTransactionAccountNumber = transaction.BankAccount.BankNumber,
                                     };
            var qIncomingLeftJoinInvoiceByAccount = from incom in qIncomingByAccount
                                                    join invoice in qInvoiceByAccount on incom.InvoiceId equals invoice.InvoiceId into invIn
                                                    from invInDIF in invIn.DefaultIfEmpty()
                                                    select new AllPropInvoiceAndIncomByAccountDto
                                                    {
                                                        AccountId = incom.AccountId,
                                                        AccountName = incom.AccountName,
                                                        CurrencyId = incom.CurrencyId,
                                                        CurrencyName = incom.CurrencyName,
                                                        IncomingEntryId = incom.IncomingEntryId,
                                                        InvoiceId = incom.InvoiceId,
                                                        IncomingName = incom.IncomingName,
                                                        Money = incom.Money,
                                                        ExchangeRate = incom.ExchangeRate ?? 1,
                                                        TransactionInfo = incom.TransactionInfo,
                                                        BTransactionMoney = incom.BTransactionMoney,
                                                        BTransactionId = incom.BTransactionId,
                                                        BTransactionTimeAt = incom.BTransactionTimeAt,
                                                        BTransactionNote = incom.BTransactionNote,
                                                        BankTransactionId = incom.BankTransactionId,
                                                        CreationTime = incom.CreationTime,
                                                        CreatedBy = incom.CreatedBy,
                                                        CollectionDebt = invInDIF.CollectionDebt,
                                                        Deadline = invInDIF.Deadline,
                                                        InvoiceName = invInDIF.Name,
                                                        InvoiceNumber = invInDIF.InvoiceNumber,
                                                        Note = invInDIF.Note,
                                                        Status = invInDIF.Status,
                                                        InvoiceCurrencyId = invInDIF.RevenueCurrencyId,
                                                        InvoiceCurrencyName = invInDIF.RevenueCurrencyName,
                                                        NTF = invInDIF.NTF,
                                                        ITF = invInDIF.ITF,
                                                        Month = invInDIF.Month,
                                                        Year = invInDIF.Year,
                                                        InvoiceCreationTime = invInDIF.RevenueCreationTime,
                                                        InvoiceCreatedBy = invInDIF.RevenueCreatedBy,
                                                        BTransactionAccountName = incom.BTransactionAccountName,
                                                        BTransactionAccountNumber = incom.BTransactionAccountNumber,
                                                    };
            var qInvoiceLeftJoinIncomingByAccount = from inv in qInvoiceByAccount
                                                    join incom in qIncomingByAccount on inv.InvoiceId equals incom.InvoiceId into invIn
                                                    from invInDIF in invIn.DefaultIfEmpty()
                                                    select new AllPropInvoiceAndIncomByAccountDto
                                                    {
                                                        AccountId = inv.AccountId,
                                                        AccountName = inv.AccountName,
                                                        InvoiceId = inv.InvoiceId,
                                                        CollectionDebt = inv.CollectionDebt,
                                                        Deadline = inv.Deadline,
                                                        InvoiceName = inv.Name,
                                                        Note = inv.Note,
                                                        InvoiceNumber = inv.InvoiceNumber,
                                                        Status = inv.Status,
                                                        InvoiceCurrencyId = inv.RevenueCurrencyId,
                                                        InvoiceCurrencyName = inv.RevenueCurrencyName,
                                                        NTF = inv.NTF,
                                                        ITF = inv.ITF,
                                                        Month = inv.Month,
                                                        Year = inv.Year,
                                                        InvoiceCreationTime = inv.RevenueCreationTime,
                                                        InvoiceCreatedBy = inv.RevenueCreatedBy,
                                                        Money = invInDIF.Money,
                                                        ExchangeRate = invInDIF.ExchangeRate ?? 1,
                                                        TransactionInfo = invInDIF.TransactionInfo,
                                                        CurrencyId = invInDIF.CurrencyId,
                                                        CurrencyName = invInDIF.CurrencyName,
                                                        IncomingEntryId = invInDIF.IncomingEntryId,
                                                        IncomingName = invInDIF.IncomingName,
                                                        BTransactionMoney = invInDIF.BTransactionMoney,
                                                        BTransactionId = invInDIF.BTransactionId,
                                                        BTransactionTimeAt = invInDIF.BTransactionTimeAt,
                                                        BTransactionNote = invInDIF.BTransactionNote,
                                                        CreationTime = invInDIF.CreationTime,
                                                        CreatedBy = invInDIF.CreatedBy,
                                                        BTransactionAccountName = invInDIF.BTransactionAccountName,
                                                        BTransactionAccountNumber = invInDIF.BTransactionAccountNumber,
                                                        BankTransactionId = invInDIF.BankTransactionId,
                                                    };
            var query = qIncomingLeftJoinInvoiceByAccount.Union(qInvoiceLeftJoinIncomingByAccount);
            return query;
        }
        public async Task<GetInvoiceByIdDto> GetById(long id)
        {
            var invoice = await _ws.GetAsync<Invoice>(id);
            var mInvoice = ObjectMapper.Map<GetInvoiceByIdDto>(invoice);
            return mInvoice;
        }
        public async Task<GetInvoiceByIdDto> CreateInvoice(CreateInvoiceDto input)
        {
            var revenue = ObjectMapper.Map<Invoice>(input);
            await _ws.InsertAsync(revenue);
            await CurrentUnitOfWork.SaveChangesAsync();
            return await GetById(revenue.Id);
        }
        public async Task<GetInvoiceByIdDto> UpdateInvoice(UpdateInvoiceDto input)
        {
            var revenue = await _ws.GetAsync<Invoice>(input.Id);
            ObjectMapper.Map(input, revenue);
            await _ws.UpdateAsync(revenue);
            await CurrentUnitOfWork.SaveChangesAsync();
            return await GetById(revenue.Id);
        }
        public async Task UpdateNote(UpdateNoteInvoiceDto input)
        {
            var revenue = await _ws.GetAsync<Invoice>(input.Id);
            revenue.Note = input.Note;
            await _ws.UpdateAsync(revenue);
        }
        public async Task UpdateStatus(UpdateStatusInvoiceDto input)
        {
            var revenue = await _ws.GetAsync<Invoice>(input.Id);
            revenue.Status = input.Status;
            await _ws.UpdateAsync(revenue);
        }
        public async Task<CheckAutoPaidDto> CheckAutoPaid(long accountId)
        {
            var balanceClient = await _mySettingManager.GetBalanceClientAsync();
            var currencyOfTransactions = await _ws.GetAll<IncomingEntry>()
                .Where(s => s.IncomingEntryType.IsClientPrePaid)
                .Where(s => s.BTransactions.FromAccountId == accountId)
                .Select(s => new { s.BTransactions.BankAccount.CurrencyId, s.BTransactions.BankAccount.Currency.Name, s.Value })
                .ToListAsync();
            var currencyNeedConverts = await _ws.GetAll<Invoice>()
                .Where(s => s.AccountId == accountId)
                .Where(s => s.Status != NInvoiceStatus.HOAN_THANH && s.Status != NInvoiceStatus.KHONG_TRA)
                .Select(s => new { s.CurrencyId, s.Currency.Name })
                .Distinct()
                .ToListAsync();
            List<CurrencyNeedConvertDto> currencyConverts = new List<CurrencyNeedConvertDto>();
            List<MoneyInfoDto> moneyInfos = new List<MoneyInfoDto>();
            foreach (var currencyOfTransaction in currencyOfTransactions)
            {
                foreach (var currencyNeedConvert in currencyNeedConverts)
                {
                    if (currencyOfTransaction.CurrencyId.Value == currencyNeedConvert.CurrencyId) continue;

                    if (currencyConverts.Where(s => s.FromCurrencyId == currencyOfTransaction.CurrencyId.Value)
                        .Where(s => s.ToCurrencyId == currencyNeedConvert.CurrencyId).Any())
                        continue;

                    currencyConverts.Add(new CurrencyNeedConvertDto
                    {
                        FromCurrencyId = currencyOfTransaction.CurrencyId.Value,
                        FromCurrencyName = currencyOfTransaction.Name,
                        ToCurrencyId = currencyNeedConvert.CurrencyId,
                        ToCurrencyName = currencyNeedConvert.Name,
                    });
                }
                moneyInfos.Add(new MoneyInfoDto { CurrencyId = currencyOfTransaction.CurrencyId.Value, CurrencyName = currencyOfTransaction.Name, TotalMoneyNumber = currencyOfTransaction.Value });
            }

            return new CheckAutoPaidDto
            {
                CurrencyNeedConverts = currencyConverts,
                MoneyInfos = moneyInfos,
                HasCollectionDebt = currencyNeedConverts.Any() && currencyOfTransactions.Any()
            };
        }
        public async Task Delete(long id)
        {
            var revenue = await _ws.GetAsync<Invoice>(id);
            if (revenue.Status != NInvoiceStatus.CHUA_TRA)
                throw new UserFriendlyException("Không thể xóa invoice vì có trạng thái khác Chưa Trả!");
            await _ws.DeleteAsync(revenue);
        }
        public async Task<CheckSetDoneInvoiceDto> CheckSetDoneInvoice(long invoiceId)
        {
            var invoice = await _ws.GetAll<Invoice>()
                .Where(s => s.Id == invoiceId)
                .Select(s => new CheckSetDoneInvoiceDto(s.IncomingEntries.Where(x => !x.IsDeleted).Sum(x => x.Value * x.ExchangeRate), s.CollectionDebt)
                {
                    MaxITF = s.Currency.MaxITF,
                })
                .FirstOrDefaultAsync();
            return invoice;
        }
        public async Task SetDoneInvoice(long invoiceId)
        {
            var invoice = await CheckSetDoneInvoice(invoiceId);
            if (invoice.TotalDebt > invoice.MaxITF)
                throw new UserFriendlyException($"Không thể hoàn thành invoice vì phần còn nợ > giới hạn phí chuyển khoản nội địa (MaxITF)! {invoice.TotalDebt} > {invoice.MaxITF}");
            var oldInvoice = await _ws.GetAsync<Invoice>(invoiceId);
            oldInvoice.ITF = invoice.TotalDebt;
            oldInvoice.CollectionDebt = oldInvoice.CollectionDebt - oldInvoice.ITF;
            oldInvoice.Status = NInvoiceStatus.HOAN_THANH;
            await _ws.UpdateAsync(oldInvoice);
        }

        public async Task SetClientPayDeviant(ClientPayDeviantDto input)
        {
            var deviantEntryTypeInfo = await _mySettingManager.GetDeviantClientAsync();
            var incomingEntry = await _ws.GetAsync<IncomingEntry>(input.IncomingEntryId);
            incomingEntry.IncomingEntryTypeId = deviantEntryTypeInfo.Id;
            incomingEntry.Name = input.IncomingName;
            await CurrentUnitOfWork.SaveChangesAsync();
        }
        public async Task<byte[]> ExportReport(OverviewInvoiceDto overviewInvoiceDto)
        {
            //File template
            var file = Helpers.GetInfoFileTemplate(new string[] { _env.WebRootPath, "Invoice_Template.xlsx" });
            // Thời điểm export
            var now = DateTimeUtils.GetNow();
            //Tiền tệ chính
            var currencyDefault = await GetCurrencyDefaultAsync();
            
            if(currencyDefault == default) throw new UserFriendlyException($"Chưa có tiền tệ chính");

            //Các loại tiền tệ có khoản nợ
            var currencyIds = overviewInvoiceDto.Statistics.Select(s => s.CurrencyId).ToList();

            //dict key: mã tiền tệ; value: thông tin về tiền {tên, tỉ giá}
            var dCurrencyConvert = _ws.GetAll<CurrencyConvert>()
                .Where(s => currencyIds.Contains(s.CurrencyId) || s.Currency.IsCurrencyDefault)
                .OrderByDescending(s => s.Currency.IsCurrencyDefault)
                .Select(s => new
                {
                    s.CurrencyId,
                    s.Currency.Name,
                    s.Value,
                    s.DateAt
                })
                .AsEnumerable()
                .GroupBy(s => s.CurrencyId)
                .Select(s => new
                {
                    s.Key,
                    Value = s.OrderByDescending(s => s.DateAt).FirstOrDefault()
                })
                .Where(s => s.Value.Value > 0)
                .ToDictionary(s => s.Key, s => s.Value);

            var currencyCount = dCurrencyConvert.Count();

            if (currencyIds.Count() > currencyCount)
            {
                //Tiền tệ chưa có tỉ giá
                var currencyNotCurrencyConvert = currencyIds.Where(s => !dCurrencyConvert.ContainsKey(s));
                //thông tin của các loại tiên chưa có tỉ giá
                var nameCurrency = await _ws.GetAll<Currency>()
                    .Where(s => currencyNotCurrencyConvert.Contains(s.Id))
                    .Select(s => s.Name)
                    .ToListAsync();
                throw new UserFriendlyException($"Các tiền tệ chưa có tỉ giá: {string.Join(", ", nameCurrency)}");
            }


            using (ExcelPackage pck = new ExcelPackage(file))
            {
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                ExcelWorksheet worksheet = pck.Workbook.Worksheets[0];

                //Lấy ra các ô mốc
                var mockCells = worksheet.GetCellHaveContents(new string[] {
                    "{{Thời điểm báo cáo}}",
                    "{{Mốc tiền tệ}}",
                    "{{Mốc tiền tệ 2}}",
                    "{{Default currency}}",
                    "{{Mốc khách hàng}}",
                    "{{Gộp hàng}}" 
                });

                var cellReportTime = mockCells.GetCellHaveContent("{{Thời điểm báo cáo}}");
                var cellDefaultCurrency = mockCells.GetCellHaveContent("{{Default currency}}");
                var cellMockCurrency = mockCells.GetCellHaveContent( "{{Mốc tiền tệ}}" );
                var cellMockCurrency2 = mockCells.GetCellHaveContent( "{{Mốc tiền tệ 2}}" );
                var cellMockClients = mockCells.GetCellHaveContent( "{{Mốc khách hàng}}" );
                var cellBeginMergeCell = mockCells.GetCellHaveContent( "{{Gộp hàng}}" );

                //Chèn ngày báo cáo
                cellReportTime.Value = DateTimeUtils.GetNow().ToString("dd/MM/yyyy HH:mm");
                //Chèn loại tiền mặc định
                cellDefaultCurrency.Value = currencyDefault.Name;
                //Chèn nội dung Header CÔNG NỢ
                cellBeginMergeCell.Value = "CÔNG NỢ";

                //Chèn StatisticInvoiceDto
                var currentRow = cellMockCurrency.Start.Row;
                var currentColumn = cellMockCurrency.Start.Column;

                var currentRow2 = cellMockCurrency2.Start.Row;
                var currentColumn2 = cellMockCurrency2.Start.Column;

                // dict tiền tệ và tổng nợ của nó
                var dStatistics = overviewInvoiceDto.Statistics.ToDictionary(s => s.CurrencyId, s => s);

                //dict tiền và vị trí cột
                var dIndexRowCurrency = new Dictionary<long, int>();


                foreach (var currency in dCurrencyConvert)
                {
                    //Mốc 1
                    worksheet.Cells[currentRow, currentColumn].Value = currency.Value.Name;
                    var totalValue = dStatistics.ContainsKey(currency.Key) ? dStatistics[currency.Key].DebtNumber : 0;
                    worksheet.Cells[currentRow, currentColumn + 1].Value = totalValue;
                    worksheet.Cells[currentRow, currentColumn + 2].Value = currency.Value.Value;
                    worksheet.Cells[currentRow, currentColumn + 3].Value = currency.Value.Value * totalValue;

                    //Mốc 2
                    worksheet.Cells[currentRow2, currentColumn2].Value = currency.Value.Name;

                    //Update vị trí cột tiền tệ
                    dIndexRowCurrency.Add(currency.Key, currentColumn2);

                    worksheet.InsertRow(currentRow + 1, 1, currentRow);
                    currentRow++;
                    currentRow2++;
                    currentColumn2++;
                }

                //MergedCells
                var range = worksheet.Cells[
                    cellBeginMergeCell.Start.Row + currencyCount, 
                    cellBeginMergeCell.Start.Column, 
                    cellBeginMergeCell.Start.Row + currencyCount, 
                    cellBeginMergeCell.Start.Column + currencyCount - 1
                    ];
                range.Merge = true;

                
                var clientCount = 1;

                //update vị trí hiện tại
                currentRow = cellMockClients.Start.Row + currencyCount;
                currentColumn = cellMockClients.Start.Column;

                //Chèn thông tin khách hàng nợ
                foreach (var client in overviewInvoiceDto.Pagings.Items)
                {
                    worksheet.Cells[currentRow, currentColumn].Value = clientCount;
                    worksheet.Cells[currentRow, currentColumn + 1].Value = client.AccountName;

                    var debts = client.TotalDebt.ToList();

                    foreach (var debt in debts)
                    {
                        var cell = worksheet.Cells[currentRow, dIndexRowCurrency[debt.CurrencyId]];
                        if(Math.Abs(debt.TotalMoneyNumber) >= 1)
                        {
                            cell.Value = debt.TotalMoneyNumber;
                        }
                        else if(debt.TotalMoneyNumber != 0)
                        {
                            worksheet.Comments.Add(cell, " ", Helpers.FormatMoney(debt.TotalMoneyNumber));
                        }

                    }

                    clientCount++;
                    currentRow++;
                }

                using (var stream = new MemoryStream())
                {
                    pck.SaveAs(stream);
                    var content = stream.ToArray();
                    return content;
                }
            }
        }
        

    }
}
