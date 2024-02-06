import { Component, ElementRef, EventEmitter, Injector, Input, OnInit, Output, ViewChild, ViewEncapsulation } from '@angular/core';
import { MatSelect, MatSelectChange } from '@angular/material/select';
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
  public prevSelected: number[] = []; // Use for multi-select logic
  public tempSelected: number[] = []; // Use for save value before submit
  @Input() searchText: string = "";
  @Input() undefinedValue: number[] = undefined;
  @Input() placeholderSearch: string = "";
  @Input() isDisable = false;
  @Input() isShowAll = true;
  @Input() label = "";
  @Input() isDisableParent = true;
  @Input() isRequired = true;
  @Input() placeholder = "";
  @Input() space = 1;
  @ViewChild('input') input: ElementRef;
  @ViewChild("matSelect") matSelect: MatSelect;
  @Input() selected: number[] = [];
  @Input() removeIfNotIn = true;
  @Input() warnText = "Option của bạn không còn tồn tại trong list";
  @Input() filter = new TreeInOutTypeOption;
  @Output() selectChange = new EventEmitter<number[]>();

  constructor(
    injector: Injector,
    private common: CommonService,
  ) {
    super(injector);
  }

  ngOnInit(): void {
    this.getData();
    this.tempSelected = this.selected;
    this.prevSelected = this.selected;
  }

  showAllToggle(){
    this.filter.isShowAll = !this.filter.isShowAll;
    this.getData();
  }

  getData() {
    this.common.GetTypeOptions(this.filter).subscribe(response => {
      this.treeValue = { item: { id: undefined, name: "", parentId: null }, children: [...response.result] };
      this.tempOptions = this.options = this.convertTreeToList(this.treeValue);
      if(this.removeIfNotIn && this.selected.length > 0 && !this.tempOptions.some((option) => this.selected.includes(option.value))) {
        console.warn("selected", this.selected);
        console.warn("undefinedValue", this.undefinedValue);
        abp.notify.warn(this.warnText);
        this.selected = this.undefinedValue;
        this.tempSelected = this.undefinedValue;
        this.prevSelected = this.undefinedValue;
        this.selectedChange();
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
  selectedChange() {
    this.searchText = "";
    this.selectChange.emit(this.selected);
  }

  onClearSelected() {
    this.tempSelected = [];
    this.prevSelected = [];
  }

  onSelectNode(nodeSelected: ItemNode) {
    const selectedIds = new Set(this.prevSelected); // The current set of selected item values
    const toBeToggled = new Set<number>(); // Set to keep track of items to be toggled

    // Function to recursively add all children of a node (including itself) to the toggle set
    const addNodeAndChildren = (node: ItemNode) => {
      if (!toBeToggled.has(node.value)) { // Check if the node has already been added
        toBeToggled.add(node.value); // Add the node itself to the toggle set
        this.tempOptions.forEach(child => {
          if (child.pathId.includes(`|${node.value}|`)) {
            addNodeAndChildren(child); // Recursively add children of the node
          }
        });
      }
    };

    addNodeAndChildren(nodeSelected); // Add the option and its children (including itself) to the toggle set
    
    toBeToggled.forEach(value => {
      if (this.prevSelected.includes(nodeSelected.value)) {
        if (selectedIds.has(value))
          selectedIds.delete(value); // If node is Selected, deselect
      } else {
        selectedIds.add(value); // If node is not Selected, select
      }
    });
    this.selected = Array.from(selectedIds);
    this.prevSelected = this.selected;
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

  getMarginLeft(optionLevel: number): string {
    return `${optionLevel * this.space}rem`;
  }

  handleClear() {
    this.selected = [];
    this.prevSelected = [];
  }

  onCancelSelect() {
    this.selected = this.tempSelected;
    this.prevSelected = this.tempSelected;
    this.matSelect.close();
    this.selectedChange();
  }

  onSubmitSelect() {
    this.tempSelected = this.selected;
    this.matSelect.close();
    this.selectedChange();
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

