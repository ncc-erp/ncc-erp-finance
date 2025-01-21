using Abp.Domain.Repositories;
using Abp.Domain.Services;
using Abp.Runtime.Session;
using FinanceManagement.Authorization.Users;
using FinanceManagement.Entities.NewEntities;
using FinanceManagement.Entities;
using FinanceManagement.GeneralModels;
using FinanceManagement.Managers.Settings;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;
using FinanceManagement.Services.Mezon;
using FinanceManagement.Notifications.Komu.Dtos;
using System.Linq;
using System.Text;
using System.Collections.Generic;

namespace FinanceManagement.Notifications.Mezon
{
    public class MezonNotification : DomainService, IMezonNotification
    {
        private readonly IMezonWebService _mezonWebService;
        private readonly IRepository<OutcomingEntry, long> _outcomingEntryRepo;
        private readonly IRepository<TempOutcomingEntry, long> _tempOutcomingEntryRepo;
        private readonly IRepository<User, long> _userRepo;
        private readonly IAbpSession _session;
        private readonly IOptions<ApplicationConfig> _options;
        private readonly IMySettingManager _mySettingManager;

        public MezonNotification(IMezonWebService mezonWebService,
                                 IRepository<OutcomingEntry, long> outcomingEntryRepo,
                                 IRepository<TempOutcomingEntry, long> tempOutcomingEntryRepo,
                                 IRepository<User, long> userRepo,
                                 IAbpSession session,
                                 IOptions<ApplicationConfig> options,
                                 IMySettingManager mySettingManager)
        {
            _mezonWebService = mezonWebService;
            _outcomingEntryRepo = outcomingEntryRepo;
            _tempOutcomingEntryRepo = tempOutcomingEntryRepo;
            _userRepo = userRepo;
            _session = session;
            _options = options;
            _mySettingManager = mySettingManager;
        }
        #region Core Notification
        public void NotifyWithMessage(string message, int? tenantId = int.MinValue)
        {
            string channelUrl = GetNotifyToChannelUrl();
            _mezonWebService.NotifyToChannel(channelUrl, message);
        }

        public async Task NotifyByMessageAsync(string message, int? tenantId = int.MinValue)
        {
            string channelUrl = GetNotifyToChannelUrl();
            await _mezonWebService.NotifyToChannelAsync(channelUrl, message);
        }
        #endregion

        #region Notify Salary from HRM Tool
        public void NotifySalary(long outcomingEntryId)
        {
            var outcomingEntryInfo = IQGetOutcomingEntryNotificationInfo(outcomingEntryId).FirstOrDefault();
            string channelUrl = GetNotifyToChannelUrl();
            _mezonWebService.NotifyToChannel(channelUrl, outcomingEntryInfo.MessageSalaryFromHRM);
        }

        public async Task NotifySalaryAsync(long outcomingEntryId)
        {
            var outcomingEntryInfo = IQGetOutcomingEntryNotificationInfo(outcomingEntryId).FirstOrDefault();
            string channelUrl = GetNotifyToChannelUrl();
            await _mezonWebService.NotifyToChannelAsync(channelUrl, outcomingEntryInfo.MessageSalaryFromHRM);
        }
        #endregion

        #region Notify Team Building From Timesheet Tool
        public void NotifyTeamBuilding(long outcomingEntryId)
        {
            var outcomingEntryInfo = IQGetOutcomingEntryNotificationInfo(outcomingEntryId).FirstOrDefault();
            string channelUrl = GetNotifyToChannelUrl();
            _mezonWebService.NotifyToChannel(channelUrl, outcomingEntryInfo.MessageTeamBuildingFromTimesheet);
        }

        public async Task NotifyTeamBuildingAsync(long outcomingEntryId)
        {
            var outcomingEntryInfo = IQGetOutcomingEntryNotificationInfo(outcomingEntryId).FirstOrDefault();
            string channelUrl = GetNotifyToChannelUrl();
            await _mezonWebService.NotifyToChannelAsync(channelUrl, outcomingEntryInfo.MessageTeamBuildingFromTimesheet);
        }
        #endregion

        #region Notify Change Status
        public void NotifyChangeStatus(long outcomingEntryId, string statusCode)
        {
            var outcomingEntryInfo = IQGetOutcomingEntryNotificationInfo(outcomingEntryId)
                .FirstOrDefault();
            outcomingEntryInfo.Verifier = GetUsernameLoginBySessionUserId();
            outcomingEntryInfo.StatusCode = statusCode;

            var message = GetContentNotifyChangeStatusObject(outcomingEntryInfo);
            string channelUrl = GetNotifyToChannelUrl();
            _mezonWebService.NotifyToChannelMezon(channelUrl, message);
        }

