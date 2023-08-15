using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using FinanceManagement.Entities;
using System.ComponentModel.DataAnnotations;

namespace FinanceManagement.APIs.BankAccounts.Dto
{
    [AutoMapTo(typeof(BankAccount))]
    public class BankAccountDto : EntityDto<long>
    {
        [Required]
        public string HolderName { get; set; }
        public string BankNumber { get; set; }
        public long? BankId { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "Loại tiền không được để trống.")]
        public long CurrencyId { get; set; }
        [Required]
        public long AccountId { get; set; }
        public bool IsActive { get; set; } = true;
        public double Amount { get; set; } = 0;
        public double BaseBalance { get; set; }
    }
}

