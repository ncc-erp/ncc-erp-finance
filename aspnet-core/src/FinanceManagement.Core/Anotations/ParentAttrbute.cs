using System;
using System.Collections.Generic;
using System.Text;

namespace FinanceManagement.Anotations
{
    [AttributeUsage(AttributeTargets.Property)]
    public class ParentAttrbute : Attribute
    {
        public Type ParentType { get; set; }
    }
}
