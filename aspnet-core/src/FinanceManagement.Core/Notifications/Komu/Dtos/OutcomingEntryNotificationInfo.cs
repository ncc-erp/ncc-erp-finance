using FinanceManagement.GeneralModels;
using FinanceManagement.Helper;
using FinanceManagement.Managers.Invoices.Dtos;
using FinanceManagement.Notifications.Mezon.Dto;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;

namespace FinanceManagement.Notifications.Komu.Dtos
{
    public class OutcomingEntryNotificationInfo
    {
        public long Id { get; set; }
        public string OutcomingEntryName { get; set; }
        public string OutcomingEntryTypeCode { get; set; }
        public double OutcomingEntryValue { get; set; }
        public string CurrencyCode { get; set; }
        public string BranchName { get; set; }
        public DateTime CreationTime { get; set; }
        public string CreatedBy { get; set; }
        public string StatusCode { get; set; }
        public string Verifier { get; set; }
        public string MessageSalaryFromHRM => Helpers.GetContentSalarySendNotifyKomu(Id,OutcomingEntryName,Helpers.FormatMoneyVND(OutcomingEntryValue),FinanceManagementConsts.WORKFLOW_STATUS_APPROVED);
        public string MessageTeamBuildingFromTimesheet => Helpers.GetContentTeamBuildingSendNotifyKomu(Id, OutcomingEntryName, Helpers.FormatMoneyVND(OutcomingEntryValue), FinanceManagementConsts.WORKFLOW_STATUS_START);
        public string MessageMainContentChangeStatus => Helpers.GetContentSendNotifyKomu(OutcomingEntryName,OutcomingEntryTypeCode,BranchName, CreatedBy, CreationTime);
        public string MessageSubContentChangeStatus => Helpers.GetSubContentSendNotifyKomu(Verifier, StatusCode, Id, Helpers.FormatMoneyVND(OutcomingEntryValue), CurrencyCode);
        public MezonMessage GenerateMezonMessage(string address)
        {
            string message = $"{MessageSubContentChangeStatus}\n{address}app/requestDetail/main?id={Id} {MessageMainContentChangeStatus}";

            return new MezonMessage
            {
                t = message,
                mentions = new List<Mentions>
        {
            new Mentions
            {
                username = Verifier,
                s = message.IndexOf(Verifier)
            }
        }
            };
        }



    }
}
