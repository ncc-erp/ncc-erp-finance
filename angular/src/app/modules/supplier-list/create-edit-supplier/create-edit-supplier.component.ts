import { SupplierDto } from './../../expenditure-request-detail/supplier/supplier.component';
import { SupplierService } from '@app/service/api/supplier.service';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { Component, OnInit, Inject } from '@angular/core';
import { catchError } from 'rxjs/operators';

@Component({
  selector: 'app-create-edit-supplier',
  templateUrl: './create-edit-supplier.component.html',
  styleUrls: ['./create-edit-supplier.component.css']
})
export class CreateEditSupplierComponent implements OnInit {
  isDisable =false
  supplier={} as SupplierDto
  constructor(
    @Inject(MAT_DIALOG_DATA) public data: any, private supplierService:SupplierService,
    public dialogRef:MatDialogRef<CreateEditSupplierComponent>
    
  ) {  }
  isEdit: boolean;

  ngOnInit(): void {
    if(this.data.command == "edit"){
      this.supplier=this.data.item
    }
  }
  saveAndClose() {
    this.isDisable = true
    if (this.data.command == "create") {
      this.supplierService.create(this.supplier).pipe(catchError(this.supplierService.handleError)).subscribe((res) => {
        abp.notify.success("created supplier ");
        this.dialogRef.close();
      }, () => this.isDisable = false);
    }else if(this.data.command=="link"){
      this.supplier.outcomingEntryId= Number(this.data.id)
      this.supplierService.create(this.supplier).pipe(catchError(this.supplierService.handleError)).subscribe((res) => {
        abp.notify.success("created supplier ");
        this.dialogRef.close();
      }, () => this.isDisable = false);
    }
    else {
      this.supplierService.update(this.supplier).pipe(catchError(this.supplierService.handleError)).subscribe((res) => {
        abp.notify.success("edited supplier ");
        this.dialogRef.close();
      }, () => this.isDisable = false);
    }
  }

}
