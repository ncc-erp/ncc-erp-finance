using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using FinanceManagement.Anotations;
using FinanceManagement.Entities;
using FinanceManagement.Helper;
using FinanceManagement.Paging;
using System;
using System.Collections.Generic;
using System.Text;

namespace FinanceManagement.Managers.CurrenciesConvert.Dto
{
    public class InputToFilterCurrencyConvert : GridParam
    {
        public int Month { get; set; }
        public int Year { get; set; }
    }
    public class CurrenciesConvertDto : EntityDto<long>
    {
        public DateTime DateAt { get; set; }
        [ApplySearchAttribute]
        public string CurrencyName { get; set; }
        public long CurrencyId { get; set; }
        public double Value { get; set; }
        public string ValueFormat => Helpers.FormatMoney8PartAfterDot(Value);
    }

    [AutoMapTo(typeof(CurrencyConvert))]
    public class CreateCurrencyConvertDto:EntityDto<long>
    {
        public int Year { get; set; }
        public long CurrencyId { get; set; }
        public double Value { get; set; }
    }

    public class UpdateCurrencyConvertDto
    {
        public long Id { get; set; }
        public double Value { get; set; }
    }
}
