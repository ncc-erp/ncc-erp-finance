import { MatDialogRef } from '@angular/material/dialog';
import { HrmDebtDto } from './../home.component';
import { Component, Inject, OnInit } from '@angular/core';
import { MAT_DIALOG_DATA } from '@angular/material/dialog';

@Component({
  selector: 'app-detail-nhanvien-no',
  templateUrl: './detail-nhanvien-no.component.html',
  styleUrls: ['./detail-nhanvien-no.component.css']
})
export class DetailNhanvienNoComponent implements OnInit {

  public hrmDebt = {} as HrmDebtDto

  constructor(@Inject(MAT_DIALOG_DATA) public data,
    private dialogRef: MatDialogRef<DetailNhanvienNoComponent>) {
    this.hrmDebt = this.data
  }

  ngOnInit(): void {
  }
  onclose() {
    this.dialogRef.close()
  }
}
