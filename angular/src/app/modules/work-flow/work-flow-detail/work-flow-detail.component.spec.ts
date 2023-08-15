import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { WorkFlowDetailComponent } from './work-flow-detail.component';

describe('WorkFlowDetailComponent', () => {
  let component: WorkFlowDetailComponent;
  let fixture: ComponentFixture<WorkFlowDetailComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ WorkFlowDetailComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(WorkFlowDetailComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
