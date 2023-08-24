import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { OutcomingEntrySupplierService } from './../../../../service/api/outcoming-entry-supplier.service';
import { SupplierDto } from './../supplier.component';
import { SupplierService } from './../../../../service/api/supplier.service';
import { Component, OnInit, Inject } from '@angular/core';

@Component({
  selector: 'app-link-supplier',
  templateUrl: './link-supplier.component.html',
  styleUrls: ['./link-supplier.component.css']
})
export class LinkSupplierComponent implements OnInit {
  isDisable = false
  supplierList: SupplierDto[]
  searchSupplier: string = ""
  constructor(private supplierServive: SupplierService, private outcomingSupplier: OutcomingEntrySupplierService,
    public dialogRef: MatDialogRef<LinkSupplierComponent>, @Inject(MAT_DIALOG_DATA) public data: any) { }
  supplier: any = {}
  ngOnInit(): void {
    this.getSupplier()
  }
  getSupplier() {
    this.supplierServive.getAll().subscribe(data => {
      this.supplierList = data.result.map(item => {
        item.searchText = `#${item.id} ${item.name}`;
        return item
      })
    })
  }
  saveAndClose() {
    let requestBody = {
      outcomingEntryId: this.data.outcomingEntryId,
      supplierId: this.supplier.id
    }
    this.outcomingSupplier.create(requestBody).subscribe(rs => {
      abp.notify.success("Link successful");

      this.dialogRef.close()
    })
  }

}
