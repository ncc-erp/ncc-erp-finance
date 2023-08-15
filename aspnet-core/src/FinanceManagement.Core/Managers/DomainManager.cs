using Abp.Domain.Services;
using Castle.Core.Logging;
using FinanceManagement.Entities;
using FinanceManagement.IoC;
using FinanceManagement.Managers.Settings;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceManagement.Managers
{
    public class DomainManager : DomainService
    {
        protected IWorkScope _ws { get; set; }
        protected readonly ILogger _logger;
        public IMySettingManager MySettingManager { get; set; }
        public DomainManager(IWorkScope ws)
        {
            _ws = ws;
            _logger = NullLogger.Instance;
        }
        protected virtual async Task<Currency> GetCurrencyDefaultAsync()
        {
            return await _ws.GetAll<Currency>()
                .Where(x => x.IsCurrencyDefault)
                .FirstOrDefaultAsync();
        }
        protected async Task<bool> IsAllowOutcomingEntryByMultipleCurrency()
        {
            var config = await MySettingManager.GetApplyToMultiCurrencyOutcome();
            if(bool.TryParse(config, out var result))
            {
                return result;
            }
            return false;
        }
    }
}
