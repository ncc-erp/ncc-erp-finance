using FinanceManagement.Entities;
using FinanceManagement.IoC;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using FinanceManagement.Entities.NewEntities;
using Microsoft.EntityFrameworkCore;
using FinanceManagement.Enums;
using FinanceManagement.Managers.BTransactions.Dtos;
using FinanceManagement.Extension;
using Abp.UI;
using FinanceManagement.Managers.Settings;
using FinanceManagement.Helper;
using FinanceManagement.Uitls;
using FinanceManagement.GeneralModels;
using Microsoft.AspNetCore.Http;
using System.IO;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using System.Data;
using OfficeOpenXml;
using FinanceManagement.Managers.Periods;
using FinanceManagement.Managers.OutcomingEntries;
using Abp.Collections.Extensions;
using FinanceManagement.Managers.OutcomingEntries.Dtos;
using FinanceManagement.Managers.Commons;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.AspNetCore.Mvc;
using AutoMapper.Internal;

namespace FinanceManagement.Managers.BTransactions
{
    public class BTransactionManager : DomainManager, IBTransactionManager
    {
        private readonly IMySettingManager _mySettingManager;
        private readonly IOutcomingEntryManager _outcomingEntryManager;
        private readonly ICommonManager _commonManager;

        public BTransactionManager(IWorkScope ws, IMySettingManager mySettingManager, IOutcomingEntryManager outcomingEntryManager, ICommonManager commonManager) : base(ws)
        {
            _mySettingManager = mySettingManager;
            _outcomingEntryManager = outcomingEntryManager;
            _commonManager = commonManager;
        }
        public async Task SetDoneBTransaction(long bTransactionId)
        {
            var bTransaction = await _ws.GetAsync<BTransaction>(bTransactionId);
            bTransaction.Status = BTransactionStatus.DONE;
            await CurrentUnitOfWork.SaveChangesAsync();
        }

        public async Task<ResultClientPaidDto> AddClientPaid(long fromAccountId, long btransactionId)
        {
            var item = await _ws.GetAll<BTransaction>()
                .Where(s => s.Id == btransactionId)
                .Select(s => new
                {
                    BTransaction = s,
                    BankAccountNumber = s.BankAccount.BankNumber,
                    CurrencyName = s.BankAccount.Currency.Name,
                    CurrencyId = s.BankAccount.CurrencyId
                })
                .FirstOrDefaultAsync();
            if (item.BTransaction.Money <= 0)
                throw new UserFriendlyException("Không thể trả nợ với số tiền <= 0");

            if (item.BTransaction.Status == BTransactionStatus.DONE)
                throw new UserFriendlyException("Transaction is done!");

            item.BTransaction.FromAccountId = fromAccountId;
            await _ws.UpdateAsync(item.BTransaction);

            return new ResultClientPaidDto
            {
                BankTransactionName = Helpers.GetNameBankTransaction(new InputGetNameBankTransaction
                {
                    BankNumber = item.BankAccountNumber,
                    TimeAt = item.BTransaction.TimeAt,
                    CurrencyName = item.CurrencyName,
                    Money = item.BTransaction.Money
                }),
                AccountId = fromAccountId,
                BankAccountId = item.BTransaction.BankAccountId,
                BTransactionId = btransactionId,
                Money = item.BTransaction.Money,
                TimeAt = item.BTransaction.TimeAt,
                CurrencyId = item.CurrencyId,
                CurrencyName = item.CurrencyName,
            };
        }

        public async Task<List<CurrencyNeedConvertDto>> CheckCurrencyBetweenAccountAndBTrasaction(long btransactionId, long accountId)
        {
            var currencyDefault = await GetCurrencyDefaultAsync();
            if (currencyDefault.IsNullOrDefault())
                throw new UserFriendlyException("Bạn cần xét 1 loại tiền làm loại tiền mặc định cho Request chi");

            var currencyIdOfBTransaction = await _ws.GetAll<BTransaction>()
                .Where(s => s.Id == btransactionId)
                .Select(s => new { s.BankAccount.CurrencyId, s.BankAccount.Currency.Name })
                .FirstOrDefaultAsync();

            var currencyNeedConverts = await _ws.GetAll<Invoice>()
                .Where(s => s.AccountId == accountId)
                .Where(s => s.Status != NInvoiceStatus.HOAN_THANH && s.Status != NInvoiceStatus.KHONG_TRA)
                .Where(s => s.CurrencyId != currencyIdOfBTransaction.CurrencyId)
                .Select(s => new CurrencyNeedConvertDto
                {
                    ToCurrencyId = s.CurrencyId,
                    ToCurrencyName = s.Currency.Name,
                    FromCurrencyId = currencyIdOfBTransaction.CurrencyId.Value,
                    FromCurrencyName = currencyIdOfBTransaction.Name,
                    IsReverseExchangeRate = currencyIdOfBTransaction.CurrencyId.Value == currencyDefault.Id ? true : false
                })
                .ToListAsync();
            return currencyNeedConverts.AsQueryable().DistinctBy(s => s.ToCurrencyId).ToList();
        }
        public async Task<bool> PaymentInvoiceByAccount(PaymentInvoiceForAccountDto input)
        {
            var btransaction = await _ws.GetAll<BTransaction>()
                .Where(s => s.Id == input.BTransactionId)
                .Include(s => s.BankAccount)
                .Include(s => s.FromAccount)
                .FirstOrDefaultAsync();
            if (btransaction.Status == BTransactionStatus.DONE)
                throw new UserFriendlyException("Transaction is done!");

            input.CurrencyNeedConverts.ForEach(s =>
            {
                if (s.IsReverseExchangeRate)
                {
                    s.ExchangeRate = 1 / s.ExchangeRate;
                }
            });

            var moneyOfTransaction = btransaction.Money;
            if (input.IsCreateBonus)
            {
                moneyOfTransaction -= input.IncomingEntryValue.Value;
            }
            bool isDone = false;
            using (CurrentUnitOfWork.DisableFilter(nameof(IMustHavePeriod)))
            {
                isDone = await ProcessPayment(moneyOfTransaction, btransaction, input.AccountId, input.CurrencyNeedConverts);
            }
            btransaction.Status = BTransactionStatus.DONE;
            await CurrentUnitOfWork.SaveChangesAsync();

            return isDone;
        }

        public async Task<bool> ProcessPayment(
            double money,
            BTransaction bTransaction,
            long accountId,
            List<CurrencyNeedConvertDto> currencyNeedConverts
        )
        {
            var debtIncomingEntryType = await _mySettingManager.GetDebtClientAsync();

            var invoiceNotDone = await _ws.GetAll<Invoice>()
               .Include(s => s.IncomingEntries)
               .Where(s => s.AccountId == accountId)
               .Where(s => s.Status != NInvoiceStatus.HOAN_THANH && s.Status != NInvoiceStatus.KHONG_TRA)
               .OrderBy(s => s.Deadline)
               .ToListAsync();

            var bankTransactionId = await _ws.GetAll<BankTransaction>()
                .Where(s => s.BTransactionId == bTransaction.Id)
                .Select(s => s.Id)
                .FirstOrDefaultAsync();

            if (!invoiceNotDone.Any())
            {
                await CreateEspeciallyIncoming(money, bankTransactionId, bTransaction);
                return true;
            }

            foreach (var invoice in invoiceNotDone)
            {
                if (money <= 0) break;

                if (invoice.CurrencyId != bTransaction.BankAccount.CurrencyId)
                {
                    money = await HandleTransactionDifferentCurrency(invoice, currencyNeedConverts, bTransaction, money, debtIncomingEntryType.Id, bankTransactionId);
                }
                else
                {
                    money = await HandleTransactionSameCurrency(invoice, bTransaction, money, debtIncomingEntryType.Id, bankTransactionId);
                }
            }

            bool isDone = false;
            if (money <= 0)
            {
                bTransaction.Status = BTransactionStatus.DONE;
                await _ws.UpdateAsync(bTransaction);
                isDone = true;
            }
            else
            {
                await CreateEspeciallyIncoming(
                    money,
                    bankTransactionId,
                    bTransaction
                );
            }

            await CurrentUnitOfWork.SaveChangesAsync();
            return isDone;
        }

