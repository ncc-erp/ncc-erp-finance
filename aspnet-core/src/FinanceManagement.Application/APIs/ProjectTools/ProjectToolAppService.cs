using FinanceManagement.APIs.ProjectApis.Dto;
using FinanceManagement.APIs.ProjectTools.Dtos;
using FinanceManagement.Authorization;
using FinanceManagement.Entities;
using FinanceManagement.Enums;
using FinanceManagement.IoC;
using FinanceManagement.Managers.Invoices;
using FinanceManagement.Managers.Invoices.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceManagement.APIs.ProjectTools
{
    public class ProjectToolAppService : FinanceManagementAppServiceBase
    {
        private readonly IInvoiceManager _invoiceManager;
        public ProjectToolAppService(IWorkScope workScope, IInvoiceManager invoiceManager) : base(workScope)
        {
            _invoiceManager = invoiceManager;
        }
        
        [HttpPost]
        [NccAuth]
        public async Task<ResponseResultProjectDto> CreateAllInvoices(List<CreateInvoiceFromProjectDto> input)
        {
            using (CurrentUnitOfWork.SetTenantId(AbpSession.TenantId))
            {
                var dicAccounts = WorkScope.GetAll<Account>()
                .Where(s => s.Type == AccountTypeEnum.CLIENT)
                .Select(s => new { s.Id, s.Code })
                .AsEnumerable()
                .GroupBy(x => x.Code)
                .ToDictionary(x => x.Key, x => x.Select(s => s.Id).FirstOrDefault());

                var clientCodes = input.Select(s => s.ClientCode).ToList();
                var validAccounts = ValidClientCode(dicAccounts, clientCodes);
                if (validAccounts.Any())
                    return new ResponseResultProjectDto
                    {
                        IsSuccess = false,
                        Message = $"Not Found Client Code: {string.Join(", ", validAccounts)}"
                    };

                var dicCurrencies = WorkScope.GetAll<Currency>()
                    .Select(s => new { s.Id, s.Code })
                    .AsEnumerable()
                    .GroupBy(x => x.Code)
                    .ToDictionary(x => x.Key, x => x.Select(s => s.Id).FirstOrDefault());

                var currencyCodes = input.Select(s => s.CurrencyCode).ToList();
                var validCurrencies = ValidCurrencyCode(dicCurrencies, currencyCodes);
                if (validCurrencies.Any())
                    return new ResponseResultProjectDto
                    {
                        IsSuccess = false,
                        Message = $"Not Found Currency Code: {string.Join(", ", validCurrencies)}"
                    };

                short month = input.FirstOrDefault().Month;
                int year = input.FirstOrDefault().Year;
                var oldInvoices = await WorkScope.GetAll<Invoice>()
                    .Where(s => s.Year == year && s.Month == month)
                    .ToListAsync();

                foreach (var invoice in input)
                {
                    if (!oldInvoices.Any(s => s.InvoiceNumber == invoice.InvoiceNumber))
                    {
                        await _invoiceManager.CreateInvoice(new CreateInvoiceDto
                        {
                            AccountId = dicAccounts[invoice.ClientCode],
                            CollectionDebt = invoice.CollectionDebt,
                            Deadline = invoice.Deadline,
                            Month = invoice.Month,
                            NameInvoice = invoice.NameInvoice,
                            NTF = invoice.TransferFee,
                            CurrencyId = dicCurrencies[invoice.CurrencyCode],
                            Year = invoice.Year,
                            InvoiceNumber = invoice.InvoiceNumber,
                        });
                        continue;
                    }

                    var oldInvoice = oldInvoices.Where(s => s.InvoiceNumber == invoice.InvoiceNumber && s.Status == NInvoiceStatus.CHUA_TRA).FirstOrDefault();
                    if (oldInvoice == null)
                        continue;

                    oldInvoice.AccountId = dicAccounts[invoice.ClientCode];
                    oldInvoice.CollectionDebt = invoice.CollectionDebt;
                    oldInvoice.Deadline = invoice.Deadline;
                    oldInvoice.Month = invoice.Month;
                    oldInvoice.NameInvoice = invoice.NameInvoice;
                    oldInvoice.NTF = invoice.TransferFee;
                    oldInvoice.CurrencyId = dicCurrencies[invoice.CurrencyCode];
                    oldInvoice.Year = invoice.Year;
                    oldInvoice.InvoiceNumber = invoice.InvoiceNumber;
                }

                await CurrentUnitOfWork.SaveChangesAsync();

                return new ResponseResultProjectDto
                {
                    IsSuccess = true,
                    Message = "Created Successfully"
                };
            }
        }
        private List<string> ValidClientCode(Dictionary<string, long> dicClientCode, List<string> clientCodes)
        {
            var results = new List<string>();
            if (dicClientCode == null) return results;
            foreach (var clientCode in clientCodes)
            {
                if(!dicClientCode.ContainsKey(clientCode))
                    results.Add(clientCode);
            } 
            return results;
        }
        private List<string> ValidCurrencyCode(Dictionary<string, long> dicCurrencyCode, List<string> currencyCodes)
        {
            var results = new List<string>();
            if (dicCurrencyCode == null) return results;
            foreach (var currencyCode in currencyCodes)
            {
                if(!dicCurrencyCode.ContainsKey(currencyCode))
                    results.Add(currencyCode);
            }
            return results;
        }
    }
}
