import { Router } from '@angular/router';
import { Component, EventEmitter, Input, OnInit, Output, ViewEncapsulation } from '@angular/core';

@Component({
  selector: 'app-bread-crumb',
  templateUrl: './bread-crumb.component.html',
  styleUrls: ['./bread-crumb.component.css'],
  encapsulation: ViewEncapsulation.None,
})
export class BreadCrumbComponent implements OnInit {
  @Output() navigate = new EventEmitter();
  @Input() listBreadCrumb = [];
  @Input() itemName: string;
  constructor(private router: Router) { }

  ngOnInit(): void {
  }

  public onNavigate(breadCrumb: BreadCrumbDto){
    this.router.navigate([breadCrumb.url],{
      queryParams: {
        id : breadCrumb.queryParams
      }
    })
  }
}
export class BreadCrumbDto{
  name: string;
  url?: string;
  queryParams?: Object
}
