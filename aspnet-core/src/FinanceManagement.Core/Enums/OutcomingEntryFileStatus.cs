using System;
using System.Collections.Generic;
using System.Text;

namespace FinanceManagement.Enums
{
    public enum OutcomingEntryFileStatus : byte
    {
        NoFileYet = 0,
        NotYetConfirmed = 1,
        Confirmred = 2,
    }
}
