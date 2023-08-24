import { InvoiceService } from './../../../../service/api/invoice.service';
import { Router, ActivatedRoute } from '@angular/router';
import { AppComponentBase } from '@shared/app-component-base';
import { InvoiceDto } from './../../../../service/model/Invoice.dto';
import { Component, Injector, OnInit } from '@angular/core';

@Component({
  selector: 'app-general-tab',
  templateUrl: './general-tab.component.html',
  styleUrls: ['./general-tab.component.css']
})
export class GeneralTabComponent extends AppComponentBase implements OnInit {
  readMode = true
  invoice = {} as InvoiceDto
  invoiceId: number
  constructor(injector: Injector, private router: Router, private route: ActivatedRoute,
    private invoiceService: InvoiceService) {
    super(injector)
    this.invoiceId = Number(route.snapshot.queryParamMap.get("id"))
  }

  ngOnInit(): void {
    this.getInvoiceById();
  }
  editInvoice() {

  }
  cancelEdit() {

  }
  saveInvoice() {

  }
  getInvoiceById() {
    // this.invoiceService.getInvoiceById(this.invoiceId).subscribe(data => {
    //   this.invoice = data.result
    // })
  }

}
