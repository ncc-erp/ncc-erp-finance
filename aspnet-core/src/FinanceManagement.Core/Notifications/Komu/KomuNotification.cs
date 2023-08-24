using Abp.Domain.Repositories;
using Abp.Domain.Services;
using Abp.Runtime.Session;
using FinanceManagement.Authorization.Users;
using FinanceManagement.Configuration;
using FinanceManagement.Entities;
using FinanceManagement.Entities.NewEntities;
using FinanceManagement.GeneralModels;
using FinanceManagement.Helper;
using FinanceManagement.Managers.Settings;
using FinanceManagement.Notifications.Komu.Dtos;
using FinanceManagement.Services.Komu;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceManagement.Notifications.Komu
{
    public class KomuNotification : DomainService, IKomuNotification
    {
        private readonly IKomuService _komuService;
        private readonly IRepository<OutcomingEntry, long> _outcomingEntryRepo;
        private readonly IRepository<TempOutcomingEntry, long> _tempOutcomingEntryRepo;
        private readonly IRepository<User, long> _userRepo;
        private readonly IAbpSession _session;
        private readonly IOptions<ApplicationConfig> _options;
        private readonly IMySettingManager _mySettingManager;
        public KomuNotification(
            IKomuService komuService,
            IRepository<OutcomingEntry, long> outcomingEntryRepo,
            IRepository<TempOutcomingEntry, long> tempOutcomingEntryRepo,
            IRepository<User, long> userRepo,
            IAbpSession session,
            IOptions<ApplicationConfig> options,
            IMySettingManager mySettingManager
        )
        {
            _komuService = komuService;
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
            string channelId = GetNotifyToChannelId(tenantId);
            _komuService.NotifyToChannel(message, channelId);
        }
        public async Task NotifyByMessageAsync(string message, int? tenantId = int.MinValue)
        {
            string channelId = GetNotifyToChannelId(tenantId);
            await _komuService.NotifyToChannelAsync(message, channelId);
        }
        #endregion

        #region Notify Salary from HRM Tool
        public void NotifySalary(long outcomingEntryId)
        {
            var outcomingEntryInfo = IQGetOutcomingEntryNotificationInfo(outcomingEntryId)
            .FirstOrDefault();
            var channelId = GetNotifyToChannelId();
            _komuService.NotifyToChannel(outcomingEntryInfo.MessageSalaryFromHRM, channelId);
        }
        public async Task NotifySalaryAsync(long outcomingEntryId)
        {
            var outcomingEntryInfo = await IQGetOutcomingEntryNotificationInfo(outcomingEntryId)
                .FirstOrDefaultAsync();
            var channelId = await GetNotifyToChannelIdAsync();
            await _komuService.NotifyToChannelAsync(outcomingEntryInfo.MessageSalaryFromHRM, channelId);
        }
        #endregion

        #region Notify Team Building From Timesheet Tool
        public void NotifyTeamBuilding(long outcomingEntryId)
        {
            var outcomingEntryInfo = IQGetOutcomingEntryNotificationInfo(outcomingEntryId)
            .FirstOrDefault();
            var channelId = GetNotifyToChannelId();
            _komuService.NotifyToChannel(outcomingEntryInfo.MessageTeamBuildingFromTimesheet, channelId);
        }
        public async Task NotifyTeamBuildingAsync(long outcomingEntryId)
        {
            var outcomingEntryInfo = await IQGetOutcomingEntryNotificationInfo(outcomingEntryId)
                .FirstOrDefaultAsync();
            var channelId = await GetNotifyToChannelIdAsync();
            await _komuService.NotifyToChannelAsync(outcomingEntryInfo.MessageTeamBuildingFromTimesheet, channelId);
        }
        #endregion

        #region Notify Change Status
        public void NotifyChangeStatus(long outcomingEntryId, string statusCode)
        {
            var outcomingEntryInfo = IQGetOutcomingEntryNotificationInfo(outcomingEntryId)
                .FirstOrDefault();
            outcomingEntryInfo.Verifier = GetUsernameLoginBySessionUserId();
            outcomingEntryInfo.StatusCode = statusCode;

            var message = GetContentNotifyChangeStatus(outcomingEntryInfo);
            var channelId = GetNotifyToChannelId();
            _komuService.NotifyToChannel(message, channelId);
        }
        public async Task NotifyChangeStatusAsync(long outcomingEntryId, string statusCode)
        {
            var outcomingEntryInfo = await IQGetOutcomingEntryNotificationInfo(outcomingEntryId)
                .FirstOrDefaultAsync();
            outcomingEntryInfo.Verifier = GetUsernameLoginBySessionUserId();
            outcomingEntryInfo.StatusCode = statusCode;

            var message = GetContentNotifyChangeStatus(outcomingEntryInfo);
            var channelId = await GetNotifyToChannelIdAsync();
            await _komuService.NotifyToChannelAsync(message, channelId);
        }
        private string GetContentNotifyChangeStatus(OutcomingEntryNotificationInfo outcomingEntryInfo)
        {
            var message = new StringBuilder();
            message.AppendLine(outcomingEntryInfo.MessageSubContentChangeStatus);
            message.AppendLine($"{_options.Value.ClientRootAddress}app/requestDetail/main?id={outcomingEntryInfo.Id}");
            message.AppendLine(outcomingEntryInfo.MessageMainContentChangeStatus);
            return message.ToString();
        }
        #endregion

        #region Notify Request Change
        public void NotifyRequestChangePending(long tempOutcomingEntryId, string transitionName)
        {
            var channelId = GetNotifyToChannelId();
            var requestChange = IQGetRequestChange(tempOutcomingEntryId)
                .FirstOrDefault();

            requestChange.Verifier = GetUsernameLoginBySessionUserId();
            requestChange.TransitionName = transitionName;
            _komuService.NotifyToChannel(requestChange.MessagePending, channelId);
        }
        public async Task NotifyRequestChangePendingAsync(long tempOutcomingEntryId, string transitionName)
        {
            var channelId = GetNotifyToChannelId();
            var requestChange = await IQGetRequestChange(tempOutcomingEntryId)
                .FirstOrDefaultAsync();

            requestChange.Verifier = GetUsernameLoginBySessionUserId();
            requestChange.TransitionName = transitionName;
            await _komuService.NotifyToChannelAsync(requestChange.MessagePending, channelId);
        }
        public void NotifyRequestChangeReject(long tempOutcomingEntryId, string transitionName)
        {
            var channelId = GetNotifyToChannelId();
            var requestChange = IQGetRequestChange(tempOutcomingEntryId)
                .FirstOrDefault();

            requestChange.Verifier = GetUsernameLoginBySessionUserId();
            requestChange.TransitionName = transitionName;
            _komuService.NotifyToChannel(requestChange.MessageReject, channelId);
        }
        public async Task NotifyRequestChangeRejectAsync(long tempOutcomingEntryId, string transitionName)
        {
            var channelId = GetNotifyToChannelId();
            var requestChange = await IQGetRequestChange(tempOutcomingEntryId)
                .FirstOrDefaultAsync();

            requestChange.Verifier = GetUsernameLoginBySessionUserId();
            requestChange.TransitionName = transitionName;
            await _komuService.NotifyToChannelAsync(requestChange.MessageReject, channelId);
        }
        public void NotifyRequestChangeApprove(long tempOutcomingEntryId, string transitionName)
        {
            var channelId = GetNotifyToChannelId();
            var requestChange = IQGetRequestChange(tempOutcomingEntryId)
                .FirstOrDefault();

            requestChange.Verifier = GetUsernameLoginBySessionUserId();
            requestChange.TransitionName = transitionName;
            _komuService.NotifyToChannel(requestChange.MessageApprove, channelId);
        }
        public async Task NotifyRequestChangeApproveAsync(long tempOutcomingEntryId, string transitionName)
        {
            var channelId = GetNotifyToChannelId();
            var requestChange = await IQGetRequestChange(tempOutcomingEntryId)
                .FirstOrDefaultAsync();

            requestChange.Verifier = GetUsernameLoginBySessionUserId();
            requestChange.TransitionName = transitionName;
            await _komuService.NotifyToChannelAsync(requestChange.MessageApprove, channelId);
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
                       Reason= temp.Reason,
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
                       CreatedBy = _userRepo.GetAll().Where(x => x.Id == outcom.CreatorUserId).Select(x => x.FullName).FirstOrDefault()
                   };
        }
        private async Task<string> GetNotifyToChannelIdAsync(int? tenantId = int.MinValue)
        {
            return await _mySettingManager.GetNotifyKomuChannelIdAsync(tenantId);
        }
        private string GetNotifyToChannelId(int? tenantId = int.MinValue)
        {
            return _mySettingManager.GetNotifyKomuChannelId(tenantId);
        }
        private string GetUsernameLoginBySessionUserId()
        {
            var fullName = _userRepo.GetAll()
                .Where(x => x.Id == _session.UserId)
                .Select(x => x.FullName)
                .FirstOrDefault();
            if (string.IsNullOrEmpty(fullName))
                return string.Empty;
            return fullName;
        }
        #endregion
    }
}
