import { result } from 'lodash';
import { AccountTypeService } from './../../../service/api/account-type.service';
import { AccountTypeDto } from './../../accountant-account/create-edit-accountant-account/create-edit-accountant-account.component';
import { AccountService } from './../../../service/api/account.service';
import { Component, Inject, Injector, OnInit } from '@angular/core';
import { MatDialog, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { CurrencyService } from '@app/service/api/currency.service';
import { RevenueManagedService } from '@app/service/api/revenue-managed.service';
import { AppComponentBase } from '@shared/app-component-base';
import { RevenueManagedDto } from '../revenue-managed.component';
import * as moment from 'moment';
import * as FileSaver from 'file-saver';

@Component({
  selector: 'app-create-edit-revenuemanaged',
  templateUrl: './create-edit-revenuemanaged.component.html',
  styleUrls: ['./create-edit-revenuemanaged.component.css']
})
export class CreateEditRevenuemanagedComponent extends AppComponentBase implements OnInit {
  public isDisable = false;
  isCreated = true;
  invoice = new RevenueManagedDto();
  sendDate: Date = new Date()
  deadline: Date = new Date()
  customerList = []
  accountTypeList: AccountTypeDto[] = []
  selectedAccountType = {} as any
  tempAccountTypeList: any[] = []
  listFiles: File[] = [];
  searchAccount: string = ""
  revenueManagedId: number = 0
  months: any[] = [
    { value: 1, text: "Tháng 1" },
    { value: 2, text: "Tháng 2" },
    { value: 3, text: "Tháng 3" },
    { value: 4, text: "Tháng 4" },
    { value: 5, text: "Tháng 5" },
    { value: 6, text: "Tháng 6" },
    { value: 7, text: "Tháng 7" },
    { value: 8, text: "Tháng 8" },
    { value: 9, text: "Tháng 9" },
    { value: 10, text: "Tháng 10" },
    { value: 11, text: "Tháng 11" },
    { value: 12, text: "Tháng 12" },
  ]
  currencies: any[] = [];
  listStatus: any[] = [
    { value: 0, text: "Chưa trả" },
    { value: 1, text: "Trả một phần" },
    { value: 2, text: "Hoàn thành" },
    { value: 3, text: "Không trả" }
  ];
  listRemindStatus = [
    { value: 1, text: "Nhắc lần 1" },
    { value: 2, text: "Nhắc lần 2" },
    { value: 3, text: "Nhắc lần 3" },
  ]

  constructor(injector: Injector, public dialogRef: MatDialogRef<CreateEditRevenuemanagedComponent>, private accountService: AccountService,
    private accounttypeService: AccountTypeService,
    private dialog: MatDialog,
    @Inject(MAT_DIALOG_DATA) public data: any, private _service: RevenueManagedService, private _currencyService: CurrencyService) {
    super(injector);
  }

  ngOnInit(): void {
    this.invoice.month = new Date().getMonth()
    if (this.data.command == "edit") {
      this.invoice = this.data.item
      this.isCreated = false;
      this.revenueManagedId = this.invoice.id
      this._service.revenueGetFiles(this.invoice.id).subscribe((res: any) => {
        if (res.success) {
          this.listFiles = res.result.map((item) => {
            let file = new Blob([this.s2ab(atob(item.bytes))], {
              type: ""
            });
            let arrayOfBlob = new Array<Blob>();
            arrayOfBlob.push(file);
            item = new File(arrayOfBlob, item.fileName)
            return item;
          })
        }
      })
    }
    this.getCurrencyDropDown();
    this.getAllCustomer();
    this.getAllAccountType();
  }
  getCurrencyDropDown() {
    this._currencyService.GetAllForDropdown().subscribe((res) => {
      //console.log(res);
      this.currencies = res.result;
    })
  }
  getAllCustomer() {
    this.accountService.getAll().subscribe(data => {
      this.tempAccountTypeList = data.result
      if (this.selectedAccountType?.name) {
        this.customerList = this.tempAccountTypeList.filter(item => {
          return item.accountTypeCode == this.selectedAccountType.code
        })
      }
      else {
        this.customerList = data.result
      }
    })
  }
  getAllAccountType() {
    this.accounttypeService.getAll().subscribe(data => {
      this.accountTypeList = data.result;
      if (this.data.item) {
        this.selectedAccountType = this.accountTypeList.filter(item => item.code == this.data.item?.accountTypeCode)[0]
      }
    })
  }
  onAccountTypeChange() {
    this.getAllCustomer()
  }
  saveAndClose() {
    this.invoice.sendInvoiceDate = moment(this.invoice.sendInvoiceDate).format("YYYY-MM-DD")
    this.invoice.deadline = moment(this.invoice.deadline).format("YYYY-MM-DD")

    this.isDisable = true
    if (this.data.command == "create") {
      this._service.create(this.invoice).subscribe(rs => {
        if (rs.success) {
          this._service.revenueUploadFiles(this.listFiles, rs.result).subscribe((rs) => {
            if (rs.success) {
              this.dialogRef.close(this.invoice);
              abp.notify.success(rs.result)
            }
          })
        }
      }, () => { this.isDisable = false })
    }
    else {
      //console.log("dasdasd",this.listFiles)
      this._service.update(this.invoice).subscribe(rs => {
        if (rs.success) {
          this._service.revenueUploadFiles(this.listFiles, this.invoice.id).subscribe((res) => {
            this.dialogRef.close(this.invoice);
            abp.notify.success(rs.result)
          })
        }
      }, () => { this.isDisable = false })
    }

  }
  refreshFiles() {
    this.listFiles.splice(0, this.listFiles.length)
  }
  uploadFiles(event) {
    this.listFiles.push(...event.target.files)
  }
  removeFile(file: File) {
    let index = this.listFiles.indexOf(file);
    this.listFiles.splice(index, 1)
  }
  downloadFile(name) {
    this._service.revenueDownloadFile(name).subscribe((response: any) => {
      const file = new Blob([this.s2ab(atob(response.result))], {
        type: ""
      });
      FileSaver.saveAs(file, name);
    })
  }
  s2ab(s) {
    var buf = new ArrayBuffer(s.length);
    var view = new Uint8Array(buf);
    for (var i = 0; i != s.length; ++i) view[i] = s.charCodeAt(i) & 0xFF;
    return buf;
  }
}
