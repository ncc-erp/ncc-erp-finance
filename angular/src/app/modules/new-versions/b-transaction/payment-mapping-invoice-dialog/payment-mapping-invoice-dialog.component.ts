import { Component, Inject, Injector, OnInit } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { AppComponentBase } from '@shared/app-component-base';
import { UtilitiesService } from '@app/service/api/new-versions/utilities.service';
import { BtransactionService } from '@app/service/api/new-versions/btransaction.service';
import { AppConfigurationService } from '@app/service/api/app-configuration.service';
import { CommonService } from '@app/service/api/new-versions/common.service';
import { ValueAndNameModel } from '@app/service/model/common-DTO';
import { PaymentInvoiceForAccountMapping, InvoicePaymentMapping } from '@app/service/model/b-transaction.model';
import { IncomingEntryTypeOptions } from '../link-revenue-ecognition-dialog/link-revenue-ecognition-dialog.component';
import { DefaultIncomingEntryType } from '../currency-exchange/currency-exchange.component';
import { LableDirection } from '@shared/selection-customs/selection-customs.component';
import { InvoiceService } from '@app/service/api/invoice.service';

@Component({
  selector: 'app-payment-mapping-invoice-dialog',
  templateUrl: './payment-mapping-invoice-dialog.component.html',
  styleUrls: ['./payment-mapping-invoice-dialog.component.css'],
})
export class PaymentMappingInvoiceDialogComponent extends AppComponentBase implements OnInit {
  // selections
  customerOptions: ValueAndNameModel[] = [];
  filteredCustomerOptions: ValueAndNameModel[] = [];
  public lableDirectionKhachHang: LableDirection = LableDirection.Left;

  // invoices to map (UI rows)
  invoiceMappings: UiInvoiceRow[] = [];

  // currency converts (if cross-currency)
  currencyNeedConverts: CurrencyNeedConvert[] = [];

  // incoming entry type tree + default
  public incomingEntryTypeOptions: IncomingEntryTypeOptions;
  public isDefaultIncomingEntryType = false;
  public defaultIncomingEntryType: number;

  // payload
  payment: PaymentInvoiceForAccountMapping = new PaymentInvoiceForAccountMapping();

  // state
  public isFocusing = false;
  public isDisable = false;
  public searchCustomer = '';

  constructor(
    injector: Injector,
    public dialogRef: MatDialogRef<PaymentMappingInvoiceDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: PaymentMappingInvoiceDialogData,
    public _utilities: UtilitiesService,
    private _btransaction: BtransactionService,
    private _configuration: AppConfigurationService,
    private _common: CommonService,
    private _invoice: InvoiceService,
  ) {
    super(injector);
  }

  ngOnInit(): void {
    this.setCustomerOptions();
    this.getTreeIncomingEntries();
    this.setDefaultIncomingEntryTypes();
  }

  // ============== Init data ==============

  private setCustomerOptions(): void {
    this._common.getAllClient().subscribe((response) => {
      if (!response.success) return;
      this.customerOptions = response.result;
      this.filteredCustomerOptions = [...this.customerOptions];
    });
  }

  private getTreeIncomingEntries(): void {
    this._common.getTreeIncomingEntries().subscribe((response) => {
      if (!response.success) return;
      this.incomingEntryTypeOptions = {
        item: { id: 0, name: '', parentId: null },
        children: [...response.result],
      };
    });
  }

  private setDefaultIncomingEntryTypes(): void {
    this._configuration.getDefaultMaLoaiThuKhachHangBonus().subscribe((response) => {
      if (!response.success) return;
      if (response.result) {
        this.payment.incomingEntryTypeId = Number(response.result);
      }
      this.defaultIncomingEntryType = response.result;
      this.isDefaultIncomingEntryType =
        !!response.result && response.result == this.payment.incomingEntryTypeId;
    });
  }

  // ============== Handlers ==============

