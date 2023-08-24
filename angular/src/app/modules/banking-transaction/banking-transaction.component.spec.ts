import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { BankingTransactionComponent } from './banking-transaction.component';

describe('BankingTransactionComponent', () => {
  let component: BankingTransactionComponent;
  let fixture: ComponentFixture<BankingTransactionComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ BankingTransactionComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(BankingTransactionComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
