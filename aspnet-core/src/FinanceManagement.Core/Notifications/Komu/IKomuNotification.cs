using Abp.Dependency;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace FinanceManagement.Notifications.Komu
{
    public interface IKomuNotification : ITransientDependency
    {
        void NotifyWithMessage(string message, int? tenantId = int.MinValue);
        Task NotifyByMessageAsync(string message, int? tenantId = int.MinValue);
        void NotifySalary(long outcomingEntryId);
        Task NotifySalaryAsync(long outcomingEntryId);
        void NotifyTeamBuilding(long outcomingEntryId);
        Task NotifyTeamBuildingAsync(long outcomingEntryId);
        void NotifyChangeStatus(long outcomingEntryId, string statusCode);
        Task NotifyChangeStatusAsync(long outcomingEntryId, string statusCode);
        void NotifyRequestChangePending(long tempOutcomingEntryId, string transitionName);
        Task NotifyRequestChangePendingAsync(long tempOutcomingEntryId, string transitionName);
        void NotifyRequestChangeReject(long tempOutcomingEntryId, string transitionName);
        Task NotifyRequestChangeRejectAsync(long tempOutcomingEntryId, string transitionName);
        void NotifyRequestChangeApprove(long tempOutcomingEntryId, string transitionName);
        Task NotifyRequestChangeApproveAsync(long tempOutcomingEntryId, string transitionName);
    }
}