        private async Task<double> HandleTransactionSameCurrency(
            Invoice invoice,
            BTransaction btransaction,
            double moneyOfTransaction,
            long incomingEntryTypeId,
            long? bankTransactionId
        )
        {

            var totalMoneyPaidOfRevenue = invoice.IncomingEntries
                       .Where(s => !s.IsDeleted)
                       .Select(s => s.Value * s.ExchangeRate)
                       .Sum();
            double moneyRemainningOfRevenue = (double)(invoice.CollectionDebt - totalMoneyPaidOfRevenue);

            if (moneyOfTransaction < moneyRemainningOfRevenue)
            {
                await _ws.InsertAsync<IncomingEntry>(new IncomingEntry
                {
                    InvoiceId = invoice.Id,
                    Name = $"{btransaction.FromAccount.Name} thanh toán {invoice.NameInvoice} - {invoice.Month}/{invoice.Year}",
                    BTransactionId = btransaction.Id,
                    Value = moneyOfTransaction,
                    IncomingEntryTypeId = incomingEntryTypeId,
                    ExchangeRate = FinanceManagementConsts.DEFAULT_EXCHANGE_RATE,
                    BankTransactionId = bankTransactionId,
                    AccountId = btransaction.FromAccountId,
                    CurrencyId = btransaction.BankAccount.CurrencyId,
                    PeriodId = btransaction.PeriodId
                });
                invoice.Status = NInvoiceStatus.TRA_1_PHAN;
                await _ws.UpdateAsync(invoice);
                return 0;
            }

            if (moneyOfTransaction - moneyRemainningOfRevenue < 1)
                moneyRemainningOfRevenue = moneyOfTransaction;

            await _ws.InsertAsync<IncomingEntry>(new IncomingEntry
            {
                InvoiceId = invoice.Id,
                Name = $"{btransaction.FromAccount.Name} thanh toán {invoice.NameInvoice} - {invoice.Month}/{invoice.Year}",
                BTransactionId = btransaction.Id,
                Value = moneyRemainningOfRevenue,
                IncomingEntryTypeId = incomingEntryTypeId,
                ExchangeRate = FinanceManagementConsts.DEFAULT_EXCHANGE_RATE,
                BankTransactionId = bankTransactionId,
                AccountId = btransaction.FromAccountId,
                CurrencyId = btransaction.BankAccount.CurrencyId,
                PeriodId = btransaction.PeriodId
            });
            invoice.Status = NInvoiceStatus.HOAN_THANH;

            moneyOfTransaction -= moneyRemainningOfRevenue;
            return moneyOfTransaction;
        }
        private async Task<double> HandleTransactionDifferentCurrency(
            Invoice invoice,
            List<CurrencyNeedConvertDto> currencyNeedConverts,
            BTransaction btransaction,
            double moneyOfTransaction,
            long incomingEntryTypeId,
            long? bankTransactionId
        )
        {
            //get exchange rate to convert currency this invoice
            var currencyNeedConvert = currencyNeedConverts.Where(s => s.ToCurrencyId == invoice.CurrencyId).FirstOrDefault();

            //money of BTransaction after convert
            var moneyOfTransactionAfterConvert = moneyOfTransaction * currencyNeedConvert.ExchangeRate;

            //money paid of client for this invoice
            var totalMoneyPaidOfRevenue = invoice.IncomingEntries
                   .Where(s => !s.IsDeleted)
                   .Select(s => s.Value * s.ExchangeRate)
                   .Sum();

            //money remaining of this invoice = CollectionDebt - Money Paid
            double moneyRemainningOfRevenue = (double)(invoice.CollectionDebt - totalMoneyPaidOfRevenue);

            if (moneyOfTransactionAfterConvert < moneyRemainningOfRevenue)
            {
                await _ws.InsertAsync<IncomingEntry>(new IncomingEntry
                {
                    Name = $"{btransaction.FromAccount.Name} thanh toán {invoice.NameInvoice} - {invoice.Month}/{invoice.Year}",
                    InvoiceId = invoice.Id,
                    BTransactionId = btransaction.Id,
                    Value = moneyOfTransactionAfterConvert / currencyNeedConvert.ExchangeRate,
                    ExchangeRate = currencyNeedConvert.ExchangeRate,
                    IncomingEntryTypeId = incomingEntryTypeId,
                    BankTransactionId = bankTransactionId,
                    AccountId = btransaction.FromAccountId,
                    CurrencyId = btransaction.BankAccount.CurrencyId,
                    PeriodId = btransaction.PeriodId
                });
                invoice.Status = NInvoiceStatus.TRA_1_PHAN;
                await _ws.UpdateAsync(invoice);
                return 0;
            }

            if ((moneyOfTransactionAfterConvert - moneyRemainningOfRevenue) / currencyNeedConvert.ExchangeRate < 1)
            {
                double exchangeRate = moneyRemainningOfRevenue / moneyOfTransaction;
                await _ws.InsertAsync<IncomingEntry>(new IncomingEntry
                {
                    Name = $"{btransaction.FromAccount.Name} thanh toán {invoice.NameInvoice} - {invoice.Month}/{invoice.Year}",
                    InvoiceId = invoice.Id,
                    BTransactionId = btransaction.Id,
                    Value = moneyRemainningOfRevenue / exchangeRate,
                    ExchangeRate = exchangeRate,
                    IncomingEntryTypeId = incomingEntryTypeId,
                    BankTransactionId = bankTransactionId,
                    AccountId = btransaction.FromAccountId,
                    CurrencyId = btransaction.BankAccount.CurrencyId,
                    PeriodId = btransaction.PeriodId
                });
                invoice.Status = NInvoiceStatus.HOAN_THANH;
                await _ws.UpdateAsync(invoice);

                return 0;
            }

            await _ws.InsertAsync<IncomingEntry>(new IncomingEntry
            {
                Name = $"{btransaction.FromAccount.Name} thanh toán {invoice.NameInvoice} - {invoice.Month}/{invoice.Year}",
                InvoiceId = invoice.Id,
                BTransactionId = btransaction.Id,
                Value = moneyRemainningOfRevenue / currencyNeedConvert.ExchangeRate,
                ExchangeRate = currencyNeedConvert.ExchangeRate,
                IncomingEntryTypeId = incomingEntryTypeId,
                BankTransactionId = bankTransactionId,
                AccountId = btransaction.FromAccountId,
                CurrencyId = btransaction.BankAccount.CurrencyId,
                PeriodId = btransaction.PeriodId
            });
            invoice.Status = NInvoiceStatus.HOAN_THANH;
            await _ws.UpdateAsync(invoice);

            moneyOfTransaction = (moneyOfTransactionAfterConvert - moneyRemainningOfRevenue) / currencyNeedConvert.ExchangeRate;

            return moneyOfTransaction;
        }

