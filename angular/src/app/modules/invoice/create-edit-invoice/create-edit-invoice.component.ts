import { InvoiceService } from './../../../service/api/invoice.service';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { InvoiceDto } from './../../../service/model/Invoice.dto';
import { AppComponentBase } from '@shared/app-component-base';
import { Component, Inject, Injector, OnInit } from '@angular/core';

@Component({
  selector: 'app-create-edit-invoice',
  templateUrl: './create-edit-invoice.component.html',
  styleUrls: ['./create-edit-invoice.component.css']
})
export class CreateEditInvoiceComponent extends AppComponentBase implements OnInit {
  public isDisable = false;
  invoice = {} as InvoiceDto
  constructor(injector: Injector, public dialogRef: MatDialogRef<CreateEditInvoiceComponent>,
    @Inject(MAT_DIALOG_DATA) public data: any, private service: InvoiceService) {
    super(injector)
  }
  ngOnInit(): void {
    if (this.data.command == "edit") {
      this.invoice = this.data.item
    }
  }
  saveAndClose() {
    this.isDisable = true
    if (this.data.command == "create") {
      this.service.create(this.invoice).subscribe(rs => {
        this.dialogRef.close(this.invoice);
        abp.notify.success("Create new invoice: " + this.invoice.name)
      }, () => { this.isDisable = false })
    }
    else {
      this.service.update(this.invoice).subscribe(rs => {
        this.dialogRef.close(this.invoice);
        abp.notify.success("Updated invoice: " + this.invoice.name)
      }, () => { this.isDisable = false })
    }

  }
}
