import { Component, Inject, OnInit } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { DashBoardService } from '@app/service/api/dash-board.service';
import { BaoCaoThuDto } from '../home.component';

@Component({
  selector: 'app-detail-baocao-thu',
  templateUrl: './detail-baocao-thu.component.html',
  styleUrls: ['./detail-baocao-thu.component.css']
})
export class DetailBaocaoThuComponent implements OnInit {

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


  constructor(private dashBoardService: DashBoardService,
    public dialogRef: MatDialogRef<DetailBaocaoThuComponent>,
    @Inject(MAT_DIALOG_DATA) public data) { }

  ngOnInit(): void {
    this.startDate = this.data.startDate
    this.endDate = this.data.endDate
    this.isDoanhThu = this.data.isDoanhThu
    this.getDetailBaoCaoThu()
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
