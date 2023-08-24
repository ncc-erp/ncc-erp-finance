import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { AutoPaymentDebtDialogComponent } from './auto-payment-debt-dialog.component';

describe('AutoPaymentDebtDialogComponent', () => {
  let component: AutoPaymentDebtDialogComponent;
  let fixture: ComponentFixture<AutoPaymentDebtDialogComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ AutoPaymentDebtDialogComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(AutoPaymentDebtDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
