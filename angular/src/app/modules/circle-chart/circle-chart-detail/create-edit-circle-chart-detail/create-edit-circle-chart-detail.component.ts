import { expenditureDto } from '@app/modules/expenditure/expenditure.component';
import { Component, Inject, OnInit } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { RevenuesDto } from '@app/modules/revenue/revenue.component';
import { CircleChartDetailService } from '@app/service/api/circle-chart-detail.service';
import { RevenueService } from '@app/service/api/revenue.service';
import { ExpenditureService } from '@app/service/api/expenditure.service';
import { BranchInfoDto, CircleChartDetailInfoDto, ClientInfoDto, CreateCircleChartDetailDto, UpdateCircleChartDetailDto } from '@app/service/model/circle-chart.dto';
import { BranchService } from "@app/service/api/branch.service";
import { BranchDto } from '@app/modules/branch/branch.component';
import { FormControl } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { CommonService } from '@app/service/api/new-versions/common.service';


@Component({
  selector: 'app-create-edit-circle-chart-detail',
  templateUrl: './create-edit-circle-chart-detail.component.html',
  styleUrls: ['./create-edit-circle-chart-detail.component.css']
})
export class CreateEditCircleChartDetailComponent implements OnInit {
  public router: Router
  public paramId
  public title: string = ""
  public circleChartTypeName: string = ""
  public createChartDetail: CreateCircleChartDetailDto
  public updateChartDetail: UpdateCircleChartDetailDto
  public chartDetail = {} as CircleChartDetailInfoDto
  public branchInfo = {} as BranchInfoDto
  public listClientId: number[] = []
  public clientInfos: ClientInfoDto[] = []
  public clientOptions: ClientOptionDto[] = []
  public referenceList = []
  public selectedReferences: number[] = []
  public existReferenceIds: number[] = []
  public isEdit: boolean = false
  public isIncome: boolean
  public isLoading:boolean = false;
  public listBranch: BranchDto[] = [];
  public branchSearch: FormControl = new FormControl("")
  public listBranchFilter: BranchDto[];
  public listClient: ClientInfoDto[] = [];

  public inputFilter: InputFilter = new InputFilter();

  constructor(@Inject(MAT_DIALOG_DATA) private data: any,
    public dialogRef: MatDialogRef<CreateEditCircleChartDetailComponent>,
    private revenueService: RevenueService,
    private outcomeService: ExpenditureService,
    private route: ActivatedRoute,
    private circleChartDetailService: CircleChartDetailService,
    private commonService: CommonService,
    public branchService: BranchService) {
      if (data.item) {
      this.inputFilter.isActive = null
      this.isEdit = true
      this.chartDetail = this.data.item
      this.branchInfo = this.chartDetail.branch
      this.clientInfos = this.chartDetail.clients
      this.listClientId = this.chartDetail.listClientIds
      this.title = `Chỉnh sửa detail: ${this.chartDetail.name}`
      this.circleChartTypeName = this.data.isIncome ? "Thu" : "Chi"
      this.onSelectType()
    }
    else {
      this.title = `Thêm detail mới: `
      this.chartDetail.color = "#21211f"
    }
    this.isIncome = this.data.isIncome
    this.getAllBranch();
    this.getAllClient();
    this.paramId = this.route.snapshot.queryParamMap.get('id');
    this.branchSearch.valueChanges.subscribe(() => {
      this.filterBranch();
    });
  }

  ngOnInit(): void {
  }

  getAllBranch() {
    this.branchService
      .GetAllForDropdown()
      .subscribe(
        (data) => {
          this.listBranch = data.result;
          this.listBranchFilter = this.listBranch;
        }
      );
  }

  getAllClient() {
    this.commonService
      .getAllClientInfo()
      .subscribe(
        (data) => {
          this.listClient = data.result;
          this.clientOptions = this.listClient.map((clientInfo) => {
            const clientOption = new ClientOptionDto();
            clientOption.id = clientInfo.clientId;
            clientOption.name = clientInfo.clientName;
            return clientOption;
          });
        }
      );
  }

  filterBranch(): void {
    if (this.branchSearch.value) {
      this.listBranch = this.listBranchFilter.filter(data => data.name.toLowerCase().includes(this.branchSearch.value.toLowerCase().trim()));
    } else {
      this.listBranch = this.listBranchFilter.slice();
    }
  }

  onSave() {
    this.isLoading = true;
    if (!this.data.item) {
      this.createChartDetail = {
        circleChartId: this.paramId, 
        name: this.chartDetail.name,
        color: this.chartDetail.color,
        branchId: this.branchInfo.branchId==0?null:this.branchInfo.branchId
      }
      this.circleChartDetailService.create(this.createChartDetail).subscribe(rs => {
        abp.notify.success(`Created new detail: ${this.createChartDetail.name}`)
        this.isLoading = false;
        this.dialogRef.close(true);
      },()=> this.isLoading = false)
    }
    else {
      this.updateChartDetail = {
        id: this.chartDetail.id,
        name: this.chartDetail.name,
        color: this.chartDetail.color,
        branchId: this.branchInfo.branchId==0?null:this.branchInfo.branchId,
        clientIds:this.listClientId,
      }
      this.circleChartDetailService.update(this.updateChartDetail).subscribe(rs => {
        abp.notify.success(`Edited chart setting: ${this.updateChartDetail.name}`)
        this.isLoading = false;
        this.dialogRef.close(true);
      },()=> this.isLoading = false)
    }
  }

  onChangeListClientIdSelected(selectedClientIds: number[]) {
    this.listClientId = selectedClientIds;
  }
  onExistListChange(existList: number[]) {
    this.chartDetail.listInOutcomeTypeIds = existList;
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

  private GetExistOutComeInChartDetail() {
    this.outcomeService.GetExistOutComeInCircleChartDetail(this.chartDetail.id).subscribe(data => {
      this.selectedReferences = data.result
      this.existReferenceIds = data.result.map(x => x.id)
    }
    )
  }
  private GetExistInComeInChartDetail() {
    this.revenueService.GetExistInComeInCircleChartDetail(this.chartDetail.id).subscribe(data => {
      this.selectedReferences = data.result
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
    if (this.data.isIncome) {
      this.getIncomingType()
      this.GetExistInComeInChartDetail()
    }
    else{
      this.getOutcomingType()
      this.GetExistOutComeInChartDetail()
    }
  }

  private onEmit() {
    if (this.data.isIncome) {
      this.GetExistInComeInChartDetail()
    }else {
      this.GetExistOutComeInChartDetail()
    }
  }
}


export class InputFilter {
  constructor(){
    this.isActive = null;
  }
  isActive?: boolean;
  expenseType?: number;
  revenueCounted: boolean;
  searchText: string;
}

export class ClientOptionDto {
  id: number
  name: string
}