import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { RevenueReportListComponent } from './revenue-report-list.component';

describe('RevenueReportListComponent', () => {
  let component: RevenueReportListComponent;
  let fixture: ComponentFixture<RevenueReportListComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ RevenueReportListComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(RevenueReportListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
