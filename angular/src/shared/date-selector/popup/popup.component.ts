import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { Component, OnInit, Optional, Inject, NgZone } from '@angular/core';
import * as moment from 'moment';

@Component({
  selector: 'app-popup',
  templateUrl: './popup.component.html',
  styleUrls: ['./popup.component.css']
})
export class PopupComponent implements OnInit {

  formPopup: FormGroup
  
  constructor(
    private fb: FormBuilder,
    private _dialogRef: MatDialogRef<PopupComponent>,
    private ngZone: NgZone,
    @Optional() @Inject(MAT_DIALOG_DATA) private data: any
  ) { }

  ngOnInit() {
    this.formPopup = this.fb.group({
      fromDateCustomTime: [moment(), Validators.required],
      toDateCustomTime: [moment(), Validators.required]
    })
  }
  
  submit() {
    if(!this.formPopup.valid) {
      return;
    }
    this._dialogRef.close({result: true,data: {fromDateCustomTime: moment(this.formPopup.value.fromDateCustomTime), toDateCustomTime: moment(this.formPopup.value.toDateCustomTime) }});
  }

  handleChangeFromDate() {
    this.formPopup.patchValue({
      toDateCustomTime: this.formPopup.controls['fromDateCustomTime'].value
    })
  }

  close(result: any, data?: any): void {
    this._dialogRef.close(false);
  }
}
