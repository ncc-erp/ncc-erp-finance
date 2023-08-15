import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { FinanceReviewOldComponent } from './finance-review-old.component';

describe('FinanceReviewOldComponent', () => {
  let component: FinanceReviewOldComponent;
  let fixture: ComponentFixture<FinanceReviewOldComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ FinanceReviewOldComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(FinanceReviewOldComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
