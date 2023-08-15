import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { CreateNRevenueByAccountComponent } from './create-n-revenue-by-account.component';

describe('CreateNRevenueByAccountComponent', () => {
  let component: CreateNRevenueByAccountComponent;
  let fixture: ComponentFixture<CreateNRevenueByAccountComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ CreateNRevenueByAccountComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(CreateNRevenueByAccountComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
