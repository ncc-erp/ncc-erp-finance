using Abp.Authorization;
using Abp.Configuration;
using FinanceManagement.APIs.RevenueManageds.Dto;
using FinanceManagement.Authorization;
using FinanceManagement.Authorization.Roles;
using FinanceManagement.Authorization.Users;
using FinanceManagement.Entities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using FinanceManagement.Paging;
using FinanceManagement.Extension;
using FinanceManagement.Enums;
using Microsoft.EntityFrameworkCore;
using Abp.UI;
using System.IO;
using Microsoft.AspNetCore.Http;
using FinanceManagement.IoC;

namespace FinanceManagement.APIs.RevenueManageds
{
    [AbpAuthorize]
    public class RevenueManagedAppService : FinanceManagementAppServiceBase
    {
        private readonly IHostingEnvironment _hostingEnvironment;
        public RevenueManagedAppService(IHostingEnvironment hostingEnvironment, IWorkScope workScope) : base(workScope)
        {
            this._hostingEnvironment = hostingEnvironment;
        }

        [HttpPost]
        public async Task<GetRevenueManagedDto> GetAllPaging(GridParam param, int? status)
        {
            //var listFiles = WorkScope.GetAll<RevenueManagedFile>();
            var query1 = WorkScope.GetAll<RevenueManaged>()
                                .Where(x => !status.HasValue || x.Status == (RevenueManagedStatus)status)
                                .Select(x => new RevenueManagedDto
                                {
                                    Id = x.Id,
                                    NameInvoice = x.NameInvoice,
                                    AccountId = x.AccountId,
                                    AccountTypeCode = x.Account.AccountType.Code,
                                    AccountName = x.Account.Name,
                                    Month = x.Month,
                                    CollectionDebt = x.CollectionDebt,
                                    DebtReceived = x.DebtReceived,
                                    UnitId = x.UnitId,
                                    CurrencyCode = x.Currency.Code,
                                    SendInvoiceDate = x.SendInvoiceDate,
                                    Deadline = x.Deadline,
                                    Status = x.Status,
                                    Note = x.Note,
                                    RemindStatus = x.RemindStatus,
                                    //PathFiles = listFiles.Where(s => s.RevenueManagedId == x.Id).Select(s => s.FilePath).ToList()
                                });


            IQueryable<RevenueManagedDto> query = null;
            if (param.Sort.EmptyIfNull().ToLower() != "remaindebt")
            {
                query = query1.ApplySearchAndFilter(param).OrderBy(x => x.Deadline);
            }
            else
            {
                param.Sort = "";
                if (param.SortDirection == SortDirection.ASC)
                {
                    query = query1.ApplySearchAndFilter(param).OrderBy(s => s.CollectionDebt - (s.DebtReceived.HasValue ? s.DebtReceived : 0));
                }
                else
                {
                    query = query1.ApplySearchAndFilter(param).OrderByDescending(s => s.CollectionDebt - (s.DebtReceived.HasValue ? s.DebtReceived : 0));
                }
            }
          

            var list = await query.TakePage(param).ToListAsync();
            var total = await query.CountAsync();
            var revenueManageds = new GridResult<RevenueManagedDto>(list, total);

            var remainingDebt = query.GroupBy(x => x.CurrencyCode)
                                    .Select(x => new RemainingDebt
                                    {
                                        CurrencyCode = x.Key,
                                        CollectionDebt = x.Sum(s => s.CollectionDebt),
                                        DebtReceived = x.Sum(s => s.DebtReceived),
                                    }).ToList();
                 

            var result = new GetRevenueManagedDto
            {
                RevenueManagedDtos = revenueManageds,
                RemainingDebts = remainingDebt,
            };
            return result;
        }

