using Abp.UI;
using FinanceManagement.APIs.BTransactions;
using FinanceManagement.Entities;
using FinanceManagement.Entities.NewEntities;
using FinanceManagement.Helper;
using FinanceManagement.Managers.BTransactions;
using FinanceManagement.Managers.BTransactions.Dtos;
using Microsoft.EntityFrameworkCore;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace FinanceManagement.Core.Tests.Managers
{
    public class BTransactionManager_Test : FinfastCoreTestBase
    {
        private readonly IBTransactionManager _bTransactionManager;
        public BTransactionManager_Test()
        {
            _bTransactionManager = Resolve<IBTransactionManager>();
        }

        
    }
}
