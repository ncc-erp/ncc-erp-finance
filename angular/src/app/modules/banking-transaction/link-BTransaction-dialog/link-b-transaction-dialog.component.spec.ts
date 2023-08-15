import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { LinkBTransactionDialogComponent } from './link-b-transaction-dialog.component';

describe('PaymentDialogComponent', () => {
  let component: LinkBTransactionDialogComponent;
  let fixture: ComponentFixture<LinkBTransactionDialogComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ LinkBTransactionDialogComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(LinkBTransactionDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
