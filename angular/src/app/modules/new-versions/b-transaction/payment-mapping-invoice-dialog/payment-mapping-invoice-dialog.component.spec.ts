import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { PaymentMappingInvoiceDialogComponent } from './payment-mapping-invoice-dialog.component';

describe('PaymentMappingInvoiceDialogComponent', () => {
  let component: PaymentMappingInvoiceDialogComponent;
  let fixture: ComponentFixture<PaymentMappingInvoiceDialogComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ PaymentMappingInvoiceDialogComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(PaymentMappingInvoiceDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