  handleSelectOpenedChange(isOpen: boolean) {
    this.isFocusing = isOpen;
    if (isOpen && this.filteredCustomerOptions.length === 0) {
      this.searchCustomer = '';
      this.filteredCustomerOptions = [...this.customerOptions];
    }
  }
  focusOut() {
    this.isFocusing = false;
  }

  incomingEntryTypeIdChange() {
    this.isDefaultIncomingEntryType =
      this.defaultIncomingEntryType === this.payment.incomingEntryTypeId;
  }

  defaultIncomingEntryTypeChange(evt: { checked: boolean }) {
    this.isDefaultIncomingEntryType = !!evt?.checked;
    if (this.isDefaultIncomingEntryType) {
      this._configuration
        .setDefaultMaLoaiThuKhachHangBonus({
          id: this.payment.incomingEntryTypeId?.toString(),
        } as DefaultIncomingEntryType)
        .subscribe(() => {
          abp.notify.success('Update default incoming entry successfully!');
        });
    } else {
      this._configuration.clearDefaultMaLoaiThuKhachHangBonus().subscribe(() => {
        abp.notify.success('Clear default incoming entry successfully!');
      });
    }
  }

  customerHandler() {
    if (!this.payment.accountId) return;

    // 1) load invoices for mapping (đúng service + đúng route)
    this._invoice.getListInvoiceByAccountId(this.payment.accountId).subscribe((res) => {
      if (!res.success) return;
      this.invoiceMappings = (res.result || []).map((x: any) => ({
        invoiceId: x.invoiceId,
        nameInvoice: x.nameInvoice,           // 👈 hiển thị tên hóa đơn
        remainValue: x.remainValue,
        currencyName: x.currencyName,
        value: null,
      }));
    });

    // 2) check currency convert need
    this._btransaction
      .checkAccount(this.data.bTransactionId, this.payment.accountId)
      .subscribe((response) => {
        if (!response.success) return;
        this.currencyNeedConverts = response.result;
      });
  }

  // ============== Submit ==============

  process() {
    this.isDisable = true;

    // build payload
    this.payment.bTransactionId = this.data.bTransactionId;
    this.payment.currencyNeedConverts = this.currencyNeedConverts;

    // chỉ gửi các invoice có value > 0
    this.payment.invoiceMappings = this.invoiceMappings
      .filter((x) => x.value && Number(x.value) > 0)
      .map<InvoicePaymentMapping>((x) => ({
        invoiceId: x.invoiceId,
        value: Number(('' + x.value).toString().replace(/,/g, '')),
      }));

    const accountName =
      this.customerOptions.find((x) => x.value == this.payment.accountId)?.name ?? '';

    abp.message.confirm(
      `Khách hàng <strong>${accountName}</strong> thanh toán`,
      '',
      (result: boolean) => {
        if (result) {
          this.doSave();
        } else {
          this.isDisable = false;
        }
      },
      true
    );
  }

  private doSave() {
    this._btransaction.paymentForAccountMapping(this.payment).subscribe(
      (response) => {
        if (!response.success) return;
        abp.notify.success('Updated successfully');
        this.dialogRef.close();
      },
      () => {
        this.isDisable = false;
      }
    );
  }

  // ============== Validate button ==============

  isBtnDisable() {
    if (this.payment.isCreateBonus) {
      return (
        !this.payment.incomingEntryName ||
        !this.payment.incomingEntryValue ||
        !this.payment.incomingEntryTypeId ||
        this.isDisable
      );
    }
    return this.isDisable;
  }
}

// ======= Interfaces =======

export interface PaymentMappingInvoiceDialogData {
  bTransactionId: number;
  money: number;
  currencyName: string;
}

export interface CurrencyNeedConvert {
  fromCurrencyId: number;
  fromCurrencyName: string;
  toCurrencyId: number;
  toCurrencyName: string;
  isReverseExchangeRate: boolean;
  exchangeRate?: number;
}

// UI row cho bảng invoice (thêm field hiển thị)
interface UiInvoiceRow {
  invoiceId: number;
  nameInvoice: string;     // 👈 NEW
  remainValue: number;
  currencyName: string;
  value: any;              // keep any for mask="separator"; parse on submit
}
