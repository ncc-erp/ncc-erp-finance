import { Component,
  Injector,
  OnInit,
  Output,
  EventEmitter } from '@angular/core';
import { AppComponentBase } from '@shared/app-component-base';
import { BsModalRef } from 'ngx-bootstrap/modal';

@Component({
  selector: 'app-view-review-explain',
  templateUrl: './view-review-explain.component.html',
  styleUrls: ['./view-review-explain.component.css'],
})
export class ViewReviewExplainComponent extends AppComponentBase implements OnInit {

  saving = false;
  public reviewExplain: any;

  constructor(
    injector: Injector,
    public bsModalRef: BsModalRef,
  ) {
    super(injector);
    
  }

  ngOnInit(): void {
  }

}