        private async Task CreateEspeciallyIncoming(
            double moneyOfTransaction,
            long? bankTransactionId,
            BTransaction bTransaction
        )
        {
            var balanceIncomingEntryType = await _mySettingManager.GetBalanceClientAsync();
            if (string.IsNullOrEmpty(balanceIncomingEntryType.Code))
                throw new UserFriendlyException("Not Found Balance Incoming Type!");

            await _ws.InsertAsync<IncomingEntry>(new IncomingEntry
            {
                Name = bTransaction.FromAccount.Name + " - Khách hàng trả trước",
                BTransactionId = bTransaction.Id,
                Value = moneyOfTransaction,
                ExchangeRate = FinanceManagementConsts.DEFAULT_EXCHANGE_RATE,
                IncomingEntryTypeId = balanceIncomingEntryType.Id,
                BankTransactionId = bankTransactionId,
                AccountId = bTransaction.FromAccountId,
                CurrencyId = bTransaction.BankAccount.CurrencyId,
                PeriodId = bTransaction.PeriodId
            });
        }
        public async Task<GetAllBTransactionDto> CreateTransaction(CreateBTransactionDto input)
        {
            var bTransaction = ObjectMapper.Map<BTransaction>(input);
            var id = await _ws.InsertAndGetIdAsync(bTransaction);
            await CurrentUnitOfWork.SaveChangesAsync();
            return await IQGetAllBTransaction()
                .Where(s => s.BTransactionId == id)
                .FirstOrDefaultAsync();
        }
        public IQueryable<GetAllBTransactionDto> IQGetAllBTransaction()
        {
            var query = from b in _ws.GetAll<BTransaction>()
                        join bankTrans in _ws.GetAll<BankTransaction>() on b.Id equals bankTrans.BTransactionId
                        into trans
                        from bb in trans.DefaultIfEmpty()
                        select new GetAllBTransactionDto
                        {
                            BTransactionId = b.Id,
                            BankAccountNumber = b.BankAccount.BankNumber,
                            BankAccountName = b.BankAccount.HolderName,
                            BTransactionStatus = b.Status,
                            CurrencyId = b.BankAccount.CurrencyId.Value,
                            CurrencyName = b.BankAccount.Currency.Name,
                            Note = b.Note,
                            MoneyNumber = b.Money,
                            TimeAt = b.TimeAt,
                            BankAccountId = b.BankAccountId,
                            IsCrawl = b.IsCrawl,
                            BankTransactionId = bb.Id,
                            BankTransactionName = bb.Name,
                            LastModifiedTime = b.LastModificationTime,
                            LastModifiedUserId = b.LastModifierUserId,
                            LastModifiedUser = b.LastModifiedUser != null ? b.LastModifiedUser.FullName : string.Empty,
                            CreationTime = b.CreationTime,
                            CreationUserId = b.CreatorUserId,
                            CreationUser = b.CreationUser != null ? b.CreationUser.FullName : string.Empty,
                        };
            return query;
        }
        public async Task<CreateBTransactionDto> UpdateTransaction(CreateBTransactionDto input)
        {
            var bTransaction = await _ws.GetAsync<BTransaction>(input.BTransactionId);
            ObjectMapper.Map(input, bTransaction);
            await _ws.UpdateAsync(bTransaction);
            await CurrentUnitOfWork.SaveChangesAsync();
            return input;
        }
        public async Task<long> Delete(long id)
        {
            var bTransaction = await _ws.GetAsync<BTransaction>(id);
            if (bTransaction.Status == BTransactionStatus.DONE)
                throw new UserFriendlyException("Không thể xóa giao dịch đã Hoàn Thành!");
            await _ws.DeleteAsync(bTransaction);
            return id;
        }
        public async Task CheckCreateOutcomingBankTransaction(long outcomingEntryId, long? currencyIdOfBTrans, double valueOfBTrans)
        {
            var outcomingEntry = await _ws.GetAll<OutcomingEntry>()
                .Where(s => s.Id == outcomingEntryId)
                .Select(s => new { s.WorkflowStatus.Code, s.Value })
                .FirstOrDefaultAsync();
            if ((Math.Abs(valueOfBTrans) - Math.Abs(outcomingEntry.Value)) >= 1)
                throw new UserFriendlyException("Biến động số dư không thể lớn hơn request chi");

            var canLinkWithOutComingEnd = await _mySettingManager.GetCanLinkWithOutComingEnd();
            if (canLinkWithOutComingEnd)
                return;
            //Tạm thời bỏ
            if (outcomingEntry.Code == FinanceManagementConsts.WORKFLOW_STATUS_END)
                throw new UserFriendlyException("Không thể link request chi đã Hoàn thành");
        }
        public async Task CreateOutcomingBankTransaction(long outcomingEntryId, long bankTransactionId)
        {
            await _ws.InsertAndGetIdAsync(new OutcomingEntryBankTransaction
            {
                OutcomingEntryId = outcomingEntryId,
                BankTransactionId = bankTransactionId,
            });
        }
        public async Task<LinkBTransactionInfomationDto> GetBTransactionInformation(long bTransactionId)
        {
            return await _ws.GetAll<BTransaction>().Where(s => s.Id == bTransactionId)
                       .Select(s => new LinkBTransactionInfomationDto
                       {
                           BankAccountId = s.BankAccountId,
                           Money = s.Money,
                           TimeAt = s.TimeAt,
                           Status = s.Status,
                           BankNumber = s.BankAccount.BankNumber,
                           CurrencyName = s.BankAccount.Currency.Name,
                           CurrencyId = s.BankAccount.CurrencyId,
                           FromAccountId = s.FromAccountId
                       })
                       .FirstOrDefaultAsync();
        }
        public async Task LinkBankTransactionToBTransaction(LinkBankTransactionToBTransactionDto input)
        {
            var bTransactionInfo = await _ws.GetAll<BTransaction>()
                .Where(s => s.Id == input.BTransactionId)
                .Select(s => new { s.Money, s.TimeAt, s.BankAccount.CurrencyId })
                .FirstOrDefaultAsync();

            var bankTransactionInfo = await _ws.GetAsync<BankTransaction>(input.BankTransactionId);
            bankTransactionInfo.TransactionDate = bTransactionInfo.TimeAt;
            bankTransactionInfo.FromValue = Math.Abs(bTransactionInfo.Money);
            bankTransactionInfo.ToValue = Helpers.RoundMoneyToEven(bankTransactionInfo.FromValue * input.ExchangeRate);
            bankTransactionInfo.BTransactionId = input.BTransactionId;

            //update incoming in btransaction
            var incomings = await _ws.GetAll<IncomingEntry>()
                .Where(s => s.BTransactionId == input.BTransactionId)
                .ToListAsync();
            foreach (var incoming in incomings)
            {
                incoming.BankTransactionId = bankTransactionInfo.Id;
                incoming.CurrencyId = bTransactionInfo.CurrencyId;
            }

            await SetDoneBTransaction(input.BTransactionId);
            await CurrentUnitOfWork.SaveChangesAsync();
        }

        public async Task<int> CountBTransactionPendingStatus()
        {
            return await _ws.GetAll<BTransaction>()
                    .AsNoTracking()
                    .CountAsync(x => x.Status != Enums.BTransactionStatus.DONE);
        }
        public async Task<bool> HasBTransaction()
        {
            return await _ws.GetAll<BTransaction>()
                .AsNoTracking()
                .AnyAsync();
        }

