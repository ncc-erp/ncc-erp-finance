import { Component, Input, Injector, SimpleChanges, OnChanges } from '@angular/core';
import { RevenueReportService } from '@app/service/api/revenue-report.service';
import { 
  ReportTreeIncomingEntries,
  GetAllPagingInComingEntry,
  IncomingEntryDto,
  ResultGetIncomingEntryDto
 } from '../revenue-report.component';
import { AppComponentBase } from '@shared/app-component-base';
import { UtilitiesService } from '@app/service/api/new-versions/utilities.service';

@Component({
  selector: 'app-revenue-report-list',
  templateUrl: './revenue-report-list.component.html',
  styleUrls: ['./revenue-report-list.component.css']
})
export class RevenueReportListComponent extends AppComponentBase implements OnChanges {
  @Input() node!: ReportTreeIncomingEntries;
  @Input() filterRequest!: GetAllPagingInComingEntry;
  @Input() resetVersion!: number;
  
  isExpanded = false;
  isTableLoading = false;
  historyExtend = false;

  readonly maxResultCount = 100000;

  requestList: IncomingEntryDto[] = [];

  constructor(
    private service: RevenueReportService,
    public _utilities: UtilitiesService,
    injector: Injector
  ) {
    super(injector)
   }

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['resetVersion']) {
      this.isExpanded = false;
      this.requestList = [];
    }
  }

  onTogglePanel() {
    this.isExpanded = !this.isExpanded;
    if (this.isExpanded && this.node.children.length === 0) {
      this.loadTable();
    }
  }

  loadTable() {
    this.isTableLoading = true;
    const baseRequest = this.filterRequest ? { ...this.filterRequest } : new GetAllPagingInComingEntry();

    const req: GetAllPagingInComingEntry = {
      ...baseRequest,
      skipCount: 0,
      maxResultCount: this.maxResultCount,
      incomingEntryTypeIds: [this.node.item.id]
    };
    
    this.service.getAllPaging(req).subscribe(res => {
      const result = res.result as ResultGetIncomingEntryDto;
      this.requestList = result.resultPagging.items;
      this.isTableLoading = false;
    })
  }
}
