import { Component, Input, Injector, SimpleChanges, OnChanges } from '@angular/core';
import { ReportService } from '@app/service/api/report.service';
import { 
  ReportTreeOutcomingEntries,
  GetAllPagingOutComingEntryDto,
  OutcomingEntryDto,
  ResultGetOutcomingEntryDto
 } from '../report.component';
import { AppComponentBase } from '@shared/app-component-base';
import { UtilitiesService } from '@app/service/api/new-versions/utilities.service';

@Component({
  selector: 'app-report-list',
  templateUrl: './report-list.component.html',
  styleUrls: ['./report-list.component.css']
})
export class ReportListComponent extends AppComponentBase implements OnChanges {

  @Input() node!: ReportTreeOutcomingEntries;
  @Input() filterRequest!: GetAllPagingOutComingEntryDto;
  @Input() resetVersion!: number;

  isExpanded = false;
  isTableLoading = false;
  historyExtend = false;

  pageIndex = 1;
  pageSize = 10;
  totalItems = 0;

  requestList: OutcomingEntryDto[] = [];

  get paginationId(): string {
    return this.node?.item?.id ? `report-${this.node.item.id}` : 'report';
  }

  constructor(
    private reportService: ReportService,
    public _utilities: UtilitiesService,
    injector: Injector
  ) {
    super(injector);
  }

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['resetVersion']) {
      this.isExpanded = false;
      this.pageIndex = 1;
      this.requestList = [];
      this.totalItems = 0;
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

    const skipCount = Math.max(0, (this.pageIndex - 1) * this.pageSize);
    const baseRequest = this.filterRequest ? { ...this.filterRequest } : new GetAllPagingOutComingEntryDto();

    const req: GetAllPagingOutComingEntryDto = {
      ...baseRequest,
      skipCount,
      maxResultCount: this.pageSize,
      outComingEntryTypeIds: [this.node.item.id]
    };

    this.reportService.getAllPaging(req).subscribe(res => {
      const result = res.result as ResultGetOutcomingEntryDto;
      this.requestList = result.resultPaging.items;
      this.totalItems = result.resultPaging.totalCount;
      this.isTableLoading = false;
    });
  }

  onPageChange(page: number) {
    this.pageIndex = page;
    this.loadTable();
  }

}
