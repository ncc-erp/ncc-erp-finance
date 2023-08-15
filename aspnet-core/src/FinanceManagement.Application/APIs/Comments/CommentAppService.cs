using Abp.Authorization;
using Abp.UI;
using FinanceManagement.APIs.Comments.Dto;
using FinanceManagement.Authorization;
using FinanceManagement.Authorization.Users;
using FinanceManagement.Entities;
using FinanceManagement.Enums;
using FinanceManagement.Extension;
using FinanceManagement.IoC;
using FinanceManagement.Paging;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceManagement.APIs.Comments
{
    [AbpAuthorize]
    public class CommentAppService : FinanceManagementAppServiceBase
    {
        private readonly IPermissionChecker _permissionChecker;
        public CommentAppService(IWorkScope workScope, IPermissionChecker permissionChecker) : base(workScope)
        {
            _permissionChecker = permissionChecker;
        }

        [HttpGet]
        //[AbpAuthorize(PermissionNames.Finance_OutcomingEntry_GetAllCommentByPost)]
        public async Task<GridResult<CommentDto>> GetAllCommentByPost(GridParam input, CommentType type, long requestId)
        {
            var user = WorkScope.GetAll<User>();

            var query = WorkScope.GetAll<Comment>().Where(x => x.CommentType == type)
                        .Where(x => x.PostId == requestId)
                        .Select(x => new CommentDto
                        {
                            Id = x.Id,
                            UserId = x.CreatorUserId,
                            UserName = user.FirstOrDefault(y => y.Id == x.CreatorUserId).Name,
                            Title = x.Title,
                            Content = x.Content,
                            CreateTime = DateTime.Parse(x.CreationTime.ToString("yyyy/MM/dd HH:mm:ss"))
                        });

            return await query.GetGridResult(query, input);
        }

        [HttpPost]
        [AbpAuthorize(PermissionNames.Finance_OutcomingEntry_OutcomingEntryDetail_TabGeneral_EditDisscus, PermissionNames.Finance_OutcomingEntry_OutcomingEntryDetail_TabGeneral_EditOnlyMyDisscus)]
        public async Task<CommentDto> CreateCommentByPost(CommentDto input)
        {
            
            input.Id = await WorkScope.InsertAndGetIdAsync(ObjectMapper.Map<Comment>(input));
            return input;
        }

        [HttpPut]
        [AbpAuthorize(PermissionNames.Finance_OutcomingEntry_OutcomingEntryDetail_TabGeneral_EditDisscus, PermissionNames.Finance_OutcomingEntry_OutcomingEntryDetail_TabGeneral_EditOnlyMyDisscus)]
        public async Task<CommentDto> UpdateCommentByPost(CommentDto input)
        {
            var hasPermissionUpdateAll = await _permissionChecker.IsGrantedAsync(PermissionNames.Finance_OutcomingEntry_OutcomingEntryDetail_TabGeneral_EditDisscus);
            var hasPermissionUpdateMyComment = await _permissionChecker.IsGrantedAsync(PermissionNames.Finance_OutcomingEntry_OutcomingEntryDetail_TabGeneral_EditDisscus);
            if (!hasPermissionUpdateAll &&  hasPermissionUpdateMyComment && input.UserId != AbpSession.UserId.Value)
            {
                throw new UserFriendlyException("You can't edit other people's comments");
            }

            var comment = await WorkScope.GetAsync<Comment>(input.Id);

            if (comment == null)
            {
                throw new UserFriendlyException("Comment not exist !");
            }

            comment.Content = input.Content;
            await WorkScope.UpdateAsync(ObjectMapper.Map<Comment>(comment));
            return input;

        }

        [HttpDelete]
        [AbpAuthorize(PermissionNames.Finance_OutcomingEntry_OutcomingEntryDetail_TabGeneral_DeleteDisscus, PermissionNames.Finance_OutcomingEntry_OutcomingEntryDetail_TabGeneral_DeleteOnlyMyDisscus)]
        public async Task Delete(long id)
        {
            var hasPermissionDeleteAll = await _permissionChecker.IsGrantedAsync(PermissionNames.Finance_OutcomingEntry_OutcomingEntryDetail_TabGeneral_DeleteDisscus);
            var hasPermissionDeleteMyComment = await _permissionChecker.IsGrantedAsync(PermissionNames.Finance_OutcomingEntry_OutcomingEntryDetail_TabGeneral_DeleteOnlyMyDisscus);

            
            var isExist = await WorkScope.GetAll<Comment>().AnyAsync(x => x.Id == id);

            if (!isExist)
            {
                throw new UserFriendlyException("Comment not exist !");
            }

            var isUserCreate = await WorkScope.GetAsync<Comment>(id);

            if(!hasPermissionDeleteAll && hasPermissionDeleteMyComment && isUserCreate.CreatorUserId != AbpSession.UserId.Value)
            {
                throw new UserFriendlyException("You can't delete other people's comments");
            }

            await WorkScope.DeleteAsync<Comment>(id);
        }
    }
}
