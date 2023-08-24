import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { CreateEditPeriodComponent } from './create-edit-period.component';

describe('CreateEditPeriodComponent', () => {
  let component: CreateEditPeriodComponent;
  let fixture: ComponentFixture<CreateEditPeriodComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ CreateEditPeriodComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(CreateEditPeriodComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
