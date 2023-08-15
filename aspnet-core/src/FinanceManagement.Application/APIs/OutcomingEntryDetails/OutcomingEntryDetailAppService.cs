using Abp.Authorization;
using Abp.UI;
using FinanceManagement.APIs.BankTransactions.Dto;
using FinanceManagement.APIs.OutcomingEntryBankTransactions.Dto;
using FinanceManagement.APIs.OutcomingEntryDetails.Dto;
using FinanceManagement.Authorization;
using FinanceManagement.Entities;
using FinanceManagement.Enums;
using FinanceManagement.Extension;
using FinanceManagement.Helper;
using FinanceManagement.IoC;
using FinanceManagement.Managers.OutcomingEntries;
using FinanceManagement.Managers.OutcomingEntries.Dtos;
using FinanceManagement.Managers.TempOutcomingEntries.Dtos;
using FinanceManagement.Paging;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceManagement.APIs.OutcomingEntryDetails
{
    [AbpAuthorize]
    public class OutcomingEntryDetailAppService : FinanceManagementAppServiceBase
    {
        private readonly IOutcomingEntryManager _outcomgingEntryManager;
        private readonly IWebHostEnvironment _env;
        public OutcomingEntryDetailAppService(
            IWorkScope workScope, 
            IOutcomingEntryManager outcomingEntryManager,
            IWebHostEnvironment env
        ) : base(workScope)
        {
            _outcomgingEntryManager = outcomingEntryManager;
            _env = env;
        }

        [HttpPost]
        [AbpAuthorize(PermissionNames.Finance_OutcomingEntry_OutcomingEntryDetail_TabDetailInfo_Create)]
        public async Task<GetOutcomingEntryDetailDto> Create(GetOutcomingEntryDetailDto input)
        {
            var outcomingEntry = await WorkScope.GetAll<OutcomingEntry>()
                .Include(s => s.WorkflowStatus)
                .Where(s => s.Id == input.OutcomingEntryId)
                .FirstOrDefaultAsync();
            if (outcomingEntry?.WorkflowStatus?.Code != Constants.WORKFLOW_STATUS_START)
            {
                throw new UserFriendlyException("Can only create OutcomingEntryDetail under new OutcomingEntry");
            }
            input.Id = await WorkScope.InsertAndGetIdAsync(ObjectMapper.Map<OutcomingEntryDetail>(input));

            outcomingEntry.Value = outcomingEntry.OutcomingEntryDetails.Sum(s => s.Total);
            await WorkScope.UpdateAsync(outcomingEntry);
            return input;
        }

        [HttpPut]
        [AbpAuthorize(PermissionNames.Finance_OutcomingEntry_OutcomingEntryDetail_TabDetailInfo_Edit)]
        public async Task<GetOutcomingEntryDetailDto> Update(GetOutcomingEntryDetailDto input)
        {
            var outcomingEntry = await WorkScope.GetAsync<OutcomingEntry>(input.OutcomingEntryId);
            if (outcomingEntry.WorkflowStatus.Code != Constants.WORKFLOW_STATUS_START)
            {
                throw new UserFriendlyException("Can only create OutcomingEntryDetail under new OutcomingEntry");
            }

            var outcomingEntryDetail = await WorkScope.GetAsync<OutcomingEntryDetail>(input.Id);
            await WorkScope.UpdateAsync(ObjectMapper.Map(input, outcomingEntryDetail));
            await CurrentUnitOfWork.SaveChangesAsync();

            outcomingEntry.Value = outcomingEntry.OutcomingEntryDetails.Sum(x => x.Total);
            return input;
        }

        [HttpPost]
        public async Task<GetOutcomingEntryDetailDto> ChangeDone(GetOutcomingEntryDetailDto input)
        {
            var outcomingEntryDetail = await WorkScope.GetAsync<OutcomingEntryDetail>(input.Id);
            outcomingEntryDetail.IsNotDone = input.IsNotDone;
            await CurrentUnitOfWork.SaveChangesAsync();
            return input;
        }

        [HttpPost]
        [AbpAuthorize(PermissionNames.Finance_OutcomingEntry_OutcomingEntryDetail_TabDetailInfo_ActiveDeactive)]
        public async Task ChangeStatus(GetOutcomingEntryDetailDto input)
        {
            var outcomingEntryDetail = await WorkScope.GetAsync<OutcomingEntryDetail>(input.Id);
            outcomingEntryDetail.IsNotDone = input.IsNotDone;
            await WorkScope.UpdateAsync(outcomingEntryDetail);

        }
        private IQueryable<GetOutcomingEntryDetailDto> GetAllByOutcomingEntry(long outcomingEntryId)
        {
            var query =
                (from oe in WorkScope.GetAll<OutcomingEntryDetail>().Where(oed => oed.OutcomingEntryId == outcomingEntryId)
                 join account in WorkScope.GetAll<Account>() on oe.AccountId equals account.Id into accounts
                 from acc in accounts.DefaultIfEmpty()
                 select new GetOutcomingEntryDetailDto
                 {
                     Id = oe.Id,
                     OutcomingEntryId = oe.OutcomingEntryId,
                     AccountId = oe.AccountId,
                     AccountName = acc.Name,
                     Name = oe.Name,
                     Quantity = oe.Quantity,
                     UnitPrice = oe.UnitPrice,
                     IsNotDone = oe.IsNotDone,
                     Total = oe.Total,
                     BranchId = oe.BranchId,
                     BranchName = oe.Branch.Name ?? string.Empty,
                 });

            return query;
        }

        [HttpPost]
        [AbpAuthorize(PermissionNames.Finance_OutcomingEntry_OutcomingEntryDetail_TabDetailInfo)]
        public async Task<ResultGetOutcomingEntryDetailDto> GetAllPaging(OutcomingEntryDetailFilterDto input)
        {
            var query = GetAllByOutcomingEntry(input.OutcomingEntryId).FilterByOutcomingEntryDetailFilterDto(input);
            return new ResultGetOutcomingEntryDetailDto
            {
                Paging = await query.GetGridResult(query, input.param),
                TotalMoney = await query.ApplySearchAndFilter(input.param).SumAsync(s => s.Total)
            };
        }

        public async Task<List<GetOutcomingEntryDetailDto>> GetAll(long outcomingEntryId)
        {
            return await GetAllByOutcomingEntry(outcomingEntryId).ToListAsync();
        }

        [HttpPost]
        [AbpAuthorize(PermissionNames.Finance_OutcomingEntry_OutcomingEntryDetail_TabDetailInfo_Delete)]
        public async Task Delete(DeleteOutcomingEntryDetailDto input)
        {
            var oeDetail = await WorkScope.GetAsync<OutcomingEntryDetail>(input.Id);

            var outcomingEntry = await WorkScope.GetAsync<OutcomingEntry>(oeDetail.OutcomingEntryId);
            if (outcomingEntry.WorkflowStatus.Code != Constants.WORKFLOW_STATUS_START)
            {
                throw new UserFriendlyException("Can only create OutcomingEntryDetail under new OutcomingEntry");
            }
            await WorkScope.DeleteAsync<OutcomingEntryDetail>(input.Id);

            outcomingEntry.Value -= oeDetail.Total;
            await WorkScope.UpdateAsync(outcomingEntry);
        }

        [HttpGet]
        public async Task<GetOutcomingEntryDetailDto> Get(long id)
        {
            var oeDetail = await WorkScope.GetAsync<OutcomingEntryDetail>(id);
            var oe = await WorkScope.GetAsync<OutcomingEntry>(oeDetail.OutcomingEntryId);
            var type = await WorkScope.GetAsync<OutcomingEntryType>(oe.OutcomingEntryTypeId);

            var linkedAccount = oeDetail.AccountId != null ? await WorkScope.GetAsync<Account>((long)oeDetail.AccountId) : null;

            return new GetOutcomingEntryDetailDto
            {
                Id = oeDetail.Id,
                OutcomingEntryId = oeDetail.OutcomingEntryId,
                AccountId = oeDetail.AccountId,
                AccountName = linkedAccount.Name,
                Name = oeDetail.Name,
                Quantity = oeDetail.Quantity,
                UnitPrice = oeDetail.UnitPrice,
                Total = oeDetail.Total,
                OutcomingEntryTypeCode = type.Code
            };
        }

        [HttpPost]
        [AbpAuthorize(PermissionNames.Finance_OutcomingEntry_OutcomingEntryDetail_TabRelatedBankTransaction_LinkToBTrans)]
        public async Task<OutcomingEntryBankTransactionDto> LinkToExistingTransaction(OutcomingEntryBankTransactionDto input)
        {
            var outcomingEntry = await WorkScope.GetAsync<OutcomingEntry>(input.OutcomingEntryId);
            if (outcomingEntry == null)
                throw new UserFriendlyException(String.Format($"OutcomingEntry with id = {input.Id} is not exist"));

            var isExist = await WorkScope.GetAll<OutcomingEntryBankTransaction>().AnyAsync(x => x.OutcomingEntryId == input.OutcomingEntryId && x.BankTransactionId == input.BankTransactionId);

            if (isExist)
            {
                throw new UserFriendlyException("Link to transaction already exists");
            }

            //Nếu outcoming thuộc dạng chuyển đổi thì auto link banktransaction với icomingEntry được liên kết với OutcomingEntry qua bảng RelationInOutEntry
            var outcomingEntryType = await WorkScope.GetAsync<OutcomingEntryType>(outcomingEntry.OutcomingEntryTypeId);

            await WorkScope.InsertAndGetIdAsync(ObjectMapper.Map<OutcomingEntryBankTransaction>(input));

            return input;
        }

        [HttpPost]
        // OE status = New
        public async Task<BankTransactionDto> AddBankTransaction(BankTransactionDto input, long outcomingEntryId)
        {
            var outcomingEntry = await WorkScope.GetAsync<OutcomingEntry>(outcomingEntryId);

            input.Id = await WorkScope.InsertAndGetIdAsync(ObjectMapper.Map<BankTransaction>(input));

            // Link newly created bank transaction to OutcomingEntry
            var oebtInput = new OutcomingEntryBankTransactionDto()
            {
                BankTransactionId = input.Id,
                OutcomingEntryId = outcomingEntry.Id,
            };

            var bankFromValue = await WorkScope.GetAsync<BankAccount>(input.FromBankAccountId);
            bankFromValue.Amount = bankFromValue.Amount - input.FromValue;
            await WorkScope.GetRepo<BankAccount, long>().UpdateAsync(bankFromValue);

            var bankToValue = await WorkScope.GetAsync<BankAccount>(input.ToBankAccountId);
            bankToValue.Amount = bankToValue.Amount + input.ToValue;
            await WorkScope.GetRepo<BankAccount, long>().UpdateAsync(bankToValue);

            if (input.Fee > 0)
            {
                var toAccount = await WorkScope.GetAsync<Account>(bankToValue.AccountId);
                var toAccountType = await WorkScope.GetAsync<AccountType>(toAccount.AccountTypeId);
                var toBankFEE = toAccountType.Code == Constants.ACCOUNT_TYPE_COMPANY;

                var fromAccount = await WorkScope.GetAsync<Account>(bankFromValue.AccountId);
                var FromAccountType = await WorkScope.GetAsync<AccountType>(fromAccount.AccountTypeId);
                var fromBankFEE = FromAccountType.Code == Constants.ACCOUNT_TYPE_COMPANY;

                var bankAccountFEE = await WorkScope.GetAll<BankAccount>().Where(x => x.HolderName == Constants.BANKACCOUNT_TRANSFER_FEE).FirstOrDefaultAsync();

                if ((fromBankFEE && !toBankFEE) || (fromBankFEE && toBankFEE))
                {
                    var transferFee = new BankTransaction
                    {
                        Name = $"Tính phí giao dịch của giao dịch: {input.Id}-{input.Name}",
                        FromBankAccountId = input.FromBankAccountId,
                        ToBankAccountId = bankAccountFEE.Id,
                        FromValue = input.Fee ?? 0,
                        ToValue = input.Fee ?? 0,
                        Fee = 0,
                        TransactionDate = input.TransactionDate,
                    };
                    await WorkScope.InsertAndGetIdAsync(transferFee);

                    bankFromValue.Amount -= input.Fee ?? 0;
                    await WorkScope.UpdateAsync<BankAccount>(bankFromValue);

                    bankAccountFEE.Amount += input.Fee ?? 0;
                    await WorkScope.UpdateAsync(bankAccountFEE);
                }
                else if (toBankFEE && !fromBankFEE)
                {
                    var transferFee = new BankTransaction
                    {
                        Name = $"Tính phí giao dịch của giao dịch: {input.Id}-{input.Name}",
                        FromBankAccountId = input.ToBankAccountId,
                        ToBankAccountId = bankAccountFEE.Id,
                        FromValue = input.Fee ?? 0,
                        ToValue = input.Fee ?? 0,
                        Fee = 0,
                        TransactionDate = input.TransactionDate
                    };
                    await WorkScope.InsertAndGetIdAsync(transferFee);

                    bankToValue.Amount -= input.Fee ?? 0;
                    await WorkScope.UpdateAsync<BankAccount>(bankToValue);

                    bankAccountFEE.Amount += input.Fee ?? 0;
                    await WorkScope.UpdateAsync(bankAccountFEE);
                }
            }

            await LinkToExistingTransaction(oebtInput);

            return input;
        }


        [HttpPost]
        // OE Status = Approved
        public async Task AddMultipleTransactions(long outcomingEntryId, CreateTransactionsDto input)
        {
            var outcomingEntry = await WorkScope.GetAsync<OutcomingEntry>(outcomingEntryId);
            var workflowStatus = await WorkScope.GetAsync<WorkflowStatus>(outcomingEntry.WorkflowStatusId);

            if (workflowStatus?.Code != Constants.WORKFLOW_STATUS_END)
            {
                throw new UserFriendlyException("Outcoming entry is not available for creating multiple transactions");
            }

            var bankAccounts = WorkScope.GetAll<BankAccount>();

            // Insert multiple transactions base on Outcoming entry detail records
            var bankTransactionsToSave =
                await WorkScope.GetAll<OutcomingEntryDetail>().Where(oed => oed.OutcomingEntryId == outcomingEntryId)
                    .Select(oeDetail => new BankTransactionDto
                    {
                        FromBankAccountId = input.FromBankAccountId,
                        ToBankAccountId = bankAccounts.FirstOrDefault(ba => ba.AccountId == oeDetail.AccountId).Id,
                        FromValue = oeDetail.Total,
                        ToValue = oeDetail.Total,
                        Fee = 0,
                        Note = input.Note,
                        TransactionDate = input.TransactionDate,
                    }).ToListAsync();

            var convertedBankTransactions = bankTransactionsToSave.ConvertAll(q => new BankTransaction
            {
                Name = input.Name,
                FromBankAccountId = q.FromBankAccountId,
                ToBankAccountId = q.ToBankAccountId,
                FromValue = q.FromValue,
                ToValue = q.ToValue,
                Fee = 0,
                Note = input.Note,
                TransactionDate = input.TransactionDate,
            });

            foreach (var item in bankTransactionsToSave)
            {
                var bankFromValue = await WorkScope.GetAsync<BankAccount>(item.FromBankAccountId);
                bankFromValue.Amount = bankFromValue.Amount - item.FromValue;
                await WorkScope.GetRepo<BankAccount, long>().UpdateAsync(bankFromValue);

                var bankToValue = await WorkScope.GetAsync<BankAccount>(item.FromBankAccountId);
                bankToValue.Amount = bankToValue.Amount + item.ToValue;
                await WorkScope.GetRepo<BankAccount, long>().UpdateAsync(bankToValue);
            }

            await WorkScope.InsertRangeAsync(convertedBankTransactions);
            CurrentUnitOfWork.SaveChanges();

            // Insert newly created transation ids and outcomingEntryId to OutcomingEntryBankTransaction table
            var bankTransactionIds = convertedBankTransactions.Select(cbt => cbt.Id).ToList();
            var outcomingEntryBankTransactions = bankTransactionIds.ConvertAll(btId => new OutcomingEntryBankTransaction
            {
                BankTransactionId = btId,
                OutcomingEntryId = outcomingEntryId,
            });

            await WorkScope.InsertRangeAsync(outcomingEntryBankTransactions);
        }

        [HttpPost]
        [AbpAuthorize(PermissionNames.Finance_OutcomingEntry_OutcomingEntryDetail_TabRelatedBankTransaction)]
        //OE Status = Approved
        public async Task<GridResultOutcomingEntryDetail<TransactionDetailsDto>> GetTransactionDetails(OutcomingEntryDetailFilterDto input)
        {
            var outcomingEntry = await WorkScope.GetAsync<OutcomingEntry>(input.OutcomingEntryId);

            var transactionInfo =
                (from oebt in WorkScope.GetAll<OutcomingEntryBankTransaction>().Where(re => re.OutcomingEntryId == outcomingEntry.Id)
                 join bt in WorkScope.GetAll<BankTransaction>() on oebt.BankTransactionId equals bt.Id

                 select new
                 {
                     transactionId = bt.Id,
                     tDate = bt.TransactionDate,
                     fromBankAccountId = bt.FromBankAccountId,
                     toBankAccountId = bt.ToBankAccountId,
                     fromValue = bt.FromValue,
                     toValue = bt.ToValue,
                     fee = bt.Fee,
                     name = bt.Name,
                     CreateDate = bt.CreationTime,
                     BTransactionId = bt.BTransactionId,
                     BTransactionBankAccountHolderName = bt.BTransaction.BankAccount.HolderName,
                     BTransactionBankNumber = bt.BTransaction.BankAccount.BankNumber
                 }).ToList();

            var bankAccountInfo =
                (from ba in WorkScope.GetAll<BankAccount>().Include(ba => ba.Account)
                 select new
                 {
                     BankAccountId = ba.Id,
                     AccountName = ba.Account.Name,
                     BankName = ba.Bank.Name,
                     ba.HolderName,
                     CurrencyCode = ba.Currency.Code,

                 }).ToList();
            var currency = WorkScope.GetAll<Currency>();

            var query = transactionInfo.Select(q => new TransactionDetailsDto
            {
                TransactionId = q.transactionId,
                OutcomingEntryId = input.OutcomingEntryId,
                TransactionDate = q.tDate,
                FromBank = bankAccountInfo.FirstOrDefault(ba => ba.BankAccountId == q.fromBankAccountId)?.HolderName,
                FromBankCurrencyCode = bankAccountInfo.FirstOrDefault(ba => ba.BankAccountId == q.fromBankAccountId)?.CurrencyCode,
                ToBank = bankAccountInfo.FirstOrDefault(ba => ba.BankAccountId == q.toBankAccountId)?.HolderName,
                ToBankCurrencyCode = bankAccountInfo.FirstOrDefault(ba => ba.BankAccountId == q.toBankAccountId)?.CurrencyCode,
                AccountName = bankAccountInfo.FirstOrDefault(ba => ba.BankAccountId == q.toBankAccountId)?.AccountName,
                Name = q.name,
                FromValue = q.fromValue,
                ToValue = q.toValue,
                Fee = q.fee,
                CurrencyId = outcomingEntry.CurrencyId,
                CurrencyName = currency.FirstOrDefault(x => x.Id == outcomingEntry.CurrencyId)?.Name,
                CreateDate = $"{q.CreateDate}",
                BTransactionId = q.BTransactionId,
                BTransactionInfo = q.BTransactionId.HasValue ? $"#{q.BTransactionId} {q.BTransactionBankAccountHolderName} {q.BTransactionBankNumber}" : "",
            }).AsQueryable();

            var queryApplySearchAndFilter = query.ApplySearchAndFilter(input.param);

            var list = queryApplySearchAndFilter.TakePage(input.param).ToList();
            var totalMoney = queryApplySearchAndFilter.Sum(s => s.FromValue);
            var total = queryApplySearchAndFilter.Count();

            return new GridResultOutcomingEntryDetail<TransactionDetailsDto>(list, total, totalMoney);
            //return query.GetGridResultSync(query, input.param);
        }

        [HttpPost]
        [AbpAuthorize(PermissionNames.Finance_OutcomingEntry_OutcomingEntryDetail_TabRelatedBankTransaction_DeleteLinkToBTrans)]
        public async Task DeleteLinkedTransaction(OutcomingEntryBankTransactionDto input)
        {
            var outcomingEntry = await WorkScope.GetAsync<OutcomingEntry>(input.OutcomingEntryId);

            var linkedData = WorkScope.GetAll<OutcomingEntryBankTransaction>()
                        .FirstOrDefault(oebt => oebt.BankTransactionId == input.BankTransactionId && oebt.OutcomingEntryId == outcomingEntry.Id);

            //var outcomingEntryType = await WorkScope.GetAsync<OutcomingEntryType>(outcomingEntry.OutcomingEntryTypeId);
            //if (outcomingEntryType.Code == Constants.OUTCOMING_ENTRY_TYPE_CURRENCY_EXCHANGE ||
            //    outcomingEntryType.Code == Constants.OUTCOMING_ENTRY_TYPE_MONEY_TRANSFER)
            //{
            //    var incomingBybankTransaction = await WorkScope.GetAll<IncomingEntry>().Where(x => x.BankTransactionId == input.BankTransactionId).ToListAsync();

            //    var relationInOut = await WorkScope.GetAll<RelationInOutEntry>()
            //                            .Where(x => x.OutcomingEntryId == input.OutcomingEntryId && incomingBybankTransaction.Select(y => y.Id).Contains(x.IncomingEntryId))
            //                            .FirstOrDefaultAsync();

            //    if (relationInOut == null)
            //        throw new UserFriendlyException(String.Format("Incoming and Outcoming is not linked"));

            //    var incomingEntry = await WorkScope.GetAsync<IncomingEntry>(relationInOut.IncomingEntryId);
            //    incomingEntry.BankTransactionId = 0;
            //    incomingEntry.Value = 0;
            //    await WorkScope.UpdateAsync(incomingEntry);
            //}

            if (linkedData != null)
            {
                await WorkScope.DeleteAsync<OutcomingEntryBankTransaction>(linkedData.Id);
            }
        }

        [HttpGet]
        public bool IsRequestChiIncludedInCost(long id)
        {
            var isRequestChiIncludedInCost = WorkScope.GetAll<OutcomingEntry>()
                .Where(x => x.Id == id)
                .Select(x=> x.OutcomingEntryType.ExpenseType)
                .FirstOrDefault();

            if (isRequestChiIncludedInCost != default && isRequestChiIncludedInCost == ExpenseType.REAL_EXPENSE)
            {
                return true;
            }
            return false;
        }
        #region new logic
        [HttpPost]
        public async Task ChangeAllDetails(ChangeAllDetailOutcomingEntryDto input)
        {
            await _outcomgingEntryManager.ChangeAllOutcomingEntryDetail(input);
        }
        #endregion
        public async Task<byte[]> GetTemplateInputOutcomingEntryDetail()
        {
            var file = Helpers.GetInfoFileTemplate(new string[] { _env.WebRootPath, "Template_Input_RequestChi_Detail.xlsx" });
            var branches = await WorkScope.GetAll<Branch>()
                .Select(x => x.Name)
                .ToListAsync();
            using (ExcelPackage pck = new ExcelPackage(file))
            {
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                ExcelWorksheet worksheet = pck.Workbook.Worksheets[1];

                int rowIndex = 2;
                foreach (var item in branches)
                {
                    worksheet.Cells[rowIndex,1].Value = item;
                    rowIndex++;
                }
                using (var stream = new MemoryStream())
                {
                    pck.SaveAs(stream);
                    var content = stream.ToArray();
                    return content;
                }
            }
        }
        public async Task<object> ImportFileOutcomingEntryDetail([FromForm] ImportFileOutcomingEntryDetailDto input)
        {
            var outcomingEntry = await WorkScope.GetAsync<OutcomingEntry>(input.OutcomingEntryId);
            if(outcomingEntry.WorkflowStatus.Code.Trim() != FinanceManagementConsts.WORKFLOW_STATUS_START)
            {
                throw new UserFriendlyException("Không thể import khi Request chi có trạng thái khác [START]");
            }
            var dataFile = ReadDataFromFile(input.FileInput);
            var mapToEntity = await MapToOutcomingEntryDetail(dataFile, input.OutcomingEntryId);
            foreach(var item in mapToEntity )
            {
                await WorkScope.InsertAsync(item);
            }
            await CurrentUnitOfWork.SaveChangesAsync();

            outcomingEntry.Value = outcomingEntry.OutcomingEntryDetails.Sum(x => x.Total);
            await WorkScope.UpdateAsync(outcomingEntry);

            return new { Success = mapToEntity.Count, Fail = dataFile.Count - mapToEntity.Count};
        }
        private async Task<List<OutcomingEntryDetail>> MapToOutcomingEntryDetail(List<DataFileTemplateInputOutcomingEntryDetail> input, long outcomingEntryId)
        {
            var outcomingEntryDetails = new List<OutcomingEntryDetail>();
            var dicBranches = await WorkScope.GetAll<Branch>().Select(x => new {x.Name, x.Id}).ToDictionaryAsync(x => x.Name, x => x.Id);
            
            foreach(var dto in input)
            {
                var outcomingEntryDetail = new OutcomingEntryDetail();

                if (string.IsNullOrEmpty(dto.BranchName) || !dicBranches.ContainsKey(dto.BranchName)) continue;
                outcomingEntryDetail.BranchId = dicBranches[dto.BranchName];

                if (!string.IsNullOrEmpty(dto.Name))
                {
                    outcomingEntryDetail.Name = dto.Name;
                }
                else continue;

                if (double.TryParse(dto.Price, out double price))
                {
                    outcomingEntryDetail.UnitPrice = price;
                }
                else continue;

                if(int.TryParse(dto.Quantity, out int quantity))
                {
                    outcomingEntryDetail.Quantity = quantity;
                }
                else continue;


                outcomingEntryDetail.Total = outcomingEntryDetail.Quantity * outcomingEntryDetail.UnitPrice;
                outcomingEntryDetail.OutcomingEntryId = outcomingEntryId;
                outcomingEntryDetails.Add(outcomingEntryDetail);
            }
            return outcomingEntryDetails;
        }
        private List<DataFileTemplateInputOutcomingEntryDetail> ReadDataFromFile(IFormFile file)
        {
            var result = new List<DataFileTemplateInputOutcomingEntryDetail>();
            using (ExcelPackage pck = new ExcelPackage(file.OpenReadStream()))
            {
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                ExcelWorksheet worksheet = pck.Workbook.Worksheets[0];
                int endRow = worksheet.Cells.Where(cell => !string.IsNullOrEmpty(cell.Text)).Max(cell => cell.Start.Row);
                int startRow = 2;
                for(int index = startRow; index <= endRow; index++)
                {
                    result.Add(new DataFileTemplateInputOutcomingEntryDetail()
                    {
                        Name = worksheet.Cells[index, 1].GetValue<string>(),
                        BranchName = worksheet.Cells[index, 2].GetValue<string>(),
                        Quantity = worksheet.Cells[index, 3].GetValue<string>(),
                        Price = worksheet.Cells[index, 4].GetValue<string>(),
                    });
                }
            }
            return result;
        }
    }
}
