using Abp.Dependency;
using FinanceManagement.Managers.Settings.Dtos;
using FinanceManagement.Managers.Settings.Intefaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace FinanceManagement.Managers.Settings
{
    public interface IMySettingManager : IInvoiceSetting, IOutcomingEntrySetting, IKomuSetting, ISingletonDependency, IRequestChiSetting
    {
        bool GetAllowChangeEntityInPeriodClosed();
        void SetAllowChangeEntityInPeriodClosed(string config);
    }
}
