import { Component, ElementRef, EventEmitter, Injector, Input, OnInit, Output, ViewChild } from '@angular/core';
import { TreeOutcomingEntries } from '@app/modules/expenditure-request/expenditure-request.component';
import { OptionIncomingEntryTypeDto } from '@app/modules/new-versions/b-transaction/currency-exchange/currency-exchange.component';
import { IncomingEntryTypeOptions, OptionItem } from '@app/modules/new-versions/b-transaction/link-revenue-ecognition-dialog/link-revenue-ecognition-dialog.component';
import { CommonService } from '@app/service/api/new-versions/common.service';
import { ValueAndNameModel } from '@app/service/model/common-DTO';
import { AppComponentBase } from '@shared/app-component-base';
import { TypeFilterTypeOptions } from '@shared/AppEnums';
import * as _ from 'lodash';

@Component({
  selector: 'app-tree-in-out-type',
  templateUrl: './tree-in-out-type.component.html',
  styleUrls: ['./tree-in-out-type.component.css']
})
export class TreeInOutTypeComponent extends AppComponentBase implements OnInit {

  public options: ItemNode[] = [];
  public tempOptions: ItemNode[] = [];
  public treeValue: IncomingEntryTypeOptions = new IncomingEntryTypeOptions();
  @Input() searchText: string = "";
  @Input() undefinedValue: number = undefined;
  @Input() placeholderSearch: string = "";
  @Input() isDisable = false;
  @Input() isShowAll = true;
  @Input() label = "";
  @Input() isDisableParent = true;
  @Input() isRequired = true;
  @Input() placeholder = "";
  @Input() space = 1;
  @ViewChild('input') input: ElementRef;
  @Input() selected: number;
  @Input() removeIfNotIn = true;
  @Input() warnText = "Option của bạn không còn tồn tại trong list";
  @Input() filter = new TreeInOutTypeOption;
  @Output() selectChange = new EventEmitter<number>();

  constructor(
    injector: Injector,
    private common: CommonService,
  ) {
    super(injector);
  }

  ngOnInit(): void {
    this.getData();
  }
  getData() {
    this.common.GetTypeOptions(this.filter).subscribe(response => {
      this.treeValue = { item: { id: undefined, name: "", parentId: null }, children: [...response.result] };
      this.tempOptions = this.options = this.convertTreeToList(this.treeValue);
      if(this.removeIfNotIn && this.selected != this.undefinedValue && !this.tempOptions.find(s => s.value == this.selected)) {
        console.warn("selected", this.selected);
        console.warn("undefinedValue", this.undefinedValue);
        abp.notify.warn(this.warnText);
        this.selected = this.undefinedValue;
        this.selectdChange();
      }
      this.searchTextChange();
    })
  }
  convertTreeToList(tree, pathId = "|", level = -2): ItemNode[] {
    let cloneTree = _.cloneDeep(tree);
    let result: ItemNode[] = [];
    const path = `${pathId ? pathId : ""}${cloneTree.item.id ? (cloneTree.item.id + "|") : ""}`;
    result.push({ name: cloneTree.item.name, value: cloneTree.item.id, pathId: path, level: level + 1, hasChildren: cloneTree.children.length > 0, isActive: cloneTree.item.isActive } as ItemNode)
    if (cloneTree.children)
      cloneTree.children.forEach(s => result = [...result, ...this.convertTreeToList(s, path, level + 1)]);
    return result;
  }
  selectedOpenedChange(isOpen) {
    if (!isOpen) return;
    this.input.nativeElement.focus();
  }
  searchTextChange() {
    if (!this.searchText) this.options = _.cloneDeep(this.tempOptions);
    this.filterItemNode(this.searchText);
  }
  selectdChange(){
    this.selectChange.emit(this.selected);
  }
  filterItemNode(text = "") {
    text = text.toLowerCase();

    const idNodeHaveText = this.tempOptions.filter(s => s.value && s.name.trim().toLowerCase().includes(text));
    const nodeChilrien = [...new Set(this.tempOptions.filter(s => idNodeHaveText.find(x => s.pathId.includes(`|${x.value}|`))).map(s => s.value))];
    const parentIds = [...new Set(idNodeHaveText.map(s => s.pathId).join("|").split("|"))].map(Number);
    const Ids = new Set([...parentIds, ...nodeChilrien]);
    this.options = this.tempOptions.map(s => {
      if (Ids.has(s.value)) s.hidden = false
      else s.hidden = true
      return s;
    });
  }


}
export class TreeInOutTypeOption {
  type: TypeFilterTypeOptions;
  isShowAll: boolean;
  userId?: number;
}
export class ItemNode {
  name: string;
  value: number;
  pathId: string;
  level: number;
  hasChildren: boolean;
  hidden: boolean;
  isActive:boolean
}

