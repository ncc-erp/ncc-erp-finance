using Abp.Dependency;
using FinanceManagement.Managers.Invoices.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceManagement.Managers.Invoices
{
    public interface IInvoiceManager : ITransientDependency
    {
        IQueryable<AllPropInvoiceAndIncomByAccountDto> IQGetAllInvoice();
        Task<GetInvoiceByIdDto> GetById(long id);
        Task UpdateNote(UpdateNoteInvoiceDto input);
        Task UpdateStatus(UpdateStatusInvoiceDto input);
        Task<CheckAutoPaidDto> CheckAutoPaid(long accountId);
        Task<GetInvoiceByIdDto> CreateInvoice(CreateInvoiceDto input);
        Task<GetInvoiceByIdDto> UpdateInvoice(UpdateInvoiceDto input);
        Task Delete(long id);
        Task<CheckSetDoneInvoiceDto> CheckSetDoneInvoice(long invoiceId);
        Task SetDoneInvoice(long invoiceId);
        Task SetClientPayDeviant(ClientPayDeviantDto input);
        Task<byte[]> ExportReport(OverviewInvoiceDto overviewInvoiceDto);
    }
}
