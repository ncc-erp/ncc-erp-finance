import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { CircleChartDetailService } from '@app/service/api/circle-chart-detail.service';
import { UpdateCircleChartInOutcomeTypeIdsDto } from '@app/service/model/circle-chart.dto';

@Component({
  selector: 'app-selection-tree-circle-chart',
  templateUrl: './selection-tree-circle-chart.component.html',
  styleUrls: ['./selection-tree-circle-chart.component.css']
})
export class SelectionTreeCircleChartComponent implements OnInit {
  @Input() data: any
  @Input() selected?: boolean
  @Input() disabled?: boolean
  @Input() chartDetail: any
  @Input() existList: number[] = []
  @Output() onSelectedItem = new EventEmitter();
  @Output() onRemoveItem = new EventEmitter();
  @Output() existListChange = new EventEmitter<number[]>();


  constructor(private circleChartDetailService: CircleChartDetailService) { }

  ngOnInit(): void {
  }
  ngOnChanges(): void {
    this.data.checked = this.data && this.existList && this.existList.includes(this.data.id)
  }

  onSelect() {
    if (!this.disabled) {
      this.onSelectedItem.emit()
    }
  }
  
  async onCheck(event) {
    if (!this.disabled) {
      if (!event.checked) {
        this.existList.push(this.data.id);
        this.checkAllChildren(this.data, true);
        let input = {
          id: this.chartDetail.id,
          inOutcomeTypeIds: this.existList.concat(this.data.id)
        } as UpdateCircleChartInOutcomeTypeIdsDto;
    
        this.circleChartDetailService.UpdateInOutcomeTypeIds(input).subscribe(rs => {
          abp.notify.success("added new type to circle chart");
          this.existListChange.emit(this.existList);
          this.onSelectedItem.emit();
        });
      } else {
        this.existList = this.existList.filter(id => id !== this.data.id);
        this.checkAllChildren(this.data, false);
        this.RemoveInOutComeType();
      }
    }
  }

  private checkAllChildren(node: any, checked: boolean) {
    if (node.children && node.children.length > 0) {
      for (const child of node.children) {
        child.checked = checked;
        this.existList = checked
          ? [...this.existList, child.id]
          : this.existList.filter(id => id !== child.id);

        this.checkAllChildren(child, checked);
      }
    }
  }

  onRemove() {
    this.onRemoveItem.emit()
  }
  public isFinalNode() {
    return !this.data.children || this.data.children.length == 0
  }


  public RemoveInOutComeType() {
    if (!this.disabled) {
      let input = {
        id: this.chartDetail.id,
        inOutcomeTypeIds: this.existList.filter(id => id !== this.data.id)
      } as UpdateCircleChartInOutcomeTypeIdsDto;
    
      this.circleChartDetailService.UpdateInOutcomeTypeIds(input).subscribe(rs => {
        abp.notify.success(`Remove type ${this.data.name}`);
        this.existList = this.existList.filter(id => id !== this.data.id);
        this.existListChange.emit(this.existList);
        this.onRemoveItem.emit();
      });
    }
  }

  onExistListChange(existList: number[]) {
    this.existList = existList;
  }
}

