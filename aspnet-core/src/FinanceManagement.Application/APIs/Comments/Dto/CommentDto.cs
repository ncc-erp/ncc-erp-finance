using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using FinanceManagement.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace FinanceManagement.APIs.Comments.Dto
{
    [AutoMapTo(typeof(Comment))]
    public class CommentDto : EntityDto<long>
    {
        public long CommentTypeId { get; set; }
        public long PostId { get; set; }
        public long? UserId { get; set; }
        public string UserName { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public DateTime CreateTime { get; set; }
    }
}
