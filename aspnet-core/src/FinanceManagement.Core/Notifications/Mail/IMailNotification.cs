using Abp.Dependency;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace FinanceManagement.Notifications.Mail
{
    public interface IMailNotification : ITransientDependency
    {
        void SendSalaryToCEO(long outcomingEntryId);
        Task SendSalaryToCEOAsync(long outcomingEntryId);
    }
}
