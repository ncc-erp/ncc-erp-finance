import { Component, Inject, OnInit } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { Router } from '@angular/router';
import { StatusService } from '@app/service/api/status.service';
import { catchError } from 'rxjs/operators';
import { StatusDto } from '../status.component';

@Component({
  selector: 'app-create-edit-dialog-status',
  templateUrl: './create-edit-dialog-status.component.html',
  styleUrls: ['./create-edit-dialog-status.component.css']
})
export class CreateEditDialogStatusComponent implements OnInit {

  status = {} as StatusDto;
  isDisable = false;
  isEdit: boolean;
  constructor(private _statusService: StatusService,
    public dialogRef: MatDialogRef<CreateEditDialogStatusComponent>,
    private router: Router,
    @Inject(MAT_DIALOG_DATA) public data: any) { }

  ngOnInit(): void {
    this.status = this.data.item;
    this.isEdit = this.data.item.id === undefined ? false : true;
  }
  saveAndClose() {
    this.isDisable = true
    if (this.data.command == "create") {
      this._statusService.create(this.status).pipe(catchError(this._statusService.handleError)).subscribe((res) => {
        abp.notify.success("created status ");
        this.reloadComponent()
        this.dialogRef.close();
      }, () => this.isDisable = false);
    }
    else {
      this._statusService.update(this.status).pipe(catchError(this._statusService.handleError)).subscribe((res) => {
        abp.notify.success("edited status ");
        this.reloadComponent()
        this.dialogRef.close();
      }, () => this.isDisable = false);
    }
  }
  reloadComponent() {
    this.router.navigateByUrl('', { skipLocationChange: true }).then(() => {
      this.router.navigate(['/app/status']);
    });
  }

}
