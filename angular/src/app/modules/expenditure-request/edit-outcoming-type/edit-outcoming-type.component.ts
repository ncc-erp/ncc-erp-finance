import { Component, ElementRef, Inject, OnInit, ViewChild } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { ExpenditureRequestService } from '@app/service/api/expenditure-request.service';
import { ExpenditureService } from '@app/service/api/expenditure.service';
import { CommonService } from '@app/service/api/new-versions/common.service';
import { OutcomingEntryDto, TreeOutcomingEntries } from '../expenditure-request.component';

@Component({
  selector: 'app-edit-outcoming-type',
  templateUrl: './edit-outcoming-type.component.html',
  styleUrls: ['./edit-outcoming-type.component.css']
})
export class EditOutcomingTypeComponent implements OnInit {

  @ViewChild("inputSearchOutcoming") inputSearchOutcoming: ElementRef;
  public isSaving:boolean = false
  treeOutcomingEntries: TreeOutcomingEntries[] = [];
  tmpTreeOutcomingEntries: TreeOutcomingEntries[] = [];
  searchOutcoming:string = ""
  outcomingEntry = {} as OutcomingEntryDto
  constructor(private commonService:CommonService,
    private outcomeService:ExpenditureRequestService,
    private outcomingEntryService: ExpenditureService,
    private dialogRef:MatDialogRef<EditOutcomingTypeComponent>,
     @Inject(MAT_DIALOG_DATA) public data:OutcomingEntryDto) { }

  ngOnInit(): void {
    this.getTreeOutcomingEntries()
    this.outcomingEntry = this.data
  }

  onSave(){
    this.isSaving = true
    this.outcomeService.UpdateOutcomingEntryType(this.outcomingEntry.id, this.outcomingEntry.outcomingEntryTypeId)
    .subscribe(rs => {
      abp.notify.success("Updated outcomingType")
      this.dialogRef.close(true)
      this.isSaving = false
    },
    () => this.isSaving = false)
  }

  filterData(data: TreeOutcomingEntries, level: number) {
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
      this.inputSearchOutcoming.nativeElement.focus();
      return;
    }
    this.treeOutcomingEntries = this.tmpTreeOutcomingEntries;
  }

  getTreeOutcomingEntries(){
    this.outcomingEntryService.getAllForDropdownByUserNew().subscribe((data) => {
      data.result.forEach((item) =>
        this.filterData(item as TreeOutcomingEntries, 1)
      );

      this.tmpTreeOutcomingEntries = [...this.treeOutcomingEntries];
    });
  }
}
