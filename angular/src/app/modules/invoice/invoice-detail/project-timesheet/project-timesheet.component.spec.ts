import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ProjectTimesheetComponent } from './project-timesheet.component';

describe('ProjectTimesheetComponent', () => {
  let component: ProjectTimesheetComponent;
  let fixture: ComponentFixture<ProjectTimesheetComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ProjectTimesheetComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ProjectTimesheetComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
