import { BranchDto } from './../../modules/branch/branch.component';
import { BranchService } from './../../service/api/branch.service';
import { MatDialogRef } from '@angular/material/dialog';
import { DashBoardService } from './../../service/api/dash-board.service';
import { AppComponentBase } from 'shared/app-component-base';
import { Component, Injector, OnInit } from '@angular/core';
import * as FileSaver from 'file-saver';
import * as moment from 'moment';
import { AppConsts } from '@shared/AppConsts';

@Component({
  selector: 'app-export-report',
  templateUrl: './export-report.component.html',
  styleUrls: ['./export-report.component.css']
})
export class ExportReportComponent extends AppComponentBase implements OnInit {
  public today = new Date();
  public fromDate
  public toDate

  constructor(injector: Injector, private dashboardService: DashBoardService,
    public dialogRef: MatDialogRef<ExportReportComponent>) {
    super(injector)
  }

  ngOnInit(): void {
    this.fromDate = new Date(this.today.getFullYear(), this.today.getMonth() - 1, 1);
    this.toDate = new Date(this.today.getFullYear(), this.today.getMonth(), 0);
  }

  downloadFile() {
    this.fromDate = moment(this.fromDate).format("YYYY-MM-DD")
    this.toDate = moment(this.toDate).format("YYYY-MM-DD")
    this.dashboardService.exportStatisticDashboard(AppConsts.periodId.value, this.fromDate, this.toDate).subscribe(data => {
      const file = new Blob([this.convertFile(atob(data.result))], {
        type: "application/vnd.ms-excel;charset=utf-8"
      });
      FileSaver.saveAs(file, `Bao_Cao_Thu_Chi.xlsx`);
      abp.notify.success("export successful")
      this.dialogRef.close()
    })
  }
  convertFile(fileData) {
    var buf = new ArrayBuffer(fileData.length);
    var view = new Uint8Array(buf);
    for (var i = 0; i != fileData.length; ++i) view[i] = fileData.charCodeAt(i) & 0xFF;
    return buf;
  }
}
