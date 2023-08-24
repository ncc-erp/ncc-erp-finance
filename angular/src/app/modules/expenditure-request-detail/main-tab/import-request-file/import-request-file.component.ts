import { ExpenditureRequestService } from '@app/service/api/expenditure-request.service';
import { Router } from '@angular/router';
import { catchError } from 'rxjs/operators';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { Component, OnInit, Inject } from '@angular/core';

@Component({
  selector: 'app-import-request-file',
  templateUrl: './import-request-file.component.html',
  styleUrls: ['./import-request-file.component.css']
})
export class ImportRequestFileComponent implements OnInit {

  selectedFiles: FileList;
  currentFileUpload: File;
  public uploadFile= {} as UploadFileDto;
  public isDisable = false;
  public TimesheetProjectId:any;
  constructor(private dialogRef: MatDialogRef<ImportRequestFileComponent> ,
    private requestService: ExpenditureRequestService,
    @Inject(MAT_DIALOG_DATA) public data: any,
  private router: Router
  ) { }


  ngOnInit(): void {
    this.uploadFile.OutcomingEntryId= this.data.id;
    
  }
  selectFile(event) {
    this.selectedFiles = event.target.files.item(0);
    // this.currentFileUpload = this.selectedFiles.item(0);
    //   this.selectedFiles = event.target.files;
    // this.currentFileUpload = this.selectedFiles.item(0);
  }
  

  importExcel() {
    if (!this.selectedFiles) {
      abp.message.error("Choose a file!")
      return
    }
    
    // formData.append('TimesheetProjectId', this.uploadFile.TimesheetProjectId.toString());
    // formData.append('file', this.currentFileUpload);
    // this.uploadFile.File= formData;
  
    this.requestService.UploadFile(this.selectedFiles, this.uploadFile.OutcomingEntryId )
    .pipe(catchError(this.requestService.handleError)).subscribe((res) => {
      abp.notify.success("Upload File Successful!");
      this.dialogRef.close(res);
    }, () => this.isDisable = false);
  }

}

export class UploadFileDto{
  OutcomingEntryId: number;
  file:any;
  
}