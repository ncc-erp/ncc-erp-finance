import { Component, Inject, OnInit } from '@angular/core';
import { MatDialog, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { ExpenditureRequestService } from '@app/service/api/expenditure-request.service';
import * as moment from 'moment';
import { OutcomingEntryDto } from '../expenditure-request.component';

@Component({
  selector: 'app-edit-report-date',
  templateUrl: './edit-report-date.component.html',
  styleUrls: ['./edit-report-date.component.css']
})
export class EditReportDateComponent implements OnInit {
  public isSaving: boolean = false
  public reportDate:string;
  constructor(public dialogRef: MatDialogRef<EditReportDateComponent>,
    public dialog: MatDialog,
    @Inject(MAT_DIALOG_DATA) public data: OutcomingEntryDto,
    public readonly service: ExpenditureRequestService
  ) {
  }

  ngOnInit(): void {
    if(this.data.reportDate){
      this.reportDate = moment(this.data.reportDate).format("YYYY-MM-DD");
    }else{
      this.reportDate = moment(new Date()).format("YYYY-MM-DD");
    }
  }

  handleSave() {
    this.isSaving = true
    this.service.updateReportDate(this.data.id, moment(this.reportDate).format('YYYY-MM-DD')).subscribe(result => {
      abp.notify.success('Update report date successfully')
      this.dialogRef.close(true)
      this.isSaving = false
    },
    () => this.isSaving = false)
  }

}
