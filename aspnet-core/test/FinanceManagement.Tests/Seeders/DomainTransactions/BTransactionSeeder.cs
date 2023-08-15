using FinanceManagement.Entities;
using FinanceManagement.Entities.NewEntities;
using FinanceManagement.EntityFrameworkCore;
using FinanceManagement.Enums;
using FinanceManagement.Managers.BTransactions.Dtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace FinanceManagement.Tests.Seeders.DomainTransactions
{
    public class BTransactionSeeder : ISeeder
    {
        public void Seed(FinanceManagementDbContext context)
        {
            //TODO:
            // Incoming Entry Type
            var incomingEntryTypes = new List<IncomingEntryType>()
            {
                new IncomingEntryType() { Code = FinanceManagementConsts.BalanceClientCode, CreationTime = DateTime.Now, Id = 1, Level = 1, Name = "Khách hàng trả trước" },
                new IncomingEntryType() { Code = FinanceManagementConsts.RevenueClientCode, CreationTime = DateTime.Now, Id = 2, Level = 1, Name = "Thu tiền Invoice", }
            };

            // Outcoming Entry Type 
            var outcomingEntryTypes = new List<OutcomingEntryType>()
            {
                new OutcomingEntryType() {Code = FinanceManagementConsts.OUTCOMING_ENTRY_TYPE_SALARY, CreationTime = DateTime.Now, Id = 1, Level = 1, Name = "Chi Lương"},
                new OutcomingEntryType() {Code = FinanceManagementConsts.OUTCOMING_ENTRY_TYPE_MONEY_TRANSFER, CreationTime = DateTime.Now, Id = 2, Level = 1, Name = "Chi khác"},
            };

            // Workflow status 
            var workflowStatus = new List<WorkflowStatus>()
            {
                new WorkflowStatus() {Code = FinanceManagementConsts.WORKFLOW_STATUS_START, Id = 1, Name = "Tạo mới"},
                new WorkflowStatus() {Code = FinanceManagementConsts.WORKFLOW_STATUS_APPROVED, Id = 2, Name = "Duyệt"},
                new WorkflowStatus() {Code = FinanceManagementConsts.WORKFLOW_STATUS_END, Id = 3, Name = "Thực thi"},
            };

            // Currency
            var currencies = new List<Currency>()
            {
                new Currency{ Code = "VND", Name = "VND", Id = 1, IsCurrencyDefault = true },
                new Currency{ Code = "USD", Name = "USD", Id = 2 },
                new Currency{ Code = "AUD", Name = "AUD", Id = 3 },
                new Currency{ Code = "EUR", Name = "EUR", Id = 4 },
            };

            // Account Type
            var accountTypes = new List<AccountType>()
            {
                new AccountType { Id = 1, Name = "Công ty", Code = "COMPANY" },
                new AccountType { Id = 2, Name = "Khách hàng", Code = "CLIENT" },
            };

            // Bank
            var banks = new List<Bank>()
            {
                new Bank { Code = "TCB", Name = "Ngân hàng TMCP Kỹ Thương Việt Nam", Id = 1},
                new Bank { Code = "VCB", Name = "Ngân hàng TMCP Ngoại Thương Việt Nam", Id = 2},
                new Bank { Code = "TPB", Name = "Ngân hàng thương mại cổ phần Tiên Phong", Id = 3}
            };

            // Account
            var accounts = new List<Account>()
            {
                //Company
                new Account { Id = 1, Name = "NCC", AccountTypeId = 1, Code = "NCC" },
                //Customer
                new Account { Id = 2, Name = "TEST_1", AccountTypeId = 2, Code = "TEST_1" },
                new Account { Id = 3, Name = "TEST_2", AccountTypeId = 2, Code = "TEST_2" },
                new Account { Id = 4, Name = "TEST_3", AccountTypeId = 2, Code = "TEST_3" },
                new Account { Id = 5, Name = "TEST_4", AccountTypeId = 2, Code = "TEST_4" },
                new Account { Id = 6, Name = "TEST_5", AccountTypeId = 2, Code = "TEST_5" },
                new Account { Id = 7, Name = "TEST_6", AccountTypeId = 2, Code = "TEST_6" },
                new Account { Id = 8, Name = "TEST_7", AccountTypeId = 2, Code = "TEST_7" },
                new Account { Id = 9, Name = "TEST_8", AccountTypeId = 2, Code = "TEST_8" },
                new Account { Id = 10, Name = "TEST_9", AccountTypeId = 2, Code = "TEST_9" },
                new Account { Id = 11, Name = "TEST_10", AccountTypeId = 2, Code = "TEST_10" },
                new Account { Id = 12, Name = "TEST_11", AccountTypeId = 2, Code = "TEST_11" },
                new Account { Id = 13, Name = "TEST_12", AccountTypeId = 2, Code = "TEST_12" },
                new Account { Id = 14, Name = "TEST_13", AccountTypeId = 2, Code = "TEST_13" },
                new Account { Id = 15, Name = "TEST_14", AccountTypeId = 2, Code = "TEST_14" },
                new Account { Id = 16, Name = "TEST_15", AccountTypeId = 2, Code = "TEST_15" },
                new Account { Id = 17, Name = "TEST_16", AccountTypeId = 2, Code = "TEST_16" },
                new Account { Id = 18, Name = "TEST_17", AccountTypeId = 2, Code = "TEST_17" },
                new Account { Id = 19, Name = "TEST_18", AccountTypeId = 2, Code = "TEST_18" },
                new Account { Id = 20, Name = "TEST_19", AccountTypeId = 2, Code = "TEST_19" },
                new Account { Id = 21, Name = "TEST_20", AccountTypeId = 2, Code = "TEST_20" },
                new Account { Id = 22, Name = "TEST_21", AccountTypeId = 2, Code = "TEST_21" },
                new Account { Id = 23, Name = "TEST_22", AccountTypeId = 2, Code = "TEST_22" },
                new Account { Id = 101, Name = "NCC", AccountTypeId = 1, Code = "NCC", Type = AccountTypeEnum.COMPANY },
                new Account { Id = 102, Name = "TEST_16", AccountTypeId = 2, Code = "TEST_16" },
                new Account { Id = 103, Name = "TEST_17", AccountTypeId = 2, Code = "TEST_17" },
                new Account { Id = 104, Name = "TEST_18", AccountTypeId = 2, Code = "TEST_18" },

            };

            // Bank Account
            var bankAccounts = new List<BankAccount>()
            {
                new BankAccount { Id = 1, AccountId = 1, Amount = 10000, BankId = 1, BankNumber = "123456788", CurrencyId = 1, HolderName = "Techcombank VN"},
                new BankAccount { Id = 2, AccountId = 2, Amount = 10000, BankId = 1, BankNumber = "123456789", CurrencyId = 2, HolderName = "VCB VN"},
                new BankAccount { Id = 3, AccountId = 3, Amount = 10000, BankId = 1, BankNumber = "123456789", CurrencyId = 1, HolderName = "VCB VN"},
                new BankAccount { Id = 4, AccountId = 4, Amount = 10000, BankId = 1, BankNumber = "123456789", CurrencyId = 1, HolderName = "VCB VN"},
                new BankAccount { Id = 5, AccountId = 5, Amount = 10000, BankId = 1, BankNumber = "123456789", CurrencyId = 2, HolderName = "VCB VN"},
                new BankAccount { Id = 6, AccountId = 6, Amount = 10000, BankId = 1, BankNumber = "123456789", CurrencyId = 2, HolderName = "VCB VN"},
                new BankAccount { Id = 101, AccountId = 101, Amount = 10000, BankId = 1, BankNumber = "123456788", CurrencyId = 1, HolderName = "Techcombank VN"},
                new BankAccount { Id = 102, AccountId = 102, Amount = 10000, BankId = 2, BankNumber = "123456789", CurrencyId = 2, HolderName = "VCB VN"},
                new BankAccount { Id = 103, AccountId = 103, Amount = 10000, BankId = 2, BankNumber = "123456789", CurrencyId = 1, HolderName = "VCB VN"},
                new BankAccount { Id = 104, AccountId = 104, Amount = 10000, BankId = 2, BankNumber = "123456789", CurrencyId = 1, HolderName = "VCB VN"},
            };

            // BTransaction
            var bTransactions = new List<BTransaction>()
            {
                // BĐSD > 0
                new BTransaction
                {
                    Id = 1,
                    BankAccountId = 1,
                    FromAccountId = 1,
                    Money = 100,
                    TimeAt = DateTime.Now,
                    Status = Enums.BTransactionStatus.PENDING
                },
                //new BTransaction
                //{
                //    Id = 2,
                //    BankAccountId = 1,
                //    FromAccountId = 1,
                //    Money = 100,
                //    TimeAt = DateTime.UtcNow,
                //    Status = Enums.BTransactionStatus.DONE
                //},
                // BĐSD < 0
                new BTransaction
                {
                    Id = 3,
                    BankAccountId = 1,
                    FromAccountId = 1,
                    Money = -100,
                    TimeAt = DateTime.Now,
                    Status = Enums.BTransactionStatus.PENDING
                },
                new BTransaction
                {
                    Id = 4,
                    BankAccountId = 1,
                    FromAccountId = 1,
                    Money = -100,
                    TimeAt = DateTime.UtcNow,
                    Status = Enums.BTransactionStatus.DONE
                },
                // -- TEST CHI LƯƠNG --
                // khác currency VNĐ
                new BTransaction
                {
                    Id = 5,
                    BankAccountId = 2,
                    FromAccountId = 1,
                    Money = -100,
                    TimeAt = DateTime.Now,
                    Status = Enums.BTransactionStatus.PENDING
                },
                // BĐSD > 0
                new BTransaction
                {
                    Id = 6,
                    BankAccountId = 1,
                    FromAccountId = 1,
                    Money = 100,
                    TimeAt = DateTime.UtcNow,
                    Status = Enums.BTransactionStatus.PENDING
                },
                // Tổng tiền BĐSD lớn hơn tiền chi lương khả dụng
                new BTransaction
                {
                    Id = 7,
                    BankAccountId = 1,
                    FromAccountId = 1,
                    Money = -100,
                    TimeAt = DateTime.Now,
                    Status = Enums.BTransactionStatus.PENDING
                },
                new BTransaction
                {
                    Id = 8,
                    BankAccountId = 1,
                    FromAccountId = 1,
                    Money = -100,
                    TimeAt = DateTime.UtcNow,
                    Status = Enums.BTransactionStatus.PENDING
                },
                // link 1 BĐSD < Chi lương
                new BTransaction
                {
                    Id = 9,
                    BankAccountId = 1,
                    FromAccountId = 1,
                    Money = -100,
                    TimeAt = DateTime.UtcNow,
                    Status = Enums.BTransactionStatus.PENDING
                },
                // link nhiều BĐSD < chi lương
                new BTransaction
                {
                    Id = 10,
                    BankAccountId = 1,
                    FromAccountId = 1,
                    Money = -20,
                    TimeAt = DateTime.UtcNow,
                    Status = Enums.BTransactionStatus.PENDING
                },
                new BTransaction
                {
                    Id = 11,
                    BankAccountId = 1,
                    FromAccountId = 1,
                    Money = -50,
                    TimeAt = DateTime.UtcNow,
                    Status = Enums.BTransactionStatus.PENDING
                },
                 // link nhiều BĐSD = chi lương
                new BTransaction
                {
                    Id = 12,
                    BankAccountId = 1,
                    FromAccountId = 1,
                    Money = -50,
                    TimeAt = DateTime.UtcNow,
                    Status = Enums.BTransactionStatus.PENDING
                },
                new BTransaction
                {
                    Id = 13,
                    BankAccountId = 1,
                    FromAccountId = 1,
                    Money = -50,
                    TimeAt = DateTime.UtcNow,
                    Status = Enums.BTransactionStatus.PENDING
                },
            };

            // BankTransaction
            var bankTransactions = new List<BankTransaction>()
            {
                // GDNH đã link tơi BĐSD
                new BankTransaction
                {
                    Id = 1,
                    Name = "ABC",
                    FromBankAccountId = 1,
                    ToBankAccountId = 1,
                    FromValue = 100,
                    ToValue = 100,
                    Fee = 1,
                    TransactionDate = DateTime.Now,
                    Note = "ABC",
                    LockedStatus = true,
                    BTransactionId = 1
                },
                new BankTransaction
                {
                    Id = 2,
                    Name = "ABC",
                    FromBankAccountId = 1,
                    ToBankAccountId = 1,
                    FromValue = 100,
                    ToValue = 100,
                    Fee = 1,
                    TransactionDate = DateTime.Now,
                    Note = "ABC",
                    LockedStatus = true,
                    BTransactionId = 2
                },
            };

            // RevenueManaged
            var invoices = new List<Invoice>();

            //Incoming Entry
            var incomings = new List<IncomingEntry>();

            // Outcoming Entry
            var outcomings = new List<OutcomingEntry>()
            {
                // -- LINK ĐẾN MỘT REQUEST CHI --
                // chi lương trạng thái tạo mới
                // nhỏ hơn BĐSD
                new OutcomingEntry
                {
                    Id = 1,
                    PeriodId = 1,
                    Name = "ABC",
                    OutcomingEntryTypeId = 1,
                    Value = 50,
                    WorkflowStatusId = 1,
                    AccountId = 1,
                    BranchId = 1,
                    CurrencyId = 1,
                    PaymentCode = "ABC",
                    Accreditation = false,
                },
                // bằng BĐSD
                new OutcomingEntry
                {
                    Id = 2,
                    PeriodId = 1,
                    Name = "ABC",
                    OutcomingEntryTypeId = 1,
                    Value = 100,
                    WorkflowStatusId = 1,
                    AccountId = 1,
                    BranchId = 1,
                    CurrencyId = 1,
                    PaymentCode = "ABC",
                    Accreditation = false,
                },
                // lớn hơn BĐSD
                new OutcomingEntry
                {
                    Id = 3,
                    PeriodId = 1,
                    Name = "ABC",
                    OutcomingEntryTypeId = 1,
                    Value = 150,
                    WorkflowStatusId = 1,
                    AccountId = 1,
                    BranchId = 1,
                    CurrencyId = 1,
                    PaymentCode = "ABC",
                    Accreditation = false,
                },

                // chi lương trạng thái Approved
                // nhỏ hơn
                new OutcomingEntry
                {
                    Id = 4,
                    PeriodId = 1,
                    Name = "ABC",
                    OutcomingEntryTypeId = 1,
                    Value = 50,
                    WorkflowStatusId = 2,
                    AccountId = 1,
                    BranchId = 1,
                    CurrencyId = 1,
                    PaymentCode = "ABC",
                    Accreditation = false,
                },
                // bằng BĐSD
                new OutcomingEntry
                {
                    Id = 5,
                    PeriodId = 1,
                    Name = "ABC",
                    OutcomingEntryTypeId = 1,
                    Value = 100,
                    WorkflowStatusId = 2,
                    AccountId = 1,
                    BranchId = 1,
                    CurrencyId = 1,
                    PaymentCode = "ABC",
                    Accreditation = false,
                },
                // lớn hơn BĐSD
                new OutcomingEntry
                {
                    Id = 6,
                    PeriodId = 1,
                    Name = "ABC",
                    OutcomingEntryTypeId = 1,
                    Value = 150,
                    WorkflowStatusId = 2,
                    AccountId = 1,
                    BranchId = 1,
                    CurrencyId = 1,
                    PaymentCode = "ABC",
                    Accreditation = false,
                },

                // chi lương trạng thái Thực thi
                // nhỏ hơn BĐSD
                new OutcomingEntry
                {
                    Id = 7,
                    PeriodId = 1,
                    Name = "ABC",
                    OutcomingEntryTypeId = 1,
                    Value = 50,
                    WorkflowStatusId = 3,
                    AccountId = 1,
                    BranchId = 1,
                    CurrencyId = 1,
                    PaymentCode = "ABC",
                    Accreditation = false,
                },
                // bằng BĐSD
                new OutcomingEntry
                {
                    Id = 8,
                    PeriodId = 1,
                    Name = "ABC",
                    OutcomingEntryTypeId = 1,
                    Value = 100,
                    WorkflowStatusId = 3,
                    AccountId = 1,
                    BranchId = 1,
                    CurrencyId = 1,
                    PaymentCode = "ABC",
                    Accreditation = false,
                },
                // lớn hơn BĐSD
                new OutcomingEntry
                {
                    Id = 9,
                    PeriodId = 1,
                    Name = "ABC",
                    OutcomingEntryTypeId = 1,
                    Value = 150,
                    WorkflowStatusId = 3,
                    AccountId = 1,
                    BranchId = 1,
                    CurrencyId = 1,
                    PaymentCode = "ABC",
                    Accreditation = false,
                },

                // chi các khoản khác trạng thái tạo mới
                // nhỏ hơn BĐSD
                new OutcomingEntry
                {
                    Id = 10,
                    PeriodId = 1,
                    Name = "ABC",
                    OutcomingEntryTypeId = 2,
                    Value = 50,
                    WorkflowStatusId = 1,
                    AccountId = 1,
                    BranchId = 1,
                    CurrencyId = 1,
                    PaymentCode = "ABC",
                    Accreditation = false,
                },
                // bằng BĐSD
                new OutcomingEntry
                {
                    Id = 11,
                    PeriodId = 1,
                    Name = "ABC",
                    OutcomingEntryTypeId = 2,
                    Value = 100,
                    WorkflowStatusId = 1,
                    AccountId = 1,
                    BranchId = 1,
                    CurrencyId = 1,
                    PaymentCode = "ABC",
                    Accreditation = false,
                },
                // lớn hơn BĐSD
                new OutcomingEntry
                {
                    Id = 12,
                    PeriodId = 1,
                    Name = "ABC",
                    OutcomingEntryTypeId = 2,
                    Value = 150,
                    WorkflowStatusId = 1,
                    AccountId = 1,
                    BranchId = 1,
                    CurrencyId = 1,
                    PaymentCode = "ABC",
                    Accreditation = false,
                },

                // chi các khoản khác trạng thái Approved
                // nhỏ hớn BĐSD
                new OutcomingEntry
                {
                    Id = 13,
                    PeriodId = 1,
                    Name = "ABC",
                    OutcomingEntryTypeId = 2,
                    Value = 50,
                    WorkflowStatusId = 2,
                    AccountId = 1,
                    BranchId = 1,
                    CurrencyId = 1,
                    PaymentCode = "ABC",
                    Accreditation = false,
                },
                // bằng BĐSD
                new OutcomingEntry
                {
                    Id = 14,
                    PeriodId = 1,
                    Name = "ABC",
                    OutcomingEntryTypeId = 2,
                    Value = 100,
                    WorkflowStatusId = 2,
                    AccountId = 1,
                    BranchId = 1,
                    CurrencyId = 1,
                    PaymentCode = "ABC",
                    Accreditation = false,
                },
                // lớn hớn BĐSD
                new OutcomingEntry
                {
                    Id = 15,
                    PeriodId = 1,
                    Name = "ABC",
                    OutcomingEntryTypeId = 2,
                    Value = 150,
                    WorkflowStatusId = 2,
                    AccountId = 1,
                    BranchId = 1,
                    CurrencyId = 1,
                    PaymentCode = "ABC",
                    Accreditation = false,
                },
                // bằng BĐSD, đã có liên kết tới ghi nhận thu hoặc Giao dịch ngân hàng
                new OutcomingEntry
                {
                    Id = 19,
                    PeriodId = 1,
                    Name = "ABC",
                    OutcomingEntryTypeId = 2,
                    Value = 100,
                    WorkflowStatusId = 2,
                    AccountId = 1,
                    BranchId = 1,
                    CurrencyId = 1,
                    PaymentCode = "ABC",
                    Accreditation = false,
                },
                // BĐSD = tổng tiền các request chi
                new OutcomingEntry
                {
                    Id = 20,
                    PeriodId = 1,
                    Name = "ABC",
                    OutcomingEntryTypeId = 2,
                    Value = 10,
                    WorkflowStatusId = 2,
                    AccountId = 1,
                    BranchId = 1,
                    CurrencyId = 1,
                    PaymentCode = "ABC",
                    Accreditation = false,
                },
                new OutcomingEntry
                {
                    Id = 21,
                    PeriodId = 1,
                    Name = "ABC",
                    OutcomingEntryTypeId = 2,
                    Value = 40,
                    WorkflowStatusId = 2,
                    AccountId = 1,
                    BranchId = 1,
                    CurrencyId = 1,
                    PaymentCode = "ABC",
                    Accreditation = false,
                },
                new OutcomingEntry
                {
                    Id = 22,
                    PeriodId = 1,
                    Name = "ABC",
                    OutcomingEntryTypeId = 2,
                    Value = 50,
                    WorkflowStatusId = 2,
                    AccountId = 1,
                    BranchId = 1,
                    CurrencyId = 1,
                    PaymentCode = "ABC",
                    Accreditation = false,
                },

                // chi các khoản khác trạng thái Thực thi
                // nhỏ hơn BĐSD
                new OutcomingEntry
                {
                    Id = 16,
                    PeriodId = 1,
                    Name = "ABC",
                    OutcomingEntryTypeId = 2,
                    Value = 50,
                    WorkflowStatusId = 3,
                    AccountId = 1,
                    BranchId = 1,
                    CurrencyId = 1,
                    PaymentCode = "ABC",
                    Accreditation = false,
                },
                // bằng BĐSD
                new OutcomingEntry
                {
                    Id = 17,
                    PeriodId = 1,
                    Name = "ABC",
                    OutcomingEntryTypeId = 2,
                    Value = 100,
                    WorkflowStatusId = 3,
                    AccountId = 1,
                    BranchId = 1,
                    CurrencyId = 1,
                    PaymentCode = "ABC",
                    Accreditation = false,
                },
                // lớn hơn BĐSD
                new OutcomingEntry
                {
                    Id = 18,
                    PeriodId = 1,
                    Name = "ABC",
                    OutcomingEntryTypeId = 2,
                    Value = 150,
                    WorkflowStatusId = 3,
                    AccountId = 1,
                    BranchId = 1,
                    CurrencyId = 1,
                    PaymentCode = "ABC",
                    Accreditation = false,
                },
            };

            // Outcoming Entry Detail
            var outcomingDetails = new List<OutcomingEntryDetail>()
            {
                // khác trạng thái Approved
                new OutcomingEntryDetail
                {
                    Id = 1,
                    Name = "ABC",
                    Quantity = 1,
                    UnitPrice = 50,
                    Total = 50,
                    OutcomingEntryId = 10,
                },
                new OutcomingEntryDetail
                {
                    Id = 2,
                    Name = "ABC",
                    Quantity = 1,
                    UnitPrice = 100,
                    Total = 100,
                    OutcomingEntryId = 11,
                },

                // có tổng tiền detail khác với value
                new OutcomingEntryDetail
                {
                    Id = 3,
                    Name = "ABC",
                    Quantity = 1,
                    UnitPrice = 100,
                    Total = 100,
                    OutcomingEntryId = 12,
                },
                new OutcomingEntryDetail
                {
                    Id = 4,
                    Name = "ABC",
                    Quantity = 1,
                    UnitPrice = 150,
                    Total = 150,
                    OutcomingEntryId = 13,
                },

                // chênh lêch tiền
                new OutcomingEntryDetail
                {
                    Id = 5,
                    Name = "ABC",
                    Quantity = 1,
                    UnitPrice = 150,
                    Total = 150,
                    OutcomingEntryId = 15,
                },

                new OutcomingEntryDetail
                {
                    Id = 7,
                    Name = "ABC",
                    Quantity = 1,
                    UnitPrice = 100,
                    Total = 100,
                    OutcomingEntryId = 19,
                },

                new OutcomingEntryDetail
                {
                    Id = 8,
                    Name = "ABC",
                    Quantity = 1,
                    UnitPrice = 10,
                    Total = 10,
                    OutcomingEntryId = 20,
                },
                new OutcomingEntryDetail
                {
                    Id = 9,
                    Name = "ABC",
                    Quantity = 2,
                    UnitPrice = 20,
                    Total = 40,
                    OutcomingEntryId = 21,
                },
                new OutcomingEntryDetail
                {
                    Id = 10,
                    Name = "ABC",
                    Quantity = 5,
                    UnitPrice = 10,
                    Total = 50,
                    OutcomingEntryId = 22,
                },
            };


            // Outcoming Entry BankTransaction
            var outcomingBankTransactions = new List<OutcomingEntryBankTransaction>()
            {
                // đã có giao dịch ngân hàng
                new OutcomingEntryBankTransaction
                {
                    Id = 1,
                    BankTransactionId = 1,
                    OutcomingEntryId = 19,
                    IsDeleted = false,
                }
            };

            // Relation InOutEntry
            var relationInOutEntrys = new List<RelationInOutEntry>()
            {
                // đã ghi nhận thu
                new RelationInOutEntry
                {
                    Id = 1,
                    OutcomingEntryId = 1,
                    IncomingEntryId = 19,
                    IsDeleted = false,
                }
            };

            // -- KHÁCH HÀNG THANH TOÁN --
            #region Case 1: BTransaction nhận thiếu tiền cho 1 invoice, cùng currency
            bTransactions.Add(new BTransaction
            {
                Id = 14,
                BankAccountId = 1,
                FromAccountId = 1,
                Money = 50,
                TimeAt = DateTime.UtcNow,
                Status = Enums.BTransactionStatus.PENDING
            });
            invoices.Add(new Invoice
            {
                Id = 1,
                AccountId = 1,
                Status = NInvoiceStatus.CHUA_TRA,
                InvoiceNumber = "CCC",
                CollectionDebt = 100,
                Deadline = DateTime.Now,
                CurrencyId = 1,
                Month = 10,
                Year = 2022,
                NameInvoice = "Thu thang 10"
            });
            #endregion

            #region Case 2: BTransaction nhận đủ tiền cho 1 invoice, cùng currency
            bTransactions.Add(new BTransaction
            {
                Id = 15,
                BankAccountId = 1,
                FromAccountId = 2,
                Money = 100,
                TimeAt = DateTime.UtcNow,
                Status = Enums.BTransactionStatus.PENDING
            });
            invoices.Add(new Invoice
            {
                Id = 2,
                AccountId = 2,
                Status = NInvoiceStatus.CHUA_TRA,
                InvoiceNumber = "CCC",
                CollectionDebt = 100,
                Deadline = DateTime.Now,
                CurrencyId = 1,
                Month = 10,
                Year = 2022,
                NameInvoice = "Thu thang 10"
            });
            #endregion

            #region Case 3: BTransaction nhận thừa tiền cho 1 invoice, cùng currency
            bTransactions.Add(new BTransaction
            {
                Id = 16,
                BankAccountId = 1,
                FromAccountId = 3,
                Money = 150,
                TimeAt = DateTime.UtcNow,
                Status = Enums.BTransactionStatus.PENDING
            });
            invoices.Add(new Invoice
            {
                Id = 3,
                AccountId = 3,
                Status = NInvoiceStatus.CHUA_TRA,
                InvoiceNumber = "CCC",
                CollectionDebt = 100,
                Deadline = DateTime.Now,
                CurrencyId = 1,
                Month = 10,
                Year = 2022,
                NameInvoice = "Thu thang 10"
            });
            #endregion

            #region Case 4: BTransaction nhận thiếu tiền cho 1 invoice, khác currency
            bTransactions.Add(new BTransaction
            {
                Id = 17,
                BankAccountId = 2,
                FromAccountId = 4,
                Money = 50,
                TimeAt = DateTime.UtcNow,
                Status = Enums.BTransactionStatus.PENDING
            });
            invoices.Add(new Invoice
            {
                Id = 4,
                AccountId = 4,
                Status = NInvoiceStatus.CHUA_TRA,
                InvoiceNumber = "CCC",
                CollectionDebt = 150,
                Deadline = DateTime.Now,
                CurrencyId = 1,
                Month = 10,
                Year = 2022,
                NameInvoice = "Thu thang 10"
            });
            #endregion

            #region Case 5: BTransaction nhận đủ tiền cho 1 invoice, khác currency
            bTransactions.Add(new BTransaction
            {
                Id = 18,
                BankAccountId = 2,
                FromAccountId = 5,
                Money = 50,
                TimeAt = DateTime.UtcNow,
                Status = Enums.BTransactionStatus.PENDING
            });
            invoices.Add(new Invoice
            {
                Id = 5,
                AccountId = 5,
                Status = NInvoiceStatus.CHUA_TRA,
                InvoiceNumber = "CCC",
                CollectionDebt = 100,
                Deadline = DateTime.Now,
                CurrencyId = 1,
                Month = 10,
                Year = 2022,
                NameInvoice = "Thu thang 10"
            });
            #endregion

            #region Case 6: BTransaction nhận thừa tiền cho 1 invoice, khác currency
            bTransactions.Add(new BTransaction
            {
                Id = 19,
                BankAccountId = 2,
                FromAccountId = 6,
                Money = 100,
                TimeAt = DateTime.UtcNow,
                Status = Enums.BTransactionStatus.PENDING
            });
            invoices.Add(new Invoice
            {
                Id = 6,
                AccountId = 6,
                Status = NInvoiceStatus.CHUA_TRA,
                InvoiceNumber = "CCC",
                CollectionDebt = 100,
                Deadline = DateTime.Now,
                CurrencyId = 1,
                Month = 10,
                Year = 2022,
                NameInvoice = "Thu thang 10"
            });
            #endregion

            #region Case 7: BTransaction nhận thiếu tiền cho 2 invoice, cùng currency (thiếu cả invoice 1 và 2)
            bTransactions.Add(new BTransaction
            {
                Id = 20,
                BankAccountId = 1,
                FromAccountId = 7,
                Money = 50,
                TimeAt = DateTime.UtcNow,
                Status = Enums.BTransactionStatus.PENDING
            });
            invoices.Add(new Invoice
            {
                Id = 7,
                AccountId = 7,
                Status = NInvoiceStatus.CHUA_TRA,
                InvoiceNumber = "CC",
                CollectionDebt = 100,
                Deadline = DateTime.Now,
                CurrencyId = 1,
                Month = 9,
                Year = 2022,
                NameInvoice = "Thu thang 9"
            });
            invoices.Add(new Invoice
            {
                Id = 8,
                AccountId = 7,
                Status = NInvoiceStatus.CHUA_TRA,
                InvoiceNumber = "CCC",
                CollectionDebt = 100,
                Deadline = DateTime.Now,
                CurrencyId = 1,
                Month = 10,
                Year = 2022,
                NameInvoice = "Thu thang 10"
            });
            #endregion

            #region Case 8: BTransaction nhận thiếu tiền cho 2 invoice, cùng currency (chỉ đủ 1, thiếu 2)
            bTransactions.Add(new BTransaction
            {
                Id = 21,
                BankAccountId = 1,
                FromAccountId = 8,
                Money = 100,
                TimeAt = DateTime.UtcNow,
                Status = Enums.BTransactionStatus.PENDING
            });
            invoices.Add(new Invoice
            {
                Id = 9,
                AccountId = 8,
                Status = NInvoiceStatus.CHUA_TRA,
                InvoiceNumber = "CC",
                CollectionDebt = 100,
                Deadline = DateTime.Now,
                CurrencyId = 1,
                Month = 9,
                Year = 2022,
                NameInvoice = "Thu thang 9"
            });
            invoices.Add(new Invoice
            {
                Id = 10,
                AccountId = 8,
                Status = NInvoiceStatus.CHUA_TRA,
                InvoiceNumber = "CCC",
                CollectionDebt = 100,
                Deadline = DateTime.Now,
                CurrencyId = 1,
                Month = 10,
                Year = 2022,
                NameInvoice = "Thu thang 10"
            });
            #endregion

            #region Case 9: BTransaction nhận thiếu tiền cho 2 invoice, cùng currency (đủ 1, trả 1 phần 2)
            bTransactions.Add(new BTransaction
            {
                Id = 22,
                BankAccountId = 1,
                FromAccountId = 9,
                Money = 150,
                TimeAt = DateTime.UtcNow,
                Status = Enums.BTransactionStatus.PENDING
            });
            invoices.Add(new Invoice
            {
                Id = 11,
                AccountId = 9,
                Status = NInvoiceStatus.CHUA_TRA,
                InvoiceNumber = "CC",
                CollectionDebt = 100,
                Deadline = DateTime.Now,
                CurrencyId = 1,
                Month = 9,
                Year = 2022,
                NameInvoice = "Thu thang 9"
            });
            invoices.Add(new Invoice
            {
                Id = 12,
                AccountId = 9,
                Status = NInvoiceStatus.CHUA_TRA,
                InvoiceNumber = "CCC",
                CollectionDebt = 100,
                Deadline = DateTime.Now,
                CurrencyId = 1,
                Month = 10,
                Year = 2022,
                NameInvoice = "Thu thang 10"
            });
            #endregion

            #region Case 10: BTransaction nhận đủ tiền cho 2 invoice, cùng currency
            bTransactions.Add(new BTransaction
            {
                Id = 23,
                BankAccountId = 1,
                FromAccountId = 10,
                Money = 150,
                TimeAt = DateTime.UtcNow,
                Status = Enums.BTransactionStatus.PENDING
            });
            invoices.Add(new Invoice
            {
                Id = 13,
                AccountId = 10,
                Status = NInvoiceStatus.CHUA_TRA,
                InvoiceNumber = "CC",
                CollectionDebt = 100,
                Deadline = DateTime.Now,
                CurrencyId = 1,
                Month = 9,
                Year = 2022,
                NameInvoice = "Thu thang 9"
            });
            invoices.Add(new Invoice
            {
                Id = 14,
                AccountId = 10,
                Status = NInvoiceStatus.CHUA_TRA,
                InvoiceNumber = "CCC",
                CollectionDebt = 50,
                Deadline = DateTime.Now,
                CurrencyId = 1,
                Month = 10,
                Year = 2022,
                NameInvoice = "Thu thang 10"
            });
            #endregion

            #region Case 11: BTransaction nhận thừa tiền cho 2 invoice, cùng currency
            bTransactions.Add(new BTransaction
            {
                Id = 24,
                BankAccountId = 1,
                FromAccountId = 11,
                Money = 150,
                TimeAt = DateTime.UtcNow,
                Status = Enums.BTransactionStatus.PENDING
            });
            invoices.Add(new Invoice
            {
                Id = 15,
                AccountId = 11,
                Status = NInvoiceStatus.CHUA_TRA,
                InvoiceNumber = "CC",
                CollectionDebt = 50,
                Deadline = DateTime.Now,
                CurrencyId = 1,
                Month = 9,
                Year = 2022,
                NameInvoice = "Thu thang 9"
            });
            invoices.Add(new Invoice
            {
                Id = 16,
                AccountId = 11,
                Status = NInvoiceStatus.CHUA_TRA,
                InvoiceNumber = "CCC",
                CollectionDebt = 50,
                Deadline = DateTime.Now,
                CurrencyId = 1,
                Month = 10,
                Year = 2022,
                NameInvoice = "Thu thang 10"
            });
            #endregion

            #region Case 12: BTransaction nhận thiếu tiền cho 2 invoice, khác currency (thiếu cả invoice 1 và 2)
            bTransactions.Add(new BTransaction
            {
                Id = 25,
                BankAccountId = 2,
                FromAccountId = 12,
                Money = 25,
                TimeAt = DateTime.UtcNow,
                Status = Enums.BTransactionStatus.PENDING
            });
            invoices.Add(new Invoice
            {
                Id = 17,
                AccountId = 12,
                Status = NInvoiceStatus.CHUA_TRA,
                InvoiceNumber = "CC",
                CollectionDebt = 100,
                Deadline = DateTime.Now,
                CurrencyId = 1,
                Month = 9,
                Year = 2022,
                NameInvoice = "Thu thang 9"
            });
            invoices.Add(new Invoice
            {
                Id = 18,
                AccountId = 12,
                Status = NInvoiceStatus.CHUA_TRA,
                InvoiceNumber = "CCC",
                CollectionDebt = 100,
                Deadline = DateTime.Now,
                CurrencyId = 1,
                Month = 10,
                Year = 2022,
                NameInvoice = "Thu thang 10"
            });
            #endregion

            #region Case 13: BTransaction nhận thiếu tiền cho 2 invoice, khác currency (chỉ đủ 1, thiếu 2)
            bTransactions.Add(new BTransaction
            {
                Id = 26,
                BankAccountId = 2,
                FromAccountId = 13,
                Money = 50,
                TimeAt = DateTime.UtcNow,
                Status = Enums.BTransactionStatus.PENDING
            });
            invoices.Add(new Invoice
            {
                Id = 19,
                AccountId = 13,
                Status = NInvoiceStatus.CHUA_TRA,
                InvoiceNumber = "CC",
                CollectionDebt = 100,
                Deadline = DateTime.Now,
                CurrencyId = 1,
                Month = 9,
                Year = 2022,
                NameInvoice = "Thu thang 9"
            });
            invoices.Add(new Invoice
            {
                Id = 20,
                AccountId = 13,
                Status = NInvoiceStatus.CHUA_TRA,
                InvoiceNumber = "CCC",
                CollectionDebt = 100,
                Deadline = DateTime.Now,
                CurrencyId = 1,
                Month = 10,
                Year = 2022,
                NameInvoice = "Thu thang 10"
            });
            #endregion

            #region Case 14: BTransaction nhận thiếu tiền cho 2 invoice, khác currency (đủ 1, trả 1 phần 2)
            bTransactions.Add(new BTransaction
            {
                Id = 27,
                BankAccountId = 2,
                FromAccountId = 14,
                Money = 75,
                TimeAt = DateTime.UtcNow,
                Status = Enums.BTransactionStatus.PENDING
            });
            invoices.Add(new Invoice
            {
                Id = 21,
                AccountId = 14,
                Status = NInvoiceStatus.CHUA_TRA,
                InvoiceNumber = "CC",
                CollectionDebt = 100,
                Deadline = DateTime.Now,
                CurrencyId = 1,
                Month = 9,
                Year = 2022,
                NameInvoice = "Thu thang 9"
            });
            invoices.Add(new Invoice
            {
                Id = 22,
                AccountId = 14,
                Status = NInvoiceStatus.CHUA_TRA,
                InvoiceNumber = "CCC",
                CollectionDebt = 100,
                Deadline = DateTime.Now,
                CurrencyId = 1,
                Month = 10,
                Year = 2022,
                NameInvoice = "Thu thang 10"
            });
            #endregion

            #region Case 15: BTransaction nhận đủ tiền cho 2 invoice, khác currency
            bTransactions.Add(new BTransaction
            {
                Id = 28,
                BankAccountId = 2,
                FromAccountId = 15,
                Money = 75,
                TimeAt = DateTime.UtcNow,
                Status = Enums.BTransactionStatus.PENDING
            });
            invoices.Add(new Invoice
            {
                Id = 23,
                AccountId = 15,
                Status = NInvoiceStatus.CHUA_TRA,
                InvoiceNumber = "CC",
                CollectionDebt = 100,
                Deadline = DateTime.Now,
                CurrencyId = 1,
                Month = 9,
                Year = 2022,
                NameInvoice = "Thu thang 9"
            });
            invoices.Add(new Invoice
            {
                Id = 24,
                AccountId = 15,
                Status = NInvoiceStatus.CHUA_TRA,
                InvoiceNumber = "CCC",
                CollectionDebt = 50,
                Deadline = DateTime.Now,
                CurrencyId = 1,
                Month = 10,
                Year = 2022,
                NameInvoice = "Thu thang 10"
            });
            #endregion

            #region Case 16: BTransaction nhận thừa tiền cho 2 invoice, khác currency
            bTransactions.Add(new BTransaction
            {
                Id = 29,
                BankAccountId = 2,
                FromAccountId = 16,
                Money = 100,
                TimeAt = DateTime.UtcNow,
                Status = Enums.BTransactionStatus.PENDING
            });
            invoices.Add(new Invoice
            {
                Id = 25,
                AccountId = 16,
                Status = NInvoiceStatus.CHUA_TRA,
                InvoiceNumber = "CC",
                CollectionDebt = 50,
                Deadline = DateTime.Now,
                CurrencyId = 1,
                Month = 9,
                Year = 2022,
                NameInvoice = "Thu thang 9"
            });
            invoices.Add(new Invoice
            {
                Id = 26,
                AccountId = 16,
                Status = NInvoiceStatus.CHUA_TRA,
                InvoiceNumber = "CCC",
                CollectionDebt = 50,
                Deadline = DateTime.Now,
                CurrencyId = 1,
                Month = 10,
                Year = 2022,
                NameInvoice = "Thu thang 10"
            });
            #endregion

            // -- AUTO TRẢ NỢ
            #region Case 1: Khoản tiền thừa của khách hàng < số tiền nợ của khách hàng, cùng currency
            bTransactions.Add(new BTransaction
            {
                Id = 30,
                BankAccountId = 1,
                FromAccountId = 17,
                Money = 100,
                TimeAt = DateTime.UtcNow,
                Status = Enums.BTransactionStatus.DONE
            });
            incomings.Add(new IncomingEntry
            {
                Id = 1,
                AccountId = 17,
                IncomingEntryTypeId = 1,
                BankTransactionId = 1,
                BTransactionId = 30,
                CurrencyId = 1,
                Value = 100,
                InvoiceId = null,
            });
            invoices.Add(new Invoice
            {
                Id = 27,
                AccountId = 17,
                Status = NInvoiceStatus.CHUA_TRA,
                InvoiceNumber = "CCC",
                CollectionDebt = 150,
                Deadline = DateTime.Now,
                CurrencyId = 1,
                Month = 10,
                Year = 2022,
                NameInvoice = "Thu thang 10"
            });
            #endregion

            #region Case 2: Khoản tiền thừa của khách hàng = số tiền nợ của khách hàng, cùng currency
            bTransactions.Add(new BTransaction
            {
                Id = 31,
                BankAccountId = 1,
                FromAccountId = 18,
                Money = 100,
                TimeAt = DateTime.UtcNow,
                Status = Enums.BTransactionStatus.DONE
            });
            incomings.Add(new IncomingEntry
            {
                Id = 2,
                AccountId = 18,
                IncomingEntryTypeId = 1,
                BankTransactionId = 1,
                BTransactionId = 31,
                CurrencyId = 1,
                Value = 100,
                InvoiceId = null,
            });
            invoices.Add(new Invoice
            {
                Id = 28,
                AccountId = 18,
                Status = NInvoiceStatus.CHUA_TRA,
                InvoiceNumber = "CCC",
                CollectionDebt = 100,
                Deadline = DateTime.Now,
                CurrencyId = 1,
                Month = 10,
                Year = 2022,
                NameInvoice = "Thu thang 10"
            });
            #endregion

            #region Case 3: Khoản tiền thừa của khách hàng > số tiền nợ của khách hàng, cùng currency
            bTransactions.Add(new BTransaction
            {
                Id = 32,
                BankAccountId = 1,
                FromAccountId = 19,
                Money = 150,
                TimeAt = DateTime.UtcNow,
                Status = Enums.BTransactionStatus.DONE
            });
            incomings.Add(new IncomingEntry
            {
                Id = 3,
                AccountId = 19,
                IncomingEntryTypeId = 1,
                BankTransactionId = 1,
                BTransactionId = 32,
                CurrencyId = 1,
                Value = 150,
                InvoiceId = null,
            });
            invoices.Add(new Invoice
            {
                Id = 29,
                AccountId = 19,
                Status = NInvoiceStatus.CHUA_TRA,
                InvoiceNumber = "CCC",
                CollectionDebt = 100,
                Deadline = DateTime.Now,
                CurrencyId = 1,
                Month = 10,
                Year = 2022,
                NameInvoice = "Thu thang 10"
            });
            #endregion

            #region Case 4: Khoản tiền thừa của khách hàng < số tiền nợ của khách hàng, khác currency
            bTransactions.Add(new BTransaction
            {
                Id = 33,
                BankAccountId = 2,
                FromAccountId = 20,
                Money = 50,
                TimeAt = DateTime.UtcNow,
                Status = Enums.BTransactionStatus.DONE
            });
            incomings.Add(new IncomingEntry
            {
                Id = 4,
                AccountId = 20,
                IncomingEntryTypeId = 1,
                BankTransactionId = 1,
                BTransactionId = 33,
                CurrencyId = 2,
                Value = 50,
                InvoiceId = null,
            });
            invoices.Add(new Invoice
            {
                Id = 30,
                AccountId = 20,
                Status = NInvoiceStatus.CHUA_TRA,
                InvoiceNumber = "CCC",
                CollectionDebt = 150,
                Deadline = DateTime.Now,
                CurrencyId = 1,
                Month = 10,
                Year = 2022,
                NameInvoice = "Thu thang 10"
            });
            #endregion

            #region Case 5: Khoản tiền thừa của khách hàng = số tiền nợ của khách hàng, khác currency
            bTransactions.Add(new BTransaction
            {
                Id = 34,
                BankAccountId = 2,
                FromAccountId = 21,
                Money = 50,
                TimeAt = DateTime.UtcNow,
                Status = Enums.BTransactionStatus.DONE
            });
            incomings.Add(new IncomingEntry
            {
                Id = 5,
                AccountId = 21,
                IncomingEntryTypeId = 1,
                BankTransactionId = 1,
                BTransactionId = 34,
                CurrencyId = 2,
                Value = 50,
                InvoiceId = null,
            });
            invoices.Add(new Invoice
            {
                Id = 31,
                AccountId = 21,
                Status = NInvoiceStatus.CHUA_TRA,
                InvoiceNumber = "CCC",
                CollectionDebt = 100,
                Deadline = DateTime.Now,
                CurrencyId = 1,
                Month = 10,
                Year = 2022,
                NameInvoice = "Thu thang 10"
            });
            #endregion

            #region Case 6: Khoản tiền thừa của khách hàng > số tiền nợ của khách hàng, khác currency
            bTransactions.Add(new BTransaction
            {
                Id = 35,
                BankAccountId = 2,
                FromAccountId = 22,
                Money = 150,
                TimeAt = DateTime.UtcNow,
                Status = Enums.BTransactionStatus.DONE
            });
            incomings.Add(new IncomingEntry
            {
                Id = 6,
                AccountId = 22,
                IncomingEntryTypeId = 1,
                BankTransactionId = 1,
                BTransactionId = 35,
                CurrencyId = 2,
                Value = 150,
                InvoiceId = null,
            });
            invoices.Add(new Invoice
            {
                Id = 32,
                AccountId = 22,
                Status = NInvoiceStatus.CHUA_TRA,
                InvoiceNumber = "CCC",
                CollectionDebt = 100,
                Deadline = DateTime.Now,
                CurrencyId = 1,
                Month = 10,
                Year = 2022,
                NameInvoice = "Thu thang 10"
            });
            #endregion

            // Khách hàng trả kênh tiền
            bTransactions.Add(new BTransaction
            {
                Id = 36,
                BankAccountId = 1,
                FromAccountId = 23,
                Money = 150,
                TimeAt = DateTime.UtcNow,
                Status = Enums.BTransactionStatus.DONE
            });
            incomings.Add(new IncomingEntry
            {
                Id = 7,
                AccountId = 23,
                IncomingEntryTypeId = 1,
                BankTransactionId = 1,
                BTransactionId = 36,
                CurrencyId = 1,
                Value = 150,
                InvoiceId = null,
                Name = "ABC"
            });
            // -- BÁN NGOẠI TỆ --
            #region Case 1: DONE
            bTransactions.Add(new BTransaction
            {
                Id = 100,
                BankAccountId = 2,
                FromAccountId = 2,
                Money = -100,
                TimeAt = DateTime.UtcNow,
                Status = Enums.BTransactionStatus.PENDING
            });
            bTransactions.Add(new BTransaction
            {
                Id = 101,
                BankAccountId = 1,
                FromAccountId = 1,
                Money = 200,
                TimeAt = DateTime.UtcNow,
                Status = Enums.BTransactionStatus.PENDING
            });
            bTransactions.Add(new BTransaction
            {
                Id = 102,
                BankAccountId = 2,
                FromAccountId = 2,
                Money = -100,
                TimeAt = DateTime.UtcNow,
                Status = Enums.BTransactionStatus.DONE
            });
            bTransactions.Add(new BTransaction
            {
                Id = 103,
                BankAccountId = 102,
                FromAccountId = 104,
                Money = -100,
                TimeAt = DateTime.UtcNow,
                Status = Enums.BTransactionStatus.PENDING
            });
            bTransactions.Add(new BTransaction
            {
                Id = 104,
                BankAccountId = 104,
                FromAccountId = 102,
                Money = 200,
                TimeAt = DateTime.UtcNow,
                Status = Enums.BTransactionStatus.PENDING
            });
            bTransactions.Add(new BTransaction
            {
                Id = 105,
                BankAccountId = 104,
                FromAccountId = 102,
                Money = -100,
                TimeAt = DateTime.UtcNow,
                Status = Enums.BTransactionStatus.PENDING
            });
            bTransactions.Add(new BTransaction
            {
                Id = 106,
                BankAccountId = 102,
                FromAccountId = 104,
                Money = 200,
                TimeAt = DateTime.UtcNow,
                Status = Enums.BTransactionStatus.PENDING
            });
            bTransactions.Add(new BTransaction
            {
                Id = 107,
                BankAccountId = 3,
                FromAccountId = 3,
                Money = -100,
                TimeAt = DateTime.UtcNow,
                Status = Enums.BTransactionStatus.PENDING
            });
            bTransactions.Add(new BTransaction
            {
                Id = 108,
                BankAccountId = 4,
                FromAccountId = 4,
                Money = -100,
                TimeAt = DateTime.UtcNow,
                Status = Enums.BTransactionStatus.PENDING
            });
            bTransactions.Add(new BTransaction
            {
                Id = 109,
                BankAccountId = 5,
                FromAccountId = 5,
                Money = 200,
                TimeAt = DateTime.UtcNow,
                Status = Enums.BTransactionStatus.PENDING
            });
            bTransactions.Add(new BTransaction
            {
                Id = 110,
                BankAccountId = 6,
                FromAccountId = 6,
                Money = 200,
                TimeAt = DateTime.UtcNow,
                Status = Enums.BTransactionStatus.PENDING
            });
            outcomings.Add(new OutcomingEntry
            {
                Id = 100,
                PeriodId = 1,
                Name = "ABC",
                OutcomingEntryTypeId = 2,
                Value = 200,
                WorkflowStatusId = 2,
                AccountId = 1,
                BranchId = 1,
                CurrencyId = 1,
                PaymentCode = "ABC",
                Accreditation = false,
            });
            outcomingDetails.Add(new OutcomingEntryDetail
            {
                Id = 100,
                Name = "ABC",
                Quantity = 1,
                UnitPrice = 200,
                Total = 200,
                OutcomingEntryId = 100,
            });
            incomings.Add(new IncomingEntry
            {
                Id = 100,
                AccountId = 1,
                CurrencyId = 1,
                ExchangeRate = 1,
                IncomingEntryTypeId = 2,
                BTransactionId = 100,
                Value = 200,
                BankTransactionId = 1,
            });
            outcomings.Add(new OutcomingEntry
            {
                Id = 140,
                PeriodId = 1,
                Name = "ABC",
                OutcomingEntryTypeId = 2,
                Value = 700,
                WorkflowStatusId = 2,
                AccountId = 1,
                BranchId = 1,
                CurrencyId = 1,
                PaymentCode = "ABC",
                Accreditation = false,
            });
            outcomingDetails.Add(new OutcomingEntryDetail
            {
                Id = 140,
                Name = "ABC",
                Quantity = 1,
                UnitPrice = 700,
                Total = 700,
                OutcomingEntryId = 140,
            });
            incomings.Add(new IncomingEntry
            {
                Id = 140,
                AccountId = 1,
                CurrencyId = 1,
                ExchangeRate = 1,
                IncomingEntryTypeId = 2,
                BTransactionId = 100,
                Value = 700,
                BankTransactionId = 1,
            });
            bTransactions.Add(new BTransaction
            {
                Id = 130,
                BankAccountId = 1,
                FromAccountId = 101,
                Money = 300,
                TimeAt = DateTime.UtcNow,
                Status = Enums.BTransactionStatus.PENDING
            });
            outcomings.Add(new OutcomingEntry
            {
                Id = 111,
                PeriodId = 1,
                Name = "ABC",
                OutcomingEntryTypeId = 2,
                Value = 200,
                WorkflowStatusId = 2,
                AccountId = 1,
                BranchId = 1,
                CurrencyId = 1,
                PaymentCode = "ABC",
                Accreditation = false,
            });
            outcomingDetails.Add(new OutcomingEntryDetail
            {
                Id = 111,
                Name = "ABC",
                Quantity = 1,
                UnitPrice = 200,
                Total = 200,
                OutcomingEntryId = 111,
            });
            #endregion

            // -- MUA NGOẠI TỆ --
            #region Case 1: DONE
            bTransactions.Add(new BTransaction
            {
                Id = 112,
                BankAccountId = 1,
                FromAccountId = 1,
                Money = -100,
                TimeAt = DateTime.UtcNow,
                Status = Enums.BTransactionStatus.PENDING
            });
            bTransactions.Add(new BTransaction
            {
                Id = 111,
                BankAccountId = 2,
                FromAccountId = 2,
                Money = 200,
                TimeAt = DateTime.UtcNow,
                Status = Enums.BTransactionStatus.PENDING
            });
            bTransactions.Add(new BTransaction
            {
                Id = 113,
                BankAccountId = 2,
                FromAccountId = 2,
                Money = -100,
                TimeAt = DateTime.UtcNow,
                Status = Enums.BTransactionStatus.DONE
            });
            bTransactions.Add(new BTransaction
            {
                Id = 114,
                BankAccountId = 102,
                FromAccountId = 104,
                Money = -100,
                TimeAt = DateTime.UtcNow,
                Status = Enums.BTransactionStatus.PENDING
            });
            bTransactions.Add(new BTransaction
            {
                Id = 115,
                BankAccountId = 104,
                FromAccountId = 102,
                Money = 200,
                TimeAt = DateTime.UtcNow,
                Status = Enums.BTransactionStatus.PENDING
            });
            bTransactions.Add(new BTransaction
            {
                Id = 116,
                BankAccountId = 104,
                FromAccountId = 102,
                Money = -100,
                TimeAt = DateTime.UtcNow,
                Status = Enums.BTransactionStatus.PENDING
            });
            bTransactions.Add(new BTransaction
            {
                Id = 117,
                BankAccountId = 102,
                FromAccountId = 104,
                Money = 200,
                TimeAt = DateTime.UtcNow,
                Status = Enums.BTransactionStatus.PENDING
            });
            bTransactions.Add(new BTransaction
            {
                Id = 118,
                BankAccountId = 3,
                FromAccountId = 3,
                Money = -100,
                TimeAt = DateTime.UtcNow,
                Status = Enums.BTransactionStatus.PENDING
            });
            bTransactions.Add(new BTransaction
            {
                Id = 119,
                BankAccountId = 4,
                FromAccountId = 4,
                Money = -100,
                TimeAt = DateTime.UtcNow,
                Status = Enums.BTransactionStatus.PENDING
            });
            bTransactions.Add(new BTransaction
            {
                Id = 120,
                BankAccountId = 5,
                FromAccountId = 5,
                Money = 200,
                TimeAt = DateTime.UtcNow,
                Status = Enums.BTransactionStatus.PENDING
            });
            bTransactions.Add(new BTransaction
            {
                Id = 121,
                BankAccountId = 6,
                FromAccountId = 6,
                Money = 200,
                TimeAt = DateTime.UtcNow,
                Status = Enums.BTransactionStatus.PENDING
            });
            outcomings.Add(new OutcomingEntry
            {
                Id = 101,
                PeriodId = 1,
                Name = "ABC",
                OutcomingEntryTypeId = 2,
                Value = 200,
                WorkflowStatusId = 2,
                AccountId = 1,
                BranchId = 1,
                CurrencyId = 1,
                PaymentCode = "ABC",
                Accreditation = false,
            });
            outcomingDetails.Add(new OutcomingEntryDetail
            {
                Id = 101,
                Name = "ABC",
                Quantity = 1,
                UnitPrice = 200,
                Total = 200,
                OutcomingEntryId = 101,
            });
            incomings.Add(new IncomingEntry
            {
                Id = 101,
                AccountId = 1,
                CurrencyId = 1,
                ExchangeRate = 1,
                IncomingEntryTypeId = 2,
                BTransactionId = 112,
                Value = 200,
                BankTransactionId = 1,
            });
            bTransactions.Add(new BTransaction
            {
                Id = 131,
                BankAccountId = 1,
                FromAccountId = 101,
                Money = -300,
                TimeAt = DateTime.UtcNow,
                Status = Enums.BTransactionStatus.PENDING
            });
            outcomings.Add(new OutcomingEntry
            {
                Id = 112,
                PeriodId = 1,
                Name = "ABC",
                OutcomingEntryTypeId = 2,
                Value = 200,
                WorkflowStatusId = 2,
                AccountId = 1,
                BranchId = 1,
                CurrencyId = 1,
                PaymentCode = "ABC",
                Accreditation = false,
            });
            outcomingDetails.Add(new OutcomingEntryDetail
            {
                Id = 112,
                Name = "ABC",
                Quantity = 1,
                UnitPrice = 200,
                Total = 200,
                OutcomingEntryId = 112,
            });
            outcomings.Add(new OutcomingEntry
            {
                Id = 141,
                PeriodId = 1,
                Name = "ABC",
                OutcomingEntryTypeId = 2,
                Value = 700,
                WorkflowStatusId = 2,
                AccountId = 1,
                BranchId = 1,
                CurrencyId = 1,
                PaymentCode = "ABC",
                Accreditation = false,
            });
            outcomingDetails.Add(new OutcomingEntryDetail
            {
                Id = 141,
                Name = "ABC",
                Quantity = 1,
                UnitPrice = 700,
                Total = 700,
                OutcomingEntryId = 141,
            });
            incomings.Add(new IncomingEntry
            {
                Id = 141,
                AccountId = 1,
                CurrencyId = 1,
                ExchangeRate = 1,
                IncomingEntryTypeId = 2,
                BTransactionId = 112,
                Value = 700,
                BankTransactionId = 1,
            });
            #endregion

            context.IncomingEntryTypes.AddRange(incomingEntryTypes);
            context.IncomingEntries.AddRange(incomings);
            context.Currencies.AddRange(currencies);
            context.AccountTypes.AddRange(accountTypes);
            context.OutcomingEntryTypes.AddRange(outcomingEntryTypes);
            context.WorkflowStatuses.AddRange(workflowStatus);
            context.Banks.AddRange(banks);
            context.Accounts.AddRange(accounts);
            context.BankAccounts.AddRange(bankAccounts);
            context.BTransactions.AddRange(bTransactions);
            context.BankTransactions.AddRange(bankTransactions);
            context.Invoices.AddRange(invoices);
            context.OutcomingEntries.AddRange(outcomings);
            context.OutcomingEntryDetails.AddRange(outcomingDetails);
            context.OutcomingEntryBankTransaction.AddRange(outcomingBankTransactions);
            context.RelationInOutEntry.AddRange(relationInOutEntrys);
            context.SaveChanges();
        }
    }
}
