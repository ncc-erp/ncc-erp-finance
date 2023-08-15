import { Component, Inject, Injector, OnInit } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { BankService } from '@app/service/api/bank.service';
import { AppComponentBase } from '@shared/app-component-base';
import { BankDto } from '../bank.component';

@Component({
  selector: 'app-create-edit-bank',
  templateUrl: './create-edit-bank.component.html',
  styleUrls: ['./create-edit-bank.component.css']
})
export class CreateEditBankComponent extends AppComponentBase implements OnInit {
  bank= {} as BankDto
  isDisable = false;
  constructor(
    private _bankService: BankService,
    public dialogRef: MatDialogRef<CreateEditBankComponent>,
    @Inject(MAT_DIALOG_DATA) public data: any,
    injector: Injector,
  ) { super(injector); }
  isEdit: boolean = false;

  ngOnInit(): void {

    if(this.data.command =="edit"){
      this.bank = this.data.item;
      this.isEdit = true;
    }
  }
  saveAndClose() {
    this.isDisable = true
    if (this.data.command == "create") {
      this._bankService.create(this.bank).subscribe(res => {
        this.notify.success(this.l('Create bank successfully'));
        this.dialogRef.close()
      }, () => this.isDisable = false);
    }
    else {
      this._bankService.update(this.bank).subscribe(res => {
        this.notify.success(this.l('Update bank successfully'));
        this.dialogRef.close()
      }, () => {
        this.isDisable = false
      });

    }
  }
}
