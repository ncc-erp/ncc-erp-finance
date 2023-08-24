using System;
using System.Collections.Generic;
using System.Text;

namespace FinanceManagement.Enums
{
    public enum RevenueManagedStatus : byte
    {
        Not_Yet = 0,
        Paid_Part = 1,
        Done = 2,
        Not_Paid = 3,
        Only_Paid_Part = 4
    }
    public enum RemindStatus: byte
    {
        First_Time = 1,
        Second_Time = 2,
        Third_Time = 3
    }
    public enum NInvoiceStatus: byte
    {
        CHUA_TRA = 0,
        TRA_1_PHAN = 1,
        HOAN_THANH = 2,
        KHONG_TRA = 3,
        CHI_TRA_1_PHAN = 4
    }
}
