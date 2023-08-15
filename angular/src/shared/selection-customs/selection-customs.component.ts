import { Component, ElementRef, EventEmitter, Injector, Input, OnInit, Output, ViewChild } from '@angular/core';
import { ValueAndNameModel } from '@app/service/model/common-DTO';
import { AppComponentBase } from '@shared/app-component-base';

@Component({
  selector: 'app-selection-customs',
  templateUrl: './selection-customs.component.html',
  styleUrls: ['./selection-customs.component.css']
})
export class SelectionCustomsComponent extends AppComponentBase implements OnInit {

  // Có search hay không
  @Input() searchable = true;
  // To do
  @Input() isHtml = false;
  // class của div chứa label
  @Input() labelClass = "";
  // Tooltip của label
  @Input() labelTooltip = "";
  // class của div chứa label
  @Input() selectionClass = "col";
  @Input() disable = false;
  @Input() required = false;
  // Có label hay không
  @Input() hasLable = true;
  // Vị trí của label
  @Input() lableDirection = LableDirection.Top;
  // giá trị của label
  @Input() label: string;
  // placeholder của selection
  @Input() placeholder: string;
  // giá trị của selection
  @Input() value: number | string;
  // list đầu vào
  @Input() input: ValueAndNameModel[];
  // default searchText
  @Input() searchText = "";
  @Input() placeholderSearchText = "Search text...";

  @ViewChild('search') search: ElementRef;
  @Output() ngModelChange = new EventEmitter<number | string>();
  options: ValueAndNameModel[];
  tepmOptions: ValueAndNameModel[];

  constructor(injector: Injector,) {
    super(injector);
  }
  ngOnChanges(): void {
    this.tepmOptions = this.options = this.input;
  }
  ngOnInit(): void {

  }
  isLableOnTop() {
    return this.hasLable && this.lableDirection == LableDirection.Top;
  }
  openChange(isOpen: boolean) {
    if (isOpen) {
      this.focusSearch();
      this.searchChange();
    } else {
      this.searchTextClear();
    }
  }
  focusSearch() {
    if (!this.searchable) return;
    this.search.nativeElement.focus();
  }
  selectedChange() {
    this.ngModelChange.emit(this.value);
  }
  searchTextClear(){
    this.options = this.tepmOptions;
  }
  searchChange() {
    if (!this.searchText) {
      this.searchTextClear();
      return;
    }
    const searchText = this.searchText.trim().toLocaleLowerCase();
    this.options = this.tepmOptions.filter(s => s.name.trim().toLocaleLowerCase().includes(searchText));
  }

}
export enum LableDirection {
  Top,
  Left
}
