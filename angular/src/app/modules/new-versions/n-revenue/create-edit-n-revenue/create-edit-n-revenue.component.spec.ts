import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { CreateEditNRevenueComponent } from './create-edit-n-revenue.component';

describe('CreateEditNRevenueComponent', () => {
  let component: CreateEditNRevenueComponent;
  let fixture: ComponentFixture<CreateEditNRevenueComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ CreateEditNRevenueComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(CreateEditNRevenueComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
