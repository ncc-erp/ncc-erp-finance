import { Component, Input, OnInit, ElementRef, ViewChild, Injector, Output, EventEmitter } from '@angular/core';
import { inject } from '@angular/core/testing';
import { MatDialog } from '@angular/material/dialog';
import { ActivatedRoute, Router } from '@angular/router';
import { AppComponentBase } from '@shared/app-component-base';
import * as echarts from 'echarts';
import * as moment from 'moment';
import { DetailBaocaoThuComponent } from '../detail-baocao-thu/detail-baocao-thu.component';
import { DetailBaocaoChiComponent } from '../detail-baocao-chi/detail-baocao-chi.component';
import { RevenueExpenseType } from '@shared/AppEnums';
import { ResultCircleChartDetailDto, ResultCircleChartDto } from '@app/service/model/circle-chart.dto';

@Component({
  selector: 'app-pie-chart',
  templateUrl: './pie-chart.component.html',
  styleUrls: ['./pie-chart.component.css']
})
export class PieChartComponent extends AppComponentBase implements OnInit {
  @Input() chartData: ResultCircleChartDto;
  @Input() fromDate: any;
  @Input() toDate: any;
  @ViewChild('chartContainer') chartContainer: ElementRef;
  @Output() refreshData = new EventEmitter<any>(); 
  isIncome: boolean;
  
  constructor(injector: Injector, private router: Router, private route: ActivatedRoute, private dialog: MatDialog) {
    super(injector);
  }

  ngOnInit() {
    this.isIncome = this.chartData.isIncome
  }

  ngAfterViewInit() {
    if (this.chartContainer) {
      var myChart = echarts.init(this.chartContainer.nativeElement);
      
      var option;
      option = {
        title: {
          text: this.chartData.chartName,
          left: 'center',
          textStyle: {
            fontFamily: 'Source Sans Pro',
            fontSize: 20,
          },
        },
        tooltip: {
          trigger: 'item',
        },
        legend: {
          orient: 'horizontal',
          bottom: 0,
          data: this.chartData.details?.map((item) => ({
            name: item.name
          })),
        },
        toolbox: {
          feature: {
            saveAsImage: {
              title: ""
            }
          }
        },
        grid: {
          top: '20%', // Adjust the top padding (space for the title)
          bottom: '20%', // Adjust the bottom padding (space for the legend)
        },
        series: [
          {
            type: 'pie',
            radius: '65%',
            center: ['50%', '50%'],
            selectedMode: 'single',
            data: this.chartData.details.map((item) => ({
              name: item.name,
              value: item.value,
              detail: item,
              itemStyle: {
                color: item.color
              }
            })),
            emphasis: {
              itemStyle: {
                shadowBlur: 10,
                shadowOffsetX: 0,
                shadowColor: 'rgba(0, 0, 0, 0.5)'
              }
            },
            events: {
              click: (event: any) => {
              },
            },
          }
        ]
      };
      option && myChart.setOption(option);
      myChart.on('click', (params: any) => {
        this.viewDataBaoCaoFromChartDetail(params.data.detail);
      });
    }
  }

  onRefreshData() {
    this.refreshData.emit();
  }

  viewDataBaoCaoFromChartDetail(circleChartDetail: ResultCircleChartDetailDto){
    if (this.isIncome){
      let revenueCounted = circleChartDetail.revenueExpenseType == RevenueExpenseType.REAL_REVENUE_EXPENSE ? true 
                         : circleChartDetail.revenueExpenseType == RevenueExpenseType.NON_REVENUE_EXPENSE ? false : null
      this.viewBaoCaoThuDetailFromCircleChart(revenueCounted, revenueCounted, circleChartDetail)
    }
    else{
      let branchId =  circleChartDetail.branchId == null ? 0 : circleChartDetail.branchId
      let expenseType = circleChartDetail.revenueExpenseType == RevenueExpenseType.ALL_REVENUE_EXPENSE ? -1 
                      : circleChartDetail.revenueExpenseType == RevenueExpenseType.REAL_REVENUE_EXPENSE ? 0 : 1
      this.viewBaoCaoChiDetailFromCircleChart(circleChartDetail.branchName, branchId, expenseType, circleChartDetail)
    }
  }

  viewBaoCaoThuDetailFromCircleChart(tinhVaoDoanhThu: boolean, isDoanhThu : any, circleChartDetail : ResultCircleChartDetailDto) {
    let ref = this.dialog.open(DetailBaocaoThuComponent, {
      width: "80vw",
      data: {
        startDate: this.fromDate,
        endDate: this.toDate,
        tinhVaoDoanhThu: tinhVaoDoanhThu,
        isDoanhThu : isDoanhThu,
        circleChartDetail : circleChartDetail,
        circleChart : this.chartData.chartName
      },
      disableClose: true
    });
    ref.componentInstance.refreshDataEvent.subscribe((data) => {
      this.onRefreshData();
    });
  }

  viewBaoCaoChiDetailFromCircleChart(branchName:string,branchId:number, expenseType:number , circleChartDetail : ResultCircleChartDetailDto) {
    let ref = this.dialog.open(DetailBaocaoChiComponent, {
      width: "80vw",
      data: {
        startDate: this.fromDate,
        endDate: this.toDate,        
        branchName: branchName,
        branchId: branchId,
        expenseType: expenseType,
        circleChartDetail : circleChartDetail,
        circleChart : this.chartData.chartName
      },
      disableClose: true
    });
    ref.componentInstance.refreshDataEvent.subscribe((data) => {
      this.onRefreshData();
    });
  }

}

