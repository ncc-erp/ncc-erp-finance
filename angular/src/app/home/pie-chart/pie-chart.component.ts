import { Component, Input, OnInit, ElementRef, ViewChild, Injector, Output, EventEmitter } from '@angular/core';
import { inject } from '@angular/core/testing';
import { MatDialog } from '@angular/material/dialog';
import { ActivatedRoute, Router } from '@angular/router';
import { AppComponentBase } from '@shared/app-component-base';
import * as echarts from 'echarts';
import * as moment from 'moment';
import { DetailBaocaoThuComponent } from '../detail-baocao-thu/detail-baocao-thu.component';
import { DetailBaocaoChiComponent } from '../detail-baocao-chi/detail-baocao-chi.component';
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
                color: item.color,
              },
              label: {
                show: true,
                formatter: '{b} ({d}%)', // Display name and percentage
              },
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
      this.viewBaoCaoThuDetailFromCircleChart(circleChartDetail)
    }
    else{
      this.viewBaoCaoChiDetailFromCircleChart(circleChartDetail)
    }
  }

  viewBaoCaoThuDetailFromCircleChart(circleChartDetail : ResultCircleChartDetailDto) {
    let ref = this.dialog.open(DetailBaocaoThuComponent, {
      width: "90vw",
      maxWidth: "90vw",
      data: {
        startDate: this.fromDate,
        endDate: this.toDate,
        circleChartDetail : circleChartDetail,
        circleChart : this.chartData.chartName
      },
      disableClose: true
    });
    ref.componentInstance.refreshDataEvent.subscribe((data) => {
      this.onRefreshData();
    });
  }

  viewBaoCaoChiDetailFromCircleChart(circleChartDetail : ResultCircleChartDetailDto) {
    let ref = this.dialog.open(DetailBaocaoChiComponent, {
      width: "90vw",
      maxWidth: "90vw",
      data: {
        startDate: this.fromDate,
        endDate: this.toDate,        
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

