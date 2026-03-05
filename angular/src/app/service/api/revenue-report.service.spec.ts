import { TestBed } from '@angular/core/testing';

import { RevenueReportService } from './revenue-report.service';

describe('RevenueReportService', () => {
  let service: RevenueReportService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(RevenueReportService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
