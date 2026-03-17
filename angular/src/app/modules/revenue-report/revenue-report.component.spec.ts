import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { RevenueReportComponent } from './revenue-report.component';

describe('RevenueReportComponent', () => {
  let component: RevenueReportComponent;
  let fixture: ComponentFixture<RevenueReportComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ RevenueReportComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(RevenueReportComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
