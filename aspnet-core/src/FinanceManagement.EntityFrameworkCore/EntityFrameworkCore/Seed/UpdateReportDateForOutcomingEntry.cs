using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FinanceManagement.EntityFrameworkCore.Seed
{
    public class UpdateReportDateForOutcomingEntry
    {
        private readonly FinanceManagementDbContext _context;
        public UpdateReportDateForOutcomingEntry(FinanceManagementDbContext context)
        {
            _context = context;
        }
        public void Update()
        {
            _context.OutcomingEntries
                .IgnoreQueryFilters()
                .Where(x => !x.ReportDate.HasValue && x.ExecutedTime.HasValue)
                .ToList()
                .ForEach(item => item.ReportDate = item.ExecutedTime);
            _context.SaveChanges();
        }
    }
}
