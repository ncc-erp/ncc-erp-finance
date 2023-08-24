using Abp.Authorization;
using AutoMapper;
using FinanceManagement.APIs.Explanations.Dto;
using FinanceManagement.Authorization;
using FinanceManagement.Entities;
using FinanceManagement.Enums;
using FinanceManagement.IoC;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceManagement.APIs.Explanations
{
    [AbpAuthorize]
    public class ExplanationAppService : FinanceManagementAppServiceBase
    {
        public ExplanationAppService(IWorkScope workScope) : base(workScope)
        {
        }

        [AbpAuthorize(PermissionNames.Finance_ComparativeStatistic_View)]
        public async Task<List<ExplanationDto>> GetAllByType(DateTime? startDate, DateTime? endDate, ExplanationType type)
        {
            var result = await (from c in WorkScope.GetAll<ComparativeStatistic>()
                                .Where(x => !startDate.HasValue || x.StartDate.Value.Date == startDate.Value.Date)
                                .Where(x => !endDate.HasValue || x.EndDate.Value.Date == endDate.Value.Date)
                                join ex in WorkScope.GetAll<Explanation>()
                                .Where(x => x.Type == type)
                                on c.Id equals ex.ComparativeStatisticId
                                select new ExplanationDto
                                {
                                    BankAccountId = ex.BankAccountId,
                                    BankAccountExplanation = ex.BankAccountExplanation,
                                    Type = ex.Type,
                                    ComparativeStatisticId = c.Id
                                }).ToListAsync();
            return result;
        }

       // [AbpAuthorize(PermissionNames.Finance_ComparativeStatistic_Explanation_Update)]
        public async Task<ExplanationDto> Update(ExplanationDto input)
        {
            //var explanation = await WorkScope.GetAll<Explanation>().Where(x => x.ComparativeStatisticId == input.ComparativeStatisticId && x.Type == input.Type).FirstOrDefaultAsync();
            var explanation = await WorkScope.GetAsync<Explanation>(input.Id);
            await WorkScope.UpdateAsync(ObjectMapper.Map<ExplanationDto, Explanation>(input, explanation));
            
            return input;
        }
    }
}
