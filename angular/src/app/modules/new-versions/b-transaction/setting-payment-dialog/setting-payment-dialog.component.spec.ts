import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { SettingPaymentDialogComponent } from './setting-payment-dialog.component';

describe('SettingPaymentDialogComponent', () => {
  let component: SettingPaymentDialogComponent;
  let fixture: ComponentFixture<SettingPaymentDialogComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ SettingPaymentDialogComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(SettingPaymentDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
