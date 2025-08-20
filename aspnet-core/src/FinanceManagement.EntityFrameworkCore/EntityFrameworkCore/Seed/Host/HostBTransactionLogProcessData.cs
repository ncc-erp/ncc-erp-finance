using Abp.Dependency;
using FinanceManagement.Services;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FinanceManagement.EntityFrameworkCore.Seed.Host
{
    public class HostBTransactionLogProcessData
    {
        private readonly ILogger<HostBTransactionLogProcessData> _log;
        private readonly FinanceManagementDbContext _context;
        private const string IS_BTRANSACTIONLOG_EXECUTED = "IS_BTRANSACTIONLOG_EXECUTED";
        public HostBTransactionLogProcessData(FinanceManagementDbContext context)
        {
            _context = context;
            _log = IocManager.Instance.Resolve<ILogger<HostBTransactionLogProcessData>>();
        }


        public void ProcessData()
        {
            _log.LogInformation("ProcessData()");
            var isExecutedSetting = _context.Settings
                .Where(s => s.TenantId == null && s.UserId == null && s.Name == IS_BTRANSACTIONLOG_EXECUTED)                
                .FirstOrDefault();

            if (isExecutedSetting != default && isExecutedSetting.Value == "true")
            {
                _log.LogInformation("ProcessData() isExecuted == true -> stop");
                return;
            }
            var deleteBTransactionLogs = _context.BTransactionLogs
                .Where(s => !s.IsValid && s.TimeAt >= new DateTime(2025, 08, 14))
                .ToList();

            deleteBTransactionLogs.ForEach(s => s.IsDeleted = true);

            if (isExecutedSetting != default)
            {
                isExecutedSetting.Value = "true";
            }
            else
            {
                var setting = new Abp.Configuration.Setting
                {
                    TenantId = null,
                    CreationTime = DateTime.Now,
                    CreatorUserId = null,
                    Name = IS_BTRANSACTIONLOG_EXECUTED,
                    Value = "true",
                    UserId = null,
                };
                _context.Settings.Add(setting);
            }
            
            _context.SaveChanges();

            _log.LogInformation("ProcessData() done. deleteBTransactionLogs.Count = " + deleteBTransactionLogs.Count);
            _log.LogInformation("ProcessData() done. deleteBTransactionLogs: " + JsonConvert.SerializeObject(deleteBTransactionLogs));

        }

    }
}
