using Abp.UI;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using FinanceManagement.Entities;
using FinanceManagement.APIs.Accounts.Dto;
using System.Linq;
using Abp.Authorization;
using FinanceManagement.Authorization;
using FinanceManagement.Paging;
using FinanceManagement.Extension;
using FinanceManagement.Accounts.Dto;
using System.IO;
using OfficeOpenXml;
using FinanceManagement.APIs.BankAccounts.Dto;
using FinanceManagement.IoC;
using Abp.Linq.Extensions;

namespace FinanceManagement.APIs.Accounts
{
    [AbpAuthorize]
    public class AccountAppService : FinanceManagementAppServiceBase
    {
        public AccountAppService(IWorkScope workScope) : base(workScope)
        {
        }

        [HttpPost]
        [AbpAuthorize(PermissionNames.Account_Directory_FinanceAccount_Create)]
        public async Task<AccountDto> Create(AccountDto input)
        {
            //code account are unique
            var codeExist = await WorkScope.GetAll<Account>().AnyAsync(s => s.Code == input.Code);
            //Check 1 default for each account type
            var defaultExist = await WorkScope.GetAll<Account>().AnyAsync(s => s.AccountTypeId == input.AccountTypeId && s.Default == true && input.Default == true);
            if (codeExist)
            {
                throw new UserFriendlyException("Account code already exists");
            }
            if (defaultExist)
            {
                throw new UserFriendlyException("Can only have 1 default Account for each Account Type");
            }
            input.Id = await WorkScope.InsertAndGetIdAsync(ObjectMapper.Map<Account>(input));
            CurrentUnitOfWork.SaveChanges();

            var autoCreateBankAccount = await WorkScope.GetAll<AccountType>().AnyAsync(s => s.Id == input.AccountTypeId && (s.Code == Constants.ACCOUNT_TYPE_CLIENT || s.Code == Constants.ACCOUNT_TYPE_OTHER || s.Code == Constants.ACCOUNT_TYPE_SUPPLIER));
            if (autoCreateBankAccount)
            {
                await WorkScope.InsertAsync<BankAccount>(new BankAccount
                {
                    HolderName = input.Name,
                    AccountId = input.Id
                });
            }
            return input;
        }
        [HttpPost]
        [AbpAuthorize(PermissionNames.Account_Directory_FinanceAccount_Create)]
        public async Task<NewAccountDto> CreateNew(NewAccountDto input)
        {

            if (input.Type == Enums.AccountTypeEnum.COMPANY)
            {
                var hasCOMPANY = await WorkScope.GetAll<Account>().AnyAsync(s => s.Type == Enums.AccountTypeEnum.COMPANY && s.IsActive);
                if (hasCOMPANY)
                {
                    throw new UserFriendlyException("Account has Type COMPANY already exists");
                }
            }
            //code account are unique
            var codeExist = await WorkScope.GetAll<Account>().AnyAsync(s => s.Code == input.Code);
            //Check 1 default for each account type
            var defaultExist = await WorkScope.GetAll<Account>().AnyAsync(s => s.AccountTypeId == input.AccountTypeId && s.Default == true && input.Default == true);
            if (codeExist)
            {
                throw new UserFriendlyException("Account code already exists");
            }
            if (defaultExist)
            {
                throw new UserFriendlyException("Can only have 1 default Account for each Account Type");
            }
            input.Id = await WorkScope.InsertAndGetIdAsync(ObjectMapper.Map<Account>(input));

            CurrentUnitOfWork.SaveChanges();


            var autoCreateBankAccount = input.Type != Enums.AccountTypeEnum.COMPANY;
            if(autoCreateBankAccount)
            {
                if (!input.BankId.HasValue)
                {
                    throw new UserFriendlyException("Bank is null");
                }
                if (!input.CurrencyId.HasValue)
                {
                    throw new UserFriendlyException("Currency is null");
                }
                await WorkScope.InsertAsync<BankAccount>(new BankAccount
                {
                    HolderName = input.HolderName,
                    AccountId = input.Id,
                    BankId = input.BankId.Value,
                    BankNumber = input.BankNumber,
                    CurrencyId = input.CurrencyId.Value,
                    IsActive = true,
                });
            }
            
            return input;
        }
        [HttpPut]
        [AbpAuthorize(PermissionNames.Account_Directory_FinanceAccount_Edit)]

