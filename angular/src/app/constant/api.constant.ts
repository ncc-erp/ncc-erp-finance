export const APP_CONSTANT = {
  TypeViewHomePage: {
    Week: 0,
    Month: 1,
    Quater: 2,
    Year: 3,
    AllTime: 4,
    CustomTime: 5,
    Half_Year: 6
  },
  StatusStyle: {
    START: "badge badge-pill badge-primary",
    APPROVED: "badge badge-pill badge-success",
    END: "badge badge-pill badge-end",
    REJECTED: "badge badge-pill badge-danger",
    PENDINGCEO: "badge badge-pill badge-warning",
    PENDINGCFO: "badge badge-pill badge-dark",
    TRANSFERRED: "badge badge-pill badge-info",
    PENDINGIT: "badge badge-pill badge-light",
    REJECTEDIT: "badge badge-pill badge-danger"
  },
  InvoiceStatus: {
    0: "Tạo mới",
    1: "Đã gửi",
    2: "Thanh toán một phần",
    3: "Đã thanh toán",
    4: "Không thể thanh toán",
  },
  InvoiceCreatedBy:
  {
    0: "Finance",
    1: "Project"
  },
  RevenueManagedStatus: {
    0: "Chưa trả",
    1: "Trả một phần",
    2: "Hoàn thành",
    3: "Không trả"
  },
  RemindStatus: {
    1: "Nhắc lần 1",
    2: "Nhắc lần 2",
    3: "Nhắc lần 3"
  },
  ActionStyle: {
    DELETE: "badge badge-pill badge-danger",
    UPDATE: "badge badge-pill badge-warning",
    NEW: "badge badge-pill badge-success",
    NO_ACTION: "d-none"
  },
  OverviewOutcomingEntry: {
    START: "outcoming-entry-start",
    APPROVED: "outcoming-entry-approved",
    END: "outcoming-entry-executed",
    REJECTED: "outcoming-entry-reject",
    PENDINGCEO : "outcoming-entry-pending",
    PENDINGCEO_OR_YCTDPENDINGCEO : "outcoming-entry-pending",
  },
  OverviewBTransactionStatus: {
    PENDING: "bg-warning",
    DONE: "bg-success"

  }
}
