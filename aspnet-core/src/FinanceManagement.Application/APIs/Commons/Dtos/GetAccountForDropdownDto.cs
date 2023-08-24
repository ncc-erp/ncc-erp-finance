using FinanceManagement.Entities;
using FinanceManagement.Enums;
using FinanceManagement.GeneralModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace FinanceManagement.APIs.Commons.Dtos
{
    public class GetAccountForDropdownDto : ValueAndNameDto
    {
        public AccountTypeEnum AccountType { get; set; }

    }
    public class GetAccountCompanyForDropdownDto : ValueAndNameDto
    {
        public bool IsDefault { get; set; }

    }
}
