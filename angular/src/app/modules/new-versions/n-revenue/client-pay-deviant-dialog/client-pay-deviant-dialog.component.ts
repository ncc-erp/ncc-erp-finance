import { AppConfigurationService } from '@app/service/api/app-configuration.service';
import { AppComponentBase } from '@shared/app-component-base';
import { Component, Inject, Injector, OnInit } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { NRevenueService } from '@app/service/api/new-versions/n-revenue.service';
import { catchError, map, startWith } from 'rxjs/operators';
import { HttpErrorResponse } from '@angular/common/http';
import { of } from 'rxjs';

@Component({
  selector: 'app-client-pay-deviant-dialog',
  templateUrl: './client-pay-deviant-dialog.component.html',
  styleUrls: ['./client-pay-deviant-dialog.component.css']
})
export class ClientPayDeviantDialogComponent extends AppComponentBase implements OnInit {
  incomingEntryId: number;
  accountName: string;
  title: string;
  isSaving: boolean;
  deviantCode: string;
  model: IncomingClientPayDeviant;

  constructor(injector: Injector,
    public dialogRef: MatDialogRef<ClientPayDeviantDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: DataClientPayDeviantDialog,
    private _invoice: NRevenueService,
    private _config: AppConfigurationService
  ) {
    super(injector);
    Object.assign(this, this.data);
  }

  ngOnInit(): void {
    this.setDeviantCode();
    this.setModel();
  }

  private setDeviantCode(): void {
    this._config.getDeviantCode().subscribe(res => {
      if (!res.success) return;
      this.deviantCode = res.result;
    });
  }

  private setModel(): void {
    this.model = new IncomingClientPayDeviant(this.incomingEntryId,this.accountName + " trả kênh tiền");
  }

  saveAndClose(): void {
    this._invoice.setClientPayDeviant(this.model)
      .pipe(map(data => ({ ...data, loading: false })),
        startWith({ loading: true, success: false }),
        catchError((err: HttpErrorResponse) => {
          return of({ loading: false, success: false, error: err });
        }))
      .subscribe(res => {
        this.isSaving = res.loading;
        if(!res.success) return;
        abp.notify.success("Cập nhật thành công!");
        this.dialogRef.close(true);
      });
  }
}

export class DataClientPayDeviantDialog {
  incomingEntryId: number;
  accountName: string;
  title: string;
}
export class IncomingClientPayDeviant {
  constructor(_incomingEntryId: number,_incomingName: string) {
    this.incomingEntryId = _incomingEntryId;
    this.incomingName = _incomingName;
  }
  incomingEntryId: number;
  incomingName: string;
}