        [HttpPost]
        public async Task<IActionResult> Create(RevenueManagedDto revenueManagedDto)
        {
            try
            {
                var Id = await WorkScope.InsertAndGetIdAsync(ObjectMapper.Map<RevenueManaged>(revenueManagedDto));
                return new OkObjectResult(Id);
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult(ex.Message);
            }
        }
        [HttpPost]
        public async Task<IActionResult> UploadFiles([FromForm]RevenueManagedFiles revenueManagedFiles)
        {
            try
            {
                var models = await WorkScope.GetAll<RevenueManagedFile>().Where(x => x.RevenueManagedId == revenueManagedFiles.Id).Select(x => x.FilePath).ToListAsync();

                if (revenueManagedFiles.Files == null)
                {
                    if(models.Count() > 0)
                    {
                        foreach(string fileName in models)
                        {
                            await DeleteRevenueFile(fileName, revenueManagedFiles.Id);
                        }
                    }
                    return new OkObjectResult("Success");
                }

                List<string> listFile = models.Except(revenueManagedFiles.FileNames).ToList();
                List<string> listFile1 = revenueManagedFiles.FileNames.Except(models).ToList();
                listFile.AddRange(listFile1);
               
                String pathFolder = Path.Combine(_hostingEnvironment.ContentRootPath, "Uploads", "revenuemanageds");
                if (!Directory.Exists(pathFolder))
                {
                    Directory.CreateDirectory(pathFolder);
                }
                foreach(string fileName in listFile)
                {
                    String pathFile = Path.Combine(_hostingEnvironment.ContentRootPath, "Uploads", "revenuemanageds", fileName);
                    if (File.Exists(pathFile))
                    {
                        await DeleteRevenueFile(fileName, revenueManagedFiles.Id);
                    }
                    else
                    {
                        await AddRevenueFile(fileName, revenueManagedFiles.Id, revenueManagedFiles.Files, pathFolder);
                    }
                }
                
                return new OkObjectResult("Success");
            }
            catch(Exception ex)
            {
                return new BadRequestObjectResult(ex);
            }
        }
        public async Task DeleteRevenueFile(string fileName, long Id)
        {
            String pathFile = Path.Combine(_hostingEnvironment.ContentRootPath, "Uploads", "revenuemanageds", fileName);
            if (File.Exists(pathFile))
            {
                File.Delete(pathFile);
                var item = WorkScope.GetAll<RevenueManagedFile>().Where(x => x.RevenueManagedId == Id && x.FilePath == fileName).FirstOrDefault();
                await WorkScope.DeleteAsync(item);
            }
        }
        public async Task AddRevenueFile(string fileName, long Id, List<IFormFile> files, string pathFolder)
        {
            String pathFile = Path.Combine(_hostingEnvironment.ContentRootPath, "Uploads", "revenuemanageds", fileName);
            var file = files.Where(x => x.FileName.Equals(fileName)).FirstOrDefault();
            string filePath = DateTimeOffset.Now.ToUnixTimeMilliseconds() + "_" + file.FileName;
            var fileCreate = Path.Combine(pathFolder, filePath);
            using (var stream = System.IO.File.Create(fileCreate))
            {
                await file.CopyToAsync(stream);
            }

            var fileId = await WorkScope.InsertAndGetIdAsync(new RevenueManagedFile
            {
                RevenueManagedId = Id,
                FilePath = filePath
            });
        }
        [HttpGet]
        public async Task<IActionResult> DownloadFile(string Name)
        {
            try
            {
                String path = Path.Combine(_hostingEnvironment.ContentRootPath, "Uploads", "revenuemanageds",Name);
                if (File.Exists(path))
                {
                    byte[] data = File.ReadAllBytes(path);
                    return new OkObjectResult(data);
                }
                throw new UserFriendlyException("File not found");
            }
            catch(Exception ex)
            {
                throw (ex);
            }
        }
        [HttpGet]
        public async Task<IActionResult> GetFiles(long Id)
        {
            try
            {
                var models = await WorkScope.GetAll<RevenueManagedFile>().Where(x => x.RevenueManagedId == Id).ToListAsync();
                List<RevenueManagedReadFile> files = new List<RevenueManagedReadFile>();
                foreach(var item in models)
                {
                    String path = Path.Combine(_hostingEnvironment.ContentRootPath, "Uploads", "revenuemanageds", item.FilePath);
                    if (File.Exists(path))
                    {
                        RevenueManagedReadFile data = new RevenueManagedReadFile();
                        data.Bytes = File.ReadAllBytes(path);
                        data.FileName = Path.GetFileName(path);
                        files.Add(data);
                    }
                }
                return new OkObjectResult(files);
            }
            catch(Exception ex)
            {
                return new BadRequestObjectResult(ex);
            }
        }
        [HttpDelete]

        public async Task<IActionResult> Delete(long Id)
        {
            try
            {
                await WorkScope.DeleteAsync<RevenueManaged>(Id);
                return new OkObjectResult("Xóa thành công!");
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult(ex);
            }
        }
        [HttpPut]

        public async Task<IActionResult> Update(RevenueManagedDto revenueManagedDto)
        {
            try
            {
                await WorkScope.UpdateAsync<RevenueManaged>(ObjectMapper.Map<RevenueManaged>(revenueManagedDto));
                return new OkObjectResult("Cập nhật thành công!");
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult(ex);
            }

        }
    }
}
