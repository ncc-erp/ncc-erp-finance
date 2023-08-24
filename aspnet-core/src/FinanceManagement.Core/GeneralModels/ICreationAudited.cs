using System;
using System.Collections.Generic;
using System.Text;

namespace FinanceManagement.GeneralModels
{
    public interface ICustomCreationAudited
    {
        public DateTime CreationTime { get; set; }
        public long? CreationUserId { get; set; }
        public string CreationUser { get; set; }
    }
}
