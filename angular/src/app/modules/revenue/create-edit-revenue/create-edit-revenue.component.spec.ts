import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { CreateEditRevenueComponent } from './create-edit-revenue.component';

describe('CreateEditRevenueComponent', () => {
  let component: CreateEditRevenueComponent;
  let fixture: ComponentFixture<CreateEditRevenueComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ CreateEditRevenueComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(CreateEditRevenueComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
