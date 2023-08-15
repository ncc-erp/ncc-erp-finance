import { Component, ElementRef, EventEmitter, Injector, Input, OnInit, Output, ViewChild } from '@angular/core';
import { AppComponentBase } from '@shared/app-component-base';
import { OptionIncomingEntryTypeDto } from '../../currency-exchange/currency-exchange.component';
import { IncomingEntryTypeOptions, OptionItem } from '../../link-revenue-ecognition-dialog/link-revenue-ecognition-dialog.component';

@Component({
  selector: 'app-selection-tree-incoming-entry-type',
  templateUrl: './selection-tree-incoming-entry-type.component.html',
  styleUrls: ['./selection-tree-incoming-entry-type.component.css']
})
export class SelectionTreeIncomingEntryTypeComponent extends AppComponentBase implements OnInit {

  public listOptionItem: OptionItem[] = [];
  public listOptionItemFindData: OptionItem[] = [];
  public options: OptionIncomingEntryTypeDto[] = [];
  public searchText: string = "";
  @Input() isDisable = false;
  @Input() isDisableParent = true;
  @Input() disabled = false;
  @Input() isRequired = true;
  @Input() treeValue: IncomingEntryTypeOptions = new IncomingEntryTypeOptions();
  @Input() placeholder = "";
  @Input() space = 1;
  @ViewChild('input') input: ElementRef;
  @Input() selected: number;
  @Output() selectChange = new EventEmitter<number>();

  constructor(
    injector: Injector,
  ) {
    super(injector);
  }

  ngOnInit(): void {
    this.options = this.getOptionAllTree();
  }
  selectedOpenedChange(isOpen: boolean) {
    if(!isOpen){
      this.options = this.getOptionAllTree();
    }else{
      this.searchTextChange();
      this.input.nativeElement.focus();
    }
  }
  selectedChange(){
    this.selectChange.emit(this.selected);
  }
  getOptionAllTree(): OptionIncomingEntryTypeDto[]{
    return this.convertTreeValueToTreeOption(this.treeValue, -1);
  }
  convertTreeValueToTreeOption(node: IncomingEntryTypeOptions, level: number) : OptionIncomingEntryTypeDto[]{
    let optionItem: OptionIncomingEntryTypeDto = {id : node.item.id, name : node.item.name, hasChildren : false, space : level};
    let treeOption: OptionIncomingEntryTypeDto[] = [];
    if(node.children.length > 0)
    {
      optionItem.hasChildren = true;
      treeOption.push(optionItem);
      node.children.forEach(childNode => {
        treeOption.push(...this.convertTreeValueToTreeOption(childNode, level + 1));
      })
    }
    else{
      treeOption.push(optionItem);
    }
    return treeOption;
  }
  searchTextChange(){
    let incomingEntryTypeOptions = this.convertTreeValueToListOption(this.treeValue, this.searchText);
    if(incomingEntryTypeOptions){
      this.options = this.convertTreeValueToTreeOption(incomingEntryTypeOptions, -1);
    }
    else{
      this.options = [];
    }
  }
  convertTreeValueToListOption(node: IncomingEntryTypeOptions, searchInTree: string) : IncomingEntryTypeOptions{
    let nodeClone : IncomingEntryTypeOptions = JSON.parse(JSON.stringify(node));
    if(this.removeVietnameseTones(nodeClone.item.name).toLowerCase().includes(this.removeVietnameseTones(searchInTree).toLowerCase())){
      return nodeClone;
    }

    if(!nodeClone.children){
      return null;
    }

    let returnChild: IncomingEntryTypeOptions[] = [];

    node.children.forEach(childNode => {
      let p = this.convertTreeValueToListOption(childNode, searchInTree);
      if(p){
        returnChild.push(p);
      }
    });

    if(returnChild.length > 0){
      nodeClone.children = returnChild;
      return nodeClone;
    }

    return null;
  }
}
