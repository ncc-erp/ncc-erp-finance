import { Component, Input, OnInit, ElementRef, ViewChild } from '@angular/core';
import * as echarts from 'echarts';

@Component({
  selector: 'app-pie-chart',
  templateUrl: './pie-chart.component.html',
  styleUrls: ['./pie-chart.component.css']
})
export class PieChartComponent implements OnInit {
  @Input() chartData: any;
  @ViewChild('chartContainer') chartContainer: ElementRef;
  
  constructor() {}

  ngOnInit() {
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
            fontSize: 16,
          },
          padding: 20,
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
        series: [
          {
            type: 'pie',
            radius: '65%',
            center: ['50%', '50%'],
            selectedMode: 'single',
            data: this.chartData.details.map((item) => ({
              name: item.name,
              value: item.value,
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
            }
          }
        ]
      };
      option && myChart.setOption(option);
    }
  }
}

