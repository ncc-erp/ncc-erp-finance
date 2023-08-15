import { AppComponentBase } from '@shared/app-component-base';
import { ActivatedRoute, Router } from '@angular/router';
import { Component, Injector, OnInit } from '@angular/core';

@Component({
  selector: 'app-invoice-detail',
  templateUrl: './invoice-detail.component.html',
  styleUrls: ['./invoice-detail.component.css']
})
export class InvoiceDetailComponent extends AppComponentBase implements OnInit {
  invoiceId: any
  currentUrl: string = ""
  constructor(private route: ActivatedRoute, private router: Router, injector:Injector) {
    super(injector)
  }
  ngOnInit(): void {
    this.invoiceId = this.route.snapshot.queryParamMap.get("id")
    this.router.navigate(['invoice-general'], {
      relativeTo: this.route, queryParams: {
        id: this.invoiceId
      },
      replaceUrl: true
    })
    this.router.events.subscribe(res => this.currentUrl = this.router.url)
  }
  routingMainTab() {
    this.router.navigate(['invoice-general'], {
      relativeTo: this.route, queryParams: {
        id: this.invoiceId
      },
    })
  }
  routingProjectTimesheet() {
    this.router.navigate(['project-timesheet'], {
      relativeTo: this.route, queryParams: {
        id: this.invoiceId
      },
    })
  }
  routingTransaction() {
    this.router.navigate(['transaction-tab'], {
      relativeTo: this.route, queryParams: {
        id: this.invoiceId
      },
    })
  }
}
