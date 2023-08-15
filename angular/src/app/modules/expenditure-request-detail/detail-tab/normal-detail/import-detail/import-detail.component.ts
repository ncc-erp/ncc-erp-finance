import { catchError } from 'rxjs/operators';
import { Component, Inject, OnInit } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { RequestDetailService } from '@app/service/api/request-detail.service';

@Component({
  selector: 'app-import-detail',
  templateUrl: './import-detail.component.html',
  styleUrls: ['./import-detail.component.css']
})
export class ImportDetailComponent implements OnInit {

  selectedFiles: FileList;
  currentFileUpload: File;
  public uploadFile = {} as ImportDetailDto;
  public isDisable = false;
  public TimesheetProjectId: any;
  constructor(private dialogRef: MatDialogRef<ImportDetailComponent>,
    private requestDetailService: RequestDetailService,
    @Inject(MAT_DIALOG_DATA) public data: any,
  ) { }


  ngOnInit(): void {
    this.uploadFile.outcomingEntryId = this.data.id;

  }
  selectFile(event) {
    this.selectedFiles = event.target.files.item(0);
  }


  importExcel() {
    if (!this.selectedFiles) {
      abp.message.error("Choose a file!")
      return
    }
    this.requestDetailService.importFileOutcomingEntryDetail(this.selectedFiles, this.uploadFile.outcomingEntryId)
      .pipe(catchError(this.requestDetailService.handleError)).subscribe((res) => {
        if(!!res.body){
          let message = `success <strong class='text-success'>${res?.body.result?.success}</strong>, fail <strong class='text-danger'>${res?.body.result?.fail}</strong>`

          abp.message.info(message, "import result", {isHTML:true})
          this.dialogRef.close(res);
        }
      }, () => this.isDisable = false);
  }
}

export interface ImportDetailDto {
  outcomingEntryId: number,
  file: any
}
