using System.ComponentModel.DataAnnotations;

namespace FinanceManagement.Users.Dto
{
    public class ChangeUserLanguageDto
    {
        [Required]
        public string LanguageName { get; set; }
    }
}