        public async Task<AccountDto> Update(AccountDto input)
        {

            var defaultExist = await WorkScope.GetAll<Account>().AnyAsync(s => s.AccountTypeId == input.AccountTypeId && s.Default == true && s.Id != input.Id && input.Default == true);
            if (defaultExist)
            {
                throw new UserFriendlyException("Can only have 1 default Account for each Account Type");
            }
            var codeExist = await WorkScope.GetAll<Account>().AnyAsync(x => x.Code == input.Code && x.Id != input.Id);
            if (codeExist)
                throw new UserFriendlyException("Code is already exist");

            //var autoCreateBankAccount = await WorkScope.GetAll<AccountType>().AnyAsync(s => s.Id == input.AccountTypeId && (s.Code == Constants.ACCOUNT_TYPE_CLIENT || s.Code == Constants.ACCOUNT_TYPE_OTHER));
            //var hasBankAccount = await WorkScope.GetAll<BankAccount>().AnyAsync(ba => ba.AccountId == input.Id);
            //if (autoCreateBankAccount && !hasBankAccount)
            //{
            //    await WorkScope.InsertAsync<BankAccount>(new BankAccount
            //    {
            //        HolderName = input.Name,
            //        AccountId = input.Id
            //    });
            //}
            var Account = await WorkScope.GetAsync<Account>(input.Id);
            if(Account.Type == Enums.AccountTypeEnum.COMPANY && input.Type != Enums.AccountTypeEnum.COMPANY)
            {
                throw new UserFriendlyException("Không thể update account loại COMPANY sang các loại khác");
            }

            if (Account.Type != Enums.AccountTypeEnum.COMPANY && input.Type == Enums.AccountTypeEnum.COMPANY)
            {
                throw new UserFriendlyException("Không thể update các account loại khác sang loại COMPANY");
            }

            input.IsActive = Account.IsActive;
            await WorkScope.UpdateAsync(ObjectMapper.Map<AccountDto, Account>(input, Account));
            return input;
        }
        [HttpPost]
        [AbpAuthorize(PermissionNames.Account_Directory_FinanceAccount)]

        public async Task<GridResult<AccountDto>> GetAllPaging1(GridParam input)
        {
            var query = WorkScope.GetAll<Account>().Select(s => new AccountDto
            {
                Id = s.Id,
                AccountTypeId = s.AccountTypeId,
                Code = s.Code,
                Name = s.Name,
                IsActive = s.IsActive,
                Default = s.Default
            });
            return await query.GetGridResult(query, input);
        }
        [HttpPost]
        [AbpAuthorize(PermissionNames.Account_Directory_FinanceAccount)]
        public async Task<GridResult<GetAccountDto>> GetAllPaging(FilterAccountDto input)
        {
            var bankAccount = WorkScope.GetAll<BankAccount>()
                .WhereIf(input.BankAccountStatus.HasValue, t => t.IsActive == input.BankAccountStatus)
                .Select(s => new
                {
                    s.BankNumber,
                    s.HolderName,
                    s.Id,
                    s.IsActive,
                    s.AccountId,
                    CurrencyName = s.CurrencyId.HasValue ? s.Currency.Name : "",
                    CurrencyCode = s.CurrencyId.HasValue ? s.Currency.Code : "",
                    BankName = s.BankId.HasValue ? s.Bank.Name : "",
                    BankCode = s.BankId.HasValue ? s.Bank.Code : "",
                });

            var query = WorkScope.GetAll<Account>().Select(s => new GetAccountDto
            {
                Id = s.Id,
                Default = s.Default,
                AccountTypeId = s.AccountTypeId,
                AccountTypeName = s.AccountType.Name,
                IsActive = s.IsActive,
                AccountTypeCode = s.AccountType.Code,
                Name = s.Name,
                Code = s.Code,
                Type = s.Type,
                Banks = bankAccount.Where(b => b.AccountId == s.Id)
                .Select(s => new GetBankAccountDto
                {
                    BankNumber = s.BankNumber,
                    HolderName = s.HolderName,
                    Id = s.Id,
                    Status = s.IsActive,
                    CurrencyName = s.CurrencyName,
                    BankName = s.BankName,
                    BankCode = s.BankCode,
                    CurrencyCode = s.CurrencyCode,
                })
            });
            return query.GetGridResultSync(query, input);
        }


