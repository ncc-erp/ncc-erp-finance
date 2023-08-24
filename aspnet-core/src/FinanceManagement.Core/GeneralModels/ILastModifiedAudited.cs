using System;
using System.Collections.Generic;
using System.Text;

namespace FinanceManagement.GeneralModels
{
    public interface ILastModifiedAudited : ILastModifiedTimeAudited
    {
        public long? LastModifiedUserId { get; set; }
        public string LastModifiedUser { get; set; }
    }
    public interface ILastModifiedTimeAudited
    {
        DateTime? LastModifiedTime { get; set; }
    }
}
