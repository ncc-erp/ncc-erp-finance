import { Component, EventEmitter, Inject, OnInit, Output } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA, MatDialog } from '@angular/material/dialog';
import { DashBoardService } from '@app/service/api/dash-board.service';
import { BaoCaoThuDto } from '../home.component';
import { RevenueExpenseType } from '@shared/AppEnums';
import { CircleChartDetailInfoDto, InputListCircleChartDto, ResultCircleChartDetailDto } from '@app/service/model/circle-chart.dto';
import { CreateEditCircleChartDetailComponent } from '@app/modules/circle-chart/circle-chart-detail/create-edit-circle-chart-detail/create-edit-circle-chart-detail.component';
import { CircleChartDetailService } from '@app/service/api/circle-chart-detail.service';

@Component({
  selector: 'app-detail-baocao-thu',
  templateUrl: './detail-baocao-thu.component.html',
  styleUrls: ['./detail-baocao-thu.component.css']
})
export class DetailBaocaoThuComponent implements OnInit {
  @Output() refreshDataEvent = new EventEmitter<any>();
  public baoCaoThu: BaoCaoThuDto[] = []
  public startDate: string = ""
  public endDate: string = ""
  public isDoanhThu: any;
  public currentPage: number = 1
  public itemPerPage:number = 10
  public total: number = 0
  sortColumn: string;
  sortDirect: number;
  iconSort: string;
  sortedBaoCaoThu: BaoCaoThuDto[]=[];
  public circleChartDetail: ResultCircleChartDetailDto;
  public circleChart: any;
  ALL_REVENUE_EXPENSE = RevenueExpenseType.ALL_REVENUE_EXPENSE;
  REAL_REVENUE_EXPENSE = RevenueExpenseType.REAL_REVENUE_EXPENSE;
  NON_REVENUE_EXPENSE = RevenueExpenseType.NON_REVENUE_EXPENSE;
  
  constructor(
    private dashBoardService: DashBoardService,
    private circleChartDetailService: CircleChartDetailService,
    public dialogRef: MatDialogRef<DetailBaocaoThuComponent>,
    private dialog: MatDialog,
    @Inject(MAT_DIALOG_DATA) public data) { }

  ngOnInit(): void {
    this.startDate = this.data.startDate
    this.endDate = this.data.endDate
    this.isDoanhThu = this.data.isDoanhThu
    if (this.data.circleChartDetail) {
      this.circleChartDetail = this.data.circleChartDetail
      this.getDetailBaoCaoThuForCircleChart()
    }
    else {
      this.getDetailBaoCaoThu()
    }
    if (this.data.circleChart){
      this.circleChart = this.data.circleChart
    }
  }

  refresh(){
    this.refreshDataEvent.emit();
    this.onClose();
  }


  getDetailBaoCaoThu() {
    this.dashBoardService.GetDataBaoCaoThu(this.startDate, this.endDate, this.isDoanhThu).subscribe(rs => {
      this.baoCaoThu = rs.result;
      this.sortedBaoCaoThu = this.baoCaoThu.slice();      
      if(this.data.tinhVaoDoanhThu){
        this.baoCaoThu = this.baoCaoThu.filter(x => x.isDoanhThu == this.data.tinhVaoDoanhThu)
      }

      this.total = this.baoCaoThu.reduce((sum, val) => {
        return sum += val.totalVND
      }, 0)

    })
  }

  getDetailBaoCaoThuForCircleChart() {
    this.dashBoardService.GetDataBaoCaoThuForCircleChart(this.startDate, this.endDate, this.circleChartDetail.id).subscribe(rs => {
      this.baoCaoThu = rs.result;
      this.sortedBaoCaoThu = this.baoCaoThu.slice();      
      this.total = this.baoCaoThu.reduce((sum, val) => {
        return sum += val.totalVND
      }, 0)
    })
  }

  public onViewDetail(id: number) {
    this.circleChartDetailService.GetCircleChartDetailInfoById(id).subscribe(data =>{
      let item = data.result;
      let ref = this.dialog.open(CreateEditCircleChartDetailComponent, {
        width: "70vw",
        data: {
          item: item,
          isIncome: true,
          isViewOnly: true
        },
        disableClose: true,
      });
      ref.componentInstance.onSaveChange.subscribe((data) => {
        this.refresh()
      });
    })
  }

  onClose() {
    this.dialogRef.close()
  }

  sortData(data) {
    if (this.sortColumn !== data) {
      this.sortDirect = -1;
    }
    this.sortColumn = data;
    this.sortDirect++;
    if (this.sortDirect > 1) {
      this.iconSort = "";
      this.sortDirect = -1;
    }
    if (this.sortDirect == 1) {
      this.iconSort = "fas fa-sort-amount-down";  // Descending sort
      this.sortDesc(this.sortColumn);
    } else if (this.sortDirect == 0) {
      this.iconSort = "fas fa-sort-amount-up";    // Ascending sort
      this.sortAsc(this.sortColumn);
    } else {
      this.iconSort = "fas fa-sort";              // Default
      this.sortedBaoCaoThu = this.baoCaoThu.slice();
    }
  }

  sortAsc(sortColumn: string){
    this.sortedBaoCaoThu.sort((a,b) => (typeof a[sortColumn] === "number") ? a[sortColumn]-b[sortColumn] : (a[sortColumn] ?? "").localeCompare(b[sortColumn] ?? ""));
  }
  sortDesc(sortColumn: string){
    this.sortedBaoCaoThu.sort((a,b) => (typeof a[sortColumn] === "number") ? b[sortColumn]-a[sortColumn] : (b[sortColumn] ?? "").localeCompare(a[sortColumn] ?? ""));
  }
}
