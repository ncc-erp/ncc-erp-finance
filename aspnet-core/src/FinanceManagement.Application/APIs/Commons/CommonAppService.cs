using FinanceManagement.Entities.NewEntities;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using FinanceManagement.APIs.Commons.Dtos;
using Microsoft.EntityFrameworkCore;
using FinanceManagement.Helper;
using FinanceManagement.Enums;
using FinanceManagement.GeneralModels;
using FinanceManagement.Entities;
using FinanceManagement.IoC;
using FinanceManagement.Uitls;
using FinanceManagement.APIs.IncomingEntryTypes.Dto;
using iText.Commons.Utils;
using FinanceManagement.Managers.Commons;
using Abp.Linq.Extensions;
using FinanceManagement.Extension;
using Abp.Authorization;
using FinanceManagement.Authorization;
using FinanceManagement.Managers.CircleChartDetails.Dtos;

namespace FinanceManagement.APIs.Commons
{
    public class CommonAppService : FinanceManagementAppServiceBase
    {
        private readonly ICommonManager _commonManager;
        private readonly IPermissionChecker _permissionChecker;
        public CommonAppService(IWorkScope workScope, ICommonManager commonManager, IPermissionChecker permissionChecker) : base(workScope)
        {
            _commonManager = commonManager;
            _permissionChecker = permissionChecker;
        }

        [HttpGet]
        public async Task<List<ValueAndNameDto>> GetBankAccountsInBTransaction()
        {
            return await WorkScope.GetAll<BTransaction>()
                .Select(x => new ValueAndNameDto
                {
                    Value = x.BankAccountId,
                    Name = x.BankAccount.HolderName
                })
                .Distinct()
                .ToListAsync();
        }
        [HttpGet]
        public async Task<List<ValueAndNameDto>> GetBankAccoutForCompany()
        {
            return await WorkScope.GetAll<BankAccount>()
                .Where(q => q.Account.Type == AccountTypeEnum.COMPANY)
                .Select(s => new ValueAndNameDto
                {
                    Name = s.HolderName,
                    Value = s.Id
                }).ToListAsync();
        }
        [HttpGet]
        public async Task<List<GetAccountCompanyForDropdownDto>> GetAllAccounts(bool isInActive = false)
        {
            return await WorkScope.GetAll<Account>()
                .WhereIf(!isInActive, s => s.IsActive)
                .Select(x => new GetAccountCompanyForDropdownDto
                {
                    Name = x.Name + " [" + ((AccountTypeEnum)x.Type).ToString() + "]",
                    Value = x.Id,
                    IsDefault = x.Default
                }).ToListAsync();
        }
        [HttpGet]
        public List<ValueAndNameDto> GetRevenueStatuses()
        {
            return Helpers.ListRevenueStatuses;
        }
        [HttpGet]
        public List<ValueAndNameDto> GetInvoiceStatuses()
        {
            return Helpers.ListInvoiceStatuses;
        }
        [HttpGet]
        public List<ValueAndNameDto> GetBTransactionStatus()
        {
            return Helpers.ListBTransactionStatuses;
        }
        [HttpGet]
        public async Task<List<ValueAndNameDto>> GetAccountInRevenues()
        {
            return await WorkScope.GetAll<RevenueManaged>()
                                    .Select(s => new ValueAndNameDto
                                    {
                                        Name = s.Account.Name + " [" + s.Account.Code + "]",
                                        Value = s.AccountId.HasValue ? s.AccountId.Value : 0,
                                    })
                                    .Distinct()
                                    .ToListAsync();
        }

        [HttpGet]
        public async Task<List<ValueAndNameDto>> GetAccountInInvoices()
        {
            return await WorkScope.GetAll<Invoice>()
                                    .Select(s => new ValueAndNameDto
                                    {
                                        Name = s.Account.Name + " [" + s.Account.Code + "]",
                                        Value = s.AccountId,
                                    })
                                    .Distinct()
                                    .ToListAsync();
        }

