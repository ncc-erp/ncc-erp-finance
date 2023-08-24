import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { CreateEditWorkFlowComponent } from './create-edit-work-flow.component';

describe('CreateEditWorkFlowComponent', () => {
  let component: CreateEditWorkFlowComponent;
  let fixture: ComponentFixture<CreateEditWorkFlowComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ CreateEditWorkFlowComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(CreateEditWorkFlowComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
