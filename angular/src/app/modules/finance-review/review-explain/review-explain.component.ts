import { Component,
  Injector,
  OnInit,
  Output,
  EventEmitter } from '@angular/core';
import { AppComponentBase } from '@shared/app-component-base';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { DashBoardService } from '../../../service/api/dash-board.service';

@Component({
  selector: 'app-review-explain',
  templateUrl: './review-explain.component.html',
  styleUrls: ['./review-explain.component.css'],
})
export class ReviewExplainComponent extends AppComponentBase implements OnInit {

  saving = false;
  public comparativeData: any;
  notes: any = {
    incomingDiffVNDNote: '',
    outcomingDiffVNDNote: '',
    incomingDiffUSDNote: '',
    outcomingDiffUSDNote: ''
  };

  @Output() onSave = new EventEmitter<any>();

  constructor(
    injector: Injector,
    public bsModalRef: BsModalRef,
    private dashBoardService: DashBoardService
  ) {
    super(injector);
    
  }

  ngOnInit(): void {
  }

  save(): void {
    this.saving = true;
    this.dashBoardService.SendReviewExplain({
      incomingVND: this.comparativeData.totalVNDIn,
      incomingUSD: this.comparativeData.totalUSDIn,
      outcomingVND: this.comparativeData.totalVNDOut,
      outcomingUSD: this.comparativeData.totalUSDOut,
      incomingVNDTransaction: this.comparativeData.totalVNDInTransaction,
      incomingUSDTransaction: this.comparativeData.totalUSDInTransaction,
      outcomingVNDTransaction: this.comparativeData.totalVNDOutTransaction,
      outcomingUSDTransaction: this.comparativeData.totalUSDOutTransaction,
      incomingDiffVND: this.comparativeData.totalVNDIn - this.comparativeData.totalVNDInTransaction,
      incomingDiffUSD: this.comparativeData.totalUSDIn - this.comparativeData.totalUSDInTransaction,
      outcomingDiffVND: this.comparativeData.totalVNDOut - this.comparativeData.totalVNDOutTransaction,
      outcomingDiffUSD: this.comparativeData.totalUSDOut - this.comparativeData.totalUSDOutTransaction,
      incomingDiffVNDNote: this.notes.incomingDiffVNDNote,
      incomingDiffUSDNote: this.notes.incomingDiffUSDNote,
      outcomingDiffVNDNote: this.notes.outcomingDiffVNDNote,
      outcomingDiffUSDNote: this.notes.outcomingDiffUSDNote,
    })
    .subscribe(() => {
      this.notify.info(this.l('Update Successful'));
      this.bsModalRef.hide();
      this.onSave.emit();
    },()=>this.saving=false);
  }

}
