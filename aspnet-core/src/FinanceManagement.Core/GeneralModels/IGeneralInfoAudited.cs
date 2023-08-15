using System;
using System.Collections.Generic;
using System.Text;

namespace FinanceManagement.GeneralModels
{
    public interface IGeneralInfoAudited : ICustomCreationAudited,ILastModifiedAudited
    {
        public string UpdatedBy { get; }
        public DateTime UpdatedTime { get; }
    }
}
