﻿using Abp.AutoMapper;
using FinanceManagement.Authentication.External;

namespace FinanceManagement.Models.TokenAuth
{
    [AutoMapFrom(typeof(ExternalLoginProviderInfo))]
    public class ExternalLoginProviderInfoModel
    {
        public string Name { get; set; }

        public string ClientId { get; set; }
    }
}
