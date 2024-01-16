import { Component, Inject, OnInit } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { BranchDto } from '@app/modules/branch/branch.component';
import { BranchService } from '@app/service/api/branch.service';
import { ExpenditureRequestService } from '@app/service/api/expenditure-request.service';
import { RequestDetailService } from '@app/service/api/request-detail.service';

@Component({
  selector: 'app-update-branch',
  templateUrl: './update-branch.component.html',
  styleUrls: ['./update-branch.component.css']
})
export class UpdateBranchComponent implements OnInit {

  constructor(private service: ExpenditureRequestService, private requestDetailService: RequestDetailService,
    @Inject(MAT_DIALOG_DATA) public data: any,
    private dialogRef: MatDialogRef<any>,
    private branchService: BranchService,
    ) { }
  public branchId: number;
  public branchList: BranchDto[]
  public tempBranchList: BranchDto[];
  public searchBranch:string = "";
  ngOnInit(): void {
    this.branchId = this.data.oldBranchId;
    this.getAllBranch();
  }

  getAllBranch() {
    this.branchService.GetAllForDropdown().subscribe((rs)=>{
      this.branchList = rs.result;
      this.tempBranchList = rs.result;
    })
  }

  filterBranch() {
    this.branchList = this.tempBranchList.filter(item => item.name.trim().toLowerCase().includes(this.searchBranch.trim().toLowerCase()))
  }

  branchSelectOpenedChange() {
    if (this.branchList.length) return;
    this.searchBranch = "";
    this.filterBranch();
  }

  saveAndClose(){
    if (this.data.isRequestDetail == true){
      let input = {
        requestDetailId :  this.data.requestId,
        branchId :  this.branchId
      } as UpdateRequestDetailBranchDto;
      this.requestDetailService.updateBranch(input).subscribe((rs)=>{
        if(rs){
          abp.notify.success("Update branch successful");
          this.dialogRef.close(true);
        }
      },()=> this.dialogRef.close());
    } else {
      let input = {
        requestId :  this.data.requestId,
        branchId :  this.branchId
      } as UpdateBranchDto;

      this.service.forceUpdateBranch(input).subscribe((rs)=>{
        if(rs){
          abp.notify.success("Update branch successful");
          this.dialogRef.close(true);
        }
      },()=> this.dialogRef.close());
    }

  }
}
export class UpdateBranchDto{
  requestId: number;
  branchId: number;
}
export class UpdateRequestDetailBranchDto{
  requestDetailId: number;
  branchId: number;
}