        [HttpGet]
        public async Task<List<ValueAndNameDto>> GetAllClient(bool isInActive = false)
        {
            return await WorkScope.GetAll<Account>()
                                    .Where(x => x.Type == AccountTypeEnum.CLIENT)
                                    .WhereIf(!isInActive, s => s.IsActive)
                                    .Select(s => new ValueAndNameDto
                                    {
                                        Name = s.Name + " [" + s.Code + "]",
                                        Value = s.Id,
                                    })
                                    .Distinct()
                                    .ToListAsync();
        }
        [HttpGet]
        public async Task<List<ClientInfoDto>> GetAllClientInfo(bool isInActive = false)
        {
            return await WorkScope.GetAll<Account>()
                                    .Where(x => x.Type == AccountTypeEnum.CLIENT)
                                    .WhereIf(!isInActive, s => s.IsActive)
                                    .Select(s => new ClientInfoDto
                                    {
                                        ClientName = s.Name + " [" + s.Code + "]",
                                        ClientId = s.Id,
                                    })
                                    .Distinct()
                                    .ToListAsync();
        }
        [HttpGet]
        public async Task<List<ValueAndNameDto>> GetAllAccountCompany(bool isShowAll)
        {
            return await WorkScope.GetAll<Account>()
                .Where(x => x.Type == AccountTypeEnum.COMPANY)
                .WhereIf(!isShowAll, s => s.IsActive)
                .Select(s => new ValueAndNameDto
                {
                    Name = s.Name + " [" + s.Code + "]",
                    Value = s.Id,
                })
                .AsNoTracking()
                .Distinct()
                .ToListAsync();
        }


        [HttpGet]
        public async Task<List<ValueAndNameDto>> GetIncomingEntryTypes(bool isShowInActive = false)
        {
            return await WorkScope.GetAll<IncomingEntryType>()
                .WhereIf(!isShowInActive, s => s.IsActive)
                .Select(s => new ValueAndNameDto
                {
                    Name = s.Name,
                    Value = s.Id
                }).ToListAsync();
        }

        [HttpGet]
        public async Task<List<BankAccountOption>> GettAllBankAccount(bool isInActive = false)
        {
            return await WorkScope.GetAll<BankAccount>()
                .WhereIf(!isInActive, s => s.IsActive)
                .Where(x => x.Account.Type == AccountTypeEnum.COMPANY)
                .Select(s => new BankAccountOption
                {
                    BankAccountId = s.Id,
                    BankAccountHolderName = s.HolderName,
                    AccountId = s.AccountId,
                    AccountName = s.Account.Name,
                    AccountTypeId = s.Account.AccountTypeId,
                    AccountTypeName = s.Account.AccountType.Name,
                    AccountTypeEnum = s.Account.Type,
                    BankAccountCurrencyCode = s.Currency.Code,
                    BankAccountCurrencyId = s.CurrencyId,
                    BankAccountCurrencyName = s.Currency.Name,
                    BankAccountTypeCode = s.Account.AccountType.Code,
                    BankAccountNumber = s.BankNumber
                }).ToListAsync();
        }
        [HttpGet]
        public async Task<List<ValueAndNameDto>> GetAllAccounTypeEnum()
        {
            return Enum.GetValues(typeof(AccountTypeEnum)).Cast<AccountTypeEnum>()
                .Select(s => new ValueAndNameDto
                {
                    Name = s.ToString(),
                    Value = s.GetHashCode()
                }).ToList();
        }
        [HttpGet]
        public async Task<IEnumerable<TreeItem<OutputCategoryEntryType>>> GetTreeIncomingEntries(bool isActiveOnly = true)
        {
            await Task.CompletedTask;
            return _commonManager.GetTreeIncomingEntries(isActiveOnly);
        }
        [HttpGet]
        public async Task<IEnumerable<TreeItem<OutputCategoryEntryType>>> GetTreeOutcomingEntries(bool isActiveOnly = true)
        {
            await Task.CompletedTask;
            return _commonManager.GetTreeOutcomingEntries(isActiveOnly);
        }

