import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { EditReportDateComponent } from './edit-report-date.component';

describe('EditReportDateComponent', () => {
  let component: EditReportDateComponent;
  let fixture: ComponentFixture<EditReportDateComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ EditReportDateComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(EditReportDateComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
