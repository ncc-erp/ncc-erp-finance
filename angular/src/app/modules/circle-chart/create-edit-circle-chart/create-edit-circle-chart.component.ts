import { expenditureDto } from './../../expenditure/expenditure.component';
import { Component, Inject, OnInit } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { CircleChartService } from '@app/service/api/circle-chart.service';
import { RevenueService } from '@app/service/api/revenue.service';
import { CircleChartDto } from '@app/service/model/circle-chart.dto';
import { ExpenditureService } from '@app/service/api/expenditure.service';

@Component({
  selector: 'app-create-edit-circle-chart',
  templateUrl: './create-edit-circle-chart.component.html',
  styleUrls: ['./create-edit-circle-chart.component.css']
})
export class CreateEditCircleChartComponent implements OnInit {

  public title: string = ""
  public circleChart = {} as CircleChartDto
  public isEdit: boolean = false
  public isLoading:boolean = false;


  constructor(@Inject(MAT_DIALOG_DATA) private data: CircleChartDto,
    public dialogRef: MatDialogRef<CreateEditCircleChartComponent>,
    private CircleChartService: CircleChartService) {
    if (data) {
      this.isEdit = true
      this.circleChart = this.data
      this.title = `Chỉnh sửa Circle Chart: ${this.circleChart.name}`
    }
    else {
      this.title = "Thêm mới Circle Chart"
    }
  }

  ngOnInit(): void {
  }

  onSave() {
    this.isLoading = true;
    if (!this.data) {
      this.CircleChartService.create(this.circleChart).subscribe(rs => {
        abp.notify.success(`Created new setting: ${this.circleChart.name}`)
        this.isLoading = false;
        this.dialogRef.close(true);
      },()=> this.isLoading = false)
    }
    else {
      this.CircleChartService.update(this.circleChart).subscribe(rs => {
        abp.notify.success(`Edited chart setting: ${this.circleChart.name}`)
        this.isLoading = false;
        this.dialogRef.close(true);
      },()=> this.isLoading = false)
    }
  }
}

