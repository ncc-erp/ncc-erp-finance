namespace FinanceManagement
{
    public class FinanceManagementConsts
    {
        public const string LocalizationSourceName = "FinanceManagement";

        public const string ConnectionStringName = "Default";

        public const bool MultiTenancyEnabled = true;
        public const string RevenueClientCode = "RCC";
        public const string BalanceClientCode = "BCC";
        public const double DEFAULT_EXCHANGE_RATE = 1;
        public const string VND_CURRENCY_NAME = "VND";

        //Workflow Status
        public const string WORKFLOW_STATUS_START = "START";
        public const string WORKFLOW_STATUS_APPROVED = "APPROVED";
        public const string WORKFLOW_STATUS_PENDINGCEO = "PENDINGCEO";
        public const string WORKFLOW_STATUS_PENDINGCFO = "PENDINGCFO";
        public const string WORKFLOW_STATUS_TRANSFERED = "TRANSFERED";
        public const string WORKFLOW_STATUS_REJECTED = "REJECTED";
        public const string WORKFLOW_STATUS_END = "END";
        //Outcoming Entry Type
        public const string OUTCOMING_ENTRY_TYPE_SALARY = "SALARY";
        public const string OUTCOMING_ENTRY_TYPE_TEAM_BUILDING = "team_building";
        public const string OUTCOMING_ENTRY_TYPE_CURRENCY_EXCHANGE = "CURRENCY EXCHANGE";
        public const string OUTCOMING_ENTRY_TYPE_MONEY_TRANSFER = "MONEY TRANSFER";

        public const string BRANCH_CODE_CTY = "CTY";

        public const string LINK_REQUEST_CHANGE = "{0}app/requestDetail/main?id={1}&tempId={2}";
        public const string TEN_GHI_NHAN_THU_WHEN_BAN_NGOAI_TE = "Thu bán {0} {1} tỉ giá trung bình {2}";
        public const string TEN_GHI_NHAN_THU_KHI_MUA_NGOAI_TE = "MUA {0} {1} tỉ giá trung bình {2}";

        public const string BANK_ACCOUNT_OPTION_NAME = "{0} ({1}) {2} [{3}]";

        //AccountType
        public const string ACCOUNT_TYPE_EMPLOYEE = "EMPLOYEE";
        public const string ACCOUNT_TYPE_CLIENT = "CLIENT";
        public const string ACCOUNT_TYPE_OTHER = "OTHER";
        public const string ACCOUNT_TYPE_COMPANY = "COMPANY";
        public const string ACCOUNT_TYPE_SUPPLIER = "SUPPLIER";
    }
}