        //[HttpPost]
        //[AbpAuthorize(PermissionNames.Account_Directory_Account_ViewAll)]
        //public async Task<GridResult<GetAccountDto>> GetAllPaging1(GridParam input)
        //{
        //    var query = WorkScope.GetAll<Account>()
        //        .Select(s => new GetAccountDto {
        //            AccountTypeCode = s.AccountType.Code,
        //            AccountTypeId = s.AccountType.Id,
        //            AccountTypeName = s.AccountType.Name,
        //            Name = s.Name,
        //            Code = s.Code,
        //            Id = s.Id,
        //            Default = s.Default,
        //            Banks = s.BankAccounts.Select(t => new GetBankAccountDto
        //            {
        //                BankNumber = t.BankNumber,
        //                HolderName = t.HolderName,
        //                CurrencyName = t.Currency?.Name,
        //                BankName = t.Bank?.Name,
        //                BankCode = t.Bank?.Code,
        //                CurrencyCode = t.Currency?.Code,
        //                Id = t.BankAccountId
        //            }).AsQueryable()
        //        });

        //    return query.GetGridResultSync(query, input);
        //}


        [HttpGet]
        public async Task<List<GetAccountDto>> GetAll()
        {

            var listAccountType = await WorkScope.GetAll<AccountType>().ToListAsync();
            var listAccount = await WorkScope.GetAll<Account>().Select(s => new GetAccountDto
            {
                Id = s.Id,
                AccountTypeId = s.AccountTypeId,
                AccountTypeName = s.AccountType.Name,
                AccountTypeCode = s.AccountType.Code,
                Code = s.Code,
                Name = s.Name,
                Default = s.Default
            }).ToListAsync();
            foreach (var item in listAccount)
            {
                item.AccountTypeName = listAccountType.FirstOrDefault(s => s.Id == item.AccountTypeId).Code;
            }
            return listAccount;
        }



        [HttpDelete]
        [AbpAuthorize(PermissionNames.Account_Directory_FinanceAccount_Delete)]
        public async Task Delete(long id)
        {
            var account = await WorkScope.GetAll<Account>().FirstOrDefaultAsync(s => s.Id == id);
            if (account == null)
            {
                throw new UserFriendlyException("Account doesn't exist");
            }

            var hasBankAccount = await WorkScope.GetAll<BankAccount>().AnyAsync(b => b.AccountId == id);
            if (hasBankAccount)
            {
                throw new UserFriendlyException("Can not delete Account when you have linked Bank Account");
            }

            var hasIncomingEntries = await WorkScope.GetAll<IncomingEntry>().AnyAsync(ie => ie.AccountId == id);
            if (hasIncomingEntries)
            {
                throw new UserFriendlyException("Can not delete Account when you have linked Incoming entry");
            }
            await WorkScope.DeleteAsync<Account>(id);
        }
        [HttpGet]
        public async Task<AccountDto> Get(long id)
        {
            return await WorkScope.GetAll<Account>().Where(s => s.Id == id).Select(s => new AccountDto
            {
                Id = s.Id,
                AccountTypeId = s.AccountTypeId,
                Code = s.Code,
                Name = s.Name,
                IsActive = s.IsActive,
                Default = s.Default
            }).FirstOrDefaultAsync();
        }
        [HttpPost]
        [AbpAuthorize(PermissionNames.Account_Directory_FinanceAccount_ActiveDeactive)]
        public async Task ChangeStatus(AccountDto input)
        {
            var account = WorkScope.GetAll<Account>().Where(s => s.Id == input.Id).FirstOrDefault();
            if (account == default)
            {
                throw new UserFriendlyException("Không tồn tại đối tượng kế toán id = " + input.Id);
            }
            account.IsActive = input.IsActive;
            await WorkScope.UpdateAsync(account);
        }
        [HttpGet]
        public async Task<AccountDto> GetAccountDefault()
        {
            var accounts = await WorkScope.GetAll<Account>()
                .Select(s => new AccountDto
                {
                    Id = s.Id,
                    AccountTypeId = s.AccountTypeId,
                    Code = s.Code,
                    Name = s.Name,
                    Default = s.Default,
                    Type = s.Type
                })
                .ToListAsync();
            var accountDefault = accounts.Where(s => s.Default).FirstOrDefault();
            var accountFirstCompany = accounts.Where(s => s.Type == Enums.AccountTypeEnum.COMPANY).FirstOrDefault();
            if(accountDefault != default)
            {
                return accountDefault;
            }
            if (accountFirstCompany != default)
            {
                return accountFirstCompany;
            }
            return default;
        }


