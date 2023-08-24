using System;
using System.Collections.Generic;
using System.Text;

namespace FinanceManagement
{
    public class Constants
    {
        //Workflow Status
        public const string WORKFLOW_STATUS_START = "START";
        public const string WORKFLOW_STATUS_APPROVED = "APPROVED";
        public const string WORKFLOW_STATUS_PENDINGCEO = "PENDINGCEO";
        public const string WORKFLOW_STATUS_PENDINGCFO = "PENDINGCFO";
        public const string WORKFLOW_STATUS_TRANSFERED = "TRANSFERED";
        public const string WORKFLOW_STATUS_REJECTED = "REJECTED";
        public const string WORKFLOW_STATUS_END = "END";
        public const string WORKFLOW_STATUS_OTHER_END = "OTHEREND";
        public const string WORKFLOW_STATUS_OR_YCTD_PENDINGCEO = "PENDINGCEO_OR_YCTDPENDINGCEO";

        //AccountType
        public const string ACCOUNT_TYPE_EMPLOYEE = "EMPLOYEE";
        public const string ACCOUNT_TYPE_CLIENT = "CLIENT";
        public const string ACCOUNT_TYPE_OTHER = "OTHER";
        public const string ACCOUNT_TYPE_COMPANY = "COMPANY";
        public const string ACCOUNT_TYPE_SUPPLIER = "SUPPLIER";

        //Account
        public const string ACCOUNT_CODE_NCCPLUS = "NCCPLUS";

        //Currency
        public const string CURRENCY_VND = "VND";

        //Role
        public const string ROLE_KETOAN = "Kế toán";
        public const string ROLE_ADMIN = "Admin";
        public const string ROLE_SUPPORT_KE_TOAN = "Support kế toán";
        public const string ROLE_CEO = "CEO";
        public const string ROLE_CFO = "CFO";

        //Outcoming Entry Type
        public const string OUTCOMING_ENTRY_TYPE_SALARY = "SALARY";
        public const string OUTCOMING_ENTRY_TYPE_CURRENCY_EXCHANGE = "CURRENCY EXCHANGE";
        public const string OUTCOMING_ENTRY_TYPE_MONEY_TRANSFER = "MONEY TRANSFER";

        //Incoming Entry Type
        public const string INCOMING_ENTRY_TYPE_CURRENCY_EXCHANGE = "CURRENCY EXCHANGE";
        public const string INCOMING_ENTRY_TYPE_MONEY_TRANSFER = "MONEY TRANSFER";

        //Bank
        public const string BANK_TCB = "TCB";

        //BankAccount
        public const string BANKACCOUNT_TRANSFER_FEE = "Ghi nhận phí giao dịch";

        //Branch
        public const string BRANCH_CODE_HN = "HN";

        //Komu Channel Type 
        public const string PM_CHANNEL = "sendMessageToThongBaoPM";
        public const string GENERAL_CHANNEL = "sendMessageToThongBao";
        public const string FINANCE_CHANNEL = "sendMessageToFinance";

    }
}
