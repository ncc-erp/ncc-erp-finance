using Abp.AutoMapper;
using Abp.Domain.Entities;
using FinanceManagement.Entities.NewEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FinanceManagement.Managers.Periods.Dtos
{
    [AutoMapTo(typeof(Period))]
    public class FormPeriod : Entity<int>
    {
        public bool IsActive { get; set; }
        public string Name { get; set; }
        public DateTime StartDate { get; set; }
    }

    public class CreatePeriodAndPeriodBankAccountDto 
    {
        public string Name { get; set; }
        public List<CreatePeriodBankAccountDto> PeriodBankAccounts { get; set; }
    }

    [AutoMapTo(typeof (Period))]
    public class UpdatePeriodDto
    {
        public string Name { get; set; }
    } 

    [AutoMapTo(typeof(Period))]
    public class CreatePeriodForTheFirstTime
    {
        public string Name { get; set; }
        public DateTime StartDate { get; set; }
        public List<CreatePeriodBankAccountTheFirstTime> PeriodBankAccounts { get; set; }
    }
}
