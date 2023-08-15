using Abp.UI;
using DocumentFormat.OpenXml.VariantTypes;
using FinanceManagement.APIs.BTransactions;
using FinanceManagement.Entities;
using FinanceManagement.Entities.NewEntities;
using FinanceManagement.Extension;
using FinanceManagement.GeneralModels;
using FinanceManagement.Helper;
using FinanceManagement.IoC;
using FinanceManagement.Managers.BankTransactions.Dtos;
using FinanceManagement.Managers.BTransactions.Dtos;
using FinanceManagement.Managers.IncomingEntries.Dtos;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace FinanceManagement.Application.Test.APIs
{
    public class BTransactionAppService_Tests : FinfastApplicationTestBase
    {
        private readonly BTransactionAppService _bTransactionAppService;
        private readonly IWorkScope _workScope;
        public BTransactionAppService_Tests()
        {
            _workScope = Resolve<IWorkScope>();
            _bTransactionAppService = Resolve<BTransactionAppService>();
        }

        [Fact]
        public void GetAllPaging_Test()
        {
            var grids = new BTransactionGridParam()
            {
                MaxResultCount = 10,
                SkipCount = 0
            };
            WithUnitOfWork(async () =>
            {
                var result = await _bTransactionAppService.GetAllPaging(grids, null);
                result.TotalCount.ShouldBeGreaterThan(0);
            });
        }

        // Link tới một request chi
        #region Case 1: Không thể link Yêu cầu chi với Giao dịch đã Hoàn Thành
        [Fact]
        public void LinkOutcomingEntryWithBTransaction_Test1()
        {
            var inputLinkOutcomingWithBTransaction = new LinkOutcomingWithBTransactionDto
            {
                BTransactionId = 4,
                ToBankAccountId = 1,
                OutcomingEntryId = 1,
                ExchangeRate = 1,
            };

            WithUnitOfWork(async () =>
            {
                await Assert.ThrowsAsync<UserFriendlyException>(async () =>
                await _bTransactionAppService.LinkOutcomingEntryWithBTransaction(inputLinkOutcomingWithBTransaction));
            });
        }
        #endregion

        #region Case 2: Không link Yêu cầu chi với số tiền > 0
        [Fact]
        public void LinkOutcomingEntryWithBTransaction_Test2()
        {
            var inputLinkOutcomingWithBTransaction = new LinkOutcomingWithBTransactionDto
            {
                BTransactionId = 1,
                ToBankAccountId = 1,
                OutcomingEntryId = 1,
                ExchangeRate = 1,
            };

            WithUnitOfWork(async () =>
            {
                await Assert.ThrowsAsync<UserFriendlyException>(async () =>
                await _bTransactionAppService.LinkOutcomingEntryWithBTransaction(inputLinkOutcomingWithBTransaction));
            });
        }
        #endregion

        #region Case 3: Biến động số dư không thể lớn hơn request chi
        [Fact]
        public void LinkOutcomingEntryWithBTransaction_Test3()
        {
            var inputLinkOutcomingWithBTransaction = new LinkOutcomingWithBTransactionDto
            {
                BTransactionId = 3,
                ToBankAccountId = 1,
                OutcomingEntryId = 1,
                ExchangeRate = 1,
            };

            WithUnitOfWork(async () =>
            {
                await Assert.ThrowsAsync<UserFriendlyException>(async () =>
                await _bTransactionAppService.LinkOutcomingEntryWithBTransaction(inputLinkOutcomingWithBTransaction));
            });
        }
        #endregion

        #region Case 4: Biến động số dư đã được link tới GDNH
        [Fact]
        public void LinkOutcomingEntryWithBTransaction_Test4()
        {
            var inputLinkOutcomingWithBTransaction = new LinkOutcomingWithBTransactionDto
            {
                BTransactionId = 1,
                ToBankAccountId = 1,
                OutcomingEntryId = 1,
                ExchangeRate = 1,
            };

            WithUnitOfWork(async () =>
            {
                await Assert.ThrowsAsync<UserFriendlyException>(async () =>
                await _bTransactionAppService.LinkOutcomingEntryWithBTransaction(inputLinkOutcomingWithBTransaction));
            });
        }
        #endregion

        #region Case 5: BĐSD < Request chi đang Approved
        [Fact]
        public async Task LinkOutcomingEntryWithBTransaction_Test5()
        {
            var input = new LinkOutcomingWithBTransactionDto
            {
                BTransactionId = 3,
                ToBankAccountId = 1,
                OutcomingEntryId = 15,
                ExchangeRate = 1,
            };

            long expectedBankTransactionId = 3;
            var expectedOutcomingBankTransactionId = 2;

            await WithUnitOfWorkAsync(async () =>
            {
                await _bTransactionAppService.LinkOutcomingEntryWithBTransaction(input);

                var allOutcomingBankTransaction = _workScope.GetAll<OutcomingEntryBankTransaction>().ToList();
                var allbanktransaction = _workScope.GetAll<BankTransaction>().ToList();
                var bankTransactionInfo = await _workScope.GetAsync<BankTransaction>(expectedBankTransactionId);
                var outcomingBankTransactionInfo = await _workScope.GetAsync<OutcomingEntryBankTransaction>(expectedOutcomingBankTransactionId);
                var checkDoneBTransaction = await _workScope.GetAsync<BTransaction>(input.BTransactionId);

                allbanktransaction.Count().ShouldBe(3);
                bankTransactionInfo.BTransactionId.ShouldBe(input.BTransactionId);
                bankTransactionInfo.FromBankAccountId.ShouldBe(1);
                bankTransactionInfo.ToBankAccountId.ShouldBe(input.ToBankAccountId);
                bankTransactionInfo.FromValue.ShouldBe(100);
                bankTransactionInfo.ToValue.ShouldBe(100);

                allOutcomingBankTransaction.Count().ShouldBe(2);
                outcomingBankTransactionInfo.OutcomingEntryId.ShouldBe(input.OutcomingEntryId);
                outcomingBankTransactionInfo.BankTransactionId.ShouldBe(3);

                checkDoneBTransaction.Status.ShouldBe(Enums.BTransactionStatus.DONE);

                // check đối soát thu
                //VND
                //double expectedMoney = 0;

                //var allIncomingEntryVND = _workScope.GetAll<IncomingEntry>().Where(s => s.CurrencyId == 1).Select(s => s.Value).ToList();
                //var allBTransactionVND = _workScope.GetAll<BTransaction>().Where(s => s.BankAccount.CurrencyId == 1 && s.Money > 0).Where(s => s.Status == Enums.BTransactionStatus.DONE).Select(s => s.Money).ToList();

                //double totalValueIncomingEntryVND = 0;
                //double totalMoneyBTransactionVND = 0;

                //foreach (var item in allIncomingEntryVND)
                //{
                //    totalValueIncomingEntryVND += item;
                //}
                //foreach (var item in allBTransactionVND)
                //{
                //    totalMoneyBTransactionVND += item;
                //}
                //Assert.Equal(totalValueIncomingEntryVND - totalMoneyBTransactionVND, expectedMoney);

                ////USD
                //var allIncomingEntryUSD = _workScope.GetAll<IncomingEntry>().Where(s => s.CurrencyId == 2).Select(s => s.Value).ToList();
                //var allBTransactionUSD = _workScope.GetAll<BTransaction>().Where(s => s.BankAccount.CurrencyId == 2 && s.Money > 0).Where(s => s.Status == Enums.BTransactionStatus.DONE).Select(s => s.Money).ToList();

                //double totalValueIncomingEntryUSD = 0;
                //double totalMoneyBTransactionUSD = 0;

                //foreach (var item in allIncomingEntryUSD)
                //{
                //    totalValueIncomingEntryUSD += item;
                //}
                //foreach (var item in allBTransactionUSD)
                //{
                //    totalMoneyBTransactionUSD += item;
                //}
                //Assert.Equal(totalValueIncomingEntryUSD - totalMoneyBTransactionUSD, expectedMoney);

                /*
                // check đối soát chi - VNĐ
                var allBTransactionNegativeVND = _workScope.GetAll<BTransaction>().Where(s => s.BankAccount.CurrencyId == 1 && s.Money < 0).Where(s => s.Status == Enums.BTransactionStatus.DONE).Select(s => s.Id).ToList();
                var allOutcomingEntryVND = _workScope.GetAll<OutcomingEntry>().Where(s => s.CurrencyId == 1).Select(s => s.Value).ToList();
                var allBankTransactionVND = _workScope.GetAll<BankTransaction>().Where(s => s.BTransactionId != null && allBTransactionNegativeVND.Contains(s.BTransactionId.Value)).Select(s => s.ToValue).ToList();

                double totalValueOutcomingEntryVND = 0;
                double totalValueBankTransactionVND = 0;
                foreach (var item in allOutcomingEntryVND)
                {
                    totalValueOutcomingEntryVND += item;
                }
                foreach (var item in allBankTransactionVND)
                {
                    totalValueBankTransactionVND += item;
                }
                

                // Request chi chưa thực thi nhưng có giao dịch ngân hàng
                var qOutcomingEntryVND = from oce in _workScope.GetAll<OutcomingEntry>()
                                         from ocbt in _workScope.GetAll<OutcomingEntryBankTransaction>()
                                         where oce.Id == ocbt.OutcomingEntryId && oce.WorkflowStatusId != 3
                                         select oce.Value;
                double tongXuatTien = 0;
                foreach(var item in qOutcomingEntryVND.ToList())
                {
                    tongXuatTien += item;
                }
                Assert.Equal(totalValueOutcomingEntryVND - totalValueBankTransactionVND, tongXuatTien);
                */
            });

            await Task.CompletedTask;
        }
        #endregion

        #region Case 7: BĐSD = Request chi đang Approved
        [Fact]
        public async Task LinkOutcomingEntryWithBTransaction_Test7()
        {
            var input = new LinkOutcomingWithBTransactionDto
            {
                BTransactionId = 3,
                ToBankAccountId = 1,
                OutcomingEntryId = 14,
                ExchangeRate = 1,
            };

            long expectedBankTransactionId = 3;
            var expectedOutcomingBankTransactionId = 2;

            await WithUnitOfWorkAsync(async () =>
            {
                await _bTransactionAppService.LinkOutcomingEntryWithBTransaction(input);

                var allbanktransaction = _workScope.GetAll<BankTransaction>();
                var bankTransactionInfo = await _workScope.GetAsync<BankTransaction>(expectedBankTransactionId);
                var outcomingBankTransactionInfo = await _workScope.GetAsync<OutcomingEntryBankTransaction>(expectedOutcomingBankTransactionId);
                var checkDoneBTransaction = await _workScope.GetAsync<BTransaction>(input.BTransactionId);

                bankTransactionInfo.BTransactionId.ShouldBe(input.BTransactionId);
                bankTransactionInfo.FromBankAccountId.ShouldBe(1);
                bankTransactionInfo.ToBankAccountId.ShouldBe(input.ToBankAccountId);
                bankTransactionInfo.FromValue.ShouldBe(100);
                bankTransactionInfo.ToValue.ShouldBe(100);

                outcomingBankTransactionInfo.OutcomingEntryId.ShouldBe(input.OutcomingEntryId);
                outcomingBankTransactionInfo.BankTransactionId.ShouldBe(3);

                checkDoneBTransaction.Status.ShouldBe(Enums.BTransactionStatus.DONE);
            });
        }
        #endregion

        // kiểm tra chi lương 
        #region Case 1: INPUT null
        [Fact]
        public void CheckLinkOutcomingEntrySalaryWithBTransactions_Test1()
        {
            var input = new LinkOutcomingSalaryWithBTransactionsDto { };
            input = null;
            WithUnitOfWork(async () =>
            {
                await Assert.ThrowsAsync<UserFriendlyException>(async () =>
                await _bTransactionAppService.CheckLinkOutcomingEntrySalaryWithBTransactions(input));
            });
        }
        #endregion

        #region Case 2: Vui lòng chọn biến động số dư
        [Fact]
        public void CheckLinkOutcomingEntrySalaryWithBTransactions_Test2()
        {
            var input = new LinkOutcomingSalaryWithBTransactionsDto
            {
                BTransactionIds = null,
                ToBankAccountId = 1,
                OutcomingEntryId = 1,
                ExchangeRate = FinanceManagementConsts.DEFAULT_EXCHANGE_RATE
            };
            WithUnitOfWork(async () =>
            {
                await Assert.ThrowsAsync<UserFriendlyException>(async () =>
                await _bTransactionAppService.CheckLinkOutcomingEntrySalaryWithBTransactions(input));
            });
        }
        #endregion

        #region Case 3: Vui lòng chọn Request chi
        [Fact]
        public void CheckLinkOutcomingEntrySalaryWithBTransactions_Test3()
        {
            var input = new LinkOutcomingSalaryWithBTransactionsDto
            {
                BTransactionIds = new List<long> { 1 },
                ToBankAccountId = 1,
                OutcomingEntryId = default,
                ExchangeRate = FinanceManagementConsts.DEFAULT_EXCHANGE_RATE
            };
            WithUnitOfWork(async () =>
            {
                await Assert.ThrowsAsync<UserFriendlyException>(async () =>
                await _bTransactionAppService.CheckLinkOutcomingEntrySalaryWithBTransactions(input));
            });
        }
        #endregion

        #region Case 4: Vui lòng chọn tài khoản ngân hàng chi
        [Fact]
        public void CheckLinkOutcomingEntrySalaryWithBTransactions_Test4()
        {
            var input = new LinkOutcomingSalaryWithBTransactionsDto
            {
                BTransactionIds = new List<long> { 1 },
                ToBankAccountId = default,
                OutcomingEntryId = 1,
                ExchangeRate = FinanceManagementConsts.DEFAULT_EXCHANGE_RATE
            };
            WithUnitOfWork(async () =>
            {
                await Assert.ThrowsAsync<UserFriendlyException>(async () =>
                await _bTransactionAppService.CheckLinkOutcomingEntrySalaryWithBTransactions(input));
            });
        }
        #endregion

        #region Case 5: Không thể chi lương bằng các biến động số dư có loại tiền khác VNĐ
        [Fact]
        public void CheckCurrencyLinkOutcomingEntrySalaryWithBTransactions_Test5()
        {
            var input = new LinkOutcomingSalaryWithBTransactionsDto
            {
                BTransactionIds = new List<long> { 5 },
                ToBankAccountId = 1,
                OutcomingEntryId = 1,
                ExchangeRate = 1
            };

            WithUnitOfWork(async () =>
            {
                await Assert.ThrowsAsync<UserFriendlyException>(async () =>
                await _bTransactionAppService.CheckCurrencyLinkOutcomingEntrySalaryWithBTransactions(input));
            });

        }
        #endregion

        #region Case 6: Không thể chi lương bằng các biến động số dư > 0
        [Fact]
        public void CheckLinkOutcomingEntrySalaryWithBTransactions_Test6()
        {
            var input = new LinkOutcomingSalaryWithBTransactionsDto
            {
                BTransactionIds = new List<long> { 6 },
                ToBankAccountId = 1,
                OutcomingEntryId = 1,
                ExchangeRate = 1
            };
            WithUnitOfWork(async () =>
            {
                await Assert.ThrowsAsync<UserFriendlyException>(async () =>
                await _bTransactionAppService.CheckLinkOutcomingEntrySalaryWithBTransactions(input));
            });
        }
        #endregion

        #region Case 7: Tổng tiền biến động số dư lớn hơn số tiền chi khả dụng
        [Fact]
        public void CheckLinkOutcomingEntrySalaryWithBTransactions_Test7()
        {
            var input = new LinkOutcomingSalaryWithBTransactionsDto
            {
                BTransactionIds = new List<long> { 7, 8 },
                ToBankAccountId = 1,
                OutcomingEntryId = 5,
                ExchangeRate = 1
            };
            WithUnitOfWork(async () =>
            {
                await Assert.ThrowsAsync<UserFriendlyException>(async () =>
                await _bTransactionAppService.CheckLinkOutcomingEntrySalaryWithBTransactions(input));
            });
        }
        #endregion

        #region Case 8: Link 1 BĐSD < Chi lương
        [Fact]
        public async Task LinkOutcomingEntrySalaryWithBTransactions_Test8()
        {
            var input = new LinkOutcomingSalaryWithBTransactionsDto
            {
                BTransactionIds = new List<long> { 9 },
                ToBankAccountId = 1,
                OutcomingEntryId = 6,
                ExchangeRate = FinanceManagementConsts.DEFAULT_EXCHANGE_RATE
            };

            long expectedBankTransactionId = 3;
            var expectedOutcomingBankTransactionId = 2;

            await WithUnitOfWorkAsync(async () =>
            {
                var isDone = _bTransactionAppService.LinkOutcomingEntrySalaryWithBTransactions(input);
                Assert.NotNull(isDone);

                var allbanktransaction = _workScope.GetAll<BankTransaction>().ToList();
                var allOutcomingbanktransaction = _workScope.GetAll<OutcomingEntryBankTransaction>().ToList();
                var bankTransactionInfo = await _workScope.GetAsync<BankTransaction>(expectedBankTransactionId);
                var outcomingBankTransactionInfo = await _workScope.GetAsync<OutcomingEntryBankTransaction>(expectedOutcomingBankTransactionId);
                var checkDoneBTransaction = await _workScope.GetAsync<BTransaction>(9);

                allbanktransaction.Count().ShouldBe(3);
                bankTransactionInfo.BTransactionId.ShouldBe(9);
                bankTransactionInfo.FromBankAccountId.ShouldBe(1);
                bankTransactionInfo.ToBankAccountId.ShouldBe(input.ToBankAccountId);
                bankTransactionInfo.FromValue.ShouldBe(100);
                bankTransactionInfo.ToValue.ShouldBe(100);

                allOutcomingbanktransaction.Count().ShouldBe(2);
                outcomingBankTransactionInfo.OutcomingEntryId.ShouldBe(input.OutcomingEntryId);
                outcomingBankTransactionInfo.BankTransactionId.ShouldBe(3);

                checkDoneBTransaction.Status.ShouldBe(Enums.BTransactionStatus.DONE);
            });
        }
        #endregion

        #region Case 9: Link 1 BĐSD = Chi lương
        [Fact]
        public async Task CheckLinkOutcomingEntrySalaryWithBTransactions_Test9()
        {
            var input = new LinkOutcomingSalaryWithBTransactionsDto
            {
                BTransactionIds = new List<long> { 9 },
                ToBankAccountId = 1,
                OutcomingEntryId = 5,
                ExchangeRate = FinanceManagementConsts.DEFAULT_EXCHANGE_RATE
            };

            long expectedBankTransactionId = 3;
            var expectedOutcomingBankTransactionId = 2;

            await WithUnitOfWorkAsync(async () =>
            {
                var isDone = _bTransactionAppService.LinkOutcomingEntrySalaryWithBTransactions(input);
                Assert.NotNull(isDone);

                var allbanktransaction = _workScope.GetAll<BankTransaction>().ToList();
                var allOutcomingbanktransaction = _workScope.GetAll<OutcomingEntryBankTransaction>().ToList();
                var bankTransactionInfo = await _workScope.GetAsync<BankTransaction>(expectedBankTransactionId);
                var outcomingBankTransactionInfo = await _workScope.GetAsync<OutcomingEntryBankTransaction>(expectedOutcomingBankTransactionId);
                var checkDoneBTransaction = await _workScope.GetAsync<BTransaction>(9);

                allbanktransaction.Count().ShouldBe(3);
                bankTransactionInfo.BTransactionId.ShouldBe(9);
                bankTransactionInfo.FromBankAccountId.ShouldBe(1);
                bankTransactionInfo.ToBankAccountId.ShouldBe(input.ToBankAccountId);
                bankTransactionInfo.FromValue.ShouldBe(100);
                bankTransactionInfo.ToValue.ShouldBe(100);

                allOutcomingbanktransaction.Count().ShouldBe(2);
                outcomingBankTransactionInfo.OutcomingEntryId.ShouldBe(input.OutcomingEntryId);
                outcomingBankTransactionInfo.BankTransactionId.ShouldBe(3);

                checkDoneBTransaction.Status.ShouldBe(Enums.BTransactionStatus.DONE);
            });
        }
        #endregion

        #region Case 10: Link nhiều BĐSD < Chi lương
        [Fact]
        public async Task LinkOutcomingEntrySalaryWithBTransactions_Test10()
        {
            var input = new LinkOutcomingSalaryWithBTransactionsDto
            {
                BTransactionIds = new List<long> { 10, 11 },
                ToBankAccountId = 1,
                OutcomingEntryId = 5,
                ExchangeRate = FinanceManagementConsts.DEFAULT_EXCHANGE_RATE
            };

            await WithUnitOfWorkAsync(async () =>
            {
                var isDone = _bTransactionAppService.LinkOutcomingEntrySalaryWithBTransactions(input);
                Assert.NotNull(isDone);

                var allbanktransaction = _workScope.GetAll<BankTransaction>().ToList();
                var allOutcomingbanktransaction = _workScope.GetAll<OutcomingEntryBankTransaction>().ToList();
                var checkDoneBTransaction10 = await _workScope.GetAsync<BTransaction>(10);
                var checkDoneBTransaction11 = await _workScope.GetAsync<BTransaction>(11);

                allbanktransaction.Count().ShouldBe(4);

                allOutcomingbanktransaction.Count().ShouldBe(3);

                checkDoneBTransaction10.Status.ShouldBe(Enums.BTransactionStatus.DONE);
                checkDoneBTransaction11.Status.ShouldBe(Enums.BTransactionStatus.DONE);
            });
        }
        #endregion

        #region Case 11: Link nhiều BĐSD = Chi lương
        [Fact]
        public async Task LinkOutcomingEntrySalaryWithBTransactions_Test11()
        {
            var input = new LinkOutcomingSalaryWithBTransactionsDto
            {
                BTransactionIds = new List<long> { 12, 13 },
                ToBankAccountId = 1,
                OutcomingEntryId = 5,
                ExchangeRate = FinanceManagementConsts.DEFAULT_EXCHANGE_RATE
            };

            await WithUnitOfWorkAsync(async () =>
            { 
                var isDone = _bTransactionAppService.LinkOutcomingEntrySalaryWithBTransactions(input);
                Assert.NotNull(isDone);

                var allbanktransaction = _workScope.GetAll<BankTransaction>().ToList();
                var allOutcomingbanktransaction = _workScope.GetAll<OutcomingEntryBankTransaction>().ToList();
                var checkDoneBTransaction12 = await _workScope.GetAsync<BTransaction>(12);
                var checkDoneBTransaction13 = await _workScope.GetAsync<BTransaction>(13);

                allbanktransaction.Count().ShouldBe(4);

                allOutcomingbanktransaction.Count().ShouldBe(3);

                checkDoneBTransaction12.Status.ShouldBe(Enums.BTransactionStatus.DONE);
                checkDoneBTransaction13.Status.ShouldBe(Enums.BTransactionStatus.DONE);
            });
        }
        #endregion

        // Link tới nhiều request chi
        #region Case 1: BĐSD = tổng tiền các request chi
        [Fact]
        public async Task LinkMultipleOutcomingEntryWithBTransaction_Test1()
        {
            var input = new LinkMultipleOutcomingEntryWithBTransactionDto
            {
                BTransactionId = 3,
                ToBankAccountId = 1,
                ExchangeRate = 1,
                OutcomingEntryIds = new List<long> { 20, 21, 22 }
            };

            long expectedBankTransactionId = 3;

            await WithUnitOfWorkAsync(async () =>
            {
                var output = "{ Success = True, BankTransactionId = 3 }";

                var result = await _bTransactionAppService.LinkMultipleOutcomingEntryWithBTransaction(input);
                Assert.Equal(output, result.ToString());

                var allOutcomingbanktransaction = _workScope.GetAll<OutcomingEntryBankTransaction>().ToList();
                var bankTransactionInfo = await _workScope.GetAsync<BankTransaction>(expectedBankTransactionId);
                var checkDoneBTransaction = await _workScope.GetAsync<BTransaction>(input.BTransactionId);

                bankTransactionInfo.BTransactionId.ShouldBe(input.BTransactionId);
                bankTransactionInfo.FromBankAccountId.ShouldBe(1);
                bankTransactionInfo.ToBankAccountId.ShouldBe(input.ToBankAccountId);
                bankTransactionInfo.FromValue.ShouldBe(100);
                bankTransactionInfo.ToValue.ShouldBe(100);

                allOutcomingbanktransaction.Count.ShouldBe(4);
                checkDoneBTransaction.Status.ShouldBe(Enums.BTransactionStatus.DONE);
            });
        }
        #endregion

        #region Case 2: Request chi có tổng tiền detail khác value
        [Fact]
        public void LinkMultipleOutcomingEntryWithBTransaction_Test2()
        {
            WithUnitOfWork(async () =>
            {
                var input = new LinkMultipleOutcomingEntryWithBTransactionDto
                {
                    BTransactionId = 3,
                    ToBankAccountId = 1,
                    ExchangeRate = 1,
                    OutcomingEntryIds = new List<long> { 12, 13 }
                };

                var output = "{ ErrorMessage = #12 ABC có tổng tiền detail 100 khác 150, " +
                "Success = False, " +
                "OutcomingEntries = System.Collections.Generic.List`1[System.Object] }";

                var result = await _bTransactionAppService.LinkMultipleOutcomingEntryWithBTransaction(input);
                Assert.Equal(output, result.ToString());
            });
        }
        #endregion

        #region Case 3: Không thể link request chi khác trạng thái Approved
        [Fact]
        public void LinkMultipleOutcomingEntryWithBTransaction_Test3()
        {
            WithUnitOfWork(async () =>
            {
                var input = new LinkMultipleOutcomingEntryWithBTransactionDto
                {
                    BTransactionId = 3,
                    ToBankAccountId = 1,
                    ExchangeRate = 1,
                    OutcomingEntryIds = new List<long> { 10, 11 }
                };

                var output = "{ ErrorMessage = Không thể link với các request chi khác trạng thái APPROVED: 10,11, " +
                "Success = False, " +
                "OutcomingEntries = System.Collections.Generic.List`1[<>f__AnonymousType12`2[System.Int64,System.String]] }";

                var result = await _bTransactionAppService.LinkMultipleOutcomingEntryWithBTransaction(input);
                Assert.Equal(output, result.ToString());
            });
        }
        #endregion

        #region Case 4: Không thể link request chi đến biến động số dư đã DONE
        [Fact]
        public void LinkMultipleOutcomingEntryWithBTransaction_Test4()
        {
            WithUnitOfWork(async () =>
            {
                var input = new LinkMultipleOutcomingEntryWithBTransactionDto
                {
                    BTransactionId = 4,
                    ToBankAccountId = 1,
                    ExchangeRate = 1,
                    OutcomingEntryIds = new List<long> { }
                };

                var output = "{ ErrorMessage = Không thể link request chi đến biến động số dư đã DONE, " +
                "Success = False }";

                var result = await _bTransactionAppService.LinkMultipleOutcomingEntryWithBTransaction(input);
                Assert.Equal(output, result.ToString());
            });
        }
        #endregion

        #region Case 5: Không thể link BĐSD đến request chi do chênh lệch tiền
        [Fact]
        public void LinkMultipleOutcomingEntryWithBTransaction_Test5()
        {
            WithUnitOfWork(async () =>
            {
                var input = new LinkMultipleOutcomingEntryWithBTransactionDto
                {
                    BTransactionId = 3,
                    ToBankAccountId = 1,
                    ExchangeRate = 1,
                    OutcomingEntryIds = new List<long> { 15 }
                };

                var output = "{ ErrorMessage = Không thể link do chênh lệch tiền BTransaction: -100 và Request chi 150, " +
                "Success = False }";

                var result = await _bTransactionAppService.LinkMultipleOutcomingEntryWithBTransaction(input);
                Assert.Equal(output, result.ToString());
            });
        }
        #endregion

        #region Case 6: Không thể link Biến động số dự tới Request chi đã có liên kết tới ghi nhận thu hoặc Giao dịch ngân hàng
        [Fact]
        public void LinkMultipleOutcomingEntryWithBTransaction_Test6()
        {
            WithUnitOfWork(async () =>
            {
                var input = new LinkMultipleOutcomingEntryWithBTransactionDto
                {
                    BTransactionId = 3,
                    ToBankAccountId = 1,
                    ExchangeRate = 1,
                    OutcomingEntryIds = new List<long> { 19 }
                };

                var output = "{ ErrorMessage = Không thể link Biến động số dự tới Request chi đã có liên kết tới ghi nhận thu hoặc Giao dịch ngân hàng, " +
                "Success = False, OutcomingEntries = System.Collections.Generic.List`1[<>f__AnonymousType12`2[System.Int64,System.String]] }";

                var result = await _bTransactionAppService.LinkMultipleOutcomingEntryWithBTransaction(input);
                Assert.Equal(output, result.ToString());
            });
        }
        #endregion

        // link tới ghi nhận thu
        #region Case 1: Không thể link Ghi nhận thu với Giao dịch đã Hoàn Thành
        [Fact]
        public void CreateIncomingEntry_Test1()
        {
            WithUnitOfWork(async () =>
            {
                var input = new LinkIncomingEntryDto
                {
                    Name = "ABC",
                    IncomingEntryTypeId = 1,
                    BTransactionId = 30,
                    FromBankAccountId = 1
                };

                await Assert.ThrowsAsync<UserFriendlyException>(async () =>
                await _bTransactionAppService.CreateIncomingEntry(input));
            });
        }
        #endregion

        #region Case 2: Không link ghi nhận thu với số tiền <= 0
        [Fact]
        public void CreateIncomingEntry_Test2()
        {
            WithUnitOfWork(async () =>
            {
                var input = new LinkIncomingEntryDto
                {
                    Name = "ABC",
                    IncomingEntryTypeId = 1,
                    BTransactionId = 3,
                    FromBankAccountId = 1
                };

                await Assert.ThrowsAsync<UserFriendlyException>(async () =>
                await _bTransactionAppService.CreateIncomingEntry(input));
            });
        }
        #endregion

        #region Case 3: Điều kiện đúng
        [Fact]
        public async Task CreateIncomingEntry_Test3()
        {
            var input = new LinkIncomingEntryDto
            {
                Name = "ABC",
                IncomingEntryTypeId = 1,
                BTransactionId = 1,
                FromBankAccountId = 1
            };

            var expectedBankTransactionId = 3;
            var expectedIncomingEntryId = 1;

            WithUnitOfWork(async () =>
            {
                var result = await _bTransactionAppService.CreateIncomingEntry(input);

                Assert.Equal(expectedBankTransactionId, result.BankTransactionId);
                Assert.Equal(expectedIncomingEntryId, result.IncomingEntryId);

                var allBankTransaction = _workScope.GetAll<BankTransaction>().ToList();
                var bankTransactionInfo = await _workScope.GetAsync<BankTransaction>(expectedBankTransactionId);
                var allIncomingEntry = _workScope.GetAll<IncomingEntry>().ToList();
                var incomingEntryInfo = await _workScope.GetAsync<IncomingEntry>(expectedIncomingEntryId);
                var checkDoneBTransaction = await _workScope.GetAsync<BTransaction>(input.BTransactionId);

                allBankTransaction.Count().ShouldBe(3);
                bankTransactionInfo.BTransactionId.ShouldBe(input.BTransactionId);
                bankTransactionInfo.FromBankAccountId.ShouldBe(input.FromBankAccountId);
                bankTransactionInfo.ToBankAccountId.ShouldBe(1);
                bankTransactionInfo.FromValue.ShouldBe(100);
                bankTransactionInfo.ToValue.ShouldBe(100);

                allIncomingEntry.Count().ShouldBe(1);
                incomingEntryInfo.BankTransactionId.ShouldBe(expectedBankTransactionId);
                incomingEntryInfo.BTransactionId.ShouldBe(input.BTransactionId);
                incomingEntryInfo.IncomingEntryTypeId.ShouldBe(input.IncomingEntryTypeId);
                incomingEntryInfo.Name.ShouldBe(input.Name);
                incomingEntryInfo.Value.ShouldBe(100);
                incomingEntryInfo.CurrencyId.ShouldBe(1);

                checkDoneBTransaction.Status.ShouldBe(Enums.BTransactionStatus.DONE);
            });

            await Task.CompletedTask;
        }
        #endregion

        // Khách hàng thanh toán
        #region Case 1: Không thể trả nợ với số tiền <= 0
        [Fact]
        public void PaymentInvoiceByAccount_Test1()
        {
            var input = new PaymentInvoiceForAccountDto
            {
                BTransactionId = 3,
                AccountId = 1,
            };
            WithUnitOfWork(async () =>
            {
                await Assert.ThrowsAsync<UserFriendlyException>(async () =>
                await _bTransactionAppService.PaymentInvoiceByAccount(input));
            });
        }
        #endregion

        #region Case 2: BĐSD đã DONE
        [Fact]
        public void PaymentInvoiceByAccount_Test2()
        {
            var input = new PaymentInvoiceForAccountDto
            {
                BTransactionId = 30,
                AccountId = 1,
            };
            WithUnitOfWork(async () =>
            {
                await Assert.ThrowsAsync<UserFriendlyException>(async () =>
                await _bTransactionAppService.PaymentInvoiceByAccount(input));
            });
        }
        #endregion

        #region Case 3: BTransaction nhận thiếu tiền cho 1 invoice, cùng currency
        [Fact]
        public async Task PaymentInvoiceByAccount_Test3()
        {
            var input = new PaymentInvoiceForAccountDto()
            {
                AccountId = 1,
                BTransactionId = 14
            };

            var expectedBankTransactionId = 3;
            var expectedIncomingEntryId = 1;

            WithUnitOfWork(async () =>
            {
                await _bTransactionAppService.PaymentInvoiceByAccount(input);

                var allBankTransaction = _workScope.GetAll<BankTransaction>().ToList();
                var allIncomingEntry = _workScope.GetAll<IncomingEntry>().ToList();
                var bankTransactionInfo = await _workScope.GetAsync<BankTransaction>(expectedBankTransactionId);
                var incomingEntryInfo = await _workScope.GetAsync<IncomingEntry>(expectedIncomingEntryId);
                var checkStatusInvoice = await _workScope.GetAsync<Invoice>(1);
                var checkDoneBTransaction = await _workScope.GetAsync<BTransaction>(input.BTransactionId);

                allBankTransaction.Count().ShouldBe(3);
                bankTransactionInfo.BTransactionId.ShouldBe(input.BTransactionId);
                bankTransactionInfo.ToBankAccountId.ShouldBe(1);
                bankTransactionInfo.FromValue.ShouldBe(50);
                bankTransactionInfo.ToValue.ShouldBe(50);

                allIncomingEntry.Count().ShouldBe(1);
                incomingEntryInfo.AccountId.ShouldBe(input.AccountId);
                incomingEntryInfo.InvoiceId.ShouldBe(1);
                incomingEntryInfo.BankTransactionId.ShouldBe(expectedBankTransactionId);
                incomingEntryInfo.Id.ShouldBe(expectedIncomingEntryId);
                incomingEntryInfo.Value.ShouldBe(50);

                checkStatusInvoice.Status.ShouldBe(Enums.NInvoiceStatus.TRA_1_PHAN);
                checkDoneBTransaction.Status.ShouldBe(Enums.BTransactionStatus.DONE);
            });

            await Task.CompletedTask;
        }
        #endregion

        #region Case 4: BTransaction nhận đủ tiền cho 1 invoice, cùng currency
        [Fact]
        public async Task PaymentInvoiceByAccount_Test4()
        {
            var input = new PaymentInvoiceForAccountDto()
            {
                AccountId = 2,
                BTransactionId = 15
            };

            var expectedBankTransactionId = 3;
            var expectedIncomingEntryId = 1;

            WithUnitOfWork(async () =>
            {
                await _bTransactionAppService.PaymentInvoiceByAccount(input);

                var allBankTransaction = _workScope.GetAll<BankTransaction>().ToList();
                var allIncomingEntry = _workScope.GetAll<IncomingEntry>().ToList();

                var bankTransactionInfo = await _workScope.GetAsync<BankTransaction>(expectedBankTransactionId);
                var incomingEntryInfo = await _workScope.GetAsync<IncomingEntry>(expectedIncomingEntryId);

                var checkStatusInvoice = await _workScope.GetAsync<Invoice>(2);
                var checkDoneBTransaction = await _workScope.GetAsync<BTransaction>(input.BTransactionId);

                allBankTransaction.Count().ShouldBe(3);
                bankTransactionInfo.BTransactionId.ShouldBe(input.BTransactionId);
                bankTransactionInfo.ToBankAccountId.ShouldBe(1);
                bankTransactionInfo.FromValue.ShouldBe(100);
                bankTransactionInfo.ToValue.ShouldBe(100);

                allIncomingEntry.Count().ShouldBe(1);
                incomingEntryInfo.InvoiceId.ShouldBe(2);
                incomingEntryInfo.AccountId.ShouldBe(1);
                incomingEntryInfo.BankTransactionId.ShouldBe(expectedBankTransactionId);
                incomingEntryInfo.Id.ShouldBe(expectedIncomingEntryId);
                incomingEntryInfo.Value.ShouldBe(100);

                checkStatusInvoice.Status.ShouldBe(Enums.NInvoiceStatus.HOAN_THANH);
                checkDoneBTransaction.Status.ShouldBe(Enums.BTransactionStatus.DONE);
            });

            await Task.CompletedTask;
        }
        #endregion

        #region Case 5: BTransaction nhận thừa tiền cho 1 invoice, cùng currency
        [Fact]
        public async Task PaymentInvoiceByAccount_Test5()
        {
            var input = new PaymentInvoiceForAccountDto()
            {
                AccountId = 3,
                BTransactionId = 16
            };

            var expectedBankTransactionId = 3;

            WithUnitOfWork(async () =>
            {
                await _bTransactionAppService.PaymentInvoiceByAccount(input);

                var allBankTransaction = _workScope.GetAll<BankTransaction>().ToList();
                var allIncomingEntry = _workScope.GetAll<IncomingEntry>().ToList();

                var bankTransactionInfo = await _workScope.GetAsync<BankTransaction>(expectedBankTransactionId);
                var incomingEntryInfo1 = await _workScope.GetAsync<IncomingEntry>(1);
                var incomingEntryInfo2 = await _workScope.GetAsync<IncomingEntry>(2);

                var checkStatusInvoice = await _workScope.GetAsync<Invoice>(3);
                var checkDoneBTransaction = await _workScope.GetAsync<BTransaction>(input.BTransactionId);

                allBankTransaction.Count().ShouldBe(3);
                bankTransactionInfo.BTransactionId.ShouldBe(input.BTransactionId);
                bankTransactionInfo.ToBankAccountId.ShouldBe(1);
                bankTransactionInfo.FromValue.ShouldBe(150);
                bankTransactionInfo.ToValue.ShouldBe(150);

                allIncomingEntry.Count().ShouldBe(2);
                incomingEntryInfo1.InvoiceId.ShouldBe(3);
                incomingEntryInfo1.AccountId.ShouldBe(1);
                incomingEntryInfo1.BankTransactionId.ShouldBe(expectedBankTransactionId);
                incomingEntryInfo1.Id.ShouldBe(1);
                incomingEntryInfo1.Value.ShouldBe(100);

                incomingEntryInfo2.InvoiceId.ShouldBe(null);
                incomingEntryInfo2.AccountId.ShouldBe(3);
                incomingEntryInfo2.BankTransactionId.ShouldBe(expectedBankTransactionId);
                incomingEntryInfo2.Id.ShouldBe(2);
                incomingEntryInfo2.Value.ShouldBe(50);

                checkStatusInvoice.Status.ShouldBe(Enums.NInvoiceStatus.HOAN_THANH);
                checkDoneBTransaction.Status.ShouldBe(Enums.BTransactionStatus.DONE);
            });

            await Task.CompletedTask;
        }
        #endregion

        #region Case 6: BTransaction nhận thiếu tiền cho 1 invoice, khác currency
        [Fact]
        public async Task PaymentInvoiceByAccount_Test6()
        {
            var input = new PaymentInvoiceForAccountDto()
            {
                AccountId = 4,
                BTransactionId = 17,
                CurrencyNeedConverts = new List<CurrencyNeedConvertDto> {
                new CurrencyNeedConvertDto {
                    FromCurrencyId = 2,
                    ToCurrencyId = 1,
                    ExchangeRate = 2
                }
                },
            };

            var expectedBankTransactionId = 3;

            WithUnitOfWork(async () =>
            {
                await _bTransactionAppService.PaymentInvoiceByAccount(input);

                var allBankTransaction = _workScope.GetAll<BankTransaction>().ToList();
                var allIncomingEntry = _workScope.GetAll<IncomingEntry>().ToList();

                var bankTransactionInfo = await _workScope.GetAsync<BankTransaction>(expectedBankTransactionId);
                var incomingEntryInfo1 = await _workScope.GetAsync<IncomingEntry>(1);

                var checkStatusInvoice = await _workScope.GetAsync<Invoice>(4);
                var checkDoneBTransaction = await _workScope.GetAsync<BTransaction>(input.BTransactionId);

                allBankTransaction.Count().ShouldBe(3);
                bankTransactionInfo.BTransactionId.ShouldBe(input.BTransactionId);
                bankTransactionInfo.ToBankAccountId.ShouldBe(2);
                bankTransactionInfo.FromValue.ShouldBe(50);
                bankTransactionInfo.ToValue.ShouldBe(50);

                allIncomingEntry.Count().ShouldBe(1);
                incomingEntryInfo1.InvoiceId.ShouldBe(4);
                incomingEntryInfo1.AccountId.ShouldBe(2);
                incomingEntryInfo1.BankTransactionId.ShouldBe(expectedBankTransactionId);
                incomingEntryInfo1.Id.ShouldBe(1);
                incomingEntryInfo1.CurrencyId.ShouldBe(2);
                incomingEntryInfo1.Value.ShouldBe(50);

                checkStatusInvoice.Status.ShouldBe(Enums.NInvoiceStatus.TRA_1_PHAN);
                checkDoneBTransaction.Status.ShouldBe(Enums.BTransactionStatus.DONE);
            });

            await Task.CompletedTask;
        }
        #endregion

        #region Case 7: BTransaction nhận đủ tiền cho 1 invoice, khác currency
        [Fact]
        public async Task PaymentInvoiceByAccount_Test7()
        {
            var input = new PaymentInvoiceForAccountDto()
            {
                AccountId = 5,
                BTransactionId = 18,
                CurrencyNeedConverts = new List<CurrencyNeedConvertDto> {
                new CurrencyNeedConvertDto {
                    FromCurrencyId = 2,
                    ToCurrencyId = 1,
                    ExchangeRate = 2
                }
                },
            };

            var expectedBankTransactionId = 3;

            WithUnitOfWork(async () =>
            {
                await _bTransactionAppService.PaymentInvoiceByAccount(input);

                var allBankTransaction = _workScope.GetAll<BankTransaction>().ToList();
                var allIncomingEntry = _workScope.GetAll<IncomingEntry>().ToList();

                var bankTransactionInfo = await _workScope.GetAsync<BankTransaction>(expectedBankTransactionId);
                var incomingEntryInfo1 = await _workScope.GetAsync<IncomingEntry>(1);

                var checkStatusInvoice = await _workScope.GetAsync<Invoice>(5);
                var checkDoneBTransaction = await _workScope.GetAsync<BTransaction>(input.BTransactionId);

                allBankTransaction.Count().ShouldBe(3);
                bankTransactionInfo.BTransactionId.ShouldBe(input.BTransactionId);
                bankTransactionInfo.ToBankAccountId.ShouldBe(2);
                bankTransactionInfo.FromValue.ShouldBe(50);
                bankTransactionInfo.ToValue.ShouldBe(50);

                allIncomingEntry.Count().ShouldBe(1);
                incomingEntryInfo1.InvoiceId.ShouldBe(5);
                incomingEntryInfo1.AccountId.ShouldBe(2);
                incomingEntryInfo1.BankTransactionId.ShouldBe(expectedBankTransactionId);
                incomingEntryInfo1.Id.ShouldBe(1);
                incomingEntryInfo1.CurrencyId.ShouldBe(2);
                incomingEntryInfo1.Value.ShouldBe(50);

                checkStatusInvoice.Status.ShouldBe(Enums.NInvoiceStatus.HOAN_THANH);
                checkDoneBTransaction.Status.ShouldBe(Enums.BTransactionStatus.DONE);
            });

            await Task.CompletedTask;
        }
        #endregion

        #region Case 8: BTransaction nhận thừa tiền cho 1 invoice, khác currency
        [Fact]
        public async Task PaymentInvoiceByAccount_Test8()
        {
            var input = new PaymentInvoiceForAccountDto()
            {
                AccountId = 6,
                BTransactionId = 19,
                CurrencyNeedConverts = new List<CurrencyNeedConvertDto> {
                new CurrencyNeedConvertDto {
                    FromCurrencyId = 2,
                    ToCurrencyId = 1,
                    ExchangeRate = 2
                }
                },
            };

            var expectedBankTransactionId = 3;

            WithUnitOfWork(async () =>
            {
                await _bTransactionAppService.PaymentInvoiceByAccount(input);

                var allBankTransaction = _workScope.GetAll<BankTransaction>().ToList();
                var allIncomingEntry = _workScope.GetAll<IncomingEntry>().ToList();

                var bankTransactionInfo = await _workScope.GetAsync<BankTransaction>(expectedBankTransactionId);
                var incomingEntryInfo1 = await _workScope.GetAsync<IncomingEntry>(1);
                var incomingEntryInfo2 = await _workScope.GetAsync<IncomingEntry>(2);

                var checkStatusInvoice = await _workScope.GetAsync<Invoice>(6);
                var checkDoneBTransaction = await _workScope.GetAsync<BTransaction>(input.BTransactionId);

                allBankTransaction.Count().ShouldBe(3);
                bankTransactionInfo.BTransactionId.ShouldBe(input.BTransactionId);
                bankTransactionInfo.ToBankAccountId.ShouldBe(2);
                bankTransactionInfo.FromValue.ShouldBe(100);
                bankTransactionInfo.ToValue.ShouldBe(100);

                allIncomingEntry.Count().ShouldBe(2);
                incomingEntryInfo1.InvoiceId.ShouldBe(6);
                incomingEntryInfo1.AccountId.ShouldBe(2);
                incomingEntryInfo1.BankTransactionId.ShouldBe(expectedBankTransactionId);
                incomingEntryInfo1.Id.ShouldBe(1);
                incomingEntryInfo1.CurrencyId.ShouldBe(2);
                incomingEntryInfo1.Value.ShouldBe(50);

                incomingEntryInfo2.InvoiceId.ShouldBe(null);
                incomingEntryInfo2.AccountId.ShouldBe(6);
                incomingEntryInfo2.BankTransactionId.ShouldBe(expectedBankTransactionId);
                incomingEntryInfo2.Id.ShouldBe(2);
                incomingEntryInfo2.CurrencyId.ShouldBe(2);
                incomingEntryInfo2.Value.ShouldBe(50);

                checkStatusInvoice.Status.ShouldBe(Enums.NInvoiceStatus.HOAN_THANH);
                checkDoneBTransaction.Status.ShouldBe(Enums.BTransactionStatus.DONE);
            });

            await Task.CompletedTask;
        }
        #endregion

        #region Case 9: BTransaction nhận thiếu tiền cho 2 invoice, cùng currency (không đủ cả invoice 1 và 2)
        [Fact]
        public async Task PaymentInvoiceByAccount_Test9()
        {
            var input = new PaymentInvoiceForAccountDto()
            {
                AccountId = 7,
                BTransactionId = 20
            };

            var expectedBankTransactionId = 3;

            WithUnitOfWork(async () =>
            {
                await _bTransactionAppService.PaymentInvoiceByAccount(input);

                var allBankTransaction = _workScope.GetAll<BankTransaction>().ToList();
                var allIncomingEntry = _workScope.GetAll<IncomingEntry>().ToList();

                var bankTransactionInfo = await _workScope.GetAsync<BankTransaction>(expectedBankTransactionId);
                var incomingEntryInfo1 = await _workScope.GetAsync<IncomingEntry>(1);

                var checkStatusInvoice1 = await _workScope.GetAsync<Invoice>(7);
                var checkStatusInvoice2 = await _workScope.GetAsync<Invoice>(8);
                var checkDoneBTransaction = await _workScope.GetAsync<BTransaction>(input.BTransactionId);

                allBankTransaction.Count().ShouldBe(3);
                bankTransactionInfo.BTransactionId.ShouldBe(input.BTransactionId);
                bankTransactionInfo.ToBankAccountId.ShouldBe(1);
                bankTransactionInfo.FromValue.ShouldBe(50);
                bankTransactionInfo.ToValue.ShouldBe(50);

                allIncomingEntry.Count().ShouldBe(1);
                incomingEntryInfo1.InvoiceId.ShouldBe(7);
                incomingEntryInfo1.AccountId.ShouldBe(1);
                incomingEntryInfo1.BankTransactionId.ShouldBe(expectedBankTransactionId);
                incomingEntryInfo1.Id.ShouldBe(1);
                incomingEntryInfo1.CurrencyId.ShouldBe(1);
                incomingEntryInfo1.Value.ShouldBe(50);

                checkStatusInvoice1.Status.ShouldBe(Enums.NInvoiceStatus.TRA_1_PHAN);
                checkStatusInvoice2.Status.ShouldBe(Enums.NInvoiceStatus.CHUA_TRA);
                checkDoneBTransaction.Status.ShouldBe(Enums.BTransactionStatus.DONE);
            });

            await Task.CompletedTask;
        }
        #endregion

        #region Case 10: BTransaction nhận thiếu tiền cho 2 invoice, cùng currency (chỉ đủ invoice 1, nhưng thiếu invoice 2)
        [Fact]
        public async Task PaymentInvoiceByAccount_Test10()
        {
            var input = new PaymentInvoiceForAccountDto()
            {
                AccountId = 8,
                BTransactionId = 21
            };

            var expectedBankTransactionId = 3;

            WithUnitOfWork(async () =>
            {
                await _bTransactionAppService.PaymentInvoiceByAccount(input);

                var allBankTransaction = _workScope.GetAll<BankTransaction>().ToList();
                var allIncomingEntry = _workScope.GetAll<IncomingEntry>().ToList();

                var bankTransactionInfo = await _workScope.GetAsync<BankTransaction>(expectedBankTransactionId);
                var incomingEntryInfo1 = await _workScope.GetAsync<IncomingEntry>(1);

                var checkStatusInvoice1 = await _workScope.GetAsync<Invoice>(9);
                var checkStatusInvoice2 = await _workScope.GetAsync<Invoice>(10);
                var checkDoneBTransaction = await _workScope.GetAsync<BTransaction>(input.BTransactionId);

                allBankTransaction.Count().ShouldBe(3);
                bankTransactionInfo.BTransactionId.ShouldBe(input.BTransactionId);
                bankTransactionInfo.ToBankAccountId.ShouldBe(1);
                bankTransactionInfo.FromValue.ShouldBe(100);
                bankTransactionInfo.ToValue.ShouldBe(100);

                allIncomingEntry.Count().ShouldBe(1);
                incomingEntryInfo1.InvoiceId.ShouldBe(9);
                incomingEntryInfo1.AccountId.ShouldBe(1);
                incomingEntryInfo1.BankTransactionId.ShouldBe(expectedBankTransactionId);
                incomingEntryInfo1.Id.ShouldBe(1);
                incomingEntryInfo1.CurrencyId.ShouldBe(1);
                incomingEntryInfo1.Value.ShouldBe(100);

                checkStatusInvoice1.Status.ShouldBe(Enums.NInvoiceStatus.HOAN_THANH);
                checkStatusInvoice2.Status.ShouldBe(Enums.NInvoiceStatus.CHUA_TRA);
                checkDoneBTransaction.Status.ShouldBe(Enums.BTransactionStatus.DONE);
            });

            await Task.CompletedTask;
        }
        #endregion

        #region Case 11: BTransaction nhận thiếu tiền cho 2 invoice, cùng currency (đủ invoice 1, trả một phần invoice 2)
        [Fact]
        public async Task PaymentInvoiceByAccount_Test11()
        {
            var input = new PaymentInvoiceForAccountDto()
            {
                AccountId = 9,
                BTransactionId = 22
            };

            var expectedBankTransactionId = 3;

            WithUnitOfWork(async () =>
            {
                await _bTransactionAppService.PaymentInvoiceByAccount(input);

                var allBankTransaction = _workScope.GetAll<BankTransaction>().ToList();
                var allIncomingEntry = _workScope.GetAll<IncomingEntry>().ToList();

                var bankTransactionInfo = await _workScope.GetAsync<BankTransaction>(expectedBankTransactionId);
                var incomingEntryInfo1 = await _workScope.GetAsync<IncomingEntry>(1);
                var incomingEntryInfo2 = await _workScope.GetAsync<IncomingEntry>(2);

                var checkStatusInvoice1 = await _workScope.GetAsync<Invoice>(11);
                var checkStatusInvoice2 = await _workScope.GetAsync<Invoice>(12);
                var checkDoneBTransaction = await _workScope.GetAsync<BTransaction>(input.BTransactionId);

                allBankTransaction.Count().ShouldBe(3);
                bankTransactionInfo.BTransactionId.ShouldBe(input.BTransactionId);
                bankTransactionInfo.ToBankAccountId.ShouldBe(1);
                bankTransactionInfo.FromValue.ShouldBe(150);
                bankTransactionInfo.ToValue.ShouldBe(150);

                allIncomingEntry.Count().ShouldBe(2);
                incomingEntryInfo1.InvoiceId.ShouldBe(11);
                incomingEntryInfo1.AccountId.ShouldBe(1);
                incomingEntryInfo1.BankTransactionId.ShouldBe(expectedBankTransactionId);
                incomingEntryInfo1.Id.ShouldBe(1);
                incomingEntryInfo1.CurrencyId.ShouldBe(1);
                incomingEntryInfo1.Value.ShouldBe(100);

                incomingEntryInfo2.InvoiceId.ShouldBe(12);
                incomingEntryInfo2.AccountId.ShouldBe(1);
                incomingEntryInfo2.BankTransactionId.ShouldBe(expectedBankTransactionId);
                incomingEntryInfo2.Id.ShouldBe(2);
                incomingEntryInfo2.CurrencyId.ShouldBe(1);
                incomingEntryInfo2.Value.ShouldBe(50);

                checkStatusInvoice1.Status.ShouldBe(Enums.NInvoiceStatus.HOAN_THANH);
                checkStatusInvoice2.Status.ShouldBe(Enums.NInvoiceStatus.TRA_1_PHAN);
                checkDoneBTransaction.Status.ShouldBe(Enums.BTransactionStatus.DONE);
            });

            await Task.CompletedTask;
        }
        #endregion

        #region Case 12: BTransaction nhận đủ tiền cho 2 invoice, cùng currency 
        [Fact]
        public async Task PaymentInvoiceByAccount_Test12()
        {
            var input = new PaymentInvoiceForAccountDto()
            {
                AccountId = 10,
                BTransactionId = 23
            };

            var expectedBankTransactionId = 3;

            WithUnitOfWork(async () =>
            {
                await _bTransactionAppService.PaymentInvoiceByAccount(input);

                var allBankTransaction = _workScope.GetAll<BankTransaction>().ToList();
                var allIncomingEntry = _workScope.GetAll<IncomingEntry>().ToList();

                var bankTransactionInfo = await _workScope.GetAsync<BankTransaction>(expectedBankTransactionId);
                var incomingEntryInfo1 = await _workScope.GetAsync<IncomingEntry>(1);
                var incomingEntryInfo2 = await _workScope.GetAsync<IncomingEntry>(2);

                var checkStatusInvoice1 = await _workScope.GetAsync<Invoice>(13);
                var checkStatusInvoice2 = await _workScope.GetAsync<Invoice>(14);
                var checkDoneBTransaction = await _workScope.GetAsync<BTransaction>(input.BTransactionId);

                allBankTransaction.Count().ShouldBe(3);
                bankTransactionInfo.BTransactionId.ShouldBe(input.BTransactionId);
                bankTransactionInfo.ToBankAccountId.ShouldBe(1);
                bankTransactionInfo.FromValue.ShouldBe(150);
                bankTransactionInfo.ToValue.ShouldBe(150);

                allIncomingEntry.Count().ShouldBe(2);
                incomingEntryInfo1.InvoiceId.ShouldBe(13);
                incomingEntryInfo1.AccountId.ShouldBe(1);
                incomingEntryInfo1.BankTransactionId.ShouldBe(expectedBankTransactionId);
                incomingEntryInfo1.Id.ShouldBe(1);
                incomingEntryInfo1.CurrencyId.ShouldBe(1);
                incomingEntryInfo1.Value.ShouldBe(100);

                incomingEntryInfo2.InvoiceId.ShouldBe(14);
                incomingEntryInfo2.AccountId.ShouldBe(1);
                incomingEntryInfo2.BankTransactionId.ShouldBe(expectedBankTransactionId);
                incomingEntryInfo2.Id.ShouldBe(2);
                incomingEntryInfo2.CurrencyId.ShouldBe(1);
                incomingEntryInfo2.Value.ShouldBe(50);

                checkStatusInvoice1.Status.ShouldBe(Enums.NInvoiceStatus.HOAN_THANH);
                checkStatusInvoice2.Status.ShouldBe(Enums.NInvoiceStatus.HOAN_THANH);
                checkDoneBTransaction.Status.ShouldBe(Enums.BTransactionStatus.DONE);
            });

            await Task.CompletedTask;
        }
        #endregion

        #region Case 13: BTransaction nhận thừa tiền cho 2 invoice, cùng currency 
        [Fact]
        public async Task PaymentInvoiceByAccount_Test13()
        {
            var input = new PaymentInvoiceForAccountDto()
            {
                AccountId = 11,
                BTransactionId = 24
            };

            var expectedBankTransactionId = 3;

            WithUnitOfWork(async () =>
            {
                await _bTransactionAppService.PaymentInvoiceByAccount(input);

                var allBankTransaction = _workScope.GetAll<BankTransaction>().ToList();
                var allIncomingEntry = _workScope.GetAll<IncomingEntry>().ToList();

                var bankTransactionInfo = await _workScope.GetAsync<BankTransaction>(expectedBankTransactionId);
                var incomingEntryInfo1 = await _workScope.GetAsync<IncomingEntry>(1);
                var incomingEntryInfo2 = await _workScope.GetAsync<IncomingEntry>(2);
                var incomingEntryInfo3 = await _workScope.GetAsync<IncomingEntry>(3);

                var checkStatusInvoice1 = await _workScope.GetAsync<Invoice>(15);
                var checkStatusInvoice2 = await _workScope.GetAsync<Invoice>(16);
                var checkDoneBTransaction = await _workScope.GetAsync<BTransaction>(input.BTransactionId);

                allBankTransaction.Count().ShouldBe(3);
                bankTransactionInfo.BTransactionId.ShouldBe(input.BTransactionId);
                bankTransactionInfo.ToBankAccountId.ShouldBe(1);
                bankTransactionInfo.FromValue.ShouldBe(150);
                bankTransactionInfo.ToValue.ShouldBe(150);

                allIncomingEntry.Count().ShouldBe(3);
                incomingEntryInfo1.InvoiceId.ShouldBe(15);
                incomingEntryInfo1.AccountId.ShouldBe(1);
                incomingEntryInfo1.BankTransactionId.ShouldBe(expectedBankTransactionId);
                incomingEntryInfo1.Id.ShouldBe(1);
                incomingEntryInfo1.CurrencyId.ShouldBe(1);
                incomingEntryInfo1.Value.ShouldBe(50);

                incomingEntryInfo2.InvoiceId.ShouldBe(16);
                incomingEntryInfo2.AccountId.ShouldBe(1);
                incomingEntryInfo2.BankTransactionId.ShouldBe(expectedBankTransactionId);
                incomingEntryInfo2.Id.ShouldBe(2);
                incomingEntryInfo2.CurrencyId.ShouldBe(1);
                incomingEntryInfo2.Value.ShouldBe(50);

                incomingEntryInfo3.InvoiceId.ShouldBe(null);
                incomingEntryInfo3.AccountId.ShouldBe(11);
                incomingEntryInfo3.BankTransactionId.ShouldBe(expectedBankTransactionId);
                incomingEntryInfo3.Id.ShouldBe(3);
                incomingEntryInfo3.CurrencyId.ShouldBe(1);
                incomingEntryInfo3.Value.ShouldBe(50);

                checkStatusInvoice1.Status.ShouldBe(Enums.NInvoiceStatus.HOAN_THANH);
                checkStatusInvoice2.Status.ShouldBe(Enums.NInvoiceStatus.HOAN_THANH);
                checkDoneBTransaction.Status.ShouldBe(Enums.BTransactionStatus.DONE);
            });

            await Task.CompletedTask;
        }
        #endregion

        #region Case 14: BTransaction nhận thiếu tiền cho 2 invoice, khác currency (thiếu cả invoice 1 và 2)
        [Fact]
        public async Task PaymentInvoiceByAccount_Test14()
        {
            var input = new PaymentInvoiceForAccountDto()
            {
                AccountId = 12,
                BTransactionId = 25,
                CurrencyNeedConverts = new List<CurrencyNeedConvertDto> {
                    new CurrencyNeedConvertDto {
                        FromCurrencyId = 2,
                        ToCurrencyId = 1,
                        ExchangeRate = 2
                    }
                },
            };

            var expectedBankTransactionId = 3;

            WithUnitOfWork(async () =>
            {
                await _bTransactionAppService.PaymentInvoiceByAccount(input);

                var allBankTransaction = _workScope.GetAll<BankTransaction>().ToList();
                var allIncomingEntry = _workScope.GetAll<IncomingEntry>().ToList();

                var bankTransactionInfo = await _workScope.GetAsync<BankTransaction>(expectedBankTransactionId);
                var incomingEntryInfo1 = await _workScope.GetAsync<IncomingEntry>(1);

                var checkStatusInvoice1 = await _workScope.GetAsync<Invoice>(17);
                var checkStatusInvoice2 = await _workScope.GetAsync<Invoice>(18);
                var checkDoneBTransaction = await _workScope.GetAsync<BTransaction>(input.BTransactionId);

                allBankTransaction.Count().ShouldBe(3);
                bankTransactionInfo.BTransactionId.ShouldBe(input.BTransactionId);
                bankTransactionInfo.ToBankAccountId.ShouldBe(2);
                bankTransactionInfo.FromValue.ShouldBe(25);
                bankTransactionInfo.ToValue.ShouldBe(25);

                allIncomingEntry.Count().ShouldBe(1);
                incomingEntryInfo1.InvoiceId.ShouldBe(17);
                incomingEntryInfo1.AccountId.ShouldBe(2);
                incomingEntryInfo1.BankTransactionId.ShouldBe(expectedBankTransactionId);
                incomingEntryInfo1.Id.ShouldBe(1);
                incomingEntryInfo1.CurrencyId.ShouldBe(2);
                incomingEntryInfo1.Value.ShouldBe(25);

                checkStatusInvoice1.Status.ShouldBe(Enums.NInvoiceStatus.TRA_1_PHAN);
                checkStatusInvoice2.Status.ShouldBe(Enums.NInvoiceStatus.CHUA_TRA);
                checkDoneBTransaction.Status.ShouldBe(Enums.BTransactionStatus.DONE);
            });

            await Task.CompletedTask;
        }
        #endregion

        #region Case 15: BTransaction nhận thiếu tiền cho 2 invoice, khác currency (chỉ đủ invoice 1, nhưng thiếu  invoice 2)
        [Fact]
        public async Task PaymentInvoiceByAccount_Test15()
        {
            var input = new PaymentInvoiceForAccountDto()
            {
                AccountId = 13,
                BTransactionId = 26,
                CurrencyNeedConverts = new List<CurrencyNeedConvertDto> {
                    new CurrencyNeedConvertDto {
                        FromCurrencyId = 2,
                        ToCurrencyId = 1,
                        ExchangeRate = 2
                    }
                },
            };

            var expectedBankTransactionId = 3;

            WithUnitOfWork(async () =>
            {
                await _bTransactionAppService.PaymentInvoiceByAccount(input);

                var allBankTransaction = _workScope.GetAll<BankTransaction>().ToList();
                var allIncomingEntry = _workScope.GetAll<IncomingEntry>().ToList();

                var bankTransactionInfo = await _workScope.GetAsync<BankTransaction>(expectedBankTransactionId);
                var incomingEntryInfo1 = await _workScope.GetAsync<IncomingEntry>(1);

                var checkStatusInvoice1 = await _workScope.GetAsync<Invoice>(19);
                var checkStatusInvoice2 = await _workScope.GetAsync<Invoice>(20);
                var checkDoneBTransaction = await _workScope.GetAsync<BTransaction>(input.BTransactionId);

                allBankTransaction.Count().ShouldBe(3);
                bankTransactionInfo.BTransactionId.ShouldBe(input.BTransactionId);
                bankTransactionInfo.ToBankAccountId.ShouldBe(2);
                bankTransactionInfo.FromValue.ShouldBe(50);
                bankTransactionInfo.ToValue.ShouldBe(50);

                allIncomingEntry.Count().ShouldBe(1);
                incomingEntryInfo1.InvoiceId.ShouldBe(19);
                incomingEntryInfo1.AccountId.ShouldBe(2);
                incomingEntryInfo1.BankTransactionId.ShouldBe(expectedBankTransactionId);
                incomingEntryInfo1.Id.ShouldBe(1);
                incomingEntryInfo1.CurrencyId.ShouldBe(2);
                incomingEntryInfo1.Value.ShouldBe(50);

                checkStatusInvoice1.Status.ShouldBe(Enums.NInvoiceStatus.HOAN_THANH);
                checkStatusInvoice2.Status.ShouldBe(Enums.NInvoiceStatus.CHUA_TRA);
                checkDoneBTransaction.Status.ShouldBe(Enums.BTransactionStatus.DONE);
            });
            await Task.CompletedTask;
        }
        #endregion

        #region Case 16: BTransaction nhận thiếu tiền cho 2 invoice, khác currency (đủ invoice 1, trả một phần invoice 2)
        [Fact]
        public async Task PaymentInvoiceByAccount_Test16()
        {
            var input = new PaymentInvoiceForAccountDto()
            {
                AccountId = 14,
                BTransactionId = 27,
                CurrencyNeedConverts = new List<CurrencyNeedConvertDto> {
                    new CurrencyNeedConvertDto {
                        FromCurrencyId = 2,
                        ToCurrencyId = 1,
                        ExchangeRate = 2
                    }
                },
            };

            var expectedBankTransactionId = 3;

            WithUnitOfWork(async () =>
            {
                await _bTransactionAppService.PaymentInvoiceByAccount(input);

                var allBankTransaction = _workScope.GetAll<BankTransaction>().ToList();
                var allIncomingEntry = _workScope.GetAll<IncomingEntry>().ToList();

                var bankTransactionInfo = await _workScope.GetAsync<BankTransaction>(expectedBankTransactionId);
                var incomingEntryInfo1 = await _workScope.GetAsync<IncomingEntry>(1);
                var incomingEntryInfo2 = await _workScope.GetAsync<IncomingEntry>(2);

                var checkStatusInvoice1 = await _workScope.GetAsync<Invoice>(21);
                var checkStatusInvoice2 = await _workScope.GetAsync<Invoice>(22);
                var checkDoneBTransaction = await _workScope.GetAsync<BTransaction>(input.BTransactionId);

                allBankTransaction.Count().ShouldBe(3);
                bankTransactionInfo.BTransactionId.ShouldBe(input.BTransactionId);
                bankTransactionInfo.ToBankAccountId.ShouldBe(2);
                bankTransactionInfo.FromValue.ShouldBe(75);
                bankTransactionInfo.ToValue.ShouldBe(75);

                allIncomingEntry.Count().ShouldBe(2);

                incomingEntryInfo1.InvoiceId.ShouldBe(21);
                incomingEntryInfo1.AccountId.ShouldBe(2);
                incomingEntryInfo1.BankTransactionId.ShouldBe(expectedBankTransactionId);
                incomingEntryInfo1.Id.ShouldBe(1);
                incomingEntryInfo1.CurrencyId.ShouldBe(2);
                incomingEntryInfo1.Value.ShouldBe(50);

                incomingEntryInfo2.InvoiceId.ShouldBe(22);
                incomingEntryInfo2.AccountId.ShouldBe(2);
                incomingEntryInfo2.BankTransactionId.ShouldBe(expectedBankTransactionId);
                incomingEntryInfo2.Id.ShouldBe(2);
                incomingEntryInfo2.CurrencyId.ShouldBe(2);
                incomingEntryInfo2.Value.ShouldBe(25);

                checkStatusInvoice1.Status.ShouldBe(Enums.NInvoiceStatus.HOAN_THANH);
                checkStatusInvoice2.Status.ShouldBe(Enums.NInvoiceStatus.TRA_1_PHAN);
                checkDoneBTransaction.Status.ShouldBe(Enums.BTransactionStatus.DONE);
            });
            await Task.CompletedTask;
        }
        #endregion

        #region Case 17: BTransaction nhận đủ tiền cho 2 invoice, khác currency
        [Fact]
        public async Task PaymentInvoiceByAccount_Test17()
        {
            var input = new PaymentInvoiceForAccountDto()
            {
                AccountId = 15,
                BTransactionId = 28,
                CurrencyNeedConverts = new List<CurrencyNeedConvertDto> {
                    new CurrencyNeedConvertDto {
                        FromCurrencyId = 2,
                        ToCurrencyId = 1,
                        ExchangeRate = 2
                    }
                },
            };

            var expectedBankTransactionId = 3;

            WithUnitOfWork(async () =>
            {
                await _bTransactionAppService.PaymentInvoiceByAccount(input);

                var allBankTransaction = _workScope.GetAll<BankTransaction>().ToList();
                var allIncomingEntry = _workScope.GetAll<IncomingEntry>().ToList();

                var bankTransactionInfo = await _workScope.GetAsync<BankTransaction>(expectedBankTransactionId);
                var incomingEntryInfo1 = await _workScope.GetAsync<IncomingEntry>(1);
                var incomingEntryInfo2 = await _workScope.GetAsync<IncomingEntry>(2);

                var checkStatusInvoice1 = await _workScope.GetAsync<Invoice>(23);
                var checkStatusInvoice2 = await _workScope.GetAsync<Invoice>(24);
                var checkDoneBTransaction = await _workScope.GetAsync<BTransaction>(input.BTransactionId);

                allBankTransaction.Count().ShouldBe(3);
                bankTransactionInfo.BTransactionId.ShouldBe(input.BTransactionId);
                bankTransactionInfo.ToBankAccountId.ShouldBe(2);
                bankTransactionInfo.FromValue.ShouldBe(75);
                bankTransactionInfo.ToValue.ShouldBe(75);

                allIncomingEntry.Count().ShouldBe(2);

                incomingEntryInfo1.InvoiceId.ShouldBe(23);
                incomingEntryInfo1.AccountId.ShouldBe(2);
                incomingEntryInfo1.BankTransactionId.ShouldBe(expectedBankTransactionId);
                incomingEntryInfo1.Id.ShouldBe(1);
                incomingEntryInfo1.CurrencyId.ShouldBe(2);
                incomingEntryInfo1.Value.ShouldBe(50);

                incomingEntryInfo2.InvoiceId.ShouldBe(24);
                incomingEntryInfo2.AccountId.ShouldBe(2);
                incomingEntryInfo2.BankTransactionId.ShouldBe(expectedBankTransactionId);
                incomingEntryInfo2.Id.ShouldBe(2);
                incomingEntryInfo2.CurrencyId.ShouldBe(2);
                incomingEntryInfo2.Value.ShouldBe(25);

                checkStatusInvoice1.Status.ShouldBe(Enums.NInvoiceStatus.HOAN_THANH);
                checkStatusInvoice2.Status.ShouldBe(Enums.NInvoiceStatus.HOAN_THANH);
                checkDoneBTransaction.Status.ShouldBe(Enums.BTransactionStatus.DONE);
            });
            await Task.CompletedTask;
        }
        #endregion

        #region Case 18: BTransaction nhận thừa tiền cho 2 invoice, khác currency
        [Fact]
        public async Task PaymentInvoiceByAccount_Test18()
        {
            var input = new PaymentInvoiceForAccountDto()
            {
                AccountId = 16,
                BTransactionId = 29,
                CurrencyNeedConverts = new List<CurrencyNeedConvertDto> {
                    new CurrencyNeedConvertDto {
                        FromCurrencyId = 2,
                        ToCurrencyId = 1,
                        ExchangeRate = 2
                    }
                },
            };

            var expectedBankTransactionId = 3;

            WithUnitOfWork(async () =>
            {
                await _bTransactionAppService.PaymentInvoiceByAccount(input);

                var allBankTransaction = _workScope.GetAll<BankTransaction>().ToList();
                var allIncomingEntry = _workScope.GetAll<IncomingEntry>().ToList();

                var bankTransactionInfo = await _workScope.GetAsync<BankTransaction>(expectedBankTransactionId);
                var incomingEntryInfo1 = await _workScope.GetAsync<IncomingEntry>(1);
                var incomingEntryInfo2 = await _workScope.GetAsync<IncomingEntry>(2);
                var incomingEntryInfo3 = await _workScope.GetAsync<IncomingEntry>(3);

                var checkStatusInvoice1 = await _workScope.GetAsync<Invoice>(25);
                var checkStatusInvoice2 = await _workScope.GetAsync<Invoice>(26);
                var checkDoneBTransaction = await _workScope.GetAsync<BTransaction>(input.BTransactionId);

                allBankTransaction.Count().ShouldBe(3);
                bankTransactionInfo.BTransactionId.ShouldBe(input.BTransactionId);
                bankTransactionInfo.ToBankAccountId.ShouldBe(2);
                bankTransactionInfo.FromValue.ShouldBe(100);
                bankTransactionInfo.ToValue.ShouldBe(100);

                allIncomingEntry.Count().ShouldBe(3);

                incomingEntryInfo1.InvoiceId.ShouldBe(25);
                incomingEntryInfo1.AccountId.ShouldBe(2);
                incomingEntryInfo1.BankTransactionId.ShouldBe(expectedBankTransactionId);
                incomingEntryInfo1.Id.ShouldBe(1);
                incomingEntryInfo1.CurrencyId.ShouldBe(2);
                incomingEntryInfo1.Value.ShouldBe(25);

                incomingEntryInfo2.InvoiceId.ShouldBe(26);
                incomingEntryInfo2.AccountId.ShouldBe(2);
                incomingEntryInfo2.BankTransactionId.ShouldBe(expectedBankTransactionId);
                incomingEntryInfo2.Id.ShouldBe(2);
                incomingEntryInfo2.CurrencyId.ShouldBe(2);
                incomingEntryInfo2.Value.ShouldBe(25);

                incomingEntryInfo3.InvoiceId.ShouldBe(null);
                incomingEntryInfo3.AccountId.ShouldBe(16);
                incomingEntryInfo3.BankTransactionId.ShouldBe(expectedBankTransactionId);
                incomingEntryInfo3.Id.ShouldBe(3);
                incomingEntryInfo3.CurrencyId.ShouldBe(2);
                incomingEntryInfo3.Value.ShouldBe(50);

                checkStatusInvoice1.Status.ShouldBe(Enums.NInvoiceStatus.HOAN_THANH);
                checkStatusInvoice2.Status.ShouldBe(Enums.NInvoiceStatus.HOAN_THANH);
                checkDoneBTransaction.Status.ShouldBe(Enums.BTransactionStatus.DONE);
            });
            await Task.CompletedTask;
        }
        #endregion

        // Tạo nhiều ghi nhận thu
        #region Case 1: Ghi nhận thu NULL
        [Fact]
        public async Task CheckCreateMultiIncomingEntry_Test1()
        {
            var input = new LinkMultiIncomingEntryDto
            {
                IncomingEntries = null,
                FromBankAccountId = 1,
                BTransactionId = 1,
            };

            WithUnitOfWork(async () =>
            {
                await Assert.ThrowsAsync<UserFriendlyException>(async () =>
                await _bTransactionAppService.CreateMultiIncomingEntry(input));
            });

            await Task.CompletedTask;
        }
        #endregion

        #region Case 2: Không thể tạo ghi nhận thu có số tiền <= 0
        [Fact]
        public async Task CheckCreateMultiIncomingEntry_Test2()
        {
            var input = new LinkMultiIncomingEntryDto
            {
                IncomingEntries = new List<CreateIncomingEntryDto>
                {
                    new CreateIncomingEntryDto
                    {
                         IncomingEntryTypeId = 1,
                         BankTransactionId = 1,
                         Name = "ABC",
                         Value = -100,
                         CurrencyId = 1,
                         ExchangeRate = 1,
                         BTransactionId = 1,
                    }
                },
                FromBankAccountId = 1,
                BTransactionId = 1,
            };

            WithUnitOfWork(async () =>
            {
                await Assert.ThrowsAsync<UserFriendlyException>(async () =>
                await _bTransactionAppService.CreateMultiIncomingEntry(input));
            });

            await Task.CompletedTask;
        }
        #endregion

        #region Case 3: Chưa chọn biến động số dư
        [Fact]
        public async Task CheckCreateMultiIncomingEntry_Test3()
        {
            var input = new LinkMultiIncomingEntryDto
            {
                IncomingEntries = new List<CreateIncomingEntryDto>
                {
                    new CreateIncomingEntryDto
                    {
                         IncomingEntryTypeId = 1,
                         BankTransactionId = 1,
                         Name = "ABC",
                         Value = 100,
                         CurrencyId = 1,
                         ExchangeRate = 1,
                         BTransactionId = null,
                    }
                },
                FromBankAccountId = 1,
                BTransactionId = null,
            };

            WithUnitOfWork(async () =>
            {
                await Assert.ThrowsAsync<UserFriendlyException>(async () =>
                await _bTransactionAppService.CreateMultiIncomingEntry(input));
            });

            await Task.CompletedTask;
        }
        #endregion

        #region Case 4: Chưa chọn BANK gửi
        [Fact]
        public async Task CheckCreateMultiIncomingEntry_Test4()
        {
            var input = new LinkMultiIncomingEntryDto
            {
                IncomingEntries = new List<CreateIncomingEntryDto>
                {
                    new CreateIncomingEntryDto
                    {
                         IncomingEntryTypeId = 1,
                         BankTransactionId = 1,
                         Name = "ABC",
                         Value = 100,
                         CurrencyId = 1,
                         ExchangeRate = 1,
                         BTransactionId = 1,
                    }
                },
                FromBankAccountId = null,
                BTransactionId = 1,
            };

            WithUnitOfWork(async () =>
            {
                await Assert.ThrowsAsync<UserFriendlyException>(async () =>
                await _bTransactionAppService.CreateMultiIncomingEntry(input));
            });

            await Task.CompletedTask;
        }
        #endregion

        #region Case 5: Không tồn tại BankId = 3
        [Fact]
        public async Task CheckCreateMultiIncomingEntry_Test5()
        {
            var input = new LinkMultiIncomingEntryDto
            {
                IncomingEntries = new List<CreateIncomingEntryDto>
                {
                    new CreateIncomingEntryDto
                    {
                         IncomingEntryTypeId = 1,
                         BankTransactionId = 1,
                         Name = "ABC",
                         Value = 100,
                         CurrencyId = 1,
                         ExchangeRate = 1,
                         BTransactionId = 1,
                    }
                },
                FromBankAccountId = 3,
                BTransactionId = 1,
            };

            WithUnitOfWork(async () =>
            {
                await Assert.ThrowsAsync<UserFriendlyException>(async () =>
                await _bTransactionAppService.CreateMultiIncomingEntry(input));
            });

            await Task.CompletedTask;
        }
        #endregion

        #region Case 6: Không thể tạo ghi nhận thu với biến động số dư đã hoàn thành
        [Fact]
        public async Task CheckCreateMultiIncomingEntry_Test6()
        {
            var input = new LinkMultiIncomingEntryDto
            {
                IncomingEntries = new List<CreateIncomingEntryDto>
                {
                    new CreateIncomingEntryDto
                    {
                         IncomingEntryTypeId = 1,
                         BankTransactionId = 1,
                         Name = "ABC",
                         Value = 100,
                         CurrencyId = 1,
                         ExchangeRate = 1,
                         BTransactionId = 30,
                    }
                },
                FromBankAccountId = 1,
                BTransactionId = 30,
            };

            WithUnitOfWork(async () =>
            {
                await Assert.ThrowsAsync<UserFriendlyException>(async () =>
                await _bTransactionAppService.CreateMultiIncomingEntry(input));
            });

            await Task.CompletedTask;
        }
        #endregion

        #region Case 7: Không thể tạo ghi nhận thu với biến động số dư có số tiền <= 0
        [Fact]
        public async Task CheckCreateMultiIncomingEntry_Test7()
        {
            var input = new LinkMultiIncomingEntryDto
            {
                IncomingEntries = new List<CreateIncomingEntryDto>
                {
                    new CreateIncomingEntryDto
                    {
                         IncomingEntryTypeId = 1,
                         BankTransactionId = 1,
                         Name = "ABC",
                         Value = 100,
                         CurrencyId = 1,
                         ExchangeRate = 1,
                         BTransactionId = 3,
                    }
                },
                FromBankAccountId = 1,
                BTransactionId = 3,
            };

            WithUnitOfWork(async () =>
            {
                await Assert.ThrowsAsync<UserFriendlyException>(async () =>
                await _bTransactionAppService.CreateMultiIncomingEntry(input));
            });

            await Task.CompletedTask;
        }
        #endregion

        #region Case 8: Không thể tạo ghi nhận thu Bank gửi có loại tiền khác biến động số dư
        [Fact]
        public async Task CheckCreateMultiIncomingEntry_Test8()
        {
            var input = new LinkMultiIncomingEntryDto
            {
                IncomingEntries = new List<CreateIncomingEntryDto>
                {
                    new CreateIncomingEntryDto
                    {
                         IncomingEntryTypeId = 1,
                         BankTransactionId = 1,
                         Name = "ABC",
                         Value = 100,
                         CurrencyId = 1,
                         ExchangeRate = 1,
                         BTransactionId = 1,
                    }
                },
                FromBankAccountId = 2,
                BTransactionId = 1,
            };

            WithUnitOfWork(async () =>
            {
                await Assert.ThrowsAsync<UserFriendlyException>(async () =>
                await _bTransactionAppService.CreateMultiIncomingEntry(input));
            });

            await Task.CompletedTask;
        }
        #endregion

        #region Case 9: Không thể tạo ghi nhận thu Tổng tiền các ghi nhận thu Khác tiền của biến động số dư
        [Fact]
        public async Task CheckCreateMultiIncomingEntry_Test9()
        {
            var input = new LinkMultiIncomingEntryDto
            {
                IncomingEntries = new List<CreateIncomingEntryDto>
                {
                    new CreateIncomingEntryDto
                    {
                         IncomingEntryTypeId = 1,
                         BankTransactionId = 1,
                         Name = "ABC",
                         Value = 150,
                         CurrencyId = 1,
                         ExchangeRate = 1,
                         BTransactionId = 1,
                    }
                },
                FromBankAccountId = 1,
                BTransactionId = 1,
            };

            WithUnitOfWork(async () =>
            {
                await Assert.ThrowsAsync<UserFriendlyException>(async () =>
                await _bTransactionAppService.CreateMultiIncomingEntry(input));
            });

            await Task.CompletedTask;
        }
        #endregion

        #region Case 10: Điều kiện đúng
        [Fact]
        public async Task CheckCreateMultiIncomingEntry_Test10()
        {
            var input = new LinkMultiIncomingEntryDto
            {
                IncomingEntries = new List<CreateIncomingEntryDto>
                {
                    new CreateIncomingEntryDto
                    {
                         IncomingEntryTypeId = 1,
                         BankTransactionId = 1,
                         Name = "ABC",
                         Value = 50,
                         CurrencyId = 1,
                         ExchangeRate = 1,
                         BTransactionId = 1,
                    },
                    new CreateIncomingEntryDto
                    {
                         IncomingEntryTypeId = 1,
                         BankTransactionId = 1,
                         Name = "ABC",
                         Value = 50,
                         CurrencyId = 1,
                         ExchangeRate = 1,
                         BTransactionId = 1,
                    }
                },
                FromBankAccountId = 1,
                BTransactionId = 1,
            };

            var expectedBankTransactionId = 3;

            WithUnitOfWork(async () =>
            {
                var allIncomingEntrysBefore = _workScope.GetAll<IncomingEntry>().ToList();
                var result = await _bTransactionAppService.CreateMultiIncomingEntry(input);
                var allIncomingEntrysAfter = _workScope.GetAll<IncomingEntry>().ToList();

                Assert.Equal(allIncomingEntrysBefore.Count()+2, allIncomingEntrysAfter.Count());
                Assert.Equal(expectedBankTransactionId, result.BankTransactionId);
            });

            await Task.CompletedTask;
        }
        #endregion

        // -- BÁN NGOẠI TỆ --
        #region Case 1: DONE
        [Fact]
        public async Task ConversionTransaction_Test1()
        {
            var input = new ConversionTransactionDto
            {
                MinusBTransactionIds = new List<long> { 100 },
                PlusBTransactionIds = new List<long> { 101 },
                OutcomingEntryId = 100,
                IncomingEntryTypeId = 2,
                FromBankAccountId = 1,
                ToBankAccountId = 1,
            };

            WithUnitOfWork(async () =>
            {
                var allBankTransactionBefore = _workScope.GetAll<BankTransaction>().ToList();
                await _bTransactionAppService.ConversionTransaction(input);
                var allBankTransactionAfter = _workScope.GetAll<BankTransaction>().ToList();
                var checkStatusBTransaction100 = await _workScope.GetAsync<BTransaction>(100);
                var checkStatusBTransaction101 = await _workScope.GetAsync<BTransaction>(101);
                Assert.NotNull(input);

                allBankTransactionAfter.Count().ShouldBeGreaterThan(allBankTransactionBefore.Count());

                checkStatusBTransaction100.Status.ShouldBe(Enums.BTransactionStatus.DONE);
                checkStatusBTransaction101.Status.ShouldBe(Enums.BTransactionStatus.DONE);
            });

            await Task.CompletedTask;
        }
        #endregion

        #region Case 2: Không có yêu cầu chi
        [Fact]
        public async Task ConversionTransaction_Test2()
        {
            var input = new ConversionTransactionDto
            {
                MinusBTransactionIds = new List<long> { 100 },
                PlusBTransactionIds = new List<long> { 101 },
                IncomingEntryTypeId = 2,
                FromBankAccountId = 1,
                ToBankAccountId = 1,
            };

            WithUnitOfWork(async () =>
            {
                await Assert.ThrowsAsync<UserFriendlyException>(async () =>
                await _bTransactionAppService.ConversionTransaction(input));
            });

            await Task.CompletedTask;
        }
        #endregion

        #region Case 3: Không có tài khoản ngân hàng NHẬN
        [Fact]
        public async Task ConversionTransaction_Test3()
        {
            var input = new ConversionTransactionDto
            {
                MinusBTransactionIds = new List<long> { 100 },
                PlusBTransactionIds = new List<long> { 101 },
                OutcomingEntryId = 100,
                IncomingEntryTypeId = 2,
                FromBankAccountId = 1,
            };

            WithUnitOfWork(async () =>
            {
                await Assert.ThrowsAsync<UserFriendlyException>(async () =>
                await _bTransactionAppService.ConversionTransaction(input));
            });
            await Task.CompletedTask;
        }
        #endregion

        #region Case 4: Không có tài khoản ngân hàng GỬI
        [Fact]
        public async Task ConversionTransaction_Test4()
        {
            var input = new ConversionTransactionDto
            {
                MinusBTransactionIds = new List<long> { 100 },
                PlusBTransactionIds = new List<long> { 101 },
                OutcomingEntryId = 100,
                IncomingEntryTypeId = 2,
                ToBankAccountId = 1,
            };

            WithUnitOfWork(async () =>
            {
                await Assert.ThrowsAsync<UserFriendlyException>(async () =>
                await _bTransactionAppService.ConversionTransaction(input));
            });
            await Task.CompletedTask;
        }
        #endregion

        #region Case 5: Không có loại thu
        [Fact]
        public async Task ConversionTransaction_Test5()
        {
            var input = new ConversionTransactionDto
            {
                MinusBTransactionIds = new List<long> { 100 },
                PlusBTransactionIds = new List<long> { 101 },
                OutcomingEntryId = 100,
                FromBankAccountId = 1,
                ToBankAccountId = 1,
            };

            WithUnitOfWork(async () =>
            {
                await Assert.ThrowsAsync<UserFriendlyException>(async () =>
                await _bTransactionAppService.ConversionTransaction(input));
            });
            await Task.CompletedTask;
        }
        #endregion

        #region Case 6: Không có biến động số dư âm
        [Fact]
        public async Task ConversionTransaction_Test6()
        {
            var input = new ConversionTransactionDto
            {
                PlusBTransactionIds = new List<long> { 101 },
                OutcomingEntryId = 100,
                IncomingEntryTypeId = 2,
                FromBankAccountId = 1,
                ToBankAccountId = 1,
            };

            WithUnitOfWork(async () =>
            {
                await Assert.ThrowsAsync<UserFriendlyException>(async () =>
                await _bTransactionAppService.ConversionTransaction(input));
            });
            await Task.CompletedTask;
        }
        #endregion

        #region Case 7: Không có biến động số dư dương
        [Fact]
        public async Task ConversionTransaction_Test7()
        {
            var input = new ConversionTransactionDto
            {
                MinusBTransactionIds = new List<long> { 100 },
                OutcomingEntryId = 100,
                IncomingEntryTypeId = 2,
                FromBankAccountId = 1,
                ToBankAccountId = 1,
            };

            WithUnitOfWork(async () =>
            {
                await Assert.ThrowsAsync<UserFriendlyException>(async () =>
                await _bTransactionAppService.ConversionTransaction(input));
            });
            await Task.CompletedTask;
        }
        #endregion

        #region Case 8: Không tìm thấy loại thu
        [Fact]
        public async Task ConversionTransaction_Test8()
        {
            var input = new ConversionTransactionDto
            {
                MinusBTransactionIds = new List<long> { 100 },
                PlusBTransactionIds = new List<long> { 101 },
                OutcomingEntryId = 100,
                IncomingEntryTypeId = 150,
                FromBankAccountId = 1,
                ToBankAccountId = 1,
            };
            WithUnitOfWork(async () =>
            {
                await Assert.ThrowsAsync<UserFriendlyException>(async () =>
                await _bTransactionAppService.ConversionTransaction(input));
            });
            await Task.CompletedTask;
        }
        #endregion

        #region Case 9: Cần xét 1 loại tiền làm loại tiền mặc định cho Request chi
        [Fact]
        public async Task ConversionTransaction_Test9()
        {
            var input = new ConversionTransactionDto
            {
                MinusBTransactionIds = new List<long> { 100 },
                PlusBTransactionIds = new List<long> { 101 },
                OutcomingEntryId = 100,
                IncomingEntryTypeId = 1,
                FromBankAccountId = 1,
                ToBankAccountId = 1,
            };
            WithUnitOfWork(async () =>
            {
                await Assert.ThrowsAsync<UserFriendlyException>(async () =>
                await _bTransactionAppService.ConversionTransaction(input));
            });
            await Task.CompletedTask;
        }
        #endregion

        #region Case 10: Không tồn tại tài khoản ngân hàng gửi
        [Fact]
        public async Task ConversionTransaction_Test10()
        {
            var input = new ConversionTransactionDto
            {
                MinusBTransactionIds = new List<long> { 100 },
                PlusBTransactionIds = new List<long> { 101 },
                OutcomingEntryId = 100,
                IncomingEntryTypeId = 1,
                FromBankAccountId = 150,
                ToBankAccountId = 1,
            };
            WithUnitOfWork(async () =>
            {
                await Assert.ThrowsAsync<UserFriendlyException>(async () =>
                await _bTransactionAppService.ConversionTransaction(input));
            });
            await Task.CompletedTask;
        }
        #endregion

        #region Case 11: Không tồn tại tài khoản ngân hàng nhận
        [Fact]
        public async Task ConversionTransaction_Test11()
        {
            var input = new ConversionTransactionDto
            {
                MinusBTransactionIds = new List<long> { 100 },
                PlusBTransactionIds = new List<long> { 101 },
                OutcomingEntryId = 100,
                IncomingEntryTypeId = 1,
                FromBankAccountId = 1,
                ToBankAccountId = 150,
            };
            WithUnitOfWork(async () =>
            {
                await Assert.ThrowsAsync<UserFriendlyException>(async () =>
                await _bTransactionAppService.ConversionTransaction(input));
            });
            await Task.CompletedTask;
        }
        #endregion

        #region Case 12: Không thể bán ngoại tệ với tài khoản ngân hàng GỬI là tài khoản thuộc công ty
        [Fact]
        public async Task ConversionTransaction_Test12()
        {
            var input = new ConversionTransactionDto
            {
                MinusBTransactionIds = new List<long> { 100 },
                PlusBTransactionIds = new List<long> { 101 },
                OutcomingEntryId = 100,
                IncomingEntryTypeId = 1,
                FromBankAccountId = 101,
                ToBankAccountId = 102,
            };
            WithUnitOfWork(async () =>
            {
                await Assert.ThrowsAsync<UserFriendlyException>(async () =>
                await _bTransactionAppService.ConversionTransaction(input));
            });
            await Task.CompletedTask;
        }
        #endregion

        #region Case 13: Không thể bán ngoại tệ với tài khoản ngân hàng NHẬN là tài khoản thuộc công ty
        [Fact]
        public async Task ConversionTransaction_Test13()
        {
            var input = new ConversionTransactionDto
            {
                MinusBTransactionIds = new List<long> { 100 },
                PlusBTransactionIds = new List<long> { 101 },
                OutcomingEntryId = 100,
                IncomingEntryTypeId = 1,
                FromBankAccountId = 102,
                ToBankAccountId = 101,
            };
            WithUnitOfWork(async () =>
            {
                await Assert.ThrowsAsync<UserFriendlyException>(async () =>
                await _bTransactionAppService.ConversionTransaction(input));
            });
            await Task.CompletedTask;
        }
        #endregion

        #region Case 14: Không thể bán ngoại tệ với tài khoản ngân hàng NHẬN có loại tiền khác Default
        [Fact]
        public async Task ConversionTransaction_Test14()
        {
            var input = new ConversionTransactionDto
            {
                MinusBTransactionIds = new List<long> { 100 },
                PlusBTransactionIds = new List<long> { 101 },
                OutcomingEntryId = 100,
                IncomingEntryTypeId = 1,
                FromBankAccountId = 103,
                ToBankAccountId = 102,
            };
            WithUnitOfWork(async () =>
            {
                await Assert.ThrowsAsync<UserFriendlyException>(async () =>
                await _bTransactionAppService.ConversionTransaction(input));
            });
            await Task.CompletedTask;
        }
        #endregion

        #region Case 15: Không thể bán ngoại tệ với tài khoản ngân hàng GỬI có loại tiền khác Default
        [Fact]
        public async Task ConversionTransaction_Test15()
        {
            var input = new ConversionTransactionDto
            {
                MinusBTransactionIds = new List<long> { 100 },
                PlusBTransactionIds = new List<long> { 101 },
                OutcomingEntryId = 100,
                IncomingEntryTypeId = 1,
                FromBankAccountId = 102,
                ToBankAccountId = 103,
            };
            WithUnitOfWork(async () =>
            {
                await Assert.ThrowsAsync<UserFriendlyException>(async () =>
                await _bTransactionAppService.ConversionTransaction(input));
            });
            await Task.CompletedTask;
        }
        #endregion

        #region Case 16: Không thể thực hiện thao tác vì không tìm thấy các biến động số dư
        [Fact]
        public async Task ConversionTransaction_Test16()
        {
            var input = new ConversionTransactionDto
            {
                MinusBTransactionIds = new List<long> { 100, 150 },
                PlusBTransactionIds = new List<long> { 101 },
                OutcomingEntryId = 100,
                IncomingEntryTypeId = 1,
                FromBankAccountId = 103,
                ToBankAccountId = 104,
            };
            WithUnitOfWork(async () =>
            {
                await Assert.ThrowsAsync<UserFriendlyException>(async () =>
                await _bTransactionAppService.ConversionTransaction(input));
            });
            await Task.CompletedTask;
        }
        #endregion

        #region Case 17: Không thể thực hiện thao tác vì có các biến động số dư đã DONE
        [Fact]
        public async Task ConversionTransaction_Test17()
        {
            var input = new ConversionTransactionDto
            {
                MinusBTransactionIds = new List<long> { 102 },
                PlusBTransactionIds = new List<long> { 101 },
                OutcomingEntryId = 100,
                IncomingEntryTypeId = 1,
                FromBankAccountId = 103,
                ToBankAccountId = 104,
            };
            WithUnitOfWork(async () =>
            {
                await Assert.ThrowsAsync<UserFriendlyException>(async () =>
                await _bTransactionAppService.ConversionTransaction(input));
            });
            await Task.CompletedTask;
        }
        #endregion

        #region Case 18: Các biến động số dư âm phải có cùng loại tiền
        [Fact]
        public async Task ConversionTransaction_Test18()
        {
            var input = new ConversionTransactionDto
            {
                MinusBTransactionIds = new List<long> { 103, 105 },
                PlusBTransactionIds = new List<long> { 101 },
                OutcomingEntryId = 100,
                IncomingEntryTypeId = 1,
                FromBankAccountId = 103,
                ToBankAccountId = 104,
            };
            WithUnitOfWork(async () =>
            {
                await Assert.ThrowsAsync<UserFriendlyException>(async () =>
                await _bTransactionAppService.ConversionTransaction(input));
            });
            await Task.CompletedTask;
        }
        #endregion

        #region Case 19: Các biến động số dư dương phải có cùng loại tiền
        [Fact]
        public async Task ConversionTransaction_Test19()
        {
            var input = new ConversionTransactionDto
            {
                MinusBTransactionIds = new List<long> { 101 },
                PlusBTransactionIds = new List<long> { 104, 106 },
                OutcomingEntryId = 100,
                IncomingEntryTypeId = 1,
                FromBankAccountId = 103,
                ToBankAccountId = 104,
            };
            WithUnitOfWork(async () =>
            {
                await Assert.ThrowsAsync<UserFriendlyException>(async () =>
                await _bTransactionAppService.ConversionTransaction(input));
            });
            await Task.CompletedTask;
        }
        #endregion

        #region Case 20: Tổng tiền biến động số dư lớn hơn số tiền chi khả dụng
        [Fact]
        public async Task ConversionTransaction_Test20()
        {
            var input = new ConversionTransactionDto
            {
                MinusBTransactionIds = new List<long> { 100 },
                PlusBTransactionIds = new List<long> { 130 },
                OutcomingEntryId = 111,
                IncomingEntryTypeId = 1,
                FromBankAccountId = 1,
                ToBankAccountId = 1,
            };
            WithUnitOfWork(async () =>
            {
                await Assert.ThrowsAsync<UserFriendlyException>(async () =>
                await _bTransactionAppService.ConversionTransaction(input));
            });
            await Task.CompletedTask;
        }
        #endregion

        #region Case 21: Select nhiều biến động số dư
        [Fact]
        public async Task ConversionTransaction_Test21()
        {
            var input = new ConversionTransactionDto
            {
                MinusBTransactionIds = new List<long> { 100, 107, 108 },
                PlusBTransactionIds = new List<long> { 101, 109, 110 },
                OutcomingEntryId = 140,
                IncomingEntryTypeId = 2,
                FromBankAccountId = 1,
                ToBankAccountId = 1,
            };

            WithUnitOfWork(async () =>
            {
                var allBankTransactionBefore = _workScope.GetAll<BankTransaction>().ToList();
                await _bTransactionAppService.ConversionTransaction(input);
                var allBankTransactionAfter = _workScope.GetAll<BankTransaction>().ToList();
                var checkStatusBTransaction100 = await _workScope.GetAsync<BTransaction>(100);
                var checkStatusBTransaction101 = await _workScope.GetAsync<BTransaction>(101);
                var checkStatusBTransaction107 = await _workScope.GetAsync<BTransaction>(107);
                var checkStatusBTransaction108 = await _workScope.GetAsync<BTransaction>(108);
                var checkStatusBTransaction109 = await _workScope.GetAsync<BTransaction>(109);
                var checkStatusBTransaction110 = await _workScope.GetAsync<BTransaction>(110);
                Assert.NotNull(input);

                allBankTransactionAfter.Count().ShouldBeGreaterThan(allBankTransactionBefore.Count());

                checkStatusBTransaction100.Status.ShouldBe(Enums.BTransactionStatus.DONE);
                checkStatusBTransaction101.Status.ShouldBe(Enums.BTransactionStatus.DONE);
                checkStatusBTransaction107.Status.ShouldBe(Enums.BTransactionStatus.DONE);
                checkStatusBTransaction108.Status.ShouldBe(Enums.BTransactionStatus.DONE);
                checkStatusBTransaction109.Status.ShouldBe(Enums.BTransactionStatus.DONE);
                checkStatusBTransaction110.Status.ShouldBe(Enums.BTransactionStatus.DONE);
            });

            await Task.CompletedTask;
        }
        #endregion

        // -- MUA NGOẠI TỆ --
        #region Case 1: DONE
        [Fact]
        public async Task MuaNgoaiTe_Test1()
        {
            var input = new ConversionTransactionDto
            {
                MinusBTransactionIds = new List<long> { 112 },
                PlusBTransactionIds = new List<long> { 111 },
                OutcomingEntryId = 101,
                IncomingEntryTypeId = 2,
                FromBankAccountId = 1,
                ToBankAccountId = 1,
            };

            WithUnitOfWork(async () =>
            {
                var allBankTransactionBefore = _workScope.GetAll<BankTransaction>().ToList();
                await _bTransactionAppService.MuaNgoaiTe(input);
                var allBankTransactionAfter = _workScope.GetAll<BankTransaction>().ToList();
                var checkStatusBTransaction112 = await _workScope.GetAsync<BTransaction>(112);
                var checkStatusBTransaction111 = await _workScope.GetAsync<BTransaction>(111);
                Assert.NotNull(input);

                allBankTransactionAfter.Count().ShouldBeGreaterThan(allBankTransactionBefore.Count());

                checkStatusBTransaction112.Status.ShouldBe(Enums.BTransactionStatus.DONE);
                checkStatusBTransaction111.Status.ShouldBe(Enums.BTransactionStatus.DONE);
            });

            await Task.CompletedTask;
        }
        #endregion

        #region Case 2 : Không có yêu cầu chi
        [Fact]
        public async Task MuaNgoaiTe_Test2()
        {
            var input = new ConversionTransactionDto
            {
                MinusBTransactionIds = new List<long> { 111 },
                PlusBTransactionIds = new List<long> { 112 },
                IncomingEntryTypeId = 2,
                FromBankAccountId = 1,
                ToBankAccountId = 1,
            };

            WithUnitOfWork(async () =>
            {
                await Assert.ThrowsAsync<UserFriendlyException>(async () =>
                await _bTransactionAppService.MuaNgoaiTe(input));
            });

            await Task.CompletedTask;
        }
        #endregion

        #region Case 3 : Không có tài khoản ngân hàng NHẬN
        [Fact]
        public async Task MuaNgoaiTe_Test3()
        {
            var input = new ConversionTransactionDto
            {
                MinusBTransactionIds = new List<long> { 111 },
                PlusBTransactionIds = new List<long> { 112 },
                IncomingEntryTypeId = 2,
                OutcomingEntryId = 101,
                FromBankAccountId = 1,
            };

            WithUnitOfWork(async () =>
            {
                await Assert.ThrowsAsync<UserFriendlyException>(async () =>
                await _bTransactionAppService.MuaNgoaiTe(input));
            });

            await Task.CompletedTask;
        }
        #endregion

        #region Case 4 : Không có tài khoản ngân hàng GỬI
        [Fact]
        public async Task MuaNgoaiTe_Test4()
        {
            var input = new ConversionTransactionDto
            {
                MinusBTransactionIds = new List<long> { 111 },
                PlusBTransactionIds = new List<long> { 112 },
                IncomingEntryTypeId = 2,
                OutcomingEntryId = 101,
                ToBankAccountId = 1,
            };

            WithUnitOfWork(async () =>
            {
                await Assert.ThrowsAsync<UserFriendlyException>(async () =>
                await _bTransactionAppService.MuaNgoaiTe(input));
            });

            await Task.CompletedTask;
        }
        #endregion

        #region Case 5 : Không có loại thu
        [Fact]
        public async Task MuaNgoaiTe_Test5()
        {
            var input = new ConversionTransactionDto
            {
                MinusBTransactionIds = new List<long> { 111 },
                PlusBTransactionIds = new List<long> { 112 },
                OutcomingEntryId = 101,
                FromBankAccountId = 1,
                ToBankAccountId = 1,
            };

            WithUnitOfWork(async () =>
            {
                await Assert.ThrowsAsync<UserFriendlyException>(async () =>
                await _bTransactionAppService.MuaNgoaiTe(input));
            });

            await Task.CompletedTask;
        }
        #endregion

        #region Case 6 : Không có biến động số dư âm
        [Fact]
        public async Task MuaNgoaiTe_Test6()
        {
            var input = new ConversionTransactionDto
            {
                PlusBTransactionIds = new List<long> { 112 },
                IncomingEntryTypeId = 2,
                OutcomingEntryId = 101,
                FromBankAccountId = 1,
                ToBankAccountId = 1,
            };

            WithUnitOfWork(async () =>
            {
                await Assert.ThrowsAsync<UserFriendlyException>(async () =>
                await _bTransactionAppService.MuaNgoaiTe(input));
            });

            await Task.CompletedTask;
        }
        #endregion

        #region Case 7 : Không có biến động số dư dương
        [Fact]
        public async Task MuaNgoaiTe_Test7()
        {
            var input = new ConversionTransactionDto
            {
                MinusBTransactionIds = new List<long> { 111 },
                IncomingEntryTypeId = 2,
                OutcomingEntryId = 101,
                FromBankAccountId = 1,
                ToBankAccountId = 1,
            };

            WithUnitOfWork(async () =>
            {
                await Assert.ThrowsAsync<UserFriendlyException>(async () =>
                await _bTransactionAppService.MuaNgoaiTe(input));
            });

            await Task.CompletedTask;
        }
        #endregion

        #region Case 8 : Không tìm thấy loại thu
        [Fact]
        public async Task MuaNgoaiTe_Test8()
        {
            var input = new ConversionTransactionDto
            {
                MinusBTransactionIds = new List<long> { 111 },
                PlusBTransactionIds = new List<long> { 112 },
                OutcomingEntryId = 101,
                IncomingEntryTypeId = 150,
                FromBankAccountId = 1,
                ToBankAccountId = 1,
            };

            WithUnitOfWork(async () =>
            {
                await Assert.ThrowsAsync<UserFriendlyException>(async () =>
                await _bTransactionAppService.MuaNgoaiTe(input));
            });

            await Task.CompletedTask;
        }
        #endregion

        #region Case 9 : Không tồn tại tài khoản ngân hàng gửi
        [Fact]
        public async Task MuaNgoaiTe_Test9()
        {
            var input = new ConversionTransactionDto
            {
                MinusBTransactionIds = new List<long> { 111 },
                PlusBTransactionIds = new List<long> { 112 },
                IncomingEntryTypeId = 2,
                OutcomingEntryId = 101,
                FromBankAccountId = 150,
                ToBankAccountId = 1,
            };

            WithUnitOfWork(async () =>
            {
                await Assert.ThrowsAsync<UserFriendlyException>(async () =>
                await _bTransactionAppService.MuaNgoaiTe(input));
            });

            await Task.CompletedTask;
        }
        #endregion

        #region Case 10 : Không tồn tại tài khoản ngân hàng nhận
        [Fact]
        public async Task MuaNgoaiTe_Test10()
        {
            var input = new ConversionTransactionDto
            {
                MinusBTransactionIds = new List<long> { 111 },
                PlusBTransactionIds = new List<long> { 112 },
                IncomingEntryTypeId = 2,
                OutcomingEntryId = 101,
                FromBankAccountId = 1,
                ToBankAccountId = 150,
            };

            WithUnitOfWork(async () =>
            {
                await Assert.ThrowsAsync<UserFriendlyException>(async () =>
                await _bTransactionAppService.MuaNgoaiTe(input));
            });

            await Task.CompletedTask;
        }
        #endregion

        #region Case 11 : Không thể bán ngoại tệ với tài khoản ngân hàng GỬI là tài khoản thuộc công ty
        [Fact]
        public async Task MuaNgoaiTe_Test11()
        {
            var input = new ConversionTransactionDto
            {
                MinusBTransactionIds = new List<long> { 111 },
                PlusBTransactionIds = new List<long> { 112 },
                IncomingEntryTypeId = 2,
                OutcomingEntryId = 101,
                FromBankAccountId = 101,
                ToBankAccountId = 1,
            };

            WithUnitOfWork(async () =>
            {
                await Assert.ThrowsAsync<UserFriendlyException>(async () =>
                await _bTransactionAppService.MuaNgoaiTe(input));
            });

            await Task.CompletedTask;
        }
        #endregion

        #region Case 12 : Không thể bán ngoại tệ với tài khoản ngân hàng NHẬN là tài khoản thuộc công ty
        [Fact]
        public async Task MuaNgoaiTe_Test12()
        {
            var input = new ConversionTransactionDto
            {
                MinusBTransactionIds = new List<long> { 111 },
                PlusBTransactionIds = new List<long> { 112 },
                IncomingEntryTypeId = 2,
                OutcomingEntryId = 101,
                FromBankAccountId = 1,
                ToBankAccountId = 101,
            };

            WithUnitOfWork(async () =>
            {
                await Assert.ThrowsAsync<UserFriendlyException>(async () =>
                await _bTransactionAppService.MuaNgoaiTe(input));
            });

            await Task.CompletedTask;
        }
        #endregion

        #region Case 13 : Không thể thực hiện thao tác vì không tìm thấy các biến động số dư
        [Fact]
        public async Task MuaNgoaiTe_Test13()
        {
            var input = new ConversionTransactionDto
            {
                MinusBTransactionIds = new List<long> { 111, 150 },
                PlusBTransactionIds = new List<long> { 112, 160 },
                IncomingEntryTypeId = 2,
                OutcomingEntryId = 101,
                FromBankAccountId = 1,
                ToBankAccountId = 1,
            };

            WithUnitOfWork(async () =>
            {
                await Assert.ThrowsAsync<UserFriendlyException>(async () =>
                await _bTransactionAppService.MuaNgoaiTe(input));
            });

            await Task.CompletedTask;
        }
        #endregion

        #region Case 14 : Không thể thực hiện thao tác vì có các biến động số dư đã DONE
        [Fact]
        public async Task MuaNgoaiTe_Test14()
        {
            var input = new ConversionTransactionDto
            {
                MinusBTransactionIds = new List<long> { 113 },
                PlusBTransactionIds = new List<long> { 112 },
                IncomingEntryTypeId = 2,
                OutcomingEntryId = 101,
                FromBankAccountId = 1,
                ToBankAccountId = 1,
            };

            WithUnitOfWork(async () =>
            {
                await Assert.ThrowsAsync<UserFriendlyException>(async () =>
                await _bTransactionAppService.MuaNgoaiTe(input));
            });

            await Task.CompletedTask;
        }
        #endregion

        #region Case 15 : Bạn cần xét 1 loại tiền làm loại tiền mặc định cho Request chi
        [Fact]
        public async Task MuaNgoaiTe_Test15()
        {
            var input = new ConversionTransactionDto
            {
                MinusBTransactionIds = new List<long> { 111 },
                PlusBTransactionIds = new List<long> { 112 },
                IncomingEntryTypeId = 2,
                OutcomingEntryId = 101,
                FromBankAccountId = 1,
                ToBankAccountId = 1,
            };

            WithUnitOfWork(async () =>
            {
                await Assert.ThrowsAsync<UserFriendlyException>(async () =>
                await _bTransactionAppService.MuaNgoaiTe(input));
            });

            await Task.CompletedTask;
        }
        #endregion

        #region Case 16 : Các biến động số dư âm phải có cùng loại tiền
        [Fact]
        public async Task MuaNgoaiTe_Test16()
        {
            var input = new ConversionTransactionDto
            {
                MinusBTransactionIds = new List<long> { 114, 116 },
                PlusBTransactionIds = new List<long> { 112 },
                IncomingEntryTypeId = 2,
                OutcomingEntryId = 101,
                FromBankAccountId = 1,
                ToBankAccountId = 1,
            };

            WithUnitOfWork(async () =>
            {
                await Assert.ThrowsAsync<UserFriendlyException>(async () =>
                await _bTransactionAppService.MuaNgoaiTe(input));
            });

            await Task.CompletedTask;
        }
        #endregion

        #region Case 17 : Các biến động số dương phải có cùng loại tiền - Lỗi Code
        [Fact]
        public async Task MuaNgoaiTe_Test17()
        {
            var input = new ConversionTransactionDto
            {
                MinusBTransactionIds = new List<long> { 111 },
                PlusBTransactionIds = new List<long> { 115, 117 },
                IncomingEntryTypeId = 2,
                OutcomingEntryId = 101,
                FromBankAccountId = 1,
                ToBankAccountId = 1,
            };

            WithUnitOfWork(async () =>
            {
                await Assert.ThrowsAsync<UserFriendlyException>(async () =>
                await _bTransactionAppService.MuaNgoaiTe(input));
            });

            await Task.CompletedTask;
        }
        #endregion

        #region Case 18 : Tổng tiền biến động số dư lớn hơn số tiền chi khả dụng
        [Fact]
        public async Task MuaNgoaiTe_Test18()
        {
            var input = new ConversionTransactionDto
            {
                MinusBTransactionIds = new List<long> { 131 },
                PlusBTransactionIds = new List<long> { 101 },
                OutcomingEntryId = 112,
                IncomingEntryTypeId = 1,
                FromBankAccountId = 1,
                ToBankAccountId = 1,
            };
            WithUnitOfWork(async () =>
            {
                await Assert.ThrowsAsync<UserFriendlyException>(async () =>
                await _bTransactionAppService.CheckMuaNgoaiTe(input));
            });
            await Task.CompletedTask;
        }
        #endregion

        #region Case 19: Select nhiều biến động số dư
        [Fact]
        public async Task MuaNgoaiTe_Test19()
        {
            var input = new ConversionTransactionDto
            {
                MinusBTransactionIds = new List<long> { 112, 118, 119 },
                PlusBTransactionIds = new List<long> { 111, 120, 121 },
                OutcomingEntryId = 141,
                IncomingEntryTypeId = 2,
                FromBankAccountId = 1,
                ToBankAccountId = 1,
            };

            WithUnitOfWork(async () =>
            {
                var allBankTransactionBefore = _workScope.GetAll<BankTransaction>().ToList();
                await _bTransactionAppService.MuaNgoaiTe(input);
                var allBankTransactionAfter = _workScope.GetAll<BankTransaction>().ToList();
                var checkStatusBTransaction112 = await _workScope.GetAsync<BTransaction>(112);
                var checkStatusBTransaction111 = await _workScope.GetAsync<BTransaction>(111);
                var checkStatusBTransaction118 = await _workScope.GetAsync<BTransaction>(118);
                var checkStatusBTransaction119 = await _workScope.GetAsync<BTransaction>(119);
                var checkStatusBTransaction120 = await _workScope.GetAsync<BTransaction>(120);
                var checkStatusBTransaction121 = await _workScope.GetAsync<BTransaction>(121);
                Assert.NotNull(input);

                allBankTransactionAfter.Count().ShouldBeGreaterThan(allBankTransactionBefore.Count());

                checkStatusBTransaction112.Status.ShouldBe(Enums.BTransactionStatus.DONE);
                checkStatusBTransaction111.Status.ShouldBe(Enums.BTransactionStatus.DONE);
                checkStatusBTransaction118.Status.ShouldBe(Enums.BTransactionStatus.DONE);
                checkStatusBTransaction119.Status.ShouldBe(Enums.BTransactionStatus.DONE);
                checkStatusBTransaction120.Status.ShouldBe(Enums.BTransactionStatus.DONE);
                checkStatusBTransaction121.Status.ShouldBe(Enums.BTransactionStatus.DONE);
            });

            await Task.CompletedTask;
        }
        #endregion
    }
}
