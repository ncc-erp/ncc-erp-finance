import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { EditStatusNRevenueComponent } from './edit-status-n-revenue.component';

describe('EditStatusNRevenueComponent', () => {
  let component: EditStatusNRevenueComponent;
  let fixture: ComponentFixture<EditStatusNRevenueComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ EditStatusNRevenueComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(EditStatusNRevenueComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
