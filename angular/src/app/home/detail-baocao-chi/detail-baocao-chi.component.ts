import { data } from 'jquery';
import { Component, Inject, OnInit } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { DashBoardService } from '@app/service/api/dash-board.service';
import { BaoCaoChiDto, baoCaoFilterOption, BaoCaoThuDto } from '../home.component';
import { ExpenseType } from '@shared/AppEnums';
import * as FileSaver from 'file-saver';

@Component({
  selector: 'app-detail-baocao-chi',
  templateUrl: './detail-baocao-chi.component.html',
  styleUrls: ['./detail-baocao-chi.component.css']
})
export class DetailBaocaoChiComponent implements OnInit {

  public baoCaoChi: BaoCaoChiDto[] = []
  public startDate: string = ""
  public endDate: string = "" 
  public branchId: number;
  public expenseType: number;
  public total: number = 0
  public currentPage: number = 1
  public itemPerPage:number = 10
  sortColumn: string;
  sortDirect: number;
  iconSort: string;
  sortedBaoCaoChi: BaoCaoChiDto[]=[];


  constructor(private dashBoardService: DashBoardService,
    public dialogRef: MatDialogRef<DetailBaocaoChiComponent>,
    @Inject(MAT_DIALOG_DATA) public data) { }

  ngOnInit(): void {
    this.startDate = this.data.startDate
    this.endDate = this.data.endDate
    this.branchId = this.data.branchId   
    this.expenseType = this.data.expenseType
    this.getDetailBaoCaoChi()
  }

  getDetailBaoCaoChi() {
    this.dashBoardService.GetDataBaoCaoChi(this.startDate, this.endDate, this.branchId, this.expenseType).subscribe(rs => {
      this.baoCaoChi = rs.result;      

      if (this.data.branchName != "Tổng cộng") {
        this.baoCaoChi = this.baoCaoChi.filter(x => x.branchName.toLowerCase().trim() == this.data.branchName.toLowerCase().trim())
      }
      if (this.data.tinhVaoChiPhi) {
        this.baoCaoChi = this.baoCaoChi.filter(x => x.expenseType === baoCaoFilterOption.REAL_EXPENSE)
      }

      this.sortedBaoCaoChi = this.baoCaoChi.slice();
      
      this.total = this.baoCaoChi.reduce((sum, val) => {
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
      this.sortedBaoCaoChi = this.baoCaoChi.slice();
    }
  }

  sortAsc(sortColumn: string){
    this.sortedBaoCaoChi.sort((a,b) => (typeof a[sortColumn] === "number") ? a[sortColumn]-b[sortColumn] : (a[sortColumn] ?? "").localeCompare(b[sortColumn] ?? ""));
  }
  sortDesc(sortColumn: string){
    this.sortedBaoCaoChi.sort((a,b) => (typeof a[sortColumn] === "number") ? b[sortColumn]-a[sortColumn] : (b[sortColumn] ?? "").localeCompare(a[sortColumn] ?? ""));
  }

  convertFile(fileData) {
    var buf = new ArrayBuffer(fileData.length);
    var view = new Uint8Array(buf);
    for (var i = 0; i != fileData.length; ++i) view[i] = fileData.charCodeAt(i) & 0xFF;
    return buf;
  }

  exportBBC(){
    abp.message.confirm(
      "Bạn có chắc muốn xuất danh sách chi của " + (this.data.branchName == 'Tổng cộng' ? 'Toàn công ty' : this.data.branchName) + " ?", 
      "Xác nhận", 
      (result) => {
      if (result) { 
          this.dashBoardService.exportExcelBBC(this.startDate, this.endDate, this.branchId).subscribe(data => {
            const file = new Blob([this.convertFile(atob(data.result))], {
              type: "application/vnd.ms-excel;charset=utf-8"
            });
            let type = "";
            if(this.data.expenseType === -1){
              type = "Tổng_Chi_";
            }
            else if(this.data.expenseType === 0){
              type = "Chi_Thực_";
            }
            else if(this.data.expenseType === 1){
              type = "Chi_Không_Thực_";
            }
            var branchNameReplace = this.data.branchName.replace(/ /g, "_");
            FileSaver.saveAs(file, `Báo_Cáo_${type}Chi_Nhánh_${branchNameReplace}.xlsx`);
            abp.notify.success("export successful")
            this.dialogRef.close()
          })
        }
    });
  }
  
}
