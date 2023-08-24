import { AppComponentBase } from '@shared/app-component-base';
import { ValueAndNameModel } from '@app/service/model/common-DTO';
import { Component, OnInit, Injector } from '@angular/core';
import { MatDialogRef } from '@angular/material/dialog';
import { CommonService } from '@app/service/api/new-versions/common.service';
import { BtransactionService } from '@app/service/api/new-versions/btransaction.service';

@Component({
  selector: 'app-import-file',
  templateUrl: './import-file.component.html',
  styleUrls: ['./import-file.component.css']
})
export class ImportFileComponent extends AppComponentBase implements OnInit {
  bankAccountOptions: ValueAndNameModel[] = [];
  selectedBankAccountId: number;
  selectedFile: File;
  isSaving: boolean;
  constructor(
    injector: Injector,
    private dialogRef: MatDialogRef<ImportFileComponent>,
    private _common: CommonService,
    private _bTransaction: BtransactionService
  ) {
    super(injector);
  }

  ngOnInit(): void {
    this.setBankAccountOptions();
  }

  selectFile(event) {
    this.selectedFile = event.target.files.item(0);
  }

  setBankAccountOptions(): void {
    this._common.gettAllBankAccount().subscribe(response => {
      if (!response.success) return;

      this.bankAccountOptions = response.result;
    });
  }
  importExcel() {
    if (!this.selectedFile) {
      abp.message.error("Choose a file!")
      return
    }
    if (!this.selectedBankAccountId) {
      abp.message.error("Choose a Bank Account!")
      return
    }
    const formData = new FormData();
    formData.append('bankAccountId', this.selectedBankAccountId.toString());
    formData.append('bTransactionFile', this.selectedFile);

    this.isSaving = true;

    this._bTransaction.importBTransaction(formData)
      .subscribe(response => {
        if (response.success) {
          this.dialogRef.close(response.result);
          return;
        }
        this.dialogRef.close();
      }, () => this.isSaving = false);
  }
}