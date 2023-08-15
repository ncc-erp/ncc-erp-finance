import { AppComponentBase } from '@shared/app-component-base';
import { RevenueManagedDto } from './../revenue-managed.component';
import { RevenueManagedService } from './../../../service/api/revenue-managed.service';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { Component, OnInit, Inject, Injector } from '@angular/core';

@Component({
  selector: 'app-upload-file-dialog',
  templateUrl: './upload-file-dialog.component.html',
  styleUrls: ['./upload-file-dialog.component.css']
})
export class UploadFileDialogComponent extends AppComponentBase implements OnInit {
  listFiles: File[] = [];
  revuenue = new RevenueManagedDto();
  revenueManagedId: number = 0
  constructor(injector: Injector, public dialogRef: MatDialogRef<UploadFileDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: any, private _service: RevenueManagedService) {
    super(injector)
  }
  ngOnInit(): void {
      this.revuenue = this.data
      // this.revenueManagedId = this.revuenue.id
      this._service.revenueGetFiles(this.revuenue.id).subscribe((res: any) => {
        if (res.success) {
          this.listFiles = res.result.map((item) => {
            let file = new Blob([this.s2ab(atob(item.bytes))], {
              type: ""
            });
            let arrayOfBlob = new Array<Blob>();
            arrayOfBlob.push(file);
            item = new File(arrayOfBlob, item.fileName)
            return item;
          })
        }
      })
      // this.selectedAccountType = this.data.item.accountTypeCode    

  }
  s2ab(s) {
    var buf = new ArrayBuffer(s.length);
    var view = new Uint8Array(buf);
    for (var i = 0; i != s.length; ++i) view[i] = s.charCodeAt(i) & 0xFF;
    return buf;
  }

  saveAndClose(){
    this._service.revenueUploadFiles(this.listFiles,  this.revuenue.id).subscribe((rs) => {
      if (rs.success) {
        this.dialogRef.close(this.revuenue);
        abp.notify.success(rs.result)
      }
    })
  }
  removeFile(file: File) {
    let index = this.listFiles.indexOf(file);
    this.listFiles.splice(index, 1)
  }
  uploadFiles(event) {
    this.listFiles.push(...event.target.files)
  }
  refreshFiles() {
    this.listFiles.splice(0, this.listFiles.length)
  }

}