        public async Task<List<BTransDto>> GetBTransactionOptions()
        {
            return await WorkScope.GetAll<BTransaction>()
                .Select(s => new BTransDto {
                    Id = s.Id,
                    Name = s.BankAccount.HolderName,
                    Money = s.Money,
                    CurrencyName = s.BankAccount.Currency.Name,
                    TimeAt = s.TimeAt,
                    CurrencyColor = CommonUtils.GetCurrencyColor(s.BankAccount.Currency.Name)
                }).ToListAsync();
        }

        [HttpGet]
        public async Task<List<ValueAndNameDto>> GetOutcomingEntry()
        {
            return await WorkScope.GetAll<OutcomingEntry>()
                .Select(s => new ValueAndNameDto
                {
                    Name = $"#{s.Id} {s.Name} {s.OutcomingEntryType.Name} ({Helpers.FormatMoney(s.Value)}{s.Currency.Name})",
                    Value = s.Id
                }).ToListAsync();
        }
        [HttpGet]
        public async Task<List<ValueAndNameDto>> GetApprovedOutcomingEntry()
        {
            return await WorkScope.GetAll<OutcomingEntry>()
                .Where(s => s.WorkflowStatus.Code == FinanceManagementConsts.WORKFLOW_STATUS_APPROVED || s.WorkflowStatus.Code == FinanceManagementConsts.WORKFLOW_STATUS_END)
                .Select(s => new ValueAndNameDto
                {
                    Name = $"#{s.Id} {s.Name} {s.OutcomingEntryType.Name} ({Helpers.FormatMoney(s.Value)}{s.Currency.Name}) {s.CreationTime.ToString("dd/MM/yyyy")}",
                    Value = s.Id
                }).ToListAsync();
        }
        [HttpGet]
        public  List<SelectionOutcomingEntry> GetApprovedAndEndOutcomingEntry(bool onlyApproved = true, long currencyId = 0)
        {
            var approvedAndEndOutcomingEntrys = WorkScope.GetAll<OutcomingEntry>()
                .WhereIf(currencyId != 0, s => s.CurrencyId == currencyId)
                .Where(s => s.WorkflowStatus.Code == FinanceManagementConsts.WORKFLOW_STATUS_APPROVED || s.WorkflowStatus.Code == FinanceManagementConsts.WORKFLOW_STATUS_END)
                .Select(s => new
                {
                    Name = $"#{s.Id} {s.Name} ({s.OutcomingEntryType.Name}) ({Helpers.FormatMoney(s.Value)}{s.Currency.Name}) {s.CreationTime.ToString("dd/MM/yyyy")}",
                    Value = s.Id,
                    StatusCode = s.WorkflowStatus.Code,
                    StatusName = s.WorkflowStatus.Name,
                    LastModifiedTime = s.LastModificationTime,
                    TypeName = s.OutcomingEntryType.Name,
                    TypeId = s.OutcomingEntryType.Id,
                    Money = s.Value
                })
                .ToList();
            var approvedOutcomingEntrys = approvedAndEndOutcomingEntrys.Where(s => s.StatusCode == FinanceManagementConsts.WORKFLOW_STATUS_APPROVED)
                .Select(s => new SelectionOutcomingEntry
                {
                    Name = s.Name,
                    Value = s.Value,
                    StatusCode = s.StatusCode,
                    StatusName = s.StatusName,
                    TypeId = s.TypeId,
                    TypeName = s.TypeName,
                    Money = s.Money
                }).ToList();
            if (onlyApproved)
            {
                return approvedOutcomingEntrys;
            }
            var last2Month = DateTimeUtils.FirstDayOfCurrentyMonth().AddMonths(-2);
            var endOutcomingEntrys = approvedAndEndOutcomingEntrys.Where(s => s.StatusCode == FinanceManagementConsts.WORKFLOW_STATUS_END)                                    
                .Where(s => s.LastModifiedTime > last2Month)
                .Select(s => new SelectionOutcomingEntry
                {
                    Name = s.Name,
                    Value = s.Value,
                    StatusCode = s.StatusCode,
                    StatusName = s.StatusName,
                    TypeId = s.TypeId,
                    TypeName = s.TypeName,
                    Money = s.Money
                }).ToList();
            return approvedOutcomingEntrys.Concat(endOutcomingEntrys).ToList();
        }
        [HttpGet]
        public async Task<List<ValueAndNameDto>> GetAccountTypeIdOptions()
        {
            return await WorkScope.GetAll<AccountType>()
                .Select(s => new ValueAndNameDto
                {
                    Name = s.Name,
                    Value = s.Id
                }).ToListAsync();
        }
        [HttpGet]
        public async Task<List<ValueAndNameDto>> GetAccountIdOptions(long accountTypeId, bool isInActive = false)
        {
            return await WorkScope.GetAll<Account>()
                .WhereIf(!isInActive, s => s.IsActive)
                .Where(s => s.AccountTypeId == accountTypeId)
                .Select(s => new ValueAndNameDto
                {
                    Name = s.Name,
                    Value = s.Id
                }).ToListAsync();
        }
        [HttpGet]
        public async Task<List<BankAccountOption>> GettAllBankAccountByAccoutId(long accountId, bool isInActive = false)
        {
            return await WorkScope.GetAll<BankAccount>()
                //.Where(x => x.Account.Type == AccountTypeEnum.COMPANY)
                .WhereIf(!isInActive, s => s.IsActive)
                .OrderBy(s => Math.Abs(s.Account.Type - Enums.AccountTypeEnum.COMPANY))
                .Where(x => x.AccountId == accountId)
                .Select(s => new BankAccountOption
                {
                    BankAccountId = s.Id,
                    BankAccountHolderName = s.HolderName,
                    AccountId = s.AccountId,
                    AccountName = s.Account.Name,
                    AccountTypeId = s.Account.AccountTypeId,
                    AccountTypeName = s.Account.AccountType.Name,
                    AccountTypeEnum = s.Account.Type,
                    BankAccountCurrencyCode = s.Currency.Code,
                    BankAccountCurrencyId = s.CurrencyId,
                    BankAccountCurrencyName = s.Currency.Name,
                    BankAccountTypeCode = s.Account.AccountType.Code,
                    BankAccountNumber = s.BankNumber
                }).ToListAsync();
        }
        [HttpGet]
        public async Task<List<ValueAndNameDto>> GetIncomingEntryByIncomingEntryId(long incomingEntryTypeId)
        {
            return await WorkScope.GetAll<IncomingEntry>()
                .Where(x => x.IncomingEntryTypeId == incomingEntryTypeId)
                .Select(x => new ValueAndNameDto
                {
                    Name = x.Name,
                    Value = x.Id
                }).ToListAsync();
        }
        [HttpGet]
        public async Task<List<BankAccountOption>> GetCommonBankAccountTransactionOpstion(bool isInActive = false)
        {
            return await WorkScope.GetAll<BankAccount>()
                .WhereIf(!isInActive, s => s.IsActive)
                .OrderBy(s => Math.Abs(s.Account.Type - Enums.AccountTypeEnum.COMPANY))
                .Select(s => new BankAccountOption
                {
                    BankAccountId = s.Id,
                    BankAccountHolderName = s.HolderName,
                    AccountId = s.AccountId,
                    AccountName = s.Account.Name,
                    AccountTypeId = s.Account.AccountTypeId,
                    AccountTypeName = s.Account.AccountType.Name,
                    AccountTypeEnum = s.Account.Type,
                    BankAccountCurrencyCode = s.Currency.Code,
                    BankAccountCurrencyId = s.CurrencyId,
                    BankAccountCurrencyName = s.Currency.Name,
                    BankAccountTypeCode = s.Account.AccountType.Code,
                    BankAccountNumber = s.BankNumber
                }).ToListAsync();
        }
        [HttpGet]
        public async Task<List<BankAccountOption>> GetBankAccountVNDOpstion(bool isInActive = false)
        {
            var currencyDefault = await GetCurrencyDefaultAsync();

            return await WorkScope.GetAll<BankAccount>()
                .Where(s => s.CurrencyId == currencyDefault.Id)
                .WhereIf(!isInActive, s => s.IsActive)
                .OrderBy(s => Math.Abs(s.Account.Type - Enums.AccountTypeEnum.COMPANY))
                .Select(s => new BankAccountOption
                {
                    BankAccountId = s.Id,
                    BankAccountHolderName = s.HolderName,
                    AccountId = s.AccountId,
                    AccountName = s.Account.Name,
                    AccountTypeId = s.Account.AccountTypeId,
                    AccountTypeName = s.Account.AccountType.Name,
                    AccountTypeEnum = s.Account.Type,
                    BankAccountCurrencyCode = s.Currency.Code,
                    BankAccountCurrencyId = s.CurrencyId,
                    BankAccountCurrencyName = s.Currency.Name,
                    BankAccountTypeCode = s.Account.AccountType.Code,
                    BankAccountNumber = s.BankNumber
                }).ToListAsync();
        }
        [HttpGet]
        public async Task<List<IncomingEntryTypeDto>> GetAllIncomingEntryType(bool isInActive = false)
        {
            return await WorkScope.GetAll<IncomingEntryType>()
                .WhereIf(!isInActive, s => s.IsActive)
                .Select(s => new IncomingEntryTypeDto
                {
                    Id = s.Id,
                    Code = s.Code,
                    Name = s.Name,
                    PathId = s.PathId,
                    PathName = s.PathName,
                    Level = s.Level,
                    ParentId = s.ParentId,
                    RevenueCounted = s.RevenueCounted
                }).ToListAsync();
        }
        [HttpGet]
        public async Task<IEnumerable<ValueAndNameDto>> GetAllBank()
        {
            return await WorkScope.GetAll<Bank>()
                .Select(s => new ValueAndNameDto
                {
                    Value = s.Id,
                    Name = s.Name
                })
                .AsNoTracking()
                .ToListAsync();
        }
        [HttpGet]
        public async Task<IEnumerable<ValueAndNameDto>> GetAllCurrency()
        {
            return await WorkScope.GetAll<Currency>()
                .Select(s => new ValueAndNameDto
                {
                    Value = s.Id,
                    Name = s.Name
                })
                .AsNoTracking()
                .ToListAsync();
        }
        [HttpGet]
        public IEnumerable<BankAccountOption> GetAllBankAccount(bool? isActive)
        {
            return WorkScope.GetAll<BankAccount>()
                .WhereIf(isActive.HasValue, s => s.IsActive == isActive)
                .OrderBy(x => Math.Abs(x.Account.Type - AccountTypeEnum.COMPANY))
                .Select(s => new BankAccountOption
                {
                    BankAccountId = s.Id,
                    BankAccountHolderName = s.HolderName,
                    AccountId = s.AccountId,
                    AccountName = s.Account.Name,
                    AccountTypeId = s.Account.AccountTypeId,
                    AccountTypeName = s.Account.AccountType.Name,
                    AccountTypeEnum = s.Account.Type,
                    BankAccountCurrencyCode = s.Currency.Code,
                    BankAccountCurrencyId = s.CurrencyId,
                    BankAccountCurrencyName = s.Currency.Name,
                    BankAccountTypeCode = s.Account.AccountType.Code,
                    BankAccountNumber = s.BankNumber
                })
                .AsNoTracking()
                .ToList();
        }
        public async Task<List<BankAccountOption>> GetBankAccountByCurrency(long currencyId, bool isInActive = false)
        {
            return await WorkScope.GetAll<BankAccount>()
                .WhereIf(!isInActive, s => s.IsActive)
                .Where(s => s.CurrencyId == currencyId)
                .OrderBy(s => Math.Abs(s.Account.Type - Enums.AccountTypeEnum.COMPANY))
                .Select(s => new BankAccountOption
                {
                    BankAccountId = s.Id,
                    BankAccountHolderName = s.HolderName,
                    AccountId = s.AccountId,
                    AccountName = s.Account.Name,
                    AccountTypeId = s.Account.AccountTypeId,
                    AccountTypeName = s.Account.AccountType.Name,
                    AccountTypeEnum = s.Account.Type,
                    BankAccountCurrencyCode = s.Currency.Code,
                    BankAccountCurrencyId = s.CurrencyId,
                    BankAccountCurrencyName = s.Currency.Name,
                    BankAccountTypeCode = s.Account.AccountType.Code,
                    BankAccountNumber = s.BankNumber
                }).ToListAsync();
        }
        [HttpGet]
        public long? GetDefaultBankAccountByCurrencyId(long currencyId)
        {
            return WorkScope.GetAll<Currency>()
                .Where(s => s.Id == currencyId)
                .Select(s => s.DefaultBankAccountId)
                .FirstOrDefault();
        }
        [HttpGet]
        public long? GetConversionTransactionDefaultBankAccountByCurrencyId(long currencyId)
        {
            return WorkScope.GetAll<Currency>()
                .Where(s => s.Id == currencyId)
                .Select(s => s.DefaultBankAccountIdWhenSell)
                .FirstOrDefault();
        }
        public async Task<List<BankAccountOption>> GetBankAccountByCurrencyCode(string code)
        {
            return await WorkScope.GetAll<BankAccount>()
                .OrderBy(s => Math.Abs(s.Account.Type- AccountTypeEnum.COMPANY))
                .Where(s => s.Currency.Code.Trim().ToLower().Equals(code.Trim().ToLower()))
                .Select(s => new BankAccountOption
                {
                    BankAccountId = s.Id,
                    BankAccountHolderName = s.HolderName,
                    AccountId = s.AccountId,
                    AccountName = s.Account.Name,
                    AccountTypeId = s.Account.AccountTypeId,
                    AccountTypeName = s.Account.AccountType.Name,
                    AccountTypeEnum = s.Account.Type,
                    BankAccountCurrencyCode = s.Currency.Code,
                    BankAccountCurrencyId = s.CurrencyId,
                    BankAccountCurrencyName = s.Currency.Name,
                    BankAccountTypeCode = s.Account.AccountType.Code,
                    BankAccountNumber = s.BankNumber
                }).ToListAsync();
        }
        [HttpGet]
        public long? GetDefaultFromBankAccountByCurrencyIdWhenBuy(long currencyId)
        {
            return WorkScope.GetAll<Currency>()
                .Where(s => s.Id == currencyId)
                .Select(s => s.DefaultFromBankAccountIdWhenBuy)
                .FirstOrDefault();
        }
        [HttpGet]
        public long? GetDefaultToBankAccountByCurrencyIdWhenBuy(long currencyId)
        {
            return WorkScope.GetAll<Currency>()
                .Where(s => s.Id == currencyId)
                .Select(s => s.DefaultToBankAccountIdWhenBuy)
                .FirstOrDefault();
        }
        [HttpPost]
        public async Task<List<BankAccountOption>> GetBankAccountOptions(FilterBankAccount filterBankAccount)
        {
            return await WorkScope.GetAll<BankAccount>()
                .WhereIf(filterBankAccount.CurrencyId.HasValue, s => s.CurrencyId == filterBankAccount.CurrencyId.Value)
                .WhereIf(!filterBankAccount.CurrencyNameOrCode.IsEmpty(), s => s.Currency.Name == filterBankAccount.CurrencyNameOrCode || s.Currency.Code == filterBankAccount.CurrencyNameOrCode)
                .WhereIf(filterBankAccount.IsActive.HasValue, s => s.IsActive == filterBankAccount.IsActive.Value)
                .WhereIf(filterBankAccount.IsAccountTypeNotCompany, s => s.Account.Type != AccountTypeEnum.COMPANY)
                .OrderBy(s => Math.Abs(s.Account.Type - filterBankAccount.OrderByType))
                .Select(s => new BankAccountOption
                {
                    BankAccountId = s.Id,
                    BankAccountHolderName = s.HolderName,
                    AccountId = s.AccountId,
                    AccountName = s.Account.Name,
                    AccountTypeId = s.Account.AccountTypeId,
                    AccountTypeName = s.Account.AccountType.Name,
                    AccountTypeEnum = s.Account.Type,
                    BankAccountCurrencyCode = s.Currency.Code,
                    BankAccountCurrencyId = s.CurrencyId,
                    BankAccountCurrencyName = s.Currency.Name,
                    BankAccountTypeCode = s.Account.AccountType.Code,
                    BankAccountNumber = s.BankNumber
                }).ToListAsync();
        }
        [HttpGet]
        public async Task<List<ValueAndNameDto>> GetBankTransactionFromCompanyByCurrency(long? currencyId)
        {
            var isAllowOutcomingEntryByMutipleCurrency = await IsAllowOutcomingEntryByMutipleCurrency();
            var bankAccounts = WorkScope.GetAll<BankAccount>()
                .WhereIf(isAllowOutcomingEntryByMutipleCurrency, s => s.Account.Type == AccountTypeEnum.COMPANY)
                .WhereIf(isAllowOutcomingEntryByMutipleCurrency && currencyId.HasValue, s => s.CurrencyId == currencyId)
                .Select(s => s.Id);

            return await WorkScope.GetAll<BankTransaction>()
                .Where(s => bankAccounts.Contains(s.FromBankAccountId))
                .Select(s => new ValueAndNameDto
                {
                    Name = $"#{s.Id} {s.Name} {s.FromValue}",
                    Value = s.Id
                }).ToListAsync();
        }

