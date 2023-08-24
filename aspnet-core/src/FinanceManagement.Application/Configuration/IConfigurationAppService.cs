using System.Threading.Tasks;
using FinanceManagement.Configuration.Dto;

namespace FinanceManagement.Configuration
{
    public interface IConfigurationAppService
    {
        Task ChangeUiTheme(ChangeUiThemeInput input);
    }
}
