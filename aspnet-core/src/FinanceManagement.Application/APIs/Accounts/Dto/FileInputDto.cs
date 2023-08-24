using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace FinanceManagement.Accounts.Dto
{
    public class FileInputDto
    {
        public IFormFile File { get; set; }
    }
}