        public async Task<DifferentBetweenBankTransAndBTransDto> CheckDifferentBetweenBankTransAndBTrans(LinkBankTransactionToBTransactionDto input)
        {
            var bTransactionInfo = await _ws.GetAll<BTransaction>()
                .Where(s => s.Id == input.BTransactionId)
                .Select(s => new { s.Money, s.TimeAt, s.BankAccountId, CurrencyName = s.BankAccount.Currency.Name, s.FromAccountId, s.BankAccount.CurrencyId })
                .FirstOrDefaultAsync();

            var bankTransactionInfo = await _ws.GetAll<BankTransaction>()
                .Where(s => s.Id == input.BankTransactionId)
                .Select(s => new { s.FromValue, s.TransactionDate, s.ToBankAccountId, s.FromBankAccountId, s.BTransactionId })
                .FirstOrDefaultAsync();
            if (bankTransactionInfo.BTransactionId.HasValue)
                throw new UserFriendlyException($"Giao dịch ngân hàng #{input.BankTransactionId} đã được link đến biến động số dư #{bankTransactionInfo.BTransactionId}");

            var bankTransactionInfoByBTransactionId = await _ws.GetAll<BankTransaction>()
                .Where(s => s.BTransactionId == input.BTransactionId)
                .Select(s => new { s.Id, s.Name })
                .FirstOrDefaultAsync();
            if (bankTransactionInfoByBTransactionId != null)
                throw new UserFriendlyException($"Biến động số dư #{input.BTransactionId} đã được link tới giao dịch ngân hàng #{bankTransactionInfoByBTransactionId.Id}-{bankTransactionInfoByBTransactionId.Name}");

            var fromAccount = await _ws.GetAll<BankAccount>()
                .Where(s => s.Id == bankTransactionInfo.FromBankAccountId)
                .Select(s => new { s.AccountId, s.CurrencyId })
                .FirstOrDefaultAsync();

            var result = new DifferentBetweenBankTransAndBTransDto();
            //ghi nhận thu 
            if (bTransactionInfo.Money > 0)
            {
                if (bTransactionInfo.FromAccountId.HasValue && fromAccount.AccountId != bTransactionInfo.FromAccountId)
                    throw new UserFriendlyException("Không thể link giao dịch ngân hàng tới biến động số dư vì khác client");
                if (bTransactionInfo.CurrencyId != fromAccount.CurrencyId)
                    throw new UserFriendlyException("Không thể link giao dịch ngân hàng tới biến động số dư vì khác loại tiền từ bank gửi");
                if (bTransactionInfo.BankAccountId != bankTransactionInfo.ToBankAccountId)
                    throw new UserFriendlyException("Không thể link giao dịch ngân hàng tới biến động số dư vì khác số tài khoản nhận tiền");
            }
            //request chi
            else
            {
                if (bTransactionInfo.CurrencyId != fromAccount.CurrencyId)
                    throw new UserFriendlyException("Không thể link giao dịch ngân hàng tới biến động số dư vì khác loại tiền từ bank gửi");

                var currencyNameOfBankAccount = await _ws.GetAll<BankAccount>()
                .Where(s => s.Id == bankTransactionInfo.ToBankAccountId)
                .Select(s => s.Currency.Name)
                .FirstOrDefaultAsync();

                if (bTransactionInfo.CurrencyName != currencyNameOfBankAccount)
                {
                    result.FromCurrencyName = bTransactionInfo.CurrencyName;
                    result.ToCurrencyName = currencyNameOfBankAccount;
                    result.IsDifferentCurrency = true;
                }
            }

            if (bankTransactionInfo.FromValue != bTransactionInfo.Money)
            {
                result.BTransactionValueNumber = bTransactionInfo.Money;
                result.BankTransactionValueNumber = bankTransactionInfo.FromValue;
                result.IsDifferentValue = true;
            }

            if (bankTransactionInfo.TransactionDate.Date != bTransactionInfo.TimeAt.Date)
            {
                result.BTransactionTimeAt = bTransactionInfo.TimeAt;
                result.BankTransactionTimeAt = bankTransactionInfo.TransactionDate;
                result.IsDifferentValue = true;
            }

            return result;
        }
        public async Task<object> ImportBTransaction(ImportBTransactionDto input)
        {
            var dataFile = ReadExcelFileSAX(input.BTransactionFile);
            var dataTransform = TransformDataFileExcel(dataFile);
            var dateTimes = dataTransform.Data.Select(s => s.ValueDate.Date);

            var bTransactions = await _ws.GetAll<BTransaction>()
                .Where(x => x.BankAccountId == input.BankAccountId)
                .Where(x => dateTimes.Contains(x.TimeAt.Date))
                .ToListAsync();

            var listSuccess = new List<string>();
            var listExists = new List<string>();
            var listPending = new List<string>();
            foreach (var item in dataTransform.Data)
            {
                if (item.Value > 0 && (item.TransactionVAT > 0 || item.TransactionFee > 0))
                {
                    listPending.Add($"Dòng {item.Row} cần chờ được xử lý");
                    continue;
                }

                if (bTransactions.Where(x => x.Note.Contains(item.Message) && x.TimeAt.Date == item.ValueDate.Date && x.Money == item.Value).Any())
                {
                    listExists.Add($"Đã tồn tại dòng {item.Row}");
                    continue;
                }

                await _ws.InsertAsync(new BTransaction
                {

                    BankAccountId = input.BankAccountId,
                    Money = item.Value == 0 ? item.TransactionFee + item.TransactionVAT : item.Value,
                    TimeAt = item.ValueDate,
                    Note = item.Message,
                    Status = BTransactionStatus.PENDING,
                });
                listSuccess.Add($"Thêm mới thành công dòng {item.Row}");
            }
            return new { Pending = listPending, Exists = listExists, Success = listSuccess, Errors = dataTransform.Errors };
        }
        private ResultTransformDataFromFile TransformDataFileExcel(List<DataFileImport> input)
        {
            var result = new ResultTransformDataFromFile();
            foreach (var item in input)
            {
                StringBuilder error = new StringBuilder($"Dòng {item.Row}: ");
                var rs = new DataTransformFromFile();
                bool isSuccess = true;
                //transform value date
                var pDateTime = DateTimeUtils.ParseDateTime(item.ValueDate);
                if (pDateTime.IsValid)
                {
                    rs.ValueDate = pDateTime.Result.Value;
                }
                else
                {
                    isSuccess = false;
                    error.Append("Không convert được VALUE DATE. ");
                }
                //transform debit
                if (!string.IsNullOrEmpty(item.Debit))
                {
                    var pDebit = Helpers.ParseMoney(item.Debit);
                    if (pDebit.IsValid)
                    {
                        rs.Value = pDebit.Result.Value;
                    }
                    else
                    {
                        isSuccess = false;
                        error.Append("Không convert được DEBIT. ");
                    }
                }
                //transform credit
                if (!string.IsNullOrEmpty(item.Credit))
                {
                    var pCredit = Helpers.ParseMoney(item.Credit);
                    if (pCredit.IsValid)
                    {
                        rs.Value = pCredit.Result.Value;
                    }
                    else
                    {
                        isSuccess = false;
                        error.Append("Không convert được CREDIT. ");
                    }
                }
                //transform transaction fee
                if (!string.IsNullOrEmpty(item.TransactionFee))
                {
                    var pTransferFee = Helpers.ParseMoney(item.TransactionFee);
                    if (pTransferFee.IsValid)
                    {
                        rs.TransactionFee = pTransferFee.Result.Value;
                    }
                    else
                    {
                        isSuccess = false;
                        error.Append("không convert được TRANSFER FEE. ");
                    }
                }
                //transform transaction vat
                if (!string.IsNullOrEmpty(item.TransactionVAT))
                {
                    var pTransactionVAT = Helpers.ParseMoney(item.TransactionVAT);
                    if (pTransactionVAT.IsValid)
                    {
                        rs.TransactionVAT = pTransactionVAT.Result.Value;
                    }
                    else
                    {
                        isSuccess = false;
                        error.Append("không convert được TRANSACTION VAT. ");
                    }
                }
                rs.Message = item.Message;
                rs.Row = item.Row;

                if (isSuccess) result.Data.Add(rs);
                else result.Errors.Add(error.ToString());
            }
            return result;
        }
        private List<DataFileImport> ReadExcelFileSAX(IFormFile file)
        {
            var list = new List<DataFileImport>();
            using (var stream = file.OpenReadStream())
            {
                using (var package = new ExcelPackage(stream))
                {
                    ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

                    ExcelWorksheet worksheet = package.Workbook.Worksheets[0];

                    var columnCount = worksheet.Dimension.Columns;

                    var rowCount = worksheet.Dimension.End.Row;

                    int startRow = 18;
                    for (int row = startRow; row <= rowCount; row++)
                    {
                        var item = new DataFileImport
                        {
                            Row = row,
                            ValueDate = worksheet.Cells[row, 1].GetValue<string>(),
                            Message = worksheet.Cells[row, 4].GetValue<string>(),
                            Debit = worksheet.Cells[row, 5].GetValue<string>(),
                            Credit = worksheet.Cells[row, 6].GetValue<string>(),
                            TransactionFee = worksheet.Cells[row, 7].GetValue<string>(),
                            TransactionVAT = worksheet.Cells[row, 8].GetValue<string>()
                        };
                        if (string.IsNullOrEmpty(item.ValueDate))
                            break;
                        list.Add(item);
                    }
                    return list;
                }
            }
        }
        /// <summary>
        /// Get Information Link OutcomingEntry to BTransaction
        /// Belong to Period, BTransaction name, Value, Currency -> BankTransaction -> Request chi
        /// </summary>
        /// <param name="bTransactionId"></param>
        /// <returns></returns>
        public async Task<GetInfoRollbackOutcomingEntryWithBTransactionDto> GetInfoRollbankOutcomingEntryWithBTransaction(long bTransactionId)
        {
            var qBankAccountInfo = _ws.GetAll<BankAccount>()
                .Select(x => new { x.Id, x.HolderName, CurrencyName = x.Currency.Name });
            var query = from b in _ws.GetAll<BTransaction>().Where(x => x.Id == bTransactionId)
                        join bankTrans in _ws.GetAll<BankTransaction>() on b.Id equals bankTrans.BTransactionId into gr
                        from bt in gr.DefaultIfEmpty()
                        select new GetInfoRollbackOutcomingEntryWithBTransactionDto
                        {
                            BTransactionInfo = new GetInfoRollbackBTransactionDto
                            {
                                BankAccountName = b.BankAccount.HolderName,
                                BankNumber = b.BankAccount.BankNumber,
                                BTransactionId = bTransactionId,
                                CurrencyName = b.BankAccount.Currency.Name,
                                Money = b.Money,
                                Note = b.Note,
                                TimeAt = b.TimeAt
                            },
                            BankTransactionInfo = new GetInfoRollbackBankTransactionDto
                            {
                                BankTransactionId = bt.Id,
                                BankTransactionName = bt.Name,
                                FromBankAccountId = bt.FromBankAccountId,
                                FromValue = bt.FromValue,
                                ToBankAccountId = bt.ToBankAccountId,
                                ToValue = bt.ToValue,
                            },
                            RollbackOutcomingEntryInfos = bt.OutcomingEntryBankTransactions
                            .Select(x => new GetInfoRollbackOutcomingEntryDto
                            {
                                OutcomingEntryId = x.Id,
                                Name = x.OutcomingEntry.Name,
                                Value = x.OutcomingEntry.Value,
                                OutcomingEntryTypeName = x.OutcomingEntry.OutcomingEntryType.Name,
                                WorkflowStatus = x.OutcomingEntry.WorkflowStatus.Name,
                                WorkflowStatusCode = x.OutcomingEntry.WorkflowStatus.Code
                            })
                        };
            var result = await query.FirstOrDefaultAsync();
            if (result == null)
                throw new UserFriendlyException($"Không tìm thấy Biến động số dư {bTransactionId}");

            if (result.BankTransactionInfo == null)
                return result;

            var listBankAccountId = new List<long?>() { result.BankTransactionInfo.FromBankAccountId, result.BankTransactionInfo.ToBankAccountId };

            var dicbankAccountInfos = await qBankAccountInfo
                .Where(x => listBankAccountId.Contains(x.Id))
                .ToDictionaryAsync(x => x.Id, x => new { x.HolderName, x.CurrencyName });

            result.BankTransactionInfo.FromBankAccountName = dicbankAccountInfos[result.BankTransactionInfo.FromBankAccountId].HolderName;
            result.BankTransactionInfo.FromCurrencyName = dicbankAccountInfos[result.BankTransactionInfo.FromBankAccountId].CurrencyName;
            result.BankTransactionInfo.ToBankAccountName = dicbankAccountInfos[result.BankTransactionInfo.ToBankAccountId].HolderName;
            result.BankTransactionInfo.ToCurrencyName = dicbankAccountInfos[result.BankTransactionInfo.ToBankAccountId].CurrencyName;

            return result;
        }
        /// <summary>
        /// Function dùng cho 2 chức năng:
        /// Link 1 request chi - thu hồi
        /// Link nhiều request chi - thu hồi
        /// </summary>
        /// <param name="bTransactionId"></param>
        /// <param name="currentPeriodId"></param>
        /// <returns></returns>
        /// <exception cref="UserFriendlyException"></exception>
        public async Task RollBackOutcomingEntryWithBTransaction(long bTransactionId, int currentPeriodId)
        {
            var query = from b in _ws.GetAll<BTransaction>().Where(x => x.Id == bTransactionId)
                        join bankTrans in _ws.GetAll<BankTransaction>() on b.Id equals bankTrans.BTransactionId into gr
                        from bt in gr.DefaultIfEmpty()
                        select new
                        {
                            BTransaction = b,
                            BankTransaction = bt,
                            OutcomingEntries = bt.OutcomingEntryBankTransactions.Select(b => b.OutcomingEntry),
                            OutcomingEntryBankTransaction = bt.OutcomingEntryBankTransactions,
                            ValueOutcomingEntries = bt.OutcomingEntryBankTransactions.Sum(b => b.OutcomingEntry.Value)
                        };
            var result = await query.FirstOrDefaultAsync();

            if (result == null)
                throw new UserFriendlyException($"Không tìm thấy biến động số dư {bTransactionId}");

            if (result.BTransaction != null && result.BankTransaction == null)
            {
                result.BTransaction.Status = BTransactionStatus.PENDING;
                await CurrentUnitOfWork.SaveChangesAsync();
                return;
            }

            if (currentPeriodId != 0 && result.BTransaction.PeriodId != currentPeriodId)
                throw new UserFriendlyException("Không thể chỉnh sửa biến động số dư trong kì [IN_ACTIVE]");

            if (result.BTransaction.Status != BTransactionStatus.DONE)
                throw new UserFriendlyException("Biến động số dư chưa [DONE]");

            var countOutcomingEntry = result.OutcomingEntries.Count();
            //link 1 request chi với 1 biến động số dư
            if (countOutcomingEntry == 1)
            {
                var outcomingEntry = result.OutcomingEntries.First();

                //if (outcomingEntry.WorkflowStatus.Code.Trim() == FinanceManagementConsts.WORKFLOW_STATUS_END)
                //    throw new UserFriendlyException("Không thể thu hồi vì Request chi đã [THỰC THI]");

                if (result.BankTransaction.ToValue > outcomingEntry.Value)
                    throw new UserFriendlyException($"Không thể thu hồi vì giao dịch ngân hàng {result.BankTransaction.ToValue} > Request chi {outcomingEntry.Value}");


                // Chuyển trạng thái của request chi về Approved
                if (outcomingEntry.WorkflowStatus.Code.Trim() == FinanceManagementConsts.WORKFLOW_STATUS_END)
                {
                    var workflowApprovedId = await _commonManager.GetWorkflowStatusApprovedId();
                    if (workflowApprovedId == default)
                    {
                        throw new UserFriendlyException($"Không thể thu hồi vì Request chi đã [THỰC THI] và không tồn tại WORKFLOWcó Code là {FinanceManagementConsts.WORKFLOW_STATUS_APPROVED}");
                    }
                    outcomingEntry.WorkflowStatusId = workflowApprovedId;
                    await _ws.UpdateAsync(outcomingEntry);
                    await _outcomingEntryManager.CreateOutcomingStatusHistory(new CreateOutcomingEntryStatusHistoryDto
                    {
                        OutcomingEntryId = outcomingEntry.Id,
                        WorkflowStatusId = outcomingEntry.WorkflowStatusId,
                        Value = outcomingEntry.Value,
                        CurrencyName = outcomingEntry.Currency.Name

                    }, currentPeriodId);

                }

            }
            //link nhiều request chi tới 1 biến động số dư
            else if (countOutcomingEntry > 1)
            {
                var allOutcomingEntryIdNotEnd = result.OutcomingEntries
                    .Where(x => x.WorkflowStatus.Code.Trim() != FinanceManagementConsts.WORKFLOW_STATUS_END)
                    .Select(x => x.Id)
                    .ToList();

                if (allOutcomingEntryIdNotEnd.Any())
                    throw new UserFriendlyException($"Không thể rollback khi có request chi chưa [THỰC THI]: {string.Join(",", allOutcomingEntryIdNotEnd)}");

                if (Math.Abs(result.BankTransaction.ToValue - result.ValueOutcomingEntries) >= 1)
                    throw new UserFriendlyException($"Không thể rollback khi Giao dịch ngân hàng {Helpers.FormatMoney(result.BankTransaction.ToValue)} != tổng request chi {result.ValueOutcomingEntries}");

                //move to approved status
                var workflowApproveId = await _ws.GetAll<WorkflowStatus>()
                    .Where(x => x.Code.Trim() == FinanceManagementConsts.WORKFLOW_STATUS_APPROVED)
                    .Select(x => x.Id)
                    .FirstOrDefaultAsync();

                result.OutcomingEntries.ToList().ForEach(item =>
                {
                    item.WorkflowStatusId = workflowApproveId;
                    _outcomingEntryManager.CreateOutcomingStatusHistory(new CreateOutcomingEntryStatusHistoryDto
                    {
                        OutcomingEntryId = item.Id,
                        Value = item.Value,
                        WorkflowStatusId = item.WorkflowStatusId,
                        CurrencyName = item.Currency.Name
                    }, currentPeriodId);
                });
            }
            //delete outcoming entry bank transaction
            result.OutcomingEntryBankTransaction.ToList().ForEach(item => item.IsDeleted = true);
            //delete bank transaction
            result.BankTransaction.IsDeleted = true;
            //revert b-transaction status
            result.BTransaction.Status = BTransactionStatus.PENDING;

            await CurrentUnitOfWork.SaveChangesAsync();
        }
        /// <summary>
        /// Bán ngoại tệ
        /// </summary>
        /// <param name="conversionTransactionDto"></param>
        /// <returns></returns>
        /// <exception cref="UserFriendlyException"></exception>
        public async Task<bool> CheckConversionTransaction(ConversionTransactionDto conversionTransactionDto)
        {
            if (!conversionTransactionDto.OutcomingEntryId.HasValue)
            {
                throw new UserFriendlyException("Không có yêu cầu chi");
            }
            if (!conversionTransactionDto.ToBankAccountId.HasValue)
            {
                throw new UserFriendlyException("Không có tài khoản ngân hàng NHẬN");
            }
            if (!conversionTransactionDto.FromBankAccountId.HasValue)
            {
                throw new UserFriendlyException("Không có tài khoản ngân hàng GỬI");
            }
            if (!conversionTransactionDto.IncomingEntryTypeId.HasValue)
            {
                throw new UserFriendlyException("Không có loại thu");
            }
            if (conversionTransactionDto.MinusBTransactionIds == null || conversionTransactionDto.MinusBTransactionIds.IsNullOrEmpty())
            {
                throw new UserFriendlyException("Không có biến động số dư âm");
            }
            if (conversionTransactionDto.PlusBTransactionIds == null || conversionTransactionDto.PlusBTransactionIds.IsNullOrEmpty())
            {
                throw new UserFriendlyException("Không có biến động số dư dương");
            }

            if (await IsAllowOutcomingEntryByMultipleCurrency())
            {
                throw new UserFriendlyException("Tính năng không còn được hỗ trợ");
            }

            var incomingEntryType = _ws.GetAll<IncomingEntryType>()
                .Where(s => s.Id == conversionTransactionDto.IncomingEntryTypeId)
                .FirstOrDefault();

            if (incomingEntryType == default)
            {
                throw new UserFriendlyException($"Không tìm thấy loại thu Id = {conversionTransactionDto.IncomingEntryTypeId}");
            }

            var bankAccount = _ws.GetAll<BankAccount>()
                .Select(s => new
                {
                    s.Id,
                    s.Account.Type,
                    s.CurrencyId
                })
                .Where(s => s.Id == conversionTransactionDto.ToBankAccountId || s.Id == conversionTransactionDto.FromBankAccountId)
                .ToList();

            var fromBankAccount = bankAccount.Where(s => s.Id == conversionTransactionDto.FromBankAccountId).FirstOrDefault();
            var toBankAccount = bankAccount.Where(s => s.Id == conversionTransactionDto.ToBankAccountId).FirstOrDefault();

            if (fromBankAccount == default)
            {
                throw new UserFriendlyException($"Không tồn tại tài khoản ngân hàng gửi Id = {conversionTransactionDto.FromBankAccountId}");
            }
            if (toBankAccount == default)
            {
                throw new UserFriendlyException($"Không tồn tại tài khoản ngân hàng nhận Id = {conversionTransactionDto.ToBankAccountId}");
            }

            if (fromBankAccount.Type == AccountTypeEnum.COMPANY)
            {
                throw new UserFriendlyException($"Không thể bán ngoại tệ với tài khoản ngân hàng GỬI là tài khoản thuộc công ty");
            }
            if (toBankAccount.Type == AccountTypeEnum.COMPANY)
            {
                throw new UserFriendlyException($"Không thể bán ngoại tệ với tài khoản ngân hàng NHẬN là tài khoản thuộc công ty");
            }

            var bTransactions = _ws.GetAll<BTransaction>()
                .Where(s => conversionTransactionDto.BTransactionIds.Contains(s.Id))
                .Select(s => new
                {
                    s.Id,
                    s.Money,
                    s.BankAccount.CurrencyId,
                    s.Status,
                    CurrencyName = s.BankAccount.Currency.Name,
                })
                .ToList();

            var doneBTransactionIds = bTransactions.Where(s => s.Status == BTransactionStatus.DONE).Select(s => s.Id.ToString()).ToList();
            var bTransactionCanNotFind = conversionTransactionDto.BTransactionIds.Where(s => !bTransactions.Select(x => x.Id).Contains(s)).ToList();

            if (!bTransactionCanNotFind.IsEmpty())
            {
                throw new UserFriendlyException($"Không thể thực hiện thao tác vì không tìm thấy các biến động số dư có Id: {string.Join(",", bTransactionCanNotFind)}");
            }

            if (!doneBTransactionIds.IsEmpty())
            {
                throw new UserFriendlyException($"Không thể thực hiện thao tác vì có các biến động số dư đã DONE: {string.Join(",", doneBTransactionIds)}");
            }

            var MinusBTransaction = bTransactions.Where(s => conversionTransactionDto.MinusBTransactionIds.Contains(s.Id)).ToList();
            var PlusBTransaction = bTransactions.Where(s => conversionTransactionDto.PlusBTransactionIds.Contains(s.Id)).ToList();
            double totalMoney = PlusBTransaction.Sum(s => s.Money);

            var currencyMinusBTransaction = MinusBTransaction.Select(s => s.CurrencyId).Distinct();
            var currencyPlusBTransaction = PlusBTransaction.Select(s => s.CurrencyId).Distinct();

            if (currencyMinusBTransaction.Count() > 1)
            {
                throw new UserFriendlyException("Các biến động số dư âm phải có cùng loại tiền");
            }
            if (currencyPlusBTransaction.Count() > 1)
            {
                throw new UserFriendlyException("Các biến động số dư dương phải có cùng loại tiền");
            }
            var currencyDefault = await GetCurrencyDefaultAsync();
            if (currencyDefault.IsNullOrDefault())
                throw new UserFriendlyException("Bạn cần xét 1 loại tiền làm loại tiền mặc định cho Request chi");

            var currencyOtherCurrencyDefaultPlusBTransaction = PlusBTransaction.Where(s => s.CurrencyId != currencyDefault.Id);

            if (!currencyOtherCurrencyDefaultPlusBTransaction.IsEmpty())
            {
                throw new UserFriendlyException($"Các biến động số DƯƠNG phải có cùng loại tiền {currencyDefault.Name}");
            }
            if (toBankAccount.CurrencyId != currencyDefault.Id)
            {
                throw new UserFriendlyException($"Không thể bán ngoại tệ với tài khoản ngân hàng NHẬN có loại tiền khác " + currencyDefault.Name);
            }
            if (fromBankAccount.CurrencyId != currencyDefault.Id)
            {
                throw new UserFriendlyException($"Không thể bán ngoại tệ với tài khoản ngân hàng GỬI có loại tiền khác " + currencyDefault.Name);
            }

            var outcomgingEntryMoneyInfo = await _outcomingEntryManager.GetOutcomingEntryMoneyInfo(conversionTransactionDto.OutcomingEntryId.Value);

            if (outcomgingEntryMoneyInfo.Avalible < totalMoney)
            {
                throw new UserFriendlyException($"<div class='text-left'>Tổng tiền biến động số dư: <strong>{Helpers.FormatMoney(totalMoney)}</strong>" +
                    $"</br>lớn hơn số tiền chi khả dụng là: " +
                    $"<strong> {Helpers.FormatMoney(outcomgingEntryMoneyInfo.Avalible)}</strong> " +
                    $"<i class='fa fa-question-circle' " +
                    $"data-toggle = 'tooltip' data-placement = 'right' title = 'Số tiền cần chi: {Helpers.FormatMoney(outcomgingEntryMoneyInfo.NeedToSpend)}, " +
                    $"số tiền đã chi: {Helpers.FormatMoney(-outcomgingEntryMoneyInfo.Spent)}'></i>" +
                    $"</div>");
            }
            return true;

        }