        public async Task NotifyChangeStatusAsync(long outcomingEntryId, string statusCode)
        {
            var outcomingEntryInfo = IQGetOutcomingEntryNotificationInfo(outcomingEntryId)
                .FirstOrDefault();
            outcomingEntryInfo.Verifier = GetUsernameLoginBySessionUserId();
            outcomingEntryInfo.StatusCode = statusCode;

            var message = GetContentNotifyChangeStatus(outcomingEntryInfo);
            string channelUrl = GetNotifyToChannelUrl();
            await _mezonWebService.NotifyToChannelAsync(channelUrl, message);
        }
        private string GetContentNotifyChangeStatus(OutcomingEntryNotificationInfo outcomingEntryInfo)
        {
            var url = $"{_options.Value.ClientRootAddress}app/requestDetail/main?id={outcomingEntryInfo.Id}";

            var cleanSubContent = outcomingEntryInfo.MessageSubContentChangeStatus.Replace("*", "");
            if (!cleanSubContent.StartsWith("@"))
            {
                cleanSubContent = "@" + cleanSubContent;
            }
            var cleanMainContent = outcomingEntryInfo.MessageMainContentChangeStatus
                .Replace("*", "")
                .Replace("`", "");

            var username = cleanSubContent.Split(' ')[0].TrimStart('@');
            var usernameLength = username.Length + 2;
            var mentionEndIndex = cleanSubContent.Length;
            var urlStartIndex = mentionEndIndex + 1;
            var urlEndIndex = urlStartIndex + url.Length + 1;
            var textStartIndex = urlEndIndex;
            var textEndIndex = textStartIndex + cleanMainContent.Length + 1;

            var webhookMessage = new
            {
                t = $" {cleanSubContent}\n{url}\n{cleanMainContent}",
                mk = new[]
                    {
        new { type = "lk", s = urlStartIndex, e = urlEndIndex },
        new { type = "t", s = textStartIndex, e = textEndIndex }
    },
                mentions = new[]
                    {
        new
        {
            username = username, 
            s = 0,
            e = usernameLength
        }
    }
            };

            return System.Text.Json.JsonSerializer.Serialize(webhookMessage);
        }


        private OutcomingEntryMessageDto GetContentNotifyChangeStatusObject(OutcomingEntryNotificationInfo outcomingEntryInfo)
        {
            var url = $"{_options.Value.ClientRootAddress}app/requestDetail/main?id={outcomingEntryInfo.Id}";

            var cleanSubContent = outcomingEntryInfo.MessageSubContentChangeStatus.Replace("*", "");
            if (!cleanSubContent.StartsWith("@"))
            {
                cleanSubContent = "@" + cleanSubContent;
            }
            var cleanMainContent = outcomingEntryInfo.MessageMainContentChangeStatus
                .Replace("*", "")
                .Replace("`", "");
            var username = cleanSubContent.Split(' ')[0].TrimStart('@');
            var usernameLength = username.Length + 2;
            var mentionEndIndex = cleanSubContent.Length;
            var urlStartIndex = mentionEndIndex + 1;
            var urlEndIndex = urlStartIndex + url.Length + 1;
            var textStartIndex = urlEndIndex;
            var textEndIndex = textStartIndex + cleanMainContent.Length + 1;

            return new OutcomingEntryMessageDto
            {

                t = $" {cleanSubContent}\n{url}\n{cleanMainContent}",
                mk = new List<MkDto>
                {
                    new MkDto { type = "lk", s = urlStartIndex, e = urlEndIndex },
                    new MkDto { type = "t", s = textStartIndex, e = textEndIndex }
                },
                mentions = new List<MentionDto>
                {
                    new MentionDto
                    {
                        username = username,
                        s = 0,
                        e = usernameLength
                    }
                }
            };
}

        #endregion

        #region Notify Request Change
        public void NotifyRequestChangePending(long tempOutcomingEntryId, string transitionName)
        {
            var requestChange = IQGetRequestChange(tempOutcomingEntryId)
                .FirstOrDefault();

            requestChange.Verifier = GetUsernameLoginBySessionUserId();
            requestChange.TransitionName = transitionName;
            string channelUrl = GetNotifyToChannelUrl();
            _mezonWebService.NotifyToChannel(channelUrl, requestChange.MessagePending);
        }

        public async Task NotifyRequestChangePendingAsync(long tempOutcomingEntryId, string transitionName)
        {
            var requestChange = IQGetRequestChange(tempOutcomingEntryId)
                .FirstOrDefault();

            requestChange.Verifier = GetUsernameLoginBySessionUserId();
            requestChange.TransitionName = transitionName;
            string channelUrl = GetNotifyToChannelUrl();
            await _mezonWebService.NotifyToChannelAsync(channelUrl, requestChange.MessagePending);
        }

