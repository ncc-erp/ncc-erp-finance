import { Component, EventEmitter, Input, OnInit, Output, ViewEncapsulation } from '@angular/core';
import { Router } from '@angular/router';

@Component({
  selector: 'app-bread-crumb',
  templateUrl: './bread-crumb.component.html',
  styleUrls: ['./bread-crumb.component.css'],
  encapsulation: ViewEncapsulation.None,
})
export class BreadCrumbComponent implements OnInit {
  @Output() navigate = new EventEmitter();
  @Output() refreshCurrentPage = new EventEmitter();

  @Input() listBreadCrumb: BreadCrumbDto[] = [];
  @Input() itemName: string;

  constructor(private router: Router) {}

  ngOnInit(): void {}

  onRefreshCurrentPage() {
    this.refreshCurrentPage.emit();
  }

  onNavigate(breadCrumb: BreadCrumbDto) {
    this.router.navigate([breadCrumb.url], {
      queryParams: breadCrumb.queryParams,
    });
  }
}

export class BreadCrumbDto {
  name: string;
  url?: string;
  queryParams?: any;
}
