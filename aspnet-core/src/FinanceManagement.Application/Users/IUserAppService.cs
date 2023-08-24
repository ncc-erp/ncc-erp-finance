using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using FinanceManagement.Roles.Dto;
using FinanceManagement.Users.Dto;

namespace FinanceManagement.Users
{
    public interface IUserAppService : IAsyncCrudAppService<UserDto, long, PagedUserResultRequestDto, CreateUserDto, UserDto>
    {
        Task<ListResultDto<RoleDto>> GetRoles();

        Task ChangeLanguage(ChangeUserLanguageDto input);

        Task<bool> ChangePassword(ChangePasswordDto input);
    }
}
