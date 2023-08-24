using Abp.Net.Mail;
using FinanceManagement.Services.Komu;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace FinanceManagement.Notifications.Mail
{
    public class MailNotification : IMailNotification
    {
        public void SendSalaryToCEO(long outcomingEntryId)
        {
            throw null;
        }

        public async Task SendSalaryToCEOAsync(long outcomingEntryId)
        {
            await Task.CompletedTask;
            throw null;
        }
    }
}
