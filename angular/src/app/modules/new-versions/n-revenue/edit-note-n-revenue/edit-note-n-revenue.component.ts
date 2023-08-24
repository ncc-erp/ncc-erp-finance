import { Component, Inject, Injector, OnInit } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { NRevenueService } from '@app/service/api/new-versions/n-revenue.service';
import { UpdateNoteDto } from '@app/service/model/n-revenue.model';
import { AppComponentBase } from '@shared/app-component-base';
import { ListNRevenueComponent } from '../list-n-revenue/list-n-revenue.component';

@Component({
  selector: 'app-edit-note-n-revenue',
  templateUrl: './edit-note-n-revenue.component.html',
  styleUrls: ['./edit-note-n-revenue.component.css']
})
export class EditNoteNRevenueComponent extends AppComponentBase implements OnInit {
  public isDisable = false;
  revenue = {} as UpdateNoteDto
  public title: string = 'Update note invoice';
  constructor(injector: Injector, public dialogRef: MatDialogRef<ListNRevenueComponent>,
    @Inject(MAT_DIALOG_DATA) public data: any,
    private _revenue: NRevenueService) {
    super(injector);
    this.revenue = this.data.item;
  }
  ngOnInit(): void {
  }
  saveAndClose() {
    this.isDisable = true;
    this._revenue.updateNote(this.revenue).subscribe(rs => {
      if (rs.success) {
        abp.notify.success("Updated note successfully");
        this.dialogRef.close(this.revenue);
      }
    }, () => { this.isDisable = false })
  }
}
