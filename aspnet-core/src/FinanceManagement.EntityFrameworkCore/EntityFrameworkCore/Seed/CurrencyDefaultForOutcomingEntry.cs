using FinanceManagement.Extension;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FinanceManagement.EntityFrameworkCore.Seed
{
    public class CurrencyDefaultForOutcomingEntry
    {
        private readonly FinanceManagementDbContext _context;
        public CurrencyDefaultForOutcomingEntry(FinanceManagementDbContext context)
        {
            this._context = context;
        }
        public void Update()
        {
            var currencyDefault = _context.Currencies
                .IgnoreQueryFilters()
                .Where(x => x.IsCurrencyDefault && !x.TenantId.HasValue)
                .FirstOrDefault();
            if (!currencyDefault.IsNullOrDefault())
                return;

            var currencyIdOutcoming = _context.OutcomingEntries
                .IgnoreQueryFilters()
                .Where(x => !x.TenantId.HasValue && x.CurrencyId.HasValue)
                .Select(x => x.CurrencyId)
                .FirstOrDefault();

            var currency = _context.Currencies
                .IgnoreQueryFilters()
                .Where(x => x.Id == currencyIdOutcoming)
                .FirstOrDefault();

            if (currency.IsNullOrDefault())
                return;

            currency.IsCurrencyDefault = true;

            _context.SaveChanges();
        }
    }
}
