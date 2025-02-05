using Abp.Dependency;
using FinanceManagement.Notifications.Mezon.Dto;
using System.Threading.Tasks;

namespace FinanceManagement.Notifications.Mezon
{
    public interface IMezonNotification : INotification
    {       void NotifyWithMezonMessage(MezonMessage message, int? tenantId = int.MinValue);
    }
}
