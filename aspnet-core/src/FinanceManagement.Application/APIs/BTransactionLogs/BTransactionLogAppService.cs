using FinanceManagement.Entities.NewEntities;
using FinanceManagement.IoC;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using FinanceManagement.APIs.BTransactionLogs.Dtos;
using FinanceManagement.Paging;
using FinanceManagement.Extension;
using Abp.Linq.Extensions;

namespace FinanceManagement.APIs.BTransactionLogs
{
    public class BTransactionLogAppService : FinanceManagementAppServiceBase
    {
        public BTransactionLogAppService(IWorkScope workScope) : base(workScope)
        {
        }
        [HttpPost]
        public async Task<GridResult<BTransactionLogDto>> GetAllPaging(BTransactionLogGridParam gridParams)
        {
            var query = WorkScope.GetAll<BTransactionLog>()
                .Select(x => new BTransactionLogDto
                {
                    Id = x.Id,
                    Content = x.Message,
                    CreationTime = x.CreationTime,
                    IsValid = x.IsValid,
                    ErrorMessage = x.ErrorMessage,
                    TimeAt = x.TimeAt,
                    Key = x.Key,
                });
            if (gridParams.FilterDateTimeParam != null)
            {
                query = query.WhereIf(gridParams.FilterDateTimeParam.FromDate.HasValue, x => x.TimeAt.Date >= gridParams.FilterDateTimeParam.FromDate.Value.Date)
                             .WhereIf(gridParams.FilterDateTimeParam.ToDate.HasValue, x => x.TimeAt.Date <= gridParams.FilterDateTimeParam.ToDate.Value.Date);
            }
            return await query.OrderByDescending(x => x.CreationTime).GetGridResult(query,gridParams);
        }
    }
}
