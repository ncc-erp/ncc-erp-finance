import { LinkSupplierComponent } from './link-supplier/link-supplier.component';
import { MatDialog } from '@angular/material/dialog';
import { OutcomingEntrySupplierService } from './../../../service/api/outcoming-entry-supplier.service';
import { ActivatedRoute } from '@angular/router';
import { SupplierService } from './../../../service/api/supplier.service';
import { Component, Injector, OnInit } from '@angular/core';
import { catchError } from 'rxjs/operators';
import { CreateEditSupplierComponent } from '@app/modules/supplier-list/create-edit-supplier/create-edit-supplier.component';
import { supportsPassiveEventListeners } from '@angular/cdk/platform';
import { AppComponentBase } from '@shared/app-component-base';
import { PERMISSIONS_CONSTANT } from '@app/constant/permission.constant';



@Component({
  selector: 'app-supplier',
  templateUrl: './supplier.component.html',
  styleUrls: ['./supplier.component.css']
})
export class SupplierComponent  extends AppComponentBase {
  requestId: string
  supplierList: SupplierDto[]
  constructor(private supplier: SupplierService, private route: ActivatedRoute, injector:Injector,
    private outcomeSupplierService:OutcomingEntrySupplierService, private dialog:MatDialog) { 
      super(injector)
    }

  ngOnInit(): void {
    this.requestId = this.route.snapshot.queryParamMap.get("id")
    this.getAllSupplier()
  }
  getAllSupplier() {
    this.supplier.getAllByOutcomingEntry(this.requestId).subscribe(data => {
      this.supplierList = data.result
    })
  }

  deleteLinked(item: any) {
    abp.message.confirm(
      "Deleted '" +item.name+ "'?",
      "",
      (result: boolean) => {
        if (result) {
          this.outcomeSupplierService.RemoveSupplierFromOutcomingEntry(item.outcomingEntrySupplierId).pipe(catchError(this.outcomeSupplierService.handleError)).subscribe(() => {
            abp.notify.success("Deleted linked "+item.name);
            this.getAllSupplier()
          });
        }
      }
    );
  }
  linkToSupplier(){
   let dialogRef= this.dialog.open(LinkSupplierComponent, {
      data: {
        item: {},
        outcomingEntryId: this.requestId
      },
      width: "900px",
      disableClose: true,
    });
    dialogRef.afterClosed().subscribe(()=>{
      this.getAllSupplier()
    })
  }
 createSupplier(){
    let dialogRef= this.dialog.open(CreateEditSupplierComponent, {
      data:{
        command: "link",
        id: this.requestId
      }, width: "700px",
    })
    dialogRef.afterClosed().subscribe(()=>{
      this.getAllSupplier();
    })
  }

  isShowTabSupplier(){
    return this.permission.isGranted(PERMISSIONS_CONSTANT.Finance_OutcomingEntry_OutcomingEntryDetail_TabSupplier);
  }
  isShowCreateBtn(){
    return this.permission.isGranted(PERMISSIONS_CONSTANT.Finance_OutcomingEntry_OutcomingEntryDetail_TabSupplier_CreateSupplier)
  }
  isShowLinkBtn(){
    return this.permission.isGranted(PERMISSIONS_CONSTANT.Finance_OutcomingEntry_OutcomingEntryDetail_TabSupplier_LinkToSupplier)
  }
  isShowDeleteLinkBtn(){
    return this.permission.isGranted(PERMISSIONS_CONSTANT.Finance_OutcomingEntry_OutcomingEntryDetail_TabSupplier_DeleteLinkToSupplier)
  }
  
}
export class SupplierDto {
  name: string;
  phoneNumber: string;
  address: string;
  contactPersonName: string;
  contactPersonPhone: string;
  taxNumber: string;
  outcomingEntrySupplierId: number;
  outcomingEntryId: number;
  id: number;
  searchText: string
}
