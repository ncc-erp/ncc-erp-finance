using FinanceManagement.Managers.Invoices;
using FinanceManagement.Managers.Invoices.Dtos;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace FinanceManagement.Core.Tests.Managers
{
    public class InvoiceManager_Tests : FinfastCoreTestBase
    {
        private readonly IInvoiceManager _invoiceManager;
        public InvoiceManager_Tests()
        {
            _invoiceManager = Resolve<IInvoiceManager>();
        }
        [Fact]
        public void Create_Invoice_Test()
        {
            var input = new CreateInvoiceDto()
            {
                AccountId = 3,
                CollectionDebt = 50000,
                Deadline = DateTime.UtcNow,
                Month = 1,
                NameInvoice = "Thu invoice tháng 1",
                Note = "Cần thu nợ gấp",
                CurrencyId = 1,
                InvoiceNumber = "234",
                NTF = 23565,
                Year = 2022
            };
            WithUnitOfWork(async () =>
            {
                var result = await _invoiceManager.CreateInvoice(input);
                result.ShouldNotBeNull();
            });
        }
    }
}
