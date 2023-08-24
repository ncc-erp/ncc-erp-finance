import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { LinkBTransactionMultiOutComingDialogComponent } from './link-b-transaction-multi-out-coming-dialog.component';

describe('LinkBTransactionMultiOutComingDialogComponent', () => {
  let component: LinkBTransactionMultiOutComingDialogComponent;
  let fixture: ComponentFixture<LinkBTransactionMultiOutComingDialogComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ LinkBTransactionMultiOutComingDialogComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(LinkBTransactionMultiOutComingDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
