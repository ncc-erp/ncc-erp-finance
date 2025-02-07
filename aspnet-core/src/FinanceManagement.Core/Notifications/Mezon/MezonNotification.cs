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
using System.Linq;
using System.Text;
using FinanceManagement.Notifications.Mezon.Dto;
using System.Collections.Generic;

namespace FinanceManagement.Notifications.Mezon
{
    public class MezonNotification : DomainService, INotification
    {
        private readonly MezonWebService _mezonWebService;
        private readonly IRepository<OutcomingEntry, long> _outcomingEntryRepo;
        private readonly IRepository<TempOutcomingEntry, long> _tempOutcomingEntryRepo;
        private readonly IRepository<User, long> _userRepo;
        private readonly IAbpSession _session;
        private readonly IOptions<ApplicationConfig> _options;
        private readonly IMySettingManager _mySettingManager;

        public MezonNotification(MezonWebService mezonWebService,
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
            string channelUrl = GetNotifyToChannelUrl(tenantId);
            var messageMezon = ConvertStringToMezonMessage(message);
            _mezonWebService.NotifyToChannelMezon(messageMezon, channelUrl);
        }

        public async Task NotifyByMessageAsync(string message, int? tenantId = int.MinValue)
        {
            string channelUrl = GetNotifyToChannelUrl(tenantId);
            var messageMezon = ConvertStringToMezonMessage(message);
            await _mezonWebService.NotifyToChannelMezonAsync(messageMezon, channelUrl);
        }
        #endregion

        #region Notify Salary from HRM Tool
        public void NotifySalary(long outcomingEntryId)
        {
            var outcomingEntryInfo = IQGetOutcomingEntryNotificationInfo(outcomingEntryId).FirstOrDefault();
            string channelUrl = GetNotifyToChannelUrl();
            var mezonMessage = ConvertStringToMezonMessage(outcomingEntryInfo.MessageSalaryFromHRM);
            _mezonWebService.NotifyToChannelMezon( mezonMessage,channelUrl);
        }

        public async Task NotifySalaryAsync(long outcomingEntryId)
        {
            var outcomingEntryInfo = IQGetOutcomingEntryNotificationInfo(outcomingEntryId).FirstOrDefault();
            string channelUrl = GetNotifyToChannelUrl();
            var mezonMessage = ConvertStringToMezonMessage(outcomingEntryInfo.MessageSalaryFromHRM);
            await _mezonWebService.NotifyToChannelMezonAsync(mezonMessage, channelUrl);
        }
        #endregion

        #region Notify Team Building From Timesheet Tool
        public void NotifyTeamBuilding(long outcomingEntryId)
        {
            var outcomingEntryInfo = IQGetOutcomingEntryNotificationInfo(outcomingEntryId).FirstOrDefault();
            string channelUrl = GetNotifyToChannelUrl();
            var mezonMessage = ConvertStringToMezonMessage(outcomingEntryInfo.MessageTeamBuildingFromTimesheet);
            _mezonWebService.NotifyToChannelMezon(mezonMessage, channelUrl);
        }

        public async Task NotifyTeamBuildingAsync(long outcomingEntryId)
        {
            var outcomingEntryInfo = IQGetOutcomingEntryNotificationInfo(outcomingEntryId).FirstOrDefault();
            string channelUrl = GetNotifyToChannelUrl();
            var mezonMessage = ConvertStringToMezonMessage(outcomingEntryInfo.MessageTeamBuildingFromTimesheet);
            await _mezonWebService.NotifyToChannelMezonAsync(mezonMessage,channelUrl);
        }     
        #endregion
         private MezonMessage ConvertStringToMezonMessage(string message)
        {
            return new MezonMessage()
            {
                t = message,
                mentions = new List<Mentions> { }
            };
           
        }
        #region Notify Change Status
        public void NotifyChangeStatus(long outcomingEntryId, string statusCode)
        {
            var outcomingEntryInfo = IQGetOutcomingEntryNotificationInfo(outcomingEntryId)
                .FirstOrDefault();
            outcomingEntryInfo.Verifier = GetUsernameLoginBySessionUserId();
            outcomingEntryInfo.StatusCode = statusCode;

            var message = outcomingEntryInfo.GenerateMezonMessage(_options.Value.ClientRootAddress);
            string channelUrl = GetNotifyToChannelUrl();
            _mezonWebService.NotifyToChannelMezon(message ,channelUrl);
        }

