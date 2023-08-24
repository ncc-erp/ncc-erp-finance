using FinanceManagement.APIs.Invoices;
using FinanceManagement.APIs.Invoices.Dto;
using FinanceManagement.Entities;
using FinanceManagement.IoC;
using FinanceManagement.Managers.BTransactions.Dtos;
using FinanceManagement.Managers.Invoices.Dtos;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace FinanceManagement.Application.Test.APIs
{
    public class InvoiceAppService_Tests : FinfastApplicationTestBase
    {
        private readonly IWorkScope _workScope;
        private readonly InvoiceAppService _invoiceAppService;
        public InvoiceAppService_Tests()
        {
            _workScope = Resolve<IWorkScope>();
            _invoiceAppService = Resolve<InvoiceAppService>();
        }
        [Fact]
        public void CheckAutoPayment_Tests()
        {
            WithUnitOfWork(async () =>
            {
                var result = await _invoiceAppService.CheckAutoPaid(800);
                result.HasCollectionDebt.ShouldBeFalse();
            });
        }
        [Fact]
        public void AutoPaymentForAccount_Tests()
        {
            WithUnitOfWork(async () =>
            {
                var result = await _invoiceAppService.AutoPaidForAccount(new AutoPaidDto
                {
                    AccountId = 800
                });
                UsingDbContext((context) =>
                {
                    var incomings = context.IncomingEntries.Where(s => s.BTransactionId == 800 && s.IncomingEntryTypeId == 1).ToList();
                    incomings.Any().ShouldBeFalse();
                });
            });
        }

        // Auto trả nợ
        #region Case 1: Khoản tiền thừa của khách hàng < số tiền nợ của khách hàng, cùng currency
        [Fact]
        public async Task AutoPaymentForAccount_Test1()
        {
            var input = new AutoPaidDto
            {
                AccountId = 17
            };

            var expectedMessage = "Auto Payment Successfully";

            WithUnitOfWork(async () =>
            {
                var allIncomingEntrysBefore = _workScope.GetAll<IncomingEntry>().ToList();

                var result = await _invoiceAppService.AutoPaidForAccount(input);
                Assert.Equal(expectedMessage, result);
                var allIncomingEntrysAfter = _workScope.GetAll<IncomingEntry>().ToList();

                var checkStatusInvoice = await _workScope.GetAsync<Invoice>(27);

                allIncomingEntrysAfter.Count().ShouldBeGreaterThan(allIncomingEntrysBefore.Count());

                checkStatusInvoice.Status.ShouldBe(Enums.NInvoiceStatus.TRA_1_PHAN);
            });

            await Task.CompletedTask;
        }
        #endregion

        #region Case 2: Khoản tiền thừa của khách hàng = số tiền nợ của khách hàng, cùng currency
        [Fact]
        public async Task AutoPaymentForAccount_Test2()
        {
            var input = new AutoPaidDto
            {
                AccountId = 18
            };

            var expectedMessage = "Auto Payment Successfully";

            WithUnitOfWork(async () =>
            {
                var allIncomingEntrysBefore = _workScope.GetAll<IncomingEntry>().ToList();

                var result = await _invoiceAppService.AutoPaidForAccount(input);
                Assert.Equal(expectedMessage, result);
                var allIncomingEntrysAfter = _workScope.GetAll<IncomingEntry>().ToList();

                var checkStatusInvoice = await _workScope.GetAsync<Invoice>(28);

                allIncomingEntrysAfter.Count().ShouldBeGreaterThan(allIncomingEntrysBefore.Count());

                checkStatusInvoice.Status.ShouldBe(Enums.NInvoiceStatus.HOAN_THANH);
            });

            await Task.CompletedTask;
        }
        #endregion

        #region Case 3: Khoản tiền thừa của khách hàng > số tiền nợ của khách hàng, cùng currency
        [Fact]
        public async Task AutoPaymentForAccount_Test3()
        {
            var input = new AutoPaidDto
            {
                AccountId = 19
            };

            var expectedMessage = "Auto Payment Successfully";

            WithUnitOfWork(async () =>
            {
                var allIncomingEntrysBefore = _workScope.GetAll<IncomingEntry>().ToList();

                var result = await _invoiceAppService.AutoPaidForAccount(input);
                Assert.Equal(expectedMessage, result);
                var allIncomingEntrysAfter = _workScope.GetAll<IncomingEntry>().ToList();

                var checkStatusInvoice = await _workScope.GetAsync<Invoice>(29);

                allIncomingEntrysAfter.Count().ShouldBeGreaterThan(allIncomingEntrysBefore.Count());

                checkStatusInvoice.Status.ShouldBe(Enums.NInvoiceStatus.HOAN_THANH);
            });

            await Task.CompletedTask;
        }
        #endregion

        #region Case 4: Khoản tiền thừa của khách hàng < số tiền nợ của khách hàng, khác currency
        [Fact]
        public async Task AutoPaymentForAccount_Test4()
        {
            var input = new AutoPaidDto
            {
                AccountId = 20,
                CurrencyNeedConverts = new List<CurrencyNeedConvertDto> {
                new CurrencyNeedConvertDto {
                    FromCurrencyId = 2,
                    ToCurrencyId = 1,
                    ExchangeRate = 2
                }
                }
            };

            var expectedMessage = "Auto Payment Successfully";

            WithUnitOfWork(async () =>
            {
                var allIncomingEntrysBefore = _workScope.GetAll<IncomingEntry>().ToList();

                var result = await _invoiceAppService.AutoPaidForAccount(input);
                Assert.Equal(expectedMessage, result);
                var allIncomingEntrysAfter = _workScope.GetAll<IncomingEntry>().ToList();

                var checkStatusInvoice = await _workScope.GetAsync<Invoice>(30);

                allIncomingEntrysAfter.Count().ShouldBeGreaterThan(allIncomingEntrysBefore.Count());

                checkStatusInvoice.Status.ShouldBe(Enums.NInvoiceStatus.TRA_1_PHAN);
            });

            await Task.CompletedTask;
        }
        #endregion

        #region Case 5: Khoản tiền thừa của khách hàng = số tiền nợ của khách hàng, khác currency
        [Fact]
        public async Task AutoPaymentForAccount_Test5()
        {
            var input = new AutoPaidDto
            {
                AccountId = 21,
                CurrencyNeedConverts = new List<CurrencyNeedConvertDto> {
                new CurrencyNeedConvertDto {
                    FromCurrencyId = 2,
                    ToCurrencyId = 1,
                    ExchangeRate = 2
                }
                }
            };

            var expectedMessage = "Auto Payment Successfully";

            WithUnitOfWork(async () =>
            {
                var allIncomingEntrysBefore = _workScope.GetAll<IncomingEntry>().ToList();

                var result = await _invoiceAppService.AutoPaidForAccount(input);
                Assert.Equal(expectedMessage, result);
                var allIncomingEntrysAfter = _workScope.GetAll<IncomingEntry>().ToList();

                var checkStatusInvoice = await _workScope.GetAsync<Invoice>(31);

                allIncomingEntrysAfter.Count().ShouldBeGreaterThan(allIncomingEntrysBefore.Count());

                checkStatusInvoice.Status.ShouldBe(Enums.NInvoiceStatus.HOAN_THANH);
            });

            await Task.CompletedTask;
        }
        #endregion

        #region Case 6: Khoản tiền thừa của khách hàng > số tiền nợ của khách hàng, khác currency
        [Fact]
        public async Task AutoPaymentForAccount_Test6()
        {
            var input = new AutoPaidDto
            {
                AccountId = 22,
                CurrencyNeedConverts = new List<CurrencyNeedConvertDto> {
                new CurrencyNeedConvertDto {
                    FromCurrencyId = 2,
                    ToCurrencyId = 1,
                    ExchangeRate = 2
                }
                }
            };


            var expectedMessage = "Auto Payment Successfully";

            WithUnitOfWork(async () =>
            {
                var allIncomingEntrysBefore = _workScope.GetAll<IncomingEntry>().ToList();

                var result = await _invoiceAppService.AutoPaidForAccount(input);
                Assert.Equal(expectedMessage, result);
                var allIncomingEntrysAfter = _workScope.GetAll<IncomingEntry>().ToList();

                var checkStatusInvoice = await _workScope.GetAsync<Invoice>(32);

                allIncomingEntrysAfter.Count().ShouldBeGreaterThan(allIncomingEntrysBefore.Count());

                checkStatusInvoice.Status.ShouldBe(Enums.NInvoiceStatus.HOAN_THANH);
            });

            await Task.CompletedTask;
        }
        #endregion

        // Khách hàng trả kênh tiền
        [Fact]
        public async Task SetClientPayDeviant_Test()
        {
            var input = new ClientPayDeviantDto
            {
                IncomingEntryId = 7,
                IncomingName = "ABC",
            };

            WithUnitOfWork(async () =>
            {
                var incomingEntryBefore = await _workScope.GetAsync<IncomingEntry>(7);
                await _invoiceAppService.SetClientPayDeviant(input);

                var incomingEntryAfter = await _workScope.GetAsync<IncomingEntry>(7);

                incomingEntryAfter.AccountId.ShouldBe(23);
                incomingEntryAfter.BTransactionId.ShouldBe(36);
                incomingEntryAfter.Name.ShouldBe("ABC");
                incomingEntryAfter.BankTransactionId.ShouldBe(1);
                incomingEntryAfter.IncomingEntryTypeId.ShouldBe(0);
                incomingEntryAfter.Value.ShouldBe(150);
            });
            await Task.CompletedTask;
        }
    }
}
