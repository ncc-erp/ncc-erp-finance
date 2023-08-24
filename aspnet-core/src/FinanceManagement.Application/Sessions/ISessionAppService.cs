using System.Threading.Tasks;
using Abp.Application.Services;
using FinanceManagement.Sessions.Dto;

namespace FinanceManagement.Sessions
{
    public interface ISessionAppService : IApplicationService
    {
        Task<GetCurrentLoginInformationsOutput> GetCurrentLoginInformations();
    }
}