        public async Task<bool> CheckMuaNgoaiTe(ConversionTransactionDto conversionTransactionDto)
        {
            if (!conversionTransactionDto.OutcomingEntryId.HasValue)
            {
                throw new UserFriendlyException("Không có yêu cầu chi");
            }
            if (!conversionTransactionDto.ToBankAccountId.HasValue)
            {
                throw new UserFriendlyException("Không có tài khoản ngân hàng NHẬN");
            }
            if (!conversionTransactionDto.FromBankAccountId.HasValue)
            {
                throw new UserFriendlyException("Không có tài khoản ngân hàng GỬI");
            }
            if (!conversionTransactionDto.IncomingEntryTypeId.HasValue)
            {
                throw new UserFriendlyException("Không có loại thu");
            }
            if (conversionTransactionDto.MinusBTransactionIds == null || conversionTransactionDto.MinusBTransactionIds.IsNullOrEmpty())
            {
                throw new UserFriendlyException("Không có biến động số dư âm");
            }
            if (conversionTransactionDto.PlusBTransactionIds == null || conversionTransactionDto.PlusBTransactionIds.IsNullOrEmpty())
            {
                throw new UserFriendlyException("Không có biến động số dư dương");
            }
            if (await IsAllowOutcomingEntryByMultipleCurrency())
            {
                throw new UserFriendlyException("Tính năng không còn được hỗ trợ");
            }

            var incomingEntryType = _ws.GetAll<IncomingEntryType>()
                .Where(s => s.Id == conversionTransactionDto.IncomingEntryTypeId)
                .FirstOrDefault();

            if (incomingEntryType == default)
            {
                throw new UserFriendlyException($"Không tìm thấy loại thu Id = {conversionTransactionDto.IncomingEntryTypeId}");
            }

            var bankAccount = _ws.GetAll<BankAccount>()
                .Select(s => new
                {
                    s.Id,
                    s.Account.Type,
                    s.Currency.Name,
                    s.Currency.Code,
                    s.CurrencyId
                })
                .Where(s => s.Id == conversionTransactionDto.ToBankAccountId || s.Id == conversionTransactionDto.FromBankAccountId)
                .ToList();

            var fromBankAccount = bankAccount.Where(s => s.Id == conversionTransactionDto.FromBankAccountId).FirstOrDefault();
            var toBankAccount = bankAccount.Where(s => s.Id == conversionTransactionDto.ToBankAccountId).FirstOrDefault();

            if (fromBankAccount == default)
            {
                throw new UserFriendlyException($"Không tồn tại tài khoản ngân hàng gửi Id = {conversionTransactionDto.FromBankAccountId}");
            }
            if (toBankAccount == default)
            {
                throw new UserFriendlyException($"Không tồn tại tài khoản ngân hàng nhận Id = {conversionTransactionDto.ToBankAccountId}");
            }

            if (fromBankAccount.Type == AccountTypeEnum.COMPANY)
            {
                throw new UserFriendlyException($"Không thể bán ngoại tệ với tài khoản ngân hàng GỬI là tài khoản thuộc công ty");
            }
            if (toBankAccount.Type == AccountTypeEnum.COMPANY)
            {
                throw new UserFriendlyException($"Không thể bán ngoại tệ với tài khoản ngân hàng NHẬN là tài khoản thuộc công ty");
            }

            var bTransactions = _ws.GetAll<BTransaction>()
                .Where(s => conversionTransactionDto.BTransactionIds.Contains(s.Id))
                .Select(s => new
                {
                    s.Id,
                    s.Money,
                    s.BankAccount.CurrencyId,
                    CurrencyName = s.BankAccount.Currency.Name,
                    s.Status
                })
                .ToList();

            var doneBTransactionIds = bTransactions.Where(s => s.Status == BTransactionStatus.DONE).Select(s => s.Id.ToString()).ToList();
            var bTransactionCanNotFind = conversionTransactionDto.BTransactionIds.Where(s => !bTransactions.Select(x => x.Id).Contains(s)).ToList();

            if (!bTransactionCanNotFind.IsEmpty())
            {
                throw new UserFriendlyException($"Không thể thực hiện thao tác vì không tìm thấy các biến động số dư có Id: {string.Join(",", bTransactionCanNotFind)}");
            }

            if (!doneBTransactionIds.IsEmpty())
            {
                throw new UserFriendlyException($"Không thể thực hiện thao tác vì có các biến động số dư đã DONE: {string.Join(",", doneBTransactionIds)}");
            }

            var MinusBTransaction = bTransactions.Where(s => conversionTransactionDto.MinusBTransactionIds.Contains(s.Id)).ToList();
            var PlusBTransaction = bTransactions.Where(s => conversionTransactionDto.PlusBTransactionIds.Contains(s.Id)).ToList();

            var currencyPlusBTransaction = PlusBTransaction.Select(s => s.CurrencyId).Distinct();

            if (currencyPlusBTransaction.Count() > 1)
            {
                throw new UserFriendlyException("Các biến động số dư dương phải có cùng loại tiền");
            }
            var currencyDefault = await GetCurrencyDefaultAsync();
            if (currencyDefault.IsNullOrDefault())
                throw new UserFriendlyException("Bạn cần xét 1 loại tiền làm loại tiền mặc định cho Request chi");

            var currencyOtherCurrencyDefaultMinusBTransaction = MinusBTransaction.Where(s => s.CurrencyId != currencyDefault.Id);

            if (!currencyOtherCurrencyDefaultMinusBTransaction.IsEmpty())
            {
                throw new UserFriendlyException($"Các biến động số âm phải có cùng loại tiền {currencyDefault.Name}");
            }

            if (toBankAccount.CurrencyId != MinusBTransaction.FirstOrDefault().CurrencyId)
            {
                throw new UserFriendlyException($"Không thể mua ngoại tệ với tài khoản ngân hàng NHẬN có loại tiền khác" + MinusBTransaction.FirstOrDefault().CurrencyName);
            }
            if (fromBankAccount.CurrencyId != PlusBTransaction.FirstOrDefault().CurrencyId)
            {
                throw new UserFriendlyException($"Không thể mua ngoại tệ với tài khoản ngân hàng GỬI trả có loại tiền khác" + PlusBTransaction.FirstOrDefault().CurrencyName);
            }

            var totalMoneyMinusBTransaction = Math.Abs(MinusBTransaction.Sum(s => s.Money));
            var outcomgingEntryMoneyInfo = await _outcomingEntryManager.GetOutcomingEntryMoneyInfo(conversionTransactionDto.OutcomingEntryId.Value);

            if (outcomgingEntryMoneyInfo.Avalible < totalMoneyMinusBTransaction)
            {
                throw new UserFriendlyException($"<div class='text-left'>Tổng tiền biến động số dư: <strong>{Helpers.FormatMoney(totalMoneyMinusBTransaction)}</strong>" +
                    $"</br>lớn hơn số tiền chi khả dụng là: " +
                    $"<strong> {Helpers.FormatMoney(outcomgingEntryMoneyInfo.Avalible)}</strong> " +
                    $"<i class='fa fa-question-circle' " +
                    $"data-toggle = 'tooltip' data-placement = 'right' title = 'Số tiền cần chi: {Helpers.FormatMoney(outcomgingEntryMoneyInfo.NeedToSpend)}, " +
                    $"số tiền đã chi: {Helpers.FormatMoney(-outcomgingEntryMoneyInfo.Spent)}'></i>" +
                    $"</div>");
            }
            return true;

        }
        public async Task CheckCurrencyBTransactionWithOutCome(long bTransactionId, long outcomingEntryId)
        {
            var bTransactionCurrency = await _ws.GetAll<BTransaction>().Where(s => s.Id == bTransactionId).Select(s => s.BankAccount.CurrencyId).FirstOrDefaultAsync();
            var outcomingEntryCurrency = await _ws.GetAll<OutcomingEntry>().Where(s => s.Id == outcomingEntryId).Select(s => s.CurrencyId).FirstOrDefaultAsync();

            if (bTransactionCurrency != outcomingEntryCurrency)
            {
                throw new UserFriendlyException("Tiền của biến động số dư khác tiền của request chi");
            }
        }
        public async Task CheckCurrencyBTransactionWithBankAccount(long bTransactionId, long bankAccountId)
        {
            var bTransactionCurrency = await _ws.GetAll<BTransaction>().Where(s => s.Id == bTransactionId).Select(s => s.BankAccount.CurrencyId).FirstOrDefaultAsync();
            var bankAccountCurrency = await _ws.GetAll<BankAccount>().Where(s => s.Id == bankAccountId).Select(s => s.CurrencyId).FirstOrDefaultAsync();

            if (bTransactionCurrency != bankAccountCurrency)
            {
                throw new UserFriendlyException("Tiền của biến động số dư khác tiền của tài khoản ngân hàng");
            }
        }

