import { Component, Inject, Injector, OnInit } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { AppConfigurationService } from '@app/service/api/app-configuration.service';
import { AppComponentBase } from '@shared/app-component-base';
import { ListBtransactionComponent } from '../list-btransaction/list-btransaction.component';

@Component({
  selector: 'app-setting-payment-dialog',
  templateUrl: './setting-payment-dialog.component.html',
  styleUrls: ['./setting-payment-dialog.component.css']
})

export class SettingPaymentDialogComponent extends AppComponentBase implements OnInit {
  title: string = 'Setting khách hàng thanh toán';
  isSaving: boolean = false;
  public especialIncomingEntryType: EspecialIncomingEntryTypeDto;
  
  constructor(injector: Injector,
    public dialogRef: MatDialogRef<ListBtransactionComponent>,
    private _settingService: AppConfigurationService,
    @Inject(MAT_DIALOG_DATA) public data: any,

  ) {
    super(injector);
  }
  ngOnInit(): void {
    this.getEspecialIncomingEntryType();
  }

  getEspecialIncomingEntryType() {
    this._settingService.getEspecialIncomingEntryType()
      .subscribe(response => {
        if (!response.success) return;
        this.especialIncomingEntryType = response.result;
      });
  }

  setCssClassName(IncomingEntryTypeStatus: string) {
    if (IncomingEntryTypeStatus == "Active")
      return "active"
    if (IncomingEntryTypeStatus == "Inactive")
      return "inactive"
    if (IncomingEntryTypeStatus == "Not found")
      return "error"
    else
      return ""
  }

  saveAndClose() {
    this.isSaving = true
    this._settingService.setEspecialIncomingEntryType(this.especialIncomingEntryType).subscribe(response => {
      abp.notify.success("Updated successfully");
      this.especialIncomingEntryType = response.result;
      if (response.result.isAllActive == true) {        
        this.dialogRef.close();
      }     
      this.isSaving = false
    }, err =>  this.isSaving = false)
  }
}

export interface EspecialIncomingEntryTypeDto {
  debtIncomingEntryTypeCode: string;
  debtIncomingEntryTypeStatus: string;
  balanceIncomingEntryTypeCode: string;
  balanceIncomingEntryTypeStatus: string;
  deviantIncomingEntryTypeCode: string;
  deviantIncomingEntryTypeStatus: string;
  isAllActive: boolean;
}