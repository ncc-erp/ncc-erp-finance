import { BehaviorSubject } from 'rxjs';
import { NgxMatDateFormats } from "@angular-material-components/datetime-picker";
import { BankTransactionFilterDateTimeType, ExpressionEnum } from "./AppEnums";

export class AppConsts {
  static remoteServiceBaseUrl: string;
  static appBaseUrl: string;
  static googleClientId: string;
  static enableNormalLogin: boolean;
  static appBaseHref: string; // returns angular's base-href parameter value if used during the publish
  static periodId = new BehaviorSubject<number>(0)
  static periodStartDate = null

  static localeMappings: any = [];

  static readonly userManagement = {
    defaultAdminUserName: "admin",
  };

  static readonly localization = {
    defaultLocalizationSourceName: "FinanceManagement",
  };

  static readonly authorization = {
    encryptedAuthTokenName: "enc_auth_token",
  };

  static readonly ExpressionOptions = [
    { value: ExpressionEnum.NO_FILTER, name: "Không filter" },
    { value: ExpressionEnum.LARGER_OR_EQUAL, name: ">=" },
    { value: ExpressionEnum.LESS_OR_EQUAL, name: "<=" },
    { value: ExpressionEnum.EQUAL, name: "=" },
    { value: ExpressionEnum.FT, name: "Trong khoảng" },
  ];

  static readonly VALUE_OPTIONS_ALL: number | string = "";
  static readonly FIRST_PAGE = 1;
  static readonly EXCHANGE_RATE_DEFAULT = 1;
  static DEFAULT_CURRENCY_CODE: string;
  static DEFAULT_CURRENCY_ID: number;
  static CONFIG_CURRENCY_OUTCOMINGENTRY: boolean;
  static RefreshPeriod = -1;
  static GetFirstPeriod = -2;
}
export const DATE_TIME_OPTIONS = [
  "All",
  "Day",
  "Week",
  "Month",
  "Quarter",
  "Half-year",
  "Year",
  "Custom",
];

export const CUSTOM_DATE_FORMATS = {
  parse: {
    dateInput: "DD/MM/YYYY",
  },
  display: {
    dateInput: "DD/MM/YYYY",
    monthYearLabel: "MM/YYYY",
    dateA11yLabel: "DD/MM/YYYY",
    monthYearA11yLabel: "MMMM YYYY",
  },
};

export const NGX_CUSTOM_DATE_FORMATS: NgxMatDateFormats = {
  parse: {
    dateInput: "DD/MM/YYYY HH:mm",
  },
  display: {
    dateInput: "DD/MM/YYYY HH:mm",
    monthYearLabel: "MM/YYYY",
    dateA11yLabel: "DD/MM/YYYY",
    monthYearA11yLabel: "MMMM YYYY",
  },
};
export const OPTION_ALL: number = -1;
export const BANK_TRANSCATION_DATE_TIME_OPTIONS = [
  { name: "All", value: BankTransactionFilterDateTimeType.NO_FILTER },
  { name: "Ngày giao dịch", value: BankTransactionFilterDateTimeType.TRANSACTION_TIME },
  { name: "Ngày tạo", value: BankTransactionFilterDateTimeType.CREATE_TIME },
];