        public async Task<bool> CheckChiChuyenDoi(ChiChuyenDoiDto chiChuyenDoidto)
        {
            var incomingEntryType = _ws.GetAll<IncomingEntryType>()
                .Where(s => s.Id == chiChuyenDoidto.IncomingEntryTypeId)
                .FirstOrDefault();

            if (incomingEntryType == default)
            {
                throw new UserFriendlyException($"Không tìm thấy loại thu Id = {chiChuyenDoidto.IncomingEntryTypeId}");
            }

            var bankAccount = _ws.GetAll<BankAccount>()
                .Select(s => new
                {
                    s.Id,
                    s.Account.Type,
                    s.Currency.Name,
                    s.Currency.Code,
                })
                .Where(s => s.Id == chiChuyenDoidto.ToBankAccountId || s.Id == chiChuyenDoidto.FromBankAccountId)
                .ToList();

            var fromBankAccount = bankAccount.Where(s => s.Id == chiChuyenDoidto.FromBankAccountId).FirstOrDefault();
            var toBankAccount = bankAccount.Where(s => s.Id == chiChuyenDoidto.ToBankAccountId).FirstOrDefault();

            if (fromBankAccount == default)
            {
                throw new UserFriendlyException($"Không tồn tại tài khoản ngân hàng gửi Id = {chiChuyenDoidto.FromBankAccountId}");
            }
            if (toBankAccount == default)
            {
                throw new UserFriendlyException($"Không tồn tại tài khoản ngân hàng nhận Id = {chiChuyenDoidto.ToBankAccountId}");
            }

            if (fromBankAccount.Type == AccountTypeEnum.COMPANY)
            {
                throw new UserFriendlyException($"Không thể chi chuyển đổi với tài khoản ngân hàng GỬI là tài khoản thuộc công ty");
            }
            if (toBankAccount.Type == AccountTypeEnum.COMPANY)
            {
                throw new UserFriendlyException($"Không thể chi chuyển đổi với tài khoản ngân hàng NHẬN là tài khoản thuộc công ty");
            }
            var bTransactions = _ws.GetAll<BTransaction>()
                .Where(s => chiChuyenDoidto.BTransactionIds.Contains(s.Id))
                .Select(s => new
                {
                    s.Id,
                    s.Money,
                    s.BankAccount.CurrencyId,
                    s.Status
                })
                .ToList();

            var doneBTransactionIds = bTransactions.Where(s => s.Status == BTransactionStatus.DONE).Select(s => s.Id.ToString()).ToList();
            var bTransactionCanNotFind = chiChuyenDoidto.BTransactionIds.Where(s => !bTransactions.Select(x => x.Id).Contains(s)).ToList();

            if (!bTransactionCanNotFind.IsEmpty())
            {
                throw new UserFriendlyException($"Không thể thực hiện thao tác vì không tìm thấy các biến động số dư có Id: {string.Join(",", bTransactionCanNotFind)}");
            }

            if (!doneBTransactionIds.IsEmpty())
            {
                throw new UserFriendlyException($"Không thể thực hiện thao tác vì có các biến động số dư đã DONE: {string.Join(",", doneBTransactionIds)}");
            }

            var MinusBTransaction = bTransactions.Where(s => chiChuyenDoidto.MinusBTransactionIds.Contains(s.Id)).ToList();
            var PlusBTransaction = bTransactions.Where(s => chiChuyenDoidto.PlusBTransactionIds.Contains(s.Id)).ToList();

            var currencyPlusBTransaction = PlusBTransaction.Select(s => s.CurrencyId).Distinct();
            var currencyMinusBTransaction = MinusBTransaction.Select(s => s.CurrencyId).Distinct();
            if (currencyPlusBTransaction.Count() > 1)
            {
                throw new UserFriendlyException("Các biến động số dư DƯƠNG phải có cùng loại tiền");
            }
            if (currencyMinusBTransaction.Count() > 1)
            {
                throw new UserFriendlyException("Các biến động số dư ÂM phải có cùng loại tiền");
            }
            await CheckCurrencyBTransactionWithOutCome(chiChuyenDoidto.MinusBTransactionIds[0], chiChuyenDoidto.OutcomingEntryId.Value);
            await CheckCurrencyBTransactionWithBankAccount(chiChuyenDoidto.MinusBTransactionIds[0], chiChuyenDoidto.ToBankAccountId.Value);
            await CheckCurrencyBTransactionWithBankAccount(chiChuyenDoidto.PlusBTransactionIds[0], chiChuyenDoidto.FromBankAccountId.Value);

            var totalMoneyMinusBTransaction = Math.Abs(MinusBTransaction.Sum(s => s.Money));
            var outcomgingEntryMoneyInfo = await _outcomingEntryManager.GetOutcomingEntryMoneyInfo(chiChuyenDoidto.OutcomingEntryId.Value);

            if (outcomgingEntryMoneyInfo.Avalible < totalMoneyMinusBTransaction)
            {
                throw new UserFriendlyException($"<div class='text-left'>Tổng tiền biến động số dư: <strong>{Helpers.FormatMoney(totalMoneyMinusBTransaction)}</strong>" +
                    $"</br>lớn hơn số tiền chi khả dụng là: " +
                    $"<strong> {Helpers.FormatMoney(outcomgingEntryMoneyInfo.Avalible)}</strong> " +
                    $"<i class='fa fa-question-circle' " +
                    $"data-toggle = 'tooltip' data-placement = 'right' title = 'Số tiền cần chi: {Helpers.FormatMoney(outcomgingEntryMoneyInfo.NeedToSpend)}, " +
                    $"số tiền đã chi: {Helpers.FormatMoney(-outcomgingEntryMoneyInfo.Spent)}'></i>" +
                    $"</div>");
            }
            return true;

        }

