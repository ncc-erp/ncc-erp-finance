using FinanceManagement.Paging;

namespace FinanceManagement.Users.Dto
{
    public class PagedUserResultRequestDto : GridParam
    {
        public string Keyword { get; set; }
        public bool? IsActive { get; set; }
    }
}
