using Abp.Authorization;
using Abp.UI;
using FinanceManagement.APIs.OutcomingEntrySuppliers.Dto;
using FinanceManagement.Authorization;
using FinanceManagement.Entities;
using FinanceManagement.IoC;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceManagement.APIs.OutcomingEntrySuppliers
{
    [AbpAuthorize]
    public class OutcomingEntrySupplierAppService : FinanceManagementAppServiceBase
    {
        public OutcomingEntrySupplierAppService(IWorkScope workScope) : base(workScope)
        {
        }

        [HttpPost]
        [AbpAuthorize(PermissionNames.Finance_OutcomingEntry_OutcomingEntryDetail_TabSupplier_LinkToSupplier)]
        public async Task<OutcomingEntrySupplierDto> Create(OutcomingEntrySupplierDto Input)
        {
            var isExist = await WorkScope.GetAll<OutcomingEntrySupplier>().AnyAsync(s => s.OutcomingEntryId == Input.OutcomingEntryId && s.SupplierId == Input.SupplierId);

            if (isExist)
                throw new UserFriendlyException(string.Format("Link to Supplier already existed"));

            Input.Id = await WorkScope.InsertAndGetIdAsync(ObjectMapper.Map<OutcomingEntrySupplier>(Input));

            return Input;
        }

        [HttpPut]
        [AbpAuthorize(PermissionNames.Finance_OutcomingEntry_OutcomingEntryDetail_TabSupplier)]
        public async Task<OutcomingEntrySupplierDto> Update(OutcomingEntrySupplierDto Input)
        {
            var isExist = await WorkScope.GetAll<OutcomingEntrySupplier>().AnyAsync(s => s.Id != Input.Id && s.OutcomingEntryId == Input.OutcomingEntryId && s.SupplierId == Input.SupplierId);

            if (isExist)
                throw new UserFriendlyException(string.Format("Link to Supplier already existed"));

            var oes = await WorkScope.GetAsync<OutcomingEntrySupplier>(Input.Id);
            await WorkScope.UpdateAsync(ObjectMapper.Map<OutcomingEntrySupplierDto, OutcomingEntrySupplier>(Input, oes));

            return Input;
        }

        [HttpDelete]
        [AbpAuthorize(PermissionNames.Finance_OutcomingEntry_OutcomingEntryDetail_TabSupplier_DeleteLinkToSupplier)]
        public async Task RemoveSupplierFromOutcomingEntry(long Id)
        {
            var isSupplier = await WorkScope.GetAll<OutcomingEntrySupplier>().FirstOrDefaultAsync(s => s.Id == Id);

            if (isSupplier == null)
            {
                throw new UserFriendlyException("OutcomingEntrySupplier Id doesn't exist");
            }
            await WorkScope.DeleteAsync<OutcomingEntrySupplier>(Id);
        }
    }
}
