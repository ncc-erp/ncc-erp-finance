export interface StatisticResultDto {
  result: BanktransStatisticDto[],
  totalBankTransactions: TotalByCurrencyDto[],
  totalHoanTien: TotalByCurrencyDto[],
  totalOutcomingEntries: TotalByCurrencyDto[]
}
export interface BanktransStatisticDto {
  outcomingEntryId: number,
  outcomingEntryName: string,
  outcomingEntryStatus: string,
  valueMoney: number,
  money: string,
  outcomingEntryTypeName: string,
  totalValueMoneyBankTransaction: number,
  bankTransactions: BankTransDto[]
  totalBankTransactions: TotalByCurrencyDto[]
  totalHoanTien: TotalByCurrencyDto[]
}

export interface TotalByCurrencyDto {
  currencyId: number,
  currencyName: string,
  value: number,
  valueFormat: string
}

export interface BankTransDto {
  bankTransactionId: number
  bankTransactionName: string
  fromBankAccountId: number
  fromBankAccountName: string
  fromCurrencyName: string
  fromValueNumber: number
  fromValue: string
  toBankAccountId: number
  toCurrencyName: string
  toBankAccountName: string
  toValueNumber: number
  toValue: string
  timeAt: string
  bTransactionId: number
  bTransactionName: Storage
  currencyBTransaction: string
  valueMoneyBTransaction: number
  moneyBTransaction: number
}

export interface BankAccountStatisticDto {
  bankAccountId: number,
  bankAccountName: string,
  bankNumber: string,
  currencyName: string,
  baseBalance: number,
  increaseNumber: number,
  increase: string,
  reduceNumber: number,
  reduce: string,
  currentBalananceNumber: number,
  currentBalanace: string
}

export interface StatisticsIncomingEntryDto {
  currencyName: string,
  totalIncomingNumber: number,
  totalIncoming: string,
  totalBTransactionNubmer: number,
  totalBTransaction: string,
  diffMoneyNumber: number,
  diffMoney: string
}

export interface StatisticsOutcomingEntry {
  currencyName: string,
  totalOutcomingNumber: number,
  totalOutcoming: string,
  totalBankTransactionNumber: number,
  totalBankTransaction: string,
  diffMoneyNumber: number,
  diffMoney: string
  totalRefund: string
  totalRefundNumber: number
}

export interface StatisticByCurrencyDto {
  tienTe: string,
  duDauKi: number,
  duDauKiFormat: string,
  thuSo: number,
  thuSoFormat: string,
  thuSaoKe: number,
  thuSaoKeFormat: string,
  chenhLechThu: number,
  chenhLechThuFormat: string,
  chiSo: number,
  chiSoFormat: string,
  chiSaoKe: number,
  chiSaoKeFormat: string,
  hoanTien: number,
  hoanTienFormat: string,
  chenhLechChi: number,
  chenhLechChiFormat: string,
  duTheoSo: number,
  duTheoSoFormat: string,
  duTheoSaoKe: number,
  duTheoSaoKeFormat: string,
  chenhLech: number,
  chenhLechFormat: string,
  isShow: true
}
export interface ExchangeRates{
  tienTe: string;
  exchangeRate: number;
  ExchangeRateFormat: string;
}
export interface nomney{
  value: number,
  valueFormat: string,
}
