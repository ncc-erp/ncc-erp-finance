import { TenantAvailabilityState } from "@shared/service-proxies/service-proxies";

export class AppTenantAvailabilityState {
  static Available: number = TenantAvailabilityState._1;
  static InActive: number = TenantAvailabilityState._2;
  static NotFound: number = TenantAvailabilityState._3;
}

export enum SortDirectionEnum {
  Ascending = 0,
  Descending = 1
}

export enum DateSelectorEnum {
  ALL = "All",
  DAY = "Day",
  WEEK = "Week",
  MONTH = "Month",
  QUARTER = "Quarter",
  HALF_YEAR = "Half-year",
  YEAR = "Year",
  CUSTOM = "Custom",
}
export enum ExpressionEnum {
  NO_FILTER = 0,
  LESS_OR_EQUAL = 1,
  LARGER_OR_EQUAL = 2,
  EQUAL = 3,
  FT = 4,
}
export enum BTransactionStatusColor {
  PENDING = "rgb(253, 126, 20)",
  DONE = "rgb(40, 167, 69)",
  DEFAULT = "",
}
export enum BTransactionStatus {
  PENDING = 0,
  DONE = 1,
}

export enum CurrencyColor {
  VND = "#000",
  USD = "blue",
  YEN = "rgb(66, 3, 44)",
  EURO = "orange",
  KRW = "rgb(135, 88, 255)",
}

export enum RevenueManagedStatus {
  Not_Yet = 0,
  Paid_Part = 1,
  Done = 2,
  Not_Paid = 3,
  Only_Paid_Part = 4,
}
export enum InvoiceStatus {
  CHUA_TRA = 0,
  TRA_1_PHAN = 1,
  HOAN_THANH = 2,
  KHONG_TRA = 3,
  CHI_TRA_1_PHAN = 4,
}
export enum RevenueManagedStatusAccount {
  NOTE_YET = 0,
  DONE = 1,
}

export enum CommandDialog {
  CREATE,
  EDIT,
}
export enum ActionTypeEnum {
  NEW = 0,
  UPDATE = 1,
  DELETE = 2,
  NO_ACTION = 3,
}

export enum StatusEnum {
  ALL = -1,
  ACTIVE = 1,
  INACTIVE = 0,
}
export enum AccountTypeEnum {
  CLIENT,
  EMPLOYEE,
  COMPANY,
  MEDIUM,
  OTHER,
  SUPPLIER,
  FREELANCER
}
export enum BankTransactionFilterDateTimeType{
  NO_FILTER = 0,
  TRANSACTION_TIME = 1,
  CREATE_TIME = 2,
}
export enum ExpenseType{
  REAL_EXPENSE = 0,
  NON_EXPENSE = 1
}
export enum TypeFilterTypeOptions{
  OUTCOMING_ENTRY_TYPE = 0,
  INCOMING_ENTRY_TYPE = 1,
}
