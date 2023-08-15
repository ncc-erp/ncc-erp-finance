import { Component, Injector, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { InvoiceService } from '@app/service/api/invoice.service';
import { AppComponentBase } from '@shared/app-component-base';

@Component({
  selector: 'app-transaction-tab',
  templateUrl: './transaction-tab.component.html',
  styleUrls: ['./transaction-tab.component.css']
})
export class TransactionTabComponent extends AppComponentBase implements OnInit {
  invoiceId: number;
  listBankTransacations: BankTransactionDto[]= [];
  isTableLoading: boolean = false
  constructor(injector: Injector, private router: Router, private route: ActivatedRoute,
    private invoiceService: InvoiceService) {
    super(injector);
    this.invoiceId = Number(route.snapshot.queryParamMap.get("id"))
  }

  ngOnInit(): void {
    this.getBankTransactionsByInvoice();
  }
  getBankTransactionsByInvoice(){
    this.isTableLoading = true
    // this.invoiceService.getBankTransactionsByInvoice(this.invoiceId).subscribe( data => {
    //   this.isTableLoading = false
    //   this.listBankTransacations = data.result;
    // }, () => {
    //   this.isTableLoading = false
    // })
  }
}
export class BankTransactionDto{
  id: number;
  fee: number;
  fromBankAccountId: number;
  fromValue: number;
  toValue: number;
  lockedStatus: boolean;
  name: string;
  note: string;
  toBankAccountId: number;
  transactionDate: Date;
}