        [HttpPost]
        public async Task<object> ImportAccountAndBankAccoutFromFile([FromForm] FileInputDto input)
        {
            var listError = new List<string>();

            if (input == null)
            {
                throw new UserFriendlyException(String.Format("No file upload!"));
            }
            string[] paths = { ".xlsx", ".xltx" };

            if (!paths.Contains(Path.GetExtension(input.File.FileName)))
            {
                throw new UserFriendlyException(String.Format("Invalid file upload. Extension must be .xlsx or .xltx"));
            }

            var accountType = await WorkScope.GetAll<AccountType>().FirstOrDefaultAsync(x => x.Code == Constants.ACCOUNT_TYPE_EMPLOYEE);
            if (accountType == null)
                throw new UserFriendlyException($"AccountType with Code '{Constants.ACCOUNT_TYPE_EMPLOYEE}' not exist");

            var currency = await WorkScope.GetAll<Currency>().FirstOrDefaultAsync(x => x.Code == Constants.CURRENCY_VND);
            if (currency == null)
                throw new UserFriendlyException($"Currency with Code '{Constants.CURRENCY_VND}' not exist");

            var mapBanks = await WorkScope.GetAll<Bank>().Select(s => new { s.Id, s.Code }).ToDictionaryAsync(s => s.Code, s => s.Id);

            List<BankAccountImportDto> list = new List<BankAccountImportDto>();
            using (var stream = new MemoryStream())
            {
                await input.File.CopyToAsync(stream);

                using (var package = new ExcelPackage(stream))
                {
                    ExcelWorksheet worksheet = package.Workbook.Worksheets[0];
                    var rowCount = worksheet.Dimension.Rows;

                    for (int row = 2; row <= rowCount; row++)
                    {

                        string accountName = worksheet.Cells[row, 1].Value.ToString();
                        string accountCode = worksheet.Cells[row, 2].Value.ToString();
                        string bankAccountName = worksheet.Cells[row, 3].Value.ToString();
                        string bankAccountNumber = worksheet.Cells[row, 4].Value.ToString();
                        string bankCode = worksheet.Cells[row, 5].Value.ToString();


                        if (String.IsNullOrEmpty(accountName)
                                    || String.IsNullOrEmpty(accountCode)
                                    || String.IsNullOrEmpty(bankAccountName)
                                    || String.IsNullOrEmpty(bankAccountNumber)
                                    || String.IsNullOrEmpty(bankCode)
                                    )
                        {
                            listError.Add(String.Format("{0} : {1} {2} {3} {4} {5} | some cell is null or empty", row, accountName, accountCode, bankAccountName, bankAccountNumber, bankCode));
                            continue;
                        }

                        if (!mapBanks.ContainsKey(worksheet.Cells[row, 5].Value.ToString()))
                        {
                            listError.Add(String.Format("{0} : {1} {2} {3} {4} {5} | not exist BankCOde in DB", row, accountName, accountCode, bankAccountName, bankAccountNumber, bankCode));
                            continue;
                        }

                        list.Add(new BankAccountImportDto
                        {
                            AccountName = accountName,
                            AccountCode = accountCode,
                            HolderName = bankAccountName,
                            BankAccountNumber = bankAccountNumber,
                            BankCode = bankCode
                        });
                    }
                }
            }

            foreach (BankAccountImportDto item in list)
            {
                long bankId = mapBanks[item.BankCode];

                var account = new Account
                {
                    AccountTypeId = accountType.Id,
                    Name = item.AccountName?.Trim(),
                    Code = item.AccountCode?.Trim(),
                    Default = false
                };
                account.Id = await WorkScope.InsertAndGetIdAsync(account);
                var bankAccount = new BankAccount
                {
                    HolderName = item.HolderName?.Trim(),
                    BankNumber = item.BankAccountNumber?.Trim(),
                    AccountId = account.Id,
                    BankId = bankId,
                    CurrencyId = currency.Id,
                    Amount = 0
                };
                bankAccount.Id = await WorkScope.InsertAndGetIdAsync(bankAccount);

            }
            return new
            {
                ListError = listError,
                TotalSuccess = list.Count()
            };
        }


    }
}


