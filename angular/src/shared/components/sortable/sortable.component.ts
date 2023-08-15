import { Component, Input, OnInit } from '@angular/core';
import { SortDirectionEnum } from '@shared/AppEnums';




@Component({
  selector: 'sortable',
  templateUrl: './sortable.component.html',
  styleUrls: ['./sortable.component.css']
})
export class SortableComponent {
  @Input() sortProperty: string = "";
  @Input() sortDirection: number
  @Input() name: string = ""

  sortDirectionEnum = {
    Ascending: 0,
    Descending: 1
  }


}

