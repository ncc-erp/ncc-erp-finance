using Abp.Authorization;
using Abp.UI;
using FinanceManagement.APIs.Suppliers.Dto;
using FinanceManagement.Authorization;
using FinanceManagement.Entities;
using FinanceManagement.Extension;
using FinanceManagement.IoC;
using FinanceManagement.Paging;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceManagement.APIs.Suppliers
{
    [AbpAuthorize]
    public class SupplierAppService : FinanceManagementAppServiceBase
    {
        public SupplierAppService(IWorkScope workScope) : base(workScope)
        {
        }

        [HttpGet]
        public async Task<List<SupplierDto>> GetAll()
        {
            return await WorkScope.GetAll<Supplier>().Select(s => new SupplierDto
                        {
                            Id = s.Id,
                            Name = s.Name,
                            PhoneNumber = s.PhoneNumber,
                            Address = s.Address,
                            ContactPersonName = s.ContactPersonName,
                            ContactPersonPhone = s.ContactPersonPhone,
                            TaxNumber = s.TaxNumber
                        }).ToListAsync();
        }

        [HttpGet]
        public async Task<List<SupplierDto>> GetAllForDropdown()
        {
            return await WorkScope.GetAll<Supplier>().Select(s => new SupplierDto
            {
                Id = s.Id,
                Name = s.Name,
            }).ToListAsync();
        }

        [HttpPost]
        [AbpAuthorize(PermissionNames.Directory_Supplier)]
        public async Task<GridResult<SupplierDto>> GetAllPaging(GridParam Input)
        {
            var query = WorkScope.GetAll<Supplier>().Select(s => new SupplierDto
                                {
                                    Id = s.Id,
                                    Name = s.Name,
                                    PhoneNumber = s.PhoneNumber,
                                    Address = s.Address,
                                    ContactPersonName = s.ContactPersonName,
                                    ContactPersonPhone = s.ContactPersonPhone,
                                    TaxNumber = s.TaxNumber
                                });
            return await query.GetGridResult(query, Input);
        }

        [HttpGet]

        public async Task<SupplierDto> Get(long Id)
        {
            return await WorkScope.GetAll<Supplier>().Where(x => x.Id == Id)
                        .Select(s => new SupplierDto
                        {
                            Id = s.Id,
                            Name = s.Name,
                            PhoneNumber = s.PhoneNumber,
                            Address = s.Address,
                            ContactPersonName = s.ContactPersonName,
                            ContactPersonPhone = s.ContactPersonPhone,
                            TaxNumber = s.TaxNumber
                        }).FirstOrDefaultAsync();
        }

        [HttpPost]
        [AbpAuthorize(PermissionNames.Directory_Supplier_Create, PermissionNames.Finance_OutcomingEntry_OutcomingEntryDetail_TabSupplier_CreateSupplier)]
        public async Task<SupplierDto> Create(SupplierDto Input)
        {
            var nameExist = await WorkScope.GetAll<Supplier>().AnyAsync(x => x.Name == Input.Name);
            if(nameExist)
            {
                throw new UserFriendlyException("Supplier name already exist");
            }

            Input.Id = await WorkScope.InsertAndGetIdAsync(ObjectMapper.Map<Supplier>(Input));

            if (Input.OutcomingEntryId != null)
            {
                var OutcomingEntrySupplier = new OutcomingEntrySupplier
                {
                    OutcomingEntryId = (long)Input.OutcomingEntryId,
                    SupplierId = Input.Id
                };

                await WorkScope.InsertAndGetIdAsync<OutcomingEntrySupplier>(OutcomingEntrySupplier);
            }

            return Input;
        }

        [HttpPut]
        [AbpAuthorize(PermissionNames.Directory_Supplier_Update)]
        public async Task<SupplierDto> Update(SupplierDto Input)
        {
            var isSupplier = await WorkScope.GetAll<Supplier>().FirstOrDefaultAsync(s => s.Id == Input.Id);

            if (isSupplier == null)
            {
                throw new UserFriendlyException("Supplier Id doesn't exist");
            }

            var nameExist = await WorkScope.GetAll<Supplier>().AnyAsync(x => x.Name == Input.Name && x.Id != Input.Id);
            if (nameExist)
            {
                throw new UserFriendlyException("Supplier name already exist");
            }
            var supplier = await WorkScope.GetAsync<Supplier>(Input.Id);
            await WorkScope.UpdateAsync(ObjectMapper.Map<SupplierDto, Supplier>(Input, supplier));

            return Input;
        }

        [HttpDelete]
        [AbpAuthorize(PermissionNames.Directory_Supplier_Delete)]
        public async Task Delete(long id)
        {
            var isSupplier = await WorkScope.GetAll<Supplier>().FirstOrDefaultAsync(s => s.Id == id);

            if (isSupplier == null)
            {
                throw new UserFriendlyException("Supplier Id doesn't exist");
            }

            var isOutcoming = await WorkScope.GetAll<OutcomingEntrySupplier>().AnyAsync(x => x.SupplierId == id);

            if(isOutcoming)
            {
                throw new UserFriendlyException("Supplier Id already exist in the OutcomingEntrySupplier. You can't delete it");
            }

            await WorkScope.DeleteAsync<Supplier>(id);
        }

        [HttpGet]
        [AbpAuthorize(PermissionNames.Finance_OutcomingEntry_OutcomingEntryDetail_TabSupplier)]
        public async Task<List<SupplierDto>> GetAllByOutcomingEntry(long OutcomingEntryId)
        {
            var query = from s in WorkScope.GetAll<Supplier>()
                        join oes in WorkScope.GetAll<OutcomingEntrySupplier>() on s.Id equals oes.SupplierId
                        where oes.OutcomingEntryId == OutcomingEntryId
                        select new SupplierDto
                        {
                            Id = s.Id,
                            Name = s.Name,
                            PhoneNumber = s.PhoneNumber,
                            Address = s.Address,
                            ContactPersonName = s.ContactPersonName,
                            ContactPersonPhone = s.ContactPersonPhone,
                            TaxNumber = s.TaxNumber,
                            OutcomingEntrySupplierId = oes.Id
                        };
            return await query.ToListAsync();
        }
    }
}