        public async Task<GetInfoRollbackClientPaidDto> GetInfoRollbackClientPaid(long bTransactionId)
        {
            var qBankAccInfo = _ws.GetAll<BankAccount>().Select(x => x);

            var query = from b in _ws.GetAll<BTransaction>().Where(b => b.Id == bTransactionId)
                        join bt in _ws.GetAll<BankTransaction>() on b.Id equals bt.BTransactionId
                        select new GetInfoRollbackClientPaidDto
                        {
                            BTransactionInfor = new BTransactionInforDto
                            {
                                BTransactionId = b.Id,
                                BankAccountId = b.BankAccountId,
                                BankAccountName = b.BankAccount.HolderName,
                                BankNumber = b.BankAccount.BankNumber,
                                Note = b.Note,
                                TimeAt = b.TimeAt,
                                Money = b.Money,
                                CurrencyName = b.BankAccount.Currency.Name,
                                CurrencyId = b.BankAccount.CurrencyId,
                                Status = b.Status
                            },
                            BankTransactionInfor = new BankTransactionInforDto
                            {
                                BankTransactionId = bt.Id,
                                BankTransactionName = bt.Name,
                                FromBankAccountId = bt.FromBankAccountId,
                                FromBankAccountName = qBankAccInfo.Where(ba => ba.Id == bt.FromBankAccountId).FirstOrDefault().HolderName,
                                FromValue = bt.FromValue,
                                FromCurrencyId = qBankAccInfo.Where(ba => ba.Id == bt.FromBankAccountId).FirstOrDefault().CurrencyId,
                                FromCurrencyName = qBankAccInfo.Where(ba => ba.Id == bt.FromBankAccountId).FirstOrDefault().Currency.Name,
                                ToBankAccountId = bt.ToBankAccountId,
                                ToBankAccountName = qBankAccInfo.Where(ba => ba.Id == bt.ToBankAccountId).FirstOrDefault().HolderName,
                                ToCurrencyId = qBankAccInfo.Where(ba => ba.Id == bt.ToBankAccountId).FirstOrDefault().CurrencyId,
                                ToCurrencyName = qBankAccInfo.Where(ba => ba.Id == bt.ToBankAccountId).FirstOrDefault().Currency.Name,
                                ToValue = bt.ToValue,
                                Fee = bt.Fee,
                            },
                            IncomingEntrieInfors = bt.IncomingEntries.Join(_ws.GetAll<Invoice>(), inc => inc.InvoiceId, inv => inv.Id, (inc, inv) => new IncomingEntryInforDto
                            {
                                IncomingEntryId = inc.Id,
                                IncomingEntryName = inc.Name,
                                Money = inc.ExchangeRate == null ? inc.Value : inc.Value * inc.ExchangeRate.Value,
                                CurrencyId = inc.CurrencyId,
                                CurrencyName = inc.Currency.Name,
                                ExchangeRate = inc.ExchangeRate,
                                BankTransactionId = inc.BankTransactionId,
                                InvoiceId = inc.InvoiceId,
                                InvoiceName = inv.NameInvoice,
                                InvoiceDateMonth = inv.Month,
                                InvoiceDateYear = inv.Year,
                                InvoiceCurrencyName = inv.Currency.Name,
                                InvoiceStatus = inv.Status,
                            }).ToList()
                        };

            var result = await query.FirstOrDefaultAsync();
            if (result == null)
                throw new UserFriendlyException($"Không tìm thấy Biến động số dư {bTransactionId}");
            if (result.BankTransactionInfor == null)
                return result;

  

            return result;
        }