        public async Task NotifyChangeStatusAsync(long outcomingEntryId, string statusCode)
        {
            var outcomingEntryInfo = IQGetOutcomingEntryNotificationInfo(outcomingEntryId)
                .FirstOrDefault();
            outcomingEntryInfo.Verifier = GetUsernameLoginBySessionUserId();
            outcomingEntryInfo.StatusCode = statusCode;

            var message = outcomingEntryInfo.GenerateMezonMessage(_options.Value.ClientRootAddress);
            string channelUrl = GetNotifyToChannelUrl();
            await _mezonWebService.NotifyToChannelMezonAsync(message , channelUrl );
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
            var mezonMessage = ConvertStringRequestChangeToMezonMessage(requestChange.MessagePending, requestChange.Verifier);
            _mezonWebService.NotifyToChannelMezon(mezonMessage, channelUrl);
        }

        public async Task NotifyRequestChangePendingAsync(long tempOutcomingEntryId, string transitionName)
        {
            var requestChange = IQGetRequestChange(tempOutcomingEntryId)
                .FirstOrDefault();

            requestChange.Verifier = GetUsernameLoginBySessionUserId();
            requestChange.TransitionName = transitionName;
            string channelUrl = GetNotifyToChannelUrl();
            var mezonMessage = ConvertStringRequestChangeToMezonMessage(requestChange.MessagePending, requestChange.Verifier);
            await _mezonWebService.NotifyToChannelMezonAsync(mezonMessage, channelUrl);
        }

        public void NotifyRequestChangeReject(long tempOutcomingEntryId, string transitionName)
        {
            var requestChange = IQGetRequestChange(tempOutcomingEntryId)
                .FirstOrDefault();

            requestChange.Verifier = GetUsernameLoginBySessionUserId();
            requestChange.TransitionName = transitionName;
            string channelUrl = GetNotifyToChannelUrl();
            var mezonMessage = ConvertStringRequestChangeToMezonMessage(requestChange.MessageReject, requestChange.Verifier);
            _mezonWebService.NotifyToChannelMezon(mezonMessage,channelUrl);
        }

        public async Task NotifyRequestChangeRejectAsync(long tempOutcomingEntryId, string transitionName)
        {
            var requestChange = IQGetRequestChange(tempOutcomingEntryId)
                .FirstOrDefault();

            requestChange.Verifier = GetUsernameLoginBySessionUserId();
            requestChange.TransitionName = transitionName;
            string channelUrl = GetNotifyToChannelUrl();
            var mezonMessage = ConvertStringRequestChangeToMezonMessage(requestChange.MessageReject, requestChange.Verifier);
            await _mezonWebService.NotifyToChannelMezonAsync(mezonMessage, channelUrl);
        }

        public void NotifyRequestChangeApprove(long tempOutcomingEntryId, string transitionName)
        {
            var requestChange = IQGetRequestChange(tempOutcomingEntryId)
                .FirstOrDefault();

            requestChange.Verifier = GetUsernameLoginBySessionUserId();
            requestChange.TransitionName = transitionName;
            string channelUrl = GetNotifyToChannelUrl();
            var mezonMessage = ConvertStringRequestChangeToMezonMessage(requestChange.MessageApprove, requestChange.Verifier);
            _mezonWebService.NotifyToChannelMezon(mezonMessage, channelUrl);        
        }

        public async Task NotifyRequestChangeApproveAsync(long tempOutcomingEntryId, string transitionName)
        {
            var requestChange = IQGetRequestChange(tempOutcomingEntryId)
                .FirstOrDefault();

            requestChange.Verifier = GetUsernameLoginBySessionUserId();
            requestChange.TransitionName = transitionName;
            string channelUrl = GetNotifyToChannelUrl();
            var mezonMessage = ConvertStringRequestChangeToMezonMessage(requestChange.MessageApprove, requestChange.Verifier);
            await _mezonWebService.NotifyToChannelMezonAsync(mezonMessage, channelUrl);
        }

        private MezonMessage ConvertStringRequestChangeToMezonMessage(string message, string verifier)
        {
            return new MezonMessage
            {
                t = message,
                mentions = new List<Mentions>
                {
                        new Mentions
                        {
                           username = verifier,
                           s = message.IndexOf(verifier)
                        }

                }
            };
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
                       CreatedBy = _userRepo.GetAll().Where(x => x.Id == outcom.CreatorUserId).Select(x => x.FullName).FirstOrDefault()
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
            var emailAddress = _userRepo.GetAll()
                .Where(x => x.Id == _session.UserId)
                .Select(x => x.EmailAddress)
                .FirstOrDefault();
            if (string.IsNullOrEmpty(emailAddress))
                return string.Empty;
            return emailAddress.Split('@')[0];
        }
        #endregion
    }
}
