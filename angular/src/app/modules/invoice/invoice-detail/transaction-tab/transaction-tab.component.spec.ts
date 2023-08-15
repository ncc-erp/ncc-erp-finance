import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { TransactionTabComponent } from './transaction-tab.component';

describe('TransactionTabComponent', () => {
  let component: TransactionTabComponent;
  let fixture: ComponentFixture<TransactionTabComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ TransactionTabComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(TransactionTabComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
