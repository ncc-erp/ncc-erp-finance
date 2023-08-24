import { ComparisionDto, COMPARISIONS } from './../filter/filter.component';
import { InputFilterDto } from '@shared/filter/filter.component';
import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';

@Component({
  selector: 'app-table-filter',
  templateUrl: './table-filter.component.html',
  styleUrls: ['./table-filter.component.css']
})
export class TableFilterComponent implements OnInit {

  @Input() inputFilters: InputFilterDto[];
  @Input() item: any;
  @Output() emitChange = new EventEmitter<any>();
  @Output() deleteDataFilter = new EventEmitter<any>();
  selectedPropertyName: string;
  selectedComparision: number;
  value: any;

  comparisions: ComparisionDto[] = [];
  constructor() { }
  ngOnInit(): void {
    if (this.item.propertyName === '') {
      this.comparisions = [];
    }
    else {
      let comps = this.inputFilters.find(i => i.propertyName === this.item.propertyName).comparisions;
      comps.forEach(element => {
        var com = new ComparisionDto();
        com.id = element;
        com.name = COMPARISIONS[element];
        this.comparisions.push(com);
      });
    }
  }

  onChange(value: string | number, name: string): void {
    if (name === 'propertyName') {
      this.emitChange.emit({ name: 'comparision', value: undefined })
      if (value == '') {
        this.comparisions = [];
        return;
      }
      var comps = this.inputFilters.find(i => i.propertyName === value).comparisions;
      this.comparisions = [];
      comps.forEach(element => {
        var com = new ComparisionDto();
        com.id = element;
        com.name = COMPARISIONS[element];
        this.comparisions.push(com);
      });
    }
    this.emitChange.emit({ name, value })
  }

  deleteFilter() {
    this.deleteDataFilter.emit();
  }

}
