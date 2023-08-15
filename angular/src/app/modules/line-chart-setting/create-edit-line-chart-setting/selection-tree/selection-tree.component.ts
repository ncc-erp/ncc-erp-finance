import { AddLinechartSettingDto } from './../../../../service/model/linechartSetting.dto';
import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { LinechartSettingService } from '@app/service/api/linechart-setting.service';

@Component({
  selector: 'app-selection-tree',
  templateUrl: './selection-tree.component.html',
  styleUrls: ['./selection-tree.component.css']
})
export class SelectionTreeComponent implements OnInit {
  @Input() data: any
  @Input() selected?: boolean
  @Input() chartId: number
  @Input() existList: number[] = []
  @Output() onSelectedItem = new EventEmitter();
  @Output() onRemoveItem = new EventEmitter();

  constructor(private lineChartSettingService: LinechartSettingService) { }

  ngOnInit(): void {

  }
  ngOnChanges(): void {
    this.data.checked = this.data && this.existList && this.existList.includes(this.data.id)
  }

  onSelect() {
    this.onSelectedItem.emit()
  }
  async onCheck(event) {
    if (!event.checked) {
      let input = {
        linechartId: this.chartId,
        referenceId: this.data.id
      } as AddLinechartSettingDto
      this.lineChartSettingService.addReferenceToLineChart(input).subscribe(rs => {
        abp.notify.success("added new setting to line chart")
        this.onSelectedItem.emit()
      })
    }
    else {
      this.RemoveLineChartReference()
    }
  }

  onRemove() {
    this.onRemoveItem.emit()
  }
  public isFinalNode() {
    return !this.data.children || this.data.children.length == 0
  }


  public RemoveLineChartReference() {
    let input = {
      chartSettingId: this.chartId,
      referenceId: this.data.id
    }
    this.lineChartSettingService.RemoveLineChartReference(input).subscribe(rs => {
      abp.notify.success(`Remove setting ${this.data.name}}`)
      this.onRemoveItem.emit()
    })
  }



}