        public async Task RollbackClientPaid(long bTransactionId)
        {
            // get btransaction to update status, fromaccountId
            var bTransaction = await _ws.GetAll<BTransaction>().Where(b => b.Id == bTransactionId).FirstOrDefaultAsync();

            if (bTransaction == null)
                throw new UserFriendlyException(string.Format("BTransaction Id {0} is not fould", bTransactionId));

            if (bTransaction.Status == BTransactionStatus.PENDING)
                throw new UserFriendlyException(string.Format("BTransaction Id {0} is Pending", bTransactionId));

            // get banktransaction to delete
            var bankTransaction = await _ws.GetAll<BankTransaction>().Where(bnt => bnt.BTransactionId == bTransactionId).FirstOrDefaultAsync();

            var invoiceOfAccountHasIncom = _ws.GetAll<Invoice>()
                .Where(inv => inv.AccountId == bTransaction.FromAccountId && inv.IncomingEntries.Count() > 0)
                .Include(inv => inv.IncomingEntries)
                .ToList();

            foreach (var invoice in invoiceOfAccountHasIncom)
            {
                // delete incom in invoice 
                var incomInInvoice = invoice.IncomingEntries.Where(inv => inv.BTransactionId == bTransaction.Id).ToList(); // get incom by btransactionid
                incomInInvoice.ForEach(async inc =>
                {
                    var countIncom = invoice.IncomingEntries.Count();

                    inc.IsDeleted = true;
                    await _ws.UpdateAsync(inc);

                    if (countIncom < 2)
                    {
                        invoice.Status = NInvoiceStatus.CHUA_TRA;
                    }
                    else
                    {
                        invoice.Status = NInvoiceStatus.TRA_1_PHAN;
                    }
                    await _ws.UpdateAsync(invoice);
                });
            }

            // delete banktransaction
            bankTransaction.IsDeleted = true;
            await _ws.UpdateAsync(bankTransaction);

            // update fromAccountId and Status of BTransaction
            bTransaction.FromAccountId = null;
            bTransaction.Status = BTransactionStatus.PENDING;
            await _ws.UpdateAsync(bTransaction);

            // Update database
            await CurrentUnitOfWork.SaveChangesAsync();
        }
    }
}
