import { Component, EventEmitter, Inject, Injector, OnInit, Output } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA, MatDialog } from '@angular/material/dialog';
import { DashBoardService } from '@app/service/api/dash-board.service';
import { BaoCaoThuDto } from '../home.component';
import { RevenueExpenseType } from '@shared/AppEnums';
import { ResultCircleChartDetailDto } from '@app/service/model/circle-chart.dto';
import { CreateEditCircleChartDetailComponent } from '@app/modules/circle-chart/circle-chart-detail/create-edit-circle-chart-detail/create-edit-circle-chart-detail.component';
import { CircleChartDetailService } from '@app/service/api/circle-chart-detail.service';
import { PERMISSIONS_CONSTANT } from '@app/constant/permission.constant';
import { Router } from '@angular/router';
import { AppComponentBase } from '@shared/app-component-base';

@Component({
  selector: 'app-detail-baocao-thu',
  templateUrl: './detail-baocao-thu.component.html',
  styleUrls: ['./detail-baocao-thu.component.css']
})
export class DetailBaocaoThuComponent extends AppComponentBase implements OnInit {
  @Output() refreshDataEvent = new EventEmitter<any>();
  public baoCaoThu: BaoCaoThuDto[] = []
  public startDate: string = ""
  public endDate: string = ""
  public isDoanhThu: any;
  public currentPage: number = 1
  public itemPerPage:number = 10
  public pageSizeType: number = 10;
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
  Finance_BankTransaction_BankTransactionDetail_TabIncommingEntry_View = PERMISSIONS_CONSTANT.Finance_BankTransaction_BankTransactionDetail_TabIncommingEntry_View;

  constructor(
    private dashBoardService: DashBoardService,
    private circleChartDetailService: CircleChartDetailService,
    public dialogRef: MatDialogRef<DetailBaocaoThuComponent>,
    private dialog: MatDialog,
    @Inject(MAT_DIALOG_DATA) public data,
    injector: Injector,
    private router: Router) {
       super(injector)
    }

  ngOnInit(): void {
    this.startDate = this.data.startDate
    this.endDate = this.data.endDate
    this.isDoanhThu = this.data.isDoanhThu
    if (this.data.circleChartDetail) {
      this.circleChartDetail = this.data.circleChartDetail
    }
    if (this.data.circleChart){
      this.circleChart = this.data.circleChart
    }
    this.refresh()
  }
  refresh(){
    if (this.data.circleChartDetail) {
      this.getDetailBaoCaoThuForCircleChart()
    }
    else {
      this.getDetailBaoCaoThu()
    }
  }

  refreshChart(){
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
  changePageSize() {
    this.currentPage = 1;
    this.itemPerPage = this.pageSizeType;
    this.refresh();
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
        this.refreshChart()
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

  showRevenueRecordDetail(id: any) {
    if (this.permission.isGranted(this.Finance_BankTransaction_BankTransactionDetail_TabIncommingEntry_View)) {
      const url = this.router.createUrlTree(["/app/detail"], {
        queryParams: { id: id,  index: 1 },
      }).toString();
  
      // Create an anchor element
      const link = document.createElement('a');
      link.href = url;
      link.target = '_blank';
      link.rel = 'noopener noreferrer';
  
      // Simulate a click on the link
      link.click();
    }
  }
  
  isAllowToRoutingRevenueRecordDetail(){
    return this.isGranted(this.Finance_BankTransaction_BankTransactionDetail_TabIncommingEntry_View)
  }
}