        public void NotifyRequestChangeReject(long tempOutcomingEntryId, string transitionName)
        {
            var requestChange = IQGetRequestChange(tempOutcomingEntryId)
                .FirstOrDefault();

            requestChange.Verifier = GetUsernameLoginBySessionUserId();
            requestChange.TransitionName = transitionName;
            string channelUrl = GetNotifyToChannelUrl();
            _mezonWebService.NotifyToChannel(channelUrl, requestChange.MessageReject);
        }

        public async Task NotifyRequestChangeRejectAsync(long tempOutcomingEntryId, string transitionName)
        {
            var requestChange = IQGetRequestChange(tempOutcomingEntryId)
                .FirstOrDefault();

            requestChange.Verifier = GetUsernameLoginBySessionUserId();
            requestChange.TransitionName = transitionName;
            string channelUrl = GetNotifyToChannelUrl();
            await _mezonWebService.NotifyToChannelAsync(channelUrl, requestChange.MessageReject);
        }

        public void NotifyRequestChangeApprove(long tempOutcomingEntryId, string transitionName)
        {
            var requestChange = IQGetRequestChange(tempOutcomingEntryId)
                .FirstOrDefault();

            requestChange.Verifier = GetUsernameLoginBySessionUserId();
            requestChange.TransitionName = transitionName;
            string channelUrl = GetNotifyToChannelUrl();
            _mezonWebService.NotifyToChannel(channelUrl, requestChange.MessageApprove);
        }

        public async Task NotifyRequestChangeApproveAsync(long tempOutcomingEntryId, string transitionName)
        {
            var requestChange = IQGetRequestChange(tempOutcomingEntryId)
                .FirstOrDefault();

            requestChange.Verifier = GetUsernameLoginBySessionUserId();
            requestChange.TransitionName = transitionName;
            string channelUrl = GetNotifyToChannelUrl();
            await _mezonWebService.NotifyToChannelAsync(channelUrl, requestChange.MessageApprove);
        }

        private IQueryable<ContentNotificationRequestChange> IQGetRequestChange(long tempOutcomingEntryId)
        {
            return from temp in _tempOutcomingEntryRepo.GetAll().Where(x => x.Id == tempOutcomingEntryId)
                   join rootOut in _outcomingEntryRepo.GetAll() on temp.RootOutcomingEntryId equals rootOut.Id
                   select new ContentNotificationRequestChange
                   {
                       OutcomingEntryId = temp.RootOutcomingEntryId,
                       CurrencyName = temp.Currency.Code,
                       OldCurrencyName = rootOut.Currency.Name,
                       OutcomingEntryName = temp.Name,
                       TempOutcomingEntryId = tempOutcomingEntryId,
                       Reason = temp.Reason,
                       Value = temp.Value,
                       OldValue = rootOut.Value,
                       ClientRootAddress = _options.Value.ClientRootAddress,
                   };
        }
        #endregion

        #region Get Info General
        private IQueryable<OutcomingEntryNotificationInfo> IQGetOutcomingEntryNotificationInfo(long outcomingEntryId)
        {
            return from outcom in _outcomingEntryRepo.GetAll().Where(x => x.Id == outcomingEntryId)
                   select new OutcomingEntryNotificationInfo
                   {
                       Id = outcom.Id,
                       OutcomingEntryName = outcom.Name,
                       OutcomingEntryTypeCode = outcom.OutcomingEntryType.Code,
                       OutcomingEntryValue = outcom.Value,
                       CurrencyCode = outcom.Currency.Code,
                       BranchName = outcom.Branch.Name,
                       CreationTime = outcom.CreationTime,
                       CreatedBy = _userRepo.GetAll().Where(x => x.Id == outcom.CreatorUserId).Select(x => x.UserName).FirstOrDefault()
                   };
        }
        private async Task<string> GetNotifyToChannelUrlAsync(int? tenantId = int.MinValue)
        {
            return await _mySettingManager.GetNotifyMezonChannelUrlAsync(tenantId);
        }
        private string GetNotifyToChannelUrl(int? tenantId = int.MinValue)
        {
            return _mySettingManager.GetNotifyMezonChannelUrl(tenantId);
        }
        private string GetUsernameLoginBySessionUserId()
        {
            var username = _userRepo.GetAll()
                .Where(x => x.Id == _session.UserId)
                .Select(x => x.UserName)
                .FirstOrDefault();
            if (string.IsNullOrEmpty(username))
                return string.Empty;
            return username;
        }
        #endregion
    }
}
