using Abp.Application.Services;
using FinanceManagement.MultiTenancy.Dto;

namespace FinanceManagement.MultiTenancy
{
    public interface ITenantAppService : IAsyncCrudAppService<TenantDto, int, PagedTenantResultRequestDto, CreateTenantDto, TenantDto>
    {
    }
}

