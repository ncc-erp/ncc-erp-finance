import { Component, ElementRef, Inject, OnInit, ViewChild } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { IncomingEntryDto, TreeIncomingEntries } from '@app/modules/revenue-recording/revenue-recording.component';
import { CommonService } from '@app/service/api/new-versions/common.service';
import { RevenueRecordService } from '@app/service/api/revenue-record.service';
import { UpdateIncomeTypeDto } from '@app/service/model/n-revenue.model';

@Component({
  selector: 'app-update-imcome-type',
  templateUrl: './update-imcome-type.component.html',
  styleUrls: ['./update-imcome-type.component.css']
})
export class UpdateImcomeTypeComponent implements OnInit {

  @ViewChild("inputSearchIncoming") inputSearchIncoming: ElementRef;
  public isSaving: boolean = false
  treeOutcomingEntries: TreeIncomingEntries[] = [];
  tmpTreeOutcomingEntries: TreeIncomingEntries[] = [];
  searchOutcoming: string = ""
  incomingEntry = {} as IncomingEntryDto

  constructor(private commonService: CommonService,
    private incomeService: RevenueRecordService,
    private dialogRef: MatDialogRef<UpdateImcomeTypeComponent>,
    @Inject(MAT_DIALOG_DATA) public data: IncomingEntryDto) { }

  ngOnInit(): void {
    this.getTreeOutcomingEntries()
    this.incomingEntry = this.data
  }

  onSave() {
    let dto = {
      id: this.incomingEntry.id,
      incomeTypeId: this.incomingEntry.incomingEntryTypeId
    } as UpdateIncomeTypeDto
    this.isSaving = true
    this.incomeService.updateIcomeType(dto)
      .subscribe(rs => {
        abp.notify.success("Updated incomingEntryType")
        this.dialogRef.close(true)
        this.isSaving = false
      },
        () => this.isSaving = false)
  }

  filterData(data: TreeIncomingEntries, level: number) {
    data.paddingLevel = "";
    for (let i = 1; i < level; i++) {
      data.paddingLevel += "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;";
    }
    this.treeOutcomingEntries.push(data);
    if (data.children.length > 0) {
      data.children.forEach((item) => {
        this.filterData(item, level + 1);
      });
    }
  }


  selectionOutcomingOpenChange(isOpen: boolean) {
    if (isOpen) {
      this.inputSearchIncoming.nativeElement.focus();
      return;
    }
    this.treeOutcomingEntries = this.tmpTreeOutcomingEntries;
  }

  getTreeOutcomingEntries() {
    this.commonService.getTreeIncomingEntries(true).subscribe((data) => {
      data.result.forEach((item) =>
        this.filterData(item as TreeIncomingEntries, 1)
      );

      this.tmpTreeOutcomingEntries = [...this.treeOutcomingEntries];
    });
  }
}
