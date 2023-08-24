import { InvoiceService } from './../../../../service/api/invoice.service';
import { AppComponentBase } from '@shared/app-component-base';
import { Component, Injector, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import * as FileSaver from 'file-saver';

@Component({
  selector: 'app-project-timesheet',
  templateUrl: './project-timesheet.component.html',
  styleUrls: ['./project-timesheet.component.css']
})
export class ProjectTimesheetComponent extends AppComponentBase implements OnInit {
  projectTimeSheetList: ProjectTimeSheetDto[] = []
  invoiceId: string;
  isTableLoading: boolean = false
  constructor(injector: Injector, private invoiceService: InvoiceService, private route: ActivatedRoute) {
    super(injector)
    this.invoiceId = route.snapshot.queryParamMap.get("id");
  }

  ngOnInit(): void {
    this.getProjectTimeSheet();
  }
  getProjectTimeSheet() {
    this.isTableLoading = true
    // this.invoiceService.getProjectTimeSheet(this.invoiceId).subscribe(data => {
    //   this.projectTimeSheetList = data.result
    //   this.isTableLoading = false
    // }, () => {
    //   this.isTableLoading = false
    // })
  }
  downloadFile(id: any) {
    // this.invoiceService.DownloadFileTimesheetProject(id).subscribe(data => {
    //   const file = new Blob([this.s2ab(atob(data.result.data))], {
    //     type: "application/vnd.ms-excel;charset=utf-8"
    //   });
    //   FileSaver.saveAs(file, data.result.fileName);
    // })

  }
  s2ab(s) {
    var buf = new ArrayBuffer(s.length);
    var view = new Uint8Array(buf);
    for (var i = 0; i != s.length; ++i) view[i] = s.charCodeAt(i) & 0xFF;
    return buf;
  }
}
export class ProjectTimeSheetDto {
  invoiceId: number
  projectName: string
  fileName: any
  id: number
}
