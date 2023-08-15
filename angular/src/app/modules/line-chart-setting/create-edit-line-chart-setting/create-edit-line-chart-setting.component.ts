import { expenditureDto } from './../../expenditure/expenditure.component';
import { Component, Inject, OnInit } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { InputFilterRevenue, RevenuesDto } from '@app/modules/revenue/revenue.component';
import { LinechartSettingService } from '@app/service/api/linechart-setting.service';
import { RevenueService } from '@app/service/api/revenue.service';
import { LineChartSettingDto } from '@app/service/model/linechartSetting.dto';
import { ExpenditureService } from '@app/service/api/expenditure.service';

@Component({
  selector: 'app-create-edit-line-chart-setting',
  templateUrl: './create-edit-line-chart-setting.component.html',
  styleUrls: ['./create-edit-line-chart-setting.component.css']
})
export class CreateEditLineChartSettingComponent implements OnInit {

  public title: string = ""
  public chartSetting = {} as LineChartSettingDto
  public incomeList: RevenuesDto[] = []
  public outcomeList: expenditureDto[] = []
  public referenceList = []
  public seletedReferences: number[] = []
  public finalOutcomeTypeList = []
  public searchOutcome: string = ""
  public selectedReferences = []
  public existReferenceIds: number[] = []
  public searchReference: string = ""
  public isEdit: boolean = false
  public lineChartIncome: LineChartRefereneceInfoDto[] = []
  public lineChartOutcome: LineChartRefereneceInfoDto[] = []
  public chartType
  public isLoading:boolean = false;

  public inputFilter: InputFilter = new InputFilter();

  constructor(@Inject(MAT_DIALOG_DATA) private data: LineChartSettingDto,
    public dialogRef: MatDialogRef<CreateEditLineChartSettingComponent>,
    private revenueService: RevenueService,
    private outcomeService: ExpenditureService,
    private lineChartSettingService: LinechartSettingService) {
    this.chartType = chartSettingType
    if (data) {
      this.inputFilter.isActive = true
      this.isEdit = true
      this.chartSetting = this.data
      this.title = `Chỉnh sửa chart setting: ${this.chartSetting.name}`
      this.onSelectType()
    }
    else {
      this.title = "Thêm mới chart setting"
      this.chartSetting.color = "#21211f"
    }
  }

  ngOnInit(): void {
  }

  onSave() {
    this.isLoading = true;
    if (!this.data) {
      this.lineChartSettingService.create(this.chartSetting).subscribe(rs => {
        abp.notify.success(`Created new setting: ${this.chartSetting.name}`)
        this.isLoading = false;
        this.dialogRef.close(true);
      },()=> this.isLoading = false)
    }
    else {
      this.lineChartSettingService.update(this.chartSetting).subscribe(rs => {
        abp.notify.success(`Edited chart setting: ${this.chartSetting.name}`)
        this.isLoading = false;
        this.dialogRef.close(true);
      },()=> this.isLoading = false)
    }
  }

  private getOutcomingType() {
    this.outcomeService.getAllByStatus(this.inputFilter).subscribe(data => {
      this.referenceList = this.filtertData(data.result)
    }
    )
  }
  private getIncomingType() {
    this.revenueService.getAllByStatus(this.inputFilter).subscribe(data => {
      this.referenceList = this.filtertData(data.result)
    }
    )
  }

  private GetExistOutComeInChartSetting() {
    this.outcomeService.GetExistOutComeInChartSetting(this.data.id).subscribe(data => {
      this.seletedReferences = data.result
      this.existReferenceIds = data.result.map(x => x.id)
    }
    )
  }
  private GetExistInComeInChartSetting() {
    this.revenueService.GetExistInComeInChartSetting(this.data.id).subscribe(data => {
      this.seletedReferences = data.result
      this.existReferenceIds = data.result.map(x => x.id)
    }
    )
  }

  private filtertData(arr) {
    let rs = arr.filter((item) => {
      if (item.parentId == null) {
        return (item.children = arr.filter((child) => {
          return child.parentId == item.id;
        }));
      } else {
        return (item.children = arr.filter((child) => {
          return child.parentId == item.id;
        }));
      }
    }).filter((finalItem) => {
      return finalItem.parentId == null;
    });
    return rs;
  }

  onSelect() {
    this.onEmit()
  }
  onRemove() {
    this.onEmit()
  }

  onSelectType() {
    if (this.chartSetting.type == this.chartType.Outcome) {
      this.getOutcomingType()
      this.GetExistOutComeInChartSetting()
    }
    if (this.chartSetting.type == this.chartType.Income) {
      this.getIncomingType()
      this.GetExistInComeInChartSetting()
    }
  }

  onSelectActiveType() {
    this.inputFilter.isActive = true;
  }

  private onEmit() {
    if (this.chartSetting.type == this.chartType.Outcome) {
      this.GetExistOutComeInChartSetting()
    }
    if (this.chartSetting.type == this.chartType.Income) {
      this.GetExistInComeInChartSetting()
    }
  }

  public getLineChartInCome() {
    this.lineChartSettingService.getLineChartInCome(this.data.id)
      .subscribe(rs => {
        this.lineChartIncome = rs.result
      })
  }

  public getLineChartOutCome() {
    this.lineChartSettingService.getLineChartOutCome(this.data.id)
      .subscribe(rs => {
        this.lineChartOutcome = rs.result
      })
  }

}

export class LineChartRefereneceInfoDto {
  id: number
  name: string
}
export enum chartSettingType {
  Income = 0,
  Outcome = 1
}
export class InputFilter {
  constructor(){
    this.isActive = true;
  }
  isActive?: boolean;
  expenseType?: number;
  revenueCounted: boolean;
  searchText: string;
}
