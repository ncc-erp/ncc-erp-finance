import { Component, Inject, OnInit } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { ImportBTransactionResult } from '@app/service/model/b-transaction.model';
import { ImportFileComponent } from '../import-file/import-file.component';

@Component({
  selector: 'app-dialog-result-import-file',
  templateUrl: './dialog-result-import-file.component.html',
  styleUrls: ['./dialog-result-import-file.component.css']
})
export class DialogResultImportFileComponent implements OnInit {

  constructor(
    private dialogRef: MatDialogRef<ImportFileComponent>,
    @Inject(MAT_DIALOG_DATA) public importBTransactionResult: ImportBTransactionResult
  ) { }

  ngOnInit(): void {
  }
  close(){
    this.dialogRef.close();
  }
}
