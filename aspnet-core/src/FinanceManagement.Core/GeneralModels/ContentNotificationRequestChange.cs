using FinanceManagement.Helper;
using System;
using System.Collections.Generic;
using System.Text;

namespace FinanceManagement.GeneralModels
{
    public class ContentNotificationRequestChange
    {
        public long OutcomingEntryId { get; set; }
        public string OutcomingEntryName { get; set; }
        public double Value { get; set; }
        public double OldValue { get; set; }
        public string CurrencyName { get; set; }
        public string OldCurrencyName { get; set; }
        public string Verifier { get; set; }
        public long TempOutcomingEntryId { get; set; }
        public string ClientRootAddress { get; set; }
        public string Reason { get; set; }
        public string TransitionName { get; set; }
        public string GetURLMessage => string.Format(FinanceManagementConsts.LINK_REQUEST_CHANGE, ClientRootAddress, OutcomingEntryId, TempOutcomingEntryId);
        public string MessagePending => GetMessage(FinanceManagementConsts.WORKFLOW_STATUS_PENDINGCEO);
        public string MessageApprove => GetMessage(FinanceManagementConsts.WORKFLOW_STATUS_APPROVED);
        public string MessageReject => GetMessage(FinanceManagementConsts.WORKFLOW_STATUS_REJECTED);
        private string GetTransitionName(string workflowCode)
        {
            return string.IsNullOrEmpty(TransitionName) ? workflowCode : TransitionName;
        }
        private string GetMessage(string typeMessage)
        {
            string transitionName = GetTransitionName(typeMessage);
            switch (typeMessage)
            {
                case FinanceManagementConsts.WORKFLOW_STATUS_PENDINGCEO:
                    return Helpers.GetContentRequestChangePendingCEO(Verifier, OutcomingEntryId, OutcomingEntryName, Value, OldValue, transitionName, GetURLMessage, Reason, CurrencyName, OldCurrencyName);
                case FinanceManagementConsts.WORKFLOW_STATUS_APPROVED:
                    return Helpers.GetContentRequestChangeApprove(Verifier, OutcomingEntryId, OutcomingEntryName, Value, OldValue, transitionName, GetURLMessage, CurrencyName, OldCurrencyName);
                case FinanceManagementConsts.WORKFLOW_STATUS_REJECTED:
                    return Helpers.GetContentRequestChangeReject(Verifier, OutcomingEntryId, OutcomingEntryName, Value, OldValue, transitionName, GetURLMessage, CurrencyName, OldCurrencyName);
                default:
                    throw new NotImplementedException($"Not implement message type: {typeMessage}");
            }
        }
    }
}
