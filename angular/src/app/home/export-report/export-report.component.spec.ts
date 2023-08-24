import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ExportReportComponent } from './export-report.component';

describe('ExportReportComponent', () => {
  let component: ExportReportComponent;
  let fixture: ComponentFixture<ExportReportComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ExportReportComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ExportReportComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
