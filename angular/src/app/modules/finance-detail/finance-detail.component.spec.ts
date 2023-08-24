import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { FinanceDetailComponent } from './finance-detail.component';

describe('FinanceDetailComponent', () => {
  let component: FinanceDetailComponent;
  let fixture: ComponentFixture<FinanceDetailComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ FinanceDetailComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(FinanceDetailComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