        [HttpGet]
        public async Task<IEnumerable<TreeItem<OutputCategoryEntryType>>> GetOptionTreeOutcomingEntriesByUser(bool isShowAll)
        {
            var isViewAll = await _permissionChecker.IsGrantedAsync(PermissionNames.Finance_OutcomingEntry_ViewAllOutcomingEntryTypeFilter);

            if (isViewAll)
            {
                return await GetTreeOutcomingEntries();
            }
            else
            {
                return await _commonManager.GetTreeOutcomingTypeByUserId(AbpSession.UserId.Value, isShowAll);
            }
        }
        [HttpGet]
        public long? GetDefaultToBankAccountByCurrencyId(long currencyId)
        {
            return WorkScope.GetAll<Currency>()
                .Where(s => s.Id == currencyId)
                .Select(s => s.DefaultBankAccountId)
                .FirstOrDefault();
        }
        [HttpPost]
        public async Task<IEnumerable<TreeItem<OutputCategoryEntryType>>> GetTypeOptions(FilterTypeOptions input)
        {
            if(input.Type == TypeFilterTypeOptions.INCOMING_ENTRY_TYPE) 
                return await GetTreeIncomingEntries(!input.IsShowAll);

            if (input.UserId.HasValue)
                return await GetOptionTreeOutcomingEntriesByUser(input.IsShowAll);

            return await GetTreeOutcomingEntries(!input.IsShowAll);
        }


    }
}

