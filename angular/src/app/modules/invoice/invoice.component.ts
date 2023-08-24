import { PERMISSIONS_CONSTANT } from '@app/constant/permission.constant';
import { MatDialog } from '@angular/material/dialog';
import { InvoiceService } from './../../service/api/invoice.service';
import { InvoiceDto } from './../../service/model/Invoice.dto';
import { Component, Injector, OnInit } from '@angular/core';
import { PagedListingComponentBase, PagedRequestDto, PagedResultResultDto } from '@shared/paged-listing-component-base';
import { finalize, catchError } from 'rxjs/operators';
import { CreateEditInvoiceComponent } from './create-edit-invoice/create-edit-invoice.component';
import * as FileSaver from 'file-saver';

@Component({
  selector: 'app-invoice',
  templateUrl: './invoice.component.html',
  styleUrls: ['./invoice.component.css']
})
export class InvoiceComponent extends PagedListingComponentBase<InvoiceDto> implements OnInit {
  protected list(request: PagedRequestDto, pageNumber: number, finishedCallback: Function): void {
    throw new Error('Method not implemented.');
  }
  protected delete(entity: InvoiceDto): void {
    throw new Error('Method not implemented.');
  }
  Finance_Invoice_Update = PERMISSIONS_CONSTANT.Finance_Invoice_Update
  Finance_Invoice_Create = PERMISSIONS_CONSTANT.Finance_Invoice_Create
  Finance_Invoice_Delete = PERMISSIONS_CONSTANT.Finance_Invoice_Delete
  requestParam:PagedRequestDto
  public invoiceList: InvoiceDto[] = []
  // protected list(request: PagedRequestDto, pageNumber: number, finishedCallback: Function): void {
  //   this.requestParam =request
  //   this.invoiceService
  //     .getAllPaging(request)
  //     .pipe(finalize(() => {
  //       finishedCallback();
  //     }))
  //     .subscribe((result: PagedResultResultDto) => {
  //       this.invoiceList = result.result.items;
  //       this.showPaging(result.result, pageNumber);
  //     });
  // }
  // protected delete(entity: any): void {
  //   abp.message.confirm(
  //     "Delete invoice '" + entity.name + "'?",
  //     "",
  //     (result: boolean) => {
  //       if (result) {
  //         this.invoiceService.deleteInvoice(entity.id).pipe(catchError(this.invoiceService.handleError)).subscribe(() => {
  //           abp.notify.success("Deleted invoice: " + entity.name);
  //           this.refresh()
  //         });
  //       }
  //     }
  //   );
  // }

  constructor(injector: Injector, private invoiceService: InvoiceService, private dialog: MatDialog) {
    super(injector)
  }

  ngOnInit(): void {
    //this.refresh();
  }
  // createInvoice() {
  //   this.showDialog("create")
  // }
  // editInvoice(invoice) {
  //   this.showDialog("edit", invoice)
  // }
  // showDialog(command: String, item?: any): void {
  //   let invoice = {} as InvoiceDto
  //   if (command == "edit") {
  //     invoice = {
  //       id: item.id,
  //       name: item.name,
  //       accountCode: item.accountCode,
  //       note: item.note,
  //       status: item.status,
  //       timeAt: item.timeAt,
  //       totalPrice: item.totalPrice,
  //       clientName:item.clientName,
  //       project: item.project
  //     }
  //   }
  //   let dialogRef = this.dialog.open(CreateEditInvoiceComponent, {
  //     data: {
  //       item: invoice,
  //       command: command,
  //     },
  //     width: "700px",
  //     disableClose: true,
  //   });
  //   dialogRef.afterClosed().subscribe(rs => {
  //     if (rs) {
  //       this.refresh()
  //     }
  //   })
  // }
  // viewDetail(invoice:InvoiceDto){
  //   if(this.permission.isGranted(this.Finance_Invoice_ViewDetail)){
  //     this.router.navigate(['app/invoiceDetail'], {
  //       queryParams: {
  //         id: invoice.id,
  //       }
  //     })
  //   }

  // }
  // downloadFile() {
  //   this.invoiceService.exportExcel(this.requestParam).subscribe(data => {
  //     const file = new Blob([this.convertFile(atob(data.result))], {
  //       type: "application/vnd.ms-excel;charset=utf-8"
  //     });
  //     FileSaver.saveAs(file,`Hoá đơn.xlsx`);
  //   })
  
  // }



}

